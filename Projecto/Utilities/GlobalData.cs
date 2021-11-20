using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projecto.Models;

namespace Projecto
{
    public static class GlobalData
    {
        public static UserData ActualUser { get; set; }
        public static UserData Receptor { get; set; }
        public static string para { get; set; }
        public static string ArchivoEntrada { get; set; }
        public static string ArchivoSalida { get; set; }
    }
}
