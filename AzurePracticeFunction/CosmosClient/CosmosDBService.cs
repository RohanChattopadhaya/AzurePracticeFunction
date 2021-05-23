using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using StackExchange.Redis;

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

        IDatabase cache = Connection.GetDatabase();

        private static Lazy<ConnectionMultiplexer> cache_connection = CreateConnection();
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return cache_connection.Value;
            }
        }

        public async Task<object> AddItemAsync(AddDetails item)
        {
            item.id = Guid.NewGuid().ToString();
            var response = _container.CreateItemAsync<AddDetails>(item, new PartitionKey(item.UniqueId)).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                await cache.KeyDeleteAsync("AllData");
                return Response.Response<AddDetails>.Success(item);
            }
            return Response.Response<string>.Fail("Failed");
        }

        public async Task<bool> DeleteItemAsync(string id, string partitionKey)
        {
            var response = _container.DeleteItemAsync<AddDetails>(id, new PartitionKey(partitionKey)).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                await cache.KeyDeleteAsync(id);
                await cache.KeyDeleteAsync("AllData");
                return true;
            }
            return false;
        }

        public async Task<AddDetails> GetItemAsync(string id)
        {
            AddDetails details = new AddDetails();
            var dataInRedis = await cache.StringGetAsync(id);
            if (dataInRedis.IsNullOrEmpty) {
                string query = $"select * from c where c.id = '{id}'";
                QueryDefinition queryDefinition = new QueryDefinition(query);
                FeedIterator<AddDetails> feedIterator = _container.GetItemQueryIterator<AddDetails>(queryDefinition);

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
                var serializedData = JsonConvert.SerializeObject(details);
                await cache.StringSetAsync(id, serializedData);
                return details;
            }
            else
            {
                var detailedData = JsonConvert.DeserializeObject<AddDetails>(cache.StringGet(id));
                return detailedData;
            }          
        }

        public async Task<List<AddDetails>> GetItemsAsync(string query)
        {
            List<AddDetails> listDetails = new List<AddDetails>();
            var dataInRedis = await cache.StringGetAsync("AllData");
            if (dataInRedis.IsNullOrEmpty)
            {
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
                        listDetails.Add(details);
                    }
                }
                var serializedData = JsonConvert.SerializeObject(listDetails);
                await cache.StringSetAsync("AllData", serializedData);
                return listDetails;
            }
            else
            {
                var detailedData = JsonConvert.DeserializeObject<List<AddDetails>>(await cache.StringGetAsync("AllData"));
                return detailedData;
            }
        }

        public async Task<bool> UpdateItemAsync(string id, AddDetails item)
        {
            //ItemResponse<AddDetails> itemResponse =  _container.ReadItemAsync<AddDetails>(id, new PartitionKey(item.UniqueId)).GetAwaiter().GetResult();
            //AddDetails addDetails = itemResponse.Resource;

            //addDetails.EmailID = item.EmailID;           

            var response = _container.ReplaceItemAsync<AddDetails>(item, id,new PartitionKey(item.UniqueId)).GetAwaiter().GetResult();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await cache.KeyDeleteAsync(id);
                await cache.KeyDeleteAsync("AllData");
                return true;
            }

            return false;
        }

        private static Lazy<ConnectionMultiplexer> CreateConnection()
        {
            string cacheConnectionstring = "practicecache.redis.cache.windows.net:6380,password=H0HTM5jHzjDHe2Ex1s8hOsCC8SytjSGEccVClnHTmZc=,ssl=True,abortConnect=False";
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(cacheConnectionstring);
            });
        }
    }
}
