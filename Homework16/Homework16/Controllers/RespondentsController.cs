using FluentValidation;
using Homework16.Models;
using Microsoft.AspNetCore.Mvc;

namespace Homework16.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class RespondentsController : Controller
    {
        private readonly Database _storage;
        private readonly IValidator<Person> _validator;

        public RespondentsController(Database storage, IValidator<Person> validator)
        {
            _storage = storage;
            _validator = validator;

        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            var validationResult = await _validator.ValidateAsync(person);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var list = await _storage.ReadAllAsync();
            list.Add(person);
            await _storage.WriteAllAsync(list);

            return Ok(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _storage.ReadAllAsync();
            return Ok(list);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var list = await _storage.ReadAllAsync();
            if (id < 0 || id >= list.Count)
                return BadRequest("Can't find requested index.");

            return Ok(list[id]);
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Search([FromQuery] double salary)
        {
            var list = await _storage.ReadAllAsync();

            return Ok(list.Where(p=>p.Salary>salary));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var list = await _storage.ReadAllAsync();
            if (id < 0 || id >= list.Count)
                return BadRequest("Can't find requested index.");

            list.RemoveAt(id);
            await _storage.WriteAllAsync(list);

            return Ok(list);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Person person)
        {
            var list = await _storage.ReadAllAsync();
            if (id < 0 || id >= list.Count)
                return BadRequest("Can't find requested index.");

            var validationResult = await _validator.ValidateAsync(person);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            list[id] = person;
            await _storage.WriteAllAsync(list);

            return Ok(list);
        }
    }
}
