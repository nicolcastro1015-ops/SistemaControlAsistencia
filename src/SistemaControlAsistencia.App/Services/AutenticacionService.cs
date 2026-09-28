using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Services
{
    // Responsable de validar credenciales, recuperar el usuario, validar su estado y
    // controlar la autenticación (CA-01). LoginWindow es quien llama a Login().
    public class AutenticacionService
    {
        // Por interfaz (no UsuarioRepository directo) para poder probar con un repositorio
        // falso en memoria, ver AutenticacionServiceTests.cs.
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

            // Mismo mensaje si el correo no existe o si la contraseña es incorrecta: no se
            // debe revelar cuál de los dos falló.
            if (usuario == null || !PasswordHasher.Verificar(contrasena, usuario.ContrasenaHash))
                return ResultadoOperacion<Usuario>.Fallo("Correo o contraseña incorrectos.");

            if (!usuario.Estado)
                return ResultadoOperacion<Usuario>.Fallo("El usuario se encuentra inactivo. Contacte al administrador.");

            return ResultadoOperacion<Usuario>.Ok("Inicio de sesión correcto.", usuario);
        }
    }
}
