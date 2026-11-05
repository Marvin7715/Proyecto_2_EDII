
using System.Text.Json;

namespace TransRed.Core.Servicios.Persistencia
{
    public class AlmacenamientoJson
    {
        private readonly JsonSerializerOptions opciones;

        public AlmacenamientoJson()
        {
            opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public DatosRed? Cargar(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                throw new ArgumentException("La ruta del archivo es obligatoria.");

            if (!File.Exists(ruta))
                return null;

            string contenido = File.ReadAllText(ruta);

            if (string.IsNullOrWhiteSpace(contenido))
                return null;

            DatosRed? datos = JsonSerializer.Deserialize<DatosRed>(
                contenido,
                opciones);

            return datos;
        }

        public void Guardar(DatosRed datos, string ruta)
        {
            if (datos == null)
                throw new ArgumentNullException(nameof(datos));

            if (string.IsNullOrWhiteSpace(ruta))
                throw new ArgumentException("La ruta del archivo es obligatoria.");

            string? directorio = Path.GetDirectoryName(ruta);

            if (!string.IsNullOrWhiteSpace(directorio))
                Directory.CreateDirectory(directorio);

            string contenido = JsonSerializer.Serialize(datos, opciones);

            string rutaTemporal = ruta + ".tmp";

            try
            {
                File.WriteAllText(rutaTemporal, contenido);
                File.Move(rutaTemporal, ruta, true);
            }
            finally
            {
                if (File.Exists(rutaTemporal))
                    File.Delete(rutaTemporal);
            }
        }

        public void Exportar(DatosRed datos, string rutaDestino)
        {
            Guardar(datos, rutaDestino);
        }

        public DatosRed? Importar(string rutaOrigen)
        {
            return Cargar(rutaOrigen);
        }
    }
}
