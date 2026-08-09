using Homework22.Context;
using Homework22.Models;
using Homework22.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Homework22.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PersonController : Controller
    {
        private readonly IPersonService _service;
        public PersonController(IPersonService service)
        {
            _service = service; ;
        }


        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created=_service.Add(person);
            return Ok(created);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = _service.GetAll();
            return Ok(list);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = _service.GetById(id);
            if (person == null)
                return BadRequest("Can't find requested index.");

            return Ok(person);
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Search([FromQuery] double salary)
        {
            var list = _service.Search(salary);
            return Ok(list);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var person = _service.GetById(id);
            if (person == null)
                return BadRequest("Can't find requested index.");

            _service.Remove(id);
            return Ok();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Person updatedPerson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existing = _service.GetById(id);
            if (existing == null)
                return BadRequest("Can't find requested index.");

            var updated = _service.Update(id, updatedPerson);
            return Ok(updated);
        }
    }
}
