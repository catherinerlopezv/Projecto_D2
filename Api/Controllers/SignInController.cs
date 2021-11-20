using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Services;
namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly UserService _loginService;
        public SignInController(UserService loginService) 
        {
            _loginService = loginService;
        }

        [HttpPost]
        public ActionResult<UsuarioI> Create(UsuarioI user)
        {
            List<UsuarioI> usuarios = _loginService.Get();    
            if (!(usuarios.Exists(x => x.NickName == user.NickName)))
            {
                user.Code = usuarios.Count + 1;
                _loginService.Create(user);
                return StatusCode(201);
            }
            else
            {
                return StatusCode(409);
            }

        }

        [HttpDelete("{nombre}")]
        public IActionResult Delete(string nombre)
        {
            _loginService.Remove(nombre);
            return NoContent();
        }

        [HttpPut]
        public IActionResult Update([FromBody] UsuarioI user)
        {
            var NewData = _loginService.Get(user.NickName);
            if (NewData == null)
            {
                return NotFound();
            }
            user.Id = NewData.Id;
            _loginService.Update(user.NickName, user);
            return NoContent();

        }

    }
}