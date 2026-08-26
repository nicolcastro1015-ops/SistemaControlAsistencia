using System;
using Microsoft.Data.SqlClient;
using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    /// <summary>
    /// PRUEBAS DE INTEGRACIÓN (sección 34 del enunciado).
    ///
    /// A diferencia de las pruebas unitarias anteriores (que usan repositorios falsos en memoria),
    /// estas pruebas se conectan a una instancia REAL de SQL Server y ejecutan el flujo completo:
    ///
    ///   Servicio -> Repositorio -> Base de datos -> Repositorio -> Servicio
    ///
    /// Requisitos para ejecutarlas:
    ///   1. Haber ejecutado BaseDatosAsistencia.txt en SQL Server (ver README).
    ///   2. Tener configurada la cadena de conexión en appsettings.json del proyecto de pruebas
    ///      (se copia automáticamente desde el proyecto App gracias a la referencia de proyecto,
    ///      pero si su instancia de SQL Server tiene otro nombre, ajuste appsettings.json).
    ///
    /// Si SQL Server no está disponible, estas pruebas fallarán con un mensaje de conexión claro
    /// en lugar de silenciarse: eso es intencional, para que el estudiante note que faltó levantar
    /// la base de datos antes de ejecutar "Ejecutar pruebas de integración".
    /// </summary>
    public class IntegracionBaseDatosTests
    {
        private static bool BaseDeDatosDisponible()
        {
            try
            {
                using SqlConnection conexion = DatabaseHelper.CrearConexion();
                conexion.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Fact]
        public void Login_ContraBaseDeDatosReal_AutenticaAlAdministradorDeDemostracion()
        {
            if (!BaseDeDatosDisponible())
            {
                Assert.Fail("SQL Server no está disponible. Ejecute BaseDatosAsistencia.txt y revise appsettings.json antes de correr esta prueba de integración.");
                return;
            }

            var servicio = new AutenticacionService(new UsuarioRepository());
            var resultado = servicio.Login("nicol.castro@empresa.cl", "Admin123!");

            Assert.True(resultado.Exito, resultado.Mensaje);
            Assert.Equal(Roles.Administrador, resultado.Datos!.Rol);
        }

        [Fact]
        public void CrearYConsultarUsuario_ContraBaseDeDatosReal_PersisteCorrectamente()
        {
            if (!BaseDeDatosDisponible())
            {
                Assert.Fail("SQL Server no está disponible. Ejecute BaseDatosAsistencia.txt y revise appsettings.json antes de correr esta prueba de integración.");
                return;
            }

            var repositorio = new UsuarioRepository();
            var servicio = new UsuarioService(repositorio);

            string correoUnico = $"prueba.integracion.{Guid.NewGuid():N}@empresa.cl";
            Usuario nuevo = new Usuario
            {
                Nombre = "Prueba",
                Apellidos = "Integracion",
                Correo = correoUnico,
                Rol = Roles.Empleado,
                Estado = true
            };

            var resultadoCreacion = servicio.CrearUsuario(nuevo, "ClavePrueba1!");
            Assert.True(resultadoCreacion.Exito, resultadoCreacion.Mensaje);

            Usuario? recuperado = repositorio.ObtenerPorCorreo(correoUnico);
            Assert.NotNull(recuperado);
            Assert.Equal("Prueba", recuperado!.Nombre);

            // Limpieza: se desactiva (eliminación lógica) para no dejar basura en la base de demostración.
            servicio.EliminarUsuario(recuperado.IdUsuario);
        }

        [Fact]
        public void RegistrarAsistencia_ContraBaseDeDatosReal_QuedaAsociadaAlUsuario()
        {
            if (!BaseDeDatosDisponible())
            {
                Assert.Fail("SQL Server no está disponible. Ejecute BaseDatosAsistencia.txt y revise appsettings.json antes de correr esta prueba de integración.");
                return;
            }

            var usuarioRepositorio = new UsuarioRepository();
            var asistenciaRepositorio = new AsistenciaRepository();
            var asistenciaServicio = new AsistenciaService(asistenciaRepositorio);

            Usuario? admin = usuarioRepositorio.ObtenerPorCorreo("nicol.castro@empresa.cl");
            Assert.NotNull(admin);

            Asistencia? ultimoAntes = asistenciaServicio.ObtenerUltimoRegistro(admin!.IdUsuario);

            // Se usa una fecha distinta a "hoy" para no chocar con los datos de demostración
            // ni con ejecuciones repetidas de esta misma prueba en el mismo día.
            DateTime fechaDePrueba = new DateTime(2020, 1, 1, 8, 0, 0);
            var resultado = asistenciaServicio.RegistrarEntrada(admin.IdUsuario, fechaDePrueba);

            Assert.True(resultado.Exito, resultado.Mensaje);

            Asistencia? ultimoDespues = asistenciaServicio.ObtenerUltimoRegistro(admin.IdUsuario);
            Assert.NotNull(ultimoDespues);
            Assert.NotEqual(ultimoAntes?.IdAsistencia, ultimoDespues!.IdAsistencia);
        }
    }
}
