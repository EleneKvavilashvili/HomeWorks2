using FluentValidation;
using Homework21.Models;

namespace Homework21.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(p => p.Country).NotEmpty().WithMessage("Required to specify country.");

            RuleFor(p => p.City).NotEmpty().WithMessage("Required to specify city.");

            RuleFor(p => p.HomeNumber).NotEmpty().WithMessage("Required to specify home number.");
        }
    }
}
