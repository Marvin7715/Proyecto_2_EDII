using TransRed.Core.Modelos;

namespace TransRed.Core.Servicios
{
    public interface IGestorTransporte
    {
        bool RegistrarParada(Parada parada);

        Parada? BuscarParada(string codigo);

        bool EliminarParada(string codigo);

        Parada[] SugerirParadas(string prefijo);

        bool RegistrarRuta(
            string codigoOrigen,
            string codigoDestino,
            double minutos,
            double costo,
            bool bidireccional);

        Parada[] BuscarRutaMasRapida(
            string codigoOrigen,
            string codigoDestino);

        Parada[] BuscarRutaMasEconomica(
            string codigoOrigen,
            string codigoDestino);

        Parada[] RecorrerBFS(string codigoInicio);

        Parada[] RecorrerDFS(string codigoInicio);

        bool EstanConectadas(
            string codigoOrigen,
            string codigoDestino);

        ResultadoRuta? ObtenerResultadoRuta(
            string codigoOrigen,
            string codigoDestino,
            bool usarCosto);
    }
}