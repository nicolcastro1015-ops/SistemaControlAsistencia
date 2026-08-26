using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    /// <summary>
    /// Pruebas de la verificación de rol que usan las ventanas (AdminMainWindow) antes de
    /// ejecutar cualquier acción administrativa. La UI esconde botones, pero la comprobación
    /// real ocurre aquí: SesionActual.EsAdministrador, evaluada también dentro del constructor
    /// de AdminMainWindow (sección 6 del enunciado: "no basta con esconder botones").
    /// </summary>
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
