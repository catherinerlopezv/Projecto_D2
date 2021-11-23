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
using Projecto.Utilities;
using Archivos;
using System.IO;
namespace Projecto.Controllers
{
    public class MensajesController : Controller
    {
        ChatAPI _api = new ChatAPI();
        SDES sdes = new SDES();
        GenerarClavesSeguras dh = new GenerarClavesSeguras(); //Diffie-helfman
        public async Task<IActionResult> Index(string id)
        {
            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);
            

            if (id != null || id != "")
            {
                globaldata.para = id;
            }
            if (globaldata.Receptor == null) // || globaldata.Receptor != id)
            {
                HttpClient client1 = _api.Initial();
                HttpResponseMessage respuesta = await client1.GetAsync($"api/Login/{globaldata.para}");
                if (respuesta.IsSuccessStatusCode)
                {
                    var results = respuesta.Content.ReadAsStringAsync().Result;

                    globaldata.Receptor = JsonConvert.DeserializeObject<UserData>(results); 
                }
            }

            jsondata = JsonConvert.SerializeObject(globaldata);
            HttpContext.Session.SetString("globaldata", jsondata);
            ViewData["nickname"] = globaldata.ActualUser.NickName;
            List <MensajesViewModel> mensajes = new List<MensajesViewModel>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{globaldata.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajes = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);

            }
            mensajes = mensajes.FindAll(x => ((x.Receptor == globaldata.para && x.Emisor == globaldata.ActualUser.NickName) || (x.Emisor == globaldata.para && x.Receptor == globaldata.ActualUser.NickName)));
            mensajes = mensajes.OrderBy(x => x.Date).ToList();
            int claveCifrado = dh.GenerarLlave(globaldata.ActualUser.Code, globaldata.Receptor.Code);
            mensajes.ForEach(x => x.Cuerpo = sdes.MDesencriptar2(claveCifrado, x.Cuerpo));


