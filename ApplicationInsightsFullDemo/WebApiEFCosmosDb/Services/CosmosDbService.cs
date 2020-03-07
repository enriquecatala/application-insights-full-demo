using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiEFCosmosDb.Models;

namespace WebApiEFCosmosDb.Services
{
    public class CosmosDbService : ICosmosDbService
    {

        private Microsoft.Azure.Cosmos.Container _container;
        public CosmosDbService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }
        public async Task DeleteProduct(int id)
        {
            await this._container.DeleteItemAsync<Product>(id.ToString(), new PartitionKey(id));
        }

        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                ItemResponse<Product> response = await this._container.ReadItemAsync<Product>(id.ToString(), new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Product>(new QueryDefinition(queryString));
            List<Product> results = new List<Product>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public bool ProductExists(int id)
        {
            throw new NotImplementedException();
        }

        public async Task  PostProduct(Product product)
        {
            var retorno = await this._container.CreateItemAsync<Product>(product, new PartitionKey(product.ProductId));
            
        }
    }
}
