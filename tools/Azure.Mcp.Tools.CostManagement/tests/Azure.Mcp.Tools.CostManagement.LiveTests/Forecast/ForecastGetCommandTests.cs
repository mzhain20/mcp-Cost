// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.Mcp.Tests;
using Azure.Mcp.Tests.Client;
using Azure.Mcp.Tests.Client.Helpers;
using Xunit;

namespace Azure.Mcp.Tools.CostManagement.LiveTests.Forecast;

public class ForecastGetCommandTests(ITestOutputHelper output) : CommandTestsBase(output)
{
    [Fact]
    public async Task Should_get_forecast_data_by_subscription_id()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionId }
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        // Verify the cost data has basic properties
        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
        // Rows might be empty for some subscriptions, so we just verify it's an array
    }

    [Fact]
    public async Task Should_get_forecast_data_by_subscription_name()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionName }
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_forecast_data_with_custom_date_range()
    {
        // Test with a specific date range (next 7 days)
        var fromDate = DateTime.UtcNow.Date;
        var toDate = DateTime.UtcNow.Date.AddDays(7);

        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "from-date", fromDate.ToString("yyyy-MM-dd") },
                { "to-date", toDate.ToString("yyyy-MM-dd") }
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_forecast_data_with_monthly_granularity()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "granularity", "Monthly" }
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_forecast_data_with_group_by_resource_group()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "group-by", new[] { "ResourceGroup" } }
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_forecast_data_with_amortized_cost_type()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "type", "AmortizedCost" }
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_forecast_data_with_usage_type()
    {
        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "type", "Usage" }
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
    }

    [Fact]
    public async Task Should_get_forecast_data_with_all_parameters()
    {
        // Test with all available parameters to ensure comprehensive functionality
        var fromDate = DateTime.UtcNow.Date;
        var toDate = DateTime.UtcNow.Date.AddDays(7);

        var result = await CallToolAsync(
            "azmcp_costmanagement_forecast_get",
            new()
            {
                { "subscription", Settings.SubscriptionId },
                { "type", "ActualCost" },
                { "granularity", "Daily" },
                { "from-date", fromDate.ToString("yyyy-MM-dd") },
                { "to-date", toDate.ToString("yyyy-MM-dd") },
                { "aggregation-name", "Cost" },
                { "aggregation-function", "Sum" },
                { "include-fresh-partial-cost", false},
                { "include-actual-cost", false}
            });

        var forecastData = result.AssertProperty("CostData");
        Assert.Equal(JsonValueKind.Object, forecastData.ValueKind);

        // Verify the response structure
        var columns = forecastData.GetProperty("columns");
        Assert.Equal(JsonValueKind.Array, columns.ValueKind);
        Assert.NotEmpty(columns.EnumerateArray());

        var rows = forecastData.GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);

        // Verify column structure
        foreach (var column in columns.EnumerateArray())
        {
            Assert.True(column.TryGetProperty("name", out _));
            Assert.True(column.TryGetProperty("type", out _));
        }
    }
}
