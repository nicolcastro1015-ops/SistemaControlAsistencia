using System.Windows;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    /// <summary>
    /// Ventana principal del empleado. Deliberadamente NO incluye botones de Usuarios,
    /// Dashboard administrativo ni Reportes: un empleado solo puede marcar entrada/salida,
    /// ver sus datos básicos y cerrar sesión (sección 6 del enunciado).
    /// </summary>
    public partial class EmpleadoMainWindow : Window
    {
        public EmpleadoMainWindow()
        {
            InitializeComponent();

            if (SesionActual.UsuarioAutenticado == null)
            {
                Loaded += (_, _) => VolverALogin();
                return;
            }

            TxtNombreUsuario.Text = SesionActual.UsuarioAutenticado.NombreCompleto;
            TxtRolUsuario.Text = SesionActual.UsuarioAutenticado.Rol;

            ContenidoPrincipal.Content = new AsistenciaView();
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e) => VolverALogin();

        private void VolverALogin()
        {
            SesionActual.CerrarSesion();
            new LoginWindow().Show();
            Close();
        }
    }
}
