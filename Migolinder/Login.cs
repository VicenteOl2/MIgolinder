using System;
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

        // TODO UI (Mañana): Cambiar el nombre de 'button1' a 'btnIngresar' desde el panel de Propiedades.
        private void button1_Click(object sender, EventArgs e)
        {
            string usuario = textUsuario.Text;
            string clave = textClave.Text;

            // Hay que validar contra la tabla Usuarios en Clever Cloud, no dejarlas en texto plano para GitHub.
            if ((usuario == "admin" && clave == "MinderAdmin.2026") ||
                (usuario == "bodega_chillan" && clave == "Fierros.2026"))
            {
          

                // (Opcional) Comenté el MessageBox para que el login sea más rápido y fluido
                // MessageBox.Show("¡Bienvenido al sistema de Minder Construcciones!", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 2. Levantar interfaz principal
                Panel menuPrincipal = new Panel();
                menuPrincipal.SetUsuario(usuario);
                menuPrincipal.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos. Intenta de nuevo.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void Form1_Load(object sender, EventArgs e) 
        {
            CacheDatos.IniciarPrecarga();
        }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }

    }
}