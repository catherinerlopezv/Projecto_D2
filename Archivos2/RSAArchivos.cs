using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Archivos2
{

    public class Cifrado
    {
        public List<byte> RSACifrado(FileStream archivo, int n, int e)
        {
            List<byte> lista = new List<byte>();
            var reader = new BinaryReader(archivo);
            var buffer = new byte[2000000];
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                buffer = reader.ReadBytes(2000000);
                foreach (var item in buffer)
                {
                    var ok = BigInteger.ModPow(item, (BigInteger)e, (BigInteger)n);
                    byte[] bytes = BitConverter.GetBytes((long)ok);
                    foreach (var b in bytes)
                    {
                        lista.Add(b);
                    }
                }
            }
            reader.Close();
            archivo.Close();

            return lista;
        }
        public List<string> generarLlaves(int p, int q)
        {
            GenerarLlave Llave = new GenerarLlave();
            return Llave.generarLlave(p, q);
        }
    }
    public class GenerarLlave
    {
        public List<string> generarLlave(int p, int q)
        {
            List<string> llave = new List<string>();
            int n = p * q;
            int phi = (p - 1) * (q - 1);
            int e = CalE(phi);
            int d = calD(phi, e);
            llave.Add(n + "," + e);
            llave.Add(n + "," + d);
            return llave;
        }

        int CalE(int phi)
        {
            int e = 2;
            int n = 2;

            while (phi % e == 0)
            {
                bool esPrimo = true;
                for (int i = 2; i < n; i++)
                {
                    if (n % i == 0)
                    {
                        esPrimo = false;
                        break;
                    }
                }

                if (esPrimo)
                {
                    e = n;
                }

                n++;
            }

            return e;
        }

        int calD(int phi, int e)
        {
            int numAux = 0;
            int aux2 = 0;
            int aux3 = 0;
            int[,] intervalos = new int[2, 2];
            intervalos[0, 0] = phi;
            intervalos[0, 1] = phi;
            intervalos[1, 0] = e;
            intervalos[1, 1] = 1;

            while (intervalos[1, 0] != 1)
            {
                numAux = intervalos[0, 0] / intervalos[1, 0];
                aux2 = intervalos[0, 0];
                aux3 = intervalos[0, 1];
                intervalos[0, 0] = intervalos[1, 0];
                intervalos[0, 1] = intervalos[1, 1];
                intervalos[1, 0] = aux2 - (intervalos[1, 0] * numAux);
                intervalos[1, 1] = aux3 - (intervalos[1, 1] * numAux);

                if (intervalos[1, 1] < 0)
                {
                    int numero = intervalos[1, 1];
                    intervalos[1, 1] = (numero % phi + phi) % phi;
                }
            }

            return intervalos[1, 1];
        }

    }
    public class Descifrar
    {
        public List<byte> descifrar(FileStream archivo, int n, int d)
        {
            List<byte> bpb = new List<byte>();
            int contador = 0;
            var reader = new BinaryReader(archivo);
            var buffer = new byte[2000000];
            List<byte> bytes = new List<byte>();
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                buffer = reader.ReadBytes(2000000);
                foreach (var item in buffer)
                {
                    bytes.Add(item);
                    if (bytes.Count == 8)
                    {
                        byte[] by = new byte[bytes.Count];
                        foreach (var bytee in bytes)
                        {
                            by[contador] = bytee;
                            contador++;
                        }
                        long num = BitConverter.ToInt32(by, 0);
                        var ok = BigInteger.ModPow(num, (BigInteger)d, (BigInteger)n);
                        bpb.Add((byte)ok);
                        bytes.Clear();
                        contador = 0;
                    }
                }
            }
            reader.Close();
            archivo.Close();

            return bpb;
        }

    }
    public class RSAArchivos
    {
        int p = 0, q = 0, n = 0, nmod = 0, e = 0, D; 
        Queue<byte> entradas = new Queue<byte>(); 
        Queue<byte> cifrado = new Queue<byte>(); 

   
        public void GenerarLLave(int p, int q)
        { 
            this.p = p;
            this.q = q;
            n = (this.p * this.q); 
            nmod = (this.p - 1) * (this.q - 1); 
        RCalculos:
            e = encontrar(nmod, n); 
            D = inversoM(e, nmod); 
            if (D == e)
            {
                goto RCalculos;
            }
            var kpublica = Tuple.Create(n, e);
            var kprivada = Tuple.Create(n, D); 
            var raiz1 = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        "public.key");
            var raiz2 = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        "private.key");
            escribir(raiz1, kpublica);
            escribir(raiz2, kprivada);
        }
        public void CifrarMod(string llaves, string texto) 
        {
            Tuple<int, int> llave = guardarLlave(llaves);
            leerArchivos(entradas, texto);
            int e = llave.Item1;
            int n = llave.Item2;
            while (entradas.Count > 0)
            {
                BigInteger Be = new BigInteger(entradas.Dequeue());
                BigInteger exponentes = new BigInteger(n);
                BigInteger modulo = new BigInteger(e);
                int cifrarI = (int)(BigInteger.ModPow(Be, exponentes, modulo));      
                if (cifrarI < 256)
                {
                    cifrado.Enqueue(0);
                    cifrado.Enqueue(0);
                    cifrado.Enqueue(Convert.ToByte(cifrarI));
                }
                if (cifrarI > 255 && cifrarI < 65026)
                {
                    cifrado.Enqueue(0);
                    int c = cifrarI / 255;
                    cifrado.Enqueue(Convert.ToByte(c));
                    int r = cifrarI % 255;
                    cifrado.Enqueue(Convert.ToByte(r));
                }
                if (cifrarI > 65026)
                {
                    int c = cifrarI / 65025;
                    cifrado.Enqueue(Convert.ToByte(c));
                    c = cifrarI % 65025;
                    cifrado.Enqueue(Convert.ToByte((c / 255)));
                    int r = c % 255;
                    cifrado.Enqueue(Convert.ToByte(r));

                }
            } 
           
            FileInfo informacionArchivo = new FileInfo(texto);
            informacionArchivo.Name.Replace(".txt", ".cif");
        }
        public void DescifrarMod(string llaves, string texto)
        {
            Queue<int> original = new Queue<int>();
            Tuple<int, int> k = guardarLlave(llaves);
            leerArchivos(entradas, texto);
            int e = k.Item1;
            int n = k.Item2;

            depurar(original, entradas);
            while (original.Count > 0)
            {
                
                BigInteger Be = new BigInteger(original.Dequeue());
                BigInteger exponentes = new BigInteger(n);
                BigInteger modulo = new BigInteger(e);
                int descifrarI = (int)(BigInteger.ModPow(Be, exponentes, modulo));
                if (descifrarI > 255) 
                {
                    descifrarI %= 40;
                }
                cifrado.Enqueue(Convert.ToByte(descifrarI));
            }
        
            FileInfo informacionArchivo = new FileInfo(texto);
            string rutaEncriptada = informacionArchivo.Name.Replace(".cif", ".txt");
            escribirArchivos(cifrado, rutaEncriptada);
        }
    
        public bool revisarPrimo(int n) 
        {
            int divisor = 0;
            for (int i = 1; i <= n; i++)
            {
                if (n % i == 0)
                {
                    divisor++;
                }
            }
            if (divisor == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ndiferente(int a, int b)
        {
            if (a != b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool mayor(int a, int b)
        {
            if ((a * b) >= 255)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    
        int nmcd(int nmayor, int nmenor)
        {
            int resultado = 0;
            do 
            {
                resultado = nmenor;
                nmenor = nmayor % nmenor;
                nmayor = resultado;
            } while (nmenor != 0);
            return resultado;
        }
        int encontrar(int Phi, int N) 
        {
            List<int> primos = new List<int>();
            for (int i = 2; i < Phi; i++) 
            {
                if ((nmcd(Phi, i) == 1) && (nmcd(N, i) == 1))
                {
                    primos.Add(i);
                }

            }
            Random r = new Random();
            return primos[r.Next(0, primos.Count / 2)];

        }
        int inversoM(int Entrada, int modulo)
        {
            int temp = modulo, salida = 0, Columna = 1;
            while (Entrada > 0)
            {
                int Entero_SiguienteIteracion = temp / Entrada, ModuloPrevio = Entrada;
                Entrada = temp % ModuloPrevio;
                temp = ModuloPrevio;
                ModuloPrevio = Columna;
                Columna = salida - Entero_SiguienteIteracion * ModuloPrevio;
                salida = ModuloPrevio;

            }
            salida %= modulo;
            if (salida < 0) salida = (salida + modulo) % modulo;
            return salida;
        }
        void escribir(string nombre, Tuple<int, int> t) 
        {
            StreamWriter writer = new StreamWriter(nombre);
            writer.Write(t.Item1 + "," + t.Item2);
            writer.Close();
        }

        Tuple<int, int> guardarLlave(string path)
        {
            StreamReader leer = new StreamReader(path);
            string texto = leer.ReadLine();
            string[] split = texto.Split(',');
            int e = Convert.ToInt32(split[0]);
            int n = Convert.ToInt32(split[1]);
            var k = Tuple.Create(e, n);
            leer.Close();
            return k;
        }
        void leerArchivos(Queue<byte> texto, string raiz) 
        {
            const int limite = 1024;
            var buffer = new byte[limite];
            using (var archivo = new FileStream(raiz, FileMode.Open))
            {
                using (var leer = new BinaryReader(archivo))
                {
                    while (leer.BaseStream.Position != leer.BaseStream.Length)
                    {
                        buffer = leer.ReadBytes(limite);
                        foreach (var item in buffer)
                        {
                            texto.Enqueue(item);
                        }
                    }

                }

            }


        }
        void escribirArchivos(Queue<byte> texto, string raiz)
        {
            using (var leer = new FileStream(raiz, FileMode.Create))
            {
                using (var escribir = new BinaryWriter(leer))
                {
                    while (texto.Count > 0)
                    {
                        escribir.Write(texto.Dequeue());
                    }
                }
            }
        }
        void depurar(Queue<int> salidas, Queue<byte> texto)
        {
            int cola;
            int c1, c2, c3;
            while (texto.Count > 0)
            {
                c1 = (int)texto.Dequeue();
                c2 = (int)texto.Dequeue();
                c3 = (int)texto.Dequeue();
                cola = (c1 * 65025) + (c2 * 255) + c3;
                salidas.Enqueue(cola);
            }
        }
    }
}
