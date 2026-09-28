using System.Text.RegularExpressions;

namespace SistemaControlAsistencia.App.Helpers
{
    // Validaciones genéricas de texto, reutilizadas por UsuarioService al crear/editar (GU-01/GU-02).
    public static class Validaciones
    {
        // Formato básico de correo (algo@algo.algo); no valida contra dominios reales.
        private static readonly Regex RegexCorreo = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        public static bool EsCorreoValido(string? correo) =>
            !string.IsNullOrWhiteSpace(correo) && RegexCorreo.IsMatch(correo.Trim());

        public static bool EsTextoValido(string? texto, int minimo = 2) =>
            !string.IsNullOrWhiteSpace(texto) && texto.Trim().Length >= minimo;

        // Solo valida longitud; la seguridad real la da PasswordHasher al guardarla.
        public static bool EsContrasenaValida(string? contrasena) =>
            !string.IsNullOrWhiteSpace(contrasena) && contrasena.Length >= 6;
    }
}
