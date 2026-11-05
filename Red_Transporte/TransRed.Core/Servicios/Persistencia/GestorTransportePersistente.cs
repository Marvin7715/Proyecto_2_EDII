
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;

namespace TransRed.Core.Servicios.Persistencia
{
    public class GestorTransportePersistente : IGestorTransporte
    {
        private GestorTransporte gestor;
        private readonly AlmacenamientoJson almacenamiento;
        private readonly string rutaArchivo;
        private string nombreRed;

        public GestorTransportePersistente(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo es obligatoria.");

            this.rutaArchivo = Path.GetFullPath(rutaArchivo);

            almacenamiento = new AlmacenamientoJson();
            gestor = new GestorTransporte();

            nombreRed = "Red de transporte del suroccidente de Guatemala";

            DatosRed? datos = almacenamiento.Cargar(this.rutaArchivo);

            if (datos != null)
            {
                gestor = ConstruirGestor(datos);
                nombreRed = datos.Nombre;
            }
            else
            {
                Guardar();
            }
        }

        public string RutaArchivo => rutaArchivo;

        public void Guardar()
        {
            DatosRed datos = ObtenerDatos();

            almacenamiento.Guardar(datos, rutaArchivo);
        }

        public void Exportar(string rutaDestino)
        {
            DatosRed datos = ObtenerDatos();

            almacenamiento.Exportar(datos, rutaDestino);
        }

        public void Importar(string rutaOrigen)
        {
            DatosRed? datos = almacenamiento.Importar(rutaOrigen);

            if (datos == null)
                throw new InvalidDataException(
                    "No fue posible leer el archivo JSON.");

            GestorTransporte nuevoGestor = ConstruirGestor(datos);

            gestor = nuevoGestor;
            nombreRed = datos.Nombre;

            Guardar();
        }

        private DatosRed ObtenerDatos()
        {
            Parada[] paradas = gestor.ObtenerTodasParadas();
            Conexion[] conexiones = gestor.ObtenerTodasConexiones();

            ParadaJson[] paradasJson = new ParadaJson[paradas.Length];

            for (int i = 0; i < paradas.Length; i++)
            {
                paradasJson[i] = new ParadaJson
                {
                    Codigo = paradas[i].Codigo,
                    Nombre = paradas[i].Nombre,
                    Latitud = paradas[i].Latitud,
                    Longitud = paradas[i].Longitud
                };
            }

            ConexionJson[] conexionesJson =
                new ConexionJson[conexiones.Length];

            for (int i = 0; i < conexiones.Length; i++)
            {
                conexionesJson[i] = new ConexionJson
                {
                    Origen = conexiones[i].CodigoOrigen,
                    Destino = conexiones[i].CodigoDestino,
                    Minutos = conexiones[i].Minutos,
                    Costo = conexiones[i].Costo
                };
            }

            return new DatosRed
            {
                Nombre = nombreRed,
                Version = 1,
                Paradas = paradasJson,
                Conexiones = conexionesJson
            };
        }

