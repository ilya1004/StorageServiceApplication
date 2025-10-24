using FluentValidation;

namespace StorageService.Application.Storekeepers.UseCases.GetStorekeepers;

public class GetStorekeepersValidator : AbstractValidator<GetStorekeepersQuery>
{
    public GetStorekeepersValidator()
    {
        RuleFor(x => x.PageNo)
            .NotEmpty()
            .InclusiveBetween(1, 1_000_000_000);

        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 1000);
    }
}