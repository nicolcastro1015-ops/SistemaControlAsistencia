using System;
using System.Collections.Generic;
using System.Linq;
using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.Tests.Fakes
{
    /// <summary>Repositorio en memoria usado por las pruebas de AsistenciaService y ReporteService.</summary>
    public class FakeAsistenciaRepository : IAsistenciaRepository
    {
        public List<Asistencia> Registros { get; } = new();
        private int _siguienteId = 1;

        public void Registrar(Asistencia asistencia)
        {
            asistencia.IdAsistencia = _siguienteId++;
            Registros.Add(asistencia);
        }

        public List<Asistencia> ListarPorUsuarioYFecha(int idUsuario, DateTime fecha) =>
            Registros.Where(a => a.IdUsuario == idUsuario && a.FechaHora.Date == fecha.Date)
                     .OrderBy(a => a.FechaHora).ToList();

        public Asistencia? ObtenerUltimoRegistro(int idUsuario) =>
            Registros.Where(a => a.IdUsuario == idUsuario).OrderByDescending(a => a.FechaHora).FirstOrDefault();

        public List<Asistencia> ListarPorFecha(DateTime fecha) =>
            Registros.Where(a => a.FechaHora.Date == fecha.Date).OrderBy(a => a.FechaHora).ToList();

        public List<Asistencia> ListarPorUsuario(int idUsuario) =>
            Registros.Where(a => a.IdUsuario == idUsuario).OrderByDescending(a => a.FechaHora).ToList();

        public List<Asistencia> ListarRecientes(int cantidad) =>
            Registros.OrderByDescending(a => a.FechaHora).Take(cantidad).ToList();
    }
}
