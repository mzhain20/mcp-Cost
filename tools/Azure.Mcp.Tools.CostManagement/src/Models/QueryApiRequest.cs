// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The definition of a query request to the Cost Management API.
/// </summary>
public class QueryApiRequest
{
    /// <summary>
    /// The type of the query.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The time frame for pulling data for the query.
    /// </summary>
    [JsonPropertyName("timeframe")]
    public string Timeframe { get; set; } = string.Empty;

    /// <summary>
    /// Has definition for data in this query.
    /// </summary>
    [JsonPropertyName("dataset")]
    public QueryDataset? Dataset { get; set; }

    /// <summary>
    /// Has time period for pulling data for the query.
    /// </summary>
    [JsonPropertyName("timePeriod")]
    public QueryTimePeriod? TimePeriod { get; set; }
}
