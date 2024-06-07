@description('The location into which your Azure resources should be deployed.')
param location string = resourceGroup().location

@description('Select the type of environment you want to provision. Allowed values are Production, Staging, and Development.')
@allowed([
  'Development'
])
param environmentName string

@description('A unique suffix to add to resource names that need to be globally unique.')
@maxLength(13)
param resourceNameSuffix string = uniqueString(resourceGroup().id)

@description('The name of the project.')
param projectName string

var environmentConfigurationMap = {
  Development: {
    environmentAbbreviation: 'dev'
    appServicePlan: {
      sku: {
        name: 'F1'
      }
    }
  }
}

// Define the names for resources.
var environmentAbbreviation = environmentConfigurationMap[environmentName].environmentAbbreviation
var appServiceAppName = 'as-${projectName}-${resourceNameSuffix}-${environmentAbbreviation}'
var appServicePlanName = 'plan-${projectName}-${environmentAbbreviation}'

// Define the SKUs for each component based on the environment type.
var appServicePlanSku = environmentConfigurationMap[environmentName].appServicePlan.sku

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: appServicePlanName
  location: location
  sku: appServicePlanSku
}


resource appServiceApp 'Microsoft.Web/sites@2021-01-15' = {
  name: appServiceAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
//     httpsOnly: true
  }
}



output appServiceAppName string = appServiceApp.name
output appServiceAppHostName string = appServiceApp.properties.defaultHostName
