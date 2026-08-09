using Homework22.Models;
using Homework22.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homework22.Tests
{
    public class PersonServiceFake : IPersonService
    {
        private readonly List<Person> _persons;
        public PersonServiceFake()
        {
            _persons = new List<Person>()
            {
                new Person()
                {
                    Id = 1,
                    Firstname = "Elene",
                    Lastname = "Kvavilashvili",
                    JobPosition = "Developer",
                    Salary = 9000,
                    WorkExperience = 2,
                    PersonAddress = new Address { Id = 1, Country = "Georgia", City = "Tbilisi", HomeNumber = "102" }
                },
                new Person()
                {
                    Id = 2,
                    Firstname = "firstname",
                    Lastname = "lastname",
                    JobPosition = "IT consultant",
                    Salary = 4500,
                    WorkExperience = 4,
                    PersonAddress = new Address { Id = 2, Country = "Georgia", City = "Batumi", HomeNumber = "5B" }
                }
            };
        }
        public Person Add(Person person)
        {
            person.Id = _persons.Max(p => p.Id) + 1;
            _persons.Add(person);
            return person;
        }

        public IEnumerable<Person> GetAll()
        {
            return _persons;
        }

        public Person GetById(int id)
        {
            return _persons.FirstOrDefault(p => p.Id == id);
        }

        public void Remove(int id)
        {
            var existing = _persons.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                _persons.Remove(existing);
            }
        }

        public IEnumerable<Person> Search(double salary)
        {
            return _persons.Where(p => p.Salary > salary);
        }

        public Person Update(int id, Person updatedPerson)
        {
            var existing = _persons.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                existing.Firstname = updatedPerson.Firstname;
                existing.Lastname = updatedPerson.Lastname;
                existing.JobPosition = updatedPerson.JobPosition;
                existing.Salary = updatedPerson.Salary;
                existing.WorkExperience = updatedPerson.WorkExperience;
                existing.PersonAddress = updatedPerson.PersonAddress;
            }
            return existing;
        }
    }
}
