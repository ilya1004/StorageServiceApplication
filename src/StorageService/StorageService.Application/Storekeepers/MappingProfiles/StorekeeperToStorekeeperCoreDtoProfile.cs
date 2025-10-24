using AutoMapper;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Domain.Entities;

namespace StorageService.Application.Storekeepers.MappingProfiles;

public class StorekeeperToStorekeeperCoreDtoProfile : Profile
{
    public StorekeeperToStorekeeperCoreDtoProfile()
    {
        CreateMap<Storekeeper, StorekeeperCoreDto>();
    }
}