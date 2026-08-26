using System;

namespace SistemaControlAsistencia.App.Models
{
    /// <summary>
    /// Entidad Asistencia. Corresponde a la tabla ASISTENCIA de la base de datos ControlAsistencia.
    /// </summary>
    public class Asistencia
    {
        public int IdAsistencia { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public TipoRegistro TipoRegistro { get; set; }

        // Propiedades de conveniencia usadas por los reportes (se llenan al hacer el JOIN en el repositorio)
        public string? NombreUsuario { get; set; }
        public string? ApellidosUsuario { get; set; }
    }
}
