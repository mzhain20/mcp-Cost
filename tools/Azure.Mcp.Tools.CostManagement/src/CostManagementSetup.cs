// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Areas;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Tools.CostManagement.Commands.Cost;
using Azure.Mcp.Tools.CostManagement.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azure.Mcp.Tools.CostManagement;

public class CostManagementSetup : IAreaSetup
{
    public string Name => "costmanagement";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICostManagementService, CostManagementService>();
    }

    public void RegisterCommands(CommandGroup rootGroup, ILoggerFactory loggerFactory)
    {
        var costManagement = new CommandGroup(Name, "Cost Management operations - Commands for querying Azure Cost Management data for usage and cost information. Allows filtering by from and to date, time granularity, and grouping by Azure dimensions.");
        rootGroup.AddSubGroup(costManagement);

        costManagement.AddCommand("get", new CostGetCommand(
            loggerFactory.CreateLogger<CostGetCommand>()));
    }
}
