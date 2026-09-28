using System.Collections.Generic;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Data
{
    // Abstracción del acceso a datos de USUARIO. Se define como interfaz para poder probar
    // AutenticacionService y UsuarioService con FakeUsuarioRepository (en memoria), sin
    // necesitar SQL Server encendido. La implementación real está en UsuarioRepository.cs.
    public interface IUsuarioRepository
    {
        Usuario? ObtenerPorId(int idUsuario);
        Usuario? ObtenerPorCorreo(string correo);
        List<Usuario> ListarTodos();

        // excluirId se usa al editar (GU-02), para no comparar el correo del usuario contra sí mismo.
        bool ExisteCorreo(string correo, int? excluirId = null);

        int Crear(Usuario usuario);
        void Actualizar(Usuario usuario);

        // GU-03: DELETE físico y definitivo (no una desactivación). Gracias a ON DELETE CASCADE
        // en ASISTENCIA, el historial del usuario se borra solo, en la misma operación.
        void Eliminar(int idUsuario);
    }
}
