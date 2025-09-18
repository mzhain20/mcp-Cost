// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Mcp.Core.Options;
using Azure.Mcp.Tools.Aks.Models;

namespace Azure.Mcp.Tools.Aks.Services;

public interface IAksService
{
    Task<List<Cluster>> ListClusters(
        string subscription,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<Cluster?> GetCluster(
        string subscription,
        string clusterName,
        string resourceGroup,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<List<NodePool>> ListNodePools(
        string subscription,
        string resourceGroup,
        string clusterName,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);

    Task<NodePool?> GetNodePool(
        string subscription,
        string resourceGroup,
        string clusterName,
        string nodePoolName,
        string? tenant = null,
        RetryPolicyOptions? retryPolicy = null);
}
