using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SICXEAnalizer
{

  public class CSECT
    {
        public List<string> errores = new List<string>();
        public List<string> tabsimb = new List<string>();
        public List<int> dirTabsim = new List<int>();
        public List<string> dirTabimHex = new List<string>();
        public List<string> format = new List<string>();
        public List<string> errorType = new List<string>();
        public List<int> contProg = new List<int>();
        public List<string> p1 = new List<string>();
        public List<string> probando = new List<string>();
        public List<string> mododir = new List<string>();
        public List<string> Errorespaso2 = new List<string>();
        public int simbolduplicated = 0;
        public List<string> Tipotabsim = new List<string>();
        public List<string>referenciaext = new List<string>();
        public int noEsEtiqueta = 0;
        public int paso = 0;
        public int defex = 0;
        public int expresionlarga = 0;
        public string etiquetaEQU = "";
        public int verificaExpresion = 0;
        public int directivaEQU = 0;
        public string resultadoexpresion = "";
        public int valorEtiqueta = 0;
        public int mr  = 0;
        public string cphex;
        public List<string> conthex = new List<string>();
        public List<string> codobj = new List<string>();
        public List<string> referenciaExterna = new List<string>();
        public int cp = 0;
        public int b = 0;
        public int format4 = 0;
        public int form4 = 0;
        public List<int> lineas = new List<int>();
        public string BASE = "0";
        public List<string> SimbolosWord = new List<string>();
        public string nombreSeccion = "";
    }
}
