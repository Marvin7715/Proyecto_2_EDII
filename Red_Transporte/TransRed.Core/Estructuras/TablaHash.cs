using System;
using TransRed.Core.Modelos;

namespace TransRed.Core.Estructuras
{
    public class TablaHash
    {
        private class Nodo
        {
            public Parada Valor { get; set; }
            public Nodo? Siguiente { get; set; }

            public Nodo(Parada valor)
            {
                Valor = valor;
                Siguiente = null;
            }
        }

        private readonly Nodo?[] cubetas;

        public TablaHash(int capacidad = 101)
        {
            if (capacidad <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacidad),
                    "La capacidad debe ser mayor que cero.");

            cubetas = new Nodo?[capacidad];
        }

        private int CalcularIndice(string codigo)
        {
            int hash = 0;

            unchecked
            {
                foreach (char caracter in codigo)
                {
                    hash = hash * 31 + char.ToUpperInvariant(caracter);
                }
            }

            return (int)((uint)hash % (uint)cubetas.Length);
        }

        public bool Insertar(Parada parada)
        {
            ArgumentNullException.ThrowIfNull(parada);

            int indice = CalcularIndice(parada.Codigo);
            Nodo? actual = cubetas[indice];

            while (actual != null)
            {
                if (string.Equals(
                    actual.Valor.Codigo,
                    parada.Codigo,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                actual = actual.Siguiente;
            }

            Nodo nuevo = new Nodo(parada);
            nuevo.Siguiente = cubetas[indice];
            cubetas[indice] = nuevo;

            return true;
        }

        public Parada? Buscar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            int indice = CalcularIndice(codigo.Trim());
            Nodo? actual = cubetas[indice];

            while (actual != null)
            {
                if (string.Equals(
                    actual.Valor.Codigo,
                    codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Valor;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        public bool Eliminar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            int indice = CalcularIndice(codigo.Trim());
            Nodo? actual = cubetas[indice];
            Nodo? anterior = null;

            while (actual != null)
            {
                if (string.Equals(
                    actual.Valor.Codigo,
                    codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (anterior == null)
                        cubetas[indice] = actual.Siguiente;
                    else
                        anterior.Siguiente = actual.Siguiente;

                    return true;
                }

                anterior = actual;
                actual = actual.Siguiente;
            }

            return false;
        }

        public Parada[] ObtenerTodas()
        {
            int cantidad = 0;

            foreach (Nodo? primero in cubetas)
            {
                Nodo? actual = primero;

                while (actual != null)
                {
                    cantidad++;
                    actual = actual.Siguiente;
                }
            }

            Parada[] paradas = new Parada[cantidad];
            int posicion = 0;

            foreach (Nodo? primero in cubetas)
            {
                Nodo? actual = primero;

                while (actual != null)
                {
                    paradas[posicion] = actual.Valor;
                    posicion++;
                    actual = actual.Siguiente;
                }
            }

            return paradas;
        }
    }
}