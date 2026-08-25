using FluentValidation;
using LoanAPI.Models;

namespace LoanAPI.Validators
{
    public class LoanValidator : AbstractValidator<Loan>
    {
        public LoanValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.PeriodMonths).GreaterThan(0);
        }
    }
}
