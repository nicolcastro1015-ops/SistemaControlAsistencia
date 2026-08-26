using System;

namespace SistemaControlAsistencia.App.Helpers
{
    /// <summary>
    /// Reglas horarias centralizadas del negocio (RE-01 y RE-02).
    /// Al mantener los límites en un único lugar, si la empresa cambia el horario
    /// laboral solo se modifica esta clase, sin tocar servicios, vistas ni pruebas.
    ///
    /// Las reglas trabajan sobre TimeSpan (hora del día) y no sobre DateTime.Now,
    /// para que las pruebas unitarias puedan verificar los límites exactos (09:29, 09:30, 09:31, etc.)
    /// sin depender del reloj real del computador (ver sección 43 del enunciado).
    /// </summary>
    public static class ReglasHorarias
    {
        /// <summary>Hora límite de entrada. 09:30 en punto se considera puntual, NO atraso.</summary>
        public static readonly TimeSpan HoraLimiteEntrada = new TimeSpan(9, 30, 0);

        /// <summary>Hora mínima de salida normal. 17:30 en punto se considera normal, NO anticipada.</summary>
        public static readonly TimeSpan HoraMinimaSalidaNormal = new TimeSpan(17, 30, 0);

        /// <summary>true si la hora de entrada es estrictamente posterior a 09:30.</summary>
        public static bool EsAtraso(TimeSpan horaEntrada) => horaEntrada > HoraLimiteEntrada;

        /// <summary>true si la hora de salida es estrictamente anterior a 17:30.</summary>
        public static bool EsSalidaAnticipada(TimeSpan horaSalida) => horaSalida < HoraMinimaSalidaNormal;
    }
}
