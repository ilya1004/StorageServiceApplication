using MediatR;
using Microsoft.AspNetCore.Mvc;
using StorageService.API.RequestDtos.Input;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Application.Storekeepers.UseCases.CreateStorekeeper;
using StorageService.Application.Storekeepers.UseCases.DeleteStorekeeper;
using StorageService.Application.Storekeepers.UseCases.GetStorekeeperById;
using StorageService.Application.Storekeepers.UseCases.GetStorekeepers;
using StorageService.Application.Storekeepers.UseCases.GetStorekeepersLookup;
using StorageService.Application.Storekeepers.UseCases.UpdateStorekeeper;
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

    /// <summary>
    /// Create storekeeper
    /// </summary>
    /// <param name="storekeeperDto">Storekeeper to create</param>
    /// <returns>Created storekeeper</returns>
    [HttpPost]
    [ProducesResponseType(typeof(StorekeeperCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StorekeeperCoreDto>> Create(
        [FromBody] CreateOrUpdateStorekeeperDto storekeeperDto)
    {
        var result = await _sender.Send(
            new CreateStorekeeperCommand(storekeeperDto.FullName),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get a paginated list of all non-deleted storekeepers with details count
    /// </summary>
    /// <param name="requestDto">Pagination parameters</param>
    /// <returns>Paginated result with storekeeper and details count</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResultModel<StorekeeperWithDetailsCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResultModel<StorekeeperWithDetailsCountDto>>> GetAllStorekeepersAsync(
        [FromQuery] PaginatedRequestDto requestDto)
    {
        var result = await _sender.Send(
            new GetStorekeepersQuery(requestDto.PageNo, requestDto.PageSize),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get a storekeepers lookup
    /// </summary>
    /// <returns>Lookup of active storekeepers</returns>
    [HttpGet]
    [Route("lookup")]
    [ProducesResponseType(typeof(List<StorekeeperCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<StorekeeperCoreDto>>> GetStorekeepersLookupAsync()
    {
        var result = await _sender.Send(
            new GetStorekeepersLookupQuery(),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get storekeeper by Id
    /// </summary>
    /// <param name="id">Storekeeper Id</param>
    /// <returns>The storekeeper by Id</returns>
    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(StorekeeperCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StorekeeperCoreDto>> GetStorekeeperByIdAsync(int id)
    {
        var result = await _sender.Send(
            new GetStorekeeperByIdQuery(id),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update storekeeper by Id
    /// </summary>
    /// <param name="id">The storekeeper Id to update</param>
    /// <param name="storekeeperDto">Storekeeper data to update</param>
    /// <returns>Updated storekeeper</returns>
    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(StorekeeperCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StorekeeperCoreDto>> UpdateStorekeeperAsync(
        int id, [FromBody] CreateOrUpdateStorekeeperDto storekeeperDto)
    {
        var result = await _sender.Send(
            new UpdateStorekeeperCommand(id, storekeeperDto.FullName),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Deletes a storekeeper by Id
    /// </summary>
    /// <param name="id">The storekeeper Id to delete</param>
    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteStorekeeperAsync(int id)
    {
        await _sender.Send(new DeleteStorekeeperCommand(id), HttpContext.RequestAborted);

        return NoContent();
    }
}