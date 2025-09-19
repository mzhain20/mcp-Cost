// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Options.Forecast;

public class ForecastGetCommandOptions : BaseCostManagementOptions
{
    [JsonPropertyName(ForecastGetCommandOptionDefinitions.Type)]
    public string? Type { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.Timeframe)]
    public string? Timeframe { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.Granularity)]
    public string? Granularity { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.FromDate)]
    public DateTime? FromDate { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.ToDate)]
    public DateTime? ToDate { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.AggregationName)]
    public string? AggregationName { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.AggregationFunction)]
    public string? AggregationFunction { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.IncludeFreshPartialCost)]
    public bool? IncludeFreshPartialCost { get; set; }

    [JsonPropertyName(ForecastGetCommandOptionDefinitions.IncludeActualCost)]
    public bool? IncludeActualCost { get; set; }
}
