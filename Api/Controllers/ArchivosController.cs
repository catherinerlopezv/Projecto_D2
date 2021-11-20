using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Services;
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivosController : ControllerBase
    {
        private readonly MensajesService _mensajesService;
        public ArchivosController(MensajesService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet("{nombre}")]
        public ActionResult<MensajesI> Get(string nombre)
        {
            var user = _mensajesService.GetFile(nombre);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

    }
}