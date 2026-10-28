using System;
using TransRed.Core.Modelos;

namespace TransRed.Core.Estructuras
{
    public class Trie
    {
        private class Nodo
        {
            public Nodo?[] Hijos { get; } = new Nodo?[char.MaxValue + 1];

            public Parada? Parada { get; set; }
        }

        private readonly Nodo raiz = new Nodo();

        public bool Insertar(Parada parada)
        {
            ArgumentNullException.ThrowIfNull(parada);

            string nombre = Normalizar(parada.Nombre);
            Nodo actual = raiz;

            foreach (char caracter in nombre)
            {
                int indice = caracter;

                if (actual.Hijos[indice] == null)
                    actual.Hijos[indice] = new Nodo();

                actual = actual.Hijos[indice]!;
            }

            if (actual.Parada != null)
                return false;

            actual.Parada = parada;
            return true;
        }

        public Parada[] BuscarPorPrefijo(string prefijo)
        {
            if (string.IsNullOrWhiteSpace(prefijo))
                return Array.Empty<Parada>();

            string texto = Normalizar(prefijo);
            Nodo actual = raiz;

            foreach (char caracter in texto)
            {
                actual = actual.Hijos[caracter]!;

                if (actual == null)
                    return Array.Empty<Parada>();
            }

            Parada[] resultados = new Parada[0];
            Recopilar(actual, ref resultados);

            return resultados;
        }

        public bool Eliminar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            Nodo? terminal = BuscarNodo(Normalizar(nombre));

            if (terminal?.Parada == null)
                return false;

            terminal.Parada = null;
            return true;
        }

        private Nodo? BuscarNodo(string texto)
        {
            Nodo actual = raiz;

            foreach (char caracter in texto)
            {
                Nodo? siguiente = actual.Hijos[caracter];

                if (siguiente == null)
                    return null;

                actual = siguiente;
            }

            return actual;
        }

        private void Recopilar(Nodo nodo, ref Parada[] resultados)
        {
            if (nodo.Parada != null)
            {
                int longitud = resultados.Length;
                Array.Resize(ref resultados, longitud + 1);
                resultados[longitud] = nodo.Parada;
            }

            foreach (Nodo? hijo in nodo.Hijos)
            {
                if (hijo != null)
                    Recopilar(hijo, ref resultados);
            }
        }

        private string Normalizar(string texto)
        {
            return texto.Trim().ToUpperInvariant();
        }
    }
}