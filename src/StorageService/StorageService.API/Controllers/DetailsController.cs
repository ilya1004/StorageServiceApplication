using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StorageService.API.RequestDtos.Input;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Details.UseCases.CreateDetail;
using StorageService.Application.Details.UseCases.DeleteDetail;
using StorageService.Application.Details.UseCases.GetDetails;
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

    [HttpPost]
    public async Task<ActionResult<DetailCoreDto>> CreateAsync([FromBody] CreateDetailDto createDetailDto)
    {
        var result = await _sender.Send(
            _mapper.Map<CreateDetailCommand>(createDetailDto),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResultModel<DetailCoreDto>>> GetAllDetailsAsync(
        [FromQuery] PaginatedRequestDto requestDto)
    {
        var result = await _sender.Send(
            new GetDetailsQuery(requestDto.PageNo, requestDto.PageSize),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _sender.Send(new DeleteDetailCommand(id), HttpContext.RequestAborted);

        return NoContent();
    }
}