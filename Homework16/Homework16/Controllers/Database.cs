using Homework16.Models;
using System.Text.Json;

namespace Homework16.Controllers
{
    public class Database
    {
        private const string FilePath = "respondents.json";

        public async Task<List<Person>> ReadAllAsync()
        {
            if (!File.Exists(FilePath))
                return new List<Person>();

            var json = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<Person>>(json) ?? new List<Person>();
        }

        public async Task WriteAllAsync(List<Person> people)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(people, options);
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}
