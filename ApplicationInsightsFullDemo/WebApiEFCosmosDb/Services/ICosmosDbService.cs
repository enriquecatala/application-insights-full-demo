using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiEFCosmosDb.Models;

namespace WebApiEFCosmosDb.Services
{
    public interface ICosmosDbService
    {
        Task PostProduct(Product product);
        Task DeleteProduct(int id);
        bool ProductExists(int id);
        Task<ActionResult<Product>> GetProduct(int id);
        Task<ActionResult<IEnumerable<Product>>> GetProducts(string queryString);
    }
}
