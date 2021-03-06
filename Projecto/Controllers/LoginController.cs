using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        public ActionResult Index()
        {
            //Limpiar variables globales
            GlobalData globaldata = new GlobalData();
            string jsondata = JsonConvert.SerializeObject(globaldata);
            HttpContext.Session.SetString("globaldata", jsondata);
            
            return View();
        }
      
        [HttpPost]
        public async Task<IActionResult> Ingresar(string username, string password)
        {
            GlobalData globaldata = new GlobalData();
            
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
                    globaldata.ActualUser = user;
                    string jsondata = JsonConvert.SerializeObject(globaldata);
                    HttpContext.Session.SetString("globaldata", jsondata);
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
