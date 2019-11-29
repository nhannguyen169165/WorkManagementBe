using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {
        private readonly WorkManagementContext _context;

        public AuthenticationsController(WorkManagementContext context)
        {
            _context = context;
        }

        // GET: api/Authentications
        [HttpGet]
        public IEnumerable<Authentication> GetAuthentication()
        {
            return _context.Authentication;
        }

        // GET: api/Authentications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthentication([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authentication = await _context.Authentication.FindAsync(id);

            if (authentication == null)
            {
                return NotFound();
            }

            return Ok(authentication);
        }

        // PUT: api/Authentications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthentication([FromRoute] int id, [FromBody] Authentication authentication)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != authentication.Id)
            {
                return BadRequest();
            }

            _context.Entry(authentication).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthenticationExists(id))
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

        // POST: api/Authentications
        [HttpPost]
        public async Task<IActionResult> PostAuthentication([FromBody] Authentication authentication)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Authentication.Add(authentication);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthentication", new { id = authentication.Id }, authentication);
        }

        // DELETE: api/Authentications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthentication([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authentication = await _context.Authentication.FindAsync(id);
            if (authentication == null)
            {
                return NotFound();
            }

            _context.Authentication.Remove(authentication);
            await _context.SaveChangesAsync();

            return Ok(authentication);
        }

        private bool AuthenticationExists(int id)
        {
            return _context.Authentication.Any(e => e.Id == id);
        }
    }
}