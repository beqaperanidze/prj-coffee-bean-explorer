using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Application.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
            .ForMember(dest => dest.BeanName,
                opt => opt.MapFrom(src => src.Bean != null ? src.Bean.Name : string.Empty));

        CreateMap<CreateReviewDto, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Bean, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<UpdateReviewDto, Review>()
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
    }
}