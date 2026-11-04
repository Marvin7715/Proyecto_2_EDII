namespace TransRed.Core.Modelos
{
    public class Conexion
    {
        public string CodigoOrigen { get; }
        public string CodigoDestino { get; }
        public double Minutos { get; }
        public double Costo { get; }

        public Conexion(
            string codigoOrigen,
            string codigoDestino,
            double minutos,
            double costo)
        {
            CodigoOrigen = codigoOrigen;
            CodigoDestino = codigoDestino;
            Minutos = minutos;
            Costo = costo;
        }
    }
}