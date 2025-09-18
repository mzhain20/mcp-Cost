// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.Mcp.Tools.CostManagement.Models;

/// <summary>
/// Constants for Cost Management API values.
/// </summary>
public static class CostManagementConstants
{
    /// <summary>
    /// Export/Query types.
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
    }

    /// <summary>
    /// Function types for aggregation.
    /// </summary>
    public static class FunctionType
    {
        public const string Sum = "Sum";
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