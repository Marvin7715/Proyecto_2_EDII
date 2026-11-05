
using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

// Explorador principal y planificador visual
public class ExploradorPrincipal : UserControl
{
    private readonly IGestorTransporte gestor;
    private readonly LienzoRed mapa;

    private readonly ComboBox cmbOrigen = new();
    private readonly ComboBox cmbDestino = new();

    private readonly Button btnRapida = new();
    private readonly Button btnEconomica = new();

    private readonly Label lblEstado = new();
    private readonly Label lblResumen = new();

    private readonly FlowLayoutPanel listaPasos = new();

    private bool usarCosto;

    public ExploradorPrincipal(IGestorTransporte gestor)
    {
        this.gestor = gestor;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.Fondo;

        mapa = new LienzoRed(gestor)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty
        };

        var estructura = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(16, 16, 18, 16),
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        estructura.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 350));

        estructura.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        estructura.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        estructura.Controls.Add(CrearPlanificador(), 0, 0);
        estructura.Controls.Add(CrearMapa(), 1, 0);

        Controls.Add(estructura);

        CargarParadas();
        SeleccionarModo(false);
        MostrarEstadoInicial();
    }

    private Control CrearPlanificador()
    {
        var tarjeta = new GradientSurface(
            Color.White,
            Color.FromArgb(251, 249, 244),
            20)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 14, 0),
            Padding = new Padding(20, 22, 20, 18)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        int[] alturas =
        {
            30, 49, 75, 75, 73, 60, 29, 95
        };

        foreach (int alto in alturas)
        {
            tabla.RowStyles.Add(
                new RowStyle(SizeType.Absolute, alto));
        }

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Etiqueta(
                "PLANIFICA TU VIAJE",
                8.5f,
                TransRedTheme.Cobre,
                true),
            0, 0);

        tabla.Controls.Add(
            Etiqueta(
                "Tu próximo recorrido",
                15.5f,
                TransRedTheme.Tinta,
                true),
            0, 1);

        tabla.Controls.Add(
            CrearCampo("ORIGEN", cmbOrigen), 0, 2);

        tabla.Controls.Add(
            CrearCampo("DESTINO", cmbDestino), 0, 3);

        tabla.Controls.Add(CrearModos(), 0, 4);

        var botonBuscar = new Button
        {
            Text = "Explorar recorrido  →",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 7, 0, 9)
        };

        UiStyles.AplicarBotonPrincipal(botonBuscar);
        botonBuscar.Click += (_, _) => CalcularRuta();

        tabla.Controls.Add(botonBuscar, 0, 5);

        tabla.Controls.Add(
            Etiqueta(
                "RESUMEN DEL VIAJE",
                8,
                TransRedTheme.TextoTenue,
                true),
            0, 6);

        tabla.Controls.Add(CrearResumen(), 0, 7);
        tabla.Controls.Add(CrearItinerario(), 0, 8);

        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private Control CrearCampo(
        string titulo, ComboBox combo)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 5, 0, 9)
        };

        panel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 25));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(
            Etiqueta(
                titulo, 8,
                TransRedTheme.TextoSecundario,
                true),
            0, 0);

        UiStyles.AplicarCombo(combo);

        combo.Dock = DockStyle.Fill;
        combo.Margin = Padding.Empty;
        combo.DropDownWidth = 420;
        combo.DropDownHeight = 300;
        combo.IntegralHeight = false;

        panel.Controls.Add(combo, 0, 1);
        return panel;
    }

    private Control CrearModos()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 4, 0, 7),
            BackColor = Color.Transparent
        };

        panel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        panel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 26));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var etiqueta = Etiqueta(
            "TIPO DE RECORRIDO",
            8,
            TransRedTheme.TextoSecundario,
            true
        );

        panel.Controls.Add(etiqueta, 0, 0);
        panel.SetColumnSpan(etiqueta, 2);

        ConfigurarSelector(btnRapida, "Más rápida");
        ConfigurarSelector(btnEconomica, "Económica");

        btnRapida.Margin = new Padding(0, 2, 4, 0);
        btnEconomica.Margin = new Padding(4, 2, 0, 0);

        btnRapida.Click += (_, _) => SeleccionarModo(false);
        btnEconomica.Click += (_, _) => SeleccionarModo(true);

        panel.Controls.Add(btnRapida, 0, 1);
        panel.Controls.Add(btnEconomica, 1, 1);

        return panel;
    }

    private static void ConfigurarSelector(
        Button boton, string texto)
    {
        boton.Text = texto;
        boton.Dock = DockStyle.Fill;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.Font = new Font("Segoe UI", 9.5f);
        boton.Cursor = Cursors.Hand;
        boton.UseVisualStyleBackColor = false;
    }

    private Control CrearResumen()
    {
        var tarjeta = new GradientSurface(
            Color.FromArgb(232, 243, 232),
            Color.FromArgb(247, 244, 232),
            13)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 3, 0, 6),
            Padding = new Padding(13, 9, 13, 9)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 35));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        lblEstado.Dock = DockStyle.Fill;
        lblEstado.BackColor = Color.Transparent;
        lblEstado.ForeColor = TransRedTheme.Bosque;
        lblEstado.Font = new Font(
            "Segoe UI", 12, FontStyle.Bold);
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;
        lblEstado.AutoEllipsis = true;

        lblResumen.Dock = DockStyle.Fill;
        lblResumen.BackColor = Color.Transparent;
        lblResumen.ForeColor =
            TransRedTheme.TextoSecundario;
        lblResumen.Font = new Font("Segoe UI", 9);
        lblResumen.TextAlign = ContentAlignment.MiddleLeft;
        lblResumen.AutoEllipsis = true;

        tabla.Controls.Add(lblEstado, 0, 0);
        tabla.Controls.Add(lblResumen, 0, 1);

        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private Control CrearItinerario()
    {
        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = Color.Transparent
        };

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 28));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenedor.Controls.Add(
            Etiqueta(
                "PARADAS DEL RECORRIDO",
                8,
                TransRedTheme.TextoTenue,
                true),
            0, 0);

        listaPasos.Dock = DockStyle.Fill;
        listaPasos.FlowDirection = FlowDirection.TopDown;
        listaPasos.WrapContents = false;
        listaPasos.AutoScroll = true;
        listaPasos.Margin = Padding.Empty;
        listaPasos.Padding = new Padding(0, 5, 4, 5);
        listaPasos.BackColor = Color.Transparent;

        listaPasos.Resize += (_, _) =>
        {
            foreach (Control control in listaPasos.Controls)
                control.Width = Math.Max(
                    100, listaPasos.ClientSize.Width - 8);
        };

        contenedor.Controls.Add(listaPasos, 0, 1);
        return contenedor;
    }

    private Control CrearMapa()
    {
        var superficie = new GradientSurface(
            Color.FromArgb(239, 245, 236),
            Color.FromArgb(228, 238, 228),
            22)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(14)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 48));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Etiqueta(
                "RED REGIONAL  /  EXPLORADOR INTERACTIVO",
                9,
                TransRedTheme.Bosque,
                true),
            0, 0);

        tabla.Controls.Add(mapa, 0, 1);

        superficie.Controls.Add(tabla);
        return superficie;
    }

    private void CargarParadas()
    {
        cmbOrigen.Items.Clear();
        cmbDestino.Items.Clear();

        foreach (Parada parada in gestor.ObtenerTodasParadas())
        {
            cmbOrigen.Items.Add(parada);
            cmbDestino.Items.Add(parada);
        }

        cmbOrigen.SelectedIndex = -1;
        cmbDestino.SelectedIndex = -1;
    }

    private void SeleccionarModo(bool economica)
    {
        usarCosto = economica;

        btnRapida.BackColor = economica
            ? TransRedTheme.AcentoSuave
            : TransRedTheme.Bosque;

        btnRapida.ForeColor = economica
            ? TransRedTheme.Bosque
            : Color.White;

        btnEconomica.BackColor = economica
            ? TransRedTheme.Bosque
            : TransRedTheme.AcentoSuave;

        btnEconomica.ForeColor = economica
            ? Color.White
            : TransRedTheme.Bosque;
    }

    private void MostrarEstadoInicial()
    {
        lblEstado.Text = "Tu viaje empieza aquí";
        lblResumen.Text = "Elige origen, destino y tipo de ruta";

        listaPasos.Controls.Clear();
    }

    private void CalcularRuta()
    {
        if (cmbOrigen.SelectedItem is not Parada origen ||
            cmbDestino.SelectedItem is not Parada destino)
        {
            lblEstado.Text = "Faltan paradas";
            lblResumen.Text = "Selecciona origen y destino";

            listaPasos.Controls.Clear();
            mapa.MostrarRuta(Array.Empty<string>());
            return;
        }

        if (origen.Codigo == destino.Codigo)
        {
            lblEstado.Text = "Destino no válido";
            lblResumen.Text = "Elige dos paradas diferentes";

            listaPasos.Controls.Clear();
            mapa.MostrarRuta(Array.Empty<string>());
            return;
        }

        // El Core conserva todo el cálculo de Dijkstra
        ResultadoRuta? resultado =
            gestor.ObtenerResultadoRuta(
                origen.Codigo,
                destino.Codigo,
                usarCosto
            );

        if (resultado == null ||
            resultado.Paradas.Length == 0)
        {
            lblEstado.Text = "Ruta no disponible";
            lblResumen.Text = "No existe un camino entre esas paradas";

            listaPasos.Controls.Clear();
            mapa.MostrarRuta(Array.Empty<string>());
            return;
        }

        MostrarResultado(resultado);
    }

    private void MostrarResultado(ResultadoRuta resultado)
    {
        lblEstado.Text = usarCosto
            ? $"Q {resultado.CostoTotal:0.##}  ·  Económica"
            : $"{resultado.TiempoTotal:0.##} min  ·  Rápida";

        lblResumen.Text =
            $"{resultado.TiempoTotal:0.##} min   ·   " +
            $"Q {resultado.CostoTotal:0.##}   ·   " +
            $"{resultado.Paradas.Length} paradas";

        listaPasos.SuspendLayout();
        listaPasos.Controls.Clear();

        for (int i = 0; i < resultado.Paradas.Length; i++)
        {
            Parada parada = resultado.Paradas[i];

            bool primera = i == 0;
            bool ultima = i == resultado.Paradas.Length - 1;

            listaPasos.Controls.Add(
                CrearPaso(parada, i + 1, primera, ultima));
        }

        listaPasos.ResumeLayout(true);

        string[] codigos = new string[
            resultado.Paradas.Length];

        for (int i = 0; i < codigos.Length; i++)
            codigos[i] = resultado.Paradas[i].Codigo;

        mapa.MostrarRuta(codigos);
    }

    private Control CrearPaso(
        Parada parada,
        int numero,
        bool inicio,
        bool fin)
    {
        Color acento = inicio
            ? TransRedTheme.Acento
            : fin
                ? TransRedTheme.Cobre
                : Color.FromArgb(153, 183, 158);

        var fila = new TableLayoutPanel
        {
            Width = Math.Max(
                150, listaPasos.ClientSize.Width - 8),
            Height = 56,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 0, 0, 4),
            Padding = Padding.Empty,
            BackColor = Color.Transparent
        };

        fila.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 40));

        fila.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        var marca = new Label
        {
            Text = inicio ? "●" : fin ? "◆" : "○",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI Symbol", 16),
            ForeColor = acento,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var informacion = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = Color.Transparent,
            Margin = Padding.Empty
        };

        informacion.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 25));

        informacion.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        string titulo = inicio
            ? $"ORIGEN  ·  {parada.Codigo}"
            : fin
                ? $"DESTINO  ·  {parada.Codigo}"
                : $"{numero:00}  ·  {parada.Codigo}";

        informacion.Controls.Add(
            Etiqueta(
                titulo,
                9,
                inicio || fin
                    ? TransRedTheme.Bosque
                    : TransRedTheme.Acento,
                true),
            0, 0);

        informacion.Controls.Add(
            Etiqueta(
                parada.Nombre,
                8.5f,
                TransRedTheme.TextoSecundario),
            0, 1);

        fila.Controls.Add(marca, 0, 0);
        fila.Controls.Add(informacion, 1, 0);

        return fila;
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
}
