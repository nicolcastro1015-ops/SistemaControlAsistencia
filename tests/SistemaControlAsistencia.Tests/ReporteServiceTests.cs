using System;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;
using SistemaControlAsistencia.Tests.Fakes;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    public class ReporteServiceTests
    {
        private readonly FakeUsuarioRepository _usuarioRepositorio = new();
        private readonly FakeAsistenciaRepository _asistenciaRepositorio = new();
        private readonly ReporteService _servicio;
        private readonly DateTime _fecha = new DateTime(2026, 8, 24);

        public ReporteServiceTests()
        {
            _servicio = new ReporteService(_usuarioRepositorio, _asistenciaRepositorio);

            CrearUsuario(1, "Ana", "Torres");
            CrearUsuario(2, "Pedro", "Soto");
            CrearUsuario(3, "Carlos", "Fuentes");
        }

        private void CrearUsuario(int id, string nombre, string apellidos)
        {
            _usuarioRepositorio.Usuarios.Add(new Usuario
            {
                IdUsuario = id,
                Nombre = nombre,
                Apellidos = apellidos,
                Correo = $"{nombre}.{apellidos}@empresa.cl".ToLower(),
                ContrasenaHash = PasswordHasher.Generar("Clave123!"),
                Rol = Roles.Empleado,
                Estado = true
            });
        }

        private void Registrar(int idUsuario, TipoRegistro tipo, TimeSpan hora) =>
            _asistenciaRepositorio.Registrar(new Asistencia
            {
                IdUsuario = idUsuario,
                TipoRegistro = tipo,
                FechaHora = _fecha.Add(hora)
            });

        [Fact]
        public void ObtenerAtrasos_SoloIncluyeEntradasDespuesDeLas0930()
        {
            Registrar(1, TipoRegistro.Entrada, new TimeSpan(9, 29, 0));  // puntual
            Registrar(2, TipoRegistro.Entrada, new TimeSpan(9, 30, 0));  // puntual (límite exacto)
            Registrar(3, TipoRegistro.Entrada, new TimeSpan(9, 31, 0));  // atraso

            var atrasos = _servicio.ObtenerAtrasos(_fecha);

            Assert.Single(atrasos);
            Assert.Equal(3, atrasos[0].IdUsuario);
        }

        [Fact]
        public void ObtenerSalidasAnticipadas_SoloIncluyeSalidasAntesDeLas1730()
        {
            Registrar(1, TipoRegistro.Salida, new TimeSpan(17, 29, 0)); // anticipada
            Registrar(2, TipoRegistro.Salida, new TimeSpan(17, 30, 0)); // normal (límite exacto)
            Registrar(3, TipoRegistro.Salida, new TimeSpan(17, 31, 0)); // normal

            var salidas = _servicio.ObtenerSalidasAnticipadas(_fecha);

            Assert.Single(salidas);
            Assert.Equal(1, salidas[0].IdUsuario);
        }

        [Fact]
        public void ObtenerInasistencias_DetectaAUsuariosActivosSinNingunRegistro()
        {
            // Ana y Pedro marcan asistencia; Carlos no tiene ningún registro ese día.
            Registrar(1, TipoRegistro.Entrada, new TimeSpan(9, 0, 0));
            Registrar(1, TipoRegistro.Salida, new TimeSpan(17, 30, 0));
            Registrar(2, TipoRegistro.Entrada, new TimeSpan(9, 0, 0));
            Registrar(2, TipoRegistro.Salida, new TimeSpan(17, 30, 0));

            var inasistencias = _servicio.ObtenerInasistencias(_fecha);

            Assert.Single(inasistencias);
            Assert.Equal(3, inasistencias[0].IdUsuario);
            Assert.Equal("Carlos", inasistencias[0].Nombre);
        }

        [Fact]
        public void ObtenerInasistencias_NoIncluyeUsuariosInactivos()
        {
            _usuarioRepositorio.Usuarios.Add(new Usuario
            {
                IdUsuario = 4,
                Nombre = "Jorge",
                Apellidos = "Diaz",
                Correo = "jorge.diaz@empresa.cl",
                ContrasenaHash = PasswordHasher.Generar("Clave123!"),
                Rol = Roles.Empleado,
                Estado = false
            });

            Registrar(1, TipoRegistro.Entrada, new TimeSpan(9, 0, 0));
            Registrar(1, TipoRegistro.Salida, new TimeSpan(17, 30, 0));
            Registrar(2, TipoRegistro.Entrada, new TimeSpan(9, 0, 0));
            Registrar(2, TipoRegistro.Salida, new TimeSpan(17, 30, 0));

            var inasistencias = _servicio.ObtenerInasistencias(_fecha);

            // Carlos (activo, sin registro) sí aparece; Jorge (inactivo, sin registro) NO debe aparecer.
            Assert.Single(inasistencias);
            Assert.Equal("Carlos", inasistencias[0].Nombre);
        }
    }
}
