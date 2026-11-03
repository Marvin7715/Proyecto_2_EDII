using TransRed.Core.Modelos;

namespace TransRed.Core.Modelos
{
    public class ResultadoRuta
    {
        public Parada[] Paradas { get; }
        public double TiempoTotal { get; }
        public double CostoTotal { get; }

        public ResultadoRuta(
            Parada[] paradas,
            double tiempoTotal,
            double costoTotal)
        {
            Paradas = paradas;
            TiempoTotal = tiempoTotal;
            CostoTotal = costoTotal;
        }
    }
}