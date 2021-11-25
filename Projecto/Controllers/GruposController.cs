using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projecto.Models;
using MongoDB.Driver;
using System.Net.Http;
using Api.Models;
using Newtonsoft.Json;
using Projecto.Helper;
using System.Dynamic;

namespace Projecto.Controllers
{
    public class GruposController : Controller
    {
        public static List<ContactosI> lista = new List<ContactosI>();
        readonly ChatAPI _api = new ChatAPI();

        private MyGlobalData GlobalData = new MyGlobalData();
        public async Task<IActionResult> Index()
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            lista.Clear();
            HttpClient client = _api.Initial();
            var res = await client.GetAsync($"api/Contactos/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                var contactosUser = JsonConvert.DeserializeObject<ContactosI>(resultas);
                lista.Add(contactosUser);
            }
            return RedirectToAction("MisContactos", "Grupos", lista);
        }


        public static List<Grupos> lista2 = new List<Grupos>();
        private async Task<List<ContactosI>> GetTeacherAsync()
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            lista.Clear();
            HttpClient client = _api.Initial();
            var res = await client.GetAsync($"api/Contactos/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                ContactosI contactosUser = JsonConvert.DeserializeObject<ContactosI>(resultas);
                lista.Add(contactosUser);
            }
            return (lista);
        }
        public async Task<List<Grupos>> GetStudentsAsync()
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            lista2.Clear();
            HttpClient client = _api.Initial();
            var res = await client.GetAsync($"api/Grupos/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode && res.StatusCode.ToString()!="NoContent")
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                List<Grupos> contactosUser = JsonConvert.DeserializeObject<List<Grupos>>(resultas);
                lista2 = contactosUser;
            }
            
            
          

            return (lista2);
        }


        [HttpGet]
        public ActionResult MisContactos()
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            try
            {
                // TODO: Add insert logic here
                ViewData["NickName"] = GlobalData.ActualUser.NickName;
                return View(lista);
            }
            catch
            {
                return View();
            }
        }

        /*
        public ActionResult Ghost()
        {
            ViewBag.Message = "Welcome to my demo!";
            ViewBag.Teachers = GetTeachers();
            ViewBag.Students = GetStudents();
            return View();
        }
        */


        public async Task<ActionResult> GhostAsync()
        {
            var tupleModel = new Tuple<IEnumerable<ContactosI>, IEnumerable<Grupos>>(await GetTeacherAsync(), await GetStudentsAsync());
          
            return View(tupleModel);
            
        }
        
        [HttpPost]
        public IActionResult Grupo(List<string> integrantes, string usuarioLogueado, string nombreGrupo)
        {

            CrearGrupo(nombreGrupo, usuarioLogueado);

           

            ActualizarIntegrantesGrupo(integrantes, usuarioLogueado, nombreGrupo);

            return RedirectToAction("Index", "Menu");
        }
        public void CrearGrupo(string nombreDelGrupo, string usuarioCreador)
        {
            var database = Conexion().GetDatabase("Death");
            var collection = database.GetCollection<BsonDocument>("Grupos");
          
            var _collection = database.GetCollection<BsonDocument>("Grupos");
            var filter = Builders<BsonDocument>.Filter.Eq("Grupo", nombreDelGrupo);
            var result = _collection.Find(filter).ToList();
            if (result.Count!=0)
            {
                nombreDelGrupo = nombreDelGrupo + (result.Count + 1);
            }
            var document = new BsonDocument
            {
                { "Grupo", nombreDelGrupo },
                { "Usuario", usuarioCreador },
                { "Amigos", new BsonArray() }
            };

            collection.InsertOne(document);
        }
        private MongoClient Conexion()
        {
            var cliente = new MongoClient("mongodb+srv://uwu:321@death.rfwpy.mongodb.net/admin?authSource=admin&replicaSet=atlas-wwl3nk-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
            return cliente;
        }

        public bool ActualizarIntegrantesGrupo(List<string> integrantes, string usuarioCreador, string nombreGrupo)
        {
            var database = Conexion().GetDatabase("Death");
            var coleccion = database.GetCollection<Grupos>("Grupos");
            var filtro = Builders<Grupos>.Filter.Eq("Usuario", usuarioCreador) & Builders<Grupos>.Filter.Eq("Grupo", nombreGrupo);
            var update = Builders<Grupos>.Update.Set("Amigos", integrantes);
            var respuesta = coleccion.UpdateOne(filtro, update);

            return respuesta.IsModifiedCountAvailable;
        }

    }
}
