using CoffeeBeanExplorer.Application.DTOs;

namespace CoffeeBeanExplorer.Application.Services.Interfaces;

public interface IBeanService
{
    IEnumerable<BeanDto> GetAllBeans();
    BeanDto? GetBeanById(int id);
    BeanDto CreateBean(CreateBeanDto dto);
    bool UpdateBean(int id, UpdateBeanDto dto);
    bool DeleteBean(int id);
}