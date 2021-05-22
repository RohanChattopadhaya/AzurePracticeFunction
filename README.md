# AzurePracticeFunction

Here Azure Function is dealing with data on respect of CosmosDB.

* local.settings.json file main Details -  

 {
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "CosmosDb": {
    "ConnectionString": "",
    "DatabaseName": "",
    "ContainerName": ""
  }
}
