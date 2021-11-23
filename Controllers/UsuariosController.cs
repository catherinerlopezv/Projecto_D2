using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Projecto.Models;

namespace Projecto.Controllers
{
    public class UsuariosController : Controller
    {
        string Baseurl = "http://localhost:24733/";
        public async Task<ActionResult> Index()
        {
            List<UserData> users = new List<UserData>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/Login/");
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<UserData>>(Response);

                }
                return View(users);
            }
        }
    }
}