// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Options;
using Azure.Mcp.Tools.CostManagement.Models;

namespace Azure.Mcp.Tools.CostManagement.Services;

public interface ICostManagementService
{
    /// <summary>
    /// Query Azure Cost Management data for usage and cost information.
    /// </summary>
    /// <param name="subscription">The subscription ID to query.</param>
    /// <param name="type">The type of cost data to query (ActualCost, AmortizedCost, Usage).</param>
    /// <param name="granularity">The granularity of the data (Daily, Monthly).</param>
    /// <param name="fromDate">Start date for the query period.</param>
    /// <param name="toDate">End date for the query period.</param>
    /// <param name="groupBy">Dimensions to group the data by.</param>
    /// <param name="aggregationCostType">The cost type for aggregation (Cost, PreTaxCost, etc.).</param>
    /// <param name="tenant">Optional tenant ID.</param>
    /// <param name="retryPolicy">Optional retry policy configuration.</param>
    /// <returns>Query results containing cost and usage data.</returns>
    Task<QueryApiResponse?> QueryCosts(
        string subscription,
        string type,
        string granularity,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string[]? groupBy = null,
        string aggregationCostType = "Cost",
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<QueryApiResponse?> QueryForecast(
        string subscription,
        string type,
        string granularity,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string aggregationCostType = "Cost",
        bool includeActualCost = false,
        bool includeFreshPartialCost = false,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);
}
