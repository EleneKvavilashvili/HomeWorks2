using FluentValidation;
using LoanAPI.Models;

namespace LoanAPI.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Everyone has a name.").MaximumLength(50).WithMessage("Name is too long.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Everyone has a surname.").MaximumLength(50).WithMessage("Surname is too long.");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Everyone has an username.").MaximumLength(50).WithMessage("Username is too long.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Contact info is required.");
            RuleFor(x => x.Age).GreaterThanOrEqualTo(18).WithMessage("Must be of legal age.");
            RuleFor(x => x.MonthlyIncome).GreaterThanOrEqualTo(0);
            RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("Password is required.");
        }
    }
}
