using FluentValidation;

namespace StorageService.Application.Details.UseCases.DeleteDetail;

public class DeleteDetailValidator : AbstractValidator<DeleteDetailCommand>
{
    public DeleteDetailValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}