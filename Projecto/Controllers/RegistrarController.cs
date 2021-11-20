using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Archivos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projecto.Helper;
using Projecto.Models;

namespace Projecto.Controllers
{
    public class RegistrarController : Controller
    {
        ChatAPI _api = new ChatAPI();
        // GET: Registrar
        public ActionResult Index()
        {
            return View();
        }

        // GET: Registrar/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }



        // GET: Registrar/Create
        [HttpPost]
        public IActionResult Create(string nombre, string nickName, string password)
        {
            if (nombre == null || nombre == "" || nickName == null || nickName == "" || password == null || password == "")
            {
                return Content("Ingrese todos los valores");
            }
            UserData newUser = new UserData();
            newUser.Name = nombre;
            newUser.NickName = nickName;
            Ceasar cesar = new Ceasar();
            newUser.Password = cesar.Cifrar(password, "1234567890'¿qwertyuiop");
            HttpClient client = _api.Initial();

            var postTask = client.PostAsJsonAsync<UserData>("api/SignIn", newUser);
            postTask.Wait();

            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Login");
            }

            return Content("Error en la creación de un nuevo usuario");
        }


        public ActionResult Edit(int id)
        {
            return View();
        }

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

        // GET: Registrar/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Registrar/Delete/5
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
    }
}