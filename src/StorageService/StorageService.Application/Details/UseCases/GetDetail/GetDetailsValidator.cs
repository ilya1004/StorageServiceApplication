using FluentValidation;

namespace StorageService.Application.Details.UseCases.GetDetail;

public class GetDetailsValidator : AbstractValidator<GetDetailsQuery>
{
    public GetDetailsValidator()
    {
        RuleFor(x => x.PageNo)
            .NotEmpty()
            .InclusiveBetween(1, 1_000_000_000);

        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 1000);
    }
}