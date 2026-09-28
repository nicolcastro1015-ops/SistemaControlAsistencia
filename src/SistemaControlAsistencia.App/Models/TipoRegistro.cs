namespace SistemaControlAsistencia.App.Models
{
    // Entrada o Salida. Se usa un enum en vez de strings sueltos para evitar errores de tipeo
    // ("entrada" vs "Entrada") y detectar valores inválidos al compilar. Se guarda en la base
    // de datos como texto ("Entrada"/"Salida") para que la tabla siga siendo legible desde SSMS.
    public enum TipoRegistro
    {
        Entrada,
        Salida
    }
}
