using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MotherGeo.Lilac.Telegram.Interfaces;

namespace MotherGeo.Lilac.Telegram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveController : ControllerBase
    {
        private readonly IUpdateService _updateService;

        public SaveController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        [Route("")]
        [HttpGet]
        public IActionResult SaveFotos()
        {
            _updateService.SaveFoto(1);

            _updateService.SaveFoto(7);

            return Ok();
        }
    }
}