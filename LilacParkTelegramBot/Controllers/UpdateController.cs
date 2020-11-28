using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LylacParkServices.Interfaces;
using TelegramBot;

namespace LilacParkTelegramBot.Controllers
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

        [HttpPost]
        public async Task<IActionResult> EchoMessages([FromBody]Update update)
        {
            await _updateService.EchoAsync(update);
            return Ok();
        }
    }
}