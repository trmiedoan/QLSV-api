using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QlsvApi.Data;
using QlsvApi.Models;

namespace QlsvApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public MajorsController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: api/Majors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MajorDto>>> GetMajors()
        {
            return await _context.Majors.Select(m => m.ToMajorDto()).ToListAsync();
        }

        // // GET: api/Majors/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<Major>> GetMajor(int id)
        // {
        //     var major = await _context.Majors.FindAsync(id);

        //     if (major == null)
        //     {
        //         return NotFound();
        //     }

        //     return major;
        // }

        // // PUT: api/Majors/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutMajor(int id, Major major)
        // {
        //     if (id != major.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(major).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!MajorExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // // POST: api/Majors
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<Major>> PostMajor(Major major)
        // {
        //     _context.Majors.Add(major);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetMajor", new { id = major.Id }, major);
        // }

        // // DELETE: api/Majors/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteMajor(int id)
        // {
        //     var major = await _context.Majors.FindAsync(id);
        //     if (major == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Majors.Remove(major);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool MajorExists(int id)
        {
            return _context.Majors.Any(e => e.Id == id);
        }
    }
}
