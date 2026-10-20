using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Migolinder
{
    public static class ConexionBD
    {
        // TODO URGENTE (Mañana): Mover esta cadena al archivo App.config dentro de <connectionStrings>.
        // Subir credenciales duras a GitHub es un riesgo crítico de seguridad.
        private static readonly string CadenaConexion =
            "Server=bfes4mrrgwzpspgsb9j9-mysql.services.clever-cloud.com;" +
            "Port=3306;" +
            "Database=bfes4mrrgwzpspgsb9j9;" +
            "Uid=u3p2t4fyeyhaf1ee;" +
            "Pwd=xOVS3edCNPheeYfZw7kz;" +
            "SslMode=Required;" +
            "AllowPublicKeyRetrieval=True;" +
            "Connection Timeout=30;"; // Añadido: 30 segundos de espera para compensar el ping hacia París.

        public static MySqlConnection ObtenerConexion()
        {
            var con = new MySqlConnection(CadenaConexion);
            con.Open();
            return con;
        }

        public static DataTable CargarDatos(string consultaSql)
        {
            DataTable tabla = new DataTable();

            // TODO (Mañana): Manejar excepciones de red aquí con try/catch en caso de caída del servidor.
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