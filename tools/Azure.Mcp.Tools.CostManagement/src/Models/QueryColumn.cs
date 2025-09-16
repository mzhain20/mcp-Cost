// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// QueryColumn properties.
/// </summary>
public sealed class QueryColumn
{
    /// <summary>
    /// The name of column.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The type of column.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}