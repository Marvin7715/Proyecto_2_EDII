using System;
using TransRed.Core.Modelos;

namespace TransRed.Core.Estructuras
{
    public class Trie
    {
        private class Nodo
        {
            public char Caracter { get; }
            public Nodo? PrimerHijo { get; set; }
            public Nodo? SiguienteHermano { get; set; }
            public Parada? Parada { get; set; }

            public Nodo(char caracter)
            {
                Caracter = caracter;
                PrimerHijo = null;
                SiguienteHermano = null;
                Parada = null;
            }
        }

        private readonly Nodo raiz = new Nodo('\0');

        public bool Insertar(Parada parada)
        {
            ArgumentNullException.ThrowIfNull(parada);

            string nombre = Normalizar(parada.Nombre);
            Nodo actual = raiz;

            foreach (char caracter in nombre)
            {
                Nodo? hijo = BuscarHijo(actual, caracter);

                if (hijo == null)
                {
                    hijo = new Nodo(caracter);
                    hijo.SiguienteHermano = actual.PrimerHijo;
                    actual.PrimerHijo = hijo;
                }

                actual = hijo;
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
                Nodo? hijo = BuscarHijo(actual, caracter);

                if (hijo == null)
                    return Array.Empty<Parada>();

                actual = hijo;
            }

            Parada[] resultados = Array.Empty<Parada>();
            Recopilar(actual, ref resultados);

            return resultados;
        }

        public bool Eliminar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            Nodo? terminal = BuscarNodo(Normalizar(nombre));

            if (terminal == null || terminal.Parada == null)
                return false;

            terminal.Parada = null;
            return true;
        }

        private Nodo? BuscarHijo(Nodo padre, char caracter)
        {
            Nodo? actual = padre.PrimerHijo;

            while (actual != null)
            {
                if (actual.Caracter == caracter)
                    return actual;

                actual = actual.SiguienteHermano;
            }

            return null;
        }

        private Nodo? BuscarNodo(string texto)
        {
            Nodo actual = raiz;

            foreach (char caracter in texto)
            {
                Nodo? hijo = BuscarHijo(actual, caracter);

                if (hijo == null)
                    return null;

                actual = hijo;
            }

            return actual;
        }

        private void Recopilar(
            Nodo nodo,
            ref Parada[] resultados)
        {
            if (nodo.Parada != null)
            {
                int longitud = resultados.Length;
                Array.Resize(ref resultados, longitud + 1);
                resultados[longitud] = nodo.Parada;
            }

            Nodo? hijo = nodo.PrimerHijo;

            while (hijo != null)
            {
                Recopilar(hijo, ref resultados);
                hijo = hijo.SiguienteHermano;
            }
        }

        private string Normalizar(string texto)
        {
            return texto.Trim().ToUpperInvariant();
        }
    }
}