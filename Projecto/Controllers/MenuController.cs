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
using MongoDB.Bson;
using MongoDB.Driver;

namespace Projecto.Controllers
{
    public class MenuController : Controller
    {
        readonly ChatAPI _api = new ChatAPI();
        private MyGlobalData GlobalData = new MyGlobalData();

        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        // GET: Menu/Create
        public static List<ContactosI> lista = new List<ContactosI>();
        public static List<UsuarioI> lista3= new List<UsuarioI>();
        public static List<UsuarioI> lista4 = new List<UsuarioI>();
        public async Task<IActionResult> MisContact()
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            lista.Clear();
            HttpClient client = _api.Initial();
            var res = await client.GetAsync($"api/Contactos/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                ContactosI contactosUser = JsonConvert.DeserializeObject<ContactosI>(resultas); //Obtener de los datos del usuario ingresado
                lista.Add(contactosUser);
            }
            return RedirectToAction("Ghost", "Grupos", lista);
        }
        [HttpGet]
        public ActionResult MisContactos()
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

        public async Task<ActionResult> AddContactoAsync()
        {
             GlobalData.obtieneSesion(HttpContext.Session);
            HttpClient client = _api.Initial();
            var res = await client.GetAsync($"api/Login/");
            if (res.IsSuccessStatusCode)
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                List<UsuarioI> contactosUser = JsonConvert.DeserializeObject<List<UsuarioI>>(resultas); //Obtener de los datos del usuario ingresado
                var contactos = contactosUser;

                for (int i = 0; i < contactosUser.Count; i++)
                {
                    if (contactosUser[i].Requests != null )
                    {
                        if (contactosUser[i].Requests.Count != 0)
                        {
                            for (int j = 0; j < contactosUser[i].Requests.Count; j++)
                            {

                                for (int k = 0; k < lista.Count; k++)
                                {
                                    for (int l = 0; l < lista[k].ContactosAmigos.Count; l++)
                                    {
                                        contactosUser = contactosUser.FindAll(x => x.NickName != lista[k].ContactosAmigos[l]);
                                        if (contactosUser.Count == 0)
                                        {
                                            break;
                                        }
                                    }
                                    if (contactosUser.Count == 0)
                                    {
                                        break;
                                    }
                                }
                                if (contactosUser.Count == 0)
                                {
                                    break;
                                }
                                contactosUser = contactosUser.FindAll(x => x.NickName != GlobalData.ActualUser.NickName);
                                if (contactosUser.Count<=i)
                                {
                                    break;
                                }
                                if (contactosUser[i].Requests==null)
                                {
                                    break;

                                }
                            }
                        }
                    }
                }

            A:
                for (int i = 0; i < contactosUser.Count; i++)
                {

                    if (contactosUser[i].Requests != null)
                    {
                        for (int j = 0; j < contactosUser[i].Requests.Count; j++)
                        {
                            if (contactosUser[i].Requests[j] == GlobalData.ActualUser.NickName || contactosUser[i].NickName == GlobalData.ActualUser.NickName)
                            {
                                contactosUser.RemoveAt(i);
                                goto A;
                            }


                            if (contactosUser.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (contactosUser[i].NickName == GlobalData.ActualUser.NickName)
                        {
                            contactosUser.RemoveAt(i);
                            goto A;
                        }
                    }
                }
            B:
                for (int i = 0; i < contactosUser.Count; i++)
                {
                    if (lista != null&&lista.Count!=0)
                    {


                        for (int j = 0; j < lista[0].ContactosAmigos.Count; j++)
                        {
                            if (contactosUser[i].NickName == lista[0].ContactosAmigos[j])
                            {
                                contactosUser = contactosUser.FindAll(x => x.NickName != lista[0].ContactosAmigos[j]);
                                goto B;
                            }
                            if (contactosUser[i].NickName==GlobalData.ActualUser.NickName)
                            {
                                contactosUser = contactosUser.FindAll(x => x.NickName != GlobalData.ActualUser.NickName);
                                goto B;
                            }
                        }
                    }
                }



                
                contactos = contactos.FindAll(x => x.NickName == GlobalData.ActualUser.NickName);
                if (contactos.Count!=0)
                {
                    if (contactos[0].Requests == null)
                    {
                        contactos[0].Requests = new List<string>();

                    }
                    for (int i = 0; i < contactos[0].Requests.Count; i++)
                    {
                        var temp=contactosUser.FindAll(x => x.NickName == contactos[0].Requests[i]);
                        if (temp != null)
                        {
                            contactosUser= contactosUser.FindAll(x => x.NickName != contactos[0].Requests[i]);
                        }

                    }
                }
               
               
                lista3 = contactosUser;
                lista4 = contactos;
                if (lista3.SequenceEqual(lista4))
                {
                    lista3 = new List<UsuarioI>();
                    lista4 = new List<UsuarioI>();
                }
            }


            var tupleModel = new Tuple<List<UsuarioI>, List<UsuarioI>>(lista3, lista4);


            ViewData["NickName"] = GlobalData.ActualUser.NickName;
            return View(tupleModel);

        }

        public async Task<IActionResult> GrupoAsync(List<string> integrantes, string usuarioLogueado)
        {


            for (int i = 0; i < integrantes.Count; i++)
            {
                HttpClient client2 = _api.Initial();
                var res2 = await client2.GetAsync($"api/SignIn/GetT/{integrantes[i]},{usuarioLogueado},{i.ToString()}");
            }


            return RedirectToAction("Index", "Menu");
        }
        public async Task<IActionResult> AceptarAsync(List<string> integrantes, string usuarioLogueado, string Aceptar)
        {
            if (Aceptar == "Aceptar")
            {
                for (int i = 0; i < integrantes.Count; i++)
                {
                    await AddConta(integrantes[i], usuarioLogueado);
                    await AddConta(usuarioLogueado, integrantes[i]);
                }
                for (int i = 0; i < integrantes.Count; i++)
                {
                    HttpClient client2 = _api.Initial();
                    var res2 = await client2.GetAsync($"api/SignIn/GetT/{"borrar"},{usuarioLogueado},{integrantes[i]}");
                    var res3 = await client2.GetAsync($"api/SignIn/GetT/{"borrar"},{integrantes[i]},{usuarioLogueado}");
                }
            }
            else if (Aceptar=="Cancelar")
            {
                for (int i = 0; i < integrantes.Count; i++)
                {
                    HttpClient client2 = _api.Initial();
                    var res2 = await client2.GetAsync($"api/SignIn/GetT/{"borrar"},{usuarioLogueado},{integrantes[i]}");
                    var res3 = await client2.GetAsync($"api/SignIn/GetT/{"borrar"},{integrantes[i]},{usuarioLogueado}");
                }
            }



            return RedirectToAction("Index", "Menu");
        }
        private MongoClient Conexion()
        {
            var cliente = new MongoClient("mongodb+srv://uwu:321@death.rfwpy.mongodb.net/admin?authSource=admin&replicaSet=atlas-wwl3nk-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
            return cliente;
        }


       





        [HttpPost]
        public async Task<IActionResult> AddConta(string contact,string actual)
        {
            HttpClient client = _api.Initial();
            if (contact != "" && contact != null)
            {
                HttpResponseMessage res = await client.GetAsync($"api/Login/{contact}");
                _ = new UserData();
                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    UserData user = JsonConvert.DeserializeObject<UserData>(results);
                    res = await client.GetAsync($"api/Contactos/{actual}");
                    if (res.IsSuccessStatusCode)
                    {
                        var resultas = res.Content.ReadAsStringAsync().Result;
                        var contactosUser = JsonConvert.DeserializeObject<ContactosI>(resultas); //Obtener de los datos del usuario ingresado
                        if (!(contactosUser.ContactosAmigos.Contains(user.NickName)))
                        {
                            contactosUser.ContactosAmigos.Add(user.NickName);
                            var postTask = client.PutAsJsonAsync<ContactosI>($"api/Contactos/{actual}", contactosUser);
                            postTask.Wait();
                            if (postTask.Result.IsSuccessStatusCode)
                            {
                                return RedirectToAction("Index", "Menu");
                            }
                        }

                    }
                    else
                    {
                        ContactosI nuevoContacto = new ContactosI
                        {
                            OwnerNickName = actual,
                            ContactosAmigos = new List<string>()
                        };
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
           GlobalData.obtieneSesion(HttpContext.Session);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Login/{GlobalData.ActualUser.NickName}");
            _ = new UserData();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                UserData user = JsonConvert.DeserializeObject<UserData>(results);
                return View(user);
            }
            ViewData["NickName"] = GlobalData.ActualUser.NickName;
            return View();
        }

        public async Task<IActionResult> Borrar(string id)
        {
           GlobalData.obtieneSesion(HttpContext.Session);
            _ = new MensajesViewModel();
            HttpClient client = _api.Initial();
            _ = await client.DeleteAsync($"api/SignIn/{id}");

            return Redirect("http://localhost:10999/Login" + GlobalData.ParaGrupos);
        }

        [HttpPost]
        public ActionResult Edit(UserData userData)
        {
             GlobalData.obtieneSesion(HttpContext.Session);
            try
            {
                if (userData.Name != null && userData.Name != "" && userData.Password != null && userData.Password != "")
                {
                    GlobalData.ActualUser.Name = userData.Name;
                    GlobalData.ActualUser.Password = userData.Password;
                    GlobalData.actualizaSesion(HttpContext.Session);
                    HttpClient client = _api.Initial();
                    var res = client.PutAsJsonAsync($"api/SignIn/", GlobalData.ActualUser);
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