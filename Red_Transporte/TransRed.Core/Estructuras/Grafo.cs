using System;
using System.Collections.Generic;
using TransRed.Core.Modelos;

namespace TransRed.Core.Estructuras
{
    public class Grafo
    {
        private class Vertice
        {
            public Parada Parada { get; }
            public Arista? PrimeraArista { get; set; }

            public Vertice(Parada parada)
            {
                Parada = parada;
                PrimeraArista = null;
            }
        }

        private class Arista
        {
            public int Destino { get; set; }
            public double Minutos { get; }
            public double Costo { get; }
            public Arista? Siguiente { get; set; }

            public Arista(int destino, double minutos, double costo)
            {
                Destino = destino;
                Minutos = minutos;
                Costo = costo;
                Siguiente = null;
            }
        }

        private Vertice?[] vertices;
        private int cantidad;

        public int Cantidad => cantidad;

        public Grafo(int capacidadInicial = 10)
        {
            if (capacidadInicial < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(capacidadInicial));

            vertices = new Vertice?[capacidadInicial];
            cantidad = 0;
        }

        public bool AgregarParada(Parada parada)
        {
            ArgumentNullException.ThrowIfNull(parada);

            if (BuscarIndice(parada.Codigo) != -1)
                return false;

            if (cantidad == vertices.Length)
                Array.Resize(ref vertices, vertices.Length * 2);

            vertices[cantidad] = new Vertice(parada);
            cantidad++;

            return true;
        }

        public bool EliminarParada(string codigo)
        {
            int indice = BuscarIndice(codigo);

            if (indice == -1)
                return false;

            for (int i = 0; i < cantidad; i++)
            {
                if (vertices[i] == null)
                    continue;

                Arista? anterior = null;
                Arista? actual = vertices[i]!.PrimeraArista;

                while (actual != null)
                {
                    Arista? siguiente = actual.Siguiente;

                    if (actual.Destino == indice)
                    {
                        if (anterior == null)
                            vertices[i]!.PrimeraArista = siguiente;
                        else
                            anterior.Siguiente = siguiente;
                    }
                    else
                    {
                        if (actual.Destino > indice)
                            actual.Destino--;

                        anterior = actual;
                    }

                    actual = siguiente;
                }
            }

            for (int i = indice; i < cantidad - 1; i++)
                vertices[i] = vertices[i + 1];

            cantidad--;
            vertices[cantidad] = null;

            return true;
        }

        public Parada? BuscarParada(string codigo)
        {
            int indice = BuscarIndice(codigo);

            return indice == -1 ? null : vertices[indice]!.Parada;
        }

        public bool RegistrarRuta(
            string codigoOrigen,
            string codigoDestino,
            double minutos,
            double costo,
            bool bidireccional)
        {
            if (double.IsNaN(minutos) ||
                double.IsInfinity(minutos) ||
                minutos < 0)
                return false;

            if (double.IsNaN(costo) ||
                double.IsInfinity(costo) ||
                costo < 0)
                return false;

            int origen = BuscarIndice(codigoOrigen);
            int destino = BuscarIndice(codigoDestino);

            if (origen == -1 || destino == -1 || origen == destino)
                return false;

            if (ExisteArista(origen, destino))
                return false;

            if (bidireccional && ExisteArista(destino, origen))
                return false;

            AgregarArista(origen, destino, minutos, costo);

            if (bidireccional)
                AgregarArista(destino, origen, minutos, costo);

            return true;
        }

        public bool EliminarRuta(
            string codigoOrigen,
            string codigoDestino,
            bool bidireccional)
        {
            int origen = BuscarIndice(codigoOrigen);
            int destino = BuscarIndice(codigoDestino);

            if (origen == -1 || destino == -1)
                return false;

            bool eliminada = EliminarArista(origen, destino);

            if (bidireccional)
                EliminarArista(destino, origen);

            return eliminada;
        }

        public Parada[] RecorrerBFS(string codigoInicio)
        {
            int inicio = BuscarIndice(codigoInicio);

            if (inicio == -1)
                return Array.Empty<Parada>();

            bool[] visitados = new bool[cantidad];
            Parada[] resultado = new Parada[cantidad];
            Queue<int> cola = new Queue<int>();
            int total = 0;

            visitados[inicio] = true;
            cola.Enqueue(inicio);

            while (cola.Count > 0)
            {
                int actual = cola.Dequeue();
                resultado[total++] = vertices[actual]!.Parada;

                Arista? arista = vertices[actual]!.PrimeraArista;

                while (arista != null)
                {
                    if (!visitados[arista.Destino])
                    {
                        visitados[arista.Destino] = true;
                        cola.Enqueue(arista.Destino);
                    }

                    arista = arista.Siguiente;
                }
            }

            return AjustarResultado(resultado, total);
        }

