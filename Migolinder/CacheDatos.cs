using System;
using System.Data;
using System.Threading.Tasks;

namespace Migolinder
{
    public static class CacheDatos
    {
        public static DataTable TablaTraspasos { get; set; }
        public static DataTable TablaInventario { get; set; }
        public static Task TareaCargaInicial { get; private set; }

        public static void IniciarPrecarga()
        {
            TareaCargaInicial = Task.Run(() =>
            {
                try
                {
                    // Consulta con los nombres exactos en minúsculas de Clever Cloud
                    string queryTraspasos = "SELECT * FROM traspasos LIMIT 100";
                    TablaTraspasos = ConexionBD.CargarDatos(queryTraspasos);

                    string queryInventario = "SELECT * FROM inventario";
                    TablaInventario = ConexionBD.CargarDatos(queryInventario);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error precargando datos: {ex.Message}");
                }
            });
        }
    }
}