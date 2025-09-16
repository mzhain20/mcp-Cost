// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Mcp.Core.Options;
using Azure.Mcp.Core.Services.Azure;
using Azure.Mcp.Core.Services.Azure.Subscription;
using Azure.Mcp.Core.Services.Azure.Tenant;
using Azure.Mcp.Core.Services.Http;
using Azure.Mcp.Tools.CostManagement.Models;

namespace Azure.Mcp.Tools.CostManagement.Services;

public sealed class CostManagementService(
    ISubscriptionService subscriptionService,
    ITenantService tenantService,
    IHttpClientService httpClientService) : BaseAzureService(tenantService), ICostManagementService
{
    private readonly ISubscriptionService _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    private readonly IHttpClientService _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));

    private const string AzureManagementBaseUrl = "https://management.azure.com";
    private const string CostManagementApiVersion = "2025-03-01";

    public async Task<QueryApiResponse?> QueryCosts(
        string subscription,
        string type,
        string granularity,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string[]? groupBy = null,
        string aggregationCostType = "Cost",
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null)
    {
        ValidateRequiredParameters(subscription, type, granularity);

        try
        {
            var credential = await GetCredential();
            var token = await credential.GetTokenAsync(
                new TokenRequestContext(new[] { $"{AzureManagementBaseUrl}/.default" }),
                CancellationToken.None);

            using var client = _httpClientService.CreateClient(new Uri(AzureManagementBaseUrl));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            // Build the query request
            QueryApiRequest queryRequest = BuildQueryRequest(type, granularity, fromDate, toDate, groupBy, aggregationCostType);

            // Serialize the request body
            var jsonContent = JsonSerializer.Serialize(queryRequest, CostManagementJsonContext.Default.QueryApiRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Make the API call
            var url = $"/subscriptions/{subscription}/providers/Microsoft.CostManagement/query?api-version={CostManagementApiVersion}&top=5000";
            using var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<QueryApiResponse>(responseContent, CostManagementJsonContext.Default.QueryApiResponse);

            return apiResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error querying cost management data: {ex.Message}", ex);
        }
    }

    private static QueryApiRequest BuildQueryRequest(
        string type,
        string granularity,
        DateTime? fromDate,
        DateTime? toDate,
        string[]? groupBy,
        string aggregationCostType)
    {
        // Calculate default time period if not provided (last 30 days)
        var endDate = toDate ?? DateTime.UtcNow.Date;
        var startDate = fromDate ?? endDate.AddDays(-30);

        var request = new QueryApiRequest
        {
            Type = type,
            Timeframe = "Custom",
            Dataset = new QueryDataset
            {
                Granularity = granularity,
                Aggregation = new Dictionary<string, QueryAggregation>
                {
                    ["Cost"] = new QueryAggregation
                    {
                        Name = aggregationCostType,
                        Function = "Sum"
                    }
                },
                Grouping = groupBy?.Select(dimension => new QueryGrouping
                {
                    Type = "Dimension",
                    Name = dimension
                }).ToArray(),
            },
            TimePeriod = new QueryTimePeriod
            {
                From = startDate,
                To = endDate
            }
        };

        return request;
    }
}
