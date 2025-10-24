using AutoMapper;
using StorageService.Application.Details.Dtos;
using StorageService.Domain.Entities;

namespace StorageService.Application.Details.MappingProfiles;

public class DetailToDetailCoreDtoProfile : Profile
{
    public DetailToDetailCoreDtoProfile()
    {
        CreateMap<Detail, DetailCoreDto>();
    }
}