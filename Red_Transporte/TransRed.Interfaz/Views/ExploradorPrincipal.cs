
using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

// Planificador con resultados y mapa en una sola pantalla
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

    private readonly FlowLayoutPanel itinerario = new();

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

        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = new Padding(16, 14, 18, 14),
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 350));

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        principal.Controls.Add(CrearPlanificador(), 0, 0);
        principal.Controls.Add(CrearMapa(), 1, 0);

        Controls.Add(principal);

        CargarParadas();
        BuscadorTrie.Activar(cmbOrigen, gestor);
        BuscadorTrie.Activar(cmbDestino, gestor);
        SeleccionarModo(false);
        RestablecerResultado();
    }

    private Control CrearPlanificador()
    {
        var tarjeta = new GradientSurface(
            Color.White,
            Color.FromArgb(250, 249, 244),
            20)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 13, 0),
            Padding = new Padding(20, 20, 20, 16)
        };

        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        int[] alturas = { 26, 48, 72, 72, 68, 56, 28, 86 };

        foreach (int altura in alturas)
        {
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, altura));
        }

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.Controls.Add(
            Etiqueta("PLANIFICA TU VIAJE", 8.5f,
                TransRedTheme.Cobre, true),
            0, 0);

        contenido.Controls.Add(
            Etiqueta("Tu recorrido", 19,
                TransRedTheme.Tinta, true),
            0, 1);

        contenido.Controls.Add(
            CrearCampo("ORIGEN", cmbOrigen),
            0, 2);

        contenido.Controls.Add(
            CrearCampo("DESTINO", cmbDestino),
            0, 3);

        contenido.Controls.Add(CrearSelectorModo(), 0, 4);

        var buscar = new Button
        {
            Text = "Explorar recorrido  →",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 5, 0, 9)
        };

        UiStyles.AplicarBotonPrincipal(buscar);
        buscar.Click += (_, _) => CalcularRuta();

        contenido.Controls.Add(buscar, 0, 5);

        contenido.Controls.Add(
            Etiqueta("RESUMEN DEL VIAJE", 8,
                TransRedTheme.TextoTenue, true),
            0, 6);

        contenido.Controls.Add(CrearResumen(), 0, 7);
        contenido.Controls.Add(CrearItinerario(), 0, 8);

        tarjeta.Controls.Add(contenido);
        return tarjeta;
    }

    private Control CrearCampo(string titulo, ComboBox combo)
    {
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 2, 0, 8)
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 25));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Etiqueta(titulo, 8,
                TransRedTheme.TextoSecundario, true),
            0, 0);

        UiStyles.AplicarCombo(combo);

        combo.Dock = DockStyle.Fill;
        combo.Margin = Padding.Empty;
        combo.DropDownWidth = 430;
        combo.DropDownHeight = 290;
        combo.IntegralHeight = false;

        tabla.Controls.Add(combo, 0, 1);

        return tabla;
    }

    private Control CrearSelectorModo()
    {
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = Padding.Empty,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 2, 0, 5)
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 50));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 25));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var etiqueta = Etiqueta(
            "TIPO DE RECORRIDO",
            8,
            TransRedTheme.TextoSecundario,
            true);

        tabla.Controls.Add(etiqueta, 0, 0);
        tabla.SetColumnSpan(etiqueta, 2);

        ConfigurarOpcion(btnRapida, "Más rápida");
        ConfigurarOpcion(btnEconomica, "Económica");

        btnRapida.Margin = new Padding(0, 1, 5, 0);
        btnEconomica.Margin = new Padding(5, 1, 0, 0);

        btnRapida.Click += (_, _) => SeleccionarModo(false);
        btnEconomica.Click += (_, _) => SeleccionarModo(true);

        tabla.Controls.Add(btnRapida, 0, 1);
        tabla.Controls.Add(btnEconomica, 1, 1);

        return tabla;
    }

    private static void ConfigurarOpcion(
        Button boton, string texto)
    {
        boton.Text = texto;
        boton.Dock = DockStyle.Fill;
        boton.Cursor = Cursors.Hand;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.UseVisualStyleBackColor = false;
        boton.Font = new Font("Segoe UI", 9.5f);
    }

    private Control CrearResumen()
    {
        var tarjeta = new GradientSurface(
            Color.FromArgb(233, 243, 232),
            Color.FromArgb(247, 244, 232),
            14)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 3, 0, 6),
            Padding = new Padding(12, 9, 12, 9)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 56));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 44));

        lblEstado.Dock = DockStyle.Fill;
        lblEstado.Font = new Font(
            "Segoe UI", 11.5f, FontStyle.Bold);
        lblEstado.ForeColor = TransRedTheme.Bosque;
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;
        lblEstado.AutoEllipsis = true;

        lblResumen.Dock = DockStyle.Fill;
        lblResumen.Font = new Font("Segoe UI", 9);
        lblResumen.ForeColor =
            TransRedTheme.TextoSecundario;
        lblResumen.TextAlign = ContentAlignment.MiddleLeft;
        lblResumen.AutoEllipsis = true;

        tabla.Controls.Add(lblEstado, 0, 0);
        tabla.Controls.Add(lblResumen, 0, 1);

        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private Control CrearItinerario()
    {
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = Color.Transparent
        };

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 30));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Etiqueta("PARADAS DEL RECORRIDO", 8,
                TransRedTheme.TextoTenue, true),
            0, 0);

        itinerario.Dock = DockStyle.Fill;
        itinerario.FlowDirection = FlowDirection.TopDown;
        itinerario.WrapContents = false;
        itinerario.AutoScroll = true;
        itinerario.Margin = Padding.Empty;
        itinerario.Padding = new Padding(0, 4, 2, 4);
        itinerario.BackColor = Color.Transparent;

        itinerario.Resize += (_, _) => AjustarItinerario();

        tabla.Controls.Add(itinerario, 0, 1);

        return tabla;
    }

    private Control CrearMapa()
    {
        var superficie = new GradientSurface(
            Color.FromArgb(237, 244, 235),
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
            new RowStyle(SizeType.Absolute, 46));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            Etiqueta(
                "RED REGIONAL  /  MAPA INTERACTIVO",
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

    private void RestablecerResultado()
    {
        lblEstado.Text = "Tu viaje empieza aquí";
        lblResumen.Text = "Selecciona dos paradas";
        LimpiarItinerario();
    }

    private void CalcularRuta()
    {
        if (cmbOrigen.SelectedItem is not Parada origen ||
            cmbDestino.SelectedItem is not Parada destino)
        {
            MostrarError(
                "Faltan paradas",
                "Selecciona origen y destino");
            return;
        }

        if (origen.Codigo == destino.Codigo)
        {
            MostrarError(
                "Destino no válido",
                "Elige dos paradas diferentes");
            return;
        }

        // El resultado se calcula en el Core
        ResultadoRuta? resultado =
            gestor.ObtenerResultadoRuta(
                origen.Codigo,
                destino.Codigo,
                usarCosto);

        if (resultado == null ||
            resultado.Paradas.Length == 0)
        {
            MostrarError(
                "Ruta no disponible",
                "No existe un camino entre esas paradas");
            return;
        }

        MostrarResultado(resultado);
    }

    private void MostrarResultado(ResultadoRuta resultado)
    {
        lblEstado.Text = usarCosto
            ? $"Q {resultado.CostoTotal:0.##} · Económica"
            : $"{resultado.TiempoTotal:0.##} min · Rápida";

        lblResumen.Text =
            $"{resultado.TiempoTotal:0.##} min  ·  " +
            $"Q {resultado.CostoTotal:0.##}  ·  " +
            $"{resultado.Paradas.Length} paradas";

        LimpiarItinerario();

        itinerario.SuspendLayout();

        for (int i = 0; i < resultado.Paradas.Length; i++)
        {
            Parada parada = resultado.Paradas[i];

            itinerario.Controls.Add(
                CrearPaso(
                    parada,
                    i + 1,
                    i == 0,
                    i == resultado.Paradas.Length - 1));
        }

        itinerario.ResumeLayout(true);
        AjustarItinerario();

        string[] codigos = new string[
            resultado.Paradas.Length];

        for (int i = 0; i < codigos.Length; i++)
        {
            codigos[i] = resultado.Paradas[i].Codigo;
        }

        // Conserva la animación existente
        mapa.MostrarRuta(codigos);
    }

    private Control CrearPaso(
        Parada parada,
        int numero,
        bool inicio,
        bool destino)
    {
        var fila = new Panel
        {
            Height = 68,
            Width = Math.Max(
                120,
                itinerario.ClientSize.Width -
                SystemInformation.VerticalScrollBarWidth - 14),
            Margin = new Padding(0, 0, 0, 5),
            BackColor = inicio || destino
                ? Color.FromArgb(238, 246, 236)
                : Color.Transparent
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(3, 3, 3, 3),
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 35));

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 28));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var indicador = Etiqueta(
            inicio ? "●" : destino ? "◆" : "○",
            15,
            destino ? TransRedTheme.Cobre
                    : TransRedTheme.Acento,
            true);

        indicador.TextAlign =
            ContentAlignment.MiddleCenter;

        tabla.Controls.Add(indicador, 0, 0);
        tabla.SetRowSpan(indicador, 2);

        string titulo = inicio
            ? $"ORIGEN · {parada.Codigo}"
            : destino
                ? $"DESTINO · {parada.Codigo}"
                : $"{numero:00} · {parada.Codigo}";

        tabla.Controls.Add(
            Etiqueta(titulo, 9,
                TransRedTheme.Bosque, true),
            1, 0);

        var nombre = Etiqueta(
            parada.Nombre,
            8.5f,
            TransRedTheme.TextoSecundario);

        nombre.TextAlign = ContentAlignment.TopLeft;
        nombre.AutoEllipsis = true;

        tabla.Controls.Add(nombre, 1, 1);
        fila.Controls.Add(tabla);

        return fila;
    }

    private void AjustarItinerario()
    {
        int ancho = Math.Max(
            100,
            itinerario.ClientSize.Width -
            SystemInformation.VerticalScrollBarWidth - 14);

        foreach (Control fila in itinerario.Controls)
        {
            fila.Width = ancho;
        }
    }

    private void LimpiarItinerario()
    {
        while (itinerario.Controls.Count > 0)
        {
            Control control = itinerario.Controls[0];
            itinerario.Controls.RemoveAt(0);
            control.Dispose();
        }

        itinerario.AutoScrollPosition = Point.Empty;
    }

    private void MostrarError(string titulo, string detalle)
    {
        lblEstado.Text = titulo;
        lblResumen.Text = detalle;

        LimpiarItinerario();
        mapa.MostrarRuta(Array.Empty<string>());
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
