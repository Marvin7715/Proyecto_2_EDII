# TransRed — Red de transporte del suroccidente de Guatemala

**Proyecto de Laboratorio No. 2 | Estructura de Datos II**  

## Descripción del proyecto

**TransRed** es una aplicación académica que modela una red de transporte del suroccidente de Guatemala. Permite administrar paradas y conexiones, buscar paradas por código o por el inicio de su nombre, recorrer la red y calcular rutas según dos criterios: **menor tiempo** y **menor costo**.

El objetivo es aplicar e integrar estructuras de datos en un escenario concreto. El proyecto cuenta con una **interfaz de escritorio en Windows Forms**, un **programa de consola** y una biblioteca que concentra la lógica de datos y algoritmos.

> **Alcance:** los trayectos, tiempos y costos del archivo de prueba constituyen un escenario académico. El mapa de la aplicación representa visualmente la red; sus curvas no deben interpretarse como vías cartográficas exactas ni como horarios o tarifas oficiales.

## Funcionalidades

- **Paradas:** registrar, consultar por código, listar y eliminar paradas con nombre y coordenadas.
- **Conexiones:** registrar y eliminar trayectos entre paradas, con tiempo (minutos), costo (quetzales) y opción bidireccional.
- **Búsqueda predictiva:** sugerir paradas mediante prefijos del nombre, utilizando un **Trie**; el selector también permite localizar códigos.
- **Explorar red:** seleccionar origen y destino, calcular la ruta más rápida o la más económica y visualizar el itinerario sobre un mapa interactivo.
- **Algoritmos:** ejecutar y visualizar el orden de visita de **BFS** y **DFS**, y comprobar conectividad con **Union-Find**.
- **Persistencia:** cargar y guardar los cambios de paradas y conexiones en un archivo **JSON**.

## Estructuras de datos utilizadas

| Estructura | Aplicación en TransRed | ¿Por qué se eligió? |
| --- | --- | --- |
| **Tabla Hash** | Almacenar y recuperar paradas por su código. | Permite búsquedas rápidas por una clave identificadora, sin recorrer todas las paradas en condiciones habituales. |
| **Trie** | Autocompletar nombres de paradas por prefijo. | Aprovecha los caracteres compartidos entre nombres y facilita consultar las coincidencias de un prefijo. |
| **Grafo** | Representar paradas como vértices y conexiones como aristas con tiempo y costo. | Modela directamente las relaciones de transporte y permite recorridos y búsqueda de rutas óptimas. |
| **Union-Find** | Comprobar si dos paradas pertenecen a la misma componente conectada. | Gestiona conjuntos de vértices mediante unión por rango y compresión de caminos. |

### Algoritmos integrados

- **BFS (búsqueda en anchura):** visita las paradas alcanzables por niveles mediante una cola.
- **DFS (búsqueda en profundidad):** explora la red mediante una pila.
- **Dijkstra:** obtiene una ruta de peso mínimo usando, alternativamente, los minutos o el costo de las conexiones.
- **Union-Find:** agrupa los vértices conectados y permite responder consultas de pertenencia a una misma componente.

**Consideraciones de diseño:** la tabla hash usa cubetas con encadenamiento para colisiones; el Trie representa sus nodos con referencias al primer hijo y al siguiente hermano; el grafo utiliza listas de adyacencia. Dijkstra selecciona el siguiente vértice mediante una búsqueda lineal, una solución sencilla para una red pequeña, aunque menos escalable que una cola de prioridad. En la consulta de Union-Find, las aristas se consideran conexiones para agrupar componentes, independientemente de su sentido: esta consulta no equivale a comprobar si existe una ruta *dirigida* de A hacia B.

## Datos de prueba incluidos

El escenario inicial está en [`Red_Transporte/Datos/transred.json`](Red_Transporte/Datos/transred.json) y contiene:

| Elemento | Cantidad |
| --- | ---: |
| Paradas registradas | **35** |
| Conexiones dirigidas almacenadas | **102** |
| Pares de conexiones bidireccionales | **51** |

Cada parada contiene `Codigo`, `Nombre`, `Latitud` y `Longitud`. Cada conexión contiene `Origen`, `Destino`, `Minutos` y `Costo`.

Ejemplos de paradas disponibles: **XELA** (Quetzaltenango), **CC** (Cuatro Caminos), **HUE** (Huehuetenango), **RET** (Retalhuleu) y **SMA** (San Andrés Semetabaj).

> **Importante para las pruebas:** la aplicación guarda automáticamente las modificaciones realizadas mediante el gestor persistente. Antes de registrar o eliminar datos durante una demostración, crea un respaldo de `transred.json`. La aplicación escribe sobre su archivo de datos activo; restaura el respaldo al finalizar las pruebas.

## Organización del repositorio

```text
Proyecto_2_EDII/
├── README.md
└── Red_Transporte/
    ├── Red_Transporte.slnx
    ├── Datos/
    │   └── transred.json
    ├── TransRed.Core/
    │   ├── Estructuras/            # TablaHash, Trie, Grafo y UnionFind
    │   ├── Modelos/                # Parada, Conexion y ResultadoRuta
    │   └── Servicios/              # Gestor y persistencia JSON
    ├── TransRed.Interfaz/
    │   ├── Components/             # Mapa, navegación y buscador Trie
    │   ├── Design/                 # Tema y estilos visuales
    │   ├── Layout/                 # Distribución de la ventana
    │   └── Views/                  # Explorador, paradas, conexiones, algoritmos
    └── TransRed.Consola/
        └── Program.cs
```

