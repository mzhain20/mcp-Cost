// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Options;
using Azure.Mcp.Core.Services.Azure;
using Azure.Mcp.Core.Services.Azure.Subscription;
using Azure.Mcp.Core.Services.Caching;
using Azure.Mcp.Tools.Search.Commands;
using Azure.Mcp.Tools.Search.Models;
using Azure.ResourceManager.Search;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace Azure.Mcp.Tools.Search.Services;

public sealed class SearchService(ISubscriptionService subscriptionService, ICacheService cacheService) : BaseAzureService, ISearchService
{
    private readonly ISubscriptionService _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private const string CacheGroup = "search";
    private const string SearchServicesCacheKey = "services";
    private static readonly TimeSpan s_cacheDurationServices = TimeSpan.FromHours(1);
    private static readonly TimeSpan s_cacheDurationClients = TimeSpan.FromMinutes(15);

    public async Task<List<string>> ListServices(
        string subscription,
        string? tenantId = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription);

        var cacheKey = string.IsNullOrEmpty(tenantId)
            ? $"{SearchServicesCacheKey}_{subscription}"
            : $"{SearchServicesCacheKey}_{subscription}_{tenantId}";

        var cachedServices = await _cacheService.GetAsync<List<string>>(CacheGroup, cacheKey, s_cacheDurationServices);
        if (cachedServices != null)
        {
            return cachedServices;
        }

        var subscriptionResource = await _subscriptionService.GetSubscription(subscription, tenantId, retryPolicy);
        var services = new List<string>();
        try
        {
            await foreach (var service in subscriptionResource.GetSearchServicesAsync())
            {
                if (service?.Data?.Name != null)
                {
                    services.Add(service.Data.Name);
                }
            }

            await _cacheService.SetAsync(CacheGroup, cacheKey, services, s_cacheDurationServices);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving Search services: {ex.Message}", ex);
        }

        return services;
    }

    public async Task<List<IndexInfo>> GetIndexDetails(
        string serviceName,
        string? indexName,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(serviceName);

        var indexes = new List<IndexInfo>();

        if (string.IsNullOrEmpty(indexName))
        {
            try
            {
                var searchClient = await GetSearchIndexClient(serviceName, retryPolicy);
                await foreach (var index in searchClient.GetIndexesAsync())
                {
                    indexes.Add(MapToIndexInfo(index));
                }
                return indexes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Search indexes: {ex.Message}", ex);
            }
        }
        else
        {
            try
            {
                var searchClient = await GetSearchIndexClient(serviceName, retryPolicy);
                var index = await searchClient.GetIndexAsync(indexName);

                indexes.Add(MapToIndexInfo(index.Value));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving Search index details: {ex.Message}", ex);
            }
        }

        return indexes;
    }

    public async Task<List<JsonElement>> QueryIndex(
        string serviceName,
        string indexName,
        string searchText,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(serviceName, indexName, searchText);

        try
        {
            var searchClient = await GetSearchIndexClient(serviceName, retryPolicy);
            var indexDefinition = await searchClient.GetIndexAsync(indexName);
            var client = searchClient.GetSearchClient(indexName);

            var options = new SearchOptions
            {
                IncludeTotalCount = true,
                Size = 20
            };

            var vectorFields = FindVectorFields(indexDefinition.Value);
            var vectorizableFields = FindVectorizableFields(indexDefinition.Value, vectorFields);
            ConfigureSearchOptions(searchText, options, indexDefinition.Value, vectorFields);

            var searchResponse = await client.SearchAsync(searchText, SearchJsonContext.Default.JsonElement, options);

            return await ProcessSearchResults(searchResponse);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error querying Search index: {ex.Message}", ex);
        }
    }

    private static List<string> FindVectorFields(SearchIndex indexDefinition)
    {
        return [.. indexDefinition.Fields
                    .Where(f => f.VectorSearchDimensions.HasValue)
                    .Select(f => f.Name)];
    }

    private static List<string> FindVectorizableFields(SearchIndex indexDefinition, List<string> vectorFields)
    {
        var vectorizableFields = new List<string>();

        if (indexDefinition.VectorSearch?.Profiles == null || indexDefinition.VectorSearch.Algorithms == null)
        {
            return vectorizableFields;
        }

        foreach (var field in indexDefinition.Fields)
        {
            if (vectorFields.Contains(field.Name) && !string.IsNullOrEmpty(field.VectorSearchProfileName))
            {
                var profile = indexDefinition.VectorSearch.Profiles
                    .FirstOrDefault(p => p.Name == field.VectorSearchProfileName);

                if (profile != null)
                {
                    if (!string.IsNullOrEmpty(profile.VectorizerName))
                    {
                        vectorizableFields.Add(field.Name);
                    }
                }
            }
        }

        return vectorizableFields;
    }

    private async Task<SearchIndexClient> GetSearchIndexClient(string serviceName, RetryPolicyOptions? retryPolicy)
    {
        var key = $"{SearchServicesCacheKey}_{serviceName}";
        var searchClient = await _cacheService.GetAsync<SearchIndexClient>(CacheGroup, key, s_cacheDurationClients);
        if (searchClient == null)
        {
            var credential = await GetCredential();

            var clientOptions = AddDefaultPolicies(new SearchClientOptions());
            ConfigureRetryPolicy(clientOptions, retryPolicy);

            var endpoint = new Uri($"https://{serviceName}.search.windows.net");
            searchClient = new SearchIndexClient(endpoint, credential, clientOptions);
            await _cacheService.SetAsync(CacheGroup, key, searchClient, s_cacheDurationClients);
        }
        return searchClient;
    }

    private static void ConfigureSearchOptions(string q, SearchOptions options, SearchIndex indexDefinition, List<string> vectorFields)
    {
        List<string> selectedFields = [.. indexDefinition.Fields
                                                         .Where(f => f.IsHidden == false && !vectorFields.Contains(f.Name))
                                                         .Select(f => f.Name)];
        foreach (var field in selectedFields)
        {
            options.Select.Add(field);
        }

        options.VectorSearch = new VectorSearchOptions();
        foreach (var vf in vectorFields)
        {
            options.VectorSearch.Queries.Add(new VectorizableTextQuery(q) { Fields = { vf }, KNearestNeighborsCount = 50 });
        }
    }

    private static async Task<List<JsonElement>> ProcessSearchResults(Response<SearchResults<JsonElement>> searchResponse)
    {
        var results = new List<JsonElement>();
        await foreach (var result in searchResponse.Value.GetResultsAsync())
        {
            results.Add(result.Document);
        }
        return results;
    }

    private static void ConfigureRetryPolicy(SearchClientOptions options, RetryPolicyOptions? retryPolicy)
    {
        if (retryPolicy != null)
        {
            options.Retry.MaxRetries = retryPolicy.MaxRetries;
            options.Retry.Mode = retryPolicy.Mode;
            options.Retry.Delay = TimeSpan.FromSeconds(retryPolicy.DelaySeconds);
            options.Retry.MaxDelay = TimeSpan.FromSeconds(retryPolicy.MaxDelaySeconds);
            options.Retry.NetworkTimeout = TimeSpan.FromSeconds(retryPolicy.NetworkTimeoutSeconds);
        }
    }

    private static IndexInfo MapToIndexInfo(SearchIndex index)
        => new(index.Name, index.Description, [.. index.Fields.Select(MapToFieldInfo)]);

    private static FieldInfo MapToFieldInfo(SearchField field)
        => new(field.Name, field.Type.ToString(), field.IsKey, field.IsSearchable, field.IsFilterable, field.IsSortable,
            field.IsFacetable, field.IsHidden != true);
}
