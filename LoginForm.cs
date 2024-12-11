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
    public partial class LoginForm : Form
    {
        // Crear una instancia del DataContext
        // Asegúrate que el nombre coincide con el que generó tu archivo .dbml
        private DataClasses1DataContext db;

        public LoginForm()
        {
            InitializeComponent();
            // Inicializamos el DataContext
            db = new DataClasses1DataContext();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Este es el botón de Login (asumiendo que button1 es el botón "Iniciar sesión")

            string nombreUsuario = txtUser.Text.Trim();
            string contraseña = txtPassword.Text.Trim();

            // Validar campos vacíos
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor, ingrese usuario y contraseña.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Usar LINQ para buscar el usuario en la base de datos
            var usuario = db.Usuario.FirstOrDefault(u => u.USU_NOMBRE == nombreUsuario && u.USU_CONTRASEÑA == contraseña);

            if (usuario != null)
            {
                // Si encontró un usuario, las credenciales son correctas
                MessageBox.Show("Login exitoso.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Crear una instancia del formulario Principal
                Principal mainForm = new Principal();
                mainForm.Show();

                // Ocultar o cerrar el formulario de login
                this.Hide();
            }
            else
            {
                // No se encontró usuario con esas credenciales
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bttCerrar_Click(object sender, EventArgs e)
        {
            // Este es el botón para cerrar la aplicación
            Application.Exit();
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            // Puedes dejar vacío o agregar validaciones en tiempo real si lo deseas.
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            // Puedes dejar vacío o agregar validaciones en tiempo real si lo deseas.
        }
    }
}
