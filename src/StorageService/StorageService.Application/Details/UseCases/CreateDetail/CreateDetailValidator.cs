using FluentValidation;

namespace StorageService.Application.Details.UseCases.CreateDetail;

public class CreateDetailValidator : AbstractValidator<CreateDetailCommand>
{
    public CreateDetailValidator()
    {
        RuleFor(x => x.NomenclatureCode)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Count)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.StorekeeperId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.CreatedAtDate)
            .NotEmpty();
    }
}