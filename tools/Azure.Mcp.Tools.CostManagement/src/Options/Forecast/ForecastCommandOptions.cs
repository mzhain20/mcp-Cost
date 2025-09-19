// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Options.Forecast;

public class ForecastCommandOptions : BaseCostManagementOptions
{
    [JsonPropertyName(ForecastCommandOptionDefinitions.Type)]
    public string? Type { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.Timeframe)]
    public string? Timeframe { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.Granularity)]
    public string? Granularity { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.FromDate)]
    public DateTime? FromDate { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.ToDate)]
    public DateTime? ToDate { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.AggregationName)]
    public string? AggregationName { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.AggregationFunction)]
    public string? AggregationFunction { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.IncludeFreshPartialCost)]
    public bool? IncludeFreshPartialCost { get; set; }

    [JsonPropertyName(ForecastCommandOptionDefinitions.IncludeActualCost)]
    public bool? IncludeActualCost { get; set; }
}
