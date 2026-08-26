namespace SistemaControlAsistencia.App.Models
{
    /// <summary>
    /// Entidad Usuario. Corresponde a la tabla USUARIO de la base de datos ControlAsistencia.
    /// </summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Nunca se guarda la contraseña en texto plano.
        /// Formato: "{iteraciones}.{saltBase64}.{hashBase64}" (ver Helpers/PasswordHasher.cs)
        /// </summary>
        public string ContrasenaHash { get; set; } = string.Empty;

        /// <summary>
        /// Se mantiene como string (y no como enum) porque así fue definido explícitamente
        /// en el enunciado del proyecto (sección 21). Los valores permitidos se validan
        /// contra las constantes de Helpers/Roles.cs y contra el CHECK de la tabla USUARIO.
        /// </summary>
        public string Rol { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();
    }
}
