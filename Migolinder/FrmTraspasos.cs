using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace Migolinder
{
    public partial class FrmTraspasos : Form
    {
        private DataTable tablaTraspasos = new DataTable();

        public FrmTraspasos()
        {
            InitializeComponent();

            // Cargar la estructura de la tabla directamente al construir la pantalla
            InicializarTabla();
        }

        private void InicializarTabla()
        {
            // Estructura de la tabla en memoria
            if (tablaTraspasos.Columns.Count == 0)
            {
                tablaTraspasos.Columns.Add("Material", typeof(string));
                tablaTraspasos.Columns.Add("Bodega Origen", typeof(string));
                tablaTraspasos.Columns.Add("Obra Destino", typeof(string));
                tablaTraspasos.Columns.Add("Cantidad", typeof(int));
                tablaTraspasos.Columns.Add("Fecha", typeof(string));
            }

            // Asegurar que genere las columnas automáticamente
            dgvTraspasos.AutoGenerateColumns = true;
            dgvTraspasos.DataSource = tablaTraspasos;

         
            dgvTraspasos.DefaultCellStyle.ForeColor = Color.Black;
            dgvTraspasos.DefaultCellStyle.BackColor = Color.White;
            dgvTraspasos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 33, 71);
            dgvTraspasos.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvTraspasos.BackgroundColor = Color.DarkGray;
            dgvTraspasos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTraspasos.AllowUserToAddRows = false;
            dgvTraspasos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTraspasos.ReadOnly = true;
        }

        private void FrmTraspasos_Load(object sender, EventArgs e)
        {
            // Mantenemos esto por si el evento Load se llega a vincular
            InicializarTabla();
        }

        // --- REGISTRAR ---
        private void btnRegistrar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaterial.Text) || string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Por favor, completa al menos el material y la cantidad.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tablaTraspasos.Rows.Add(
                txtMaterial.Text,
                txtBodega.Text,
                txtObra.Text,
                cantidad,
                DateTime.Now.ToString("dd/MM/yyyy")
            );

            LimpiarCampos();
        }

        // --- MODIFICAR ---
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvTraspasos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una fila para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = dgvTraspasos.CurrentRow.Index;
            tablaTraspasos.Rows[index]["Material"] = txtMaterial.Text;
            tablaTraspasos.Rows[index]["Bodega Origen"] = txtBodega.Text;
            tablaTraspasos.Rows[index]["Obra Destino"] = txtObra.Text;
            tablaTraspasos.Rows[index]["Cantidad"] = cantidad;
            tablaTraspasos.Rows[index]["Fecha"] = DateTime.Now.ToString("dd/MM/yyyy");

            LimpiarCampos();
        }

        // --- ELIMINAR ---
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTraspasos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una fila para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            tablaTraspasos.Rows.RemoveAt(dgvTraspasos.CurrentRow.Index);
            LimpiarCampos();
        }

        private void dgvTraspasos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvTraspasos.Rows[e.RowIndex];
            txtMaterial.Text = fila.Cells["Material"].Value?.ToString();
            txtBodega.Text = fila.Cells["Bodega Origen"].Value?.ToString();
            txtObra.Text = fila.Cells["Obra Destino"].Value?.ToString();
            txtCantidad.Text = fila.Cells["Cantidad"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            txtMaterial.Clear();
            txtBodega.Clear();
            txtObra.Clear();
            txtCantidad.Clear();
            txtMaterial.Focus();
        }

        // --- EXPORTAR EXCEL (.xlsx real con ClosedXML) ---
        private void button4_Click(object sender, EventArgs e)
        {
            if (tablaTraspasos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog();
            guardar.Filter = "Documento de Excel (*.xlsx)|*.xlsx";
            guardar.FileName = "Reporte_Traspasos_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

            if (guardar.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var hoja = workbook.Worksheets.Add("Traspasos");

                    // Encabezados
                    for (int col = 0; col < tablaTraspasos.Columns.Count; col++)
                    {
                        hoja.Cell(1, col + 1).Value = tablaTraspasos.Columns[col].ColumnName;
                    }

                    // Filas
                    for (int fila = 0; fila < tablaTraspasos.Rows.Count; fila++)
                    {
                        for (int col = 0; col < tablaTraspasos.Columns.Count; col++)
                        {
                            hoja.Cell(fila + 2, col + 1).Value = tablaTraspasos.Rows[fila][col]?.ToString();
                        }
                    }

                    hoja.Row(1).Style.Font.Bold = true;
                    hoja.Columns().AdjustToContents();
                    workbook.SaveAs(guardar.FileName);
                }

                MessageBox.Show("¡Archivo de Excel generado correctamente!", "MINDER", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}