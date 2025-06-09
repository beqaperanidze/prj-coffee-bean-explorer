using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Application.Mapping;

public class UserListProfile : Profile
{
    public UserListProfile()
    {
        CreateMap<ListItem, ListItemDto>()
            .ForMember(dest => dest.BeanName,
                opt => opt.MapFrom(src => src.Bean != null ? src.Bean.Name : string.Empty))
            .ForMember(dest => dest.OriginCountry,
                opt => opt.MapFrom(src =>
                    src.Bean != null && src.Bean.Origin != null ? src.Bean.Origin.Country : string.Empty))
            .ForMember(dest => dest.OriginRegion,
                opt => opt.MapFrom(src => src.Bean != null && src.Bean.Origin != null ? src.Bean.Origin.Region : null))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Bean != null ? src.Bean.Price : 0));

        CreateMap<UserList, UserListDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<CreateUserListDto, UserList>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.MapFrom(_ => new List<ListItem>()));

        CreateMap<UpdateUserListDto, UserList>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}