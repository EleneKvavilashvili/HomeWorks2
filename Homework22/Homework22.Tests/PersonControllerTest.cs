using Homework22.Controllers;
using Homework22.Models;
using Homework22.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homework22.Tests
{
    public class PersonControllerTest
    {
        private readonly PersonController _controller;
        private readonly IPersonService _service;

        public PersonControllerTest()
        {
            _service = new PersonServiceFake();
            _controller = new PersonController(_service);
        }

        //GET
        [Fact]
        public async Task GetAll_WhenCalled_ReturnsOkResult()
        {
            var result = await _controller.GetAll();
            Assert.IsType<OkObjectResult>(result);
        }

        //Get by id
        [Fact]
        public async Task GetById_ExistingIdPassed_ReturnsOkResult()
        {
            var testId = 1;
            var result = await _controller.GetById(testId);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_InvalidIdPassed_ReturnsBadRequest()
        {
            var invalidId = 99;
            var result = await _controller.GetById(invalidId);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        //Delete
        [Fact]
        public async Task Remove_ExistingIdPassed_RemovesOneItem()
        {
            var existingId = 1;

            var result = await _controller.Delete(existingId);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_UnknownIdPassed_ReturnsBadRequest()
        {
            var invalidId = 99;

            var result = await _controller.Delete(invalidId);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        //Filter
        [Fact]
        public async Task Search_ValidSalaryPassed_ReturnsOkResult()
        {
            double minSalary = 3000;

            var result = await _controller.Search(minSalary);

            Assert.IsType<OkObjectResult>(result);
        }

        //Post
        [Fact]
        public async Task CreatePerson_ValidObjectPassed_ReturnsOkResult()
        {
            var newPerson = new Person
            {
                CreateDate = DateTime.Now,
                Firstname = "name",
                Lastname = "surname",
                JobPosition = "Designer",
                Salary = 2000,
                WorkExperience = 1,
                PersonAddress = new Address { Country = "Georgia", City = "Tbilisi", HomeNumber = "12A" }
            };

            var result = await _controller.CreatePerson(newPerson);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreatePerson_InvalidModelState_ReturnsBadRequest()
        {
            var newPerson = new Person
            {
                CreateDate = DateTime.Now,
                Firstname = "name",
                Lastname = "surname",
                JobPosition = "Designer",
                Salary = 2000,
                WorkExperience = 1,
                PersonAddress = new Address { Country = "Georgia", City = "Tbilisi", HomeNumber = "1" }
            };
            _controller.ModelState.AddModelError("Firstname", "Required");
            var result = await _controller.CreatePerson(newPerson);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        //Put
        [Fact]
        public async Task Update_ExistingIdPassed_ReturnsOkResult()
        {
            var validId = 1;
            var updatedPerson = new Person
            {
                CreateDate = DateTime.Now,
                Firstname = "nameU",
                Lastname = "surnameU",
                JobPosition = "Senior Developer",
                Salary = 4000,
                WorkExperience = 3,
                PersonAddress = new Address { Country = "Georgia", City = "Tbilisi", HomeNumber = "12A" }
            };

            var result = await _controller.Update(validId, updatedPerson);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Update_UnknownIdPassed_ReturnsBadRequest()
        {
            var invalidId = 99;
            var updatedPerson = new Person
            {
                CreateDate = DateTime.Now,
                Firstname = "nameUupdated",
                Lastname = "surnameUUpdated",
                JobPosition = "Senior Developer",
                Salary = 4000,
                WorkExperience = 3,
                PersonAddress = new Address { Country = "Georgia", City = "Tbilisi", HomeNumber = "12A" }
            };

            var result = await _controller.Update(invalidId, updatedPerson);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_InvalidModelState_ReturnsBadRequest()
        {
            var validId = 1;
            var updatedPerson = new Person
            {
                CreateDate = DateTime.Now,
                Firstname = "nameU",
                Lastname = "surnameU",
                JobPosition = "Senior Developer",
                Salary = 4000,
                WorkExperience = 3,
                PersonAddress = new Address { Country = "Georgia", City = "Tbilisi", HomeNumber = "12A" }
            };
            _controller.ModelState.AddModelError("Salary", "Out of range");
            var result = await _controller.Update(validId, updatedPerson);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
