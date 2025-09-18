// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Commands;
using Azure.Mcp.Tools.Monitor.Options;
using Azure.Mcp.Tools.Monitor.Services;
using Microsoft.Extensions.Logging;

namespace Azure.Mcp.Tools.Monitor.Commands.Table;

public sealed class TableListCommand(ILogger<TableListCommand> logger) : BaseWorkspaceMonitorCommand<TableListOptions>()
{
    private const string CommandTitle = "List Log Analytics Tables";
    private readonly ILogger<TableListCommand> _logger = logger;

    public override string Name => "list";

    public override string Description =>
        $"""
        List all tables in a Log Analytics workspace. Requires {WorkspaceOptionDefinitions.WorkspaceIdOrName}.
        Returns table names and schemas that can be used for constructing KQL queries.
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
        command.Options.Add(MonitorOptionDefinitions.TableType);
    }

    protected override TableListOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.TableType = parseResult.GetValueOrDefault<string>(MonitorOptionDefinitions.TableType.Name);
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
            var monitorService = context.GetService<IMonitorService>();
            var tables = await monitorService.ListTables(
                options.Subscription!,
                options.ResourceGroup!,
                options.Workspace!,
                options.TableType,
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = ResponseResult.Create(new(tables ?? []), MonitorJsonContext.Default.TableListCommandResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing tables.");
            HandleException(context, ex);
        }

        return context.Response;
    }

    internal record TableListCommandResult(List<string> Tables);
}
