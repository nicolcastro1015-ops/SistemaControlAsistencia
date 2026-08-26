using System.Text.RegularExpressions;

namespace SistemaControlAsistencia.App.Helpers
{
    /// <summary>
    /// Validaciones reutilizadas por UsuarioService y las pantallas de Login / Usuarios.
    /// </summary>
    public static class Validaciones
    {
        private static readonly Regex RegexCorreo = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        public static bool EsCorreoValido(string? correo) =>
            !string.IsNullOrWhiteSpace(correo) && RegexCorreo.IsMatch(correo.Trim());

        public static bool EsTextoValido(string? texto, int minimo = 2) =>
            !string.IsNullOrWhiteSpace(texto) && texto.Trim().Length >= minimo;

        public static bool EsContrasenaValida(string? contrasena) =>
            !string.IsNullOrWhiteSpace(contrasena) && contrasena.Length >= 6;
    }
}
