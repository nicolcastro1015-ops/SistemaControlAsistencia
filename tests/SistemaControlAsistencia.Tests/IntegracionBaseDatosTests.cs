using System;
using Microsoft.Data.SqlClient;
using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    // PRUEBAS DE INTEGRACIÓN: a diferencia de las anteriores (repositorios falsos en memoria),
    // estas se conectan a una instancia REAL de SQL Server y ejecutan el flujo completo:
    // Servicio -> Repositorio -> Base de datos -> Repositorio -> Servicio.
    //
    // Requisitos: haber ejecutado BaseDatosAsistencia.txt en SQL Server, y tener configurada la
    // cadena de conexión en appsettings.json del proyecto de pruebas. Si SQL Server no está
    // disponible, fallan con un mensaje claro en vez de saltarse en silencio.
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

            // Limpieza: DELETE físico y permanente. -1 representa a "otro administrador"
            // ejecutando la limpieza (la única condición del método es que no sea el mismo usuario).
            servicio.EliminarUsuario(recuperado.IdUsuario, idUsuarioQueEjecutaLaAccion: -1);
        }

        [Fact]
        public void RegistrarAsistencia_ContraBaseDeDatosReal_QuedaAsociadaAlUsuario()
        {
            if (!BaseDeDatosDisponible())
            {
                Assert.Fail("SQL Server no está disponible. Ejecute BaseDatosAsistencia.txt y revise appsettings.json antes de correr esta prueba de integración.");
                return;
            }

            var usuarioServicio = new UsuarioService(new UsuarioRepository());
            var asistenciaServicio = new AsistenciaService(new AsistenciaRepository());

            // La prueba crea su PROPIO usuario temporal (correo único), así no choca con los
            // registros de asistencia de cuentas reales ni con ejecuciones anteriores de la
            // misma prueba (antes fallaba con "Ya registró su entrada el día de hoy.").
            string correoUnico = $"prueba.asistencia.{Guid.NewGuid():N}@empresa.cl";
            var creado = usuarioServicio.CrearUsuario(new Usuario
            {
                Nombre = "Prueba",
                Apellidos = "Asistencia",
                Correo = correoUnico,
                Rol = Roles.Empleado,
                Estado = true
            }, "ClavePrueba1!");
            Assert.True(creado.Exito, creado.Mensaje);
            int idUsuario = creado.Datos!.IdUsuario;

            try
            {
                // Fecha controlada (no depende del día en que se ejecute la prueba).
                DateTime fechaDePrueba = new DateTime(2020, 1, 1, 8, 0, 0);
                var resultado = asistenciaServicio.RegistrarEntrada(idUsuario, fechaDePrueba);
                Assert.True(resultado.Exito, resultado.Mensaje);

                Asistencia? ultimo = asistenciaServicio.ObtenerUltimoRegistro(idUsuario);
                Assert.NotNull(ultimo);
                Assert.Equal(idUsuario, ultimo!.IdUsuario);
                Assert.Equal(TipoRegistro.Entrada, ultimo.TipoRegistro);
                Assert.Equal(fechaDePrueba, ultimo.FechaHora);
            }
            finally
            {
                // Limpieza: DELETE físico; el ON DELETE CASCADE también borra su asistencia,
                // así que la base queda igual que antes y la prueba se puede repetir.
                usuarioServicio.EliminarUsuario(idUsuario, idUsuarioQueEjecutaLaAccion: -1);
            }
        }
    }
}
