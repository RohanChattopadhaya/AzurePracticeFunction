using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(CosmosFunction.CosmosService.Startup))]
namespace CosmosFunction.CosmosService
{
    public class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureSettings();
            builder.Services.AddSingleton<ICosmosDBService>(InitializeCosmosClientInstanceAsync(_configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        }

        private void ConfigureSettings()
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            _configuration = config;
        }

        private static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string connectionString = configurationSection.GetSection("ConnectionString").Value;
            // string account = configurationSection.GetSection("Account").Value;
            //string key = configurationSection.GetSection("Key").Value;
            CosmosClient client = new CosmosClient(connectionString);
            CosmosDBService cosmosDbService = new CosmosDBService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/UniqueId");

            return cosmosDbService;
        }
    }
}
