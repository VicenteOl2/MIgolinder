using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Migolinder
{
    public static class ConexionBD
    {
        // Cadena directa con credenciales de Clever Cloud y parámetros de seguridad MySQL 8
        private static readonly string CadenaConexion =
            "Server=bfes4mrrgwzpspgsb9j9-mysql.services.clever-cloud.com;" +
            "Port=3306;" +
            "Database=bfes4mrrgwzpspgsb9j9;" +
            "Uid=u3p2t4fyeyhaf1ee;" +
            "Pwd=xOVS3edCNPheeYfZw7kz;" +
            "SslMode=Required;" +
            "AllowPublicKeyRetrieval=True;";

        public static MySqlConnection ObtenerConexion()
        {
            var con = new MySqlConnection(CadenaConexion);
            con.Open();
            return con;
        }

        public static DataTable CargarDatos(string consultaSql)
        {
            DataTable tabla = new DataTable();
            using (var con = ObtenerConexion())
            using (var cmd = new MySqlCommand(consultaSql, con))
            using (var adapter = new MySqlDataAdapter(cmd))
            {
                adapter.Fill(tabla);
            }
            return tabla;
        }

        public static int EjecutarComando(string consultaSql, MySqlParameter[] parametros = null)
        {
            using (var con = ObtenerConexion())
            using (var cmd = new MySqlCommand(consultaSql, con))
            {
                if (parametros != null)
                {
                    cmd.Parameters.AddRange(parametros);
                }
                return cmd.ExecuteNonQuery();
            }
        }
    }
}