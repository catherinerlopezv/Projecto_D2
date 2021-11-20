using System.Collections.Generic;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserService _loginService;
        public LoginController(UserService loginService)
        {
            _loginService = loginService;
        }
        [HttpGet("{nombre}")]
        public ActionResult<UsuarioI> Get(string nombre)
        {
            var user = _loginService.Get(nombre);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet]
        public List<UsuarioI> Get()
        {
            return _loginService.Get();
        }
    }
}