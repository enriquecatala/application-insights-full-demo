using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationInsightsFullDemoApi.Models;

namespace ApplicationInsightsFullDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDescriptionsController : ControllerBase
    {
        private readonly AdventureWorksDemoContext _context;

        public ProductDescriptionsController(AdventureWorksDemoContext context)
        {
            _context = context;
        }

        // GET: api/ProductDescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDescription>>> GetProductDescription()
        {
            return await _context.ProductDescription.ToListAsync();
        }

        // GET: api/ProductDescriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDescription>> GetProductDescription(int id)
        {
            var productDescription = await _context.ProductDescription.FindAsync(id);

            if (productDescription == null)
            {
                return NotFound();
            }

            return productDescription;
        }

        // PUT: api/ProductDescriptions/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductDescription(int id, ProductDescription productDescription)
        {
            if (id != productDescription.ProductDescriptionId)
            {
                return BadRequest();
            }

            _context.Entry(productDescription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductDescriptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductDescriptions
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ProductDescription>> PostProductDescription(ProductDescription productDescription)
        {
            _context.ProductDescription.Add(productDescription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductDescription", new { id = productDescription.ProductDescriptionId }, productDescription);
        }

        // DELETE: api/ProductDescriptions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductDescription>> DeleteProductDescription(int id)
        {
            var productDescription = await _context.ProductDescription.FindAsync(id);
            if (productDescription == null)
            {
                return NotFound();
            }

            _context.ProductDescription.Remove(productDescription);
            await _context.SaveChangesAsync();

            return productDescription;
        }

        private bool ProductDescriptionExists(int id)
        {
            return _context.ProductDescription.Any(e => e.ProductDescriptionId == id);
        }
    }
}
