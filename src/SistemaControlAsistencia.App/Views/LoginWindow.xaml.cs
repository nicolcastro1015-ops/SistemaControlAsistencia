using System.Windows;
using SistemaControlAsistencia.App.Helpers;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    public partial class LoginWindow : Window
    {
        private bool _actualizandoCampo;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ChkMostrarContrasena_Changed(object sender, RoutedEventArgs e)
        {
            if (ChkMostrarContrasena.IsChecked == true)
            {
                TxtContrasenaVisible.Text = PwdContrasena.Password;
                PwdContrasena.Visibility = Visibility.Collapsed;
                TxtContrasenaVisible.Visibility = Visibility.Visible;
            }
            else
            {
                PwdContrasena.Password = TxtContrasenaVisible.Text;
                TxtContrasenaVisible.Visibility = Visibility.Collapsed;
                PwdContrasena.Visibility = Visibility.Visible;
            }
        }

        private void PwdContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_actualizandoCampo) return;
            _actualizandoCampo = true;
            TxtContrasenaVisible.Text = PwdContrasena.Password;
            _actualizandoCampo = false;
        }

        private void TxtContrasenaVisible_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_actualizandoCampo) return;
            _actualizandoCampo = true;
            PwdContrasena.Password = TxtContrasenaVisible.Text;
            _actualizandoCampo = false;
        }

        private string ObtenerContrasenaActual() =>
            ChkMostrarContrasena.IsChecked == true ? TxtContrasenaVisible.Text : PwdContrasena.Password;

        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            TxtMensajeError.Visibility = Visibility.Collapsed;
            BtnIniciarSesion.IsEnabled = false;

            try
            {
                AutenticacionService servicio = FabricaServicios.CrearAutenticacionService();
                var resultado = servicio.Login(TxtCorreo.Text, ObtenerContrasenaActual());

                if (!resultado.Exito)
                {
                    MostrarError(resultado.Mensaje);
                    return;
                }

                SesionActual.IniciarSesion(resultado.Datos!);

                Window ventanaPrincipal = resultado.Datos!.Rol == Roles.Administrador
                    ? new AdminMainWindow()
                    : new EmpleadoMainWindow();

                ventanaPrincipal.Show();
                Close();
            }
            catch (System.Exception ex)
            {
                MostrarError("No fue posible conectar con la base de datos. Detalle: " + ex.Message);
            }
            finally
            {
                BtnIniciarSesion.IsEnabled = true;
            }
        }

        private void MostrarError(string mensaje)
        {
            TxtMensajeError.Text = mensaje;
            TxtMensajeError.Visibility = Visibility.Visible;
        }
    }
}
