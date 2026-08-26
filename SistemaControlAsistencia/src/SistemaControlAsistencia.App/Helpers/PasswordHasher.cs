using System;
using System.Security.Cryptography;

namespace SistemaControlAsistencia.App.Helpers
{
    /// <summary>
    /// Genera y verifica hashes de contraseña usando PBKDF2-HMACSHA256 (Rfc2898DeriveBytes),
    /// disponible de forma nativa en .NET sin necesidad de paquetes NuGet adicionales.
    ///
    /// Nunca se guarda la contraseña en texto plano. El formato de almacenamiento es:
    ///   "{iteraciones}.{saltEnBase64}.{hashEnBase64}"
    ///
    /// Guardar las iteraciones junto con el hash permite, en el futuro, aumentar el costo
    /// computacional (más iteraciones) sin invalidar las contraseñas ya creadas con un valor menor.
    /// </summary>
    public static class PasswordHasher
    {
        private const int Iteraciones = 100_000;
        private const int TamanoSalBytes = 16;
        private const int TamanoHashBytes = 32;

        public static string Generar(string contrasenaPlano)
        {
            if (string.IsNullOrEmpty(contrasenaPlano))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contrasenaPlano));

            byte[] sal = RandomNumberGenerator.GetBytes(TamanoSalBytes);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(contrasenaPlano, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHashBytes);

            return $"{Iteraciones}.{Convert.ToBase64String(sal)}.{Convert.ToBase64String(hash)}";
        }

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
