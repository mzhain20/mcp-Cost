// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The properties of the query result containing columns and rows.
/// </summary>
public sealed class QueryResultProperties
{
    /// <summary>
    /// The link (url) to the next page of results.
    /// </summary>
    [JsonPropertyName("nextLink")]
    public string? NextLink { get; set; }

    /// <summary>
    /// Array of columns.
    /// </summary>
    [JsonPropertyName("columns")]
    public QueryColumn[]? Columns { get; set; }

    /// <summary>
    /// Array of rows containing the query results.
    /// </summary>
    [JsonPropertyName("rows")]
    public object[][]? Rows { get; set; }
}