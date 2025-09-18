// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Azure.Mcp.Tools.AppConfig.Commands.Account;
using Azure.Mcp.Tools.AppConfig.Commands.KeyValue;
using Azure.Mcp.Tools.AppConfig.Commands.KeyValue.Lock;

namespace Azure.Mcp.Tools.AppConfig.Commands;

[JsonSerializable(typeof(AccountListCommand.AccountListCommandResult))]
[JsonSerializable(typeof(KeyValueDeleteCommand.KeyValueDeleteCommandResult))]
[JsonSerializable(typeof(KeyValueListCommand.KeyValueListCommandResult))]
[JsonSerializable(typeof(KeyValueLockSetCommand.KeyValueLockSetCommandResult))]
[JsonSerializable(typeof(KeyValueShowCommand.KeyValueShowResult))]
[JsonSerializable(typeof(KeyValueSetCommand.KeyValueSetCommandResult))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class AppConfigJsonContext : JsonSerializerContext
{
}
