using FluentValidation;
using Homework21.Models;

namespace Homework21.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(p => p.CreateDate).LessThanOrEqualTo(DateTime.Now).WithMessage("Cant travel through time.");

            RuleFor(p => p.Firstname).NotEmpty().WithMessage("Everyone has a name.").MaximumLength(50).WithMessage("Name is too long.");

            RuleFor(p => p.Lastname).NotEmpty().WithMessage("Everyone has a surname.").MaximumLength(50).WithMessage("Surname is too long.");

            RuleFor(p => p.JobPosition).NotEmpty().WithMessage("Required to specify employment.").MaximumLength(50).WithMessage("Position is too long.");

            RuleFor(p => p.Salary).ExclusiveBetween(0, 10000.01).WithMessage("Out of tax bracket.");

            RuleFor(p => p.WorkExperience).NotNull().WithMessage("Required to specify experience.").GreaterThanOrEqualTo(0).WithMessage("Can't have negative experience.");

            RuleFor(p => p.UserAddress).NotNull().WithMessage("Required to specify address.").SetValidator(new AddressValidator());

            RuleFor(p=>p.Username).NotEmpty().WithMessage("Everyone needs an username.").MaximumLength(50).WithMessage("Username is too long.");

            RuleFor(p => p.Password).NotEmpty().WithMessage("Everyone needs an password.").MinimumLength(8).WithMessage("Password is too long.");
        }
    }
}
