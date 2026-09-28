namespace SistemaControlAsistencia.App.Models
{
    // Representa a un trabajador (Administrador o Empleado). Corresponde a la tabla USUARIO
    // de la base de datos, leída/escrita por UsuarioRepository.cs.
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;

        // Debe ser único (UNIQUE en SQL Server). Se usa como identificador de login.
        public string Correo { get; set; } = string.Empty;

        // Nunca es la contraseña en texto plano: es el resultado de PasswordHasher.Generar().
        public string ContrasenaHash { get; set; } = string.Empty;

        // Solo puede ser "Administrador" o "Empleado" (validado en Roles.cs y con CHECK en SQL Server).
        public string Rol { get; set; } = string.Empty;

        // true = Activo, false = Inactivo. Un usuario Inactivo no puede iniciar sesión.
        public bool Estado { get; set; } = true;

        public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();
    }
}
