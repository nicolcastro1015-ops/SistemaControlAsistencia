using System;
using SistemaControlAsistencia.App.Services;
using SistemaControlAsistencia.Tests.Fakes;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    public class AsistenciaServiceTests
    {
        private readonly FakeAsistenciaRepository _repositorio = new();
        private readonly AsistenciaService _servicio;
        private readonly DateTime _hoyALas9 = new DateTime(2026, 8, 24, 9, 0, 0);

        public AsistenciaServiceTests()
        {
            _servicio = new AsistenciaService(_repositorio);
        }

        [Fact]
        public void RegistrarEntrada_PrimeraVezEnElDia_SeRegistraCorrectamente()
        {
            var resultado = _servicio.RegistrarEntrada(idUsuario: 1, _hoyALas9);

            Assert.True(resultado.Exito);
            Assert.Equal("Entrada registrada correctamente.", resultado.Mensaje);
            Assert.Single(_repositorio.Registros);
        }

        [Fact]
        public void RegistrarEntrada_SiYaExisteEntradaHoy_RechazaLaDobleEntrada()
        {
            _servicio.RegistrarEntrada(1, _hoyALas9);
            var segundoIntento = _servicio.RegistrarEntrada(1, _hoyALas9.AddMinutes(5));

            Assert.False(segundoIntento.Exito);
            Assert.Equal("Ya registró su entrada el día de hoy.", segundoIntento.Mensaje);
            Assert.Single(_repositorio.Registros);
        }

        [Fact]
        public void RegistrarSalida_SinEntradaPrevia_SeRechaza()
        {
            var resultado = _servicio.RegistrarSalida(1, _hoyALas9.AddHours(8));

            Assert.False(resultado.Exito);
            Assert.Equal("No puede registrar una salida sin haber registrado previamente su entrada.", resultado.Mensaje);
        }

        [Fact]
        public void RegistrarSalida_ConEntradaPrevia_SeRegistraCorrectamente()
        {
            _servicio.RegistrarEntrada(1, _hoyALas9);
            var resultado = _servicio.RegistrarSalida(1, _hoyALas9.AddHours(8));

            Assert.True(resultado.Exito);
            Assert.Equal("Salida registrada correctamente.", resultado.Mensaje);
        }

        [Fact]
        public void RegistrarSalida_SiYaExisteSalidaHoy_SeRechaza()
        {
            _servicio.RegistrarEntrada(1, _hoyALas9);
            _servicio.RegistrarSalida(1, _hoyALas9.AddHours(8));
            var segundaSalida = _servicio.RegistrarSalida(1, _hoyALas9.AddHours(8).AddMinutes(1));

            Assert.False(segundaSalida.Exito);
            Assert.Equal("Ya registró su salida el día de hoy.", segundaSalida.Mensaje);
        }

        [Fact]
        public void ObtenerEstadoBotones_SinRegistros_SoloPermiteMarcarEntrada()
        {
            var (puedeEntrada, puedeSalida) = _servicio.ObtenerEstadoBotones(1, _hoyALas9);

            Assert.True(puedeEntrada);
            Assert.False(puedeSalida);
        }

        [Fact]
        public void ObtenerEstadoBotones_ConEntradaRegistrada_SoloPermiteMarcarSalida()
        {
            _servicio.RegistrarEntrada(1, _hoyALas9);
            var (puedeEntrada, puedeSalida) = _servicio.ObtenerEstadoBotones(1, _hoyALas9.AddHours(1));

            Assert.False(puedeEntrada);
            Assert.True(puedeSalida);
        }
    }
}
