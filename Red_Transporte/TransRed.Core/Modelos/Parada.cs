namespace TransRed.Core.Modelos
{
    public class Parada
    {
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }

        public Parada(string codigo, string nombre, double latitud, double longitud)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código de la parada es obligatorio.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la parada es obligatorio.");

            Codigo = codigo.Trim();
            Nombre = nombre.Trim();
            Latitud = latitud;
            Longitud = longitud;
        }

        public override string ToString()
        {
            return $"{Codigo} - {Nombre}";
        }
    }
}