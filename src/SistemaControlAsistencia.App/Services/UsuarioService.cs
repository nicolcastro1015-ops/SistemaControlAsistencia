using System.Collections.Generic;
using SistemaControlAsistencia.App.Data;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;

namespace SistemaControlAsistencia.App.Services
{
    // Lógica de negocio de GU-01 (crear), GU-02 (modificar) y GU-03 (eliminar). Las
    // validaciones viven aquí, no en la vista, para que se apliquen sin importar quién llame
    // al servicio y para poder probarlas con pruebas unitarias.
    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public List<Usuario> ListarUsuarios() => _usuarioRepository.ListarTodos();

        public ResultadoOperacion<Usuario> CrearUsuario(Usuario datos, string contrasenaPlano)
        {
            ResultadoOperacion? error = ValidarCamposComunes(datos, contrasenaPlano, esNuevo: true, idActual: null);
            if (error != null)
                return ResultadoOperacion<Usuario>.Fallo(error.Mensaje);

            Usuario nuevo = new Usuario
            {
                Nombre = datos.Nombre.Trim(),
                Apellidos = datos.Apellidos.Trim(),
                Correo = datos.Correo.Trim().ToLowerInvariant(),
                ContrasenaHash = PasswordHasher.Generar(contrasenaPlano),
                Rol = datos.Rol,
                Estado = datos.Estado
            };

            nuevo.IdUsuario = _usuarioRepository.Crear(nuevo);
            return ResultadoOperacion<Usuario>.Ok("Usuario creado correctamente.", nuevo);
        }

        // La contraseña es opcional acá: si viene vacía, se conserva el hash actual.
        public ResultadoOperacion ModificarUsuario(Usuario datos, string? nuevaContrasenaPlano)
        {
            ResultadoOperacion? error = ValidarCamposComunes(datos, nuevaContrasenaPlano, esNuevo: false, idActual: datos.IdUsuario);
            if (error != null)
                return error;

            Usuario existente = _usuarioRepository.ObtenerPorId(datos.IdUsuario)
                ?? throw new KeyNotFoundException($"No existe un usuario con Id {datos.IdUsuario}.");

            existente.Nombre = datos.Nombre.Trim();
            existente.Apellidos = datos.Apellidos.Trim();
            existente.Correo = datos.Correo.Trim().ToLowerInvariant();
            existente.Rol = datos.Rol;
            existente.Estado = datos.Estado;

            if (!string.IsNullOrWhiteSpace(nuevaContrasenaPlano))
                existente.ContrasenaHash = PasswordHasher.Generar(nuevaContrasenaPlano);

            _usuarioRepository.Actualizar(existente);
            return ResultadoOperacion.Ok("Usuario modificado correctamente.");
        }

        // GU-03: elimina definitivamente al usuario y su historial de asistencia (ON DELETE
        // CASCADE). Al principio esta acción solo desactivaba la cuenta; se cambió a una
        // eliminación real tras resolver esa duda con el docente durante la revisión del
        // prototipo. Al volver a probarlo, una administradora se eliminó a sí misma por
        // coincidencia, lo que reveló el riesgo de quedar sin ningún administrador disponible.
        // Por eso se agregó esta validación: nadie puede eliminar su propia cuenta mientras la
        // tiene abierta, solo otro Administrador puede hacerlo. Se valida acá (no solo en la
        // pantalla) para que se cumpla sin importar desde dónde se llame este método.
        public ResultadoOperacion EliminarUsuario(int idUsuarioAEliminar, int idUsuarioQueEjecutaLaAccion)
        {
            if (idUsuarioAEliminar == idUsuarioQueEjecutaLaAccion)
                return ResultadoOperacion.Fallo(
                    "No puede eliminar su propia cuenta mientras tiene la sesión iniciada. " +
                    "Pida a otro administrador que la elimine.");

            Usuario? usuario = _usuarioRepository.ObtenerPorId(idUsuarioAEliminar);
            if (usuario == null)
                return ResultadoOperacion.Fallo("El usuario indicado no existe.");

            _usuarioRepository.Eliminar(idUsuarioAEliminar);
            return ResultadoOperacion.Ok("Usuario eliminado exitosamente.");
        }

        // Validaciones compartidas por CrearUsuario y ModificarUsuario.
        private ResultadoOperacion? ValidarCamposComunes(Usuario datos, string? contrasenaPlano, bool esNuevo, int? idActual)
        {
            if (!Validaciones.EsTextoValido(datos.Nombre))
                return ResultadoOperacion.Fallo("Debe ingresar un nombre válido.");

            if (!Validaciones.EsTextoValido(datos.Apellidos))
                return ResultadoOperacion.Fallo("Debe ingresar apellidos válidos.");

            if (!Validaciones.EsCorreoValido(datos.Correo))
                return ResultadoOperacion.Fallo("Debe ingresar un correo electrónico con formato válido.");

            if (_usuarioRepository.ExisteCorreo(datos.Correo.Trim().ToLowerInvariant(), idActual))
                return ResultadoOperacion.Fallo("Ya existe un usuario registrado con este correo electrónico.");

            if (esNuevo && !Validaciones.EsContrasenaValida(contrasenaPlano))
                return ResultadoOperacion.Fallo("La contraseña debe tener al menos 6 caracteres.");

            if (!esNuevo && !string.IsNullOrWhiteSpace(contrasenaPlano) && !Validaciones.EsContrasenaValida(contrasenaPlano))
                return ResultadoOperacion.Fallo("La contraseña debe tener al menos 6 caracteres.");

            if (!Roles.EsRolValido(datos.Rol))
                return ResultadoOperacion.Fallo("Debe seleccionar un rol válido (Administrador o Empleado).");

            return null;
        }
    }
}
