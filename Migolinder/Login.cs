using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Migolinder
{
    public partial class Login : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string usuario = textUsuario.Text;
            string clave = textClave.Text;

            if ((usuario == "admin" && clave == "MinderAdmin.2026") ||
                (usuario == "bodega_chillan" && clave == "Fierros.2026"))
            {
                MessageBox.Show("¡Bienvenido al sistema de Minder Construcciones!", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 1. Crear la instancia del formulario Panel
                Panel menuPrincipal = new Panel();

                // 2. Mostrar la ventana principal (Panel)
                menuPrincipal.Show();

                // 3. Ocultar el formulario de Login actual
                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos. Intenta de nuevo.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
