# TransRed — Red de transporte del suroccidente de Guatemala

**Proyecto de Laboratorio No. 2 · Estructura de Datos II**  
**Docente:** Ing. Manuel Gustavo Rojas · **Lenguaje:** C#

## Descripción

**TransRed** es un sistema académico que representa una red de transporte del suroccidente de Guatemala. Permite registrar paradas y conexiones, buscar lugares y encontrar recorridos según el **menor tiempo** o el **menor costo**.

El objetivo del proyecto es aplicar estructuras de datos en un caso práctico. Incluye una interfaz gráfica en Windows Forms, una versión de consola y almacenamiento en JSON. Los tiempos y costos son datos de prueba; el mapa es una representación visual de las conexiones, no un mapa exacto de carreteras.

## ¿Qué permite hacer?

- **Explorar red:** elegir origen y destino, recibir sugerencias al escribir y ver la ruta rápida o económica en un mapa interactivo.
- **Paradas:** registrar, buscar por código, consultar y eliminar paradas.
- **Conexiones:** agregar y eliminar rutas entre paradas, indicando minutos, costo y si funcionan en ambos sentidos.
- **Algoritmos:** visualizar recorridos **BFS** y **DFS** y comprobar conectividad con **Union-Find**.
- **Guardar información:** conservar los cambios en el archivo JSON del proyecto.

## Estructuras de datos utilizadas

| Estructura | Uso en TransRed | ¿Por qué se eligió? |
| --- | --- | --- |
| **Tabla Hash** | Buscar paradas por código. | Facilita encontrar un registro sin revisar toda la lista. |
| **Trie** | Sugerir nombres al escribir un prefijo. | Permite buscar coincidencias a partir de las primeras letras. |
| **Grafo** | Representar paradas y conexiones con tiempo y costo. | Permite recorrer la red y calcular rutas. |
| **Union-Find** | Comprobar si dos paradas pertenecen a una misma red conectada. | Agrupa paradas y facilita consultar su conectividad. |

En el grafo se utilizan **BFS** (recorrido por niveles), **DFS** (recorrido en profundidad) y **Dijkstra** (ruta de menor tiempo o menor costo). La conectividad con Union-Find considera las conexiones sin distinguir su dirección; por eso no equivale a verificar una ruta dirigida de ida.

## Organización del proyecto

```text
Red_Transporte/
├── Datos/transred.json       # Datos de prueba
├── TransRed.Core/            # Estructuras, algoritmos y persistencia
├── TransRed.Interfaz/        # Aplicación gráfica (Windows Forms)
├── TransRed.Consola/         # Menú de consola
└── Red_Transporte.slnx       # Solución .NET
```

## Requisitos y ejecución

Se necesita el **SDK de .NET 10**. Para usar la interfaz gráfica se requiere **Windows**.

1. Clonar el repositorio y entrar a la solución:

   ```powershell
   git clone https://github.com/Marvin7715/Proyecto_2_EDII.git
   cd Proyecto_2_EDII/Red_Transporte
   ```

2. Compilar:

   ```powershell
   dotnet build Red_Transporte.slnx
   ```

3. Ejecutar la aplicación gráfica:

   ```powershell
   dotnet run --project TransRed.Interfaz
   ```

   También puede ejecutarse la versión de consola:

   ```powershell
   dotnet run --project TransRed.Consola
   ```

> **Nota:** los comandos de compilación y ejecución se realizan dentro de `Red_Transporte`.

## Datos de prueba

El archivo [`Red_Transporte/Datos/transred.json`](Red_Transporte/Datos/transred.json) contiene:

- **35 paradas**, cada una con código, nombre, latitud y longitud.
- **102 conexiones dirigidas**, que forman **51 pares bidireccionales**, con origen, destino, tiempo (minutos) y costo (Q).

El JSON utiliza las listas `Paradas` y `Conexiones`. Los campos de una parada son `Codigo`, `Nombre`, `Latitud` y `Longitud`; los de una conexión son `Origen`, `Destino`, `Minutos` y `Costo`.

Ejemplos de consultas posibles: buscar `XELA` por código, escribir `Parque Central` para obtener sugerencias, ejecutar BFS desde `SMA` o consultar una ruta entre `XELA` y `HUE`.

**Importante:** al registrar o eliminar información, la aplicación guarda los cambios en el JSON. Se recomienda hacer una copia del archivo antes de realizar pruebas que modifiquen los datos.

## Documentación

- [Manual técnico](Docs/Manual_Tecnico_TransRed.pdf.pdf)
- [Manual de usuario](Docs/PROYECTO%202%20%28Manual%20de%20Usuario%29.pdf)

**Pruebas de entrega:** se documentarán por separado al menos **cinco consultas por opción** (búsquedas, prefijos, recorridos, rutas óptimas y conectividad), con sus resultados y evidencias.

---

Proyecto desarrollado en equipo. Los aportes pueden consultarse en el [historial de commits](https://github.com/Marvin7715/Proyecto_2_EDII/commits/main).