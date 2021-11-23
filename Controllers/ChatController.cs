using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projecto.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index() {
            string jsondata = HttpContext.Session.GetString("globaldata");
            GlobalData globaldata = JsonConvert.DeserializeObject<GlobalData>(jsondata);
            ViewData["nickname"] = globaldata.ActualUser.NickName;
            return View();
        }
    }
}
