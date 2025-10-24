using FluentValidation;

namespace StorageService.Application.Storekeepers.UseCases.DeleteStorekeeper;

public class DeleteStorekeeperValidator : AbstractValidator<DeleteStorekeeperCommand>
{
    public DeleteStorekeeperValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}