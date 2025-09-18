// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Commands;
using Azure.Mcp.Core.Extensions;
using Azure.Mcp.Tools.Kusto.Options;
using Azure.Mcp.Tools.Kusto.Services;
using Microsoft.Extensions.Logging;

namespace Azure.Mcp.Tools.Kusto.Commands;

public sealed class QueryCommand(ILogger<QueryCommand> logger) : BaseDatabaseCommand<QueryOptions>()
{
    private const string CommandTitle = "Query Kusto Database";
    private readonly ILogger<QueryCommand> _logger = logger;

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.Options.Add(KustoOptionDefinitions.Query);
    }

    protected override QueryOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.Query = parseResult.GetValueOrDefault<string>(KustoOptionDefinitions.Query.Name);
        return options;
    }

    public override string Name => "query";

    public override string Description =>
        """
        Execute a KQL against items in a Kusto cluster.
        Requires `cluster-uri` (or `cluster` and `subscription`), `database`, and `query`.
        Results are returned as a JSON array of documents, for example: `[{'Column1': val1, 'Column2': val2}, ...]`.
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

    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        if (!Validate(parseResult.CommandResult, context.Response).IsValid)
        {
            return context.Response;
        }

        var options = BindOptions(parseResult);

        try
        {
            List<JsonElement> results = [];
            var kusto = context.GetService<IKustoService>();

            if (UseClusterUri(options))
            {
                results = await kusto.QueryItems(
                    options.ClusterUri!,
                    options.Database!,
                    options.Query!,
                    options.Tenant,
                    options.AuthMethod,
                    options.RetryPolicy);
            }
            else
            {
                results = await kusto.QueryItems(
                    options.Subscription!,
                    options.ClusterName!,
                    options.Database!,
                    options.Query!,
                    options.Tenant,
                    options.AuthMethod,
                    options.RetryPolicy);
            }

            context.Response.Results = ResponseResult.Create(new(results ?? []), KustoJsonContext.Default.QueryCommandResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred querying Kusto. Cluster: {Cluster}, Database: {Database},"
            + " Query: {Query}", options.ClusterUri ?? options.ClusterName, options.Database, options.Query);
            HandleException(context, ex);
        }
        return context.Response;
    }

    internal record QueryCommandResult(List<JsonElement> Items);
}
