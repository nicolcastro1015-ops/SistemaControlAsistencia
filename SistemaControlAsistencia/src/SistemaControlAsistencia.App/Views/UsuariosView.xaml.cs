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

            MessageBoxResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea eliminar este usuario?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
                return;

            try
            {
                var resultado = _usuarioService.EliminarUsuario(idUsuario);
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
