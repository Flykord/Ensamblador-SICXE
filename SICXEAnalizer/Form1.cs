using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalizadorSICXE;
using Antlr4.Runtime;
using Example.Generated;
//using PruebaGramatica;

namespace SICXEAnalizer
{
    public partial class Form1 : Form
    {
        List<string> s1 = new List<string>();
        List<string> s2 = new List<string>();
        
        private OpenFileDialog doc;
        string nombre = "";
        string document;
        public string objname = "";
        public string objname1 = "";
        string extension;
        public int numeroArchivo = 0;
        public string sec1, sec2;
        Dictionary<string, string> simbolosDPrograma = new Dictionary<string, string>();
        Dictionary<string, string> simbolosDSegundaSeccion = new Dictionary<string, string>();
        string[] nemonicos = { "ADD", "ADDF", "ADDR", "AND", "CLEAR", "COMP", "COMPF", "COMPR", "DIV", "DIVF", "DIVR", "FIX", "FLOAT", "HIO", "J", "JEQ", "JGT", "JLT", "JSUB", "LDA", "LDB", "LDCH", "LDF", "LDL", "LDS", "LDT", "LDX", "LPS", "MUL", "MULF", "MULR", "NORM", "OR", "RD", "RMO", "RSUB", "SHIFTL", "SHIFTR", "SIO", "SSK", "STA", "STB", "STCH", "STF", "STI", "STL", "STS", "STSW", "STT", "STX", "SUB", "SUBF", "SUBR", "SVC", "TD", "TIO", "TIX", "TIXR", "WD" };
        string[] CodOp = { "18", "58", "90", "40", "B4", "28", "88", "A0", "24", "64", "9C", "C4", "C0", "F4", "3C", "30", "34", "38", "48", "00", "68", "50", "70", "08", "6C", "74", "04", "D0", "20", "60", "98", "C8", "44", "D8", "AC", "4C", "A4", "A8", "F0", "EC", "0C", "78", "54", "80", "D4", "14", "7C", "E8", "84", "10", "1C", "5C", "94", "B0", "E0", "F8", "2C", "B8", "DC" };
        string docerr;
        List<string> erroresP1 = new List<string>();
        private string path;
        public Form1()
        {
          
            InitializeComponent();
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
        }

