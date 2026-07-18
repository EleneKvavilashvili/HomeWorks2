namespace Homework16.Models
{
    public class Person
    {
        public DateTime CreateDate { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string JobPosition { get; set; }
        public double Salary { get; set; }
        public double WorkExperience { get; set; }
        public Address PersonAddress { get; set; }
    }
}
