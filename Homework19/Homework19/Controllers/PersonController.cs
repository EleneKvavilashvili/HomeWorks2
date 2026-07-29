using Homework19.Context;
using Homework19.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Homework19.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PersonController : Controller
    {
        private readonly PersonContext _context;
        public PersonController(PersonContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            var updatedList = await _context.Persons.Include(p => p.PersonAddress).ToListAsync();
            return Ok(updatedList);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Persons.Include(p => p.PersonAddress).ToListAsync();
            return Ok(list);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _context.Persons.Include(p => p.PersonAddress).FirstOrDefaultAsync(p => p.Id == id);
            if (person==null)
                return BadRequest("Can't find requested index.");

            return Ok(person);
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Search([FromQuery] double salary)
        {
            var query = _context.Persons.Include(p => p.PersonAddress).Where(p => p.Salary > salary);
            var list = await query.ToListAsync();

            return Ok(list);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _context.Persons.Include(p => p.PersonAddress).FirstOrDefaultAsync(p => p.Id == id);
            if (person == null)
                return BadRequest("Can't find requested index.");

            _context.Persons.Remove(person);
            if (person.PersonAddress != null)
            {
                _context.Addresses.Remove(person.PersonAddress);
            }
            await _context.SaveChangesAsync();

            var updatedList = await _context.Persons.Include(p => p.PersonAddress).ToListAsync();
            return Ok(updatedList);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Person updatedPerson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingPerson = await _context.Persons.Include(p => p.PersonAddress).FirstOrDefaultAsync(p => p.Id == id);
            if (existingPerson==null)
                return BadRequest("Can't find requested index.");

            existingPerson.CreateDate = updatedPerson.CreateDate;
            existingPerson.Firstname = updatedPerson.Firstname;
            existingPerson.Lastname = updatedPerson.Lastname;
            existingPerson.JobPosition = updatedPerson.JobPosition;
            existingPerson.Salary = updatedPerson.Salary;
            existingPerson.WorkExperience = updatedPerson.WorkExperience;

            existingPerson.PersonAddress.Country = updatedPerson.PersonAddress.Country;
            existingPerson.PersonAddress.City = updatedPerson.PersonAddress.City;
            existingPerson.PersonAddress.HomeNumber = updatedPerson.PersonAddress.HomeNumber;

            await _context.SaveChangesAsync();

            var updatedList = await _context.Persons.Include(p => p.PersonAddress).ToListAsync();
            return Ok(updatedList);
        }
    }
}
