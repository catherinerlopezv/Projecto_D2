using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactosController : ControllerBase
    {
        private readonly ContactosService _contactosService;
        public ContactosController(ContactosService contactosService)
        {
            _contactosService = contactosService;
        }


        [HttpGet("{nombre}")]
        public ActionResult<ContactosI> Get(string nombre)
        {
            var user = _contactosService.Get(nombre);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }



        //Creación de un nuevo usuario
        [HttpPost]
        public ActionResult<ContactosI> Create(ContactosI contactos)
        {
            _contactosService.Create(contactos);
            return StatusCode(201);
        }

        [HttpPut("{Nombre}")]
        public IActionResult Update(string Nombre, ContactosI contactos) //Editar alguna pizza ya creada
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