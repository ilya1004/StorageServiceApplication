using AutoMapper;
using StorageService.API.RequestDtos.Input;
using StorageService.Application.Details.UseCases.CreateDetail;
using StorageService.Application.Details.UseCases.UpdateDetail;

namespace StorageService.API.MappingProfiles;

public class CreateOrUpdateDetailDtoToCommandProfile : Profile
{
    public CreateOrUpdateDetailDtoToCommandProfile()
    {
        CreateMap<CreateOrUpdateDetailDto, CreateDetailCommand>()
            .ConstructUsing(x => new CreateDetailCommand(
                x.NomenclatureCode,
                x.Name,
                x.Count,
                x.StorekeeperId,
                x.CreatedAtDate));

        CreateMap<CreateOrUpdateDetailDto, UpdateDetailCommand>()
            .ConstructUsing(x => new UpdateDetailCommand(
                0,
                x.NomenclatureCode,
                x.Name,
                x.Count,
                x.StorekeeperId,
                x.CreatedAtDate));
    }
}