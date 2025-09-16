// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The comparison expression to be used in the query.
/// </summary>
public sealed class QueryComparisonExpression
{
    /// <summary>
    /// The name of the column to use in comparison.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The operator to use for comparison.
    /// </summary>
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    /// Array of values to use for comparison.
    /// </summary>
    [JsonPropertyName("values")]
    public string[] Values { get; set; } = [];
}