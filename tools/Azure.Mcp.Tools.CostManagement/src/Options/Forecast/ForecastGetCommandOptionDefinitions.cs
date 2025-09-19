// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.Mcp.Tools.CostManagement.Options.Forecast;

public static class ForecastGetCommandOptionDefinitions
{
    public const string Type = "type";
    public const string Timeframe = "timeframe";
    public const string Granularity = "granularity";
    public const string FromDate = "from-date";
    public const string ToDate = "to-date";
    public const string AggregationFunction = "aggregation-function";
    public const string AggregationName = "aggregation-name";
    public const string IncludeFreshPartialCost = "include-fresh-partial-cost";
    public const string IncludeActualCost = "include-actual-cost";

    public static readonly Option<string> TypeOption = new(
        $"--{Type}"
    )
    {
        Description = "The type of the query (ActualCost, AmortizedCost, Usage). Determines whether to use 'ActualCost' (default), 'AmortizedCost' or 'Usage' based on user intent.",
        Required = false
    };

    public static readonly Option<string> GranularityOption = new(
        $"--{Granularity}"
    )
    {
        Description = "Granularity groups cost data by time intervals (None, Daily, Monthly). Select based on user intent for trends or totals.",
        Required = false
    };

    public static readonly Option<DateTime?> FromDateOption = new(
        $"--{FromDate}"
    )
    {
        Description = "Start date of the time period. Use YYYY-MM-DDTHH:MM:SS format",
        Required = false
    };

    public static readonly Option<DateTime?> ToDateOption = new(
        $"--{ToDate}"
    )
    {
        Description = "End date of the time period. Use YYYY-MM-DDTHH:MM:SS format",
        Required = false
    };

    public static readonly Option<string> AggregationNameOption = new(
        $"--{AggregationName}"
    )
    {
        Description = "The type of cost to use. This can either be Pre Tax Cost or Total Cost, and it can be either in USD or local currency (PreTaxCost, Cost, PreTaxCostUSD, CostUSD).",
        Required = false
    };

    public static readonly Option<string> AggregationFunctionOption = new(
        $"--{AggregationFunction}"
    )
    {
        Description = "The type of function to use. This can be Sum (default), Count, Min, Max, Avg.",
        Required = false
    };

    public static readonly Option<bool> IncludeFreshPartialCostOption = new(
        $"--{IncludeFreshPartialCost}"
    )
    {
        Description = "A boolean determining if fresh partial cost will be included.",
        Required = false
    };

    public static readonly Option<bool> IncludeActualCostOption = new(
        $"--{IncludeActualCost}"
    )
    {
        Description = "A boolean determining if actual cost will be included.",
        Required = false
    };
}
