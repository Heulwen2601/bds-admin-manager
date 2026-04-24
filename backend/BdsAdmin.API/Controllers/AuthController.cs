using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login()
        {
            return Ok("Login success");
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok("Register success");
        }
    }
}