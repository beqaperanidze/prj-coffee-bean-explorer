using CoffeeBeanExplorer.Application.DTOs;
    using CoffeeBeanExplorer.Domain.Models;
    using Microsoft.AspNetCore.Mvc;
    
    namespace CoffeeBeanExplorer.Controllers;
    
    [ApiController]
    [Route("api/[controller]")]
    public class BeanController : ControllerBase
    {
        private static readonly List<Bean> _beans = [];
        private static int _nextId = 1;
    
        /// <summary>
        /// Get all coffee beans.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<BeanDto>> GetAll()
        {
            var dtos = _beans.Select(MapToDto).ToList();
            return Ok(dtos);
        }
    
        /// <summary>
        /// Get a bean by its ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public ActionResult<BeanDto> GetById(int id)
        {
            var bean = _beans.FirstOrDefault(b => b.Id == id);
            if (bean is null) return NotFound();
            return Ok(MapToDto(bean));
        }
    
        /// <summary>
        /// Create a new coffee bean.
        /// </summary>
        [HttpPost]
        public ActionResult<BeanDto> Create(CreateBeanDto dto)
        {
            var bean = new Bean
            {
                Id = _nextId++,
                Name = dto.Name,
                OriginId = dto.OriginId,
                RoastLevel = dto.RoastLevel,
                Description = dto.Description,
                Price = dto.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Origin = new Origin() 
            };
    
            _beans.Add(bean);
            return CreatedAtAction(nameof(GetById), new { id = bean.Id }, MapToDto(bean));
        }
    
        /// <summary>
        /// Update an existing bean.
        /// </summary>
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, UpdateBeanDto dto)
        {
            var bean = _beans.FirstOrDefault(b => b.Id == id);
            if (bean is null) return NotFound();
    
            bean.Name = dto.Name;
            bean.OriginId = dto.OriginId;
            bean.RoastLevel = dto.RoastLevel;
            bean.Description = dto.Description;
            bean.Price = dto.Price;
            bean.UpdatedAt = DateTime.UtcNow;
    
            return NoContent();
        }
    
        /// <summary>
        /// Delete a bean by its ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var bean = _beans.FirstOrDefault(b => b.Id == id);
            if (bean is null) return NotFound();
    
            _beans.Remove(bean);
            return NoContent();
        }
    
        private static BeanDto MapToDto(Bean bean) => new BeanDto
        {
            Id = bean.Id,
            Name = bean.Name,
            OriginId = bean.OriginId,
            OriginCountry = bean.Origin?.Country ?? string.Empty,
            OriginRegion = bean.Origin?.Region,
            RoastLevel = bean.RoastLevel,
            Description = bean.Description,
            Price = bean.Price,
            CreatedAt = bean.CreatedAt,
            UpdatedAt = bean.UpdatedAt
        };
    }
