// Live test runs require a resource file, so we use an empty one here.
// CostManagement operations only query existing cost data and don't require additional resources.
targetScope = 'resourceGroup'

@minLength(3)
@maxLength(24)
@description('The base resource name.')
param baseName string

@description('The client OID to grant access to test resources.')
param testApplicationOid string = deployer().objectId

var location string = resourceGroup().location
var tenantId string = subscription().tenantId

// No additional resources needed for CostManagement operations as they only query existing cost data


// Outputs will be available in test-resources-post.ps1
output location string = location

// Their keys will be uppercase
// $DeploymentOutputs.LOCATION
