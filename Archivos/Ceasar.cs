﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Archivos
{
    public class CaesarViejo
    {
        public string Palabra;
        public string Texto;
        public List<char> ListaPalabra;
        public List<char> Abecedario;
        public List<char> ListaTexto = new List<char>();
        public List<char> ListaCifrada = new List<char>();
        public Dictionary<char, char> DicccionarioCifrado = new Dictionary<char, char>();
        public CaesarViejo(string palabra, string texto)
        {
            Palabra = palabra;
            Texto = texto;
            Abecedario = "abcdefghijklmnñopqrstuvwxyz".ToArray().ToList();
            ListaTexto = Texto.ToArray().ToList();
        }

        public string Cifrado()
        {
            int posicion = 0;
            string cifrado = "";
            PalabraAbecedario();
            for (int i = 0; i < ListaTexto.Count(); i++)
            {
                if (Abecedario.Contains(ListaTexto.ElementAt(i)))
                {
                    posicion = Abecedario.IndexOf(ListaTexto.ElementAt(i));
                    ListaCifrada.Add(ListaPalabra.ElementAt(posicion));
                }
                else
                {
                    ListaCifrada.Add(ListaTexto.ElementAt(i));
                }

            }
            cifrado = string.Join('↔', ListaCifrada);
            cifrado = cifrado.Replace("↔", "");
            return cifrado;
        }

        public string Descifrado()
        {
            int posicion = 0;
            string descifrado = "";
            PalabraAbecedario();
            for (int i = 0; i < ListaTexto.Count(); i++)
            {
                if (ListaPalabra.Contains(ListaTexto.ElementAt(i)))
                {
                    posicion = ListaPalabra.IndexOf(ListaTexto.ElementAt(i));
                    ListaCifrada.Add(Abecedario.ElementAt(posicion));
                }
                else
                {
                    ListaCifrada.Add(ListaTexto.ElementAt(i));
                }

            }
            descifrado = string.Join('↔', ListaCifrada);
            descifrado = descifrado.Replace("↔", "");
            return descifrado;
        }

        public void PalabraAbecedario()
        {
            ListaPalabra = Palabra.ToArray().ToList();
            ListaPalabra = ((from s in ListaPalabra select s).Distinct()).ToList();
            ListaPalabra = ListaPalabra.Union(Abecedario).ToList();
            ListaPalabra = ((from s in ListaPalabra select s).Distinct()).ToList();

        }


    }
    public class Ceasar
    {
        List<byte> CaracteresOriginales = new List<byte> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255 };

        Queue<byte> TextoEntrada = new Queue<byte>(); 
        Queue<byte> TextoCifrado = new Queue<byte>(); 
        List<byte> Llave = new List<byte>();
        Dictionary<byte, byte> DiccionarioFinal = new Dictionary<byte, byte>();
        Dictionary<byte, byte> DiccionarioInverso = new Dictionary<byte, byte>();

        //METODOS PRINCIPALES
        public string Cifrar(string RutaEntrada, string Clave)
        {
            Llave.Clear();
            TextoCifrado.Clear();
            TextoEntrada.Clear();
            DiccionarioFinal.Clear();
            ClaveALista(Clave, Llave);
            ArmarDiccionario(Llave, CaracteresOriginales);
            LeerString(TextoEntrada, RutaEntrada);
            SustituirCaracteres(DiccionarioFinal, TextoEntrada, TextoCifrado); 
            //INTERCAMBIO DE NOMBRE AL ARCHIVO DE TEXTO
            return TextoCifra(TextoCifrado);
        }
        public void LeerString(Queue<byte> ColaChar, string Texto)
        {
            foreach (char Caracter in Texto)
            {
                ColaChar.Enqueue(Convert.ToByte(Caracter));
            }
        }
        public string Descifrar(string RutaEntrada, string Clave)
        {
            Llave.Clear();
            TextoCifrado.Clear();
            TextoEntrada.Clear();
            DiccionarioFinal.Clear();
            ClaveALista(Clave, Llave);
            ArmarDiccionario(Llave, CaracteresOriginales);
            LeerString(TextoEntrada, RutaEntrada);
            SustituirCaracteres(DiccionarioInverso, TextoEntrada, TextoCifrado); 
            return TextoCifra(TextoCifrado);
        }


        void LeerArchivo(Queue<byte> TextoAleer, string rutaOrigen) 
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
        string TextoCifra(Queue<byte> ColaByte)
        {
            string Resultado = "";
            while (ColaByte.Count > 0)
            {
                char Letra = Convert.ToChar(ColaByte.Dequeue());
                Resultado += Letra;
            }
            return Resultado;
        }
        void EscribirArchivo(Queue<byte> Texto, string ruta) 
        {
            using (var file = new FileStream(ruta, FileMode.Create))
            {
                using (var writer = new BinaryWriter(file))
                {
                    while (Texto.Count > 0)
                    {
                        writer.Write(Texto.Dequeue());
                    }
                }
            }
        }


        void ClaveALista(string Llave, List<byte> Cola)
        {
            char[] Clave = Llave.ToCharArray();
            foreach (var item in Clave)
            {
                Cola.Add(Convert.ToByte(item));
            }
        }
        void ArmarDiccionario(List<byte> Clave, List<byte> DiccionarioOriginal) 
        {
            List<byte> Auxiliar = new List<byte>();
            Auxiliar = DiccionarioOriginal;
            foreach (var item in Clave)
            {
                if (Auxiliar.Contains(item))
                {
                    Auxiliar.Remove(item);
                }
            }
            Clave.AddRange(Auxiliar);
            for (int i = 0; i < Clave.Count; i++)
            {
                DiccionarioFinal.Add(Convert.ToByte(i), Clave[i]);
                DiccionarioInverso.Add(Clave[i], Convert.ToByte(i));
            }
        }
        void SustituirCaracteres(Dictionary<byte, byte> Diccionario, Queue<byte> Texto, Queue<byte> TextoSalida)
        {
            while (Texto.Count > 0)
            {
                TextoSalida.Enqueue(Diccionario[Texto.Dequeue()]); 
            }
        }
    }
}
