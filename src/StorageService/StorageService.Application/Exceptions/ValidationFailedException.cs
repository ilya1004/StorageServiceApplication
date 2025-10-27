using FluentValidation.Results;

namespace StorageService.Application.Exceptions;

[Serializable]
public class ValidationFailedException : Exception
{
    public IReadOnlyList<ValidationFailure> Failures { get; private set; }

    public ValidationFailedException(IReadOnlyList<ValidationFailure> failures)
    {
        Failures = failures;
    }
}