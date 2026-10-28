using System;

namespace TransRed.Core.Estructuras
{
    public class UnionFind
    {
        private readonly int[] padres;
        private readonly int[] rangos;

        public int Cantidad { get; }

        public UnionFind(int cantidad)
        {
            if (cantidad < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(cantidad),
                    "La cantidad no puede ser negativa.");

            Cantidad = cantidad;
            padres = new int[cantidad];
            rangos = new int[cantidad];

            for (int i = 0; i < cantidad; i++)
            {
                padres[i] = i;
                rangos[i] = 0;
            }
        }

        public int Encontrar(int elemento)
        {
            ValidarElemento(elemento);

            if (padres[elemento] != elemento)
            {
                padres[elemento] = Encontrar(padres[elemento]);
            }

            return padres[elemento];
        }

        public bool Unir(int elementoA, int elementoB)
        {
            int raizA = Encontrar(elementoA);
            int raizB = Encontrar(elementoB);

            if (raizA == raizB)
                return false;

            if (rangos[raizA] < rangos[raizB])
            {
                padres[raizA] = raizB;
            }
            else if (rangos[raizA] > rangos[raizB])
            {
                padres[raizB] = raizA;
            }
            else
            {
                padres[raizB] = raizA;
                rangos[raizA]++;
            }

            return true;
        }

        public bool EstanConectados(int elementoA, int elementoB)
        {
            return Encontrar(elementoA) == Encontrar(elementoB);
        }

        private void ValidarElemento(int elemento)
        {
            if (elemento < 0 || elemento >= Cantidad)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elemento),
                    "El elemento está fuera del rango permitido.");
            }
        }
    }
}