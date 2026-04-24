using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok("List users");
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok($"User {id}");
        }

        [HttpPost]
        public IActionResult Create()
        {
            return Ok("Create user");
        }
    }
}