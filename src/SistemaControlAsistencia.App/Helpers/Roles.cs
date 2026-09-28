namespace SistemaControlAsistencia.App.Helpers
{
    // Constantes centralizadas de roles. Evita escribir "Administrador"/"Empleado" sueltos por
    // todo el código y el riesgo de un error de tipeo que rompa una comparación.
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Empleado = "Empleado";

        // Usado por UsuarioService antes de guardar, como respaldo del CHECK de la base de datos.
        public static bool EsRolValido(string? rol) =>
            rol == Administrador || rol == Empleado;
    }
}
