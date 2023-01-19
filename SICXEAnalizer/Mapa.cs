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
    public partial class Mapa : Form
    {
        private string cp;
        private string longitud;
        private string inicio;
        private string final;
        private List<string> registros;
        private bool termina;
       
        public Mapa(string ini, string fin, List<string> reg, string cp, string longitud)
        {
            InitializeComponent();
            inicio = ini;
            final = fin;
            registros = reg;
            this.cp = cp;
            this.longitud = longitud;
            textBox1.Text = cp;
            textBox2.Text = longitud;
            textBox1.Enabled = textBox2.Enabled = false;
            termina = false;
            llenaTablaRegistros();
        }

        private void llenaTablaRegistros()
        {

            dataGridViewMapa.Rows.Insert(0, "CP", cp);
            dataGridViewMapa.Rows.Insert(1, "A", "FFFFFF");
            dataGridViewMapa.Rows.Insert(2, "X", "FFFFFF");
            dataGridViewMapa.Rows.Insert(3, "L", "FFFFFF");
            dataGridViewMapa.Rows.Insert(4, "SW", "FFFFFF");
            dataGridViewMapa.Rows.Insert(5, "CC", "FFFFFF");
        }

        private void Mapa_Load(object sender, EventArgs e)
        {

        }
    }
}
