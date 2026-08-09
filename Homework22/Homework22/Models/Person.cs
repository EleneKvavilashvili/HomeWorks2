namespace Homework22.Models
{
    public class Person
    {
        public int Id { get; set; } //primary key
        public DateTime CreateDate { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string JobPosition { get; set; }
        public double Salary { get; set; }
        public double WorkExperience { get; set; }

        public int AddressId { get; set; } //foreign key
        public Address PersonAddress { get; set; } = new Address();
    }
}