        private void SetText(RichTextBox richTextBox1,string text)
        {
            richTextBox1.Text = text;
            

        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            doc = new OpenFileDialog();
            Errores.Clear();
            doc.Filter = "txt files (*.xe)|*.xe|All files (*.*)|*.*";
            if (doc.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(doc.FileName);
                    document = doc.FileName;
                    docerr = document;
                    objname = document;
                    extension = Path.GetExtension(docerr);
                    docerr = Path.ChangeExtension(docerr, ".err");
                    objname = Path.ChangeExtension(objname, ".obj");
                    objname1 = Path.GetFileName(objname);
                    
                    var contenido = File.ReadAllText(document);
                    var lineCount = 0;
                 

                
                    SetText(richTextBox1,sr.ReadToEnd());
                    //contenido = sr.ReadToEnd();
                    
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Registros.Clear();
            dataGridView1.Rows.Clear();
           
            Errores.Clear();
            tabsimb.Rows.Clear();
            
            int counter = 0;
            string[] nn;
            List<string> words = new List<string>();
            //string a;



            //var contenido = File.ReadAllText(document);
            var contenido = richTextBox1.Text;
            AnalizadorSICXELexer lex = new AnalizadorSICXELexer(new AntlrInputStream(contenido));
            CommonTokenStream tokens = new CommonTokenStream(lex);
            AnalizadorSICXEParser parser = new AnalizadorSICXEParser(tokens);
            List<string> tabsim = new List<string>();
            List<string> formato = new List<string>();
            List<string> regCP = new List<string>();
            List<string> mododir = new List<string>();
            List<string> codigoObjeto = new List<string>();
            List<string> dirtabsim = new List<string>();
            List<string> Registros = new List<string>();
            string tt="";
            int continuar = 0;
            int tamreg = 0;
            int contadorRegs=0;
            string cadenatemporal = "";
            List<int> cpint =new List<int>();
            parser.paso = 1;
            parser.programa();
            tabsim = parser.tabsimb;
            cpint = parser.contProg;
            regCP = parser.conthex;
            mododir = parser.mododir;
            dirtabsim = parser.dirTabimHex;
            formato = parser.format;
            erroresP1 = parser.errores;
            AnalizadorSICXELexer lexi = new AnalizadorSICXELexer(new AntlrInputStream(contenido));
            CommonTokenStream tokens2 = new CommonTokenStream(lexi);
            AnalizadorSICXEParser parser2 = new AnalizadorSICXEParser(tokens2);
            parser2.paso = 2;
            parser2.tabsimb = tabsim;
            parser2.dirTabimHex = dirtabsim;
            parser2.conthex = regCP;
            parser2.programa();
            
            codigoObjeto = parser2.probando;
            int numlinea = 0;
            IEnumerable<string> linen = File.ReadLines(document);
            //paso1.Text += string.Format("{0,-20}{1,-20}{2,-20}{3,-20}{4,-18}{5,-20}","Numero de linea", "Formato","Cp","Instruccion","Modo de Direccionamiento","Codigo Objeto") + Environment.NewLine;
            parser.probando.Add("");

           
            int nl = 0;dataGridView1.ColumnCount = 6;
             tabsimb.ColumnCount = 2;
            tabsimb.Rows.Add("Simbolo","Dirección");
            dataGridView1.Rows.Add("numlinea","formato","CP", "linea","modo dir","Codigo Objeto");
            codigoObjeto.Add("----");
            foreach (string lines in linen)
            {
                //MessageBox.Show(lines);
                string pb = "";
                
                if (numlinea < regCP.Count)
                {
                   
                   //paso1.Text += numlinea.ToString() + "\t\t"+formato[numlinea]+"\t\t"+regCP[numlinea]+ " \t"  +lines + "\t\t" + mododir[numlinea]+"\t\t"+codigoObjeto[numlinea] +Environment.NewLine;
                   dataGridView1.Rows.Add(numlinea,formato[numlinea],regCP[numlinea],lines,mododir[numlinea],codigoObjeto[numlinea]);
                    txlines.Text += numlinea+Environment.NewLine;
                    //paso1.Text += paso1.Text = string.Format("{0,-20}{1,-20}{2,-20}{3,-20}", formato[numlinea], regCP[numlinea],lines+mododir[numlinea])+Environment.NewLine;
                    numlinea++;
                }

                

            }
            dataGridView1.Rows.Add("Tamaño de programa :", parser.conthex[parser.conthex.Count-1]);
            //MessageBox.Show("Could be");
            if (erroresP1.Count > 0)
            {
               
                foreach (string i in erroresP1)
                {
                    Errores.Text += i + Environment.NewLine;
                }
                
                try
                {
                    // Create the file, or overwrite if the file exists.
                    // MessageBox.Show(docerr);
                  
                   
                }

                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                Errores.Text += "No se encontraron errores";
            }
            using (FileStream fs = File.Create(docerr))
            {
                StreamWriter writer = new StreamWriter(fs);
                // Add some information to the file.
                foreach (string b in parser.errores)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(b + Environment.NewLine);
                    fs.Write(info, 0, info.Length);

                }

            }
            string ttl = "";
            // Open the stream and read it back.
            using (StreamReader sr = File.OpenText(path))
            {
                
                string s = "";
                 cadenatemporal = "";
                int mr = 0;
                string nlz = "";
                tamreg = 0;
                continuar = 0;
                int mlz=0;
                int contl = 0;
                int lineaend = 0;
                contadorRegs = 0;
                string T = "";
                List<string> instruc = new List<string>();
                while ((s = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(s);
                    string[] c = s.Split(' ');
                    instruc.Clear();
                    foreach (string m in c)
                    {

                        //MessageBox.Show(m+m.Length);
                        if (!m.Contains(' '))
                        {
                            if (m.Length < 10 && m.Length > 0)
                            {
                                // MessageBox.Show(m+" ,"+m.Length);
                                instruc.Add(m);
                            }


                        }
                    }
                    int bande = 0;
                    
                    
                    string diri = "";
                    foreach (string subcadena in instruc)
                    {
                        if (subcadena.Contains("RESB") || subcadena.Contains("RESW"))
                        {
                            bande = 1;
                        }
                        if (subcadena.Contains("END")){
                            if (instruc.Count == 1)
                            {
                                int mb = 0;
                                bool End=false;
                                foreach(string nr in codigoObjeto)
                                {
                                    if (nr != "----")
                                    {
                                      
                                        switch (regCP[mb].Length)
                                        {
                                            case 1:
                                                Registros.Add("E" + "00000" + regCP[mb]);
                                                break;
                                            case 2:
                                                Registros.Add("E" + "0000" + regCP[mb]);
                                                break;
                                            case 3:
                                                Registros.Add("E" + "000" + regCP[mb]);
                                                break;
                                        }
                                        
                                        
                                        break;
                                    }
                                    mb++;
                                }
                            }
                            if(instruc.Count == 2)
                            {
                                string ini=buscaTabsim(instruc[1],tabsim,dirtabsim);

                                switch (ini.Length)
                                {
                                    case 1:
                                        Registros.Add("E" + "00000" + ini);
                                        break;
                                    case 2:
                                        Registros.Add("E" + "0000" + ini);
                                        break;
                                    case 3:
                                        Registros.Add("E" + "000" + ini);
                                        break;
                                }
                            }
                            lineaend = contl;
                            
                        }
                        
                    }
                    contl++;
                    if (bande==0)
                        {
                                double sn;
                                if (codigoObjeto[mr].Contains('*'))
                                {
                                    string[] quitaaterisco = codigoObjeto[mr].Split('*');
                                     sn = quitaaterisco[0].Length / 2;
                                    //MessageBox.Show("ast");
                                }
                            
                                else
                                {
                                    sn = codigoObjeto[mr].Length / 2;
                                }
                            
                            
                            if (tamreg < 30)
                            {
                                if (codigoObjeto[mr] != "----")
                                {
                                tamreg += (int)Math.Floor(sn);
                                if (codigoObjeto[mr].Contains('*'))
                                {
                                    string[] quitaaterisco = codigoObjeto[mr].Split('*');
                                    cadenatemporal += quitaaterisco[0];
                                }
                                else
                                {
                                    cadenatemporal += codigoObjeto[mr];
                                }

                                
                                }
                                
                            }
                            else
                            {
                            if (regCP[continuar].Contains('*'))
                            {
                                string[] t = regCP[continuar].Split('*');
                                diri = convierte6(t[0]);
                            }
                            else
                            {
                                diri = convierte6(regCP[continuar]);
                            }
                                
                            string twobytes= tamreg.ToString("X");
                            if (twobytes.Length == 1)
                            {
                                twobytes = "0" + tamreg.ToString();
                            }
                                nlz ="T"+diri + twobytes + cadenatemporal;
                                Registros.Add(nlz);
                                contadorRegs++;
                                continuar = mr;
                                tamreg = 0;
                                cadenatemporal = "";
                               
                            }

                        }
                        else
                        {
                            string o=convierte6(regCP[continuar]);
                        if (tamreg != 0)
                        {
                             diri = convierte6(regCP[continuar]);
                            string twobytes = tamreg.ToString("X");
                            if (twobytes.Length == 1)
                            {
                                twobytes = "0" + tamreg.ToString();
                            }
                            nlz = "T" + o + twobytes + cadenatemporal;
                            Registros.Add(nlz);
                            contadorRegs++;
                            continuar = mr;
                            tamreg = 0;
                            bande = 0;
                            cadenatemporal = "";
                        }
                        }
                    
                   

                    if (mr == 0)
                    {
                        string n = instruc[0];
                        
                        if (n.Length < 6)
                        {
                            ttl = n;
                        }
                        else
                        {
                            //MessageBox.Show(n.Length.ToString());
                            ttl = n.Substring(n.Length - 9, 6);
                            //ttl = n[0] + n[1] + n[2] + n[3] + n[4] + n[5] + n[6] ;
                        }
                        string cm = "";
                        switch (regCP[0].Length)
                        {
                            case 1:
                                cm = "00000" + regCP[0];
                                break;
                            case 2:
                                cm = "0000" + regCP[0];
                                break;
                        }
                        string cd = "";
                        switch (regCP[regCP.Count - 1].Length)
                        {
                            case 1:
                                cd = "00000" + regCP[regCP.Count - 1];
                                break;
                            case 2:
                                cd = "0000" + regCP[regCP.Count - 1];
                                break;
                            case 4:
                                cd = "00" + regCP[regCP.Count - 1];
                                break;
                        }
                        tt = "H" + ttl + cm + cd;
                    }
                    mr++;

                    //if(instruc.Count)

                }
                //MessageBox.Show(tt);
                Registros.Add(tt);
            }
            string ox = convierte6(regCP[continuar]);
            if (tamreg != 0)
            {
                string diri = convierte6(regCP[continuar]);
                string twobytes = tamreg.ToString("X");
                if (twobytes.Length == 1)
                {
                    twobytes = "0" + tamreg.ToString();
                }
                string nlzy = "T" + ox + twobytes + cadenatemporal;
                Registros.Add(nlzy);

            }
            
            int sd = 0;
            foreach(string a in codigoObjeto)
            {
                
                if (a.Contains("*"))
                {
                    int ml = cpint[sd] + 1;
                    string aux = ml.ToString("X");
                    if (aux.Length == 2)
                    {
                        aux = "0000" + aux;
                    }
                    if (aux.Length == 1)
                    {
                        aux = "00000" + aux;
                    }
                    string m = "M" +aux+"05+"+ttl;
                    Registros.Add(m);
                }
                sd++;
            }
          

            foreach (string s in Registros)
            {
                this.Registros.Text += s+ Environment.NewLine;
            }
            string pattern = "T";

            int indexOf = 0;

            while (indexOf != -1)
            {
                indexOf = this.Registros.Text.IndexOf(pattern, indexOf);
                if (indexOf != -1)
                {
                    this.Registros.Select(indexOf, pattern.Length);
                    this.Registros.SelectionColor = Color.Red;
                    indexOf += pattern.Length;
                }
            }
          
            for (int i = 0; i < parser.tabsimb.Count; i++)
            {
              
                tabsimb.Rows.Add(parser.tabsimb[i],parser.dirTabimHex[i]);
            }
            counter++;
            using (FileStream fs = File.Create(objname))
            {
                StreamWriter writer = new StreamWriter(fs);
                // Add some information to the file.
                foreach (string b in Registros)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(b + Environment.NewLine);
                    fs.Write(info, 0, info.Length);

                }

            }
        }
        public string convierte6(string m)
        {
            string a="";
            switch (m.Length)
            {
                case 1:
                    a = "00000" + m;
                    break;
                case 2:
                    a = "0000" + m;
                    break;
                case 3:
                    a = "000" + m;
                    break;
                case 4:
                    a = "00" + m;
                    break;
            }
            return a;
        }

