using CosmosFunction.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CosmosFunction.CosmosService
{
    public interface ICosmosDBService
    {
        Task<List<AddDetails>> GetItemsAsync(string query);
        Task<AddDetails> GetItemAsync(string id);
        Task<object> AddItemAsync(AddDetails item);
        Task<bool> UpdateItemAsync(string id, AddDetails item);
        Task<bool> DeleteItemAsync(string id, string partitionKey);
    }
}
