using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;

namespace Migolinder
{
    public partial class FrmTraspasos : Form
    {
        private DataTable tablaTraspasos = new DataTable();

        public FrmTraspasos()
        {
            InitializeComponent();
            CargarDatosDesdeBD();
        }

        private void FrmTraspasos_Load(object sender, EventArgs e)
        {
            CargarDatosDesdeBD();
        }

        // --- CARGAR DATOS DESDE CLEVER CLOUD ---
        private void CargarDatosDesdeBD()
        {
            try
            {
                string sql = "SELECT id AS ID, material AS Material, bodega_origen AS 'Bodega Origen', obra_destino AS 'Obra Destino', cantidad AS Cantidad, fecha AS Fecha FROM traspasos ORDER BY id DESC";
                tablaTraspasos = ConexionBD.CargarDatos(sql);

                dgvTraspasos.AutoGenerateColumns = true;
                dgvTraspasos.DataSource = tablaTraspasos;

                // Estilos del DataGridView
                dgvTraspasos.DefaultCellStyle.ForeColor = Color.Black;
                dgvTraspasos.DefaultCellStyle.BackColor = Color.White;
                dgvTraspasos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 33, 71);
                dgvTraspasos.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvTraspasos.BackgroundColor = Color.DarkGray;
                dgvTraspasos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvTraspasos.AllowUserToAddRows = false;
                dgvTraspasos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvTraspasos.ReadOnly = true;

                // Ocultar la columna ID (necesaria internamente para modificar y eliminar)
                if (dgvTraspasos.Columns["ID"] != null)
                {
                    dgvTraspasos.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- REGISTRAR EN LA BASE DE DATOS ---
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

            try
            {
                string sql = "INSERT INTO traspasos (material, bodega_origen, obra_destino, cantidad, fecha) VALUES (@mat, @ori, @des, @cant, @fec)";

                MySqlParameter[] parametros = new MySqlParameter[]
                {
                    new MySqlParameter("@mat", txtMaterial.Text.Trim()),
                    new MySqlParameter("@ori", txtBodega.Text.Trim()),
                    new MySqlParameter("@des", txtObra.Text.Trim()),
                    new MySqlParameter("@cant", cantidad),
                    new MySqlParameter("@fec", DateTime.Now.ToString("dd/MM/yyyy"))
                };

                ConexionBD.EjecutarComando(sql, parametros);
                CargarDatosDesdeBD();
                LimpiarCampos();
                MessageBox.Show("Traspaso registrado correctamente en la nube.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- MODIFICAR REGISTRO EN LA NUBE ---
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

            try
            {
                int id = Convert.ToInt32(dgvTraspasos.CurrentRow.Cells["ID"].Value);

                string sql = "UPDATE traspasos SET material = @mat, bodega_origen = @ori, obra_destino = @des, cantidad = @cant WHERE id = @id";

                MySqlParameter[] parametros = new MySqlParameter[]
                {
                    new MySqlParameter("@mat", txtMaterial.Text.Trim()),
                    new MySqlParameter("@ori", txtBodega.Text.Trim()),
                    new MySqlParameter("@des", txtObra.Text.Trim()),
                    new MySqlParameter("@cant", cantidad),
                    new MySqlParameter("@id", id)
                };

                ConexionBD.EjecutarComando(sql, parametros);
                CargarDatosDesdeBD();
                LimpiarCampos();
                MessageBox.Show("Registro actualizado en la nube.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- ELIMINAR REGISTRO EN LA NUBE ---
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTraspasos.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una fila para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Seguro que deseas eliminar este registro de la base de datos?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                int id = Convert.ToInt32(dgvTraspasos.CurrentRow.Cells["ID"].Value);

                string sql = "DELETE FROM traspasos WHERE id = @id";
                MySqlParameter[] parametros = new MySqlParameter[]
                {
                    new MySqlParameter("@id", id)
                };

                ConexionBD.EjecutarComando(sql, parametros);
                CargarDatosDesdeBD();
                LimpiarCampos();
                MessageBox.Show("Registro eliminado de la nube.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- SELECCIONAR FILA DEL GRID ---
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

        // --- EXPORTAR A EXCEL (.xlsx real con ClosedXML) ---
        private void button4_Click(object sender, EventArgs e)
        {
            if (tablaTraspasos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog
            {
                Filter = "Documento de Excel (*.xlsx)|*.xlsx",
                FileName = "Reporte_Traspasos_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx"
            };

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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}