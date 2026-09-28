using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    public partial class UsuariosView : UserControl
    {
        private class FilaUsuario
        {
            public int IdUsuario { get; set; }
            public string NombreCompleto { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public string Rol { get; set; } = string.Empty;
            public string EstadoTexto { get; set; } = string.Empty;
        }

        private readonly UsuarioService _usuarioService = FabricaServicios.CrearUsuarioService();

        public UsuariosView()
        {
            InitializeComponent();
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            try
            {
                var filas = _usuarioService.ListarUsuarios()
                    .Select(u => new FilaUsuario
                    {
                        IdUsuario = u.IdUsuario,
                        NombreCompleto = u.NombreCompleto,
                        Correo = u.Correo,
                        Rol = u.Rol,
                        EstadoTexto = u.Estado ? "Activo" : "Inactivo"
                    }).ToList();

                GridUsuarios.ItemsSource = filas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible cargar los usuarios. Detalle: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new UsuarioFormWindow();
            if (ventana.ShowDialog() == true)
                CargarUsuarios();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            int idUsuario = (int)((Button)sender).Tag;
            Usuario? usuario = _usuarioService.ListarUsuarios().FirstOrDefault(u => u.IdUsuario == idUsuario);
            if (usuario == null) return;

            var ventana = new UsuarioFormWindow(usuario);
            if (ventana.ShowDialog() == true)
                CargarUsuarios();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int idUsuario = (int)((Button)sender).Tag;
            int idUsuarioActual = SesionActual.UsuarioAutenticado!.IdUsuario;

            // Aviso inmediato si se intenta eliminar la propia cuenta: ni siquiera se
            // muestran los avisos de confirmación de más abajo. La validación real (la que
            // de verdad importa) está en UsuarioService.EliminarUsuario; este aviso es solo
            // para que la persona lo sepa de inmediato, sin esperar a los dos clics.
            if (idUsuario == idUsuarioActual)
            {
                MessageBox.Show(
                    "No puede eliminar su propia cuenta mientras tiene la sesión iniciada. " +
                    "Pida a otro administrador que la elimine.",
                    "Operación no permitida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Primer aviso: confirmación general.
            MessageBoxResult primeraConfirmacion = MessageBox.Show(
                "¿Está seguro de que desea eliminar este usuario?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (primeraConfirmacion != MessageBoxResult.Yes)
                return;

            // Segundo aviso: advertencia final, más explícita, porque la eliminación
            // ahora es un DELETE físico e irreversible (también borra su historial
            // de asistencia por el ON DELETE CASCADE de la base de datos).
            MessageBoxResult confirmacionFinal = MessageBox.Show(
                "Esta acción eliminará al usuario y TODO su historial de asistencia de forma " +
                "permanente. No se puede deshacer.\n\n¿Está totalmente seguro de continuar?",
                "Confirmación final", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirmacionFinal != MessageBoxResult.Yes)
                return;

            try
            {
                var resultado = _usuarioService.EliminarUsuario(idUsuario, idUsuarioActual);
                MessageBox.Show(resultado.Mensaje, resultado.Exito ? "Éxito" : "Error",
                    MessageBoxButton.OK, resultado.Exito ? MessageBoxImage.Information : MessageBoxImage.Warning);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible eliminar el usuario. Detalle: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
