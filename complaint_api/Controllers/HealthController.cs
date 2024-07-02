using Microsoft.AspNetCore.Mvc;

namespace complaint_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "Healthy", version = "1.0.0" });
        }
    }
}