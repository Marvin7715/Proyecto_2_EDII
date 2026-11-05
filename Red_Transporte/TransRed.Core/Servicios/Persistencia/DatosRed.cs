
namespace TransRed.Core.Servicios.Persistencia
{
    public class DatosRed
    {
        public string Nombre { get; set; } =
            "Red de transporte del suroccidente de Guatemala";

        public int Version { get; set; } = 1;

        public ParadaJson[] Paradas { get; set; } =
            Array.Empty<ParadaJson>();

        public ConexionJson[] Conexiones { get; set; } =
            Array.Empty<ConexionJson>();
    }

    public class ParadaJson
    {
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }

    public class ConexionJson
    {
        public string Origen { get; set; } = "";
        public string Destino { get; set; } = "";
        public double Minutos { get; set; }
        public double Costo { get; set; }
    }
}
