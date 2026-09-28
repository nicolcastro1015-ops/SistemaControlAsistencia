using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    // Pruebas de la verificación de rol que usa AdminMainWindow antes de dejarse abrir. La
    // interfaz esconde botones, pero la comprobación real es esta: SesionActual.EsAdministrador.
    public class RolesYSesionTests
    {
        [Fact]
        public void EsAdministrador_ConUsuarioAdministrador_RetornaTrue()
        {
            SesionActual.IniciarSesion(new Usuario { Rol = Roles.Administrador, Estado = true });
            Assert.True(SesionActual.EsAdministrador);
            SesionActual.CerrarSesion();
        }

        [Fact]
        public void EsAdministrador_ConUsuarioEmpleado_RetornaFalse()
        {
            SesionActual.IniciarSesion(new Usuario { Rol = Roles.Empleado, Estado = true });
            Assert.False(SesionActual.EsAdministrador);
            SesionActual.CerrarSesion();
        }

        [Fact]
        public void CerrarSesion_LimpiaElUsuarioAutenticado()
        {
            SesionActual.IniciarSesion(new Usuario { Rol = Roles.Administrador, Estado = true });
            SesionActual.CerrarSesion();

            Assert.Null(SesionActual.UsuarioAutenticado);
            Assert.False(SesionActual.EsAdministrador);
        }
    }
}
