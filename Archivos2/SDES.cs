using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Archivos2
{
    class Config
    {
        public List<int> P10(List<int> cambiar)
        {
            List<int> orden = obTenerConfiguracion("Permutations.txt", 1).Split(',').Select(int.Parse).ToList();
            cambiar = orden.Select(i => cambiar[i]).ToList();
            return cambiar;
        }
        public List<int> P8(List<int> cambiar)
        {
            List<int> orden = obTenerConfiguracion("Permutations.txt", 2).Split(',').Select(int.Parse).ToList();
            cambiar = orden.Select(i => cambiar[i]).ToList();
            return cambiar;
        }
        public List<int> P4(List<int> cambiar)
        {
            List<int> orden = obTenerConfiguracion("Permutations.txt", 3).Split(',').Select(int.Parse).ToList();
            cambiar = orden.Select(i => cambiar[i]).ToList();
            return cambiar;
        }
        public List<int> EP(List<int> cambiar)
        {
            List<int> orden = obTenerConfiguracion("Permutations.txt", 4).Split(',').Select(int.Parse).ToList();
            cambiar = orden.Select(i => cambiar[i]).ToList();
            return cambiar;
        }
        public List<int> IP(List<int> cambiar)
        {
            List<int> orden = obTenerConfiguracion("Permutations.txt", 5).Split(',').Select(int.Parse).ToList();
            cambiar = orden.Select(i => cambiar[i]).ToList();
            return cambiar;
        }

        public List<int> IP1(List<int> cambiar)
        {
            List<int> orden = obTenerConfiguracion("Permutations.txt", 6).Split(',').Select(int.Parse).ToList();
            cambiar = orden.Select(i => cambiar[i]).ToList();
            return cambiar;
        }

        public List<int> EPpK1(List<int> EP, List<int> K1)
        {
            List<int> cambiar = new List<int>();
            for (int i = 0; i < K1.Count; i++)
            {
                int temp = 0;
                if (EP[i] == 0 && K1[i] == 1)
                {
                    temp = 1;
                }
                else if (EP[i] == 1 && K1[i] == 0)
                {
                    temp = 1;
                }
                else
                {
                    temp = 0;
                }
                cambiar.Add(temp);
            }
            return cambiar;
        }

        public List<int> S0Tabla(List<int> verificar)
        {
            List<int> cambiar = new List<int>();
            string[,] S0 = new string[,]
            {
                { "01", "00", "11", "10" },
                { "11", "10", "01", "00" },
                { "00", "10", "01", "11" },
                { "11", "01", "11", "10" }
            };

            int fila = 0;
            int columna = 0;
            List<int> listaF = new List<int>();
            listaF.Add(verificar[0]);
            listaF.Add(verificar[3]);
            string intString = intAstring(listaF);
            if (intString.Equals("00"))
            {
                fila = 0;
            }
            else if (intString.Equals("01"))
            {
                fila = 1;
            }
            else if (intString.Equals("10"))
            {
                fila = 2;
            }
            else
            {
                fila = 3;
            }
            List<int> listaC = new List<int>();
            listaC.Add(verificar[1]);
            listaC.Add(verificar[2]);
            string intStringC = intAstring(listaC);

            if (intStringC.Equals("00"))
            {
                columna = 0;
            }
            else if (intStringC.Equals("01"))
            {
                columna = 1;
            }
            else if (intStringC.Equals("10"))
            {
                columna = 2;
            }
            else
            {
                columna = 3;
            }

            string tabla = S0[fila, columna];
            cambiar = stringAInt(tabla);

            return cambiar;
        }

        public List<int> S1Tabla(List<int> verificar)
        {
            List<int> cambiar = new List<int>();
            string[,] S1 = new string[,]
            {
                { "00", "01", "10", "11" },
                { "10", "00", "01", "11" },
                { "11", "00", "01", "00" },
                { "10", "01", "00", "11" }
            };

            int fila = 0;
            int columna = 0;
            List<int> listaF = new List<int>();
            listaF.Add(verificar[0]);
            listaF.Add(verificar[3]);
            string intString = intAstring(listaF);
            if (intString.Equals("00"))
            {
                fila = 0;
            }
            else if (intString.Equals("01"))
            {
                fila = 1;
            }
            else if (intString.Equals("10"))
            {
                fila = 2;
            }
            else
            {
                fila = 3;
            }
            List<int> listaC = new List<int>();
            listaC.Add(verificar[1]);
            listaC.Add(verificar[2]);
            string intString2 = intAstring(listaC);

            if (intString2.Equals("00"))
            {
                columna = 0;
            }
            else if (intString2.Equals("01"))
            {
                columna = 1;
            }
            else if (intString2.Equals("10"))
            {
                columna = 2;
            }
            else
            {
                columna = 3;
            }
            var tabla = S1[fila, columna];
            cambiar = stringAInt(tabla);

            return cambiar;
        }

        public List<int> XOR(List<int> p1, List<int> p2)
        {
            List<int> cambiar = p1;
            cambiar.AddRange(p2);
            return cambiar;
        }



        public List<int> SW(List<int> leftInitList, List<int> outputPtwotList)
        {
            List<int> output = new List<int>();
            for (int i = 0; i < leftInitList.Count; i++)
            {
                var temp = 0;
                if (leftInitList[i] == 0 && outputPtwotList[i] == 1)
                    temp = 1;
                else if (leftInitList[i] == 1 && outputPtwotList[i] == 0)
                    temp = 1;
                else
                    temp = 0;

                output.Add(temp);
            }
            return output;
        }



        public List<int> stringAInt(string codigo)
        {
            List<int> salida = new List<int>();
            for (int i = 0; i < codigo.Length; i++)
            {
                salida.Add((int)Char.GetNumericValue(codigo[i]));
            }
            return salida;
        }

        public string intAstring(List<int> codigo)
        {
            StringBuilder temp = new StringBuilder();
            foreach (var i in codigo)
            {
                temp.Append(i);
            }
            return temp.ToString();
        }
        static string obTenerConfiguracion(string archivo, int linea)
        {
            string[] lines = File.ReadAllLines(archivo);

            return lines[linea - 1];
        }
    }
    class Encriptar
    {
        readonly Config configuracion = new Config();
        public string encriptar(string llave1, string llave2, string codigo)
        {
            return SdesEncriptar(llave1, llave2, codigo);
        }

        private string SdesEncriptar(string llave1, string llave2, string codigo)
        {
            List<int> llave1L = configuracion.stringAInt(llave1);
            List<int> llave2L = configuracion.stringAInt(llave2);
            List<int> codigoL = configuracion.stringAInt(codigo);

            // IP 
            List<int> IP = configuracion.IP(codigoL);

            // /4
            List<int> ladoizq = new List<int>();
            List<int> ladoder = new List<int>();
            for (int i = 0; i < codigoL.Count; i++)
            {
                var temp = IP[i];
                if (i < 4)
                {
                    ladoizq.Add(temp);
                }
                else
                {
                    ladoder.Add(temp);
                }
            }

            //  EP
            List<int> EP = configuracion.EP(ladoder);

            // EP + Llave1
            List<int> EPK1 = configuracion.EPpK1(EP, llave1L);

            // + /4
            List<int> ladoizqXor = new List<int>();
            List<int> ladoderXor = new List<int>();
            for (int i = 0; i < EPK1.Count; i++)
            {
                var temp = EPK1[i];
                if (i < 4)
                {
                    ladoizqXor.Add(temp);
                }
                else
                {
                    ladoderXor.Add(temp);
                }
            }

            // S0 / S1
            List<int> S0 = configuracion.S0Tabla(ladoizqXor);
            List<int> S1 = configuracion.S1Tabla(ladoderXor);

            // + 
            List<int> S0PS1 = configuracion.XOR(S0, S1);

            // P4
            List<int> P4 = configuracion.P4(S0PS1);

            // SW
            List<int> SW = configuracion.SW(ladoizq, P4);

            // EP2
            List<int> EP2 = configuracion.EP(SW);

            // EP2 + Llave2
            List<int> EPK2 = configuracion.EPpK1(EP2, llave2L);

            // +/4
            List<int> ladoizqXor2 = new List<int>();
            List<int> ladoderXor2 = new List<int>();
            for (int i = 0; i < EPK2.Count; i++)
            {
                var temp = EPK2[i];
                if (i < 4)
                {
                    ladoizqXor2.Add(temp);
                }
                else
                {
                    ladoderXor2.Add(temp);
                }
            }

            // S0 / S1
            List<int> S02 = configuracion.S0Tabla(ladoizqXor2);
            List<int> S12 = configuracion.S1Tabla(ladoderXor2);

            // +
            List<int> S0PS12 = configuracion.XOR(S02, S12);

            // P4
            List<int> P42 = configuracion.P4(S0PS12);

            //  P4+LadoDer
            List<int> SW2 = configuracion.SW(ladoder, P42);

            // + /4
            List<int> FS = configuracion.XOR(SW2, SW);

            // IP-1
            List<int> IP1 = configuracion.IP1(FS);

            return configuracion.intAstring(IP1);
        }
    }
    class Desencriptar
    {
        Config configurar = new Config();
        public string desencriptar(string llave1, string llave2, string codigo)
        {
            return SdesDesencriptar(llave1, llave2, codigo);
        }
        private string SdesDesencriptar(string llave1, string llave2, string codigo)
        {
            List<int> llave1L = configurar.stringAInt(llave1);
            List<int> llave2L = configurar.stringAInt(llave2);
            List<int> codigoL = configurar.stringAInt(codigo);

            // IP 
            List<int> IP = configurar.IP(codigoL);

            // /4
            List<int> ladoizq = new List<int>();
            List<int> ladoder = new List<int>();
            for (int i = 0; i < codigoL.Count; i++)
            {
                var temp = IP[i];
                if (i < 4)
                {
                    ladoizq.Add(temp);
                }
                else
                {
                    ladoder.Add(temp);
                }
            }

            //  EP
            List<int> EP = configurar.EP(ladoder);

            // EP +Llave"1" =2
            List<int> EPK1 = configurar.EPpK1(EP, llave2L);

            // + /4 
            List<int> ladoizqXor = new List<int>();
            List<int> ladoderXor = new List<int>();
            for (int i = 0; i < EPK1.Count; i++)
            {
                var temp = EPK1[i];
                if (i < 4)

                {
                    ladoizqXor.Add(temp);
                }
                else
                {
                    ladoderXor.Add(temp);
                }
            }
            // S0/S1
            List<int> S0 = configurar.S0Tabla(ladoizqXor);
            List<int> S1 = configurar.S1Tabla(ladoderXor);

            // +
            List<int> S0PS1 = configurar.XOR(S0, S1);

            // P4 
            List<int> P4 = configurar.P4(S0PS1);

            // SW
            List<int> SW = configurar.SW(ladoizq, P4);

            // EP2
            List<int> EP2 = configurar.EP(SW);

            // EP2 + Llave "2" = 1
            List<int> EPK2 = configurar.EPpK1(EP2, llave1L);

            // +/4
            List<int> ladoizqXor2 = new List<int>();
            List<int> ladoderXor2 = new List<int>();
            for (int i = 0; i < EPK2.Count; i++)
            {
                var temp = EPK2[i];
                if (i < 4)
                {
                    ladoizqXor2.Add(temp);
                }
                else
                {
                    ladoderXor2.Add(temp);
                }
            }
            // S0 / S1
            List<int> S02 = configurar.S0Tabla(ladoizqXor2);
            List<int> S12 = configurar.S1Tabla(ladoderXor2);

            // +
            List<int> S0PS12 = configurar.XOR(S02, S12);

            // P4
            List<int> P42 = configurar.P4(S0PS12);

            // P4+LadoDer
            List<int> SW2 = configurar.SW(ladoder, P42);

            // + /4
            List<int> FS = configurar.XOR(SW2, SW);

            // IP-1
            List<int> IP1 = configurar.IP1(FS);

            return configurar.intAstring(IP1);
        }
    }
    public class SDES
    {



        public SDES()
        {
            ArchivosConfig();
        }
        Queue<byte> TEntrada = new Queue<byte>(); 
        Queue<byte> TCifrado = new Queue<byte>(); 
        string[,] sb1 = { 
                                    { "01", "00", "11", "10"},
                                    { "11", "10", "01", "00"},
                                    { "00", "10", "01", "11"},
                                    { "11", "01", "11", "10"}};
        string[,] sb2 = { 
                                    { "00", "01", "10", "11"},
                                    { "10", "00", "01", "11"},
                                    { "11", "00", "01", "00"},
                                    { "10", "01", "00", "11"}};

        List<string> PArchivo = new List<string>(); 
        int[] P10 = new int[10];
        int[] P8 = new int[8];
        int[] P4 = new int[4];
        int[] PE = new int[8];
        int[] PI = new int[8];
        int[] PI1 = new int[8];
        void ArchivosConfig()
        {

            StreamReader leer = new StreamReader("Permutations.txt");
            string linea;
            while ((linea = leer.ReadLine()) != null)
            {
                PArchivo.Add(linea);
            }
            leer.Close();

            P10 = PArchivo[0].Split(',').Select(Int32.Parse).ToArray();
            P8 = PArchivo[1].Split(',').Select(Int32.Parse).ToArray();
            P4 = PArchivo[2].Split(',').Select(Int32.Parse).ToArray();
            PE = PArchivo[3].Split(',').Select(Int32.Parse).ToArray();
            PI = PArchivo[4].Split(',').Select(Int32.Parse).ToArray();
            PI1 = PArchivo[5].Split(',').Select(Int32.Parse).ToArray();
        }

        public string encriptarP2(int llve, string ruta)
        {
            char[] k1 = new char[8];
            char[] k2 = new char[8];
            TCifrado.Clear(); 
            TEntrada.Clear();
            byte tcifrado = 0; 
            LCadenas(TEntrada, ruta);
            string llaves = Convert.ToString(llve, 2);
            llaves = llaves.PadLeft(10, '0');
            llaves2(llaves, ref k1, ref k2);
            while (TEntrada.Count > 0)
            {
                caractersEspeciales(TEntrada.Dequeue(), k1, k2, ref tcifrado); 
                TCifrado.Enqueue(tcifrado);
            }
            return TCifra(TCifrado);
        }
        public void LCadenas(Queue<byte> CChar, string textos)
        {
            byte[] entradas = Encoding.UTF8.GetBytes(textos);
            foreach (char Caracter in entradas)
            {
                CChar.Enqueue(Convert.ToByte(Caracter));
            }
        }
        public string TCifra(Queue<byte> textos)
        {
            string cntotal = "";
            while (textos.Count > 0)
            {
                int identificar = Convert.ToInt32(textos.Dequeue());
                string letras = Convert.ToString(identificar);
                cntotal += letras + ",";
            }

            return cntotal;
        }
        public string textoAEnviar(Queue<byte> mensajes)
        {
            byte[] arregloMensaje = mensajes.ToArray();
            string salida = Encoding.UTF8.GetString(arregloMensaje);
            return salida;
        }
        public string MDesencriptar2(int llave, string textos)
        {

            char[] k1 = new char[8];
            char[] k2 = new char[8];
            TCifrado.Clear(); 
            TEntrada.Clear(); 
            byte textosCifrados = 0; 
            Queue<byte> bytesManejo = MLectura(textos);
            string llaves = Convert.ToString(llave, 2);
            llaves = llaves.PadLeft(10, '0'); 
            llaves2(llaves, ref k1, ref k2);
            while (bytesManejo.Count > 0)
            {
                caractersEspeciales(bytesManejo.Dequeue(), k2, k1, ref textosCifrados); 
                TCifrado.Enqueue(textosCifrados);
            }
            return MDescifrar(TCifrado);
        }
        public string MDescifrar(Queue<byte> colas)
        {
            //colas=manejo();
            byte[] mensajes = TCifrado.ToArray();
            string cmensaje = Encoding.UTF8.GetString(mensajes);
            return cmensaje;
        }
        public Queue<byte> MLectura(string textos)
        {
            Queue<byte> mensajesB = new Queue<byte>();
            string[] textoB = textos.Split(',');
            for (int i = 0; i < textoB.Length - 1; i++)
            {
                mensajesB.Enqueue(Convert.ToByte(textoB[i]));
            }
            return mensajesB;
        }

        char[] CP10(string n) 
        {
            char[] llaves = n.ToCharArray();
            char[] salidas = new char[10]; 
                                      
            salidas[0] = llaves[P10[0]]; 
            salidas[1] = llaves[P10[1]];
            salidas[2] = llaves[P10[2]];
            salidas[3] = llaves[P10[3]];
            salidas[4] = llaves[P10[4]];
            salidas[5] = llaves[P10[5]];
            salidas[6] = llaves[P10[6]];
            salidas[7] = llaves[P10[7]];
            salidas[8] = llaves[P10[8]];
            salidas[9] = llaves[P10[9]];
            return salidas;
        }
        char[] CP8(char[] n) 
        {
            char[] contar = new char[8];
            contar[0] = n[P8[0]];
            contar[1] = n[P8[1]];
            contar[2] = n[P8[2]];
            contar[3] = n[P8[3]];
            contar[4] = n[P8[4]];
            contar[5] = n[P8[5]];
            contar[6] = n[P8[6]];
            contar[7] = n[P8[7]];
            return contar;
        }
        char[] CP4(char[] n) 
        {
            char[] contar = new char[4]; 
            contar[0] = n[P4[0]];
            contar[1] = n[P4[1]];
            contar[2] = n[P4[2]];
            contar[3] = n[P4[3]];
            return contar;
        }
        char[] CEP(char[] n) 
        {
            char[] contar = new char[8]; 
            contar[0] = n[PE[0]];
            contar[1] = n[PE[1]];
            contar[2] = n[PE[2]];
            contar[3] = n[PE[3]];
            contar[4] = n[PE[4]];
            contar[5] = n[PE[5]];
            contar[6] = n[PE[6]];
            contar[7] = n[PE[7]];
            return contar;
        }
        char[] CPE(char[] n) 
        {
            char[] contar = new char[8]; 
            contar[0] = n[PI[0]];
            contar[1] = n[PI[1]];
            contar[2] = n[PI[2]];
            contar[3] = n[PI[3]];
            contar[4] = n[PI[4]];
            contar[5] = n[PI[5]];
            contar[6] = n[PI[6]];
            contar[7] = n[PI[7]];
            return contar;
        }
        char[] CPI(char[] n) 
        {
            char[] contar = new char[8]; 
            contar[0] = n[PI1[0]];
            contar[1] = n[PI1[1]];
            contar[2] = n[PI1[2]];
            contar[3] = n[PI1[3]];
            contar[4] = n[PI1[4]];
            contar[5] = n[PI1[5]];
            contar[6] = n[PI1[6]];
            contar[7] = n[PI1[7]];
            return contar;
        }

        char[] LS(char[] n) 
        {
            char[] contar = new char[5];
            char p1 = n[0]; 
            contar[0] = n[1];
            contar[1] = n[2];
            contar[2] = n[3];
            contar[3] = n[4];
            contar[4] = p1;
            return contar;
        }
        char[] LS2(char[] n) 
        {
            char[] contar = new char[5];
            char p1 = n[0]; 
            char p2 = n[1]; 
            contar[0] = n[2];
            contar[1] = n[3];
            contar[2] = n[4];
            contar[3] = p1;
            contar[4] = p2;

            return contar;
        }
        char[] CXOR(char[] n, char[] n2) 
        {
            char[] contar = new char[n.Length];
            for (int i = 0; i < n.Length; i++)
            {
                if (n[i] == n2[i])
                {
                    contar[i] = '0';
                }
                else
                {
                    contar[i] = '1';
                }
            }
            return contar;
        }
        void manejodivicion(ref char[] c1, ref char[] c2, char[] fuente)
        {
            int Limite = fuente.Length / 2;
            for (int i = 0; i < (Limite); i++)
            {
                c1[i] = fuente[i];
                c2[i] = fuente[i + Limite];
            }
        }
        void llaves2(string llave, ref char[] k1, ref char[] k2)
        {
            char[] p1 = new char[5];
            char[] p2 = new char[5];
            char[] P10 = CP10(llave); 
            for (int i = 0; i < 5; i++)
            {
                p1[i] = P10[i];
                p2[i] = P10[i + 5];
            } 
            p1 = LS(p1);
            p2 = LS(p2);
            P10 = p1.Concat(p2).ToArray();
            k1 = CP8(P10); 
            p1 = LS2(p1);
            p2 = LS2(p2);
            P10 = p1.Concat(p2).ToArray(); 
            k2 = CP8(P10);

        }
        void sboxes(ref int filas, ref int columnas, char[] fuente)
        {
            string fila = "";
            string columna = "";
            fila = fuente[0].ToString() + fuente[3].ToString();
            columna += fuente[1].ToString() + fuente[2].ToString();
            filas = Convert.ToInt32(fila, 2);
            columnas = Convert.ToInt32(columna, 2);

        }
        char[] salida8bits(byte c) 
        {
            string ConvertirAbinario = Convert.ToString(c, 2); 
            ConvertirAbinario = ConvertirAbinario.PadLeft(8, '0');
            return (ConvertirAbinario.ToCharArray());
        }
        void caractersEspeciales(byte c, char[] k1, char[] k2, ref byte resultado) 
        {
            char[] bits8 = salida8bits(c); 
            char[] temp = new char[8]; 
            char[] p1 = new char[4];
            char[] p2 = new char[4];
            char[] sbA = new char[4];
            char[] sbB = new char[4];             
            char[] boxResultado = new char[4]; 
            char[] tempB = new char[4]; 

            int fila = 0, Columna = 0; 
            bits8 = CPE(bits8);
            manejodivicion(ref p1, ref p2, bits8);

            temp = CEP(p2);
            temp = CXOR(temp, k1);
            manejodivicion(ref sbA, ref sbB, temp); 
            sboxes(ref fila, ref Columna, sbA); 
            sbA = sb1[fila, Columna].ToCharArray();               
            fila = 0; Columna = 0; 
            sboxes(ref fila, ref Columna, sbB);
            sbB = sb2[fila, Columna].ToCharArray();
            boxResultado = sbA.Concat(sbB).ToArray();
            boxResultado = CP4(boxResultado);
            boxResultado = CXOR(boxResultado, p1);
            temp = p2.Concat(boxResultado).ToArray(); 
                                                                     

            manejodivicion(ref p1, ref tempB, temp);
            temp = CEP(tempB); 
            temp = CXOR(temp, k2);
            char[] primero = new char[4];
            char[] ultimo = new char[4];
            manejodivicion(ref primero, ref ultimo, temp);
            fila = 0; Columna = 0; 
            sboxes(ref fila, ref Columna, primero);
            primero = sb1[fila, Columna].ToCharArray(); 
            fila = 0; Columna = 0; 
            sboxes(ref fila, ref Columna, ultimo);
            ultimo = sb2[fila, Columna].ToCharArray();
            boxResultado = primero.Concat(ultimo).ToArray();
            boxResultado = CP4(boxResultado);
            boxResultado = CXOR(boxResultado, p1);
            temp = boxResultado.Concat(tempB).ToArray(); 
            temp = CPI(temp); 
            string escrbir = "";
            for (int i = 0; i < 8; i++)
            {
                escrbir += temp[i];
            }
            int binarios = Convert.ToInt32(escrbir, 2);
            resultado = Convert.ToByte(binarios); 

        }

    }
}
