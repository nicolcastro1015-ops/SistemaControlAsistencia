using System;

namespace SistemaControlAsistencia.App.Helpers
{
    // Reglas horarias centralizadas (RE-01 y RE-02). Si la empresa cambiara el horario, solo
    // se modifica esta clase. Trabaja con TimeSpan (no DateTime.Now) para que las pruebas
    // unitarias puedan probar cualquier hora sin depender del reloj real.
    public static class ReglasHorarias
    {
        // 09:30 en punto es puntual, NO atraso (por eso EsAtraso usa ">", no ">=").
        public static readonly TimeSpan HoraLimiteEntrada = new TimeSpan(9, 30, 0);

        // 17:30 en punto es normal, NO anticipada (por eso EsSalidaAnticipada usa "<", no "<=").
        public static readonly TimeSpan HoraMinimaSalidaNormal = new TimeSpan(17, 30, 0);

        public static bool EsAtraso(TimeSpan horaEntrada) => horaEntrada > HoraLimiteEntrada;

        public static bool EsSalidaAnticipada(TimeSpan horaSalida) => horaSalida < HoraMinimaSalidaNormal;
    }
}
