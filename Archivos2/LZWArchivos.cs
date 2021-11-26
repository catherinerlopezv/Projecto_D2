using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Archivos2
{
    public class LZWSimple
    {
        /// Comprime el texto y lo convierte en un listado de simbolos
        /// 
        public static Queue<string> Cadenas = new Queue<string>();
        public static List<int> Comprimir(string texto)
        {

            // Contruye el diccionario guardando los caractes unicos al principio del archivo y el contenido de lzw despues
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            //Limite de caracteres ascii

            for (int i = 0; i < 256; i++)
            {
                diccionario.Add(((char)i).ToString(), i);
            }
            string temp = string.Empty;
            List<int> comprimir = new List<int>();
            //Cada caracter en el texto del archivo
            foreach (char letras in texto)
            {
                //Caracteres unicos
                string guardarL = temp + letras;
                if (diccionario.ContainsKey(guardarL))
                {
                    temp = guardarL;
                }
                else
                {
                    //Guardar comprimido
                    comprimir.Add(diccionario[temp]);
                    //Si es diferente, se guarda en el diccionario
                    diccionario.Add(guardarL, diccionario.Count);
                    temp = letras.ToString();
                }
            }

            //Archivos vacios
            if (!string.IsNullOrEmpty(temp))
            {
                comprimir.Add(diccionario[temp]);
            }
            return comprimir;

        }

        public static string Descomprimir(List<int> texto)
        {
            int tamañodeDiccionario = 256;
            Dictionary<int, string> diccionario = new Dictionary<int, string>();
            /*
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            //Limite de caracteres ascii

            for (int i = 0; i < 256; i++)
            {
                diccionario.Add(((char)i).ToString(), i);
            }
            for (int i = 0; i < 256; i++)
            {
                diccionario[i] = "" + (char)i;
            }*/

            string temp = "" + (char)(int)texto.ElementAt(0);
            texto.RemoveAt(0);
            StringBuilder resultado = new StringBuilder(temp);
            foreach (int letras in texto)
            {
                string entrada;
                if (diccionario.ContainsKey(letras))
                {
                    entrada = diccionario[letras];
                }
                else if (letras == tamañodeDiccionario)
                {
                    entrada = temp + temp[0];
                }
                else
                {
                    throw new System.ArgumentException("Caracter desconocido: " + letras);
                }

                resultado.Append(entrada);

                // Agregar al diccionario
                diccionario[tamañodeDiccionario++] = temp + entrada[0];

                temp = entrada;
            }
            return resultado.ToString();
        }


    }
    public class LZWArchivos
    {
        public static Queue<string> Cadenas = new Queue<string>();
        public void ComprimirF(string path, ref string raiz)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            lecturaC(path);
            /*
   Dictionary<string, int> diccionario = new Dictionary<string, int>();
   //Limite de caracteres ascii

   for (int i = 0; i < 256; i++)
   {
       diccionario.Add(((char)i).ToString(), i);
   }
   */
            completarDic(dic, Cadenas);
            FileInfo Informacion = new FileInfo(path);
            string RutaSalida = Informacion.Name.Replace(".txt", ".lzw");
            var raiz2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", RutaSalida);

            compresiones(dic, raiz2);
            raiz = raiz2;

        }

        public static void lecturaC(string raiz)
        {
            const int lmite = 1000;
            byte[] buff = new byte[lmite];
            using (var file = new FileStream(raiz, FileMode.Open))
            {
                using (var leer = new BinaryReader(file))
                {
                    while (leer.BaseStream.Position != leer.BaseStream.Length)
                    {
                        buff = leer.ReadBytes(lmite);
                        string[] caracters = new string[buff.Length];
                        char completar = '0';
                        foreach (var bytes in buff)
                        {
                            int n = Convert.ToInt32(bytes);
                            string binarios = Convert.ToString(n, 2);
                            string codigos = binarios.PadLeft(8, completar);
                            Cadenas.Enqueue(codigos);

                        }
                    }
                }
            }
        }
        public static void completarDic(Dictionary<string, int> dic, Queue<string> texto)
        {
            foreach (var linea in texto)
            {
                if (dic.Count == 0)
                {
                    dic.Add(linea, dic.Count + 1);
                }
                else
                {
                    if (dic.ContainsKey(linea) == false)
                    {
                        dic.Add(linea, dic.Count + 1);
                    }
                }
            }
        }
        public static void compresiones(Dictionary<string, int> dic, string enviar)
        {
            tempDiccionario(dic, enviar);
            iterar(Cadenas, "", dic, enviar);
        }
        public static void tempDiccionario(Dictionary<string, int> dic, string path)
        {
            StreamWriter Escribir = new StreamWriter(path);
            foreach (var Elemento in dic)
            {
                char temp = Convert.ToChar(Elemento.Value);
                Escribir.Write(Elemento.Key + temp);
            }
            Escribir.Write("-_ ");
            Escribir.Close();
        }
        public static void iterar(Queue<string> texto, string c1, Dictionary<string, int> dic, string salida)
        {
            string siguiente;
            string actual;
            byte[] escribe = new byte[3];
            int div, resultado;

            while (texto.Count > 0)
            {
                siguiente = texto.Dequeue();
                actual = c1 + siguiente;
                if (dic.ContainsKey(actual))
                {
                    c1 = actual;
                }
                else
                {
                    dic.Add(actual, dic.Count + 1);
                    if (dic[c1] < 256)
                    {
                        escribe[0] = Convert.ToByte(0);
                        escribe[1] = Convert.ToByte(0);
                        escribe[2] = Convert.ToByte(dic[c1]);
                    }
                    if (dic[c1] > 255 && dic[c1] < 65536)
                    {
                        escribe[0] = Convert.ToByte(0);
                        div = dic[c1] / 256;
                        escribe[1] = Convert.ToByte(div);
                        resultado = dic[c1] % 256;
                        escribe[2] = Convert.ToByte(resultado);
                    }
                    if (dic[c1] > 65535)
                    {
                        div = dic[c1] / 65536;
                        escribe[0] = Convert.ToByte(div);
                        div = dic[c1] % 65536;
                        escribe[1] = Convert.ToByte(div / 256);
                        resultado = div % 256;
                        escribe[2] = Convert.ToByte(resultado);
                    }
                    ABytes(salida, escribe);
                    c1 = siguiente;
                }
            }
            {
                if (dic[c1] < 256)
                {
                    escribe[0] = Convert.ToByte(0);
                    escribe[1] = Convert.ToByte(0);
                    escribe[2] = Convert.ToByte(dic[c1]);
                }
                if (dic[c1] > 255 && dic[c1] < 65536)
                {
                    escribe[0] = Convert.ToByte(0);
                    div = dic[c1] / 256;
                    escribe[1] = Convert.ToByte(div);
                    resultado = dic[c1] % 256;
                    escribe[2] = Convert.ToByte(resultado);
                }
                if (dic[c1] > 65535)
                {
                    div = dic[c1] / 65536;
                    escribe[0] = Convert.ToByte(div);
                    div = dic[c1] % 65536;
                    escribe[1] = Convert.ToByte(div / 256);
                    resultado = div % 256;
                    escribe[2] = Convert.ToByte(resultado);
                }
                ABytes(salida, escribe);
            }
        }
        public static void ABytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        public void MDescomprimir(string path, ref string temp)
        {
            List<byte> list = new List<byte>();
            List<byte> listaDiccionario = new List<byte>();
            Queue<int> descomprimir = new Queue<int>();
            Dictionary<int, string> dic = new Dictionary<int, string>();
            descomprimirLeer(path, list);
            diccionarioAnterior(list, listaDiccionario);
            iniciarDic(listaDiccionario, dic);
            limpiar(descomprimir, list);
            FileInfo resultadoInf = new FileInfo(path);
            string raiz1 = resultadoInf.Name.Replace(".lzw", ".txt");
            var raiz2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", raiz1);
            iterarDescomprencion(descomprimir, dic, raiz1);
            temp = raiz2;
        }
        public static void descomprimirLeer(string path, List<byte> list)
        {
            const int limite = 1000;
            var Buffer = new byte[limite];
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var leer = new BinaryReader(file))
                {
                    while (leer.BaseStream.Position != leer.BaseStream.Length)
                    {
                        Buffer = leer.ReadBytes(limite);
                        foreach (var Elemento in Buffer)
                        {
                            list.Add(Elemento);
                        }
                    }
                }
            }
        }
        public static void diccionarioAnterior(List<byte> list, List<byte> dic)
        {
            int recorrer = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == 45 && list[i + 1] == 95 && list[i + 2] == 32)
                {
                    recorrer = i;
                    break;
                }
            }
            for (int i = 0; i < recorrer; i++)
            {
                dic.Add(list[i]);
            }
            list.RemoveRange(0, dic.Count + 3);
        }
        public static void iniciarDic(List<byte> list, Dictionary<int, string> dic)
        {
            string Valor1 = "", Valor2 = "", Valor3 = "", Valor4 = "", Valor5 = "", Valor6 = "", Valor7 = "", Valor8 = "", Valor = "";

            int k1;
            byte[] diccionario = list.ToArray();
            string valor = Encoding.UTF8.GetString(diccionario);
            char[] caracter = valor.ToCharArray();
            for (int i = 0; i < caracter.Length; i += 9)
            {
                Valor1 = Convert.ToString(caracter[i]);
                Valor2 = Convert.ToString(caracter[i + 1]);
                Valor3 = Convert.ToString(caracter[i + 2]);
                Valor4 = Convert.ToString(caracter[i + 3]);
                Valor5 = Convert.ToString(caracter[i + 4]);
                Valor6 = Convert.ToString(caracter[i + 5]);
                Valor7 = Convert.ToString(caracter[i + 6]);
                Valor8 = Convert.ToString(caracter[i + 7]);
                Valor = Valor1 + Valor2 + Valor3 + Valor4 + Valor5 + Valor6 + Valor7 + Valor8;
                k1 = Convert.ToInt32(caracter[i + 8]);
                dic.Add(k1, Valor);
            }
            list.Clear();
        }
        public static void limpiar(Queue<int> comprimir, List<byte> list)
        {
            int cola;
            for (int i = 0; i < list.Count; i += 3)
            {
                cola = (list[i] * 65536) + (list[i + 1] * 256) + (list[i + 2]);
                comprimir.Enqueue(cola);
            }
            list.Clear();
        }
        public static void iterarDescomprencion(Queue<int> comprimir, Dictionary<int, string> dic, string path)
        {
            List<byte> salida = new List<byte>();
            int c1, c2;
            string texto, caracter;
            int n;
            byte nbinario;
            c1 = comprimir.Dequeue();
            caracter = dic[c1];
            n = Convert.ToInt32(caracter, 2);
            nbinario = Convert.ToByte(n);
            salida.Add(nbinario);
            while (comprimir.Count > 0)
            {
                c2 = comprimir.Dequeue();
                if (dic.ContainsKey(c2) == false)
                {
                    texto = dic[c1];
                    texto += caracter;
                }
                else
                {
                    texto = dic[c2];
                }
                if (texto.Length == 8)
                {
                    n = Convert.ToInt32(texto, 2);
                    nbinario = Convert.ToByte(n);
                    salida.Add(nbinario);
                }
                else
                {
                    char[] sumar = texto.ToCharArray();
                    for (int i = 0; i < sumar.Length; i += 8)
                    {
                        string b1 = sumar[i].ToString();
                        string b2 = sumar[i + 1].ToString();
                        string b3 = sumar[i + 2].ToString();
                        string b4 = sumar[i + 3].ToString();
                        string b5 = sumar[i + 4].ToString();
                        string b6 = sumar[i + 5].ToString();
                        string b7 = sumar[i + 6].ToString();
                        string b8 = sumar[i + 7].ToString();
                        string bt = b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8;
                        n = Convert.ToInt32(bt, 2);
                        nbinario = Convert.ToByte(n);
                        salida.Add(nbinario);
                    }
                }
                caracter = texto.Substring(0, 8);
                dic.Add(dic.Count + 1, dic[c1] + caracter);
                c1 = c2;
            }
            byte[] resultado = salida.ToArray();
            using (var Stream = new FileStream(path, FileMode.Append))
            {
                Stream.Write(resultado, 0, resultado.Length);
            }
        }
    }
}
