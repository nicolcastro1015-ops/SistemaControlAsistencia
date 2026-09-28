using SistemaControlAsistencia.App.Data;

namespace SistemaControlAsistencia.App.Services
{
    // Punto único de construcción de repositorios y servicios para las pantallas. Sin
    // contenedor de inyección de dependencias, deliberadamente simple para un MVP académico.
    // Las pruebas unitarias no usan esta fábrica: instancian los servicios directamente con
    // repositorios falsos (ver carpeta Fakes).
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
