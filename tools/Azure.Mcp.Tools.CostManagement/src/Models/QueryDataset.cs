// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The definition of data present in the query.
/// </summary>
public sealed class QueryDataset
{
    /// <summary>
    /// The granularity of rows in the query.
    /// </summary>
    [JsonPropertyName("granularity")]
    public string? Granularity { get; set; }

    /// <summary>
    /// Dictionary of aggregation expression to use in the query.
    /// </summary>
    [JsonPropertyName("aggregation")]
    public Dictionary<string, QueryAggregation>? Aggregation { get; set; }

    /// <summary>
    /// Array of group by expression to use in the query.
    /// </summary>
    [JsonPropertyName("grouping")]
    public QueryGrouping[]? Grouping { get; set; }

    /// <summary>
    /// The filter expression to use in the query.
    /// </summary>
    [JsonPropertyName("filter")]
    public QueryFilter? Filter { get; set; }

    /// <summary>
    /// Has configuration information for the data in the export.
    /// </summary>
    [JsonPropertyName("configuration")]
    public QueryDatasetConfiguration? Configuration { get; set; }
}