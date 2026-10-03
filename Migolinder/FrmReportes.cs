using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace Migolinder
{
    public partial class FrmReportes : Form
    {
        public FrmReportes()
        {
            InitializeComponent();
        }

        private DataTable tabla = new DataTable();
 

        private void FrmReportes_Load(object sender, EventArgs e)
        {

            btnAgregar.Click += btnAgregar_Click;
            btnEditar.Click += btnEditar_Click;
            btnEliminar.Click += btnEliminar_Click;
            dgvReportes.CellClick += dgvReportes_CellClick;

            // 1. SOLUCIÓN AL TEXTO INVISIBLE: Ajustar colores del DataGridView
            dgvReportes.DefaultCellStyle.ForeColor = Color.Black;
            dgvReportes.DefaultCellStyle.BackColor = Color.White;
            dgvReportes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 33, 71);
            dgvReportes.DefaultCellStyle.SelectionForeColor = Color.White;

       
            tabla.Columns.Add("Código", typeof(string));
            tabla.Columns.Add("Material", typeof(string));
            tabla.Columns.Add("Stock Actual", typeof(int));
            tabla.Columns.Add("Ubicación", typeof(string));
            tabla.Columns.Add("Último Movimiento", typeof(string));

            dgvReportes.DataSource = tabla;
        }

        // --- BOTÓN AGREGAR ---
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtMaterial.Text))
            {
                MessageBox.Show("Por favor ingresa al menos el Código y el Material.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int stock = 0;
            int.TryParse(txtStock.Text, out stock);

            // Agregamos la nueva fila a la tabla
            tabla.Rows.Add(txtCodigo.Text, txtMaterial.Text, stock, txtUbicacion.Text, DateTime.Now.ToString("dd/MM/yyyy"));

            LimpiarCampos();
        }

        // --- BOTÓN EDITAR / MODIFICAR ---
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow != null && dgvReportes.CurrentRow.Index < tabla.Rows.Count)
            {
                int index = dgvReportes.CurrentRow.Index;

                int stock = 0;
                int.TryParse(txtStock.Text, out stock);

                // Modificamos los datos de la fila seleccionada
                tabla.Rows[index]["Código"] = txtCodigo.Text;
                tabla.Rows[index]["Material"] = txtMaterial.Text;
                tabla.Rows[index]["Stock Actual"] = stock;
                tabla.Rows[index]["Ubicación"] = txtUbicacion.Text;
                tabla.Rows[index]["Último Movimiento"] = DateTime.Now.ToString("dd/MM/yyyy");

                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Selecciona una fila para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- BOTÓN ELIMINAR ---
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow != null && dgvReportes.CurrentRow.Index < tabla.Rows.Count)
            {
                int index = dgvReportes.CurrentRow.Index;
                tabla.Rows.RemoveAt(index);
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Selecciona una fila para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Al hacer clic en una fila del DataGridView, cargamos sus datos en los TextBox para editar rápido
        private void dgvReportes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < tabla.Rows.Count)
            {
                DataGridViewRow fila = dgvReportes.Rows[e.RowIndex];
                txtCodigo.Text = fila.Cells["Código"].Value?.ToString();
                txtMaterial.Text = fila.Cells["Material"].Value?.ToString();
                txtStock.Text = fila.Cells["Stock Actual"].Value?.ToString();
                txtUbicacion.Text = fila.Cells["Ubicación"].Value?.ToString();
            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtMaterial.Clear();
            txtStock.Clear();
            txtUbicacion.Clear();
        }

        // --- BOTÓN EXPORTAR EXCEL ---
        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dgvReportes.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos cargados para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Documento de Excel (*.xlsx)|*.xlsx";
            guardar.FileName = "Reporte_Inventario_Minder_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var hoja = workbook.Worksheets.Add("Inventario");

                        for (int col = 0; col < dgvReportes.Columns.Count; col++)
                        {
                            hoja.Cell(1, col + 1).Value = dgvReportes.Columns[col].HeaderText;
                        }

                        for (int fila = 0; fila < dgvReportes.Rows.Count; fila++)
                        {
                            for (int col = 0; col < dgvReportes.Columns.Count; col++)
                            {
                                hoja.Cell(fila + 2, col + 1).Value = dgvReportes.Rows[fila].Cells[col].Value?.ToString();
                            }
                        }

                        hoja.Columns().AdjustToContents();
                        workbook.SaveAs(guardar.FileName);

                        MessageBox.Show("¡Archivo de Excel generado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
