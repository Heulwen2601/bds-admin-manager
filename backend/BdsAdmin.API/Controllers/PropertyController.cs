using BdsAdmin.API.Data;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
    public class PropertyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PropertyController(AppDbContext context)
        {
            _context = context;
        }

        private static PropertyResponseDto ToResponseDto(Property property)
        {
            return new PropertyResponseDto
            {
                Id = property.Id,
                UserId = property.UserId,
                CategoryId = property.CategoryId,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                PricePerM2 = property.PricePerM2,
                Area = property.Area,
                Address = property.Address,
                Ward = property.Ward,
                District = property.District,
                City = property.City,
                ProjectName = property.ProjectName,
                Status = property.Status,
                ExpiredAt = property.ExpiredAt,
                ListingCode = property.ListingCode,
                ListingType = property.ListingType,
                CreatedAt = property.CreatedAt,
                UpdatedAt = property.UpdatedAt
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? keyword,
            string? city,
            Guid? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? status,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Properties.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalized = keyword.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(normalized)
                    || (p.Description != null && p.Description.ToLower().Contains(normalized))
                    || p.Address.ToLower().Contains(normalized)
                    || p.City.ToLower().Contains(normalized)
                    || (p.ProjectName != null && p.ProjectName.ToLower().Contains(normalized)));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var normalizedCity = city.Trim().ToLower();
                query = query.Where(p => p.City.ToLower() == normalizedCity);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = status.Trim().ToLower();
                query = query.Where(p => p.Status.ToLower() == normalizedStatus);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var properties = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<PropertyResponseDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = properties.Select(ToResponseDto)
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var property = await _context.Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
                return NotFound("Khong tim thay bat dong san");

            return Ok(ToResponseDto(property));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest("CategoryId is invalid.");

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
                return BadRequest("UserId is invalid.");

            var property = new Property
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                PricePerM2 = dto.PricePerM2,
                Area = dto.Area,
                Address = dto.Address,
                Ward = dto.Ward,
                District = dto.District,
                City = dto.City,
                ProjectName = dto.ProjectName,
                Status = dto.Status,
                ExpiredAt = dto.ExpiredAt,
                ListingCode = dto.ListingCode,
                ListingType = dto.ListingType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = property.Id }, ToResponseDto(property));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto dto)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound("Khong tim thay bat dong san");

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest("CategoryId is invalid.");

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
                return BadRequest("UserId is invalid.");

            property.UserId = dto.UserId;
            property.CategoryId = dto.CategoryId;
            property.Title = dto.Title;
            property.Description = dto.Description;
            property.Price = dto.Price;
            property.PricePerM2 = dto.PricePerM2;
            property.Area = dto.Area;
            property.Address = dto.Address;
            property.Ward = dto.Ward;
            property.District = dto.District;
            property.City = dto.City;
            property.ProjectName = dto.ProjectName;
            property.Status = dto.Status;
            property.ExpiredAt = dto.ExpiredAt;
            property.ListingCode = dto.ListingCode;
            property.ListingType = dto.ListingType;
            property.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(ToResponseDto(property));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound("Khong tim thay bat dong san");

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}