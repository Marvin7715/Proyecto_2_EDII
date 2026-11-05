
using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

// Laboratorio visual de BFS, DFS y Union-Find
public class VistaAlgoritmos : UserControl
{
    private readonly IGestorTransporte gestor;

    private readonly ComboBox cmbInicio = new();
    private readonly ComboBox cmbOrigen = new();
    private readonly ComboBox cmbDestino = new();

    private readonly Label lblTitulo = new();
    private readonly Label lblDescripcion = new();
    private readonly Label lblEstado = new();
    private readonly Label lblConectividad = new();

    private readonly FlowLayoutPanel pasos = new();
    private readonly Panel areaVistas = new();

    private readonly Button btnBFS = new();
    private readonly Button btnDFS = new();
    private readonly Button btnUnion = new();
    private readonly Button btnReproducir = new();

    private readonly System.Windows.Forms.Timer animacion = new();

    private Panel vistaRecorrido = null!;
    private Panel vistaUnion = null!;

    private Parada[] recorrido = Array.Empty<Parada>();
    private int pasoActual;
    private string algoritmo = "BFS";

    public VistaAlgoritmos(IGestorTransporte gestor)
    {
        this.gestor = gestor;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.Fondo;
        Padding = new Padding(24);

        animacion.Interval = 380;
        animacion.Tick += (_, _) => AvanzarPaso();

        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 104));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 66));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        principal.Controls.Add(CrearEncabezado(), 0, 0);
        principal.Controls.Add(CrearPestanas(), 0, 1);

        areaVistas.Dock = DockStyle.Fill;
        areaVistas.BackColor = TransRedTheme.Fondo;

        vistaRecorrido = CrearVistaRecorrido();
        vistaUnion = CrearVistaUnion();

        areaVistas.Controls.Add(vistaRecorrido);
        areaVistas.Controls.Add(vistaUnion);

        principal.Controls.Add(areaVistas, 0, 2);
        Controls.Add(principal);

        CargarParadas();
        MostrarAlgoritmo("BFS");
    }

    private Control CrearEncabezado()
    {
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = TransRedTheme.Fondo
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 24));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 44));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Etiqueta("LABORATORIO DE RED", 8,
                TransRedTheme.Cobre, true), 0, 0);

        tabla.Controls.Add(
            Etiqueta("Explora los algoritmos", 22,
                TransRedTheme.Tinta, true), 0, 1);

        tabla.Controls.Add(
            Etiqueta(
                "Recorridos y conectividad sobre tu red de transporte",
                9.5f,
                TransRedTheme.TextoSecundario), 0, 2);

        return tabla;
    }

    private Control CrearPestanas()
    {
        var barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            BackColor = TransRedTheme.Fondo,
            Padding = new Padding(0, 5, 0, 8)
        };

        ConfigurarPestana(btnBFS, "BFS", 108);
        ConfigurarPestana(btnDFS, "DFS", 108);
        ConfigurarPestana(btnUnion, "Union-Find", 145);

        btnBFS.Click += (_, _) => MostrarAlgoritmo("BFS");
        btnDFS.Click += (_, _) => MostrarAlgoritmo("DFS");
        btnUnion.Click += (_, _) => MostrarAlgoritmo("Union-Find");

        barra.Controls.Add(btnBFS);
        barra.Controls.Add(btnDFS);
        barra.Controls.Add(btnUnion);

        return barra;
    }

    private void ConfigurarPestana(
        Button boton, string texto, int ancho)
    {
        boton.Text = texto;
        boton.Width = ancho;
        boton.Height = 43;
        boton.Margin = new Padding(0, 0, 9, 0);
        boton.Cursor = Cursors.Hand;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.Font = new Font("Segoe UI", 10);
        boton.UseVisualStyleBackColor = false;
    }

    private Panel CrearVistaRecorrido()
    {
        var tarjeta = CrearTarjeta();

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 86));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 94));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 46));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var encabezado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent
        };

        encabezado.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 44));
        encabezado.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        lblTitulo.Dock = DockStyle.Fill;
        lblTitulo.Font = new Font(
            "Segoe UI", 18, FontStyle.Bold);
        lblTitulo.ForeColor = TransRedTheme.Tinta;

        lblDescripcion.Dock = DockStyle.Fill;
        lblDescripcion.Font = new Font("Segoe UI", 9.5f);
        lblDescripcion.ForeColor =
            TransRedTheme.TextoSecundario;

        encabezado.Controls.Add(lblTitulo, 0, 0);
        encabezado.Controls.Add(lblDescripcion, 0, 1);

        var controles = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            Padding = new Padding(0, 8, 0, 5),
            BackColor = Color.Transparent
        };

        controles.Controls.Add(
            CrearCampo("PARADA INICIAL", cmbInicio, 300));

        var ejecutar = new Button
        {
            Text = "Ejecutar recorrido",
            Width = 172,
            Height = 40,
            Margin = new Padding(12, 30, 8, 0)
        };

        UiStyles.AplicarBotonPrincipal(ejecutar);
        ejecutar.Click += (_, _) => EjecutarRecorrido();

        btnReproducir.Text = "Reproducir";
        btnReproducir.Width = 130;
        btnReproducir.Height = 40;
        btnReproducir.Margin = new Padding(4, 30, 0, 0);

        UiStyles.AplicarBotonSecundario(btnReproducir);
        btnReproducir.Click += (_, _) => AlternarReproduccion();

        controles.Controls.Add(ejecutar);
        controles.Controls.Add(btnReproducir);

        lblEstado.Dock = DockStyle.Fill;
        lblEstado.Font = new Font(
            "Segoe UI", 10, FontStyle.Bold);
        lblEstado.ForeColor = TransRedTheme.Acento;
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;

        pasos.Dock = DockStyle.Fill;
        pasos.AutoScroll = true;
        pasos.WrapContents = true;
        pasos.Padding = new Padding(12);
        pasos.BackColor = TransRedTheme.SuperficieAlterna;

        var resultado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent
        };

        resultado.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 30));
        resultado.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        resultado.Controls.Add(
            Etiqueta("SECUENCIA DE VISITA", 8,
                TransRedTheme.TextoTenue, true), 0, 0);

        resultado.Controls.Add(pasos, 0, 1);

        tabla.Controls.Add(encabezado, 0, 0);
        tabla.Controls.Add(controles, 0, 1);
        tabla.Controls.Add(lblEstado, 0, 2);
        tabla.Controls.Add(resultado, 0, 3);

        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private Panel CrearVistaUnion()
    {
        var tarjeta = CrearTarjeta();

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.Transparent
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 65));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 105));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 64));
        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Etiqueta("¿ESTÁN CONECTADAS ESTAS PARADAS?",
                16, TransRedTheme.Tinta, true), 0, 0);

        var controles = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            Padding = new Padding(0, 6, 0, 0),
            BackColor = Color.Transparent
        };

        controles.Controls.Add(
            CrearCampo("ORIGEN", cmbOrigen, 240));

        controles.Controls.Add(
            CrearCampo("DESTINO", cmbDestino, 240));

        var comprobar = new Button
        {
            Text = "Comprobar",
            Width = 145,
            Height = 40,
            Margin = new Padding(12, 30, 0, 0)
        };

        UiStyles.AplicarBotonPrincipal(comprobar);
        comprobar.Click += (_, _) => ComprobarConexion();

        controles.Controls.Add(comprobar);

        lblConectividad.Text = "Selecciona dos paradas";
        lblConectividad.Dock = DockStyle.Fill;
        lblConectividad.Font = new Font(
            "Segoe UI", 13, FontStyle.Bold);
        lblConectividad.ForeColor = TransRedTheme.Tinta;
        lblConectividad.TextAlign =
            ContentAlignment.MiddleLeft;

        tabla.Controls.Add(controles, 0, 1);
        tabla.Controls.Add(lblConectividad, 0, 2);

        tabla.Controls.Add(
            Etiqueta(
                "La comprobación utiliza Union-Find del proyecto. " +
                "El resultado indica si las paradas pertenecen " +
                "a una misma componente conectada.",
                10,
                TransRedTheme.TextoSecundario), 0, 3);

        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private static Panel CrearTarjeta()
    {
        return new GradientSurface(
            Color.White,
            Color.FromArgb(249, 249, 244),
            18)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(26)
        };
    }

    private static Control CrearCampo(
        string titulo, ComboBox combo, int ancho)
    {
        var panel = new TableLayoutPanel
        {
            Width = ancho,
            Height = 76,
            RowCount = 2,
            ColumnCount = 1,
            Margin = Padding.Empty
        };

        panel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 27));
        panel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(
            Etiqueta(titulo, 8,
                TransRedTheme.TextoSecundario, true), 0, 0);

        UiStyles.AplicarCombo(combo);

        combo.Dock = DockStyle.Top;
        combo.DropDownWidth = 340;
        combo.Margin = Padding.Empty;

        panel.Controls.Add(combo, 0, 1);
        return panel;
    }

    private static Label Etiqueta(
        string texto,
        float tamano,
        Color color,
        bool negrita = false)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            ForeColor = color,
            BackColor = Color.Transparent,
            Font = new Font(
                "Segoe UI", tamano,
                negrita ? FontStyle.Bold : FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty,
            AutoEllipsis = true
        };
    }

    private void CargarParadas()
    {
        Parada[] paradas = gestor.ObtenerTodasParadas();

        foreach (ComboBox combo in new[]
        {
            cmbInicio, cmbOrigen, cmbDestino
        })
        {
            combo.Items.Clear();

            foreach (Parada parada in paradas)
                combo.Items.Add(parada);

            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }
    }

    private void MostrarAlgoritmo(string seleccionado)
    {
        animacion.Stop();
        algoritmo = seleccionado;

        btnBFS.BackColor = seleccionado == "BFS"
            ? TransRedTheme.Bosque : TransRedTheme.AcentoSuave;
        btnDFS.BackColor = seleccionado == "DFS"
            ? TransRedTheme.Bosque : TransRedTheme.AcentoSuave;
        btnUnion.BackColor = seleccionado == "Union-Find"
            ? TransRedTheme.Bosque : TransRedTheme.AcentoSuave;

        foreach (Button boton in new[]
        {
            btnBFS, btnDFS, btnUnion
        })
        {
            boton.ForeColor = boton.BackColor ==
                TransRedTheme.Bosque
                ? Color.White
                : TransRedTheme.Tinta;
        }

        bool esUnion = seleccionado == "Union-Find";

        vistaRecorrido.Visible = !esUnion;
        vistaUnion.Visible = esUnion;

        if (esUnion)
            vistaUnion.BringToFront();
        else
            vistaRecorrido.BringToFront();

        if (!esUnion)
        {
            lblTitulo.Text = seleccionado == "BFS"
                ? "Búsqueda en anchura"
                : "Búsqueda en profundidad";

            lblDescripcion.Text = seleccionado == "BFS"
                ? "Descubre el orden de visita por niveles"
                : "Explora cada rama antes de retroceder";

            recorrido = Array.Empty<Parada>();
            pasoActual = 0;
            pasos.Controls.Clear();
            lblEstado.Text = "Selecciona una parada para comenzar";
            btnReproducir.Text = "Reproducir";
        }
    }

    private void EjecutarRecorrido()
    {
        if (cmbInicio.SelectedItem is not Parada inicio)
            return;

        animacion.Stop();
        btnReproducir.Text = "Reproducir";

        // El Core determina el orden real
        recorrido = algoritmo == "BFS"
            ? gestor.RecorrerBFS(inicio.Codigo)
            : gestor.RecorrerDFS(inicio.Codigo);

        pasoActual = 0;
        pasos.Controls.Clear();

        lblEstado.Text = recorrido.Length == 0
            ? "No hay paradas para recorrer"
            : $"{recorrido.Length} paradas en el recorrido";

        if (recorrido.Length > 0)
            animacion.Start();
    }

    private void AlternarReproduccion()
    {
        if (recorrido.Length == 0)
            return;

        if (animacion.Enabled)
        {
            animacion.Stop();
            btnReproducir.Text = "Continuar";
            return;
        }

        if (pasoActual >= recorrido.Length)
        {
            pasoActual = 0;
            pasos.Controls.Clear();
        }

        btnReproducir.Text = "Pausar";
        animacion.Start();
    }

    private void AvanzarPaso()
    {
        if (pasoActual >= recorrido.Length)
        {
            animacion.Stop();
            btnReproducir.Text = "Reiniciar";
            return;
        }

        Parada parada = recorrido[pasoActual];
        int numero = pasoActual + 1;

        var tarjeta = new Panel
        {
            Width = 184,
            Height = 75,
            BackColor = Color.White,
            Margin = new Padding(5),
            Padding = new Padding(11)
        };

        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 25));
        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.Controls.Add(
            Etiqueta(
                $"{numero:00}  ·  {parada.Codigo}",
                10,
                TransRedTheme.Acento,
                true), 0, 0);

        contenido.Controls.Add(
            Etiqueta(
                parada.Nombre,
                8.5f,
                TransRedTheme.TextoSecundario), 0, 1);

        tarjeta.Controls.Add(contenido);
        pasos.Controls.Add(tarjeta);

        pasos.ScrollControlIntoView(tarjeta);

        pasoActual++;

        lblEstado.Text =
            $"Paso {pasoActual} de {recorrido.Length}";

        if (pasoActual >= recorrido.Length)
        {
            animacion.Stop();
            btnReproducir.Text = "Reiniciar";
        }
        else
        {
            btnReproducir.Text = "Pausar";
        }
    }

    private void ComprobarConexion()
    {
        if (cmbOrigen.SelectedItem is not Parada origen ||
            cmbDestino.SelectedItem is not Parada destino)
            return;

        if (origen.Codigo == destino.Codigo)
        {
            lblConectividad.Text = "Es la misma parada";
            lblConectividad.ForeColor =
                TransRedTheme.Acento;
            return;
        }

        // Se conserva la comprobación del Core
        bool conectadas = gestor.EstanConectadas(
            origen.Codigo, destino.Codigo);

        lblConectividad.Text = conectadas
            ? "Las paradas están conectadas"
            : "Las paradas no están conectadas";

        lblConectividad.ForeColor = conectadas
            ? TransRedTheme.Acento
            : TransRedTheme.Error;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            animacion.Dispose();

        base.Dispose(disposing);
    }
}
