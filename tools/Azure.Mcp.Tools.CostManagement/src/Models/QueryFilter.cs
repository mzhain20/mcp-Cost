// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The filter expression to be used in the export.
/// </summary>
public sealed class QueryFilter
{
    /// <summary>
    /// The logical "AND" expression. Must have at least 2 items.
    /// </summary>
    [JsonPropertyName("and")]
    public QueryFilter[]? And { get; set; }

    /// <summary>
    /// The logical "OR" expression. Must have at least 2 items.
    /// </summary>
    [JsonPropertyName("or")]
    public QueryFilter[]? Or { get; set; }

    /// <summary>
    /// Has comparison expression for a dimension.
    /// </summary>
    [JsonPropertyName("dimensions")]
    public QueryComparisonExpression? Dimensions { get; set; }

    /// <summary>
    /// Has comparison expression for a tag.
    /// </summary>
    [JsonPropertyName("tags")]
    public QueryComparisonExpression? Tags { get; set; }
}