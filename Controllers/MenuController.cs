using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Projecto.Helper;
using Projecto.Models;
using Api.Models;
using Projecto.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Projecto.Controllers
{
    public class MenuController : Controller
    {
        ChatAPI _api = new ChatAPI();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        // GET: Menu/Create
        public static List<ContactosI> lista = new List<ContactosI>();
        public async Task<IActionResult> MisContact()
        {
            lista.Clear();
            HttpClient client = _api.Initial();

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

            var res = await client.GetAsync($"api/Contactos/{globaldata.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                var contactosUser = JsonConvert.DeserializeObject<ContactosI>(resultas); //Obtener de los datos del usuario ingresado
                lista.Add(contactosUser);
            }
            return RedirectToAction("MisContactos", "Menu", lista);
        }

        public async Task<JsonResult> MisContactJson() {
            lista.Clear();
            HttpClient client = _api.Initial();

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

            var res = await client.GetAsync($"api/Contactos/{globaldata.ActualUser.NickName}");
            if (res.IsSuccessStatusCode) {
                var resultas = res.Content.ReadAsStringAsync().Result;
                var contactosUser = JsonConvert.DeserializeObject<ContactosI>(resultas); //Obtener de los datos del usuario ingresado
                lista.Add(contactosUser);
            }
            // var result = JsonConvert.SerializeObject(lista);
            return new JsonResult(lista);
        }

        [HttpGet]
        public ActionResult MisContactos(List<ContactosI> collection)
        {
            try
            {
                // TODO: Add insert logic here

                return View(lista);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult BuscarMensaje()
        {
            return View();
        }

        public ActionResult AddContacto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddConta(string contact)
        {
            HttpClient client = _api.Initial();
            if (contact != "" && contact != null)
            {
                HttpResponseMessage res = await client.GetAsync($"api/Login/{contact}");

                string jsondata = HttpContext.Session.GetString("globaldata");
                GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

                UserData user = new UserData();
                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<UserData>(results); //Obtener de los datos del usuario ingresado
                    res = await client.GetAsync($"api/Contactos/{globaldata.ActualUser.NickName}");
                    if (res.IsSuccessStatusCode)
                    {
                        var resultas = res.Content.ReadAsStringAsync().Result;
                        var contactosUser = JsonConvert.DeserializeObject<ContactosI>(resultas); //Obtener de los datos del usuario ingresado
                        if (!(contactosUser.ContactosAmigos.Contains(user.NickName)))
                        {
                            contactosUser.ContactosAmigos.Add(user.NickName);
                            var postTask = client.PutAsJsonAsync<ContactosI>($"api/Contactos/{globaldata.ActualUser.NickName}", contactosUser);
                            postTask.Wait();
                            if (postTask.Result.IsSuccessStatusCode)
                            {
                                return RedirectToAction("Index", "Menu");
                            }
                        }

                    }
                    else
                    {
                        ContactosI nuevoContacto = new ContactosI();
                        nuevoContacto.OwnerNickName = globaldata.ActualUser.NickName;
                        nuevoContacto.ContactosAmigos = new List<string>();
                        nuevoContacto.ContactosAmigos.Add(user.NickName);

                        var postTask = client.PostAsJsonAsync<ContactosI>("api/Contactos", nuevoContacto);
                        postTask.Wait();

                        if (postTask.Result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index", "Menu");
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit()
        {
            HttpClient client = _api.Initial();

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);


            HttpResponseMessage res = await client.GetAsync($"api/Login/{globaldata.ActualUser.NickName}");
            UserData user = new UserData();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserData>(results); 
                return View(user);
            }
            ViewData["nickname"] = globaldata.ActualUser.NickName;
            return View();
        }

        public async Task<IActionResult> Borrar(string id)
        {
            var mensaje = new MensajesViewModel();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync($"api/SignIn/{id}");

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

            return Redirect("http://localhost:10999/Login" + globaldata.para);
        }

        [HttpPost]
        public ActionResult Edit(UserData userData)
        {
            try
            {
                string jsondata = HttpContext.Session.GetString("globaldata");
                GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

                if (userData.Name != null && userData.Name != "" && userData.Password != null && userData.Password != "")
                {
                    globaldata.ActualUser.Name = userData.Name;
                    globaldata.ActualUser.Password = userData.Password;

                    jsondata = JsonConvert.SerializeObject(globaldata);
                    HttpContext.Session.SetString("globaldata", jsondata);

                    HttpClient client = _api.Initial();
                    var res = client.PutAsJsonAsync($"api/SignIn/", globaldata.ActualUser);
                    res.Wait();
                    if (res.Result.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}