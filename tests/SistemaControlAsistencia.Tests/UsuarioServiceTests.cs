using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;
using SistemaControlAsistencia.Tests.Fakes;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    public class UsuarioServiceTests
    {
        private readonly FakeUsuarioRepository _repositorio = new();
        private readonly UsuarioService _servicio;

        public UsuarioServiceTests()
        {
            _servicio = new UsuarioService(_repositorio);
        }

        private static Usuario CrearDatosValidos() => new Usuario
        {
            Nombre = "Carlos",
            Apellidos = "Fuentes",
            Correo = "carlos.fuentes@empresa.cl",
            Rol = Roles.Empleado,
            Estado = true
        };

        [Fact]
        public void CrearUsuario_ConDatosValidos_SeCreaCorrectamente()
        {
            var resultado = _servicio.CrearUsuario(CrearDatosValidos(), "Clave123!");

            Assert.True(resultado.Exito);
            Assert.Equal("Usuario creado correctamente.", resultado.Mensaje);
            Assert.Single(_repositorio.Usuarios);
        }

        [Fact]
        public void CrearUsuario_ConCorreoDuplicado_SeRechaza()
        {
            _servicio.CrearUsuario(CrearDatosValidos(), "Clave123!");
            var segundo = _servicio.CrearUsuario(CrearDatosValidos(), "OtraClave1!");

            Assert.False(segundo.Exito);
            Assert.Equal("Ya existe un usuario registrado con este correo electrónico.", segundo.Mensaje);
        }

        [Fact]
        public void CrearUsuario_ConCorreoInvalido_SeRechaza()
        {
            Usuario datos = CrearDatosValidos();
            datos.Correo = "correo-sin-arroba";

            var resultado = _servicio.CrearUsuario(datos, "Clave123!");

            Assert.False(resultado.Exito);
        }

        [Fact]
        public void CrearUsuario_ConContrasenaMuyCorta_SeRechaza()
        {
            var resultado = _servicio.CrearUsuario(CrearDatosValidos(), "123");

            Assert.False(resultado.Exito);
        }

        [Fact]
        public void ModificarUsuario_ConDatosValidos_SeActualizaCorrectamente()
        {
            var creado = _servicio.CrearUsuario(CrearDatosValidos(), "Clave123!");
            Usuario datosModificados = creado.Datos!;
            datosModificados.Nombre = "Carlos Andrés";

            var resultado = _servicio.ModificarUsuario(datosModificados, nuevaContrasenaPlano: null);

            Assert.True(resultado.Exito);
            Assert.Equal("Carlos Andrés", _repositorio.ObtenerPorId(datosModificados.IdUsuario)!.Nombre);
        }

        [Fact]
        public void EliminarUsuario_EliminaDeFormaFisicaYPermanente_ElUsuarioYaNoExiste()
        {
            var creado = _servicio.CrearUsuario(CrearDatosValidos(), "Clave123!");
            int id = creado.Datos!.IdUsuario;

            // Lo elimina otro administrador (Id 999). Ojo: el repositorio falso asigna Ids
            // desde 1, así que el usuario recién creado tiene Id 1; si aquí se usara 1, la
            // regla de "no eliminarse a sí mismo" bloquearía la eliminación (ese fue el error
            // que apareció en la primera ejecución de las pruebas).
            var resultado = _servicio.EliminarUsuario(id, idUsuarioQueEjecutaLaAccion: 999);

            Assert.True(resultado.Exito);
            Assert.Equal("Usuario eliminado exitosamente.", resultado.Mensaje);

            // A diferencia de la versión anterior (baja lógica), ahora el usuario
            // debe desaparecer por completo: ObtenerPorId ya no debe encontrarlo.
            Usuario? usuario = _repositorio.ObtenerPorId(id);
            Assert.Null(usuario);
        }

        [Fact]
        public void EliminarUsuario_ConIdInexistente_RetornaFallo()
        {
            var resultado = _servicio.EliminarUsuario(999, idUsuarioQueEjecutaLaAccion: 1);

            Assert.False(resultado.Exito);
        }

        [Fact]
        public void EliminarUsuario_CuandoIntentaEliminarseASiMismo_SeRechazaYNoSeElimina()
        {
            var creado = _servicio.CrearUsuario(CrearDatosValidos(), "Clave123!");
            int id = creado.Datos!.IdUsuario;

            // El mismo usuario intenta eliminar su propia cuenta.
            var resultado = _servicio.EliminarUsuario(id, idUsuarioQueEjecutaLaAccion: id);

            Assert.False(resultado.Exito);
            Assert.Equal(
                "No puede eliminar su propia cuenta mientras tiene la sesión iniciada. " +
                "Pida a otro administrador que la elimine.",
                resultado.Mensaje);

            // El usuario debe seguir existiendo: el rechazo debe ocurrir ANTES de borrar.
            Usuario? usuario = _repositorio.ObtenerPorId(id);
            Assert.NotNull(usuario);
        }
    }
}