        private static GestorTransporte ConstruirGestor(DatosRed datos)
        {
            if (datos.Version != 1)
                throw new InvalidDataException(
                    "La versión del archivo JSON no es compatible.");

            if (datos.Paradas == null || datos.Conexiones == null)
                throw new InvalidDataException(
                    "El archivo no contiene las colecciones necesarias.");

            GestorTransporte nuevoGestor = new GestorTransporte();

            foreach (ParadaJson registro in datos.Paradas)
            {
                if (registro == null ||
                    string.IsNullOrWhiteSpace(registro.Codigo) ||
                    string.IsNullOrWhiteSpace(registro.Nombre) ||
                    !double.IsFinite(registro.Latitud) ||
                    !double.IsFinite(registro.Longitud) ||
                    registro.Latitud < -90 ||
                    registro.Latitud > 90 ||
                    registro.Longitud < -180 ||
                    registro.Longitud > 180)
                {
                    throw new InvalidDataException(
                        "Se encontró una parada con datos inválidos.");
                }

                Parada parada = new Parada(
                    registro.Codigo,
                    registro.Nombre,
                    registro.Latitud,
                    registro.Longitud);

                if (!nuevoGestor.RegistrarParada(parada))
                {
                    throw new InvalidDataException(
                        $"No se pudo registrar la parada {registro.Codigo}.");
                }
            }

            foreach (ConexionJson registro in datos.Conexiones)
            {
                if (registro == null ||
                    string.IsNullOrWhiteSpace(registro.Origen) ||
                    string.IsNullOrWhiteSpace(registro.Destino) ||
                    !double.IsFinite(registro.Minutos) ||
                    !double.IsFinite(registro.Costo) ||
                    registro.Minutos < 0 ||
                    registro.Costo < 0)
                {
                    throw new InvalidDataException(
                        "Se encontró una conexión con datos inválidos.");
                }

                bool registrada = nuevoGestor.RegistrarRuta(
                    registro.Origen,
                    registro.Destino,
                    registro.Minutos,
                    registro.Costo,
                    false);

                if (!registrada)
                {
                    throw new InvalidDataException(
                        $"No se pudo registrar la conexión " +
                        $"{registro.Origen} - {registro.Destino}.");
                }
            }

            return nuevoGestor;
        }

        public bool RegistrarParada(Parada parada)
        {
            bool resultado = gestor.RegistrarParada(parada);

            if (resultado)
                Guardar();

            return resultado;
        }

        public Parada? BuscarParada(string codigo)
        {
            return gestor.BuscarParada(codigo);
        }

        public bool EliminarParada(string codigo)
        {
            bool resultado = gestor.EliminarParada(codigo);

            if (resultado)
                Guardar();

            return resultado;
        }

        public Parada[] SugerirParadas(string prefijo)
        {
            return gestor.SugerirParadas(prefijo);
        }

        public bool RegistrarRuta(
            string codigoOrigen,
            string codigoDestino,
            double minutos,
            double costo,
            bool bidireccional)
        {
            bool resultado = gestor.RegistrarRuta(
                codigoOrigen,
                codigoDestino,
                minutos,
                costo,
                bidireccional);

            if (resultado)
                Guardar();

            return resultado;
        }

        public bool EliminarRuta(
            string codigoOrigen,
            string codigoDestino,
            bool bidireccional)
        {
            bool resultado = gestor.EliminarRuta(
                codigoOrigen,
                codigoDestino,
                bidireccional);

            if (resultado)
                Guardar();

            return resultado;
        }

        public Parada[] BuscarRutaMasRapida(
            string codigoOrigen,
            string codigoDestino)
        {
            return gestor.BuscarRutaMasRapida(
                codigoOrigen,
                codigoDestino);
        }

        public Parada[] BuscarRutaMasEconomica(
            string codigoOrigen,
            string codigoDestino)
        {
            return gestor.BuscarRutaMasEconomica(
                codigoOrigen,
                codigoDestino);
        }

        public Parada[] RecorrerBFS(string codigoInicio)
        {
            return gestor.RecorrerBFS(codigoInicio);
        }

        public Parada[] RecorrerDFS(string codigoInicio)
        {
            return gestor.RecorrerDFS(codigoInicio);
        }

        public bool EstanConectadas(
            string codigoOrigen,
            string codigoDestino)
        {
            return gestor.EstanConectadas(
                codigoOrigen,
                codigoDestino);
        }

        public ResultadoRuta? ObtenerResultadoRuta(
            string codigoOrigen,
            string codigoDestino,
            bool usarCosto)
        {
            return gestor.ObtenerResultadoRuta(
                codigoOrigen,
                codigoDestino,
                usarCosto);
        }

        public Parada[] ObtenerTodasParadas()
        {
            return gestor.ObtenerTodasParadas();
        }

        public Conexion[] ObtenerTodasConexiones()
        {
            return gestor.ObtenerTodasConexiones();
        }
    }
}
