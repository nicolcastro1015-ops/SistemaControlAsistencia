namespace SistemaControlAsistencia.App.Models
{
    /// <summary>
    /// Representa el tipo de marca de asistencia.
    /// Se usa un enum en lugar de strings sueltos para evitar errores de tipeo
    /// (por ejemplo "entrada" vs "Entrada" vs "ENTRADA") y para que el compilador
    /// detecte valores inválidos en tiempo de compilación en vez de en tiempo de ejecución.
    /// El valor se guarda en la base de datos como texto ("Entrada" / "Salida")
    /// mediante ToString() / Enum.Parse, por lo que la tabla ASISTENCIA sigue siendo
    /// legible directamente desde SQL Server Management Studio.
    /// </summary>
    public enum TipoRegistro
    {
        Entrada,
        Salida
    }
}
