// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Options.Cost;

public class CostGetCommandOptions : BaseCostManagementOptions
{
    [JsonPropertyName(CostGetCommandOptionDefinitions.Type)]
    public string? Type { get; set; }

    [JsonPropertyName(CostGetCommandOptionDefinitions.Timeframe)]
    public string? Timeframe { get; set; }

    [JsonPropertyName(CostGetCommandOptionDefinitions.Granularity)]
    public string? Granularity { get; set; }

    [JsonPropertyName(CostGetCommandOptionDefinitions.FromDate)]
    public DateTime? FromDate { get; set; }

    [JsonPropertyName(CostGetCommandOptionDefinitions.ToDate)]
    public DateTime? ToDate { get; set; }

    [JsonPropertyName(CostGetCommandOptionDefinitions.GroupBy)]
    public string[]? GroupBy { get; set; }

    [JsonPropertyName(CostGetCommandOptionDefinitions.AggregationCostType)]
    public string? AggregationCostType { get; set; }
}
