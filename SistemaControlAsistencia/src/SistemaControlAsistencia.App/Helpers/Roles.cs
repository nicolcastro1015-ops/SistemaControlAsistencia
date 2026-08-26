namespace SistemaControlAsistencia.App.Helpers
{
    /// <summary>
    /// Constantes centralizadas de roles. Evita escribir literales "Administrador" / "Empleado"
    /// repetidos por todo el código y facilita validar valores permitidos en un solo lugar.
    /// </summary>
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Empleado = "Empleado";

        public static bool EsRolValido(string? rol) =>
            rol == Administrador || rol == Empleado;
    }
}
