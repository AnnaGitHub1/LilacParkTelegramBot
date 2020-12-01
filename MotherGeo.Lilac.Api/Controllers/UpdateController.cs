using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MotherGeo.Lilac.Telegram.Interfaces;
using MotherGeo.Lilac.Telegram.Model;

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
        public async Task<IActionResult> EchoMessages(CancellationToken cancellationToken, [FromBody] RequestUpdate update = null)
        {
            if (update?.Message == null)
            {
                return Ok();
            }

            await _updateService.EchoAsync(update, cancellationToken);
            return Ok();
        }
    }
}