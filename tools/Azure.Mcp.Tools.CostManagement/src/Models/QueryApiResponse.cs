// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// Result of query. It contains all columns listed under groupings and aggregation.
/// </summary>
public sealed class QueryApiResponse
{
    /// <summary>
    /// Resource Id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Resource name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Resource type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// ETag of the resource.
    /// </summary>
    [JsonPropertyName("eTag")]
    public string? ETag { get; set; }

    /// <summary>
    /// Location of the resource.
    /// </summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>
    /// SKU of the resource.
    /// </summary>
    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    /// <summary>
    /// Resource tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public Dictionary<string, string>? Tags { get; set; }

    /// <summary>
    /// The properties of the query result.
    /// </summary>
    [JsonPropertyName("properties")]
    public QueryResultProperties? Properties { get; set; }
}