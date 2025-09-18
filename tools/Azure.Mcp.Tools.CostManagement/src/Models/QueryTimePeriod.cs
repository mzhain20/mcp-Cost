// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The start and end date for pulling data for the query.
/// </summary>
public sealed class QueryTimePeriod
{
    /// <summary>
    /// The start date to pull data from.
    /// </summary>
    [JsonPropertyName("from")]
    public DateTime From { get; set; }

    /// <summary>
    /// The end date to pull data to.
    /// </summary>
    [JsonPropertyName("to")]
    public DateTime To { get; set; }
}