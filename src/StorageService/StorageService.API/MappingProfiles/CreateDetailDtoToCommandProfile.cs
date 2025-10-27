using AutoMapper;
using StorageService.API.RequestDtos.Input;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Details.UseCases.CreateDetail;

namespace StorageService.API.MappingProfiles;

public class CreateDetailDtoToCommandProfile : Profile
{
    public CreateDetailDtoToCommandProfile()
    {
        CreateMap<CreateDetailDto, CreateDetailCommand>()
            .ConstructUsing(x => new CreateDetailCommand(
                x.NomenclatureCode,
                x.Name,
                x.Count,
                x.StorekeeperId,
                x.CreatedAtDate));
    }
}