using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Services
{
    // Mantiene en memoria el usuario que inició sesión. Es estática porque la aplicación de
    // escritorio solo maneja un usuario a la vez. LoginWindow llama a IniciarSesion(); las
    // demás ventanas leen UsuarioAutenticado para saber quién está usando el sistema.
    public static class SesionActual
    {
        public static Usuario? UsuarioAutenticado { get; private set; }

        public static void IniciarSesion(Usuario usuario) => UsuarioAutenticado = usuario;

        // Se llama siempre al cerrar sesión, para que ninguna ventana anterior siga pensando
        // que hay alguien autenticado.
        public static void CerrarSesion() => UsuarioAutenticado = null;

        // Verificación real de rol (no solo ocultar botones): AdminMainWindow la revisa en su
        // constructor antes de dejarse abrir.
        public static bool EsAdministrador => UsuarioAutenticado?.Rol == Helpers.Roles.Administrador;
    }
}
