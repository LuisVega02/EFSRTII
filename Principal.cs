using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinal
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void btnMovimientos_Click(object sender, EventArgs e)
        {
            FrmMovimientos form = new FrmMovimientos();
            form.Show();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            FrmClientes form = new FrmClientes();
            form.Show();
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            FrmProveedores form = new FrmProveedores();
            form.Show();
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            FrmProductos form = new FrmProductos();
            form.Show();
        }

        private void Principal_Load(object sender, EventArgs e)
        {

        }
    }
}
