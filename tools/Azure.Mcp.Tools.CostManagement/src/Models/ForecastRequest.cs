// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// The definition of a query request to the Cost Management API.
/// </summary>
public class ForecastRequest : QueryApiRequest
{
    /// <summary>
    /// A boolean determining if actualCost will be included.
    /// </summary>
    [JsonPropertyName("includeActualCost")]
    public bool IncludeActualCost { get; set; }

    /// <summary>
    /// A boolean determining if fresh partial cost will be included.
    /// </summary>
    [JsonPropertyName("includeFreshPartialCost")]
    public bool IncludeFreshPartialCost { get; set; }
}
