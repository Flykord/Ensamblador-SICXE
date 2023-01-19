using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SICXEAnalizer
{
    public partial class Cargador : Form
    {
        public string pg1, pg2;
        public string valorInicialMapa;
        public string nombreSegundaSeccion, nombreDelPrograma;
        Dictionary<string, List<string>> tabSE = new Dictionary<string, List<string>>();
        Dictionary<string, string> simbolosDPrograma = new Dictionary<string, string>();
        Dictionary<string, string> simbolosDSegundaSeccion = new Dictionary<string, string>();
        List<string> files;
        List<List<string>> secciones = new List<List<string>>();
        List<string[]> tabse = new List<string[]>();
        public int dirSec { get; set; }
        public Cargador(string[] p1, string[] p2, string nomb1, string nomb2, Dictionary<string, string> simbolos1, Dictionary<string, string> simbolos2)
        {
            nombreDelPrograma = nomb1;
            nombreSegundaSeccion = nomb2;
            simbolosDPrograma = simbolos1;
            simbolosDSegundaSeccion = simbolos2;
            //DIREJ_dataGridView1.Visible = false;
            InitializeComponent();
            string[] programa = new string[2]; ;

            pasoUno(p1);
            pasoUno(p2);
            pasoDos(p1, MapaMemoria_dataGridView1,Color.Yellow);
            pasoDos(p2, MapaMemoria_dataGridView1,Color.Red);
            //CargadorLigadorTabse();
            //MapaDeMemoria();
            foreach (string[] s in tabse)
            {
                foreach (string a in s)
                {
                    // MessageBox.Show(a);

                }
                Tabse_dataGridView1.Rows.Add(s);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public string CompletarBits(string s)
        {
            string dato = s; ;
            switch (s.Length)
            {
                case 1:
                    dato = "000" + dato;
                    break;
                case 2:
                    dato = "00" + dato;
                    break;
                case 3:
                    dato = "0´" + dato;
                    break;
            }
            return dato;
        }

        public void MapaDeMemoria()
        {
            int i = 0;
            int value = 0000;

            while (value < 2820)
            {
                MapaMemoria_dataGridView1.Rows.Add(1);
                MapaMemoria_dataGridView1.Rows[i].Cells[0].Value = CompletarBits(value.ToString());
                value += 10;
                i++;
            }

            string dirsc = "0000";
            string direj = "0000";

            string[] programas = new string[2];
            programas[0] = pg1;
            programas[1] = pg2;

            foreach (string programa in programas)
            {

                string[] registros = programa.Split('\n');

                int lonsc = HexadecimalDecimal(registros[0].Substring(13));

                foreach (string registro in registros)
                {
                    if (registro[0] == 'T')
                    {
                        //string direccion = registro.Substring(3, 4);

                        var final = HexLiteralToLong(dirsc) + HexLiteralToLong(registro.Substring(3, 4)); //adding hex values
                        string direccion = CompletarBits(DecimalHexadecimal((int)final).ToString());

                        //remplazar el ultimo digito con el 0 para buscar en la fila de mapa de memoria
                        string direccionFila = direccion.Remove(3, 1) + '0';
                        //para buscar la columna que corresponde
                        string ultimoDigitoColumnas = direccion[3].ToString();

                        bool yaNoImportaLaDireccion = false;
                        bool salir = false;
                        int index = 0;
                        for (int fila = 0; fila < MapaMemoria_dataGridView1.Rows.Count - 1; fila++)
                        {
                            if (salir)
                            {
                                break;
                            }
                            if (direccionFila == MapaMemoria_dataGridView1.Rows[fila].Cells[0].Value.ToString() || yaNoImportaLaDireccion)
                            {
                                for (int col = 0; col < MapaMemoria_dataGridView1.Rows[fila].Cells.Count; col++)
                                {
                                    if (ultimoDigitoColumnas == MapaMemoria_dataGridView1.Rows[fila].Cells[col].OwningColumn.HeaderText || yaNoImportaLaDireccion)
                                    {
                                        MapaMemoria_dataGridView1.Rows[fila].Cells[col].Value = registro.Substring(9 + index, 2);
                                        index = index + 2;

                                        int registrosLength = registro.Length;
                                        if (9 + index == registrosLength)
                                        {
                                            salir = true;
                                            break;
                                        }
                                        yaNoImportaLaDireccion = true;
                                    }

                                }
                            }
                        }
                    }
                    else if (registro[0] == 'M')
                    {
                        List<string> values = new List<string>();
                        string name = registro.Substring(10, 6);
                        string direccionSimbolo = registro.Substring(1, 6);
                        tabSE.TryGetValue(name, out values);

                        var direccion = CompletarBits(DecimalHexadecimal(int.Parse((HexLiteralToLong(dirsc) + HexLiteralToLong(direccionSimbolo)).ToString())));
                        //remplazar el ultimo digito con el 0 para buscar en la fila de mapa de memoria
                        string direccionFila = direccion.Remove(3, 1) + '0';
                        //para buscar la columna que corresponde
                        string ultimoDigitoColumnas = direccion[3].ToString();

                        bool primeraLocalidad = true;
                        int bytesModificar = 0;
                        bool salir = false;
                        int indexEn2 = 0;

                        MessageBox.Show("direccion modificacion " + direccion.ToString());

                        for (int fila = 0; fila < MapaMemoria_dataGridView1.Rows.Count - 1; fila++)
                        {
                            if (salir)
                            {
                                break;
                            }

                            if (direccionFila == MapaMemoria_dataGridView1.Rows[fila].Cells[0].Value.ToString() || !primeraLocalidad)
                            {
                                for (int col = 0; col < MapaMemoria_dataGridView1.Rows[fila].Cells.Count; col++)
                                {
                                    if (ultimoDigitoColumnas == MapaMemoria_dataGridView1.Rows[fila].Cells[col].OwningColumn.HeaderText || !primeraLocalidad)
                                    {
                                        if (primeraLocalidad)
                                        {
                                            primeraLocalidad = false;
                                        }
                                        else
                                        {
                                            string valueAux = MapaMemoria_dataGridView1.Rows[fila].Cells[col].Value.ToString();
                                            MessageBox.Show("value " + valueAux);
                                            valueAux = DecimalHexadecimal(int.Parse((HexLiteralToLong(valueAux) + HexLiteralToLong(direccion.Substring(0 + indexEn2, 2))).ToString()));
                                            indexEn2 = indexEn2 + 2;
                                            MessageBox.Show("new value " + valueAux);

                                            MapaMemoria_dataGridView1.Rows[fila].Cells[col].Value = valueAux;

                                            bytesModificar++;

                                            int registrosLength = registro.Length;
                                            if (bytesModificar == 2)
                                            {
                                                salir = true;
                                                break;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else if (registro[0] == 'E')
                    {
                        if (registro.Length > 1)
                        {
                            MessageBox.Show(registro);
                            direj = registro.Substring(1, 6);
                            //DIREJ_dataGridView1.Rows[0].Cells[0].Value = direj.Substring(1, 4);
                        }
                        break;
                    }

                }
                dirsc = DecimalHexadecimal((HexadecimalDecimal(dirsc) + lonsc));
            }
        }


        public void CargadorLigadorTabse()
        {
            string[] programas = new string[2];
            programas[0] = pg1;
            programas[1] = pg2;

            string dirprog = "0000";
            valorInicialMapa = dirprog;
            string dirsc = dirprog;
            int i = 0;
            int j = 0;
            foreach (string programa in programas)
            {
                string[] registros = programa.Split('\n');

                int lonsc = HexadecimalDecimal(registros[0].Substring(13));

                List<string> values = new List<string>();

                bool existsInTabSE = false;

                if (i == 0)
                {
                    existsInTabSE = tabSE.TryGetValue(nombreDelPrograma, out values);
                    if (existsInTabSE)
                    {
                        MessageBox.Show("ERROR");
                    }
                    else
                    {
                        values = new List<string>();

                        values.Add(dirsc);
                        values.Add(lonsc.ToString());

                        tabSE.Add(nombreDelPrograma, values);

                        Tabse_dataGridView1.Rows.Add(1);
                        Tabse_dataGridView1.Rows[j].Cells[0].Value = nombreDelPrograma;
                        Tabse_dataGridView1.Rows[j].Cells[2].Value = dirsc;
                        Tabse_dataGridView1.Rows[j].Cells[3].Value = DecimalHexadecimal(lonsc);
                        j++;
                    }

                }
                else if (i == 1)
                {
                    existsInTabSE = tabSE.TryGetValue(nombreSegundaSeccion, out values);

                    if (existsInTabSE)
                    {
                        MessageBox.Show("ERROR");
                    }
                    else
                    {
                        values = new List<string>();

                        values.Add(dirsc.ToString());
                        values.Add(lonsc.ToString());

                        tabSE.Add(nombreSegundaSeccion, values);

                        Tabse_dataGridView1.Rows.Add(1);
                        Tabse_dataGridView1.Rows[j].Cells[0].Value = nombreSegundaSeccion;
                        Tabse_dataGridView1.Rows[j].Cells[2].Value = dirsc;
                        Tabse_dataGridView1.Rows[j].Cells[3].Value = DecimalHexadecimal(lonsc);
                        j++;
                    }
                }



                foreach (string registro in registros)
                {
                    if (registro[0] == 'E')
                    {
                        break;
                    }

                    if (registro[0] == 'D')
                    {

                        bool exists = false;

                        if (i == 0)
                        {
                            foreach (KeyValuePair<string, string> entry in simbolosDPrograma)
                            {

                                List<string> value = new List<string>();

                                exists = tabSE.TryGetValue(entry.Key, out value);

                                if (exists)
                                {
                                    MessageBox.Show("ERROR");
                                }
                                else
                                {
                                    value = new List<string>();
                                    int valorSuma = HexadecimalDecimal(dirsc) + HexadecimalDecimal(entry.Value);

                                    value.Add(DecimalHexadecimal(valorSuma));
                                    tabSE.Add(entry.Key, value);

                                    Tabse_dataGridView1.Rows.Add(1);
                                    Tabse_dataGridView1.Rows[j].Cells[1].Value = entry.Key;
                                    Tabse_dataGridView1.Rows[j].Cells[2].Value = DecimalHexadecimal(valorSuma);
                                    j++;
                                }
                            }
                        }
                        else if (i == 1)
                        {
                            foreach (KeyValuePair<string, string> entry in simbolosDSegundaSeccion)
                            {
                                List<string> value = new List<string>();

                                exists = tabSE.TryGetValue(entry.Key, out value);

                                if (exists)
                                {
                                    MessageBox.Show("ERROR");
                                }
                                else
                                {
                                    value = new List<string>();
                                    int valorSuma = HexadecimalDecimal(dirsc) + HexadecimalDecimal(entry.Value);

                                    value.Add(DecimalHexadecimal(valorSuma));
                                    tabSE.Add(entry.Key, value);

                                    Tabse_dataGridView1.Rows.Add(1);
                                    Tabse_dataGridView1.Rows[j].Cells[1].Value = entry.Key;
                                    Tabse_dataGridView1.Rows[j].Cells[2].Value = DecimalHexadecimal(valorSuma);
                                    j++;
                                }
                            }
                        }
                    }


                }
                dirsc = DecimalHexadecimal((HexadecimalDecimal(dirsc) + lonsc));
                i++;
            }
        }

        public static int HexadecimalDecimal(String hexadecimal)
        {

            int numero = 0;

            const int DIVISOR = 16;

            for (int i = 0, j = hexadecimal.Length - 1; i < hexadecimal.Length; i++, j--)
            {

                if (hexadecimal[i] >= '0' && hexadecimal[i] <= '9')
                {
                    numero += (int)Math.Pow(DIVISOR, j) * Convert.ToInt32(hexadecimal[i] + "");
                }
                else if (hexadecimal[i] >= 'A' && hexadecimal[i] <= 'F')
                {
                    numero += (int)Math.Pow(DIVISOR, j) * Convert.ToInt32((hexadecimal[i] - 'A' + 10) + "");
                }
                else
                {
                    return -1;
                }

            }

            return numero;

        }

        private long HexLiteralToLong(string hex)
        {
            //if (string.IsNullOrEmpty(hex)) throw new ArgumentException("hex");

            int i = hex.Length > 1 && hex[0] == '0' && (hex[1] == 'x' || hex[1] == 'X') ? 2 : 0;
            long value = 0;

            while (i < hex.Length)
            {
                int x = hex[i++];

                if
                    (x >= '0' && x <= '9') x = x - '0';
                else if
                    (x >= 'A' && x <= 'F') x = (x - 'A') + 10;
                else if
                    (x >= 'a' && x <= 'f') x = (x - 'a') + 10;
                else
                    throw new ArgumentOutOfRangeException("hex");

                value = 16 * value + x;
            }

            return value;
        }

        public static String DecimalHexadecimal(int numero)
        {
            if (numero == 0)
            {
                return "0";
            }

            char[] letras = { 'A', 'B', 'C', 'D', 'E', 'F' };

            String hexadecimal = "";

            const int DIVISOR = 16;
            long resto = 0;

            for (int i = numero % DIVISOR, j = 0; numero > 0; numero /= DIVISOR, i = numero % DIVISOR, j++)
            {
                resto = i % DIVISOR;
                if (resto >= 10)
                {
                    hexadecimal = letras[resto - 10] + hexadecimal;

                }
                else
                {
                    hexadecimal = resto + hexadecimal;
                }
            }


            return hexadecimal;
        }

        private void Cargador_Load(object sender, EventArgs e)
        {

        }

        public void cargadorAlgoritmo(DataGridView memoria, string[] programa)
        {
            pasoUno(programa);
            //pasoDos(programa, memoria);
        }

        public string pasoUno(string[] programa)
        {
            int dirProg = dirSec;
             
            int dirsc = dirProg;
            int lonsc = 0;
            string nombreSeccion = "";
            bool errorFlag = false;
            string msg = "";
            foreach (string linea in programa)
            {
                if (linea.Length > 0)
                {
                    if (linea[0] == 'H')
                    {
                        lonsc = Convert.ToInt32(linea.Substring(13, 6), 16);
                        nombreSeccion = linea.Substring(1, 6);
                        bool insTABSE = buscaInsertaTabSESeccion(nombreSeccion, dirsc, lonsc);
                        if (!insTABSE)
                        {
                            errorFlag = true;
                            msg += "Error: Símbolo externo duplicado [" + nombreSeccion + "]" + Environment.NewLine;
                            break;
                        }
                    }
                    else if (linea[0] == 'D')
                    {
                        int dLen = linea.Length;
                        int dirPointer = 1;
                        while (dirPointer < dLen)
                        {
                            string simbolo = linea.Substring(dirPointer, 6);
                            dirPointer += 6;
                            bool insSimbolo = buscaTabSESimbolo(simbolo);
                            if (!insSimbolo)
                            {
                                errorFlag = true;
                                msg += "Error: Símbolo externo duplicado [" + simbolo + "]" + Environment.NewLine;
                                break;
                            }
                            int dirSimbolo = Convert.ToInt32(linea.Substring(dirPointer, 6), 16);
                            dirPointer += 6;
                            insertaSimboloTabSe(simbolo, dirSimbolo + dirsc);
                        }
                        if (errorFlag == true) break;

                    }
                }

            }
            dirSec = dirsc + lonsc;
            return msg;
        }

        public string pasoDos(string[] programa, DataGridView memoria,Color c)
        {
            Color color = c;
            
            int dirsc = dirSec;
            int direj = dirsc;
            int lonsc = 0;
            bool errorFlag = false;
            string msg = "";
            foreach (string linea in programa)
            {
                int dirRow = 0;
                if (linea.Length > 0)
                {
                    if (linea[0] == 'H')
                    {
                        lonsc = Convert.ToInt32(linea.Substring(13, 6), 16);

                    }
                    else if (linea[0] == 'T')
                    {
                        // MessageBox.Show("Si");
                        int dirT = Convert.ToInt32(linea.Substring(1, 6), 16);
                        dirRow = ((dirT + dirsc) / 0x10) * 0x10;
                        creaLineaMemoria(memoria, dirRow);
                        cargaRegistroT(memoria, dirT + dirsc, linea, dirRow);
                        //cargaRegistroT(memoria, dirT + dirsc, linea, dirRow);
                    }
                    else if (linea[0] == 'M')
                    {
                        string simbolo = linea.Substring(10, 6);
                        int valor = buscarSimboloTABSE(simbolo);
                        int mObjetivo = 0;
                        if (valor != -1)
                        {
                            int direcc = Convert.ToInt32(linea.Substring(1, 6), 16);
                            
                            
                                 mObjetivo = dirsc + direcc;
                            
                            dirRow = (mObjetivo / 0x10) * 0x10;
                            modificarBytes(linea, mObjetivo, dirRow, memoria, valor,color);
                        }
                        else
                        {
                            //Activa la bandera de error (símbolo externo indefinido)
                            msg += "Error: Símbolo externo indefinido [" + simbolo + "]" + Environment.NewLine;
                            errorFlag = true;
                            break;
                        }

                    }
                    else if (linea[0] == 'E')
                    {
                        if (linea.Length > 1)
                        {
                            direj = dirsc + Convert.ToInt32(linea.Substring(1, 6), 16);
                        }
                        dirSec = dirsc + lonsc;
                    }
                }
            }
            return msg;
        }
        private void modificarBytes(string regM, int direcObjetivo, int dirRow, DataGridView memoria, int val,Color c)
        {
            Color col = c;
            int halfBytes = Convert.ToInt32(regM.Substring(7, 2), 16);
            int rIndex = getIndexofMemoryRow(dirRow, memoria);
            int cIndex = direcObjetivo - dirRow;
            string valor = "";
            int cont = 0;
            if (rIndex == -1)
            {
                creaLineaMemoria(memoria, dirRow);
                //cargaRegistroT(memoria, direcObjetivo, regM, dirRow);
                cargaRegistroMfuera(memoria, direcObjetivo, regM, dirRow, val.ToString("X6"));
            }
            else {
                for (int i = 0; i < 3; i++)
                {
                    if ((cIndex + i) > 15)
                    {
                        rIndex++;
                        cIndex = 0;
                        cont = 0;
                    }
                    if (rIndex != -1)
                    {
                        valor += memoria.Rows[rIndex].Cells[cIndex + cont].Value;
                    }
                    cont++;
                }
                switch (halfBytes)
                {
                    case 5:
                        int mx = Convert.ToInt32(valor.Substring(1, 5), 16);
                        if (regM[9] == '+')
                        {
                            mx = mx + val;
                        }
                        else
                        {
                            mx = mx - val;
                        }
                        valor = valor.Substring(0, 1) + mx.ToString("X5");
                        break;
                    case 6:
                        int mx1 = Convert.ToInt32(valor, 16);
                        if (regM[9] == '+')
                        {
                            mx1 = mx1 + val;
                        }
                        else
                        {
                            mx1 = mx1 - val;
                        }
                        valor = mx1.ToString("X6");
                        break;
                }
                rIndex = getIndexofMemoryRow(dirRow, memoria);
                cIndex = direcObjetivo - dirRow;
                cont = 0;
                for (int i = 0; i < 3; i++)
                {
                    if ((cIndex + i) > 15)
                    {
                        rIndex++;
                        cIndex = 0;
                        cont = 0;
                    }
                    if (rIndex != -1)
                    {
                        // MessageBox.Show(valor.Length.ToString()+"-"+valor);
                        if (valor.Length == (i * 2))
                        {
                            memoria.Rows[rIndex].Cells[cIndex + cont].Value = valor;
                            memoria.Rows[rIndex].Cells[cIndex + cont].Style.BackColor = col;
                        }
                        if (valor.Length > 4)
                        {
                            memoria.Rows[rIndex].Cells[cIndex + cont].Value = valor.Substring(i * 2, 2);
                            memoria.Rows[rIndex].Cells[cIndex + cont].Style.BackColor = col;
                        }
                        if (valor.Length == 4)
                        {
                            memoria.Rows[rIndex].Cells[cIndex + cont].Value = valor.Substring(i * 2 - 2, 2);
                            memoria.Rows[rIndex].Cells[cIndex + cont].Style.BackColor = col;
                        }
                    }
                    cont++;
                }
            }
        }
        private int buscarSimboloTABSE(string simbolo)
        {
            int dir = -1;
            foreach (string[] row in tabse)
            {
                if (row[0] == simbolo || row[1] == simbolo)
                {
                    dir = Convert.ToInt32(row[2], 16);
                }
            }
            return dir;
        }
        public bool buscaInsertaTabSESeccion(string simbolo, int dirIni, int longitud)
        {
            bool found = true;
            for (int i = 0; i < tabse.Count; i++)
            {
                if (tabse[i][0] == simbolo)
                {
                    found = false;
                    break;
                }
            }

            if (found == true)
            {
                string[] row = { simbolo, "", dirIni.ToString("X6"), longitud.ToString("X6") };
                tabse.Add(row);
            }
            return found;
        }

        public bool buscaTabSESimbolo(string simbolo)
        {
            bool found = true;
            for (int i = 0; i < tabse.Count; i++)
            {
                if (tabse[i][1] == simbolo)
                {
                    found = false;
                    break;
                }
            }

            return found;
        }

        public void insertaSimboloTabSe(string simbolo, int dir)
        {
            string[] row = { "", simbolo, dir.ToString("X6"), "" };
            tabse.Add(row);
        }

        public void creaLineaMemoria(DataGridView memoria, int direccion)
        {
            bool creado = true;
            for (int i = 0; i < memoria.RowCount; i++)
            {
                if (Convert.ToInt32((string)memoria.Rows[i].HeaderCell.Value, 16) == direccion)
                {
                    creado = false;
                    break;
                }
            }

            if (creado)
            {
                DataGridViewRow rn = new DataGridViewRow();
                rn.HeaderCell.Value = direccion.ToString("X6");
                memoria.Rows.Add(rn);
            }

            /**/
        }

        public void cargaRegistroT(DataGridView memoria, int direccion, string linea, int dirRenglon)
        {
            int offset = direccion - dirRenglon;
            int longitud = Convert.ToInt32(linea.Substring(7, 2), 16);
            int indiceMemoriaRow = getIndexofMemoryRow(dirRenglon, memoria);
            int TPointer = 9;

            for (int i = 0; i < longitud; i++)
            {
                string obByte = linea.Substring(TPointer, 2);
                TPointer += 2;
                memoria.Rows[indiceMemoriaRow].Cells[offset].Value = obByte;
                offset++;
                if (offset > 15)
                {
                    DataGridViewRow rn = new DataGridViewRow();
                    rn.HeaderCell.Value = (dirRenglon + 0x10).ToString("X6");
                    memoria.Rows.Add(rn);
                    indiceMemoriaRow++;
                    offset = 0;
                }
            }

        }

        public void cargaRegistroMfuera(DataGridView memoria, int direccion, string linea, int dirRenglon,string valor)
        {
            int offset = direccion - dirRenglon;
            int longitud = 3;
            int indiceMemoriaRow = getIndexofMemoryRow(dirRenglon, memoria);
            int TPointer = 0;

            for (int i = 0; i < longitud; i++)
            {
                string obByte = valor.Substring(TPointer, 2);
                TPointer += 2;
                memoria.Rows[indiceMemoriaRow].Cells[offset].Value = obByte;
                offset++;
                if (offset > 15)
                {
                    DataGridViewRow rn = new DataGridViewRow();
                    rn.HeaderCell.Value = (dirRenglon + 0x10).ToString("X6");
                    memoria.Rows.Add(rn);
                    indiceMemoriaRow++;
                    offset = 0;
                }
            }

        }
        private void MapaMemoria_dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public int getIndexofMemoryRow(int dir, DataGridView memoria)
        {
            int index = -1;

            for (int i = 0; i < memoria.RowCount; i++)
            {
                if (Convert.ToInt32((string)memoria.Rows[i].HeaderCell.Value, 16) == dir)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public List<string[]> Tabse
        {
            get { return tabse; }

            set { tabse = value; }
        }
    }
}
