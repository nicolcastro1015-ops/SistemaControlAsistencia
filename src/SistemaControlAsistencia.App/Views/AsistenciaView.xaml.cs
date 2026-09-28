using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SistemaControlAsistencia.App.Models;
using SistemaControlAsistencia.App.Services;

namespace SistemaControlAsistencia.App.Views
{
    public partial class AsistenciaView : UserControl
    {
        private readonly AsistenciaService _asistenciaService = FabricaServicios.CrearAsistenciaService();
        private readonly int _idUsuario;

        public AsistenciaView()
        {
            InitializeComponent();
            _idUsuario = SesionActual.UsuarioAutenticado!.IdUsuario;

            TxtSaludo.Text = $"Hola, {SesionActual.UsuarioAutenticado!.Nombre}";
            TxtFechaHora.Text = DateTime.Now.ToString("dddd dd 'de' MMMM 'de' yyyy - HH:mm");

            ActualizarEstado();
        }

        private void ActualizarEstado()
        {
            try
            {
                Asistencia? ultimo = _asistenciaService.ObtenerUltimoRegistro(_idUsuario);
                TxtUltimoRegistro.Text = ultimo == null
                    ? "Sin registros todavía."
                    : $"{(ultimo.TipoRegistro == TipoRegistro.Entrada ? "Entrada" : "Salida")} - {ultimo.FechaHora:dd-MM-yyyy HH:mm}";

                var (puedeEntrada, puedeSalida) = _asistenciaService.ObtenerEstadoBotones(_idUsuario, DateTime.Now);
                BtnMarcarEntrada.IsEnabled = puedeEntrada;
                BtnMarcarSalida.IsEnabled = puedeSalida;
            }
            catch (Exception ex)
            {
                MostrarMensaje("No fue posible cargar el estado de asistencia. Detalle: " + ex.Message, esError: true);
            }
        }

        private void BtnMarcarEntrada_Click(object sender, RoutedEventArgs e)
        {
            // Deshabilitar de inmediato evita que clics repetidos generen registros duplicados
            // mientras se espera la respuesta de la base de datos (sección 12/42 del enunciado).
            BtnMarcarEntrada.IsEnabled = false;
            try
            {
                var resultado = _asistenciaService.RegistrarEntrada(_idUsuario, DateTime.Now);
                MostrarMensaje(resultado.Mensaje, esError: !resultado.Exito);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al registrar la entrada. Detalle: " + ex.Message, esError: true);
            }
            finally
            {
                ActualizarEstado();
            }
        }

        private void BtnMarcarSalida_Click(object sender, RoutedEventArgs e)
        {
            BtnMarcarSalida.IsEnabled = false;
            try
            {
                var resultado = _asistenciaService.RegistrarSalida(_idUsuario, DateTime.Now);
                MostrarMensaje(resultado.Mensaje, esError: !resultado.Exito);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al registrar la salida. Detalle: " + ex.Message, esError: true);
            }
            finally
            {
                ActualizarEstado();
            }
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            TxtMensaje.Text = mensaje;
            TxtMensaje.Foreground = esError ? (Brush)FindResource("ColorError") : (Brush)FindResource("ColorExito");
            PanelMensaje.Visibility = Visibility.Visible;
        }
    }
}
