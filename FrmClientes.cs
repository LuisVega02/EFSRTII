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
    public partial class FrmClientes : Form
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public FrmClientes()
        {
            InitializeComponent();
        }

        private void Clientes_Load(object sender, EventArgs e)
        {
            dgvClientes.Columns.Clear();
            dgvClientes.Columns.Add("CLIENTE_ID", "ID");
            dgvClientes.Columns.Add("CLIENTE_NOMBRE", "Cliente");
            dgvClientes.Columns.Add("CLIENTE_CONTACTO", "Contacto");
            dgvClientes.Rows.Clear();

            // Cargar datos existentes en el DataGridView
            CargarDatosExistentes();
        }

        private void CargarDatosExistentes()
        {
            var clientes = from cli in db.Cliente
                           select new
                           {
                               cli.CLIENTE_ID,
                               cli.CLIENTE_NOMBRE,
                               cli.CLIENTE_CONTACTO
                           };

            foreach (var cliente in clientes)
            {
                dgvClientes.Rows.Add(
                    cliente.CLIENTE_ID,
                    cliente.CLIENTE_NOMBRE,
                    cliente.CLIENTE_CONTACTO
                );
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtContacto.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            dgvClientes.Rows.Add(
                0, // ID temporal
                txtNombre.Text,
                txtContacto.Text
            );

            MessageBox.Show("Cliente agregado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvClientes.SelectedRows[0];

                if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtContacto.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    return;
                }

                row.Cells["CLIENTE_NOMBRE"].Value = txtNombre.Text;
                row.Cells["CLIENTE_CONTACTO"].Value = txtContacto.Text;

                MessageBox.Show("Cliente modificado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un cliente para modificar.");
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                dgvClientes.Rows.RemoveAt(dgvClientes.SelectedRows[0].Index);
                MessageBox.Show("Cliente eliminado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un cliente para eliminar.");
            }
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvClientes.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (Convert.ToInt32(row.Cells["CLIENTE_ID"].Value) == 0)
                    {
                        Cliente nuevoCliente = new Cliente
                        {
                            CLIENTE_NOMBRE = row.Cells["CLIENTE_NOMBRE"].Value.ToString(),
                            CLIENTE_CONTACTO = row.Cells["CLIENTE_CONTACTO"].Value.ToString()
                        };

                        db.Cliente.InsertOnSubmit(nuevoCliente);
                        db.SubmitChanges();

                        row.Cells["CLIENTE_ID"].Value = nuevoCliente.CLIENTE_ID;
                    }
                }

                MessageBox.Show("Clientes insertados satisfactoriamente en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar los datos: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvClientes.Rows)
                {
                    if (row.IsNewRow) continue;

                    int clienteId = Convert.ToInt32(row.Cells["CLIENTE_ID"].Value);
                    var cliente = db.Cliente.FirstOrDefault(c => c.CLIENTE_ID == clienteId);
                    if (cliente != null)
                    {
                        cliente.CLIENTE_NOMBRE = row.Cells["CLIENTE_NOMBRE"].Value.ToString();
                        cliente.CLIENTE_CONTACTO = row.Cells["CLIENTE_CONTACTO"].Value.ToString();
                    }
                }

                db.SubmitChanges();
                MessageBox.Show("Clientes actualizados satisfactoriamente en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los datos: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvClientes.SelectedRows)
                {
                    int clienteId = Convert.ToInt32(row.Cells["CLIENTE_ID"].Value);
                    var cliente = db.Cliente.FirstOrDefault(c => c.CLIENTE_ID == clienteId);
                    if (cliente != null)
                    {
                        db.Cliente.DeleteOnSubmit(cliente);
                    }
                }

                db.SubmitChanges();
                foreach (DataGridViewRow row in dgvClientes.SelectedRows)
                {
                    dgvClientes.Rows.RemoveAt(row.Index);
                }

                MessageBox.Show("Clientes eliminados satisfactoriamente de la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar los datos: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}


