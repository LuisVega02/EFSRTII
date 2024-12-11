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
    public partial class FrmProveedores : Form
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public FrmProveedores()
        {
            InitializeComponent();
        }

        private void FrmProveedores_Load(object sender, EventArgs e)
        {
            dgvProveedor.Columns.Clear();
            dgvProveedor.Columns.Add("PROV_ID", "ID");
            dgvProveedor.Columns.Add("PROV_NOMBRE", "Proveedor");
            dgvProveedor.Columns.Add("PROV_CONTACTO", "Contacto");
            dgvProveedor.Rows.Clear();

            // Cargar datos existentes en el DataGridView
            CargarDatosExistentes();
        }

        private void CargarDatosExistentes()
        {
            var proveedores = from prov in db.Proveedor
                              select new
                              {
                                  prov.PROV_ID,
                                  prov.PROV_NOMBRE,
                                  prov.PROV_CONTACTO
                              };

            foreach (var proveedor in proveedores)
            {
                dgvProveedor.Rows.Add(
                    proveedor.PROV_ID,
                    proveedor.PROV_NOMBRE,
                    proveedor.PROV_CONTACTO
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

            dgvProveedor.Rows.Add(
                0, // ID temporal
                txtNombre.Text,
                txtContacto.Text
            );

            MessageBox.Show("Proveedor agregado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvProveedor.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvProveedor.SelectedRows[0];

                if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtContacto.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    return;
                }

                row.Cells["PROV_NOMBRE"].Value = txtNombre.Text;
                row.Cells["PROV_CONTACTO"].Value = txtContacto.Text;

                MessageBox.Show("Proveedor modificado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor para modificar.");
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvProveedor.SelectedRows.Count > 0)
            {
                dgvProveedor.Rows.RemoveAt(dgvProveedor.SelectedRows[0].Index);
                MessageBox.Show("Proveedor eliminado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor para eliminar.");
            }
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvProveedor.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (Convert.ToInt32(row.Cells["PROV_ID"].Value) == 0)
                    {
                        Proveedor nuevoProveedor = new Proveedor
                        {
                            PROV_NOMBRE = row.Cells["PROV_NOMBRE"].Value.ToString(),
                            PROV_CONTACTO = row.Cells["PROV_CONTACTO"].Value.ToString()
                        };

                        db.Proveedor.InsertOnSubmit(nuevoProveedor);
                        db.SubmitChanges();

                        row.Cells["PROV_ID"].Value = nuevoProveedor.PROV_ID;
                    }
                }

                MessageBox.Show("Proveedores insertados satisfactoriamente en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                foreach (DataGridViewRow row in dgvProveedor.Rows)
                {
                    if (row.IsNewRow) continue;

                    int proveedorId = Convert.ToInt32(row.Cells["PROV_ID"].Value);
                    var proveedor = db.Proveedor.FirstOrDefault(p => p.PROV_ID == proveedorId);
                    if (proveedor != null)
                    {
                        proveedor.PROV_NOMBRE = row.Cells["PROV_NOMBRE"].Value.ToString();
                        proveedor.PROV_CONTACTO = row.Cells["PROV_CONTACTO"].Value.ToString();
                    }
                }

                db.SubmitChanges();
                MessageBox.Show("Proveedores actualizados satisfactoriamente en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                foreach (DataGridViewRow row in dgvProveedor.SelectedRows)
                {
                    int proveedorId = Convert.ToInt32(row.Cells["PROV_ID"].Value);
                    var proveedor = db.Proveedor.FirstOrDefault(p => p.PROV_ID == proveedorId);
                    if (proveedor != null)
                    {
                        // Eliminar registros relacionados en DetalleMovimiento
                        var detalles = db.DetalleMovimiento.Where(det => det.Producto.PROV_ID == proveedorId).ToList();
                        db.DetalleMovimiento.DeleteAllOnSubmit(detalles);

                        // Eliminar registros relacionados en Producto
                        var productos = db.Producto.Where(prod => prod.PROV_ID == proveedorId).ToList();
                        db.Producto.DeleteAllOnSubmit(productos);

                        db.Proveedor.DeleteOnSubmit(proveedor);
                    }
                }

                db.SubmitChanges();
                foreach (DataGridViewRow row in dgvProveedor.SelectedRows)
                {
                    dgvProveedor.Rows.RemoveAt(row.Index);
                }

                MessageBox.Show("Proveedores eliminados satisfactoriamente de la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar los datos: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}




