using System.Windows;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    // Ventana principal del Empleado. No tiene menú de Usuarios, Dashboard ni Reportes: no
    // están ocultos, directamente no existen en esta ventana. Un Empleado solo marca su
    // entrada/salida, ve sus datos básicos y cierra sesión.
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
