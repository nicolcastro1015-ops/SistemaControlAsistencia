using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;
using SistemaControlAsistencia.Tests.Fakes;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    public class AutenticacionServiceTests
    {
        private readonly FakeUsuarioRepository _repositorio = new();
        private readonly AutenticacionService _servicio;

        public AutenticacionServiceTests()
        {
            _servicio = new AutenticacionService(_repositorio);

            _repositorio.Crear(new Usuario
            {
                Nombre = "Ana",
                Apellidos = "Torres",
                Correo = "ana.torres@empresa.cl",
                ContrasenaHash = PasswordHasher.Generar("Clave123!"),
                Rol = Roles.Empleado,
                Estado = true
            });

            _repositorio.Crear(new Usuario
            {
                Nombre = "Jorge",
                Apellidos = "Diaz",
                Correo = "jorge.diaz@empresa.cl",
                ContrasenaHash = PasswordHasher.Generar("Clave123!"),
                Rol = Roles.Empleado,
                Estado = false
            });
        }

        [Fact]
        public void Login_ConCredencialesCorrectas_RetornaExito()
        {
            var resultado = _servicio.Login("ana.torres@empresa.cl", "Clave123!");

            Assert.True(resultado.Exito);
            Assert.Equal("ana.torres@empresa.cl", resultado.Datos!.Correo);
        }

        [Fact]
        public void Login_ConContrasenaIncorrecta_RetornaMensajeGenerico()
        {
            var resultado = _servicio.Login("ana.torres@empresa.cl", "ClaveIncorrecta");

            Assert.False(resultado.Exito);
            Assert.Equal("Correo o contraseña incorrectos.", resultado.Mensaje);
        }

        [Fact]
        public void Login_ConCorreoInexistente_RetornaMismoMensajeQueContrasenaIncorrecta()
        {
            // Por seguridad, no se debe revelar si el problema fue el correo o la contraseña.
            var resultado = _servicio.Login("no.existe@empresa.cl", "Clave123!");

            Assert.False(resultado.Exito);
            Assert.Equal("Correo o contraseña incorrectos.", resultado.Mensaje);
        }

        [Fact]
        public void Login_ConUsuarioInactivo_RetornaMensajeDeInactivo()
        {
            var resultado = _servicio.Login("jorge.diaz@empresa.cl", "Clave123!");

            Assert.False(resultado.Exito);
            Assert.Equal("El usuario se encuentra inactivo. Contacte al administrador.", resultado.Mensaje);
        }

        [Fact]
        public void Login_ConCamposVacios_RetornaMensajeDeCamposObligatorios()
        {
            var resultado = _servicio.Login("", "");

            Assert.False(resultado.Exito);
            Assert.Equal("Debe ingresar correo y contraseña.", resultado.Mensaje);
        }
    }
}
