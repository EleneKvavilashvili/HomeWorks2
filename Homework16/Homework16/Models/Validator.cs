using FluentValidation;

namespace Homework16.Models
{
    public class Validator : AbstractValidator<Person>
    {
        public Validator() 
        {
            RuleFor(p => p.CreateDate).LessThanOrEqualTo(DateTime.Now).WithMessage("Cant travel through time.");

            RuleFor(p => p.Firstname).NotEmpty().WithMessage("Everyone has a name.").MaximumLength(50).WithMessage("Name is too long.");

            RuleFor(p => p.Lastname).NotEmpty().WithMessage("Everyone has a surname.").MaximumLength(50).WithMessage("Surname is too long.");

            RuleFor(p => p.JobPosition).NotEmpty().WithMessage("Required to specify employment.").MaximumLength(50).WithMessage("Position is too long.");

            RuleFor(p => p.Salary).ExclusiveBetween(0, 10000.01).WithMessage("Out of tax bracket.");

            RuleFor(p => p.WorkExperience).NotNull().WithMessage("Required to specify experience.").GreaterThanOrEqualTo(0).WithMessage("Can't have negative experience.");

            RuleFor(p => p.PersonAddress).NotNull().WithMessage("Required to specify address.");

            RuleFor(p => p.PersonAddress.Country).NotEmpty().WithMessage("Required to specify country.");

            RuleFor(p => p.PersonAddress.City).NotEmpty().WithMessage("Required to specify city.");

            RuleFor(p => p.PersonAddress.HomeNumber).NotEmpty().WithMessage("Required to specify home number.");
        }
    }
}
