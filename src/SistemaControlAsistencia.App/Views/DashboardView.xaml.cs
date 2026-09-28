using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    public partial class DashboardView : UserControl
    {
        private class FilaActividad
        {
            public string NombreCompleto { get; set; } = string.Empty;
            public string Accion { get; set; } = string.Empty;
            public string Fecha { get; set; } = string.Empty;
            public string Hora { get; set; } = string.Empty;
        }

        public DashboardView()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                UsuarioService usuarioService = FabricaServicios.CrearUsuarioService();
                AsistenciaService asistenciaService = FabricaServicios.CrearAsistenciaService();
                ReporteService reporteService = FabricaServicios.CrearReporteService();

                DateTime hoy = DateTime.Today;

                var activos = usuarioService.ListarUsuarios().Where(u => u.Estado).ToList();
                var registrosHoy = asistenciaService.ListarPorFecha(hoy);
                int presentesHoy = registrosHoy
                    .Where(a => a.TipoRegistro == Models.TipoRegistro.Entrada)
                    .Select(a => a.IdUsuario).Distinct().Count();

                TxtTotalActivos.Text = activos.Count.ToString();
                TxtPresentesHoy.Text = presentesHoy.ToString();
                TxtAtrasosHoy.Text = reporteService.ObtenerAtrasos(hoy).Count.ToString();
                TxtSalidasHoy.Text = reporteService.ObtenerSalidasAnticipadas(hoy).Count.ToString();

                var actividad = asistenciaService.ObtenerActividadReciente(10)
                    .Select(a => new FilaActividad
                    {
                        NombreCompleto = $"{a.NombreUsuario} {a.ApellidosUsuario}",
                        Accion = a.TipoRegistro == Models.TipoRegistro.Entrada ? "Entrada" : "Salida",
                        Fecha = a.FechaHora.ToString("dd-MM-yyyy"),
                        Hora = a.FechaHora.ToString("HH:mm")
                    }).ToList();

                GridActividadReciente.ItemsSource = actividad;
                TxtSinActividad.Visibility = actividad.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible cargar el dashboard. Detalle: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
