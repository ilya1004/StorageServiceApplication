using MediatR;
using Microsoft.AspNetCore.Mvc;
using StorageService.API.RequestDtos.Input;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Application.Storekeepers.UseCases.CreateStorekeeper;
using StorageService.Application.Storekeepers.UseCases.DeleteStorekeeper;
using StorageService.Application.Storekeepers.UseCases.GetStorekeepers;
using StorageService.Application.Storekeepers.UseCases.GetStorekeepersLookup;
using StorageService.Domain.Models;

namespace StorageService.API.Controllers;

[ApiController]
[Route("api/storekeepers")]
public class StorekeepersController : ControllerBase
{
    private readonly ISender _sender;

    public StorekeepersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<StorekeeperCoreDto>> Create([FromBody] CreateStorekeeperDto createStorekeeperDto)
    {
        var result = await _sender.Send(
            new CreateStorekeeperCommand(createStorekeeperDto.FullName),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResultModel<StorekeeperCoreDto>>> GetAllStorekeepersAsync(
        [FromQuery] PaginatedRequestDto requestDto)
    {
        var result = await _sender.Send(
            new GetStorekeepersQuery(requestDto.PageNo, requestDto.PageSize),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpGet]
    [Route("lookup")]
    public async Task<ActionResult<List<StorekeeperCoreDto>>> GetStorekeepersLookupAsync()
    {
        var result = await _sender.Send(
            new GetStorekeepersLookupQuery(),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> DeleteStorekeeperAsync(int id)
    {
        await _sender.Send(new DeleteStorekeeperCommand(id), HttpContext.RequestAborted);

        return NoContent();
    }
}