
namespace TransRed.Core.Servicios.Persistencia
{
    public static class RutasTransRed
    {
        public static string ObtenerArchivoPrincipal()
        {
            DirectoryInfo? directorio =
                new DirectoryInfo(AppContext.BaseDirectory);

            while (directorio != null)
            {
                string rutaSolucion = Path.Combine(
                    directorio.FullName,
                    "Red_Transporte.slnx");

                if (File.Exists(rutaSolucion))
                {
                    string carpetaDatos = Path.Combine(
                        directorio.FullName,
                        "Datos");

                    Directory.CreateDirectory(carpetaDatos);

                    return Path.Combine(
                        carpetaDatos,
                        "transred.json");
                }

                directorio = directorio.Parent;
            }

            string carpetaAlternativa = Path.Combine(
                AppContext.BaseDirectory,
                "Datos");

            Directory.CreateDirectory(carpetaAlternativa);

            return Path.Combine(
                carpetaAlternativa,
                "transred.json");
        }
    }
}