        public Parada[] RecorrerDFS(string codigoInicio)
        {
            int inicio = BuscarIndice(codigoInicio);

            if (inicio == -1)
                return Array.Empty<Parada>();

            bool[] visitados = new bool[cantidad];
            int[] pila = new int[cantidad];
            Parada[] resultado = new Parada[cantidad];
            int cima = 0;
            int total = 0;

            pila[cima++] = inicio;
            visitados[inicio] = true;

            while (cima > 0)
            {
                int actual = pila[--cima];
                resultado[total++] = vertices[actual]!.Parada;

                Arista? arista = vertices[actual]!.PrimeraArista;

                while (arista != null)
                {
                    if (!visitados[arista.Destino])
                    {
                        visitados[arista.Destino] = true;
                        pila[cima++] = arista.Destino;
                    }

                    arista = arista.Siguiente;
                }
            }

            return AjustarResultado(resultado, total);
        }

        public Parada[] BuscarRutaMasRapida(
            string codigoOrigen,
            string codigoDestino)
        {
            int origen = BuscarIndice(codigoOrigen);
            int destino = BuscarIndice(codigoDestino);

            if (origen == -1 || destino == -1)
                return Array.Empty<Parada>();

            double[] distancias = new double[cantidad];
            int[] anteriores = new int[cantidad];
            bool[] visitados = new bool[cantidad];

            for (int i = 0; i < cantidad; i++)
            {
                distancias[i] = double.PositiveInfinity;
                anteriores[i] = -1;
            }

            distancias[origen] = 0;

            for (int paso = 0; paso < cantidad; paso++)
            {
                int actual = -1;
                double menorDistancia = double.PositiveInfinity;

                for (int i = 0; i < cantidad; i++)
                {
                    if (!visitados[i] && distancias[i] < menorDistancia)
                    {
                        menorDistancia = distancias[i];
                        actual = i;
                    }
                }

                if (actual == -1)
                    break;

                if (actual == destino)
                    break;

                visitados[actual] = true;
                Arista? arista = vertices[actual]!.PrimeraArista;

                while (arista != null)
                {
                    int vecino = arista.Destino;
                    double nuevaDistancia =
                        distancias[actual] + arista.Minutos;

                    if (nuevaDistancia < distancias[vecino])
                    {
                        distancias[vecino] = nuevaDistancia;
                        anteriores[vecino] = actual;
                    }

                    arista = arista.Siguiente;
                }
            }

            if (double.IsPositiveInfinity(distancias[destino]))
                return Array.Empty<Parada>();

            int total = 0;

            for (int actual = destino;
                 actual != -1;
                 actual = anteriores[actual])
            {
                total++;
            }

            Parada[] ruta = new Parada[total];
            int posicion = total - 1;

            for (int actual = destino;
                 actual != -1;
                 actual = anteriores[actual])
            {
                ruta[posicion--] = vertices[actual]!.Parada;
            }

            return ruta;
        }

        public bool EstanConectadas(
            string codigoOrigen,
            string codigoDestino)
        {
            int origen = BuscarIndice(codigoOrigen);
            int destino = BuscarIndice(codigoDestino);

            if (origen == -1 || destino == -1)
                return false;

            UnionFind unionFind = new UnionFind(cantidad);

            for (int i = 0; i < cantidad; i++)
            {
                Arista? arista = vertices[i]!.PrimeraArista;

                while (arista != null)
                {
                    unionFind.Unir(i, arista.Destino);
                    arista = arista.Siguiente;
                }
            }

            return unionFind.EstanConectados(origen, destino);
        }

        public Parada[] ObtenerParadas()
        {
            Parada[] resultado = new Parada[cantidad];

            for (int i = 0; i < cantidad; i++)
                resultado[i] = vertices[i]!.Parada;

            return resultado;
        }

        private int BuscarIndice(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return -1;

            for (int i = 0; i < cantidad; i++)
            {
                if (string.Equals(
                    vertices[i]!.Parada.Codigo,
                    codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private void AgregarArista(
            int origen,
            int destino,
            double minutos,
            double costo)
        {
            Arista nueva = new Arista(destino, minutos, costo);
            nueva.Siguiente = vertices[origen]!.PrimeraArista;
            vertices[origen]!.PrimeraArista = nueva;
        }

        private bool ExisteArista(int origen, int destino)
        {
            Arista? actual = vertices[origen]!.PrimeraArista;

            while (actual != null)
            {
                if (actual.Destino == destino)
                    return true;

                actual = actual.Siguiente;
            }

            return false;
        }

        private bool EliminarArista(int origen, int destino)
        {
            Arista? anterior = null;
            Arista? actual = vertices[origen]!.PrimeraArista;

            while (actual != null)
            {
                if (actual.Destino == destino)
                {
                    if (anterior == null)
                        vertices[origen]!.PrimeraArista = actual.Siguiente;
                    else
                        anterior.Siguiente = actual.Siguiente;

                    return true;
                }

                anterior = actual;
                actual = actual.Siguiente;
            }

            return false;
        }

        private Parada[] AjustarResultado(
            Parada[] resultado,
            int total)
        {
            Parada[] ajustado = new Parada[total];

            for (int i = 0; i < total; i++)
                ajustado[i] = resultado[i];

            return ajustado;
        }
    }
}