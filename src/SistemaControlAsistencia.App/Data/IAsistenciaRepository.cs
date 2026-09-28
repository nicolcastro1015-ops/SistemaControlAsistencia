using System;
using System.Collections.Generic;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Data
{
    // Abstracción del acceso a datos de ASISTENCIA. Igual que IUsuarioRepository, permite
    // probar AsistenciaService y ReporteService con FakeAsistenciaRepository, sin SQL Server.
    public interface IAsistenciaRepository
    {
        void Registrar(Asistencia asistencia);

        // Usado por AsistenciaService para detectar doble entrada, doble salida, etc.
        List<Asistencia> ListarPorUsuarioYFecha(int idUsuario, DateTime fecha);

        Asistencia? ObtenerUltimoRegistro(int idUsuario);

        // Base de la que ReporteService calcula RE-01, RE-02 y RE-03.
        List<Asistencia> ListarPorFecha(DateTime fecha);

        List<Asistencia> ListarPorUsuario(int idUsuario);

        // Usado por el Dashboard para "Actividad reciente".
        List<Asistencia> ListarRecientes(int cantidad);
    }
}
