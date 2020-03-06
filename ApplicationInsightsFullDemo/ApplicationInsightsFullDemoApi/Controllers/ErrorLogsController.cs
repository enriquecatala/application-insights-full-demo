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
    public class ErrorLogsController : ControllerBase
    {
        private readonly AdventureWorksDemoContext _context;

        public ErrorLogsController(AdventureWorksDemoContext context)
        {
            _context = context;
        }

        // GET: api/ErrorLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ErrorLog>>> GetErrorLog()
        {
            return await _context.ErrorLog.ToListAsync();
        }

        // GET: api/ErrorLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ErrorLog>> GetErrorLog(int id)
        {
            var errorLog = await _context.ErrorLog.FindAsync(id);

            if (errorLog == null)
            {
                return NotFound();
            }

            return errorLog;
        }

        // PUT: api/ErrorLogs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutErrorLog(int id, ErrorLog errorLog)
        {
            if (id != errorLog.ErrorLogId)
            {
                return BadRequest();
            }

            _context.Entry(errorLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ErrorLogExists(id))
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

        // POST: api/ErrorLogs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ErrorLog>> PostErrorLog(ErrorLog errorLog)
        {
            _context.ErrorLog.Add(errorLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetErrorLog", new { id = errorLog.ErrorLogId }, errorLog);
        }

        // DELETE: api/ErrorLogs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ErrorLog>> DeleteErrorLog(int id)
        {
            var errorLog = await _context.ErrorLog.FindAsync(id);
            if (errorLog == null)
            {
                return NotFound();
            }

            _context.ErrorLog.Remove(errorLog);
            await _context.SaveChangesAsync();

            return errorLog;
        }

        private bool ErrorLogExists(int id)
        {
            return _context.ErrorLog.Any(e => e.ErrorLogId == id);
        }
    }
}
