using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MotherGeo.Lilac.Telegram.Interfaces;
using TelegramBot;

namespace MotherGeo.Lilac.Telegram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        [Route("")]
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok("Version 1.0");
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> EchoMessages([FromBody]Update update)
        {
            await _updateService.EchoAsync(update);
            return Ok();
        }
    }
}