using System;
using System.Linq;
using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Services
{
    /// <summary>
    /// Lógica de negocio de CA-01: registrar entrada, registrar salida, obtener el último
    /// registro del trabajador y validar duplicados/inconsistencias (sección 12 del enunciado).
    ///
    /// Recibe "ahora" (DateTime) como parámetro en vez de usar DateTime.Now internamente,
    /// para que las pruebas unitarias puedan simular cualquier fecha/hora sin depender
    /// del reloj real del computador (sección 43 del enunciado).
    /// </summary>
    public class AsistenciaService
    {
        private readonly IAsistenciaRepository _asistenciaRepository;

        public AsistenciaService(IAsistenciaRepository asistenciaRepository)
        {
            _asistenciaRepository = asistenciaRepository;
        }

        public ResultadoOperacion RegistrarEntrada(int idUsuario, DateTime ahora)
        {
            var registrosHoy = _asistenciaRepository.ListarPorUsuarioYFecha(idUsuario, ahora.Date);

            if (registrosHoy.Any(r => r.TipoRegistro == TipoRegistro.Entrada))
                return ResultadoOperacion.Fallo("Ya registró su entrada el día de hoy.");

            _asistenciaRepository.Registrar(new Asistencia
            {
                IdUsuario = idUsuario,
                TipoRegistro = TipoRegistro.Entrada,
                FechaHora = ahora
            });

            return ResultadoOperacion.Ok("Entrada registrada correctamente.");
        }

        public ResultadoOperacion RegistrarSalida(int idUsuario, DateTime ahora)
        {
            var registrosHoy = _asistenciaRepository.ListarPorUsuarioYFecha(idUsuario, ahora.Date);

            bool tieneEntradaHoy = registrosHoy.Any(r => r.TipoRegistro == TipoRegistro.Entrada);
            if (!tieneEntradaHoy)
                return ResultadoOperacion.Fallo("No puede registrar una salida sin haber registrado previamente su entrada.");

            bool tieneSalidaHoy = registrosHoy.Any(r => r.TipoRegistro == TipoRegistro.Salida);
            if (tieneSalidaHoy)
                return ResultadoOperacion.Fallo("Ya registró su salida el día de hoy.");

            _asistenciaRepository.Registrar(new Asistencia
            {
                IdUsuario = idUsuario,
                TipoRegistro = TipoRegistro.Salida,
                FechaHora = ahora
            });

            return ResultadoOperacion.Ok("Salida registrada correctamente.");
        }

        public Asistencia? ObtenerUltimoRegistro(int idUsuario) =>
            _asistenciaRepository.ObtenerUltimoRegistro(idUsuario);

        /// <summary>Indica qué botones deben estar habilitados hoy para el usuario (sección 42).</summary>
        public (bool puedeMarcarEntrada, bool puedeMarcarSalida) ObtenerEstadoBotones(int idUsuario, DateTime ahora)
        {
            var registrosHoy = _asistenciaRepository.ListarPorUsuarioYFecha(idUsuario, ahora.Date);
            bool tieneEntradaHoy = registrosHoy.Any(r => r.TipoRegistro == TipoRegistro.Entrada);
            bool tieneSalidaHoy = registrosHoy.Any(r => r.TipoRegistro == TipoRegistro.Salida);

            bool puedeMarcarEntrada = !tieneEntradaHoy;
            bool puedeMarcarSalida = tieneEntradaHoy && !tieneSalidaHoy;

            return (puedeMarcarEntrada, puedeMarcarSalida);
        }

        public System.Collections.Generic.List<Asistencia> ObtenerActividadReciente(int cantidad) =>
            _asistenciaRepository.ListarRecientes(cantidad);

        public System.Collections.Generic.List<Asistencia> ListarPorFecha(DateTime fecha) =>
            _asistenciaRepository.ListarPorFecha(fecha);
    }
}
