using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Data
{
    /// <summary>
    /// Implementación real de IAsistenciaRepository contra SQL Server (tabla ASISTENCIA).
    /// </summary>
    public class AsistenciaRepository : IAsistenciaRepository
    {
        public void Registrar(Asistencia asistencia)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "INSERT INTO dbo.Asistencia (IdUsuario, TipoRegistro, FechaHora) " +
                "VALUES (@IdUsuario, @TipoRegistro, @FechaHora)", conexion);

            comando.Parameters.AddWithValue("@IdUsuario", asistencia.IdUsuario);
            comando.Parameters.AddWithValue("@TipoRegistro", asistencia.TipoRegistro.ToString());
            comando.Parameters.AddWithValue("@FechaHora", asistencia.FechaHora);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        public List<Asistencia> ListarPorUsuarioYFecha(int idUsuario, DateTime fecha)
        {
            List<Asistencia> resultado = new();

            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT IdAsistencia, IdUsuario, TipoRegistro, FechaHora FROM dbo.Asistencia " +
                "WHERE IdUsuario = @IdUsuario AND CAST(FechaHora AS DATE) = @Fecha " +
                "ORDER BY FechaHora", conexion);

            comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
            comando.Parameters.AddWithValue("@Fecha", fecha.Date);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
                resultado.Add(MapearAsistencia(lector));

            return resultado;
        }

        public Asistencia? ObtenerUltimoRegistro(int idUsuario)
        {
            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT TOP 1 IdAsistencia, IdUsuario, TipoRegistro, FechaHora FROM dbo.Asistencia " +
                "WHERE IdUsuario = @IdUsuario ORDER BY FechaHora DESC", conexion);
            comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            return lector.Read() ? MapearAsistencia(lector) : null;
        }

        public List<Asistencia> ListarPorFecha(DateTime fecha)
        {
            List<Asistencia> resultado = new();

            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT a.IdAsistencia, a.IdUsuario, a.TipoRegistro, a.FechaHora, u.Nombre, u.Apellidos " +
                "FROM dbo.Asistencia a INNER JOIN dbo.Usuario u ON u.IdUsuario = a.IdUsuario " +
                "WHERE CAST(a.FechaHora AS DATE) = @Fecha ORDER BY a.FechaHora", conexion);
            comando.Parameters.AddWithValue("@Fecha", fecha.Date);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
                resultado.Add(MapearAsistenciaConUsuario(lector));

            return resultado;
        }

        public List<Asistencia> ListarPorUsuario(int idUsuario)
        {
            List<Asistencia> resultado = new();

            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT IdAsistencia, IdUsuario, TipoRegistro, FechaHora FROM dbo.Asistencia " +
                "WHERE IdUsuario = @IdUsuario ORDER BY FechaHora DESC", conexion);
            comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
                resultado.Add(MapearAsistencia(lector));

            return resultado;
        }

        public List<Asistencia> ListarRecientes(int cantidad)
        {
            List<Asistencia> resultado = new();

            using SqlConnection conexion = DatabaseHelper.CrearConexion();
            using SqlCommand comando = new SqlCommand(
                "SELECT TOP (@Cantidad) a.IdAsistencia, a.IdUsuario, a.TipoRegistro, a.FechaHora, u.Nombre, u.Apellidos " +
                "FROM dbo.Asistencia a INNER JOIN dbo.Usuario u ON u.IdUsuario = a.IdUsuario " +
                "ORDER BY a.FechaHora DESC", conexion);
            comando.Parameters.AddWithValue("@Cantidad", cantidad);

            conexion.Open();
            using SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
                resultado.Add(MapearAsistenciaConUsuario(lector));

            return resultado;
        }

        private static Asistencia MapearAsistencia(SqlDataReader lector) => new Asistencia
        {
            IdAsistencia = lector.GetInt32(0),
            IdUsuario = lector.GetInt32(1),
            TipoRegistro = Enum.Parse<Models.TipoRegistro>(lector.GetString(2)),
            FechaHora = lector.GetDateTime(3)
        };

        private static Asistencia MapearAsistenciaConUsuario(SqlDataReader lector) => new Asistencia
        {
            IdAsistencia = lector.GetInt32(0),
            IdUsuario = lector.GetInt32(1),
            TipoRegistro = Enum.Parse<Models.TipoRegistro>(lector.GetString(2)),
            FechaHora = lector.GetDateTime(3),
            NombreUsuario = lector.GetString(4),
            ApellidosUsuario = lector.GetString(5)
        };
    }
}
