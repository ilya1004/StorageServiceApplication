using FluentValidation;

namespace StorageService.Application.Storekeepers.UseCases.CreateStorekeeper;

public class CreateStorekeeperValidator : AbstractValidator<CreateStorekeeperCommand>
{
    public CreateStorekeeperValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);
    }
}