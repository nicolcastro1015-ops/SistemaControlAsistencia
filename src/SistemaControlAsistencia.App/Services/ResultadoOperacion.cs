namespace SistemaControlAsistencia.App.Services
{
    // Envoltorio simple para devolver éxito/fracaso + mensaje desde los servicios, en vez de
    // lanzar excepciones para casos de negocio esperados (correo duplicado, credenciales
    // incorrectas, etc.). Las excepciones se reservan para errores realmente inesperados.
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

    // Variante genérica que además retorna un dato (por ejemplo, el usuario autenticado).
    public class ResultadoOperacion<T> : ResultadoOperacion
    {
        public T? Datos { get; }

        private ResultadoOperacion(bool exito, string mensaje, T? datos) : base(exito, mensaje)
        {
            Datos = datos;
        }

        public static ResultadoOperacion<T> Ok(string mensaje, T datos) => new(true, mensaje, datos);

        // "new" oculta a propósito el Fallo(string) de la clase base.
        public static new ResultadoOperacion<T> Fallo(string mensaje) => new(false, mensaje, default);
    }
}
