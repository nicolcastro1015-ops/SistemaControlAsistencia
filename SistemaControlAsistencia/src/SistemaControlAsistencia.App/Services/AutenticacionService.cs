using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Services
{
    /// <summary>
    /// Responsable de validar credenciales, recuperar el usuario, validar su estado
    /// y controlar la autenticación (CA-01 / sección 5 del enunciado).
    /// </summary>
    public class AutenticacionService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AutenticacionService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public ResultadoOperacion<Usuario> Login(string? correo, string? contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
                return ResultadoOperacion<Usuario>.Fallo("Debe ingresar correo y contraseña.");

            Usuario? usuario = _usuarioRepository.ObtenerPorCorreo(correo.Trim());

            // Por seguridad, el mismo mensaje se usa tanto si el correo no existe
            // como si la contraseña es incorrecta: no se debe revelar cuál de los dos falló.
            if (usuario == null || !PasswordHasher.Verificar(contrasena, usuario.ContrasenaHash))
                return ResultadoOperacion<Usuario>.Fallo("Correo o contraseña incorrectos.");

            if (!usuario.Estado)
                return ResultadoOperacion<Usuario>.Fallo("El usuario se encuentra inactivo. Contacte al administrador.");

            return ResultadoOperacion<Usuario>.Ok("Inicio de sesión correcto.", usuario);
        }
    }
}
