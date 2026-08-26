using System;
using System.IO;
using System.Text.Json;

namespace SistemaControlAsistencia.App.Data
{
    /// <summary>
    /// Lee appsettings.json usando System.Text.Json (incluido en .NET, sin NuGet adicional)
    /// y expone la cadena de conexión hacia SQL Server.
    /// </summary>
    public static class ConfiguracionApp
    {
        private static string? _cadenaConexion;

        public static string ObtenerCadenaConexion()
        {
            if (_cadenaConexion != null)
                return _cadenaConexion;

            string ruta = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (!File.Exists(ruta))
                throw new FileNotFoundException(
                    "No se encontró appsettings.json junto al ejecutable. Verifique que el archivo " +
                    "tenga la propiedad 'Copiar si es más reciente' habilitada.", ruta);

            string json = File.ReadAllText(ruta);
            using JsonDocument documento = JsonDocument.Parse(json);

            _cadenaConexion = documento.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("ControlAsistencia")
                .GetString();

            if (string.IsNullOrWhiteSpace(_cadenaConexion))
                throw new InvalidOperationException("La cadena de conexión 'ControlAsistencia' está vacía en appsettings.json.");

            return _cadenaConexion;
        }
    }
}
