using MySql.Data.MySqlClient;
using System.Configuration;

namespace BibliotecaApp
{
    public class ConexionBD
    {
        public static MySqlConnection ObtenerConexion()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Open();
            return conexion;
        }
    }
}
