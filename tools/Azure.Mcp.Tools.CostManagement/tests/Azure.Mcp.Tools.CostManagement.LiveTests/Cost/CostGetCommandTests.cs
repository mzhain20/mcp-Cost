// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.Mcp.Tests;
using Azure.Mcp.Tests.Client;
using Azure.Mcp.Tests.Client.Helpers;
using Xunit;

namespace Azure.Mcp.Tools.CostManagement.LiveTests.Cost;

public class CostGetCommandTests(ITestOutputHelper output) : CommandTestsBase(output)
{
    [Fact]
    public async Task Should_get_cost_data_by_subscription_id()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        // Verify the cost data has basic properties
        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
        // Rows might be empty for some subscriptions, so we just verify it's an array
    }

    [Fact]
    public async Task Should_get_cost_data_by_subscription_name()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionName }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_cost_data_with_custom_date_range()
    {
        // Test with a specific date range (last 30 days)
        var toDate = DateTime.UtcNow.Date;
        var fromDate = toDate.AddDays(-30);

        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "from-date", fromDate.ToString("yyyy-MM-dd") },
                { "to-date", toDate.ToString("yyyy-MM-dd") }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_cost_data_with_monthly_granularity()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "granularity", "Monthly" }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_cost_data_with_group_by_resource_group()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "group-by", new[] { "ResourceGroup" } }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_cost_data_with_amortized_cost_type()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "type", "AmortizedCost" }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_cost_data_with_multiple_group_by_dimensions()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "group-by", new[] { "ResourceGroup", "ServiceName" } }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_cost_data_with_usage_type()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "type", "Usage" }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_cost_data_with_all_parameters()
    {
        // Test with all available parameters to ensure comprehensive functionality
        var toDate = DateTime.UtcNow.Date;
        var fromDate = toDate.AddDays(-7); // Use smaller range for faster test

        var result = await CallToolAsync(
            "azmcp_costmanagement_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "type", "ActualCost" },
                { "granularity", "Daily" },
                { "from-date", fromDate.ToString("2025-05-01") },
                { "to-date", toDate.ToString("2025-06-30") },
                { "group-by", new[] { "ResourceGroup" } },
                { "aggregation-cost-type", "Cost" }
            });

        var costData = result.AssertProperty("costData");
        Assert.Equal(JsonValueKind.Object, costData.ValueKind);

        // Verify the response structure
        var columns = costData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = costData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);

        // Verify column structure
        foreach (var column in columns.EnumerateArray())
        {
            Assert.True(column.TryGetProperty("name", out _));
            Assert.True(column.TryGetProperty("type", out _));
        }
    }
}