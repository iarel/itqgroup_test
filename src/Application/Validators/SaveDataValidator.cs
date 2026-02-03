using FluentValidation;

namespace Application.Validators;

public class SaveDataValidator : AbstractValidator<List<Dictionary<string, string>>>
{
    public SaveDataValidator()
    {
        RuleFor(x => x)
            .NotNull().WithMessage("Input cannot be null")
            .NotEmpty().WithMessage("Input cannot be empty");

        RuleForEach(x => x).ChildRules(items =>
        {
            items.RuleFor(i => i)
                .Must(dict => dict.Count == 1)
                .WithMessage("Each item must contain exactly one key-value pair.");
            
            items.RuleFor(i => i)
                .Must(dict => int.TryParse(dict.Keys.First(), out _))
                .WithMessage("Key must be a valid integer.");
        });
    }
}
