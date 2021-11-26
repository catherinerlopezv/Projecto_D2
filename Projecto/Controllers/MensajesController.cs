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
using Archivos2;
using System.IO;
using Api.Models;

namespace Projecto.Controllers
{
    public class MensajesController : Controller
    {
        ChatAPI _api = new ChatAPI();
        SDES sdes = new SDES();
        GenerarClavesSeguras dh = new GenerarClavesSeguras(); //Diffie-helfman
        private MyGlobalData GlobalData = new MyGlobalData();

        public static List<Grupos> lista2 = new List<Grupos>();
        List<MensajesViewModel> mensajes = new List<MensajesViewModel>();
        public int valor;

        public async Task<IActionResult> Index(string id, int i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            GlobalData.ParaGrupos.Clear();
            mensajes.Clear();
            if (id != null || id != "")
            {
                GlobalData.ParaGrupos.Insert(i, id);
            }
            if (GlobalData.Receptor == null || GlobalData.Receptor.NickName != id)
            {
                HttpClient client1 = _api.Initial();
                HttpResponseMessage respuesta = await client1.GetAsync($"api/Login/{GlobalData.ParaGrupos[i]}");
                if (respuesta.IsSuccessStatusCode)
                {
                    var results = respuesta.Content.ReadAsStringAsync().Result;

                    GlobalData.Receptor = JsonConvert.DeserializeObject<UserData>(results);
                }
            }
            GlobalData.actualizaSesion(HttpContext.Session);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajes = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);

            }
            //zroeth
            mensajes = mensajes.FindAll(x => ((x.Receptor[i] == GlobalData.ParaGrupos[i] && x.Emisor == GlobalData.ActualUser.NickName) || (x.Emisor == GlobalData.ParaGrupos[i] && x.Receptor[i] == GlobalData.ActualUser.NickName)));
            mensajes = mensajes.OrderBy(x => x.Date).ToList();
            int claveCifrado = dh.GenerarLlave(GlobalData.ActualUser.Code, GlobalData.Receptor.Code);

            mensajes.ForEach(x => x.Cuerpo = sdes.MDesencriptar2(claveCifrado, x.Cuerpo));


