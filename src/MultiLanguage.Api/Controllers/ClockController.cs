using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MultiLanguage.Api.Models;
using System.Xml.Linq;

namespace MultiLanguage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClockController : ControllerBase
    {

        private readonly IStringLocalizer<ClockController> _stringLocalizer;

        public ClockController(IStringLocalizer<ClockController> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        public IActionResult GetTime()
        {
            var message = string.Format
            (
                _stringLocalizer["NOW"],
                DateTime.Now.ToString("T"),
                DateTime.Now.ToString("D")
            );
            return Ok(new ClockInfoResponse(message));
        }

    }
}
