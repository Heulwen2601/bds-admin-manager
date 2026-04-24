using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private static List<Property> properties = new List<Property>
        {
            new Property { Id = 1, Name = "Can ho Quan 1", Price = 2500000000, Address = "TP.HCM" },
            new Property { Id = 2, Name = "Nha pho Thu Duc", Price = 4000000000, Address = "TP.HCM" }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var property = properties.FirstOrDefault(p => p.Id == id);

            if (property == null)
                return NotFound("Khong tim thay bat dong san");

            return Ok(property);
        }

        [HttpPost]
        public IActionResult Create(Property property)
        {
            property.Id = properties.Any() ? properties.Max(p => p.Id) + 1 : 1;
            properties.Add(property);

            return Ok(property);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Property property)
        {
            var existingProperty = properties.FirstOrDefault(p => p.Id == id);

            if (existingProperty == null)
                return NotFound("Khong tim thay bat dong san");

            existingProperty.Name = property.Name;
            existingProperty.Price = property.Price;
            existingProperty.Address = property.Address;

            return Ok(existingProperty);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var property = properties.FirstOrDefault(p => p.Id == id);

            if (property == null)
                return NotFound("Khong tim thay bat dong san");

            properties.Remove(property);

            return Ok("Xoa thanh cong");
        }
    }

    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string Address { get; set; } = "";
    }
}