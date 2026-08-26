using System.Collections.Generic;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Data
{
    /// <summary>
    /// Abstracción del acceso a datos de USUARIO. Se define como interfaz para poder
    /// probar UsuarioService y AutenticacionService con repositorios falsos (en memoria)
    /// en las pruebas unitarias, sin necesitar una conexión real a SQL Server.
    /// </summary>
    public interface IUsuarioRepository
    {
        Usuario? ObtenerPorId(int idUsuario);
        Usuario? ObtenerPorCorreo(string correo);
        List<Usuario> ListarTodos();
        bool ExisteCorreo(string correo, int? excluirId = null);
        int Crear(Usuario usuario);
        void Actualizar(Usuario usuario);
        void Desactivar(int idUsuario);
    }
}
