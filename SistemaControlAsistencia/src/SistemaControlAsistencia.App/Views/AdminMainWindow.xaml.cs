using System.Windows;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    /// <summary>
    /// Ventana contenedora del administrador. Verifica el rol en el constructor
    /// (no solo esconde botones): si por algún motivo se abriera sin un administrador
    /// autenticado, la ventana se cierra de inmediato y vuelve al Login.
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();

            if (!SesionActual.EsAdministrador)
            {
                MessageBox.Show("Esta sección requiere permisos de administrador.", "Acceso denegado",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Loaded += (_, _) => VolverALogin();
                return;
            }

            TxtNombreUsuario.Text = SesionActual.UsuarioAutenticado!.NombreCompleto;
            TxtRolUsuario.Text = SesionActual.UsuarioAutenticado!.Rol;

            MostrarVista(new DashboardView());
        }

        private void MostrarVista(UIElement vista) => ContenidoPrincipal.Content = vista;

        private void BtnDashboard_Click(object sender, RoutedEventArgs e) => MostrarVista(new DashboardView());
        private void BtnAsistencia_Click(object sender, RoutedEventArgs e) => MostrarVista(new AsistenciaView());
        private void BtnUsuarios_Click(object sender, RoutedEventArgs e) => MostrarVista(new UsuariosView());
        private void BtnReporteAtrasos_Click(object sender, RoutedEventArgs e) => MostrarVista(new ReporteAtrasosView());
        private void BtnReporteSalidas_Click(object sender, RoutedEventArgs e) => MostrarVista(new ReporteSalidasAnticipadasView());
        private void BtnReporteInasistencias_Click(object sender, RoutedEventArgs e) => MostrarVista(new ReporteInasistenciasView());

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e) => VolverALogin();

        private void VolverALogin()
        {
            SesionActual.CerrarSesion();
            new LoginWindow().Show();
            Close();
        }
    }
}