            return View(mensajes);
        }


        [HttpPost]
        public async Task<IActionResult> Buscar(string mensaje)
        {
            if (mensaje == null || mensaje == "")
            {
                return Content("No se puede realizar la búsqueda de un mensaje vacío");
            }
            List<UserData> keys = new List<UserData>();
            HttpClient client = _api.Initial();
            HttpResponseMessage usuarios = await client.GetAsync($"api/Login/");
            if (usuarios.IsSuccessStatusCode)
            {
                var results = usuarios.Content.ReadAsStringAsync().Result;
                keys = JsonConvert.DeserializeObject<List<UserData>>(results); 
            }

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);
            
            List<MensajesViewModel> mensajes = new List<MensajesViewModel>();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{globaldata.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajes = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);

            }
            List<MensajesViewModel> MisMensajes = mensajes.FindAll(x => x.Emisor == globaldata.ActualUser.NickName);
            List<MensajesViewModel> MensajesRecibidos = mensajes.FindAll(x => x.Receptor == globaldata.ActualUser.NickName);
            Dictionary<string, int> llaves = new Dictionary<string, int>();
            foreach (var item in keys)
            {
                llaves.Add(item.NickName, item.Code);
            }
            List<MensajesViewModel> mios = new List<MensajesViewModel>();
            foreach (var item in MisMensajes)
            {
                MensajesViewModel ms = new MensajesViewModel();
                ms = item;
                int llave = dh.GenerarLlave(globaldata.ActualUser.Code, llaves[item.Receptor]);
                ms.Cuerpo = sdes.MDesencriptar2(llave, item.Cuerpo);
                mios.Add(ms);

            }
            List<MensajesViewModel> recib = new List<MensajesViewModel>();
            foreach (var item in MensajesRecibidos)
            {
                MensajesViewModel ms = new MensajesViewModel();
                ms = item;
                int llave = dh.GenerarLlave(llaves[item.Emisor], globaldata.ActualUser.Code);
                ms.Cuerpo = sdes.MDesencriptar2(llave, item.Cuerpo);
                recib.Add(ms);
            }

            mios = mios.FindAll(x => x.Cuerpo.Contains(mensaje));
            recib = recib.FindAll(x => x.Cuerpo.Contains(mensaje));
            var final = mios.Union(recib);
            List<MensajesViewModel> Lfinal = final.OrderBy(x => x.Date).ToList();
            Lfinal.RemoveAll(x => x.Archivo == true);
            return View(Lfinal);

        }

        [HttpPost]
        public IActionResult NuevoMensaje(string texto)
        {
            if (texto == null || texto == "")
            {
                return Content("Ingrese un mensaje");
            }
            MensajesViewModel mensajesNuevo = new MensajesViewModel();

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

            int claveCifrado = dh.GenerarLlave(globaldata.ActualUser.Code, globaldata.ActualUser.Code);
            texto = sdes.encriptarP2(claveCifrado, texto);
            mensajesNuevo.Cuerpo = texto;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = false;
            mensajesNuevo.Emisor = globaldata.ActualUser.NickName;
            mensajesNuevo.Receptor = globaldata.para;
            mensajesNuevo.Visible = "";
            HttpClient client = _api.Initial();
            var postTask = client.PostAsJsonAsync<MensajesViewModel>("api/Mensajes", mensajesNuevo);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("http://localhost:10999/Mensajes/Index/" + globaldata.para);
            }
            return RedirectToAction("Index", "Mensajes");
        }
        // GET: Mensajes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Mensajes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mensajes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Mensajes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Mensajes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Mensajes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }


        public async Task<IActionResult> BorrarGLobal(string id)
        {
            var mensaje = new MensajesViewModel();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync($"api/Mensajes/{id}");

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

            return Redirect("http://localhost:10999/Mensajes/Index/" + globaldata.para);
        }
        public async Task<IActionResult> Borrar(string id)
        {
            HttpClient client = _api.Initial();

            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{globaldata.ActualUser.NickName}");
            List<MensajesViewModel> mensajesViews = new List<MensajesViewModel>();
            MensajesViewModel mensajeAborrar = new MensajesViewModel();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajesViews = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results); //Obtener de los datos del usuario ingresado
                mensajeAborrar = mensajesViews.Find(x => x.Id == id);
                mensajeAborrar.Visible = globaldata.ActualUser.NickName;
                var postTask = client.PutAsJsonAsync<MensajesViewModel>($"api/Mensajes/{id}", mensajeAborrar);
                postTask.Wait();
                if (postTask.Result.IsSuccessStatusCode)
                {
                    return Redirect("http://localhost:10999/Mensajes/Index/" + globaldata.para);
                }
            }

            return Redirect("http://localhost:10999/Mensajes/Index/" + globaldata.para);
        }

        // POST: Mensajes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile Archivo)
        {
            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);

            if (Archivo == null || Archivo.Length == 0)
            {
                return Content("Seleccione un archivo");
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Archivo.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Archivo.CopyToAsync(stream);
            }
            globaldata.ArchivoEntrada = path;
            LZWArchivos Compresor = new LZWArchivos();
            string RutaSalida = "";
            Compresor.ComprimirF(path, ref RutaSalida);
            globaldata.ArchivoSalida = RutaSalida;
            FileInfo fileInfo = new FileInfo(path);
            Queue<byte> Texto = new Queue<byte>();
            LeerArchivo(ref Texto, RutaSalida);
            string salidaCompreso = "";
            while (Texto.Count > 0)
            {
                salidaCompreso += Convert.ToString(Texto.Dequeue()) + ",";
            }

            jsondata = JsonConvert.SerializeObject(globaldata);
            HttpContext.Session.SetString("globaldata", jsondata);

            MensajesViewModel mensajesNuevo = new MensajesViewModel();
            mensajesNuevo.Cuerpo = salidaCompreso;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = true;
            mensajesNuevo.NombreArchivo = fileInfo.Name;
            mensajesNuevo.Emisor = globaldata.ActualUser.NickName;
            mensajesNuevo.Receptor = globaldata.para;
            mensajesNuevo.Visible = "";
            HttpClient client = _api.Initial();
            var postTask = client.PostAsJsonAsync<MensajesViewModel>("api/Mensajes", mensajesNuevo);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("http://localhost:10999/Mensajes/Index/" + globaldata.para);
            }
            return RedirectToAction("Index", "Mensajes");

        }

        void LeerArchivo(ref Queue<byte> TextoAleer, string rutaOrigen)
        {
            const int bufferLength = 1024;
            var buffer = new byte[bufferLength];
            using (var file = new FileStream(rutaOrigen, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            TextoAleer.Enqueue(item);
                        }
                    }

                }

            }


        }

        public async Task<IActionResult> Descargar_archivo(string id)
        {
            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);
            HttpClient client = _api.Initial();
            _ = new MensajesViewModel();
            HttpResponseMessage res = await client.GetAsync($"api/Files/{id}");
            Queue<byte> textoEntrada = new Queue<byte>();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                MensajesViewModel archivo = JsonConvert.DeserializeObject<MensajesViewModel>(results);
                HelperArchivos helperArchivos = new HelperArchivos();
                textoEntrada = helperArchivos.LeerCifrado(archivo.Cuerpo);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", archivo.NombreArchivo);
                path = path.Replace(".txt", ".lzw");
                helperArchivos.EscribirArchivo(textoEntrada, path);
                LZWArchivos lzw = new LZWArchivos();
                string archivoNormalRuta = "";
                lzw.MDescomprimir(path, ref archivoNormalRuta);
                globaldata.ArchivoSalida = archivoNormalRuta;
                return RedirectToAction("Download");
            }
            jsondata = JsonConvert.SerializeObject(globaldata);
            HttpContext.Session.SetString("globaldata", jsondata);
            return Redirect("http://localhost:10999/Mensajes/Index/" + globaldata.para);

        }

        public async Task<IActionResult> Download() 
        {
            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);
            string path = globaldata.ArchivoSalida;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".cif", "text/plain"},
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }
}