param(
    [string] $TenantId,
    [string] $TestApplicationId,
    [string] $ResourceGroupName,
    [string] $BaseName,
    [hashtable] $DeploymentOutputs
)

$ErrorActionPreference = "Stop"

. "$PSScriptRoot/../../../eng/common/scripts/common.ps1"
. "$PSScriptRoot/../../../eng/scripts/helpers/TestResourcesHelpers.ps1"

# Create test settings file for CostManagement live tests
New-TestSettings @PSBoundParameters -OutputPath $PSScriptRoot

# CostManagement operations only query existing cost data, so no additional setup is needed
Write-Host "CostManagement test resources setup complete. No additional resources required." -ForegroundColor Green
