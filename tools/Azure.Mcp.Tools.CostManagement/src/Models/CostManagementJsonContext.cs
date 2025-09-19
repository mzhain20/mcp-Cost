// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Tools.CostManagement.Commands;
using Azure.Mcp.Tools.CostManagement.Commands.Cost;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Azure.Mcp.Tools.CostManagement.Models;

[JsonSerializable(typeof(ForecastRequest))]
[JsonSerializable(typeof(QueryApiRequest))]
[JsonSerializable(typeof(QueryApiResponse))]
[JsonSerializable(typeof(QueryDataset))]
[JsonSerializable(typeof(QueryAggregation))]
[JsonSerializable(typeof(QueryGrouping))]
[JsonSerializable(typeof(QueryFilter))]
[JsonSerializable(typeof(QueryComparisonExpression))]
[JsonSerializable(typeof(QueryDatasetConfiguration))]
[JsonSerializable(typeof(QueryTimePeriod))]
[JsonSerializable(typeof(QueryResultProperties))]
[JsonSerializable(typeof(QueryColumn))]
[JsonSerializable(typeof(CostGetCommand.CostGetCommandResult))]
[JsonSerializable(typeof(ForecastGetCommand.ForecastGetCommandResult))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(object))]
internal partial class CostManagementJsonContext : JsonSerializerContext
{
}
