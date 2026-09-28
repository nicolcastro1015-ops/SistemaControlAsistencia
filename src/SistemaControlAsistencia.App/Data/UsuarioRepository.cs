using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Data
{
    // Implementación real de IUsuarioRepository contra SQL Server. Usa exclusivamente
    // consultas parametrizadas para evitar inyección SQL.
    public class UsuarioRepository : IUsuarioRepository
    {
        public Usuario? ObtenerPorId(int idUsuario)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT IdUsuario, Nombre, Apellidos, Correo, ContrasenaHash, Rol, Estado " +
                "FROM dbo.Usuario WHERE IdUsuario = @IdUsuario", conexion);
            comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            return lector.Read() ? MapearUsuario(lector) : null;
        }

        // Usado por AutenticacionService.Login para buscar al usuario que intenta entrar.
        public Usuario? ObtenerPorCorreo(string correo)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT IdUsuario, Nombre, Apellidos, Correo, ContrasenaHash, Rol, Estado " +
                "FROM dbo.Usuario WHERE Correo = @Correo", conexion);
            comando.Parameters.AddWithValue("@Correo", correo);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            return lector.Read() ? MapearUsuario(lector) : null;
        }

        public List<Usuario> ListarTodos()
        {
            List<Usuario> usuarios = new();

            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT IdUsuario, Nombre, Apellidos, Correo, ContrasenaHash, Rol, Estado " +
                "FROM dbo.Usuario ORDER BY Apellidos, Nombre", conexion);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
                usuarios.Add(MapearUsuario(lector));

            return usuarios;
        }

        public bool ExisteCorreo(string correo, int? excluirId = null)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            string sql = "SELECT COUNT(1) FROM dbo.Usuario WHERE Correo = @Correo";
            if (excluirId.HasValue)
                sql += " AND IdUsuario <> @ExcluirId";

            using SqlCommand comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Correo", correo);
            if (excluirId.HasValue)
                comando.Parameters.AddWithValue("@ExcluirId", excluirId.Value);

            conexion.Open();
            int total = (int)comando.ExecuteScalar();
            return total > 0;
        }

        // OUTPUT INSERTED.IdUsuario devuelve el Id generado por SQL Server en la misma sentencia.
        public int Crear(Usuario usuario)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "INSERT INTO dbo.Usuario (Nombre, Apellidos, Correo, ContrasenaHash, Rol, Estado) " +
                "OUTPUT INSERTED.IdUsuario " +
                "VALUES (@Nombre, @Apellidos, @Correo, @ContrasenaHash, @Rol, @Estado)", conexion);

            AgregarParametros(comando, usuario);

            conexion.Open();
            return (int)comando.ExecuteScalar();
        }

        public void Actualizar(Usuario usuario)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "UPDATE dbo.Usuario SET Nombre = @Nombre, Apellidos = @Apellidos, Correo = @Correo, " +
                "ContrasenaHash = @ContrasenaHash, Rol = @Rol, Estado = @Estado " +
                "WHERE IdUsuario = @IdUsuario", conexion);

            AgregarParametros(comando, usuario);
            comando.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        // GU-03: DELETE físico. Cambió de UPDATE Estado=0 a esto tras la duda resuelta con el
        // docente (ver la explicación completa en UsuarioService.EliminarUsuario). El ON DELETE
        // CASCADE de la tabla ASISTENCIA se encarga de borrar el historial del usuario.
        public void Eliminar(int idUsuario)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "DELETE FROM dbo.Usuario WHERE IdUsuario = @IdUsuario", conexion);
            comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        private static void AgregarParametros(SqlCommand comando, Usuario usuario)
        {
            comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            comando.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
            comando.Parameters.AddWithValue("@Correo", usuario.Correo);
            comando.Parameters.AddWithValue("@ContrasenaHash", usuario.ContrasenaHash);
            comando.Parameters.AddWithValue("@Rol", usuario.Rol);
            comando.Parameters.AddWithValue("@Estado", usuario.Estado);
        }

        private static Usuario MapearUsuario(SqlDataReader lector) => new Usuario
        {
            IdUsuario = lector.GetInt32(0),
            Nombre = lector.GetString(1),
            Apellidos = lector.GetString(2),
            Correo = lector.GetString(3),
            ContrasenaHash = lector.GetString(4),
            Rol = lector.GetString(5),
            Estado = lector.GetBoolean(6)
        };
    }
}
