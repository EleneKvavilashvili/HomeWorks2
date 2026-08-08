namespace Homework21.Models
{
    public class User
    {
        public int Id { get; set; } //primary key
        public DateTime CreateDate { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string JobPosition { get; set; }
        public double Salary { get; set; }
        public double WorkExperience { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public int AddressId { get; set; } //foreign key
        public Address UserAddress { get; set; } //= new Address();
    }

    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public class UserLoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
