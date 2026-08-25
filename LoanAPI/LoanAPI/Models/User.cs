using System.Text.Json.Serialization;

namespace LoanAPI.Models
{
    public class User
    {
        [JsonIgnore]
        public int Id { get; set; } //primary key

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public int MonthlyIncome { get; set; }
        public bool IsBlocked { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public bool IsAccountant { get; set; }


    }
}
