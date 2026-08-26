using SistemaControlAsistencia.App.Data;

namespace SistemaControlAsistencia.App.Services
{
    /// <summary>
    /// Punto único de construcción de repositorios y servicios para las vistas.
    /// Se mantiene deliberadamente simple (sin contenedor de inyección de dependencias)
    /// porque el proyecto es un MVP académico; para las pruebas unitarias, en cambio,
    /// los servicios se instancian directamente con repositorios falsos (ver carpeta Fakes).
    /// </summary>
    public static class FabricaServicios
    {
        private static readonly IUsuarioRepository _usuarioRepository = new UsuarioRepository();
        private static readonly IAsistenciaRepository _asistenciaRepository = new AsistenciaRepository();

        public static AutenticacionService CrearAutenticacionService() => new(_usuarioRepository);
        public static UsuarioService CrearUsuarioService() => new(_usuarioRepository);
        public static AsistenciaService CrearAsistenciaService() => new(_asistenciaRepository);
        public static ReporteService CrearReporteService() => new(_usuarioRepository, _asistenciaRepository);
    }
}
