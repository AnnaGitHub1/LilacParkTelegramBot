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
        private ISaveService _saveService;

        public SaveController(ISaveService saveService)
        {
            _saveService = saveService;
        }

        [Route("")]
        [HttpGet]
        public IActionResult SaveFotos()
        {
            _saveService.SaveFoto(1);

            _saveService.SaveFoto(7);

            return Ok();
        }
    }
}