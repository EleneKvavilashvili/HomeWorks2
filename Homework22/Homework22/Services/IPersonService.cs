using Homework22.Models;

namespace Homework22.Services
{
    public interface IPersonService
    {
        IEnumerable<Person> GetAll();
        Person GetById(int id);
        IEnumerable<Person> Search(double salary);
        Person Add(Person newItem);
        void Remove(int id);
        Person Update(int id, Person updatedPerson);
    }
}