        public string obtenConstante(string c)
        {
            string[] val = new string[1];
            if (c.Contains("H"))
            {
                val = c.Split('H');

                /* ax = Convert.ToInt32(val[0], 16);
                 ax = ax * 3;
                 contProg.Add(cp);
                 conthex.Add(cp.ToString("X"));
                 cp = cp + ax;*/
            }
            else
            {
                val[0] = c;
            }
            return val[0];
        }
        public string binTohex(string bin)
        {
            string dato = "";
            switch (bin)
            {
                case "0000":
                    bin = "0";
                    break;
                case "0001":
                    bin = "1";
                    break;
                case "0010":
                    bin = "2";
                    break;
                case "0011":
                    bin = "3";
                    break;
                case "0100":
                    bin = "4";
                    break;
                case "0101":
                    bin = "5";
                    break;
                case "0110":
                    bin = "6";
                    break;
                case "0111":
                    bin = "7";
                    break;
                case "1000":
                    bin = "8";
                    break;
                case "1001":
                    bin = "9";
                    break;
                case "1010":
                    bin = "A";
                    break;
                case "1011":
                    bin = "B";
                    break;
                case "1100":
                    bin = "C";
                    break;
                case "1101":
                    bin = "D";
                    break;
                case "1110":
                    bin = "E";
                    break;
                case "1111":
                    bin = "F";
                    break;

            }
            return bin;
        }
        public string hexToBin(char hex)
        {
            string bin = "";
            switch (hex)
            {

                case '0':
                    bin = "00";
                    break;
                case '8':
                    bin = "10";
                    break;
                case '4':
                    bin = "01";
                    break;
                case 'C':
                    bin = "11";
                    break;

            }
            return bin;
        }
        public string calculaIndirecto()
        {
            string codob = "";

            return codob;
        }
        public string buscaTabsim(string etiqueta,List<string> tabsimb,List<string>dirTabimHex)
        {
            string TA = "";
            for (int i = 0; i < tabsimb.Count; i++)
            {
                if (etiqueta == tabsimb[i])
                {
                    TA = dirTabimHex[i];
                }
            }
            return TA;
        }
        public string calculaSimple(string[] linea)
        {
            string codob="";

            return codob;
        }

        public string calculaInmediato(string[] linea)
        {
            string codob = "";
            if (linea[1].Contains('+'))
            {
                MessageBox.Show("Inmediato extendido");
            }
            return codob;
        }

        public string checaNemonico(string dato)
        {
            int m = 0;
            for (int i = 0; i < nemonicos.Length; i++)
            {
                if (nemonicos[i] == dato)
                {
                    m = i;

                }

            }
            if (m != 0)
            {
                return CodOp[m];
            }
            else
            {
                return "Error";
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cargarOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            String s = richTextBox1.Text;
          
            File.WriteAllText(path, s);
            try
            {
                var sr = new StreamReader(path);
               

                
                SetText(richTextBox1,sr.ReadToEnd());
                
                var contenido = File.ReadAllText(document);
            }
            catch (SecurityException ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                $"Details:\n\n{ex.StackTrace}");
            }
        }

        private void abrirToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            doc = new OpenFileDialog();
            Errores.Clear();
            doc.Filter = "txt files (*.xe)|*.xe|All files (*.*)|*.*";
            if (doc.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(doc.FileName);
                    document = doc.FileName;
                    docerr = document;

                    path = docerr;
                    extension = Path.GetExtension(docerr);
                    docerr = Path.ChangeExtension(docerr, ".err");
                    objname = Path.ChangeExtension(docerr, ".obj");
                    objname1 = Path.GetDirectoryName(path);
                    string temp = Path.GetFileNameWithoutExtension(path);
                    objname1 = objname1 +"\\"+temp +"1.obj";
                    
                   
                    SetText(richTextBox1,sr.ReadToEnd());
                   
                    var contenido = File.ReadAllText(document);
                    sr.Close();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
           
        }

