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

        public async Task<bool> DeleteItemAsync(string id, string partitionKey)
        {
            var response = _container.DeleteItemAsync<AddDetails>(id, new PartitionKey(partitionKey)).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            return false;
        }

        public async Task<AddDetails> GetItemAsync(string id)
        {
            string query = $"select * from c where c.id = '{id}'";
            QueryDefinition queryDefinition = new QueryDefinition(query);
            FeedIterator<AddDetails> feedIterator = _container.GetItemQueryIterator<AddDetails>(queryDefinition);
            AddDetails details = new AddDetails();
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<AddDetails> feedResponse = feedIterator.ReadNextAsync().GetAwaiter().GetResult();
                foreach (var responseDetails in feedResponse)
                {
                    details.id = responseDetails.id;
                    details.EmailID = responseDetails.EmailID;
                    details.UniqueId = responseDetails.UniqueId;
                    details.UniqueName = responseDetails.UniqueName;
                    details.items = responseDetails.items;
                }
            }
            return details;
        }

        public async Task<List<AddDetails>> GetItemsAsync(string query)
        {
            QueryDefinition queryDefinition = new QueryDefinition(query);
            FeedIterator<AddDetails> feedIterator = _container.GetItemQueryIterator<AddDetails>(queryDefinition);
            AddDetails details = new AddDetails();
            List<AddDetails> listDetails = new List<AddDetails>();
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<AddDetails> feedResponse = feedIterator.ReadNextAsync().GetAwaiter().GetResult();
                foreach (var responseDetails in feedResponse)
                {
                    details.id = responseDetails.id;
                    details.EmailID = responseDetails.EmailID;
                    details.UniqueId = responseDetails.UniqueId;
                    details.UniqueName = responseDetails.UniqueName;
                    details.items = responseDetails.items;
                    listDetails.Add(details);
                }               
            }
            return listDetails;
        }

        public async Task<bool> UpdateItemAsync(string id, AddDetails item)
        {
            //ItemResponse<AddDetails> itemResponse =  _container.ReadItemAsync<AddDetails>(id, new PartitionKey(item.UniqueId)).GetAwaiter().GetResult();
            //AddDetails addDetails = itemResponse.Resource;

            //addDetails.EmailID = item.EmailID;           

            var response = _container.ReplaceItemAsync<AddDetails>(item, id,new PartitionKey(item.UniqueId)).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
