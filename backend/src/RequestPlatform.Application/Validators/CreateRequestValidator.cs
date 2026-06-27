using FluentValidation;
using RequestPlatform.Application.DTOs;
using RequestPlatform.Domain.Enums;

namespace RequestPlatform.Application.Validators;

public class CreateRequestValidator : AbstractValidator<CreateRequestDto>
{
    public CreateRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(BeAValidType).WithMessage("Type must be one of: Vacation, Loan, Permission.");

        RuleFor(x => x.DynamicData)
            .Must(x => x.ValueKind != System.Text.Json.JsonValueKind.Undefined)
            .WithMessage("DynamicData is required.");
    }

    private static bool BeAValidType(string type)
    {
        return Enum.TryParse<RequestType>(type, ignoreCase: true, out _);
    }
}