            var tupleModel = new Tuple<List<MensajesViewModel>, int>(mensajes, i);
            ViewData["NickName"] = GlobalData.ActualUser.NickName;
            return View(tupleModel);
        }
        public async Task<IActionResult> IndexMensajes(string id, int i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
       
            var tupleModel = new Tuple<List<List<MensajesViewModel>>,string>( await IndexMensajeseAsync(id, i),id);

            ViewData["NickName"] = GlobalData.ActualUser.NickName;
            ViewData["ParaGrupos"] = GlobalData.ParaGrupos;

            return View(tupleModel);
        }


        public async Task<List<Grupos>> GetStudentsAsync()
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            lista2.Clear();
            HttpClient client = _api.Initial();
            var res = await client.GetAsync($"api/Grupos/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                List<Grupos> contactosUser = JsonConvert.DeserializeObject<List<Grupos>>(resultas);
                lista2 = contactosUser;
            }
            return (lista2);
        }

        public async Task<List<List<MensajesViewModel>>> IndexMensajeseAsync(string id, int i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            GlobalData.ParaGrupos.Clear();
            mensajes.Clear();
            List<List<MensajesViewModel>> uwu = new List<List<MensajesViewModel>>();


            List<Grupos> grupo = await GetStudentsAsync();
            grupo =grupo.FindAll(x=>x.Grupo==id);
        
            for (int k = 0; k < grupo[i].Amigos.Count; k++)
            {
               
                    if (id != null || id != "")
                    {
                        GlobalData.ParaGrupos.Insert(k, grupo[i].Amigos[k]);
                    }
                    if (GlobalData.Receptor == null || GlobalData.Receptor.NickName != id)
                    {
                        HttpClient client1 = _api.Initial();
                        HttpResponseMessage respuesta = await client1.GetAsync($"api/Login/{GlobalData.ParaGrupos[k]}");
                        if (respuesta.IsSuccessStatusCode)
                        {
                            var results = respuesta.Content.ReadAsStringAsync().Result;

                            GlobalData.Receptor = JsonConvert.DeserializeObject<UserData>(results);
                        }
                    }

                    string ad = GlobalData.ParaGrupos[i];
                    HttpClient client = _api.Initial();
                    HttpResponseMessage res = await client.GetAsync($"api/MensajesGrupo/{GlobalData.ActualUser.NickName}");
                    if (res.IsSuccessStatusCode && grupo[i].Grupo==id)
                    {
                        var results = res.Content.ReadAsStringAsync().Result;
                        mensajes = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);

                    }

                //zroeth
                    mensajes = mensajes.FindAll(x => x.Grupo == id);
                    mensajes = mensajes.FindAll(x => (x.Receptor[i] == GlobalData.ParaGrupos[k]) || 
                    (x.Emisor == GlobalData.ParaGrupos[k] && x.Receptor[i] == GlobalData.ActualUser.NickName));
                    mensajes = mensajes.OrderBy(x => x.Date).ToList();
                    int claveCifrado = dh.GenerarLlave(GlobalData.ActualUser.Code, 0);
                    mensajes.ForEach(x => x.Cuerpo = sdes.MDesencriptar2(claveCifrado, x.Cuerpo));
                    uwu.Add(mensajes);
                }


            
            GlobalData.actualizaSesion(HttpContext.Session);



            return uwu;

        }


        [HttpPost]
        public async Task<IActionResult> Buscar(string mensaje,int i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
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

            List<MensajesViewModel> mensajes = new List<MensajesViewModel>();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajes = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);

            }



            List<MensajesViewModel> mensajesG = new List<MensajesViewModel>();
            List<MensajesViewModelG> mensajesG2 = new List<MensajesViewModelG>();
            HttpResponseMessage resG = await client.GetAsync($"api/MensajesGrupo/{GlobalData.ActualUser.NickName}");
            if (resG.IsSuccessStatusCode)
            {
                var results = resG.Content.ReadAsStringAsync().Result;
                mensajesG = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);
                mensajesG2 = JsonConvert.DeserializeObject<List<MensajesViewModelG>>(results);

            }
            //zroeth
            List<MensajesViewModel> MisMensajes = mensajes.FindAll(x => x.Emisor == GlobalData.ActualUser.NickName);
            List<MensajesViewModel> MensajesRecibidos = mensajes.FindAll(x => x.Receptor[i] == GlobalData.ActualUser.NickName);

            List<MensajesViewModel> MisMensajesG = mensajesG.FindAll(x => x.Emisor == GlobalData.ActualUser.NickName);
            List<MensajesViewModelG> MensajesRecibidosG = mensajesG2.FindAll(x => x.Receptor[i] == GlobalData.ActualUser.NickName);



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
                int llave = dh.GenerarLlave(GlobalData.ActualUser.Code, llaves[item.Receptor[i]]);
                ms.Cuerpo = sdes.MDesencriptar2(llave, item.Cuerpo);
                mios.Add(ms);

            }
            foreach (var item in MisMensajesG)
            {
                MensajesViewModel ms = new MensajesViewModel();
                ms = item;
                int llave = dh.GenerarLlave(GlobalData.ActualUser.Code, 0);
                ms.Cuerpo = sdes.MDesencriptar2(llave, item.Cuerpo);
                mios.Add(ms);

            }
            var miosC = mios;
            mios = mios.FindAll(x => x.Cuerpo.Contains(mensaje));
            List<MensajesViewModel> recib = new List<MensajesViewModel>();
            foreach (var item in MensajesRecibidos)
            {
                MensajesViewModel ms = new MensajesViewModel();
                ms = item;
                int llave = dh.GenerarLlave(llaves[item.Emisor], GlobalData.ActualUser.Code);
                ms.Cuerpo = sdes.MDesencriptar2(llave, item.Cuerpo);
                recib.Add(ms);
            }
            List<MensajesViewModelG> recib2 = new List<MensajesViewModelG>();
            foreach (var item in MensajesRecibidosG)
            {
                MensajesViewModelG ms = new MensajesViewModelG();
                ms = item;
                int llave = dh.GenerarLlave(GlobalData.ActualUser.Code, 0);
                ms.Cuerpo = sdes.MDesencriptar2(llave, item.Cuerpo);
                recib2.Add(ms);
            }


           
            recib = recib.FindAll(x => x.Cuerpo.Contains(mensaje));
            recib2 = recib2.FindAll(x => x.Cuerpo.Contains(mensaje));
            recib2 = recib2.FindAll(x => !x.Emisor.Contains(GlobalData.ActualUser.NickName));
            for (int j = 0; j < recib2.Count; j++)
            {
                for (int k = 0; k < recib2[j].Receptor.Count; k++)
                {
                    if(k==0)
                    {
                        recib2[j].Receptor[k] = recib2[recib2.FindIndex(x => x.Receptor.Equals(x.Receptor))].Grupo;
                    }
                    else
                    {
                        recib2[j].Receptor[k] = "";
                    }

                }
            }

            for (int j = 0; j < mios.Count; j++)
            {
                for (int k = 0; k < mios[j].Receptor.Count; k++)
                {
                    if (mios[j].Receptor.Count != 1)
                    {
                        if (mios[j].Receptor[k] == GlobalData.ActualUser.NickName)
                        {
                            if (k == 0)
                            {
                                mios[j].Receptor[k] = mios[j].Grupo;
                            }
                            else
                            {
                                mios[j].Receptor[k] = "";
                            }
                        }
                        else
                        {
                            if (k == 0)
                            {
                                mios[j].Receptor[k] = mios[j].Grupo;
                            }
                            else if (k != 0)
                            {
                                mios[j].Receptor[k] = "";
                            }
                        }

                    }

                }
            }


            var final = mios.Union(recib);
            List<MensajesViewModel> Lfinal = final.OrderBy(x => x.Date).ToList();
            List<MensajesViewModelG> Lfinal2 = recib2.OrderBy(x => x.Date).ToList();
            Lfinal.RemoveAll(x => x.Archivo == true);
            Lfinal2.RemoveAll(x => x.Archivo == true);
           // Lfinal2.Clear();
            //Lfinal.Clear();
            var tupleModel = new Tuple<List<MensajesViewModel>, List<MensajesViewModelG>> (Lfinal, Lfinal2);
            return View(tupleModel);

        }

        [HttpPost]
        public IActionResult NuevoMensaje(string texto,int i)
        {
          GlobalData.obtieneSesion(HttpContext.Session);
            if (texto == null || texto == "")
            {
                return Content("Ingrese un mensaje");
            }
            MensajesViewModel mensajesNuevo = new MensajesViewModel();

            int claveCifrado = dh.GenerarLlave(GlobalData.ActualUser.Code, GlobalData.Receptor.Code);
            texto = sdes.encriptarP2(claveCifrado, texto);
            mensajesNuevo.Cuerpo = texto;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = false;
            mensajesNuevo.Emisor = GlobalData.ActualUser.NickName;
            mensajesNuevo.Receptor = GlobalData.ParaGrupos;
            mensajesNuevo.Visible = "";
            HttpClient client = _api.Initial();
            var postTask = client.PostAsJsonAsync<MensajesViewModel>("api/Mensajes", mensajesNuevo);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("http://localhost:10999/Mensajes/Index/" + GlobalData.ParaGrupos[i]);
            }
            return RedirectToAction("Index", "Mensajes");
        }


        [HttpPost]
        public IActionResult NuevoMensajeGrupo(string texto, string i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            if (texto == null || texto == "")
            {
                return Content("Ingrese un mensaje");
            }
            MensajesViewModel mensajesNuevo = new MensajesViewModel();
        
            int claveCifrado = dh.GenerarLlave(GlobalData.ActualUser.Code,0);
            texto = sdes.encriptarP2(claveCifrado, texto);
            mensajesNuevo.Cuerpo = texto;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = false;
            mensajesNuevo.Emisor = GlobalData.ActualUser.NickName;
            mensajesNuevo.Receptor = GlobalData.ParaGrupos;
            mensajesNuevo.Grupo = i;
            mensajesNuevo.Visible = "";
            HttpClient client = _api.Initial();
            var postTask = client.PostAsJsonAsync<MensajesViewModel>("api/MensajesGrupo", mensajesNuevo);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("http://localhost:10999/Mensajes/IndexMensajes/" + i);
            }
            return RedirectToAction("IndexMensajes", "Mensajes");
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


        public async Task<IActionResult> BorrarGLobal(string id, int i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            var mensaje = new MensajesViewModel();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync($"api/Mensajes/{id}");

            return Redirect("http://localhost:10999/Mensajes/Index/" + GlobalData.ParaGrupos[i]);
        }
        public async Task<IActionResult> BorrarGLobalG(string id, string i)
        {
            var mensaje = new MensajesViewModel();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync($"api/MensajesGrupo/{id}");

            return Redirect("http://localhost:10999/Mensajes/IndexMensajes/" + i);
        }
        public async Task<IActionResult> Borrar(string id,int i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{GlobalData.ActualUser.NickName}");
            List<MensajesViewModel> mensajesViews = new List<MensajesViewModel>();
            MensajesViewModel mensajeAborrar = new MensajesViewModel();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajesViews = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results); //Obtener de los datos del usuario ingresado
                mensajeAborrar = mensajesViews.Find(x => x.Id == id);
                mensajeAborrar.Visible = GlobalData.ActualUser.NickName;
                var postTask = client.PutAsJsonAsync<MensajesViewModel>($"api/Mensajes/{id}", mensajeAborrar);
                postTask.Wait();
                if (postTask.Result.IsSuccessStatusCode)
                {
                    return Redirect("http://localhost:10999/Mensajes/Index/" + GlobalData.ParaGrupos[i]);
                }
            }

            return Redirect("http://localhost:10999/Mensajes/Index/" + GlobalData.ParaGrupos[i]);
        }

        public async Task<IActionResult> BorrarG(string id, string i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/MensajesGrupo/{GlobalData.ActualUser.NickName}");
            List<MensajesViewModel> mensajesViews = new List<MensajesViewModel>();
            MensajesViewModel mensajeAborrar = new MensajesViewModel();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajesViews = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results); //Obtener de los datos del usuario ingresado
                mensajeAborrar = mensajesViews.Find(x => x.Id == id);
                mensajeAborrar.Visible = GlobalData.ActualUser.NickName;
                var postTask = client.PutAsJsonAsync<MensajesViewModel>($"api/MensajesGrupo/{id}", mensajeAborrar);
                postTask.Wait();
                if (postTask.Result.IsSuccessStatusCode)
                {
                    return Redirect("http://localhost:10999/Mensajes/IndexMensajes/" + i);
                }
            }

            return Redirect("http://localhost:10999/Mensajes/IndexMensajes/" + i);
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
        //envio de archivo chat simple
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile Archivo,int i)
        {
            GlobalData.obtieneSesion(HttpContext.Session);
            if (Archivo == null || Archivo.Length == 0)
            {
                return Content("Seleccione un archivo");
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Archivo.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Archivo.CopyToAsync(stream);
            }
            GlobalData.ArchivoEntrada = path;
            LZWArchivos Compresor = new LZWArchivos();
            string RutaSalida = "";
            Compresor.ComprimirF(path, ref RutaSalida);
            GlobalData.ArchivoSalida = RutaSalida;
            
            GlobalData.actualizaSesion(HttpContext.Session);
            
            FileInfo fileInfo = new FileInfo(path);
            Queue<byte> Texto = new Queue<byte>();
            LeerArchivo(ref Texto, GlobalData.ArchivoSalida);
            string salidaCompreso = "";
            while (Texto.Count > 0)
            {
                salidaCompreso += Convert.ToString(Texto.Dequeue()) + ",";
            }

            MensajesViewModel mensajesNuevo = new MensajesViewModel();
            mensajesNuevo.Cuerpo = salidaCompreso;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = true;
            mensajesNuevo.NombreArchivo = fileInfo.Name;
            mensajesNuevo.Emisor = GlobalData.ActualUser.NickName;
            mensajesNuevo.Receptor = GlobalData.ParaGrupos;
            mensajesNuevo.Visible = "";
            HttpClient client = _api.Initial();
            var postTask = client.PostAsJsonAsync<MensajesViewModel>("api/Mensajes", mensajesNuevo);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("http://localhost:10999/Mensajes/Index/" + GlobalData.ParaGrupos[i]);
            }
            return RedirectToAction("Index", "Mensajes");

        }
        //envio de archivo chat grupal
        [HttpPost]
        public async Task<IActionResult> UploadFileG(IFormFile Archivo, string i)
        {
             GlobalData.obtieneSesion(HttpContext.Session);
            if (Archivo == null || Archivo.Length == 0)
            {
                return Content("Seleccione un archivo");
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Archivo.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Archivo.CopyToAsync(stream);
            }
            GlobalData.ArchivoEntrada = path;
            LZWArchivos Compresor = new LZWArchivos();
            string RutaSalida = "";
            Compresor.ComprimirF(path, ref RutaSalida);
            GlobalData.ArchivoSalida = RutaSalida;

            GlobalData.actualizaSesion(HttpContext.Session);

            FileInfo fileInfo = new FileInfo(path);
            Queue<byte> Texto = new Queue<byte>();
            LeerArchivo(ref Texto, GlobalData.ArchivoSalida);
            string salidaCompreso = "";
            while (Texto.Count > 0)
            {
                salidaCompreso += Convert.ToString(Texto.Dequeue()) + ",";
            }

            MensajesViewModel mensajesNuevo = new MensajesViewModel();
            mensajesNuevo.Cuerpo = salidaCompreso;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = true;
            mensajesNuevo.NombreArchivo = fileInfo.Name;
            mensajesNuevo.Emisor = GlobalData.ActualUser.NickName;
            mensajesNuevo.Grupo = i;
            mensajesNuevo.Receptor = GlobalData.ParaGrupos;
            mensajesNuevo.Visible = "";
            HttpClient client = _api.Initial();
            var postTask = client.PostAsJsonAsync<MensajesViewModel>("api/MensajesGrupo", mensajesNuevo);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return Redirect("http://localhost:10999/Mensajes/IndexMensajes/" + i);
            }
            return RedirectToAction("IndexMensajes", "Mensajes");

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
        //descarga archivo individual
        public async Task<IActionResult> Descargar_archivo(string id, int i)
        {
             GlobalData.obtieneSesion(HttpContext.Session);
            HttpClient client = _api.Initial();
            _ = new MensajesViewModel();
            HttpResponseMessage res = await client.GetAsync($"api/Archivos/{id}");
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
                GlobalData.ArchivoSalida = archivoNormalRuta;
                GlobalData.actualizaSesion(HttpContext.Session);
                return RedirectToAction("Download");
            }

            return Redirect("http://localhost:10999/Mensajes/Index/" + GlobalData.ParaGrupos[i]);

        }
        //descarga de archivo grupal
        public async Task<IActionResult> Descargar_archivoG(string id, int i)
        {
           GlobalData.obtieneSesion(HttpContext.Session);
            HttpClient client = _api.Initial();
            _ = new MensajesViewModel();
            HttpResponseMessage res = await client.GetAsync($"api/ArchivosGrupo/{id}");
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
                GlobalData.ArchivoSalida = archivoNormalRuta;
                GlobalData.actualizaSesion(HttpContext.Session);
                return RedirectToAction("Download");
            }

            return RedirectToAction("IndexMensajes", GlobalData.ParaGrupos[i], i);

        }

        public async Task<IActionResult> Download() 
        {
           GlobalData.obtieneSesion(HttpContext.Session);
            var path = GlobalData.ArchivoSalida;

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