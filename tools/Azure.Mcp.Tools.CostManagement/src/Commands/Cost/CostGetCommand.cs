// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Core.Commands.Subscription;
using Azure.Mcp.Core.Extensions;
using Azure.Mcp.Core.Models.Option;
using Azure.Mcp.Tools.CostManagement.Models;
using Azure.Mcp.Tools.CostManagement.Options.Cost;
using Azure.Mcp.Tools.CostManagement.Services;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Azure.Mcp.Tools.CostManagement.Commands.Cost;

public sealed class CostGetCommand(ILogger<CostGetCommand> logger) : SubscriptionCommand<CostGetCommandOptions>
{
    private const string CommandTitle = "Get Azure Cost Management Query";
    private readonly ILogger<CostGetCommand> _logger = logger;

    // Define options from OptionDefinitions
    private readonly Option<string> _typeOption = CostGetCommandOptionDefinitions.TypeOption;
    private readonly Option<string> _granularityOption = CostGetCommandOptionDefinitions.GranularityOption;
    private readonly Option<DateTime?> _fromDateOption = CostGetCommandOptionDefinitions.FromDateOption;
    private readonly Option<DateTime?> _toDateOption = CostGetCommandOptionDefinitions.ToDateOption;
    private readonly Option<string[]> _groupByOption = CostGetCommandOptionDefinitions.GroupByOption;
    private readonly Option<string> _aggregationCostTypeOption = CostGetCommandOptionDefinitions.AggregationCostTypeOption;

    public override string Name => "get";

    public override string Description =>
        """
        Query Azure Cost Management data for usage and cost information. 
        Retrieves cost data for a subscription with flexible filtering, grouping, and time period options.
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
        command.Options.Add(_groupByOption);
        command.Options.Add(_aggregationCostTypeOption);
    }

    protected override CostGetCommandOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.Type = parseResult.GetValueOrDefault(_typeOption);
        options.Granularity = parseResult.GetValueOrDefault(_granularityOption);
        options.FromDate = parseResult.GetValueOrDefault(_fromDateOption);
        options.ToDate = parseResult.GetValueOrDefault(_toDateOption);
        options.GroupBy = parseResult.GetValueOrDefault(_groupByOption);
        options.AggregationCostType = parseResult.GetValueOrDefault(_aggregationCostTypeOption);
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
            var result = await costManagementService.QueryCosts(
                options.Subscription!,
                options.Type ?? CostManagementConstants.ExportType.ActualCost,
                options.Granularity ?? CostManagementConstants.GranularityType.Daily,
                options.FromDate,
                options.ToDate,
                options.GroupBy,
                options.AggregationCostType ?? "Cost",
                options.Tenant,
                options.RetryPolicy);

            // Set results
            context.Response.Results = result != null ?
                ResponseResult.Create(
                    new CostGetCommandResult(result),
                    CostManagementJsonContext.Default.CostGetCommandResult) :
                null;
        }
        catch (Exception ex)
        {
            // Log error with all relevant context
            _logger.LogError(ex,
                "Error querying cost management data. Subscription: {Subscription}, Type: {Type}, Options: {@Options}",
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
    internal record CostGetCommandResult(QueryApiResponse CostData);
}
