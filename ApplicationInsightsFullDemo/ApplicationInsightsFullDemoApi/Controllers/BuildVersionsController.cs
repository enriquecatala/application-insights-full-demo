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
    public class BuildVersionsController : ControllerBase
    {
        private readonly AdventureWorksDemoContext _context;

        public BuildVersionsController(AdventureWorksDemoContext context)
        {
            _context = context;
        }

        // GET: api/BuildVersions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildVersion>>> GetBuildVersion()
        {
            return await _context.BuildVersion.ToListAsync();
        }

        // GET: api/BuildVersions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BuildVersion>> GetBuildVersion(byte id)
        {
            var buildVersion = await _context.BuildVersion.FindAsync(id);

            if (buildVersion == null)
            {
                return NotFound();
            }

            return buildVersion;
        }

        // PUT: api/BuildVersions/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuildVersion(byte id, BuildVersion buildVersion)
        {
            if (id != buildVersion.SystemInformationId)
            {
                return BadRequest();
            }

            _context.Entry(buildVersion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildVersionExists(id))
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

        // POST: api/BuildVersions
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<BuildVersion>> PostBuildVersion(BuildVersion buildVersion)
        {
            _context.BuildVersion.Add(buildVersion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBuildVersion", new { id = buildVersion.SystemInformationId }, buildVersion);
        }

        // DELETE: api/BuildVersions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BuildVersion>> DeleteBuildVersion(byte id)
        {
            var buildVersion = await _context.BuildVersion.FindAsync(id);
            if (buildVersion == null)
            {
                return NotFound();
            }

            _context.BuildVersion.Remove(buildVersion);
            await _context.SaveChangesAsync();

            return buildVersion;
        }

        private bool BuildVersionExists(byte id)
        {
            return _context.BuildVersion.Any(e => e.SystemInformationId == id);
        }
    }
}
