using BdsAdmin.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private static List<Property> properties = new List<Property>
        {
            new Property { Id = Guid.NewGuid(), Title = "Can ho Quan 1", Price = 2500000000, Address = "TP.HCM", City = "TP.HCM", Area = 0 },
            new Property { Id = Guid.NewGuid(), Title = "Nha pho Thu Duc", Price = 4000000000, Address = "TP.HCM", City = "TP.HCM", Area = 0 }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var property = properties.FirstOrDefault(p => p.Id == id);

            if (property == null)
                return NotFound("Khong tim thay bat dong san");

            return Ok(property);
        }

        [HttpPost]
        public IActionResult Create(Property property)
        {
            property.Id = Guid.NewGuid();
            properties.Add(property);

            return Ok(property);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Property property)
        {
            var existingProperty = properties.FirstOrDefault(p => p.Id == id);

            if (existingProperty == null)
                return NotFound("Khong tim thay bat dong san");

            existingProperty.Title = property.Title;
            existingProperty.Price = property.Price;
            existingProperty.Address = property.Address;

            return Ok(existingProperty);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var property = properties.FirstOrDefault(p => p.Id == id);

            if (property == null)
                return NotFound("Khong tim thay bat dong san");

            properties.Remove(property);

            return Ok("Xoa thanh cong");
        }
    }
}