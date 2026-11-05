
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

// Laboratorio interactivo de grafos y algoritmos
public class VistaAlgoritmos : UserControl
{
    private readonly IGestorTransporte gestor;

    private readonly ComboBox cmbInicio = new();
    private readonly ComboBox cmbOrigen = new();
    private readonly ComboBox cmbDestino = new();

    private readonly Button btnBFS = new();
    private readonly Button btnDFS = new();
    private readonly Button btnUnion = new();

    private readonly Button btnEjecutar = new();
    private readonly Button btnAnterior = new();
    private readonly Button btnSiguiente = new();
    private readonly Button btnReproducir = new();

    private readonly Label lblTitulo = new();
    private readonly Label lblDescripcion = new();
    private readonly Label lblEstado = new();
    private readonly Label lblUnion = new();

    private readonly FlowLayoutPanel listaPasos = new();
    private readonly Panel areaPrincipal = new();

    private readonly System.Windows.Forms.Timer temporizador = new();

    private readonly GrafoLaboratorio grafo;

    private Control vistaRecorrido = null!;
    private Control vistaUnion = null!;

    private Parada[] recorrido = Array.Empty<Parada>();

    private int pasoActual;
    private string algoritmo = "BFS";

    public VistaAlgoritmos(IGestorTransporte gestor)
    {
        this.gestor = gestor;

        Dock = DockStyle.Fill;
        Padding = new Padding(20, 13, 20, 16);
        BackColor = TransRedTheme.Fondo;

        grafo = new GrafoLaboratorio(gestor)
        {
            Dock = DockStyle.Fill
        };

        temporizador.Interval = 420;
        temporizador.Tick += (_, _) => Avanzar();

        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 108));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 60));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        principal.Controls.Add(CrearEncabezado(), 0, 0);
        principal.Controls.Add(CrearPestanas(), 0, 1);

        areaPrincipal.Dock = DockStyle.Fill;
        areaPrincipal.Margin = Padding.Empty;
        areaPrincipal.BackColor = TransRedTheme.Fondo;

        vistaRecorrido = CrearVistaRecorrido();
        vistaUnion = CrearVistaUnion();

        areaPrincipal.Controls.Add(vistaRecorrido);
        areaPrincipal.Controls.Add(vistaUnion);

        principal.Controls.Add(areaPrincipal, 0, 2);
        Controls.Add(principal);

        CargarParadas();
        SeleccionarAlgoritmo("BFS");
    }

    private Control CrearEncabezado()
    {
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 25));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 49));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Texto(
                "LABORATORIO / GRAFOS",
                8,
                TransRedTheme.Cobre,
                true),
            0, 0);

        tabla.Controls.Add(
            Texto(
                "Explora los algoritmos",
                21,
                TransRedTheme.Tinta,
                true),
            0, 1);

        tabla.Controls.Add(
            Texto(
                "Visualiza recorridos y conectividad",
                9.5f,
                TransRedTheme.TextoSecundario),
            0, 2);

        return tabla;
    }

    private Control CrearPestanas()
    {
        var barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = false,
            Margin = Padding.Empty,
            Padding = new Padding(0, 6, 0, 5),
            BackColor = Color.Transparent
        };

        ConfigurarPestana(btnBFS, "BFS", 105);
        ConfigurarPestana(btnDFS, "DFS", 105);
        ConfigurarPestana(btnUnion, "Union-Find", 140);

        btnBFS.Click += (_, _) =>
            SeleccionarAlgoritmo("BFS");

        btnDFS.Click += (_, _) =>
            SeleccionarAlgoritmo("DFS");

        btnUnion.Click += (_, _) =>
            SeleccionarAlgoritmo("Union-Find");

        barra.Controls.Add(btnBFS);
        barra.Controls.Add(btnDFS);
        barra.Controls.Add(btnUnion);

        return barra;
    }

    private static void ConfigurarPestana(
        Button boton, string titulo, int ancho)
    {
        boton.Text = titulo;
        boton.Width = ancho;
        boton.Height = 43;

        boton.Margin = new Padding(0, 0, 8, 0);

        boton.Font = new Font("Segoe UI", 9.5f);
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;

        boton.Cursor = Cursors.Hand;
        boton.UseVisualStyleBackColor = false;
    }

    private Control CrearVistaRecorrido()
    {
        var tarjeta = CrearTarjeta();

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 85));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 103));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var encabezado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        encabezado.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 51));

        encabezado.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        lblTitulo.Dock = DockStyle.Fill;
        lblTitulo.Margin = Padding.Empty;
        lblTitulo.Font = new Font(
            "Segoe UI", 18, FontStyle.Bold);
        lblTitulo.ForeColor = TransRedTheme.Tinta;
        lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
        lblTitulo.AutoEllipsis = false;

        lblDescripcion.Dock = DockStyle.Fill;
        lblDescripcion.Margin = Padding.Empty;
        lblDescripcion.Font = new Font("Segoe UI", 9.5f);
        lblDescripcion.ForeColor =
            TransRedTheme.TextoSecundario;
        lblDescripcion.TextAlign =
            ContentAlignment.MiddleLeft;

        encabezado.Controls.Add(lblTitulo, 0, 0);
        encabezado.Controls.Add(lblDescripcion, 0, 1);

        tabla.Controls.Add(encabezado, 0, 0);

        var controles = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true,
            Margin = Padding.Empty,
            Padding = new Padding(0, 4, 0, 4),
            BackColor = Color.Transparent
        };

        controles.Controls.Add(
            CrearCampo("PARADA INICIAL", cmbInicio, 240));

        PrepararBoton(
            btnEjecutar, "Ejecutar", 115, true);

        PrepararBoton(
            btnAnterior, "Anterior", 90);

        PrepararBoton(
            btnSiguiente, "Siguiente", 96);

        PrepararBoton(
            btnReproducir, "Reproducir", 112);

        btnEjecutar.Click += (_, _) =>
            EjecutarRecorrido();

        btnAnterior.Click += (_, _) =>
            CambiarPaso(-1);

        btnSiguiente.Click += (_, _) =>
            CambiarPaso(1);

        btnReproducir.Click += (_, _) =>
            AlternarReproduccion();

        controles.Controls.Add(btnEjecutar);
        controles.Controls.Add(btnAnterior);
        controles.Controls.Add(btnSiguiente);
        controles.Controls.Add(btnReproducir);

        tabla.Controls.Add(controles, 0, 1);

        var cuerpo = CrearDistribucion();

        var espacioGrafo = CrearEspacioGrafo();
        espacioGrafo.Controls.Add(grafo);

        cuerpo.Controls.Add(espacioGrafo, 0, 0);

        var lateral = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 32));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 37));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        lateral.Controls.Add(
            Texto(
                "SECUENCIA DE VISITA",
                8,
                TransRedTheme.TextoTenue,
                true),
            0, 0);

        lblEstado.Dock = DockStyle.Fill;
        lblEstado.Margin = Padding.Empty;
        lblEstado.ForeColor = TransRedTheme.Acento;
        lblEstado.Font = new Font(
            "Segoe UI", 9.5f, FontStyle.Bold);
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;
        lblEstado.AutoEllipsis = true;

        lateral.Controls.Add(lblEstado, 0, 1);

        listaPasos.Dock = DockStyle.Fill;
        listaPasos.Margin = Padding.Empty;
        listaPasos.Padding = new Padding(5);
        listaPasos.FlowDirection = FlowDirection.TopDown;
        listaPasos.WrapContents = false;
        listaPasos.AutoScroll = true;
        listaPasos.BackColor =
            TransRedTheme.SuperficieAlterna;

        listaPasos.Resize += (_, _) => AjustarLista();

        lateral.Controls.Add(listaPasos, 0, 2);
        cuerpo.Controls.Add(lateral, 1, 0);

        tabla.Controls.Add(cuerpo, 0, 2);
        tarjeta.Controls.Add(tabla);

        return tarjeta;
    }

    private Control CrearVistaUnion()
    {
        var tarjeta = CrearTarjeta();

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 76));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 104));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Texto(
                "Conectividad entre paradas",
                18,
                TransRedTheme.Tinta,
                true),
            0, 0);

        var controles = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true,
            Margin = Padding.Empty,
            Padding = new Padding(0, 5, 0, 4),
            BackColor = Color.Transparent
        };

        controles.Controls.Add(
            CrearCampo("ORIGEN", cmbOrigen, 245));

        controles.Controls.Add(
            CrearCampo("DESTINO", cmbDestino, 245));

        var comprobar = new Button();

        PrepararBoton(
            comprobar, "Comprobar", 125, true);

        comprobar.Click += (_, _) =>
            ComprobarConexion();

        controles.Controls.Add(comprobar);

        tabla.Controls.Add(controles, 0, 1);

        var cuerpo = CrearDistribucion();

        // El grafo se trasladará aquí al entrar
        // a la pestaña Union-Find.
        cuerpo.Controls.Add(CrearEspacioGrafo(), 0, 0);

        var resultado = new GradientSurface(
            Color.FromArgb(234, 243, 232),
            Color.FromArgb(248, 245, 237),
            14)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(16)
        };

        lblUnion.Dock = DockStyle.Fill;
        lblUnion.Margin = Padding.Empty;
        lblUnion.Font = new Font(
            "Segoe UI", 12, FontStyle.Bold);
        lblUnion.ForeColor = TransRedTheme.Bosque;
        lblUnion.TextAlign = ContentAlignment.MiddleCenter;

        resultado.Controls.Add(lblUnion);
        cuerpo.Controls.Add(resultado, 1, 0);

        tabla.Controls.Add(cuerpo, 0, 2);
        tarjeta.Controls.Add(tabla);

        return tarjeta;
    }

    private static TableLayoutPanel CrearDistribucion()
    {
        var cuerpo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        cuerpo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 65));

        cuerpo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 35));

        cuerpo.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        return cuerpo;
    }

    private static Panel CrearEspacioGrafo()
    {
        return new Panel
        {
            Name = "EspacioGrafo",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 12, 0),
            BackColor = TransRedTheme.FondoMapa
        };
    }

    private static GradientSurface CrearTarjeta()
    {
        return new GradientSurface(
            Color.White,
            Color.FromArgb(251, 250, 246),
            18)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(20, 16, 20, 18)
        };
    }

    private static Control CrearCampo(
        string titulo,
        ComboBox combo,
        int ancho)
    {
        var tabla = new TableLayoutPanel
        {
            Width = ancho,
            Height = 78,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 8, 0),
            BackColor = Color.Transparent
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 30));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Texto(
                titulo,
                8,
                TransRedTheme.TextoSecundario,
                true),
            0, 0);

        UiStyles.AplicarCombo(combo);

        combo.Dock = DockStyle.Top;
        combo.Margin = Padding.Empty;
        combo.DropDownWidth = 360;
        combo.DropDownHeight = 290;

        tabla.Controls.Add(combo, 0, 1);

        return tabla;
    }

    private static void PrepararBoton(
        Button boton,
        string texto,
        int ancho,
        bool principal = false)
    {
        boton.Text = texto;
        boton.Width = ancho;
        boton.Height = 40;
        boton.Margin = new Padding(3, 33, 3, 0);

        if (principal)
            UiStyles.AplicarBotonPrincipal(boton);
        else
            UiStyles.AplicarBotonSecundario(boton);

        boton.Padding = Padding.Empty;
        boton.MinimumSize = Size.Empty;
    }

    private void CargarParadas()
    {
        Parada[] paradas = gestor.ObtenerTodasParadas();

        foreach (ComboBox combo in new[]
        {
            cmbInicio,
            cmbOrigen,
            cmbDestino
        })
        {
            combo.Items.Clear();

            foreach (Parada parada in paradas)
                combo.Items.Add(parada);

            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }

        if (cmbDestino.Items.Count > 1)
            cmbDestino.SelectedIndex = 1;
    }

    private void SeleccionarAlgoritmo(string nuevo)
    {
        temporizador.Stop();
        algoritmo = nuevo;

        EstilizarPestana(
            btnBFS, nuevo == "BFS");

        EstilizarPestana(
            btnDFS, nuevo == "DFS");

        EstilizarPestana(
            btnUnion, nuevo == "Union-Find");

        bool esUnion = nuevo == "Union-Find";

        vistaRecorrido.Visible = !esUnion;
        vistaUnion.Visible = esUnion;

        if (esUnion)
            vistaUnion.BringToFront();
        else
            vistaRecorrido.BringToFront();

        Panel? destino = BuscarEspacioGrafo(
            esUnion ? vistaUnion : vistaRecorrido);

        if (destino != null)
        {
            grafo.Parent = destino;
            grafo.Dock = DockStyle.Fill;
            grafo.BringToFront();
        }

        recorrido = Array.Empty<Parada>();
        pasoActual = 0;

        LimpiarLista();
        grafo.Limpiar();

        lblTitulo.Text = nuevo == "BFS"
            ? "Búsqueda en anchura"
            : "Búsqueda en profundidad";

        lblDescripcion.Text = nuevo == "BFS"
            ? "Explora la red por niveles"
            : "Explora cada rama antes de retroceder";

        lblEstado.Text =
            "Selecciona una parada para comenzar";

        lblUnion.Text = "Selecciona dos paradas";

        btnReproducir.Text = "Reproducir";
        ActualizarBotones();
    }

    private static void EstilizarPestana(
        Button boton,
        bool seleccionada)
    {
        boton.BackColor = seleccionada
            ? TransRedTheme.Bosque
            : TransRedTheme.AcentoSuave;

        boton.ForeColor = seleccionada
            ? Color.White
            : TransRedTheme.Tinta;
    }

    private static Panel? BuscarEspacioGrafo(Control raiz)
    {
        foreach (Control control in raiz.Controls)
        {
            if (control is Panel panel &&
                panel.Name == "EspacioGrafo")
                return panel;

            Panel? resultado =
                BuscarEspacioGrafo(control);

            if (resultado != null)
                return resultado;
        }

        return null;
    }

    private void EjecutarRecorrido()
    {
        if (cmbInicio.SelectedItem is not Parada inicio)
            return;

        temporizador.Stop();

        // Algoritmos originales del Core
        recorrido = algoritmo == "BFS"
            ? gestor.RecorrerBFS(inicio.Codigo)
            : gestor.RecorrerDFS(inicio.Codigo);

        pasoActual = 0;

        LimpiarLista();
        grafo.Limpiar();

        lblEstado.Text = recorrido.Length == 0
            ? "No hay paradas para recorrer"
            : $"{recorrido.Length} paradas preparadas";

        btnReproducir.Text = "Pausar";

        ActualizarBotones();

        if (recorrido.Length > 0)
            temporizador.Start();
    }

    private void AlternarReproduccion()
    {
        if (recorrido.Length == 0)
            return;

        if (temporizador.Enabled)
        {
            temporizador.Stop();
            btnReproducir.Text = "Continuar";
        }
        else
        {
            if (pasoActual >= recorrido.Length)
            {
                pasoActual = 0;
                LimpiarLista();
                grafo.Limpiar();
            }

            btnReproducir.Text = "Pausar";
            temporizador.Start();
        }

        ActualizarBotones();
    }

    private void CambiarPaso(int diferencia)
    {
        if (recorrido.Length == 0)
            return;

        temporizador.Stop();

        pasoActual = Math.Clamp(
            pasoActual + diferencia,
            0,
            recorrido.Length);

        ActualizarVistaRecorrido();

        btnReproducir.Text =
            pasoActual == recorrido.Length
                ? "Reiniciar"
                : "Continuar";

        ActualizarBotones();
    }

    private void Avanzar()
    {
        if (pasoActual < recorrido.Length)
            pasoActual++;

        ActualizarVistaRecorrido();

        if (pasoActual >= recorrido.Length)
        {
            temporizador.Stop();
            btnReproducir.Text = "Reiniciar";
        }

        ActualizarBotones();
    }

    private void ActualizarVistaRecorrido()
    {
        grafo.MostrarVisitados(
            recorrido,
            pasoActual);

        lblEstado.Text =
            $"Paso {pasoActual} de {recorrido.Length}";

        LimpiarLista();

        listaPasos.SuspendLayout();

        for (int i = 0; i < pasoActual; i++)
        {
            Parada parada = recorrido[i];

            var fila = new Panel
            {
                Width = Math.Max(
                    120,
                    listaPasos.ClientSize.Width -
                    SystemInformation.VerticalScrollBarWidth - 14),
                Height = 60,
                Margin = new Padding(0, 0, 0, 5),
                Padding = new Padding(10, 5, 10, 5),
                BackColor = i == pasoActual - 1
                    ? TransRedTheme.AcentoSuave
                    : Color.White
            };

            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            tabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 26));

            tabla.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            tabla.Controls.Add(
                Texto(
                    $"{i + 1:00} · {parada.Codigo}",
                    9,
                    TransRedTheme.Acento,
                    true),
                0, 0);

            tabla.Controls.Add(
                Texto(
                    parada.Nombre,
                    8.5f,
                    TransRedTheme.TextoSecundario),
                0, 1);

            fila.Controls.Add(tabla);
            listaPasos.Controls.Add(fila);
        }

        listaPasos.ResumeLayout(true);
        AjustarLista();

        if (pasoActual > 0)
        {
            listaPasos.ScrollControlIntoView(
                listaPasos.Controls[pasoActual - 1]);
        }
    }

    private void AjustarLista()
    {
        int ancho = Math.Max(
            100,
            listaPasos.ClientSize.Width -
            SystemInformation.VerticalScrollBarWidth - 14);

        foreach (Control fila in listaPasos.Controls)
            fila.Width = ancho;
    }

    private void LimpiarLista()
    {
        while (listaPasos.Controls.Count > 0)
        {
            Control control = listaPasos.Controls[0];
            listaPasos.Controls.RemoveAt(0);
            control.Dispose();
        }
    }

    private void ActualizarBotones()
    {
        btnAnterior.Enabled =
            recorrido.Length > 0 &&
            pasoActual > 0;

        btnSiguiente.Enabled =
            recorrido.Length > 0 &&
            pasoActual < recorrido.Length;

        btnReproducir.Enabled =
            recorrido.Length > 0;
    }

    private void ComprobarConexion()
    {
        if (cmbOrigen.SelectedItem is not Parada origen ||
            cmbDestino.SelectedItem is not Parada destino)
            return;

        bool conectadas = gestor.EstanConectadas(
            origen.Codigo,
            destino.Codigo);

        lblUnion.Text = conectadas
            ? "PARADAS CONECTADAS\n\nPertenecen a una misma componente"
            : "SIN CONEXIÓN\n\nPertenecen a componentes distintas";

        lblUnion.ForeColor = conectadas
            ? TransRedTheme.Acento
            : TransRedTheme.Error;

        grafo.MostrarComparacion(
            origen.Codigo,
            destino.Codigo);
    }

    private static Label Texto(
        string contenido,
        float tamano,
        Color color,
        bool negrita = false)
    {
        return new Label
        {
            Text = contenido,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = Color.Transparent,
            ForeColor = color,
            Font = new Font(
                "Segoe UI",
                tamano,
                negrita
                    ? FontStyle.Bold
                    : FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            temporizador.Dispose();

        base.Dispose(disposing);
    }
}

// Visualiza las paradas y conexiones reales del Core
internal sealed class GrafoLaboratorio : Control
{
    private readonly Parada[] paradas;
    private readonly Conexion[] conexiones;

    private readonly HashSet<string> visitados =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly double latMin;
    private readonly double latMax;
    private readonly double lonMin;
    private readonly double lonMax;

    private string? actual;
    private string? comparadoA;
    private string? comparadoB;

    public GrafoLaboratorio(IGestorTransporte gestor)
    {
        paradas = gestor.ObtenerTodasParadas();
        conexiones = gestor.ObtenerTodasConexiones();

        latMin = paradas.Length > 0
            ? paradas.Min(p => p.Latitud) : 0;

        latMax = paradas.Length > 0
            ? paradas.Max(p => p.Latitud) : 1;

        lonMin = paradas.Length > 0
            ? paradas.Min(p => p.Longitud) : 0;

        lonMax = paradas.Length > 0
            ? paradas.Max(p => p.Longitud) : 1;

        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = TransRedTheme.FondoMapa;
    }

    public void Limpiar()
    {
        visitados.Clear();

        actual = null;
        comparadoA = null;
        comparadoB = null;

        Invalidate();
    }

    public void MostrarVisitados(
        Parada[] orden,
        int cantidad)
    {
        visitados.Clear();

        for (int i = 0;
             i < Math.Min(cantidad, orden.Length);
             i++)
        {
            visitados.Add(orden[i].Codigo);
        }

        actual = cantidad > 0 &&
                 cantidad <= orden.Length
            ? orden[cantidad - 1].Codigo
            : null;

        comparadoA = null;
        comparadoB = null;

        Invalidate();
    }

    public void MostrarComparacion(
        string origen,
        string destino)
    {
        visitados.Clear();

        actual = null;
        comparadoA = origen;
        comparadoB = destino;

        Invalidate();
    }

    private PointF ObtenerPosicion(Parada parada)
    {
        float x = lonMax == lonMin
            ? 0.5f
            : (float)(
                (parada.Longitud - lonMin) /
                (lonMax - lonMin));

        float y = latMax == latMin
            ? 0.5f
            : 1f - (float)(
                (parada.Latitud - latMin) /
                (latMax - latMin));

        return new PointF(
            26 + x * Math.Max(1, Width - 52),
            52 + y * Math.Max(1, Height - 104));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);

        using var fuenteTitulo = new Font(
            "Segoe UI", 8.5f, FontStyle.Bold);

        TextRenderer.DrawText(
            g,
            $"RED REAL · {paradas.Length} PARADAS",
            fuenteTitulo,
            new Rectangle(
                12, 12,
                Math.Max(40, Width - 24), 25),
            TransRedTheme.Bosque);

        var indice = new Dictionary<string, Parada>(
            StringComparer.OrdinalIgnoreCase);

        foreach (Parada parada in paradas)
            indice[parada.Codigo] = parada;

        using var linea = new Pen(
            Color.FromArgb(115, 156, 186, 160),
            1.3f);

        // Solo dibuja conexiones existentes
        foreach (Conexion conexion in conexiones)
        {
            if (!indice.TryGetValue(
                    conexion.CodigoOrigen,
                    out Parada? origen))
                continue;

            if (!indice.TryGetValue(
                    conexion.CodigoDestino,
                    out Parada? destino))
                continue;

            g.DrawLine(
                linea,
                ObtenerPosicion(origen),
                ObtenerPosicion(destino));
        }

        foreach (Parada parada in paradas)
        {
            PointF punto = ObtenerPosicion(parada);

            bool visitada =
                visitados.Contains(parada.Codigo);

            bool foco = string.Equals(
                parada.Codigo,
                actual,
                StringComparison.OrdinalIgnoreCase);

            bool comparacion = string.Equals(
                parada.Codigo,
                comparadoA,
                StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    parada.Codigo,
                    comparadoB,
                    StringComparison.OrdinalIgnoreCase);

            float radio = foco || comparacion
                ? 10f
                : visitada ? 7.5f : 5f;

            Color color = foco
                ? TransRedTheme.Cobre
                : visitada || comparacion
                    ? TransRedTheme.Acento
                    : Color.White;

            using var relleno = new SolidBrush(color);

            using var borde = new Pen(
                TransRedTheme.Acento, 1.7f);

            g.FillEllipse(
                relleno,
                punto.X - radio,
                punto.Y - radio,
                radio * 2,
                radio * 2);

            g.DrawEllipse(
                borde,
                punto.X - radio,
                punto.Y - radio,
                radio * 2,
                radio * 2);

            if (foco || comparacion)
            {
                using var fuente = new Font(
                    "Segoe UI", 9, FontStyle.Bold);

                TextRenderer.DrawText(
                    g,
                    parada.Codigo,
                    fuente,
                    new Rectangle(
                        (int)punto.X - 60,
                        (int)punto.Y - 32,
                        120, 22),
                    TransRedTheme.Bosque,
                    TextFormatFlags.HorizontalCenter);
            }
        }

        using var fuenteAyuda = new Font(
            "Segoe UI", 8.5f);

        string ayuda = comparadoA != null
            ? "Paradas consultadas destacadas"
            : "Verde: visitadas  ·  Cobre: visita actual";

        TextRenderer.DrawText(
            g,
            ayuda,
            fuenteAyuda,
            new Rectangle(
                12,
                Math.Max(0, Height - 32),
                Math.Max(50, Width - 24),
                25),
            TransRedTheme.TextoSecundario);
    }
}
