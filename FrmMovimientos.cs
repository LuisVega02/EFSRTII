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
    public partial class FrmMovimientos : Form
    {
        object objVersion;
        DataClasses1DataContext db = new DataClasses1DataContext();
        public FrmMovimientos()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void FrmMovimientos_Load(object sender, EventArgs e)
        {
            dgvMovimientos.Columns.Clear();
            dgvMovimientos.Columns.Add("CAB_ID", "ID");
            dgvMovimientos.Columns.Add("CAB_CLIENTE_ID", "Cliente");
            dgvMovimientos.Columns.Add("CAB_FECHA", "Fecha");
            dgvMovimientos.Columns.Add("CAB_TIPO", "Tipo Movimiento");
            dgvMovimientos.Columns.Add("CAB_MOTIVO", "Motivo");
            dgvMovimientos.Columns.Add("PROV_NOMBRE", "Proveedor");
            dgvMovimientos.Columns.Add("DET_PROD_ID", "Producto");
            dgvMovimientos.Columns.Add("DET_CANTIDAD", "Cantidad");
            dgvMovimientos.Columns.Add("DET_PRECIO_UNITARIO", "Precio Unitario");
            dgvMovimientos.Columns.Add("DET_SUBTOTAL", "Subtotal");
            dgvMovimientos.Rows.Clear();

            var productos = from p in db.Producto
                            select p;
            cmbProductos.DataSource = productos;
            cmbProductos.DisplayMember = "PROD_NOMBRE";
            cmbProductos.ValueMember = "PROD_ID";

            var proveedores = from p in db.Proveedor
                              select p;
            cmbProveedor.DataSource = proveedores;
            cmbProveedor.DisplayMember = "PROV_NOMBRE";
            cmbProveedor.ValueMember = "PROV_ID";

            var clientes = from c in db.Cliente
                           select c;
            cmbCliente.DataSource = clientes;
            cmbCliente.DisplayMember = "CLIENTE_NOMBRE";
            cmbCliente.ValueMember = "CLIENTE_ID";

            cmbMotivo.Items.Add("Compra");
            cmbMotivo.Items.Add("Venta");
            cmbMotivo.Items.Add("Devolución");
            cmbTipoMov.Items.Add("Entrada");
            cmbTipoMov.Items.Add("Salida");

            CargarDatosExistentes();

            // Manejar el evento SelectionChanged del DataGridView
            dgvMovimientos.SelectionChanged += dgvMovimientos_SelectionChanged;
        }

        private void CargarDatosExistentes()
        {
            var movimientos = from cab in db.CabeceraMovimiento
                              join det in db.DetalleMovimiento on cab.CAB_ID equals det.CAB_ID
                              join cli in db.Cliente on cab.CAB_CLIENTE_ID equals cli.CLIENTE_ID
                              join prov in db.Proveedor on cab.PROV_ID equals prov.PROV_ID
                              join prod in db.Producto on det.DET_PROD_ID equals prod.PROD_ID
                              select new
                              {
                                  cab.CAB_ID,
                                  ClienteID = cli.CLIENTE_ID,
                                  ClienteNombre = cli.CLIENTE_NOMBRE,
                                  cab.CAB_FECHA,
                                  cab.CAB_TIPO,
                                  cab.CAB_MOTIVO,
                                  Proveedor = prov.PROV_NOMBRE,
                                  ProductoID = prod.PROD_ID,
                                  ProductoNombre = prod.PROD_NOMBRE,
                                  det.DET_CANTIDAD,
                                  det.DET_PRECIO_UNITARIO,
                                  det.DET_SUBTOTAL
                              };

            foreach (var movimiento in movimientos)
            {
                dgvMovimientos.Rows.Add(
                    movimiento.CAB_ID,
                    movimiento.ClienteID, // Usar el ID del cliente en lugar del nombre
                    movimiento.CAB_FECHA,
                    movimiento.CAB_TIPO,
                    movimiento.CAB_MOTIVO,
                    movimiento.Proveedor,
                    movimiento.ProductoID, // Usar el ID del producto en lugar del nombre
                    movimiento.DET_CANTIDAD,
                    movimiento.DET_PRECIO_UNITARIO,
                    movimiento.DET_SUBTOTAL
                );
            }
        }

        private void dgvMovimientos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMovimientos.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvMovimientos.SelectedRows[0];

                // Verificar y convertir los valores de las celdas
                if (int.TryParse(row.Cells["CAB_CLIENTE_ID"].Value.ToString(), out int clienteId))
                {
                    cmbCliente.SelectedValue = clienteId;
                }
                else
                {
                    MessageBox.Show("Error de formato en CAB_CLIENTE_ID.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (DateTime.TryParse(row.Cells["CAB_FECHA"].Value.ToString(), out DateTime fecha))
                {
                    dtpFecha.Value = fecha;
                }
                else
                {
                    MessageBox.Show("Error de formato en CAB_FECHA.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                cmbTipoMov.Text = row.Cells["CAB_TIPO"].Value.ToString();
                cmbMotivo.Text = row.Cells["CAB_MOTIVO"].Value.ToString();
                cmbProveedor.Text = row.Cells["PROV_NOMBRE"].Value.ToString();

                if (int.TryParse(row.Cells["DET_PROD_ID"].Value.ToString(), out int productoId))
                {
                    cmbProductos.SelectedValue = productoId;
                }
                else
                {
                    MessageBox.Show("Error de formato en DET_PROD_ID.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                txtCantidad.Text = row.Cells["DET_CANTIDAD"].Value.ToString();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedItem == null || cmbProveedor.SelectedItem == null || string.IsNullOrEmpty(txtCantidad.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }

            var productoId = Convert.ToInt32(cmbProductos.SelectedValue);
            var producto = db.Producto.FirstOrDefault(p => p.PROD_ID == productoId);
            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado.");
                return;
            }

            decimal precioUnitario = Convert.ToDecimal(producto.PROD_PRECIO);
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            decimal subtotal = cantidad * precioUnitario;

            dgvMovimientos.Rows.Add(
                0, // ID temporal
                cmbCliente.SelectedValue,
                dtpFecha.Value,
                cmbTipoMov.Text,
                cmbMotivo.Text,
                cmbProveedor.Text,
                productoId,
                cantidad,
                precioUnitario,
                subtotal
            );

            MessageBox.Show("Movimiento agregado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvMovimientos.SelectedRows[0];

                if (cmbProductos.SelectedItem == null || cmbProveedor.SelectedItem == null || string.IsNullOrEmpty(txtCantidad.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos.");
                    return;
                }

                var productoId = Convert.ToInt32(cmbProductos.SelectedValue);
                var producto = db.Producto.FirstOrDefault(p => p.PROD_ID == productoId);
                if (producto == null)
                {
                    MessageBox.Show("Producto no encontrado.");
                    return;
                }

                decimal precioUnitario = Convert.ToDecimal(producto.PROD_PRECIO);
                int cantidad = Convert.ToInt32(txtCantidad.Text);
                decimal subtotal = cantidad * precioUnitario;

                row.Cells["CAB_CLIENTE_ID"].Value = cmbCliente.SelectedValue;
                row.Cells["CAB_FECHA"].Value = dtpFecha.Value;
                row.Cells["CAB_TIPO"].Value = cmbTipoMov.Text;
                row.Cells["CAB_MOTIVO"].Value = cmbMotivo.Text;
                row.Cells["PROV_NOMBRE"].Value = cmbProveedor.Text;
                row.Cells["DET_PROD_ID"].Value = productoId;
                row.Cells["DET_CANTIDAD"].Value = cantidad;
                row.Cells["DET_PRECIO_UNITARIO"].Value = precioUnitario;
                row.Cells["DET_SUBTOTAL"].Value = subtotal;

                MessageBox.Show("Movimiento modificado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione una fila para modificar.");
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.SelectedRows.Count > 0)
            {
                dgvMovimientos.Rows.RemoveAt(dgvMovimientos.SelectedRows[0].Index);
                MessageBox.Show("Movimiento eliminado temporalmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione una fila para quitar.");
            }
        }

        private void btnInsertar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvMovimientos.Rows)
                {
                    if (row.IsNewRow) continue;

                    int cabeceraId;
                    if (!int.TryParse(row.Cells["CAB_ID"].Value.ToString(), out cabeceraId) || cabeceraId == 0)
                    {
                        // Crear una nueva cabecera si no existe
                        var nuevaCabecera = new CabeceraMovimiento
                        {
                            CAB_CLIENTE_ID = Convert.ToInt32(cmbCliente.SelectedValue),
                            CAB_FECHA = dtpFecha.Value,
                            CAB_TIPO = cmbTipoMov.SelectedItem.ToString(),
                            CAB_MOTIVO = cmbMotivo.SelectedItem.ToString(),
                            PROV_ID = Convert.ToInt32(cmbProveedor.SelectedValue)
                        };

                        db.CabeceraMovimiento.InsertOnSubmit(nuevaCabecera);
                        db.SubmitChanges(); // Guardamos para obtener CAB_ID

                        cabeceraId = nuevaCabecera.CAB_ID;
                        row.Cells["CAB_ID"].Value = cabeceraId;
                    }

                    int detProdId;
                    if (!int.TryParse(row.Cells["DET_PROD_ID"].Value.ToString(), out detProdId))
                    {
                        throw new FormatException($"Error de formato en DET_PROD_ID: {row.Cells["DET_PROD_ID"].Value}");
                    }

                    int detCantidad;
                    if (!int.TryParse(row.Cells["DET_CANTIDAD"].Value.ToString(), out detCantidad))
                    {
                        throw new FormatException($"Error de formato en DET_CANTIDAD: {row.Cells["DET_CANTIDAD"].Value}");
                    }

                    double detPrecioUnitario;
                    if (!double.TryParse(row.Cells["DET_PRECIO_UNITARIO"].Value.ToString(), out detPrecioUnitario))
                    {
                        throw new FormatException($"Error de formato en DET_PRECIO_UNITARIO: {row.Cells["DET_PRECIO_UNITARIO"].Value}");
                    }

                    double detSubtotal;
                    if (!double.TryParse(row.Cells["DET_SUBTOTAL"].Value.ToString(), out detSubtotal))
                    {
                        throw new FormatException($"Error de formato en DET_SUBTOTAL: {row.Cells["DET_SUBTOTAL"].Value}");
                    }

                    var detalleExistente = db.DetalleMovimiento.FirstOrDefault(d => d.CAB_ID == cabeceraId && d.DET_PROD_ID == detProdId);
                    if (detalleExistente == null)
                    {
                        DetalleMovimiento detalle = new DetalleMovimiento
                        {
                            CAB_ID = cabeceraId,
                            DET_PROD_ID = detProdId,
                            DET_CANTIDAD = detCantidad,
                            DET_PRECIO_UNITARIO = detPrecioUnitario,
                            DET_SUBTOTAL = detSubtotal
                        };

                        db.DetalleMovimiento.InsertOnSubmit(detalle);
                    }
                }

                db.SubmitChanges();

                // Ahora actualizamos el stock de productos
                foreach (DataGridViewRow row in dgvMovimientos.Rows)
                {
                    if (row.IsNewRow) continue;

                    int cabeceraId = Convert.ToInt32(row.Cells["CAB_ID"].Value);
                    var cabecera = db.CabeceraMovimiento.FirstOrDefault(c => c.CAB_ID == cabeceraId);
                    if (cabecera == null) continue;

                    string tipoMov = cabecera.CAB_TIPO; // "Entrada" o "Salida"

                    int productoId = Convert.ToInt32(row.Cells["DET_PROD_ID"].Value);
                    int cantidad = Convert.ToInt32(row.Cells["DET_CANTIDAD"].Value);

                    var producto = db.Producto.FirstOrDefault(p => p.PROD_ID == productoId);
                    if (producto == null) continue;

                    if (tipoMov == "Entrada")
                    {
                        // Aumentar stock
                        producto.PROD_CANTIDAD += cantidad;
                    }
                    else if (tipoMov == "Salida")
                    {
                        // Disminuir stock
                        producto.PROD_CANTIDAD -= cantidad;
                        if (producto.PROD_CANTIDAD < 0)
                        {
                            MessageBox.Show("La cantidad en stock no puede ser negativa. Verifique la operación.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            producto.PROD_CANTIDAD = 0; // Ajustar a 0 o manejar el caso como corresponda
                        }
                    }
                }

                db.SubmitChanges();

                MessageBox.Show("El registro se insertó satisfactoriamente y el stock se actualizó.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Error de formato: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                if (dgvMovimientos.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dgvMovimientos.SelectedRows[0];

                    // Obtener CAB_ID y DET_PROD_ID antes del cambio
                    int cabeceraId = Convert.ToInt32(row.Cells["CAB_ID"].Value);
                    int productoId = Convert.ToInt32(row.Cells["DET_PROD_ID"].Value);

                    var cabecera = db.CabeceraMovimiento.FirstOrDefault(c => c.CAB_ID == cabeceraId);
                    if (cabecera == null)
                    {
                        MessageBox.Show("Cabecera no encontrada.");
                        return;
                    }

                    var detalleExistente = db.DetalleMovimiento.FirstOrDefault(d => d.CAB_ID == cabeceraId && d.DET_PROD_ID == productoId);
                    if (detalleExistente == null)
                    {
                        MessageBox.Show("Detalle no encontrado.");
                        return;
                    }

                    // Revertir stock con la cantidad anterior
                    var producto = db.Producto.FirstOrDefault(p => p.PROD_ID == productoId);
                    if (producto != null)
                    {
                        if (cabecera.CAB_TIPO == "Entrada")
                            producto.PROD_CANTIDAD -= detalleExistente.DET_CANTIDAD;
                        else if (cabecera.CAB_TIPO == "Salida")
                            producto.PROD_CANTIDAD += detalleExistente.DET_CANTIDAD;
                    }

                    // Ahora aplicar los cambios según los nuevos valores
                    if (cmbProductos.SelectedItem == null || cmbProveedor.SelectedItem == null || string.IsNullOrEmpty(txtCantidad.Text))
                    {
                        MessageBox.Show("Por favor, complete todos los campos.");
                        return;
                    }

                    var nuevoProductoId = Convert.ToInt32(cmbProductos.SelectedValue);
                    var nuevoProducto = db.Producto.FirstOrDefault(p => p.PROD_ID == nuevoProductoId);
                    if (nuevoProducto == null)
                    {
                        MessageBox.Show("Producto no encontrado.");
                        return;
                    }

                    double precioUnitario = Convert.ToDouble(nuevoProducto.PROD_PRECIO);
                    int nuevaCantidad = Convert.ToInt32(txtCantidad.Text);
                    double subtotal = nuevaCantidad * precioUnitario;

                    cabecera.CAB_CLIENTE_ID = Convert.ToInt32(cmbCliente.SelectedValue);
                    cabecera.CAB_FECHA = dtpFecha.Value;
                    cabecera.CAB_TIPO = cmbTipoMov.SelectedItem.ToString();
                    cabecera.CAB_MOTIVO = cmbMotivo.SelectedItem.ToString();
                    cabecera.PROV_ID = Convert.ToInt32(cmbProveedor.SelectedValue);

                    // Cambiar el detalle
                    var nuevoDetalle = db.DetalleMovimiento.FirstOrDefault(d => d.CAB_ID == cabeceraId && d.DET_PROD_ID == productoId);
                    // Si cambió el producto, hay que eliminar el detalle viejo y crear uno nuevo
                    if (nuevoProductoId != productoId && nuevoDetalle != null)
                    {
                        db.DetalleMovimiento.DeleteOnSubmit(nuevoDetalle);
                        // Crear un nuevo detalle con el nuevo producto
                        var detalle = new DetalleMovimiento
                        {
                            CAB_ID = cabeceraId,
                            DET_PROD_ID = nuevoProductoId,
                            DET_CANTIDAD = nuevaCantidad,
                            DET_PRECIO_UNITARIO = precioUnitario,
                            DET_SUBTOTAL = subtotal
                        };
                        db.DetalleMovimiento.InsertOnSubmit(detalle);
                    }
                    else
                    {
                        // Mismo producto
                        nuevoDetalle.DET_CANTIDAD = nuevaCantidad;
                        nuevoDetalle.DET_PRECIO_UNITARIO = precioUnitario;
                        nuevoDetalle.DET_SUBTOTAL = subtotal;
                    }

                    db.SubmitChanges();

                    // Ajustar stock con la nueva cantidad
                    if (cabecera.CAB_TIPO == "Entrada")
                        nuevoProducto.PROD_CANTIDAD += nuevaCantidad;
                    else if (cabecera.CAB_TIPO == "Salida")
                        nuevoProducto.PROD_CANTIDAD -= nuevaCantidad;

                    db.SubmitChanges();

                    // Actualizar la fila del DGV
                    row.Cells["CAB_CLIENTE_ID"].Value = cmbCliente.SelectedValue;
                    row.Cells["CAB_FECHA"].Value = dtpFecha.Value;
                    row.Cells["CAB_TIPO"].Value = cmbTipoMov.Text;
                    row.Cells["CAB_MOTIVO"].Value = cmbMotivo.Text;
                    row.Cells["PROV_NOMBRE"].Value = cmbProveedor.Text;
                    row.Cells["DET_PROD_ID"].Value = nuevoProductoId;
                    row.Cells["DET_CANTIDAD"].Value = nuevaCantidad;
                    row.Cells["DET_PRECIO_UNITARIO"].Value = precioUnitario;
                    row.Cells["DET_SUBTOTAL"].Value = subtotal;

                    MessageBox.Show("El registro se actualizó satisfactoriamente y se ajustó el stock.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Seleccione una fila para actualizar.");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Error de formato: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los datos: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvMovimientos.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvMovimientos.SelectedRows[0];
                int cabeceraId = Convert.ToInt32(row.Cells["CAB_ID"].Value);

                var cabecera = db.CabeceraMovimiento.FirstOrDefault(c => c.CAB_ID == cabeceraId);
                if (cabecera != null)
                {
                    var detalles = db.DetalleMovimiento.Where(d => d.CAB_ID == cabeceraId).ToList();

                    // Revertir stock antes de eliminar
                    foreach (var detalle in detalles)
                    {
                        var producto = db.Producto.FirstOrDefault(p => p.PROD_ID == detalle.DET_PROD_ID);
                        if (producto != null)
                        {
                            if (cabecera.CAB_TIPO == "Entrada")
                                producto.PROD_CANTIDAD -= detalle.DET_CANTIDAD;
                            else if (cabecera.CAB_TIPO == "Salida")
                                producto.PROD_CANTIDAD += detalle.DET_CANTIDAD;
                        }
                    }

                    db.DetalleMovimiento.DeleteAllOnSubmit(detalles);
                    db.CabeceraMovimiento.DeleteOnSubmit(cabecera);
                    db.SubmitChanges();

                    dgvMovimientos.Rows.RemoveAt(row.Index);

                    MessageBox.Show("El registro se eliminó satisfactoriamente y se ajustó el stock.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Registro no encontrado en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Seleccione una fila para eliminar.");
            }
        }
        private void dgvMovimientos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
