using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Services
{
    /// <summary>
    /// Mantiene el usuario autenticado durante la vida de la aplicación.
    /// Es deliberadamente simple (estático) porque se trata de una aplicación de escritorio
    /// de un solo usuario por instancia; no existen sesiones concurrentes que administrar.
    /// </summary>
    public static class SesionActual
    {
        public static Usuario? UsuarioAutenticado { get; private set; }

        public static void IniciarSesion(Usuario usuario) => UsuarioAutenticado = usuario;

        /// <summary>
        /// Limpia la sesión. Se debe llamar SIEMPRE al cerrar sesión, para que ninguna
        /// pantalla protegida pueda seguir operando como si el usuario siguiera autenticado.
        /// </summary>
        public static void CerrarSesion() => UsuarioAutenticado = null;

        public static bool EsAdministrador => UsuarioAutenticado?.Rol == Helpers.Roles.Administrador;
    }
}
