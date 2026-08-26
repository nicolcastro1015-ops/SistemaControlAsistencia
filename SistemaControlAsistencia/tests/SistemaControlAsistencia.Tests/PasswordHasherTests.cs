using SistemaControlAsistencia.App.Helpers;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Verificar_ConLaMismaContrasena_RetornaTrue()
        {
            string hash = PasswordHasher.Generar("MiClave123!");
            Assert.True(PasswordHasher.Verificar("MiClave123!", hash));
        }

        [Fact]
        public void Verificar_ConContrasenaDistinta_RetornaFalse()
        {
            string hash = PasswordHasher.Generar("MiClave123!");
            Assert.False(PasswordHasher.Verificar("OtraClave456!", hash));
        }

        [Fact]
        public void Generar_ProduceHashesDistintosParaLaMismaContrasena()
        {
            // La sal aleatoria garantiza que dos hashes de la misma contraseña nunca sean iguales.
            string hash1 = PasswordHasher.Generar("Repetida1!");
            string hash2 = PasswordHasher.Generar("Repetida1!");
            Assert.NotEqual(hash1, hash2);
        }
    }
}
