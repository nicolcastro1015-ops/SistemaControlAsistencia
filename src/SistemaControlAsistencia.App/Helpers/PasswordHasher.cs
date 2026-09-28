using System;
using System.Security.Cryptography;

namespace SistemaControlAsistencia.App.Helpers
{
    // Genera y verifica hashes de contraseña con PBKDF2-HMACSHA256 (Rfc2898DeriveBytes),
    // incluido de forma nativa en .NET. Nunca se guarda la contraseña en texto plano: se
    // guarda "{iteraciones}.{saltBase64}.{hashBase64}". UsuarioService llama a Generar() antes
    // de guardar; AutenticacionService llama a Verificar() al validar el login.
    public static class PasswordHasher
    {
        private const int Iteraciones = 100_000;
        private const int TamanoSalBytes = 16;
        private const int TamanoHashBytes = 32;

        // La sal es aleatoria por contraseña: dos usuarios con la misma contraseña terminan
        // con hashes distintos en la base de datos.
        public static string Generar(string contrasenaPlano)
        {
            if (string.IsNullOrEmpty(contrasenaPlano))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contrasenaPlano));

            byte[] sal = RandomNumberGenerator.GetBytes(TamanoSalBytes);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(contrasenaPlano, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHashBytes);

            return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
        }

        // Recalcula el hash con la misma sal guardada y compara con FixedTimeEquals (tiempo
        // constante), para no dar pistas de temporización a un posible atacante.
        public static bool Verificar(string contrasenaPlano, string hashAlmacenado)
        {
            if (string.IsNullOrEmpty(contrasenaPlano) || string.IsNullOrEmpty(hashAlmacenado))
                return false;

            string[] partes = hashAlmacenado.Split('.');
            if (partes.Length != 3)
                return false;

            if (!int.TryParse(partes[0], out int iteraciones))
                return false;

            byte[] sal;
            byte[] hashEsperado;
            try
            {
                sal = Convert.FromBase64String(partes[1]);
                hashEsperado = Convert.FromBase64String(partes[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] hashCalculado = Rfc2898DeriveBytes.Pbkdf2(contrasenaPlano, sal, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
    }
}
