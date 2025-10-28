using FluentValidation;
using StorageService.Application.Storekeepers.UseCases.CreateStorekeeper;

namespace StorageService.Application.Storekeepers.UseCases.UpdateStorekeeper;

public class UpdateStorekeeperValidator : AbstractValidator<UpdateStorekeeperCommand>
{
    public UpdateStorekeeperValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);
    }
}