using System.Windows;
using System.Windows.Controls;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    // Un mismo formulario modal cubre GU-01 (crear) y GU-02 (modificar): cuál constructor se
    // usa marca la diferencia. UsuariosView decide qué hacer según el resultado de ShowDialog().
    public partial class UsuarioFormWindow : Window
    {
        private readonly UsuarioService _usuarioService = FabricaServicios.CrearUsuarioService();

        // null en modo Crear; con datos en modo Editar.
        private readonly Usuario? _usuarioExistente;

        public UsuarioFormWindow()
        {
            InitializeComponent();
            _usuarioExistente = null;
        }

        public UsuarioFormWindow(Usuario usuarioExistente)
        {
            InitializeComponent();
            _usuarioExistente = usuarioExistente;

            TxtTitulo.Text = "Editar usuario";
            TxtEtiquetaContrasena.Text = "Nueva contraseña (dejar en blanco para no cambiarla)";

            TxtNombre.Text = usuarioExistente.Nombre;
            TxtApellidos.Text = usuarioExistente.Apellidos;
            TxtCorreo.Text = usuarioExistente.Correo;
            ChkActivo.IsChecked = usuarioExistente.Estado;

            foreach (ComboBoxItem item in CmbRol.Items)
            {
                if ((string)item.Content == usuarioExistente.Rol)
                {
                    CmbRol.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            TxtMensajeError.Visibility = Visibility.Collapsed;

            string? rolSeleccionado = (CmbRol.SelectedItem as ComboBoxItem)?.Content as string;

            Usuario datos = new Usuario
            {
                IdUsuario = _usuarioExistente?.IdUsuario ?? 0,
                Nombre = TxtNombre.Text,
                Apellidos = TxtApellidos.Text,
                Correo = TxtCorreo.Text,
                Rol = rolSeleccionado ?? string.Empty,
                Estado = ChkActivo.IsChecked == true
            };

            try
            {
                // Todas las validaciones ocurren dentro de UsuarioService, no acá.
                if (_usuarioExistente == null)
                {
                    var resultado = _usuarioService.CrearUsuario(datos, PwdContrasena.Password);
                    if (!resultado.Exito)
                    {
                        MostrarError(resultado.Mensaje);
                        return;
                    }
                    MessageBox.Show(resultado.Mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var resultado = _usuarioService.ModificarUsuario(datos, PwdContrasena.Password);
                    if (!resultado.Exito)
                    {
                        MostrarError(resultado.Mensaje);
                        return;
                    }
                    MessageBox.Show(resultado.Mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
            }
            catch (System.Exception ex)
            {
                MostrarError("No fue posible guardar el usuario. Detalle: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            TxtMensajeError.Text = mensaje;
            TxtMensajeError.Visibility = Visibility.Visible;
        }
    }
}
