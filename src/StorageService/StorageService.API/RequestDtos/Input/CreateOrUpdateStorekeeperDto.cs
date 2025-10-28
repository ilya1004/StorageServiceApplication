namespace StorageService.API.RequestDtos.Input;

public sealed record CreateOrUpdateStorekeeperDto
{
    public string FullName { get; set; }
}