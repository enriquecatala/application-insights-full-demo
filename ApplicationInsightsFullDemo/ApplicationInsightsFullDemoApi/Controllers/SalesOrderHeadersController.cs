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
    public class SalesOrderHeadersController : ControllerBase
    {
        private readonly AdventureWorksDemoContext _context;

        public SalesOrderHeadersController(AdventureWorksDemoContext context)
        {
            _context = context;
        }

        // GET: api/SalesOrderHeaders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeader()
        {
            return await _context.SalesOrderHeader.ToListAsync();
        }

        // GET: api/SalesOrderHeaders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> GetSalesOrderHeader(int id)
        {
            var salesOrderHeader = await _context.SalesOrderHeader.FindAsync(id);

            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return salesOrderHeader;
        }

        // PUT: api/SalesOrderHeaders/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderHeader(int id, SalesOrderHeader salesOrderHeader)
        {
            if (id != salesOrderHeader.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderHeaderExists(id))
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

        // POST: api/SalesOrderHeaders
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
            _context.SalesOrderHeader.Add(salesOrderHeader);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }

        // DELETE: api/SalesOrderHeaders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> DeleteSalesOrderHeader(int id)
        {
            var salesOrderHeader = await _context.SalesOrderHeader.FindAsync(id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            _context.SalesOrderHeader.Remove(salesOrderHeader);
            await _context.SaveChangesAsync();

            return salesOrderHeader;
        }

        private bool SalesOrderHeaderExists(int id)
        {
            return _context.SalesOrderHeader.Any(e => e.SalesOrderId == id);
        }
    }
}
