using System.Collections.Generic;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly MensajesService _mensajesService;
        public MensajesController(MensajesService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet]
        public ActionResult<List<MensajesI>> Get() =>
           _mensajesService.Get();



        [HttpGet("{nombre}")] //Obtener todos los mensajes de cierto usuario
        public ActionResult<List<MensajesI>> Get(string nombre) =>
            _mensajesService.Get(nombre);

        [HttpPost]
        public ActionResult<MensajesI> Create(MensajesI msg) //Crear un mensaje
        {
            _mensajesService.Create(msg);
            return NoContent();
        }

        [HttpDelete("{texto}")]
        public IActionResult Delete(string texto) //Borrar un mensaje para ambos
        {
            var mensaje = _mensajesService.GetId(texto);
            if (mensaje == null)
            {
                return NotFound();
            }
            _mensajesService.Remove(mensaje.Id);
            return NoContent();
        }

        [HttpPut("{eliminar}")]
        public IActionResult Update(string eliminar, MensajesI mensaje)
        {
            var msg = _mensajesService.GetId(eliminar);
            if (msg == null)
            {
                return NotFound();
            }
            _mensajesService.BorrarParcial(eliminar, mensaje);
            return NoContent();
        }



    }



    [Route("api/[controller]")]
    [ApiController]
    public class BuscarMController : ControllerBase
    {
        private readonly MensajesService _mensajesService;
        public BuscarMController(MensajesService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet("{nombre}")] //Obtener todos los mensajes de cierto usuario
        public ActionResult<List<MensajesI>> Get(string nombre) =>
           _mensajesService.BuscarMensaje(nombre);
    }
}

