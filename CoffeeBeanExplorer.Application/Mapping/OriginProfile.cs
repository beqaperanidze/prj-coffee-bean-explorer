using AutoMapper;
using CoffeeBeanExplorer.Application.DTOs;
using CoffeeBeanExplorer.Domain.Models;

namespace CoffeeBeanExplorer.Application.Mapping;

public class OriginProfile : Profile
{
    public OriginProfile()
    {
        CreateMap<Origin, OriginDto>();
    }
}