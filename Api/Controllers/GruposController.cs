using Api.Models;
using Api.Services;
using System.Reflection.Emit;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GruposController : ControllerBase
    {
        private readonly GruposService _contactosService;
        public GruposController(GruposService contactosService)
        {
            _contactosService = contactosService;
        }
        [HttpGet("{nombre}")]
        public  List<Grupos> Get(string nombre)
        {
            
            var user = _contactosService.GetT(nombre);
            var user2 = _contactosService.GetTG(nombre);
            
      

            if (user2.Count != 0)
            {
                return user2;
            }
            else if( user.Count != 0)
            {
                return user;
            }
            return null;
        }

        [HttpGet("GetT/{nombre}")]
        public ActionResult<Grupos> Get2(string nombre)
        {

            var user = _contactosService.Get(nombre);
            var user2 = _contactosService.GetG(nombre);



            if (user2 != null)
            {
                return user2;
            }
            else if (user != null)
            {
                return user;
            }
            return NotFound();
        }

        //Creación de un nuevo usuario
        [HttpPost]
        public ActionResult<Grupos> Create(Grupos contactos)
        {
            _contactosService.Create(contactos);
            return StatusCode(201);
        }

        [HttpPut("{Nombre}")]
        public IActionResult Update(string Nombre, Grupos contactos) 
        {
            var contacts = _contactosService.Get(Nombre);
            if (contacts == null)
            {
                return NotFound();
            }
            contactos.Id = contacts.Id;
            _contactosService.Update(Nombre, contactos);
            return NoContent();
        }
    }
}
