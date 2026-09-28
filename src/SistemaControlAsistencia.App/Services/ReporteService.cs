using System;
using System.Collections.Generic;
using System.Linq;
using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Services
{
    // Fila de resultado para los reportes RE-01 y RE-02.
    public class FilaReporteHorario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }

    // Fila de resultado para el reporte RE-03 (sin Hora: una inasistencia no tiene una).
    public class FilaInasistencia
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }

    // Lógica de negocio de RE-01 (atrasos), RE-02 (salidas anticipadas) y RE-03
    // (inasistencias). Las reglas horarias se evalúan siempre a través de ReglasHorarias,
    // nunca comparando "09:30" directamente acá.
    public class ReporteService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAsistenciaRepository _asistenciaRepository;

        public ReporteService(IUsuarioRepository usuarioRepository, IAsistenciaRepository asistenciaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _asistenciaRepository = asistenciaRepository;
        }

        // Entradas después de las 09:30 (09:30 exacto NO es atraso).
        public List<FilaReporteHorario> ObtenerAtrasos(DateTime fecha)
        {
            return _asistenciaRepository.ListarPorFecha(fecha)
                .Where(a => a.TipoRegistro == TipoRegistro.Entrada && ReglasHorarias.EsAtraso(a.FechaHora.TimeOfDay))
                .Select(a => new FilaReporteHorario
                {
                    IdUsuario = a.IdUsuario,
                    Nombre = a.NombreUsuario ?? string.Empty,
                    Apellidos = a.ApellidosUsuario ?? string.Empty,
                    Fecha = a.FechaHora.Date,
                    Hora = a.FechaHora.TimeOfDay
                })
                .OrderBy(f => f.Apellidos)
                .ToList();
        }

        // Salidas antes de las 17:30 (17:30 exacto NO es anticipada).
        public List<FilaReporteHorario> ObtenerSalidasAnticipadas(DateTime fecha)
        {
            return _asistenciaRepository.ListarPorFecha(fecha)
                .Where(a => a.TipoRegistro == TipoRegistro.Salida && ReglasHorarias.EsSalidaAnticipada(a.FechaHora.TimeOfDay))
                .Select(a => new FilaReporteHorario
                {
                    IdUsuario = a.IdUsuario,
                    Nombre = a.NombreUsuario ?? string.Empty,
                    Apellidos = a.ApellidosUsuario ?? string.Empty,
                    Fecha = a.FechaHora.Date,
                    Hora = a.FechaHora.TimeOfDay
                })
                .OrderBy(f => f.Apellidos)
                .ToList();
        }

        // Compara usuarios activos contra quienes sí registraron algo ese día; no asume que
        // "sin filas" ya se pueda mostrar directamente.
        public List<FilaInasistencia> ObtenerInasistencias(DateTime fecha)
        {
            List<Usuario> activos = _usuarioRepository.ListarTodos().Where(u => u.Estado).ToList();
            List<Asistencia> registrosDelDia = _asistenciaRepository.ListarPorFecha(fecha);

            HashSet<int> usuariosConRegistro = registrosDelDia.Select(a => a.IdUsuario).ToHashSet();

            return activos
                .Where(u => !usuariosConRegistro.Contains(u.IdUsuario))
                .Select(u => new FilaInasistencia
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Fecha = fecha.Date
                })
                .OrderBy(f => f.Apellidos)
                .ToList();
        }
    }
}
