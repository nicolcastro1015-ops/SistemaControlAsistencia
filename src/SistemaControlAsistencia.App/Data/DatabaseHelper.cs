using Microsoft.Data.SqlClient;

namespace SistemaControlAsistencia.App.Data
{
    // Fábrica centralizada de conexiones a SQL Server. Todos los repositorios obtienen su
    // conexión aquí, así la cadena de conexión se configura en un único lugar (appsettings.json).
    public static class DatabaseHelper
    {
        public static SqlConnection CrearConexion()
        {
            string cadena = ConfiguracionApp.ObtenerCadenaConexion();
            return new SqlConnection(cadena);
        }
    }
}
