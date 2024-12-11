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
    public partial class FrmProductos : Form
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public FrmProductos()
        {
            InitializeComponent();
        }

        private void FrmProductos_Load(object sender, EventArgs e)
        {
            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add("PROD_ID", "ID");
            dgvProductos.Columns.Add("PROV_ID", "ID Proveedor");
            dgvProductos.Columns.Add("PROD_NOMBRE", "Nombre");
            dgvProductos.Columns.Add("PROD_DECRIPCION", "Descripción");
            dgvProductos.Columns.Add("PROD_PRECIO", "Precio");
            dgvProductos.Columns.Add("PROD_CANTIDAD", "Existencia");
            dgvProductos.Columns.Add("PROD_ESTAD", "Estado");
            dgvProductos.Rows.Clear();

            var proveedores = from p in db.Proveedor
                              select new
                              {
                                  p.PROV_ID,
                                  p.PROV_NOMBRE
                              };
            cmbProveedor.DataSource = proveedores;
            cmbProveedor.DisplayMember = "PROV_NOMBRE";
            cmbProveedor.ValueMember = "PROV_ID";

            cmbEstado.Items.Add("Disponible");
            cmbEstado.Items.Add("No disponible");

            CargarDatosExistentes();

            // Manejar el evento SelectionChanged del DataGridView
            dgvProductos.SelectionChanged += dgvProductos_SelectionChanged;
        }

        private void CargarDatosExistentes()
        {
            dgvProductos.Rows.Clear();
            var productos = from p in db.Producto
                            select new
                            {
                                p.PROD_ID,
                                p.PROV_ID,
                                p.PROD_NOMBRE,
                                p.PROD_DECRIPCION,
                                p.PROD_PRECIO,
                                p.PROD_CANTIDAD,
                                p.PROD_ESTAD
                            };

            foreach (var producto in productos)
            {
                dgvProductos.Rows.Add(
                    producto.PROD_ID,
                    producto.PROV_ID,
                    producto.PROD_NOMBRE,
                    producto.PROD_DECRIPCION,
                    producto.PROD_PRECIO,
                    producto.PROD_CANTIDAD,
                    producto.PROD_ESTAD
                );
            }
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvProductos.SelectedRows[0];

                try
                {
                    cmbProveedor.SelectedValue = Convert.ToInt32(row.Cells["PROV_ID"].Value);
                    txtNombre.Text = row.Cells["PROD_NOMBRE"].Value.ToString();
                    txtDescripcion.Text = row.Cells["PROD_DECRIPCION"].Value.ToString();
                    txtPrecio.Text = Convert.ToDouble(row.Cells["PROD_PRECIO"].Value).ToString();
                    txtCantidad.Text = Convert.ToInt32(row.Cells["PROD_CANTIDAD"].Value).ToString();
                    cmbEstado.Text = row.Cells["PROD_ESTAD"].Value.ToString();
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Error de formato: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbProveedor.SelectedItem == null || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtPrecio.Text) || string.IsNullOrEmpty(txtCantidad.Text) || cmbEstado.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            int proveedorId = Convert.ToInt32(cmbProveedor.SelectedValue);
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            double precio = Convert.ToDouble(txtPrecio.Text);
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            string estado = cmbEstado.SelectedItem.ToString();

            dgvProductos.Rows.Add(0, proveedorId, nombre, descripcion, precio, cantidad, estado);

            MessageBox.Show("Producto agregado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvProductos.SelectedRows[0];

                if (cmbProveedor.SelectedItem == null || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtPrecio.Text) || string.IsNullOrEmpty(txtCantidad.Text) || cmbEstado.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    return;
                }

                int proveedorId = Convert.ToInt32(cmbProveedor.SelectedValue);
                string nombre = txtNombre.Text;
                string descripcion = txtDescripcion.Text;
                double precio = Convert.ToDouble(txtPrecio.Text);
                int cantidad = Convert.ToInt32(txtCantidad.Text);
                string estado = cmbEstado.SelectedItem.ToString();

                row.Cells["PROV_ID"].Value = proveedorId;
                row.Cells["PROD_NOMBRE"].Value = nombre;
                row.Cells["PROD_DECRIPCION"].Value = descripcion;
                row.Cells["PROD_PRECIO"].Value = precio;
                row.Cells["PROD_CANTIDAD"].Value = cantidad;
                row.Cells["PROD_ESTAD"].Value = estado;

                MessageBox.Show("Producto modificado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione una fila para modificar.");
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                dgvProductos.Rows.RemoveAt(dgvProductos.SelectedRows[0].Index);
                MessageBox.Show("Producto eliminado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione una fila para eliminar.");
            }
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvProductos.Rows)
                {
                    if (row.IsNewRow) continue;

                    int productoId = Convert.ToInt32(row.Cells["PROD_ID"].Value);
                    var productoExistente = db.Producto.FirstOrDefault(p => p.PROD_ID == productoId);

                    if (productoExistente == null)
                    {
                        Producto nuevoProducto = new Producto
                        {
                            PROV_ID = Convert.ToInt32(row.Cells["PROV_ID"].Value),
                            PROD_NOMBRE = row.Cells["PROD_NOMBRE"].Value.ToString(),
                            PROD_DECRIPCION = row.Cells["PROD_DECRIPCION"].Value.ToString(),
                            PROD_PRECIO = Convert.ToDouble(row.Cells["PROD_PRECIO"].Value),
                            PROD_CANTIDAD = Convert.ToInt32(row.Cells["PROD_CANTIDAD"].Value),
                            PROD_ESTAD = row.Cells["PROD_ESTAD"].Value.ToString()
                        };

                        db.Producto.InsertOnSubmit(nuevoProducto);
                    }
                }

                db.SubmitChanges();
                MessageBox.Show("Productos insertados en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizar el DataGridView después de insertar los productos
                CargarDatosExistentes();
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
                foreach (DataGridViewRow row in dgvProductos.Rows)
                {
                    if (row.IsNewRow) continue;

                    int productoId = Convert.ToInt32(row.Cells["PROD_ID"].Value);
                    var producto = db.Producto.FirstOrDefault(p => p.PROD_ID == productoId);
                    if (producto != null)
                    {
                        producto.PROV_ID = Convert.ToInt32(row.Cells["PROV_ID"].Value);
                        producto.PROD_NOMBRE = row.Cells["PROD_NOMBRE"].Value.ToString();
                        producto.PROD_DECRIPCION = row.Cells["PROD_DECRIPCION"].Value.ToString();
                        producto.PROD_PRECIO = Convert.ToDouble(row.Cells["PROD_PRECIO"].Value);
                        producto.PROD_CANTIDAD = Convert.ToInt32(row.Cells["PROD_CANTIDAD"].Value);
                        producto.PROD_ESTAD = row.Cells["PROD_ESTAD"].Value.ToString();
                    }
                }

                db.SubmitChanges();
                MessageBox.Show("Productos actualizados en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                foreach (DataGridViewRow row in dgvProductos.SelectedRows)
                {
                    int productoId = Convert.ToInt32(row.Cells["PROD_ID"].Value);
                    var producto = db.Producto.FirstOrDefault(p => p.PROD_ID == productoId);
                    if (producto != null)
                    {
                        db.Producto.DeleteOnSubmit(producto);
                    }
                }

                db.SubmitChanges();
                MessageBox.Show("Productos eliminados de la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Eliminar las filas seleccionadas del DataGridView
                foreach (DataGridViewRow row in dgvProductos.SelectedRows)
                {
                    dgvProductos.Rows.Remove(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar los datos: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
