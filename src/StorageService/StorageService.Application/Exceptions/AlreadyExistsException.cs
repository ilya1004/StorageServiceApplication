namespace StorageService.Application.Exceptions;

[Serializable]
public class AlreadyExistsException : Exception
{
    public AlreadyExistsException(string message) : base(message)
    {
    }
}
