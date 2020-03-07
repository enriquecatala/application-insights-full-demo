using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiEFCosmosDb.Models;
using WebApiEFCosmosDb.Services;

namespace WebApiEFCosmosDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController
    {
        private readonly ICosmosDbService _context;

        public ProductsController(ICosmosDbService cosmosDbService)
        {
            _context = cosmosDbService;
        }

        /*
        Task PostProduct(Product product);
        Task DeleteProduct(int id);
        bool ProductExists(int id);

        Task<ActionResult<Product>> GetProduct(int id);
        Task<ActionResult<IEnumerable<Product>>> GetProducts(string queryString);
         */

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            string queryString = "select * from c";
            return await _context.GetProducts(queryString);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.GetProduct(id);

            if (product == null)
            {
                throw new NotImplementedException();
            }

            return product;
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task DeleteProduct(int id)
        {
           await _context.DeleteProduct(id);            
        }

        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task PostProduct(Product product)
        {
            await _context.PostProduct(product);            
        }
    }
}
