using System;
using SistemaControlAsistencia.App.Helpers;
using Xunit;

namespace SistemaControlAsistencia.Tests
{
    // Pruebas unitarias de los límites exactos de RE-01 y RE-02.
    public class ReglasHorariasTests
    {
        [Theory]
        [InlineData(9, 29, false)]  // Puntual
        [InlineData(9, 30, false)]  // 09:30 exacto NO es atraso
        [InlineData(9, 31, true)]   // Atraso
        [InlineData(10, 0, true)]   // Atraso
        public void EsAtraso_EvaluaCorrectamenteElLimiteDeLas0930(int hora, int minuto, bool esperado)
        {
            var horaEntrada = new TimeSpan(hora, minuto, 0);
            Assert.Equal(esperado, ReglasHorarias.EsAtraso(horaEntrada));
        }

        [Theory]
        [InlineData(17, 0, true)]   // Anticipada
        [InlineData(17, 29, true)]  // Anticipada
        [InlineData(17, 30, false)] // 17:30 exacto NO es anticipada
        [InlineData(17, 31, false)] // Normal
        public void EsSalidaAnticipada_EvaluaCorrectamenteElLimiteDeLas1730(int hora, int minuto, bool esperado)
        {
            var horaSalida = new TimeSpan(hora, minuto, 0);
            Assert.Equal(esperado, ReglasHorarias.EsSalidaAnticipada(horaSalida));
        }
    }
}
