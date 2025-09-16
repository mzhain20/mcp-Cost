// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.Mcp.Tools.CostManagement.Options.Cost;

public static class CostGetCommandOptionDefinitions
{
    public const string Type = "type";
    public const string Timeframe = "timeframe";
    public const string Granularity = "granularity";
    public const string FromDate = "from-date";
    public const string ToDate = "to-date";
    public const string GroupBy = "group-by";
    public const string AggregationCostType = "aggregation-cost-type";

    public static readonly Option<string> TypeOption = new(
        $"--{Type}"
    )
    {
        Description = "The type of the query (ActualCost, AmortizedCost). Determines whether to use 'ActualCost' (default) or 'AmortizedCost' based on user intent.",
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

    public static readonly Option<string[]> GroupByOption = new(
        $"--{GroupBy}"
    )
    {
        Description = "Array of column names to group by (e.g., ResourceGroup, ResourceType).",
        Required = false,
        AllowMultipleArgumentsPerToken = true
    };

    public static readonly Option<string> AggregationCostTypeOption = new(
        $"--{AggregationCostType}"
    )
    {
        Description = "The type of cost to use. This can either be Pre Tax Cost or Total Cost, and it can be either in USD or local currency (PreTaxCost, Cost, PreTaxCostUSD, CostUSD).",
        Required = false
    };
}
