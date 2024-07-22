@description('Name of the Container Apps environment')
param environmentName string = 'agendamanager-env'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Name of the Log Analytics workspace')
param logAnalyticsWorkspaceName string = 'agendamanager-logs'

@description('Name of the Container App for RavenDB')
param ravendbAppName string = 'ravendb-app'

@description('Name of the first Container App for AgendaManager')
param agendaManagerAppName1 string = 'agendamanager-app1'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    retentionInDays: 30
    sku: {
      name: 'PerGB2018'
    }
  }

}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'agendamanagerstorage'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }

}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: environmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }

  }

}

resource ravendbContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: ravendbAppName
  location: location
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
      }
      registries: []
    }
    template: {
      containers: [
        {
          name: 'ravendb'
          image: 'ravendb/ravendb:ubuntu-latest'
          resources: {
            cpu: 1
            memory: '2.0Gi'
          }
          env: [
            {
              name: 'RAVEN_SETTINGS'
              value: '{}'
            }
            {
              name: 'RAVEN_Security_UnsecuredAccessAllowed'
              value: 'PublicNetwork'
            }
            {
              name: 'RAVEN_Setup_Mode'
              value: 'None'
            }
            {
              name: 'RAVEN_License_Eula_Accepted'
              value: 'true'
            }
            {
              name: 'RAVEN_ServerUrl'
              value: 'http://0.0.0.0:8080'
            }
            {
              name: 'RAVEN_ServerUrl_Tcp'
              value: 'tcp://0.0.0.0:38888'
            }
          ]
          volumeMounts: [
            {
              volumeName: 'ravendb-volume'
              mountPath: '/var/lib/ravendb/data'
            }
          ]
        }
      ]
    }

    }
  }

resource agendaManagerContainerApp1 'Microsoft.App/containerApps@2022-03-01' = {
  name: agendaManagerAppName1
  location: location
  properties: {
    managedEnvironmentId: containerAppEnv.id
    configuration: {
      ingress: {
        external: true
        targetPort: 5158
      }
      registries: []
    }
    template: {
      containers: [
        {
          name: 'agendamanager'
          image: 'agendamanager'
          resources: {
            cpu: 1
            memory: '2.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
          ]
        }
      ]
    }
  }
}
