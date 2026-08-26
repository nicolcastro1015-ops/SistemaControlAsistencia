namespace SistemaControlAsistencia.App.Services
{
    /// <summary>
    /// Envoltorio simple para devolver éxito/fracaso + mensaje desde los servicios,
    /// en lugar de lanzar excepciones para casos de negocio esperados (correo duplicado,
    /// credenciales incorrectas, etc.). Las excepciones se reservan para errores
    /// realmente inesperados (ver Helpers de manejo de errores en las vistas).
    /// </summary>
    public class ResultadoOperacion
    {
        public bool Exito { get; }
        public string Mensaje { get; }

        protected ResultadoOperacion(bool exito, string mensaje)
        {
            Exito = exito;
            Mensaje = mensaje;
        }

        public static ResultadoOperacion Ok(string mensaje) => new(true, mensaje);
        public static ResultadoOperacion Fallo(string mensaje) => new(false, mensaje);
    }

    /// <summary>Variante genérica que además retorna un dato (por ejemplo, el usuario autenticado).</summary>
    public class ResultadoOperacion<T> : ResultadoOperacion
    {
        public T? Datos { get; }

        private ResultadoOperacion(bool exito, string mensaje, T? datos) : base(exito, mensaje)
        {
            Datos = datos;
        }

        public static ResultadoOperacion<T> Ok(string mensaje, T datos) => new(true, mensaje, datos);
        public static new ResultadoOperacion<T> Fallo(string mensaje) => new(false, mensaje, default);
    }
}
