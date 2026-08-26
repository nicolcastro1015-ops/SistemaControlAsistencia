using System;
using System.Collections.Generic;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Data
{
    /// <summary>
    /// Abstracción del acceso a datos de ASISTENCIA. Igual que IUsuarioRepository,
    /// permite probar AsistenciaService y ReporteService con datos falsos en memoria.
    /// </summary>
    public interface IAsistenciaRepository
    {
        void Registrar(Asistencia asistencia);
        List<Asistencia> ListarPorUsuarioYFecha(int idUsuario, DateTime fecha);
        Asistencia? ObtenerUltimoRegistro(int idUsuario);
        List<Asistencia> ListarPorFecha(DateTime fecha);
        List<Asistencia> ListarPorUsuario(int idUsuario);
        List<Asistencia> ListarRecientes(int cantidad);
    }
}
