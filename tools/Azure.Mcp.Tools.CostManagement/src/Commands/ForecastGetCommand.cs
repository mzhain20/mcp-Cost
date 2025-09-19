// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Core.Commands.Subscription;
using Azure.Mcp.Core.Extensions;
using Azure.Mcp.Core.Models.Option;
using Azure.Mcp.Tools.CostManagement.Models;
using Azure.Mcp.Tools.CostManagement.Options.Forecast;
using Azure.Mcp.Tools.CostManagement.Services;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Azure.Mcp.Tools.CostManagement.Commands;

public sealed class ForecastGetCommand(ILogger<ForecastGetCommand> logger) : SubscriptionCommand<ForecastGetCommandOptions>
{
    private const string CommandTitle = "Get Azure Cost Management Forecast";
    private readonly ILogger<ForecastGetCommand> _logger = logger;

    // Define options from OptionDefinitions
    private readonly Option<string> _typeOption = ForecastGetCommandOptionDefinitions.TypeOption;
    private readonly Option<string> _granularityOption = ForecastGetCommandOptionDefinitions.GranularityOption;
    private readonly Option<DateTime?> _fromDateOption = ForecastGetCommandOptionDefinitions.FromDateOption;
    private readonly Option<DateTime?> _toDateOption = ForecastGetCommandOptionDefinitions.ToDateOption;
    private readonly Option<string> _aggregationFunctionOption = ForecastGetCommandOptionDefinitions.AggregationFunctionOption;
    private readonly Option<string> _aggregationNameOption = ForecastGetCommandOptionDefinitions.AggregationNameOption;
    private readonly Option<bool> _includeFreshPartialCostOption = ForecastGetCommandOptionDefinitions.IncludeFreshPartialCostOption;
    private readonly Option<bool> _includeActualCostOption = ForecastGetCommandOptionDefinitions.IncludeActualCostOption;

    public override string Name => "get";

    public override string Description =>
        """
        Gives Azure Cost Management forecasted data for usage and cost information.
        Retrieves cost data for a subscription with flexible filtering, and time period options.
        """;

    public override string Title => CommandTitle;

    public override ToolMetadata Metadata => new()
    {
        Destructive = false,
        Idempotent = true,
        OpenWorld = true,
        ReadOnly = true,
        LocalRequired = false,
        Secret = false
    };

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.Options.Add(_typeOption);
        command.Options.Add(_granularityOption);
        command.Options.Add(_fromDateOption);
        command.Options.Add(_toDateOption);
        command.Options.Add(_aggregationFunctionOption);
        command.Options.Add(_aggregationNameOption);
        command.Options.Add(_includeActualCostOption);
        command.Options.Add(_includeFreshPartialCostOption);
    }

    protected override CostGetCommandOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.Type = parseResult.GetValueOrDefault(_typeOption);
        options.Granularity = parseResult.GetValueOrDefault(_granularityOption);
        options.FromDate = parseResult.GetValueOrDefault(_fromDateOption);
        options.ToDate = parseResult.GetValueOrDefault(_toDateOption);
        options.AggregationName = parseResult.GetValueOrDefault(_aggregationNameOption);
        options.AggregationFunction = parseResult.GetValueOrDefault(_aggregationFunctionOption);
        options.IncludeActualCost = parseResult.GetValueOrDefault(_includeActualCostOption);
        options.IncludeFreshPartialCost = parseResult.GetValueOrDefault(_includeFreshPartialCostOption);
        return options;
    }

    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        if (!Validate(parseResult.CommandResult, context.Response).IsValid)
        {
            return context.Response;
        }

        var options = BindOptions(parseResult);

        try
        {
            // Get the cost management service from DI
            var costManagementService = context.GetService<ICostManagementService>();

            // Call service operation with required parameters
            var result = await costManagementService.QueryForecast(
                options.Subscription!,
                options.Type ?? CostManagementConstants.ExportType.ActualCost,
                options.Granularity ?? CostManagementConstants.GranularityType.Daily,
                options.FromDate,
                options.ToDate,
                options.AggregationName ?? "Cost",
                options.AggregationFunction ?? "Sum",
                options.IncludeActualCost ?? false,
                options.IncludeFreshPartialCost ?? false,
                options.Tenant,
                options.RetryPolicy);

            // Set results
            context.Response.Results = result != null ?
                ResponseResult.Create(
                    new ForecastGetCommandResult(result),
                    CostManagementJsonContext.Default.ForecastGetCommandResult) :
                null;
        }
        catch (Exception ex)
        {
            // Log error with all relevant context
            _logger.LogError(ex,
                "Error getting cost management forecast data. Subscription: {Subscription}, Type: {Type}, Options: {@Options}",
                options.Subscription, options.Type, options);
            HandleException(context, ex);
        }

        return context.Response;
    }

    // Implementation-specific error handling
    protected override string GetErrorMessage(Exception ex) => ex switch
    {
        RequestFailedException reqEx when reqEx.Status == 404 =>
            "Cost data not found. Verify the subscription exists and you have access to cost management data.",
        RequestFailedException reqEx when reqEx.Status == 403 =>
            $"Authorization failed accessing cost management data. Details: {reqEx.Message}",
        RequestFailedException reqEx when reqEx.Status == 429 =>
            "Request throttled. Cost management API has rate limits. Please wait and retry.",
        RequestFailedException reqEx =>
            $"Cost management service error. Details: {reqEx.Message}",
        ArgumentException argEx =>
            $"Invalid parameter provided. Details: {argEx.Message}",
        _ => base.GetErrorMessage(ex)
    };

    protected override int GetStatusCode(Exception ex) => ex switch
    {
        RequestFailedException reqEx => reqEx.Status,
        ArgumentException => 400,
        _ => base.GetStatusCode(ex)
    };

    // Strongly-typed result record
    internal record ForecastGetCommandResult(QueryApiResponse CostData);
}
