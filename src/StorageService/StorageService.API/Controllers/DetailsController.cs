using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StorageService.API.RequestDtos.Input;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Details.UseCases.CreateDetail;
using StorageService.Application.Details.UseCases.DeleteDetail;
using StorageService.Application.Details.UseCases.GetDetailById;
using StorageService.Application.Details.UseCases.GetDetails;
using StorageService.Application.Details.UseCases.UpdateDetail;
using StorageService.Domain.Models;

namespace StorageService.API.Controllers;

[ApiController]
[Route("api/details")]
public class DetailsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public DetailsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    /// <summary>
    /// Create detail
    /// </summary>
    /// <param name="detailDto">Detail to create</param>
    /// <returns>Created detail</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DetailCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DetailCoreDto>> CreateDetailAsync(
        [FromBody] CreateOrUpdateDetailDto detailDto)
    {
        var result = await _sender.Send(
            _mapper.Map<CreateDetailCommand>(detailDto),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get a paginated list of all non-deleted details
    /// </summary>
    /// <param name="requestDto">Pagination parameters</param>
    /// <returns>Paginated result</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResultModel<DetailCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResultModel<DetailCoreDto>>> GetAllDetailsAsync(
        [FromQuery] PaginatedRequestDto requestDto)
    {
        var result = await _sender.Send(
            new GetDetailsQuery(requestDto.PageNo, requestDto.PageSize),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get detail by Id
    /// </summary>
    /// <param name="id">Detail Id</param>
    /// <returns>The detail by Id</returns>
    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(DetailCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DetailCoreDto>> GetDetailByIdAsync(int id)
    {
        var result = await _sender.Send(
            new GetDetailByIdQuery(id),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update detail by Id
    /// </summary>
    /// <param name="id">The detail Id to update</param>
    /// <param name="detailDto">Detail data to update</param>
    /// <returns>Updated detail</returns>
    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(DetailCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DetailCoreDto>> UpdateDetailAsync(
        int id, [FromBody] CreateOrUpdateDetailDto detailDto)
    {
        var command = _mapper.Map<UpdateDetailCommand>(detailDto) with { Id = id };

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Deletes a detail by Id
    /// </summary>
    /// <param name="id">The detail Id to delete</param>
    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(DetailCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteDetailAsync(int id)
    {
        await _sender.Send(new DeleteDetailCommand(id), HttpContext.RequestAborted);

        return NoContent();
    }
}