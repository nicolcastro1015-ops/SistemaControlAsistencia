using System;

namespace SistemaControlAsistencia.App.Models
{
    // Representa un movimiento de entrada o salida. Corresponde a la tabla ASISTENCIA, con
    // FOREIGN KEY hacia USUARIO (ON DELETE CASCADE: si se elimina el usuario, sus asistencias
    // se eliminan con él).
    public class Asistencia
    {
        public int IdAsistencia { get; set; }
        public int IdUsuario { get; set; }

        // Siempre la hora del sistema, nunca escrita a mano. ReporteService la compara contra
        // ReglasHorarias para saber si es atraso o salida anticipada.
        public DateTime FechaHora { get; set; }

        public TipoRegistro TipoRegistro { get; set; }

        // Se llenan solo cuando el repositorio hace JOIN con USUARIO (ver AsistenciaRepository),
        // para que los reportes puedan mostrar el nombre sin otra consulta.
        public string? NombreUsuario { get; set; }
        public string? ApellidosUsuario { get; set; }
    }
}
