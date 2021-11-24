using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projecto.Models;

namespace Projecto
{
    public class GlobalData
    {
        public UserData ActualUser { get; set; }
        public UserData Receptor { get; set; }
        public string para { get; set; }
        public string ArchivoEntrada { get; set; }
        public string ArchivoSalida { get; set; }
    }
}
