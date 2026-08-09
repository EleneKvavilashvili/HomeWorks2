using FluentValidation;
using Homework22.Models;

namespace Homework22.Validators
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