**Arquitectura:** tanto `TransRed.Interfaz` como `TransRed.Consola` utilizan `TransRed.Core`. El núcleo implementa las estructuras y operaciones; las aplicaciones de presentación consumen esos servicios. El gestor persistente se encarga de cargar y guardar los datos JSON.

## Requisitos

- **Windows 10 u 11** para ejecutar la interfaz gráfica de Windows Forms.
- **SDK de .NET 10** instalado.
- Una terminal (PowerShell, Terminal de Windows o la terminal de Visual Studio Code).
- Git para clonar el repositorio, o descargar el código fuente como ZIP desde GitHub.

Puedes comprobar la instalación del SDK con:

```powershell
dotnet --version
```

## Instalación y ejecución

**1. Clonar el repositorio:**

```powershell
git clone https://github.com/Marvin7715/Proyecto_2_EDII.git
cd Proyecto_2_EDII/Red_Transporte
```

**2. Restaurar dependencias y compilar la solución:**

```powershell
dotnet restore Red_Transporte.slnx
dotnet build Red_Transporte.slnx
```

**3. Ejecutar la aplicación gráfica (Windows):**

```powershell
dotnet run --project TransRed.Interfaz/TransRed.Interfaz.csproj
```

**4. Alternativa: ejecutar la versión de consola:**

```powershell
dotnet run --project TransRed.Consola/TransRed.Consola.csproj
```

> Los comandos de los pasos 2, 3 y 4 se ejecutan **dentro de `Red_Transporte`**. La interfaz de Windows Forms solo está destinada a Windows; la versión de consola es un proyecto separado.

## Guía rápida de uso

### Interfaz gráfica

1. **Explorar red:** elige origen y destino (puedes escribir para obtener sugerencias del Trie), selecciona **Más rápida** o **Económica** y pulsa **Explorar recorrido**. El resultado indica tiempo, costo, secuencia de paradas y camino sobre el mapa.
2. **Paradas:** consulta las existentes, busca una por código o registra una nueva proporcionando código, nombre, latitud y longitud. Puedes eliminar una parada seleccionada con confirmación.
3. **Conexiones:** selecciona dos paradas, indica minutos y costo y decide si la conexión será bidireccional. También puedes consultar y eliminar conexiones.
4. **Algoritmos:** elige **BFS** o **DFS**, selecciona una parada inicial y ejecuta el recorrido para observar las visitas sobre el grafo. En **Union-Find**, selecciona dos paradas y comprueba su conectividad.

### Versión de consola

La consola permite registrar, buscar y eliminar paradas; sugerir nombres; administrar rutas; consultar rutas rápidas o económicas; realizar recorridos BFS/DFS y comprobar conectividad. Selecciona la opción numérica correspondiente del menú.

## Pruebas y validación

La base de datos proporcionada supera los mínimos solicitados para el proyecto: **30 registros principales** y **40 conexiones o relaciones**. Las siguientes operaciones deben cubrirse en la batería de pruebas:

| Operación | Ejemplo de entrada |
| --- | --- |
| Búsqueda por código (Hash) | `XELA` |
| Búsqueda por prefijo (Trie) | `Parque Central` |
| Recorrido BFS | Inicio `SMA` |
| Recorrido DFS | Inicio `XELA` |
| Ruta más rápida (Dijkstra) | `XELA` → `OST` |
| Ruta más económica (Dijkstra) | `XELA` → `COL` |
| Conectividad (Union-Find) | `SMA` y `MOM` |

**Estado de evidencias:** la matriz formal de **cinco consultas de prueba por cada opción**, con entradas, resultados esperados, resultados obtenidos y capturas, se agregará como entregable separado. Los ejemplos anteriores son casos sugeridos, no un informe de ejecución automatizada.

## Documentación de entrega

Este README incluye el escenario, las estructuras utilizadas, su justificación básica y las instrucciones de ejecución. Los documentos de entrega restantes se incorporarán al repositorio:

- **Manual técnico:** arquitectura, modelos, diagramas, estructuras, algoritmos, complejidades y decisiones de diseño. *Pendiente de publicación.*
- **Manual de usuario:** instrucciones detalladas y capturas de las pantallas. *Pendiente de publicación.*
- **Casos y evidencias de prueba:** al menos cinco consultas por opción con sus resultados. *Pendiente de publicación.*

Una vez publicados, este apartado se actualizará con los enlaces directos a los archivos.

## Trabajo en equipo

Proyecto desarrollado de forma colaborativa. El historial de commits del repositorio permite consultar los aportes realizados sobre las estructuras, servicios, persistencia, interfaz y ajustes del sistema.

**Curso:** Estructura de Datos II  
**Docente:** Ing. Manuel Gustavo Rojas  
**Tipo de entrega:** Proyecto de laboratorio No. 2 — Entrega y presentación