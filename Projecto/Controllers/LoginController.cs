using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Projecto.Helper;
using Projecto.Models;
using Projecto.Utilities;
using Newtonsoft.Json;
using Archivos;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Projecto.Controllers
{
  public class LoginController : Controller
  {
    ChatAPI _api = new ChatAPI();
    private MyGlobalData GlobalData = new MyGlobalData();

    public ActionResult Index()
    {
      //Limpiar variables globales
      GlobalData.obtieneSesion(HttpContext.Session);
      GlobalData.Receptor = null;
      GlobalData.ActualUser = null;
      GlobalData.ParaGrupos = new List<string>();
      GlobalData.ArchivoEntrada = null;
      GlobalData.ArchivoSalida = null;
      GlobalData.actualizaSesion(HttpContext.Session);

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ingresar(string username, string password)
    {
      GlobalData.obtieneSesion(HttpContext.Session);

      if (password == null)
      {
        password = " ";
      }
      HttpClient client = _api.Initial();
      HttpResponseMessage res = await client.GetAsync($"api/Login/{username}");
      UserData user = new UserData();
      if (res.IsSuccessStatusCode)
      {
        var results = res.Content.ReadAsStringAsync().Result;
        user = JsonConvert.DeserializeObject<UserData>(results);
        Ceasar cesar = new Ceasar();
        password = cesar.Cifrar(password, "1234567890'¿qwertyuiop");
        if (user.Password == password)
        {
          GlobalData.ActualUser = user;
          // actualiza la sesion
          GlobalData.actualizaSesion(HttpContext.Session);
          return RedirectToAction("Index", "Menu");
        }
        else
        {
          return Content("Error con la autenticación, contacte al administrador");
        }

      }
      return Content("Error con la autenticación, contacte con el administrador");
    }


  }
}
