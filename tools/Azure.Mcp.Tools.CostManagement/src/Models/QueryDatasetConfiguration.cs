// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The configuration of dataset in the query.
/// </summary>
public sealed class QueryDatasetConfiguration
{
    /// <summary>
    /// Array of column names to be included in the query.
    /// </summary>
    [JsonPropertyName("columns")]
    public string[]? Columns { get; set; }
}