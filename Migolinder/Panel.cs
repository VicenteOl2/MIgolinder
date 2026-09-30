using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Migolinder
{
    public partial class Panel : Form
    {
        public Panel()
        {
            InitializeComponent();
        }
        private void AbrirFormHija(object formHija)
        {
            // Si ya hay un panel abierto, lo quita
            if (this.PanelContenedor.Controls.Count > 0)
                this.PanelContenedor.Controls.RemoveAt(0);

            // Configura el nuevo formulario para que se comporte como un panel
            Form fh = formHija as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;

            // Lo incrusta en el contenedor y lo muestra
            this.PanelContenedor.Controls.Add(fh);
            this.PanelContenedor.Tag = fh;
            fh.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new FrmDashBoard());
        }
    }
}
