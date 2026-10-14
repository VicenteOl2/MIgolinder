using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;

namespace Migolinder
{
    public partial class FrmReportes : Form
    {
        private DataTable tablaInventario = new DataTable();

        public FrmReportes()
        {
            InitializeComponent();
        }

        private void FrmReportes_Load(object sender, EventArgs e)
        {
            // Ajustar estilos visuales del DataGridView
            dgvReportes.DefaultCellStyle.ForeColor = Color.Black;
            dgvReportes.DefaultCellStyle.BackColor = Color.White;
            dgvReportes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 33, 71);
            dgvReportes.DefaultCellStyle.SelectionForeColor = Color.White;

            CargarDatosDesdeBD();
        }

        // --- CARGAR DATOS DESDE CLEVER CLOUD ---
        private void CargarDatosDesdeBD()
        {
            try
            {
                // 1. Aseguramos que la tabla exista en la base de datos
                string sqlCrearTabla = @"
                    CREATE TABLE IF NOT EXISTS inventario (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        codigo VARCHAR(50) NOT NULL,
                        nombre VARCHAR(150) NOT NULL,
                        stock INT NOT NULL DEFAULT 0,
                        bodega VARCHAR(100) NOT NULL,
                        fecha VARCHAR(20) NOT NULL
                    );";
                ConexionBD.EjecutarComando(sqlCrearTabla);

                // 2. Forzar adición de la columna 'fecha' por si la tabla ya existía de antes sin ella
                try
                {
                    ConexionBD.EjecutarComando("ALTER TABLE inventario ADD COLUMN fecha VARCHAR(20) NOT NULL DEFAULT '';");
                }
                catch
                {
                    // Si la columna 'fecha' ya existe, simplemente continúa sin interrumpir
                }

                // 3. Cargar registros
                string sql = "SELECT id AS ID, codigo AS 'Código', nombre AS 'Material', stock AS 'Stock Actual', bodega AS 'Ubicación', fecha AS 'Último Movimiento' FROM inventario ORDER BY id DESC";
                tablaInventario = ConexionBD.CargarDatos(sql);

                dgvReportes.AutoGenerateColumns = true;
                dgvReportes.DataSource = tablaInventario;
                dgvReportes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvReportes.AllowUserToAddRows = false;
                dgvReportes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvReportes.ReadOnly = true;

                // Ocultar la columna ID (necesaria internamente para editar/eliminar)
                if (dgvReportes.Columns["ID"] != null)
                {
                    dgvReportes.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- BOTÓN AGREGAR EN LA NUBE ---
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtMaterial.Text))
            {
                MessageBox.Show("Por favor ingresa al menos el Código y el Material.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int stock = 0;
            int.TryParse(txtStock.Text, out stock);

            try
            {
                string sql = "INSERT INTO inventario (codigo, nombre, stock, bodega, fecha) VALUES (@cod, @nom, @stk, @bod, @fec)";
                MySqlParameter[] parametros = new MySqlParameter[]
                {
                    new MySqlParameter("@cod", txtCodigo.Text.Trim()),
                    new MySqlParameter("@nom", txtMaterial.Text.Trim()),
                    new MySqlParameter("@stk", stock),
                    new MySqlParameter("@bod", txtUbicacion.Text.Trim()),
                    new MySqlParameter("@fec", DateTime.Now.ToString("dd/MM/yyyy"))
                };

                ConexionBD.EjecutarComando(sql, parametros);
                CargarDatosDesdeBD();
                LimpiarCampos();
                MessageBox.Show("Material guardado correctamente en la nube.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- BOTÓN EDITAR / MODIFICAR EN LA NUBE ---
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una fila para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int stock = 0;
            int.TryParse(txtStock.Text, out stock);

            try
            {
                int id = Convert.ToInt32(dgvReportes.CurrentRow.Cells["ID"].Value);

                string sql = "UPDATE inventario SET codigo = @cod, nombre = @nom, stock = @stk, bodega = @bod, fecha = @fec WHERE id = @id";
                MySqlParameter[] parametros = new MySqlParameter[]
                {
                    new MySqlParameter("@cod", txtCodigo.Text.Trim()),
                    new MySqlParameter("@nom", txtMaterial.Text.Trim()),
                    new MySqlParameter("@stk", stock),
                    new MySqlParameter("@bod", txtUbicacion.Text.Trim()),
                    new MySqlParameter("@fec", DateTime.Now.ToString("dd/MM/yyyy")),
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

        // --- BOTÓN ELIMINAR DE LA NUBE ---
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una fila para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Seguro que deseas eliminar este elemento del inventario?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                int id = Convert.ToInt32(dgvReportes.CurrentRow.Cells["ID"].Value);

                string sql = "DELETE FROM inventario WHERE id = @id";
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

        // --- SELECCIONAR FILA DEL DATA GRID VIEW ---
        private void dgvReportes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvReportes.Rows[e.RowIndex];
            txtCodigo.Text = fila.Cells["Código"].Value?.ToString();
            txtMaterial.Text = fila.Cells["Material"].Value?.ToString();
            txtStock.Text = fila.Cells["Stock Actual"].Value?.ToString();
            txtUbicacion.Text = fila.Cells["Ubicación"].Value?.ToString();
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtMaterial.Clear();
            txtStock.Clear();
            txtUbicacion.Clear();
            txtCodigo.Focus();
        }

        // --- BOTÓN EXPORTAR EXCEL ---
        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (tablaInventario.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos cargados para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog guardar = new SaveFileDialog
            {
                Filter = "Documento de Excel (*.xlsx)|*.xlsx",
                FileName = "Reporte_Inventario_Minder_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx"
            };

            if (guardar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        var hoja = workbook.Worksheets.Add("Inventario");

                        // Encabezados
                        for (int col = 0; col < tablaInventario.Columns.Count; col++)
                        {
                            hoja.Cell(1, col + 1).Value = tablaInventario.Columns[col].ColumnName;
                        }

                        // Filas
                        for (int fila = 0; fila < tablaInventario.Rows.Count; fila++)
                        {
                            for (int col = 0; col < tablaInventario.Columns.Count; col++)
                            {
                                hoja.Cell(fila + 2, col + 1).Value = tablaInventario.Rows[fila][col]?.ToString();
                            }
                        }

                        hoja.Row(1).Style.Font.Bold = true;
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

        private void label1_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
    }
}