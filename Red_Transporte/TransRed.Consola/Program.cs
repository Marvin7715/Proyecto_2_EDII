using System.Globalization;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;

GestorTransporte gestor = new GestorTransporte();

bool continuar = true;

while (continuar)
{
    Console.Clear();
    MostrarMenu();

    Console.Write("Seleccione una opción: ");
    string opcion = Console.ReadLine() ?? "";

    Console.WriteLine();

    switch (opcion)
    {
        case "1":
            RegistrarParada();
            break;

        case "2":
            BuscarParada();
            break;

        case "3":
            EliminarParada();
            break;

        case "4":
            SugerirParadas();
            break;

        case "5":
            MostrarParadas();
            break;

        case "6":
            RegistrarRuta();
            break;

        case "7":
            BuscarRutaMasRapida();
            break;

        case "8":
            BuscarRutaMasEconomica();
            break;

        case "9":
            RecorrerBFS();
            break;

        case "10":
            RecorrerDFS();
            break;

        case "11":
            VerificarConexion();
            break;

        case "0":
            continuar = false;
            Console.WriteLine("Gracias por utilizar TransRed.");
            break;

        default:
            Console.WriteLine("Opción no válida.");
            break;
    }

    if (continuar)
    {
        Console.WriteLine();
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
    }
}

void MostrarMenu()
{
    Console.WriteLine("======================================");
    Console.WriteLine("               TRANSRED");
    Console.WriteLine("       RED DE TRANSPORTE URBANO");
    Console.WriteLine("======================================");
    Console.WriteLine("1. Registrar parada");
    Console.WriteLine("2. Buscar parada");
    Console.WriteLine("3. Eliminar parada");
    Console.WriteLine("4. Sugerir paradas por nombre");
    Console.WriteLine("5. Mostrar todas las paradas");
    Console.WriteLine("6. Registrar ruta");
    Console.WriteLine("7. Buscar ruta más rápida");
    Console.WriteLine("8. Buscar ruta más económica");
    Console.WriteLine("9. Recorrer red mediante BFS");
    Console.WriteLine("10. Recorrer red mediante DFS");
    Console.WriteLine("11. Verificar conexión entre paradas");
    Console.WriteLine("0. Salir");
    Console.WriteLine("======================================");
}

void RegistrarParada()
{
    Console.WriteLine("REGISTRAR PARADA");

    string codigo = LeerTexto("Código: ");
    string nombre = LeerTexto("Nombre: ");
    double latitud = LeerNumero("Latitud: ");
    double longitud = LeerNumero("Longitud: ");

    Parada parada = new Parada(
        codigo,
        nombre,
        latitud,
        longitud);

    if (gestor.RegistrarParada(parada))
        Console.WriteLine("Parada registrada correctamente.");
    else
        Console.WriteLine("No fue posible registrar la parada. Verifique que el código y nombre no estén duplicados.");
}

void BuscarParada()
{
    Console.WriteLine("BUSCAR PARADA");

    string codigo = LeerTexto("Ingrese el código: ");

    Parada? parada = gestor.BuscarParada(codigo);

    if (parada == null)
    {
        Console.WriteLine("No se encontró la parada.");
        return;
    }

    MostrarDatosParada(parada);
}

void EliminarParada()
{
    Console.WriteLine("ELIMINAR PARADA");

    string codigo = LeerTexto("Ingrese el código: ");

    if (gestor.EliminarParada(codigo))
        Console.WriteLine("Parada eliminada correctamente.");
    else
        Console.WriteLine("No se encontró la parada.");
}

void SugerirParadas()
{
    Console.WriteLine("SUGERIR PARADAS");

    string prefijo = LeerTexto("Ingrese el inicio del nombre: ");

    Parada[] resultados = gestor.SugerirParadas(prefijo);

    if (resultados.Length == 0)
    {
        Console.WriteLine("No se encontraron coincidencias.");
        return;
    }

    foreach (Parada parada in resultados)
        MostrarDatosParada(parada);
}

void MostrarParadas()
{
    Console.WriteLine("PARADAS REGISTRADAS");

    Parada[] paradas = gestor.ObtenerTodasParadas();

    if (paradas.Length == 0)
    {
        Console.WriteLine("No hay paradas registradas.");
        return;
    }

    foreach (Parada parada in paradas)
        MostrarDatosParada(parada);
}

