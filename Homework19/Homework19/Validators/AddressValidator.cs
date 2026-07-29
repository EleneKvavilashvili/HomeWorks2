using FluentValidation;
using Homework19.Models;

namespace Homework19.Validators
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
