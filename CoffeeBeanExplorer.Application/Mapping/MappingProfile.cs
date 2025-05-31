using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? string.Empty))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? string.Empty));

        CreateMap<Bean, BeanDto>()
            .ForMember(dest => dest.OriginCountry,
                opt => opt.MapFrom(src => src.Origin != null ? src.Origin.Country : string.Empty))
            .ForMember(dest => dest.OriginRegion,
                opt => opt.MapFrom(src => src.Origin != null ? src.Origin.Region : null))
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src =>
                    src.BeanTags != null
                        ? src.BeanTags.Select(bt => new TagDto
                            { Id = bt.Tag != null ? bt.Tag.Id : 0, Name = bt.Tag != null ? bt.Tag.Name : string.Empty })
                        : Enumerable.Empty<TagDto>()));

        CreateMap<Origin, OriginDto>();

        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
            .ForMember(dest => dest.BeanName,
                opt => opt.MapFrom(src => src.Bean != null ? src.Bean.Name : string.Empty));

        CreateMap<Tag, TagDto>();

        CreateMap<UserList, UserListDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items != null
                ? src.Items.Select(item => new ListItemDto
                {
                    ListId = item.ListId,
                    BeanId = item.BeanId,
                    BeanName = item.Bean != null ? item.Bean.Name : string.Empty,
                    OriginCountry = item.Bean != null && item.Bean.Origin != null
                        ? item.Bean.Origin.Country
                        : string.Empty,
                    OriginRegion = item.Bean != null && item.Bean.Origin != null ? item.Bean.Origin.Region : null,
                    Price = item.Bean != null ? item.Bean.Price : 0
                })
                : Enumerable.Empty<ListItemDto>()));
    }
}