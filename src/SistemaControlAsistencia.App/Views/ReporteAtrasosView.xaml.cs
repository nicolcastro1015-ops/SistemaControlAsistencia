using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    public partial class ReporteAtrasosView : UserControl
    {
        private class FilaVista
        {
            public int IdUsuario { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellidos { get; set; } = string.Empty;
            public string FechaTexto { get; set; } = string.Empty;
            public string HoraTexto { get; set; } = string.Empty;
        }

        private readonly ReporteService _reporteService = FabricaServicios.CrearReporteService();

        public ReporteAtrasosView()
        {
            InitializeComponent();
            DpFecha.SelectedDate = DateTime.Today;
            Cargar(DateTime.Today);
        }

        private void DpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DpFecha.SelectedDate.HasValue)
                Cargar(DpFecha.SelectedDate.Value);
        }

        private void Cargar(DateTime fecha)
        {
            try
            {
                var filas = _reporteService.ObtenerAtrasos(fecha)
                    .Select(f => new FilaVista
                    {
                        IdUsuario = f.IdUsuario,
                        Nombre = f.Nombre,
                        Apellidos = f.Apellidos,
                        FechaTexto = f.Fecha.ToString("dd-MM-yyyy"),
                        HoraTexto = f.Hora.ToString(@"hh\:mm")
                    }).ToList();

                GridResultados.ItemsSource = filas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible cargar el reporte. Detalle: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