void RegistrarRuta()
{
    Console.WriteLine("REGISTRAR RUTA");

    string origen = LeerTexto("Código de parada de origen: ");
    string destino = LeerTexto("Código de parada de destino: ");
    double minutos = LeerNumero("Tiempo del recorrido en minutos: ");
    double costo = LeerNumero("Costo del recorrido: ");

    bool bidireccional = LeerSiNo(
        "¿La ruta funciona en ambos sentidos? (S/N): ");

    if (gestor.RegistrarRuta(
        origen,
        destino,
        minutos,
        costo,
        bidireccional))
    {
        Console.WriteLine("Ruta registrada correctamente.");
    }
    else
    {
        Console.WriteLine("No fue posible registrar la ruta. Verifique las paradas y los valores ingresados.");
    }
}

void BuscarRutaMasRapida()
{
    Console.WriteLine("BUSCAR RUTA MÁS RÁPIDA");

    string origen = LeerTexto("Código de origen: ");
    string destino = LeerTexto("Código de destino: ");

    Parada[] ruta = gestor.BuscarRutaMasRapida(origen, destino);

    MostrarRuta(ruta);
}

void BuscarRutaMasEconomica()
{
    Console.WriteLine("BUSCAR RUTA MÁS ECONÓMICA");

    string origen = LeerTexto("Código de origen: ");
    string destino = LeerTexto("Código de destino: ");

    Parada[] ruta = gestor.BuscarRutaMasEconomica(origen, destino);

    MostrarRuta(ruta);
}

void RecorrerBFS()
{
    Console.WriteLine("RECORRIDO BFS");

    string inicio = LeerTexto("Código de parada inicial: ");

    Parada[] recorrido = gestor.RecorrerBFS(inicio);

    MostrarRecorrido(recorrido);
}

void RecorrerDFS()
{
    Console.WriteLine("RECORRIDO DFS");

    string inicio = LeerTexto("Código de parada inicial: ");

    Parada[] recorrido = gestor.RecorrerDFS(inicio);

    MostrarRecorrido(recorrido);
}

void VerificarConexion()
{
    Console.WriteLine("VERIFICAR CONEXIÓN");

    string origen = LeerTexto("Código de origen: ");
    string destino = LeerTexto("Código de destino: ");

    if (gestor.EstanConectadas(origen, destino))
        Console.WriteLine("Las paradas pertenecen a la misma zona conectada.");
    else
        Console.WriteLine("Las paradas no están conectadas o alguno de los códigos no existe.");
}

void MostrarRuta(Parada[] ruta)
{
    if (ruta.Length == 0)
    {
        Console.WriteLine("No se encontró una ruta disponible.");
        return;
    }

    Console.WriteLine("Recorrido encontrado:");

    for (int i = 0; i < ruta.Length; i++)
    {
        Console.Write(ruta[i].Codigo);

        if (i < ruta.Length - 1)
            Console.Write(" -> ");
    }

    Console.WriteLine();
}

void MostrarRecorrido(Parada[] recorrido)
{
    if (recorrido.Length == 0)
    {
        Console.WriteLine("No hay paradas para recorrer desde ese punto.");
        return;
    }

    foreach (Parada parada in recorrido)
        Console.WriteLine($"{parada.Codigo} - {parada.Nombre}");
}

void MostrarDatosParada(Parada parada)
{
    Console.WriteLine("--------------------------------------");
    Console.WriteLine($"Código: {parada.Codigo}");
    Console.WriteLine($"Nombre: {parada.Nombre}");
    Console.WriteLine($"Latitud: {parada.Latitud}");
    Console.WriteLine($"Longitud: {parada.Longitud}");
}

string LeerTexto(string mensaje)
{
    string valor;

    do
    {
        Console.Write(mensaje);
        valor = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(valor))
            Console.WriteLine("Este campo es obligatorio.");

    } while (string.IsNullOrWhiteSpace(valor));

    return valor.Trim();
}

double LeerNumero(string mensaje)
{
    double valor;

    while (true)
    {
        Console.Write(mensaje);
        string entrada = Console.ReadLine() ?? "";

        if (double.TryParse(
            entrada,
            NumberStyles.Float,
            CultureInfo.CurrentCulture,
            out valor) &&
            double.IsFinite(valor))
        {
            return valor;
        }

        Console.WriteLine("Ingrese un número válido.");
    }
}

bool LeerSiNo(string mensaje)
{
    while (true)
    {
        Console.Write(mensaje);
        string respuesta = Console.ReadLine() ?? "";

        if (respuesta.Equals("S", StringComparison.OrdinalIgnoreCase))
            return true;

        if (respuesta.Equals("N", StringComparison.OrdinalIgnoreCase))
            return false;

        Console.WriteLine("Ingrese S o N.");
    }
}