        private void Regs_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog sf1 = new SaveFileDialog();
            if (sf1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Stream s = File.Open(sf1.FileName + ".xe", FileMode.CreateNew))
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        sw.Write(richTextBox1.Text);
                    }
                }
                catch { MessageBox.Show("ARCHIVO EXISTENTE- INTENTA CON OTRO NOMBRE"); }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
      
            this.Registros.Clear();
            dataGridView1.Rows.Clear();

            Errores.Clear();
            tabsimb.Rows.Clear();
            
            int counter = 0;
            string[] nn;
            List<string> words = new List<string>();
            //string a;



            //var contenido = File.ReadAllText(document);
            var contenido = richTextBox1.Text;
            GrammarSICXELexer lex = new GrammarSICXELexer(new AntlrInputStream(contenido));
            CommonTokenStream tokens = new CommonTokenStream(lex);
            GrammarSICXEParser parser = new GrammarSICXEParser(tokens);
            List<string> tabsim = new List<string>();
            List<string> tabBloques = new List<string>();
            List<string> formato = new List<string>();
            List<string> regCP = new List<string>();
            List<string> mododir = new List<string>();
            List<string> codigoObjeto = new List<string>();
            List<string> dirtabsim = new List<string>();
            List<string> Registros = new List<string>();
            string tt = "";
            int continuar = 0;
            int tamreg = 0;
            int contadorRegs = 0;
            string cadenatemporal = "";
            List<int> cpint = new List<int>();
            parser.paso = 1;
            parser.programa();
            tabsim = parser.tabsimb;
            tabBloques = parser.numBloque;
            cpint = parser.contProg;
            regCP = parser.conthex;
            mododir = parser.mododir;
            dirtabsim = parser.dirTabimHex;
            formato = parser.format;
            erroresP1 = parser.errores;
            GrammarSICXELexer lexi = new GrammarSICXELexer(new AntlrInputStream(contenido));
            CommonTokenStream tokens2 = new CommonTokenStream(lexi);
            GrammarSICXEParser parser2 = new GrammarSICXEParser(tokens2);
            parser2.paso = 2;
            parser2.tabsimb = tabsim;
            parser2.numBloque = tabBloques;
            parser2.dirTabimHex = dirtabsim;
            parser2.conthex = regCP;
            parser2.programa();

            codigoObjeto = parser2.probando;
            codigoObjeto.Add("----");
            int numlinea = 0;
            IEnumerable<string> linen = File.ReadLines(document);
            //paso1.Text += string.Format("{0,-20}{1,-20}{2,-20}{3,-20}{4,-18}{5,-20}","Numero de linea", "Formato","Cp","Instruccion","Modo de Direccionamiento","Codigo Objeto") + Environment.NewLine;
            parser.probando.Add("");


            int nl = 0; dataGridView1.ColumnCount = 6;
            tabsimb.ColumnCount = 5;
            //tabladeBloques.ColumnCount = 4;
            tabsimb.Rows.Add("Simbolo", "Dirección","Tipo","Numero de Bloque", "Referencia externa");
            
            dataGridView1.Rows.Add("numlinea", "formato", "CP", "linea", "modo dir", "Codigo Objeto");
            codigoObjeto.Add("----");
            foreach (string lines in linen)
            {
                //MessageBox.Show(lines);
                string pb = "";

                if (numlinea < regCP.Count)
                {

                    //paso1.Text += numlinea.ToString() + "\t\t"+formato[numlinea]+"\t\t"+regCP[numlinea]+ " \t"  +lines + "\t\t" + mododir[numlinea]+"\t\t"+codigoObjeto[numlinea] +Environment.NewLine;
                    dataGridView1.Rows.Add(numlinea, formato[numlinea], regCP[numlinea], lines, mododir[numlinea], codigoObjeto[numlinea]);
                    txlines.Text += numlinea + Environment.NewLine;
                    //paso1.Text += paso1.Text = string.Format("{0,-20}{1,-20}{2,-20}{3,-20}", formato[numlinea], regCP[numlinea],lines+mododir[numlinea])+Environment.NewLine;
                    numlinea++;
                }



            }
            
            dataGridView1.Rows.Add("Tamaño de programa :", parser.conthex[parser.conthex.Count - 1]);
            //MessageBox.Show("Could be");
            if (erroresP1.Count > 0)
            {

                foreach (string i in erroresP1)
                {
                    Errores.Text += i + Environment.NewLine;
                }

                try
                {
                    // Create the file, or overwrite if the file exists.
                    // MessageBox.Show(docerr);


                }

                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                Errores.Text += "No se encontraron errores";
            }
            using (FileStream fs = File.Create(docerr))
            {
                StreamWriter writer = new StreamWriter(fs);
                // Add some information to the file.
                foreach (string b in parser.errores)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(b + Environment.NewLine);
                    fs.Write(info, 0, info.Length);

                }

            }
            string ttl = "";
          
            // Open the stream and read it back.
            using (StreamReader sr = File.OpenText(path))
            {

                string s = "";
                cadenatemporal = "";
                int mr = 0;
                string nlz = "";
                tamreg = 0;
                continuar = 0;
                int mlz = 0;
                int contl = 0;
                int lineaend = 0;
                contadorRegs = 0;
                string T = "";
                List<string> instruc = new List<string>();
                codigoObjeto.Add("");
                while ((s = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(s);
                    string[] c = s.Split(' ');
                    instruc.Clear();
                    foreach (string m in c)
                    {

                        //MessageBox.Show(m+m.Length);
                        if (!m.Contains(' '))
                        {
                            if (m.Length < 10 && m.Length > 0)
                            {
                                // MessageBox.Show(m+" ,"+m.Length);
                                instruc.Add(m);
                            }


                        }
                    }
                    int bande = 0;


                    string diri = "";
                    foreach (string subcadena in instruc)
                    {
                        if (subcadena.Contains("RESB") || subcadena.Contains("RESW"))
                        {
                            bande = 1;
                        }
                        if (subcadena.Contains("END"))
                        {
                            if (instruc.Count == 1)
                            {
                                int mb = 0;
                                bool End = false;
                                foreach (string nr in codigoObjeto)
                                {
                                    if (nr != "----")
                                    {

                                        switch (regCP[mb].Length)
                                        {
                                            case 1:
                                                Registros.Add("E" + "00000" + regCP[mb]);
                                                break;
                                            case 2:
                                                Registros.Add("E" + "0000" + regCP[mb]);
                                                break;
                                            case 3:
                                                Registros.Add("E" + "000" + regCP[mb]);
                                                break;
                                        }


                                        break;
                                    }
                                    mb++;
                                }
                            }
                            if (instruc.Count == 2)
                            {
                                string ini = buscaTabsim(instruc[1], tabsim, dirtabsim);

                                switch (ini.Length)
                                {
                                    case 1:
                                        Registros.Add("E" + "00000" + ini);
                                        break;
                                    case 2:
                                        Registros.Add("E" + "0000" + ini);
                                        break;
                                    case 3:
                                        Registros.Add("E" + "000" + ini);
                                        break;
                                }
                            }
                            lineaend = contl;

                        }

                    }
                    contl++;
                    if (bande == 0)
                    {
                        double sn;
                        if (codigoObjeto[mr].Contains('*'))
                        {
                            string[] quitaaterisco = codigoObjeto[mr].Split('*');
                            sn = quitaaterisco[0].Length / 2;
                            //MessageBox.Show("ast");
                        }

                        else
                        {
                            sn = codigoObjeto[mr].Length / 2;
                        }


                        if (tamreg < 30)
                        {
                            if (codigoObjeto[mr] != "----")
                            {
                                tamreg += (int)Math.Floor(sn);
                                if (codigoObjeto[mr].Contains('*'))
                                {
                                    string[] quitaaterisco = codigoObjeto[mr].Split('*');
                                    cadenatemporal += quitaaterisco[0];
                                }
                                else
                                {
                                    cadenatemporal += codigoObjeto[mr];
                                }


                            }

                        }
                        else
                        {
                            if (regCP[continuar].Contains('*'))
                            {
                                string[] t = regCP[continuar].Split('*');
                                diri = convierte6(t[0]);
                            }
                            else
                            {
                                diri = convierte6(regCP[continuar]);
                            }

                            string twobytes = tamreg.ToString("X");
                            if (twobytes.Length == 1)
                            {
                                twobytes = "0" + tamreg.ToString();
                            }
                            nlz = "T" + diri + twobytes + cadenatemporal;
                            Registros.Add(nlz);
                            contadorRegs++;
                            continuar = mr;
                            tamreg = 0;
                            cadenatemporal = "";

                        }

                    }
                    else
                    {
                        string o = convierte6(regCP[continuar]);
                        if (tamreg != 0)
                        {
                            diri = convierte6(regCP[continuar]);
                            string twobytes = tamreg.ToString("X");
                            if (twobytes.Length == 1)
                            {
                                twobytes = "0" + tamreg.ToString();
                            }
                            nlz = "T" + o + twobytes + cadenatemporal;
                            Registros.Add(nlz);
                            contadorRegs++;
                            continuar = mr;
                            tamreg = 0;
                            bande = 0;
                            cadenatemporal = "";
                        }
                    }



                    if (mr == 0)
                    {
                        string n = instruc[0];

                        if (n.Length < 6)
                        {
                            ttl = n;
                        }
                        else
                        {
                            //MessageBox.Show(n.Length.ToString());
                            ttl = n.Substring(n.Length - 9, 6);
                            //ttl = n[0] + n[1] + n[2] + n[3] + n[4] + n[5] + n[6] ;
                        }
                        string cm = "";
                        switch (regCP[0].Length)
                        {
                            case 1:
                                cm = "00000" + regCP[0];
                                break;
                            case 2:
                                cm = "0000" + regCP[0];
                                break;
                        }
                        string cd = "";
                        switch (regCP[regCP.Count - 1].Length)
                        {
                            case 1:
                                cd = "00000" + regCP[regCP.Count - 1];
                                break;
                            case 2:
                                cd = "0000" + regCP[regCP.Count - 1];
                                break;
                            case 4:
                                cd = "00" + regCP[regCP.Count - 1];
                                break;
                        }
                        tt = "H" + ttl + cm + cd;
                    }
                    mr++;

                    //if(instruc.Count)

                }
                //MessageBox.Show(tt);
                Registros.Add(tt);
            }
            string ox = convierte6(regCP[continuar]);
            if (tamreg != 0)
            {
                string diri = convierte6(regCP[continuar]);
                string twobytes = tamreg.ToString("X");
                if (twobytes.Length == 1)
                {
                    twobytes = "0" + tamreg.ToString();
                }
                string nlzy = "T" + ox + twobytes + cadenatemporal;
                Registros.Add(nlzy);

            }

            int sd = 0;
            foreach (string a in codigoObjeto)
            {

                if (a.Contains("*"))
                {
                    string aux = regCP[sd] + 1;
                    //string aux = ml.ToString("X");
                    if (aux.Length == 2)
                    {
                        aux = "0000" + aux;
                    }
                    if (aux.Length == 1)
                    {
                        aux = "00000" + aux;
                    }
                    string m = "M" + aux + "05+" + ttl;
                    Registros.Add(m);
                }
                sd++;
            }


            foreach (string s in Registros)
            {
                this.Registros.Text += s + Environment.NewLine;
            }
            string pattern = "T";

            int indexOf = 0;

            while (indexOf != -1)
            {
                indexOf = this.Registros.Text.IndexOf(pattern, indexOf);
                if (indexOf != -1)
                {
                    this.Registros.Select(indexOf, pattern.Length);
                    this.Registros.SelectionColor = Color.Red;
                    indexOf += pattern.Length;
                }
            }
            parser.tabsimb.RemoveAt(0);
            parser.dirTabimHex.RemoveAt(0);
            parser.Tipotabsim.Add("AS");
            for (int i = 0; i < parser.Tipotabsim.Count; i++)
            {

                tabsimb.Rows.Add(parser.tabsimb[i], parser.dirTabimHex[i],parser.Tipotabsim[i],0);
            }
            //MessageBox.Show("tabsimb "+parser.tabsimb.Count.ToString()+" Tipos "+parser.Tipotabsim.Count.ToString());
          
            counter++;
            using (FileStream fs = File.Create(objname))
            {
                StreamWriter writer = new StreamWriter(fs);
                // Add some information to the file.
                foreach (string b in Registros)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(b + Environment.NewLine);
                    fs.Write(info, 0, info.Length);

                }

            }
        
           
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabsimb_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        public string buscaValorTabsimb(string dato,CSECT tabsimb)
        {
            string valor = "";
            for(int i = 0; i < tabsimb.tabsimb.Count; i++)
            {
                if (tabsimb.tabsimb[i].Contains(dato))
                {
                    valor = tabsimb.dirTabimHex[i];
                }
            }
            return valor;
        }

        public string completaEtiqueta(string s)
        {
            string result = "";
            switch (s.Length)
            {
                case 1:
                    result = s + "     ";
                    break;
                case 2:
                    result = s + "    ";
                    break;
                case 3:
                    result = s + "   ";
                    break;
                case 4:
                    result = s + "  ";
                    break;
                case 5:
                    result = s + " ";
                    break;
                case 6:
                    result = s;
                    break;
                    
            }
            return result;
        }

        public string completaValor(string s)
        {
            string result = "";
            switch (s.Length)
            {
                case 1:
                    result = "00000"+s;
                    break;
                case 2:
                    result = "0000"+s;
                    break;
                case 3:
                    result = "000"+s;
                    break;
                case 5:
                    result ="0"+s;
                    break;
                case 6:
                    result = s;
                    break;

            }
            return result;
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            var contenido = richTextBox1.Text;
            GrammarSICXELexer lex = new GrammarSICXELexer(new AntlrInputStream(contenido));
            CommonTokenStream tokens = new CommonTokenStream(lex);
            GrammarSICXEParser parser = new GrammarSICXEParser(tokens);
            parser.paso = 1;
            dataGridView1.ColumnCount = 6;
            parser.programa();
            List<string> tabsim = new List<string>();
            List<string> tabBloques = new List<string>();
            List<string> formato = new List<string>();
            List<string> regCP = new List<string>();
            List<string> mododir = new List<string>();
            List<string> codigoObjeto = new List<string>();
            List<string> dirtabsim = new List<string>();
            
            List<string> Registros = new List<string>();
            List<CSECT> seccs = new List<CSECT>();
            string tt = "";
            int continuar = 0;
            int tamreg = 0;
            int contadorRegs = 0;
            string cadenatemporal = "";
            List<int> cpint = new List<int>();
            parser.paso = 1;
            parser.programa();
            tabsim = parser.tabsimb;
            tabBloques = parser.numBloque;
            cpint = parser.contProg;
            regCP = parser.conthex;
            mododir = parser.mododir;
            seccs = parser.cs;
          
            dirtabsim = parser.dirTabimHex;
            formato = parser.format;
            erroresP1 = parser.errores;
            GrammarSICXELexer lexi = new GrammarSICXELexer(new AntlrInputStream(contenido));
            CommonTokenStream tokens2 = new CommonTokenStream(lexi);
            GrammarSICXEParser parser2 = new GrammarSICXEParser(tokens2);
            parser2.paso = 2;
            parser2.cs = seccs;
            parser2.tabsimb = tabsim;
            parser2.numBloque = tabBloques;
            parser2.dirTabimHex = dirtabsim;
            parser2.conthex = regCP;
            parser2.programa();
            //MessageBox.Show(parser2.cs.Count.ToString()+" - "+parser2.cs[0].probando.Count.ToString()+" - "+parser2.cs[1].tabsimb.Count);
            tabsimb.ColumnCount = 5;
            tabsimb.Rows.Add("Simbolo", "Dirección", "Tipo", "Numero de Bloque", "Referencia externa");
            for(int di = 0; di < parser.cs[0].tabsimb.Count; di++)
            {
                simbolosDPrograma.Add(parser.cs[0].tabsimb[di], parser.cs[0].dirTabimHex[di]);
            }
            for (int di = 0; di < parser.cs[1].tabsimb.Count; di++)
            {
                simbolosDSegundaSeccion.Add(parser.cs[1].tabsimb[di], parser.cs[1].dirTabimHex[di]);
            }
            for (int i = 0; i < parser.cs.Count; i++)
            {
               
              
                for (int m = 0; m < parser.cs[i].tabsimb.Count; m++)
                {

                    tabsimb.Rows.Add(parser.cs[i].tabsimb[m], parser.cs[i].dirTabimHex[m],parser.cs[i].Tipotabsim[m], 0,parser.cs[i].referenciaExterna[m]);
                }
                tabsimb.Rows.Add("","","","");
            }
            if (parser.errores.Count > 0)
            {
                foreach (string es in parser.errores)
                {
                    Errores.Text += es + Environment.NewLine;
                }
            }
            else
            {
                Errores.Text += "No se encontraron errores";
            }
            using (FileStream fs = File.Create(docerr))
            {
                StreamWriter writer = new StreamWriter(fs);
                // Add some information to the file.
                foreach (string b in parser.errores)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(b + Environment.NewLine);
                    fs.Write(info, 0, info.Length);

                }

            }

            int numlinea = 0;
            int numsec = 0;
            IEnumerable<string> linen = File.ReadLines(document);
            dataGridView1.Rows.Add("numlinea", "formato", "CP", "linea", "modo dir", "Codigo Objeto");
            foreach (string lines in linen)
            {
                //MessageBox.Show(lines);
                string pb = "";

                

                    //paso1.Text += numlinea.ToString() + "\t\t"+formato[numlinea]+"\t\t"+regCP[numlinea]+ " \t"  +lines + "\t\t" + mododir[numlinea]+"\t\t"+codigoObjeto[numlinea] +Environment.NewLine;
                    string[] ln = lines.Split(' ');
                    if (ln[1].Equals("CSECT"))
                    {
                   
                        numsec++;
                        dataGridView1.Rows.Add("", "", "", "");
                    numlinea = 0;
                    }
                if (numlinea < parser.cs[numsec].conthex.Count)
                {
                    dataGridView1.Rows.Add(numlinea, parser.cs[numsec].format[numlinea], parser.cs[numsec].conthex[numlinea], lines, parser.cs[numsec].mododir[numlinea], parser.cs[numsec].probando[numlinea]);
                    txlines.Text += numlinea + Environment.NewLine;
                    //paso1.Text += paso1.Text = string.Format("{0,-20}{1,-20}{2,-20}{3,-20}", formato[numlinea], regCP[numlinea],lines+mododir[numlinea])+Environment.NewLine;
                }
                    numlinea++;

            }
            string ttl = "";
            int numSec = 0;
            using (StreamReader sr = File.OpenText(path))
            {

                string s = "";
                cadenatemporal = "";
                int mr = 0;
                string nlz = "";
                tamreg = 0;
                continuar = 0;
                int mlz = 0;
                int contl = 0;
                int lineaend = 0;
                contadorRegs = 0;
                string T = "";
                List<string> instruc = new List<string>();
                List<string> refer = new List<string>();
                codigoObjeto.Add("");
                List<string> listaRegs = new List<string>();
                int numeroSeccion = 0;
                
                while ((s = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(s);
                    string[] c = s.Split(' ');


                    foreach (string d in c)
                    {
                        if (d.Contains("CSECT"))
                        {
                            numeroSeccion++;
                        }
                        if (d.Contains("EXTREF"))
                        {
                            string referencia = "";
                            string[] dn = c[1].Split(',');
                            foreach (string smn in dn)
                            {
                                string res = "";
                                res = completaEtiqueta(smn);
                                refer.Add(res);
                            }
                            foreach (string abr in refer)
                            {
                                referencia += abr;
                                // MessageBox.Show(abr);

                            }
                            Registros.Add("R"+referencia);
                            refer.Clear();
                            break;
                        }
                        if (d.Contains("EXTDEF"))
                        {
                            string referencia = "";
                            string[] dn = c[1].Split(',');
                            foreach (string smn in dn)
                            {
                                string valor="";
                                string etiq = "";
                                string val = "";
                                valor=buscaValorTabsimb(smn,parser.cs[numeroSeccion]);
                                val = completaValor(valor);
                                etiq = completaEtiqueta(smn);
                                refer.Add(etiq+val);
                            }
                            foreach (string abr in refer)
                            {
                                referencia += abr;
                                // MessageBox.Show(abr);

                            }
                            Registros.Add("D" + referencia);
                            refer.Clear();
                            break;
                        }

                    }

                    instruc.Clear();
                    foreach (string m in c)
                    {

                        //MessageBox.Show(m+m.Length);
                        if (!m.Contains(' '))
                        {
                            if (m.Length < 10 && m.Length > 0)
                            {
                                // MessageBox.Show(m+" ,"+m.Length);
                                instruc.Add(m);
                            }


                        }
                    }
                    int bande = 0;
                    int butt = 0;
                    
                    string diri = "";
                    
                    foreach (string subcadena in instruc)
                    {
                        if (subcadena.Contains("START"))
                        {
                            String lenf = parser.cs[1].conthex[parser.cs[1].conthex.Count - 1];
                            switch(parser.cs[0].conthex[parser.cs[0].conthex.Count - 1].Length)
                            {
                                case 3:
                                    lenf = "000" + lenf;
                                    break;
                                case 4:
                                    lenf = "00" + lenf;
                                    break;
                                case 5:
                                    lenf = "0" + lenf;
                                    break;
                            }
                            sec1 = instruc[0];
                            nombre = instruc[0];
                            Registros.Add("H"+completaEtiqueta(instruc[0])+"000000"+lenf);
                        }
                       
                        if (subcadena.Contains("RESB") || subcadena.Contains("RESW") || subcadena.Contains("CSECT"))
                        {
                            if (subcadena.Contains("CSECT"))
                            {
                                numSec++;
                                continuar = 0;
                                mr = 0;
                                tamreg = 0;
                                cadenatemporal = "";
                                string lenfsec = parser.cs[numeroSeccion].conthex[parser.cs[numeroSeccion].conthex.Count - 2];
                                switch (lenfsec.Length)
                                {
                                    case 3:
                                        lenfsec = "000" + lenfsec;
                                        break;
                                    case 4:
                                        lenfsec = "00" + lenfsec;
                                        break;
                                }
                                sec2 = instruc[0];
                                Registros.Add("H" +completaEtiqueta(instruc[0]) + "000000" +lenfsec);
                                nombre = instruc[0];
                            }
                            
                            bande = 1;
                        }

                        if (subcadena.Contains("END"))
                        {
                           // MessageBox.Show("End"+" "+instruc.Count+" "+buscaValorTabsimb(instruc[1],parser.cs[0]));
                            if (instruc.Count == 1)
                            {
                                int mb = 0;
                                bool End = false;
                                foreach (string nr in parser.cs[numSec].probando)
                                {
                                    if (nr != "----")
                                    {

                                        switch (parser.cs[numSec].conthex[mb].Length)
                                        {
                                            case 1:
                                                Registros.Add("E" + "00000" + parser.cs[numSec].conthex[mb]);
                                                break;
                                            case 2:
                                                Registros.Add("E" + "0000" + parser.cs[numSec].conthex[mb]);
                                                break;
                                            case 3:
                                                Registros.Add("E" + "000" + parser.cs[numSec].conthex[mb]);
                                                break;
                                        }


                                        break;
                                    }
                                    mb++;
                                }
                            }
                            if (instruc.Count == 2)
                            {
                                string ini = buscaValorTabsimb(instruc[1], parser.cs[0]);

                                switch (ini.Length)
                                {
                                    case 1:
                                        Registros.Add("E" + "00000" + ini);
                                        break;
                                    case 2:
                                        Registros.Add("E" + "0000" + ini);
                                        break;
                                    case 3:
                                        Registros.Add("E" + "000" + ini);
                                        break;
                                }
                            }
                            lineaend = contl;

                        }
                    }
                    contl++;
                    if (bande == 0)
                    {
                        double sn;
                        if (mr < parser.cs[numSec].probando.Count)
                        {
                            if (parser.cs[numSec].probando[mr].Contains('*'))
                            {
                                string[] quitaaterisco = parser.cs[numSec].probando[mr].Split('*');
                                sn = quitaaterisco[0].Length / 2;
                                //MessageBox.Show("ast");
                            }

                            else
                            {
                                sn = parser.cs[numSec].probando[mr].Length / 2;
                            }


                            if (tamreg < 30)
                            {
                                if (parser.cs[numSec].probando[mr] != "----")
                                {
                                    tamreg += (int)Math.Floor(sn);
                                    if (parser.cs[numSec].probando[mr].Contains('*'))
                                    {
                                        string[] quitaaterisco = parser.cs[numSec].probando[mr].Split('*');
                                        cadenatemporal += quitaaterisco[0];
                                    }
                                    else
                                    {
                                        cadenatemporal += parser.cs[numSec].probando[mr];
                                    }


                                }

                            }
                            else
                            {
                                if (parser.cs[numSec].conthex[continuar].Contains('*'))
                                {
                                    string[] t = parser.cs[numSec].conthex[continuar].Split('*');
                                    diri = convierte6(t[0]);
                                }
                                else
                                {
                                    diri = convierte6(parser.cs[numSec].conthex[continuar]);
                                }

                                string twobytes = tamreg.ToString("X");
                                if (twobytes.Length == 1)
                                {
                                    twobytes = "0" + tamreg.ToString();
                                }
                                nlz = "T" + diri + twobytes + cadenatemporal;
                                Registros.Add(nlz);
                                contadorRegs++;
                                continuar = mr;
                                tamreg = 0;
                                cadenatemporal = "";

                            }
                        }
                    }
                    else
                    {
                        string o = convierte6(parser.cs[numSec].conthex[continuar]);
                        if (tamreg != 0)
                        {
                            diri = convierte6(parser.cs[numSec].conthex[continuar]);
                            string twobytes = tamreg.ToString("X");
                            if (twobytes.Length == 1)
                            {
                                twobytes = "0" + tamreg.ToString();
                            }
                            nlz = "T" + o + twobytes + cadenatemporal;
                            Registros.Add(nlz);
                            contadorRegs++;
                            continuar = mr;
                            tamreg = 0;
                            bande = 0;
                            cadenatemporal = "";
                        }
                    }



                    if (mr == 0)
                    {
                        string n = instruc[0];

                        if (n.Length < 6)
                        {
                            ttl = n;
                        }
                        else
                        {
                            //MessageBox.Show(n.Length.ToString());
                            ttl = n.Substring(n.Length - 9, 6);
                            //ttl = n[0] + n[1] + n[2] + n[3] + n[4] + n[5] + n[6] ;
                        }
                        string cm = "";
                        switch (parser.cs[numSec].conthex[0].Length)
                        {
                            case 1:
                                cm = "00000" + parser.cs[numSec].conthex[0];
                                break;
                            case 2:
                                cm = "0000" + parser.cs[numSec].conthex[0];
                                break;
                        }
                        string cd = "";
                        switch (parser.cs[numSec].conthex[parser.cs[numSec].conthex.Count - 1].Length)
                        {
                            case 1:
                                cd = "00000" +parser.cs[numSec].conthex[parser.cs[numSec].conthex.Count - 1];
                                break;
                            case 2:
                                cd = "0000" + parser.cs[numSec].conthex[parser.cs[numSec].conthex.Count - 1];
                                break;
                            case 4:
                                cd = "00" + parser.cs[numSec].conthex[parser.cs[numSec].conthex.Count - 1];
                                break;
                        }
                        tt = "H" + ttl + cm + cd;
                    }
                    mr++;

                    //if(instruc.Count)

                }
                //MessageBox.Show(tt);
                Registros.Add(tt);
            }
            string ox = convierte6(parser.cs[numSec].conthex[continuar]);
            if (tamreg != 0)
            {
                string diri = convierte6(parser.cs[numSec].conthex[continuar]);
                string twobytes = tamreg.ToString("X");
                if (twobytes.Length == 1)
                {
                    twobytes = "0" + tamreg.ToString();
                }
                string nlzy = "T" + ox + twobytes + cadenatemporal;
                Registros.Add(nlzy);

            }

            int sd = 0;
            int dt = 1;
            int indic = 0;
            for (int i = 0; i < parser.cs.Count;i++) {
                foreach (string a in parser.cs[i].probando)
                {

                    if (a.Contains("*SE"))
                    {
                        //Hace falta sumarle un 1
                        string aux = (parser.cs[i].conthex[sd]) ;
                        int numeroDecimal = Convert.ToInt32(aux, 16);

                        aux = (numeroDecimal + 1).ToString("X");
                        //string aux = ml.ToString("X");
                        if (aux.Length == 2)
                        {
                            aux = "0000" + aux;
                        }
                        if (aux.Length == 1)
                        {
                            aux = "00000" + aux;
                        }
                        string m = "M" + aux + "05+" +completaEtiqueta(parser.cs[i].SimbolosWord[indic]);
                        indic++;
                        Registros.Add(m);
                    }
                    if (a.Contains("*R"))
                    {
                        //Hace falta sumarle un 1
                        string aux = (parser.cs[i].conthex[sd]);
                        int numeroDecimal = Convert.ToInt32(aux, 16);

                        aux = (numeroDecimal + 1).ToString("X");
                        //string aux = ml.ToString("X");
                        if (aux.Length == 2)
                        {
                            aux = "0000" + aux;
                        }
                        if (aux.Length == 1)
                        {
                            aux = "00000" + aux;
                        }
                        string m = "M" + aux + "05+" + completaEtiqueta(nombre);
                        indic++;
                        Registros.Add(m);
                    }
                    sd++;
                }
            }
            
          
            s1.Add(Registros[0]);
            s1.Add(Registros[1]);
            s1.Add(Registros[2]);
            s1.Add(Registros[3]);
            s1.Add(Registros[11]);
            s1.Add(Registros[12]);
            s1.Add(Registros[13]);
            s1.Add(Registros[8]);
            s2.Add(Registros[4]);
            s2.Add(Registros[5]);
            s2.Add(Registros[6]);
            s2.Add(Registros[7]);
            s2.Add(Registros[10]);
            s2.Add(parser.cs[1].SimbolosWord[0]);
            s2.Add(parser.cs[1].SimbolosWord[1]);
            s2.Add(parser.cs[1].SimbolosWord[2]);
            s2.Add("E");
            foreach (string a in s1)
            {
                this.Registros.Text += a + Environment.NewLine;
            }
           
            foreach (string a in s2)
            {
                this.Registros2.Text += a + Environment.NewLine;
            }

            using (FileStream fs = File.Create(objname))
            {
                StreamWriter writer = new StreamWriter(fs);
                // Add some information to the file.
                foreach (string b in s1)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(b + Environment.NewLine);
                    fs.Write(info, 0, info.Length);

                }

            }
            using (FileStream fs = File.Create(objname1))
            {
                StreamWriter writer = new StreamWriter(fs);
                // Add some information to the file.
                foreach (string b in s2)
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(b + Environment.NewLine);
                    fs.Write(info, 0, info.Length);

                }

            }
        }

        private void cargarOBJToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            doc = new OpenFileDialog();
            Errores.Clear();
            doc.Filter = "txt files (*.obj)|*.obj|All files (*.*)|*.*";
            if (doc.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(doc.FileName);
                    document = doc.FileName;
                    docerr = document;

                    path = docerr;
                    extension = Path.GetExtension(docerr);
                    docerr = Path.ChangeExtension(docerr, ".err");
                   // objname = Path.ChangeExtension(docerr, ".obj");
                    //objname1 = Path.GetFileNameWithoutExtension(path);
                    //objname1 = objname1 + "1.obj";
                    // MessageBox.Show(objname1);
                    //MessageBox.Show(docerr);
                    if (numeroArchivo == 0)
                    {
                        SetText(Registros, sr.ReadToEnd());
                    }
                    if (numeroArchivo == 1)
                    {
                        SetText(Registros2,sr.ReadToEnd());
                    }
                    //contenido = sr.ReadToEnd();
                    var contenido = File.ReadAllText(document);
                    sr.Close();
                    numeroArchivo++;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            
                // string inicio = dataGridArchivoIntermedio.Rows[0].Cells[1].Value.ToString();
                //string final = dataGridArchivoIntermedio.Rows[dataGridArchivoIntermedio.Rows.Count - 1].Cells[1].Value.ToString();

                //   Mapa mapa = new Mapa(inicio, final, registros, CP(), obtenerLongitud());
                //  mapa.Show();
                string[] programas = new string[2];
                programas[0] = Registros.Text;
                programas[1] = Registros2.Text;
            //string[] lineas = File.ReadAllLines(Registros.Text);
            //string[] lineas2 = File.ReadAllLines(Registros2.Text);
            //programas[0] = lineas;
            //programas[0] = Registros.Lines;
           // programas[1] = Registros2.Text;
            string[] secc1 = Registros.Lines;
            string[] secc2 = Registros2.Lines;
            
            Cargador c = new Cargador(secc1,secc2,sec1,sec2,simbolosDPrograma,simbolosDSegundaSeccion);
            c.nombreDelPrograma = sec1;
            c.nombreSegundaSeccion = sec2;
            c.Show();
           
        }
    }
}
