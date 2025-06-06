using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Application.Mapping;

public class BeanProfile : Profile
{
    public BeanProfile()
    {
        CreateMap<BeanTag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag != null ? src.Tag.Id : 0))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag != null ? src.Tag.Name : string.Empty));

        CreateMap<Bean, BeanDto>()
            .ForMember(dest => dest.OriginCountry,
                opt => opt.MapFrom(src => src.Origin != null ? src.Origin.Country : string.Empty))
            .ForMember(dest => dest.OriginRegion,
                opt => opt.MapFrom(src => src.Origin != null ? src.Origin.Region : null))
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src.BeanTags));

        CreateMap<CreateBeanDto, Bean>();
        CreateMap<UpdateBeanDto, Bean>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}