using System.Collections.Generic;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesGrupoController : ControllerBase
    {
        private readonly MensajesGrupoService _mensajesService;
        public MensajesGrupoController(MensajesGrupoService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet]
        public ActionResult<List<MensajesI>> Get() =>
           _mensajesService.Get();



        [HttpGet("{nombre}")] //Obtener todos los mensajes de cierto usuario
        public ActionResult<List<MensajesI>> Get(string nombre, int i)
        {
            var a= _mensajesService.Get(nombre, i);
            var b = _mensajesService.GetG(nombre, i);

            if (a!=null)
            {
                if (a.Count!=0)
                {
                    return a;
                }

            }
            if (b != null)
            {
                return b ;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<MensajesI> Create(MensajesI msg) //Crear un mensaje
        {
            _mensajesService.Create(msg);
            return Ok();
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
    public class BuscarMGController : ControllerBase
    {
        private readonly MensajesGrupoService _mensajesService;
        public BuscarMGController(MensajesGrupoService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet("{nombre}")] //Obtener todos los mensajes de cierto usuario
        public ActionResult<List<MensajesI>> Get(string nombre) =>
           _mensajesService.BuscarMensaje(nombre);
    }
}

