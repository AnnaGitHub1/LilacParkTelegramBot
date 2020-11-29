using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MotherGeo.Lilac.Telegram.Interfaces;
using TelegramBot;

namespace MotherGeo.Lilac.Telegram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IUpdateService _updateService;
        private readonly ILogger<Update> _logger;


        public UpdateController(IUpdateService updateService, ILogger<Update> logger)
        {
            _updateService = updateService;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok("Version 1.0");
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> EchoMessages([FromBody]Update update, CancellationToken cancellationToken)
        {
            _logger.LogDebug(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            await _updateService.EchoAsync(update, cancellationToken);
            return Ok();
        }
    }
}