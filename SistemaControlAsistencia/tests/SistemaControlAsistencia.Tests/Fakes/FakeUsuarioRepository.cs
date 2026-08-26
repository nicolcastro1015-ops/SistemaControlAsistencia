using System.Collections.Generic;
using System.Linq;
using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.Tests.Fakes
{
    /// <summary>
    /// Repositorio en memoria usado exclusivamente por las pruebas unitarias.
    /// Permite probar AutenticacionService y UsuarioService sin conexión real a SQL Server.
    /// </summary>
    public class FakeUsuarioRepository : IUsuarioRepository
    {
        public List<Usuario> Usuarios { get; } = new();
        private int _siguienteId = 1;

        public Usuario? ObtenerPorId(int idUsuario) => Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);

        public Usuario? ObtenerPorCorreo(string correo) =>
            Usuarios.FirstOrDefault(u => u.Correo.Equals(correo, System.StringComparison.OrdinalIgnoreCase));

        public List<Usuario> ListarTodos() => Usuarios.ToList();

        public bool ExisteCorreo(string correo, int? excluirId = null) =>
            Usuarios.Any(u => u.Correo.Equals(correo, System.StringComparison.OrdinalIgnoreCase)
                               && (!excluirId.HasValue || u.IdUsuario != excluirId.Value));

        public int Crear(Usuario usuario)
        {
            usuario.IdUsuario = _siguienteId++;
            Usuarios.Add(usuario);
            return usuario.IdUsuario;
        }

        public void Actualizar(Usuario usuario)
        {
            var existente = ObtenerPorId(usuario.IdUsuario);
            if (existente == null) return;

            existente.Nombre = usuario.Nombre;
            existente.Apellidos = usuario.Apellidos;
            existente.Correo = usuario.Correo;
            existente.ContrasenaHash = usuario.ContrasenaHash;
            existente.Rol = usuario.Rol;
            existente.Estado = usuario.Estado;
        }

        public void Desactivar(int idUsuario)
        {
            var existente = ObtenerPorId(idUsuario);
            if (existente != null) existente.Estado = false;
        }
    }
}
