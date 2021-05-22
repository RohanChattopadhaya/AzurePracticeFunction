using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosFunction.CosmosService
{
    public class CosmosDBService : ICosmosDBService
    {
        private Container _container;

        public CosmosDBService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }
        public async Task<object> AddItemAsync(AddDetails item)
        {
            item.id = Guid.NewGuid().ToString();
            var response = _container.CreateItemAsync<AddDetails>(item, new PartitionKey(item.UniqueId)).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return Response.Response<AddDetails>.Success(item);
            }
            return Response.Response<string>.Fail("Failed");
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<AddDetails> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AddDetails>> GetItemsAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(string id, AddDetails item)
        {
            throw new NotImplementedException();
        }
    }
}
