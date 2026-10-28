using TransRed.Core.Estructuras;
using TransRed.Core.Modelos;

namespace TransRed.Core.Servicios
{
    public class GestorTransporte : IGestorTransporte
    {
        private readonly TablaHash tablaParadas;
        private readonly Trie trieParadas;
        private readonly Grafo grafo;

        public GestorTransporte()
        {
            tablaParadas = new TablaHash();
            trieParadas = new Trie();
            grafo = new Grafo();
        }

        public bool RegistrarParada(Parada parada)
        {
            if (parada == null)
                return false;

            if (tablaParadas.Buscar(parada.Codigo) != null)
                return false;

            if (!tablaParadas.Insertar(parada))
                return false;

            if (!trieParadas.Insertar(parada))
            {
                tablaParadas.Eliminar(parada.Codigo);
                return false;
            }

            if (!grafo.AgregarParada(parada))
            {
                trieParadas.Eliminar(parada.Nombre);
                tablaParadas.Eliminar(parada.Codigo);
                return false;
            }

            return true;
        }

        public Parada? BuscarParada(string codigo)
        {
            return tablaParadas.Buscar(codigo);
        }

        public bool EliminarParada(string codigo)
        {
            Parada? parada = tablaParadas.Buscar(codigo);

            if (parada == null)
                return false;

            if (!grafo.EliminarParada(codigo))
                return false;

            tablaParadas.Eliminar(codigo);
            trieParadas.Eliminar(parada.Nombre);

            return true;
        }

        public Parada[] SugerirParadas(string prefijo)
        {
            return trieParadas.BuscarPorPrefijo(prefijo);
        }

        public bool RegistrarRuta(
            string codigoOrigen,
            string codigoDestino,
            double minutos,
            double costo,
            bool bidireccional)
        {
            if (tablaParadas.Buscar(codigoOrigen) == null)
                return false;

            if (tablaParadas.Buscar(codigoDestino) == null)
                return false;

            return grafo.RegistrarRuta(
                codigoOrigen,
                codigoDestino,
                minutos,
                costo,
                bidireccional);
        }

        public Parada[] BuscarRutaMasRapida(
            string codigoOrigen,
            string codigoDestino)
        {
            return grafo.BuscarRutaMasRapida(
                codigoOrigen,
                codigoDestino);
        }

        public Parada[] RecorrerBFS(string codigoInicio)
        {
            return grafo.RecorrerBFS(codigoInicio);
        }

        public Parada[] RecorrerDFS(string codigoInicio)
        {
            return grafo.RecorrerDFS(codigoInicio);
        }

        public bool EstanConectadas(
            string codigoOrigen,
            string codigoDestino)
        {
            return grafo.EstanConectadas(
                codigoOrigen,
                codigoDestino);
        }
    }
}