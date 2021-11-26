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
    public class ArchivosGrupoController : ControllerBase
    {
        private readonly MensajesGrupoService _mensajesService;
        public ArchivosGrupoController(MensajesGrupoService mensajeService)
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