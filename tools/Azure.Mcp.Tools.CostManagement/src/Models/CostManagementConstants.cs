// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// Constants for Cost Management API values.
/// </summary>
public static class CostManagementConstants
{
    /// <summary>
    /// Export/Query/Forecast types.
    /// </summary>
    public static class ExportType
    {
        public const string Usage = "Usage";
        public const string ActualCost = "ActualCost";
        public const string AmortizedCost = "AmortizedCost";
    }

    /// <summary>
    /// Timeframe types.
    /// </summary>
    public static class TimeframeType
    {
        public const string MonthToDate = "MonthToDate";
        public const string BillingMonthToDate = "BillingMonthToDate";
        public const string TheLastMonth = "TheLastMonth";
        public const string TheLastBillingMonth = "TheLastBillingMonth";
        public const string WeekToDate = "WeekToDate";
        public const string Custom = "Custom";
    }

    /// <summary>
    /// Granularity types.
    /// </summary>
    public static class GranularityType
    {
        public const string Daily = "Daily";
        public const string None = "None";
        public const string Monthly = "Monthly";
    }

    /// <summary>
    /// Function types for aggregation.
    /// </summary>
    public static class FunctionType
    {
        public const string Sum = "Sum";
        public const string Average = "Avg";
        public const string Count = "Count";
        public const string Min = "Min";
        public const string Max = "Max";
    }

    public static class FunctionName
    {
        public const string Cost = "Cost";
        public const string CostUSD = "CostUSD";
        public const string PreTaxCost = "PreTaxCost";
        public const string PreTaxCostUSD = "PreTaxCostUSD";
    }

    /// <summary>
    /// Query operators.
    /// </summary>
    public static class QueryOperatorType
    {
        public const string In = "In";
    }

    /// <summary>
    /// Query column types.
    /// </summary>
    public static class QueryColumnType
    {
        public const string TagKey = "TagKey";
        public const string Dimension = "Dimension";
    }
}
