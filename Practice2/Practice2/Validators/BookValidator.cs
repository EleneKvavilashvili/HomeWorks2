using FluentValidation;
using Practice2.Models;

namespace Practice2.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {   
        public BookValidator()
        {
            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(2).WithMessage("Title has to consist of min 2 symbols.");

            RuleFor(b => b.Author)
                .NotEmpty().WithMessage("Author information is required.")
                .MinimumLength(3).WithMessage("Author name has to consist of min 3 symbols.");

            RuleFor(b => b.PublishYear)
                .NotEmpty().WithMessage("Publish year is required.")
                .InclusiveBetween(1900, DateTime.Now.Year)
                .WithMessage($"Publish year muste be between 1900 and now({DateTime.Now.Year}).");

            RuleFor(b => b.Genre)
                .MaximumLength(50).WithMessage("Genre length must not be more than 50 symbols.");
        }
    }
}
