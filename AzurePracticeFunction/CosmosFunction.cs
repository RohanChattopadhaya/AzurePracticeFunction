using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CosmosFunction.CosmosService
{
    public class CosmosFunctions
    {
        private readonly ICosmosDBService _cosmosDbService;
        public CosmosFunctions(ICosmosDBService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }
        [FunctionName("InsertFunction")]
        public async Task<IActionResult> Insert(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AddDetails>(requestBody);
            var response = _cosmosDbService.AddItemAsync(data);
            
            return new OkObjectResult(response.Result);
        }

        [FunctionName("GetFunction")]
        public async Task<IActionResult> Get(
           [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AddDetails>(requestBody);
            var response = _cosmosDbService.GetItemAsync(data.id);

            return new OkObjectResult(response.Result);
        }

        [FunctionName("GetAllFunction")]
        public async Task<IActionResult> GetAll(
           [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string query = "Select * from c";
            var response = _cosmosDbService.GetItemsAsync(query);

            return new OkObjectResult(response.Result);
        }

        [FunctionName("DeleteFunction")]
        public async Task<IActionResult> Delete(
           [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AddDetails>(requestBody);
            var response = _cosmosDbService.DeleteItemAsync(data.id, data.UniqueId);

            return new OkObjectResult(response.Result);
        }

        [FunctionName("UpdateFunction")]
        public async Task<IActionResult> Update(
           [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AddDetails>(requestBody);
            var response = _cosmosDbService.UpdateItemAsync(data.id, data);

            return new OkObjectResult(response.Result);
        }
    }
}
