using Microsoft.Data.SqlClient;

namespace SistemaControlAsistencia.App.Data
{
    /// <summary>
    /// Fábrica centralizada de conexiones a SQL Server. Todos los repositorios
    /// obtienen su conexión aquí, por lo que la cadena de conexión solo se
    /// configura en un único lugar (appsettings.json).
    /// </summary>
    public static class DatabaseHelper
    {
        public static SqlConnection CrearConexion()
        {
            string cadena = ConfiguracionApp.ObtenerCadenaConexion();
            return new SqlConnection(cadena);
        }
    }
}
