using System;
using System.IO;
using System.Text.Json;

namespace SistemaControlAsistencia.App.Data
{
    // Lee la cadena de conexión desde appsettings.json. Se usa System.Text.Json porque ya
    // viene incluido en .NET, sin necesitar un paquete NuGet adicional.
    public static class ConfiguracionApp
    {
        // Se cachea tras la primera lectura, porque cada consulta abre su propia SqlConnection.
        private static string? _cadenaConexion;

        public static string ObtenerCadenaConexion()
        {
            if (_cadenaConexion != null)
                return _cadenaConexion;

            // AppContext.BaseDirectory apunta a la carpeta del .exe compilado, por eso
            // appsettings.json necesita "Copiar si es más reciente" activado en Visual Studio.
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
