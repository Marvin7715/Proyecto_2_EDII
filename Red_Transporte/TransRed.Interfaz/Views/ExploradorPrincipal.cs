
using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

// Planificación y mapa en una misma pantalla
public class ExploradorPrincipal : UserControl
{
    private readonly IGestorTransporte gestor;
    private readonly LienzoRed mapa;

    private readonly ComboBox cmbOrigen = new();
    private readonly ComboBox cmbDestino = new();

    private readonly Button btnRapida = new();
    private readonly Button btnEconomica = new();

    private readonly Label lblEstado = new();
    private readonly Label lblDetalle = new();

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
            Padding = new Padding(16, 16, 18, 16),
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 332));

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        principal.Controls.Add(CrearPlanificador(), 0, 0);
        principal.Controls.Add(CrearMapa(), 1, 0);

        Controls.Add(principal);

        CargarParadas();
        SeleccionarModo(false);
    }

    private Control CrearPlanificador()
    {
        var tarjeta = new GradientSurface(
            Color.White,
            Color.FromArgb(250, 249, 244),
            20)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 14, 0),
            Padding = new Padding(22, 22, 22, 18)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 8,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 28));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 58));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 79));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 79));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 74));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 59));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 24));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            CrearEtiqueta(
                "PLANIFICACIÓN DE VIAJE",
                8,
                true,
                TransRedTheme.Cobre),
            0, 0);

        tabla.Controls.Add(
            CrearEtiqueta(
                "Explora la red",
                17,
                true,
                TransRedTheme.Tinta),
            0, 1);

        tabla.Controls.Add(
            CrearCampo("ORIGEN", cmbOrigen),
            0, 2);

        tabla.Controls.Add(
            CrearCampo("DESTINO", cmbDestino),
            0, 3);

        tabla.Controls.Add(CrearModos(), 0, 4);

        var botonBuscar = new Button
        {
            Text = "Buscar recorrido   →",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 8, 0, 7)
        };

        UiStyles.AplicarBotonPrincipal(botonBuscar);
        botonBuscar.Click += (_, _) => CalcularRuta();

        tabla.Controls.Add(botonBuscar, 0, 5);

        tabla.Controls.Add(
            CrearEtiqueta(
                "TU RECORRIDO",
                8,
                true,
                TransRedTheme.TextoTenue),
            0, 6);

        var resultado = new GradientSurface(
            Color.FromArgb(235, 244, 232),
            Color.FromArgb(248, 244, 234),
            14)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 6, 0, 0),
            Padding = new Padding(13, 12, 13, 12)
        };

        var informacion = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent
        };

        informacion.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        informacion.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 48));

        informacion.RowStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        lblEstado.Text = "Sin recorrido seleccionado";
        lblEstado.Dock = DockStyle.Fill;
        lblEstado.ForeColor = TransRedTheme.Bosque;
        lblEstado.Font = new Font(
            "Segoe UI", 11, FontStyle.Bold);
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;

        lblDetalle.Text = "Selecciona origen y destino";
        lblDetalle.Dock = DockStyle.Fill;
        lblDetalle.ForeColor =
            TransRedTheme.TextoSecundario;
        lblDetalle.Font = new Font("Segoe UI", 9);
        lblDetalle.TextAlign = ContentAlignment.TopLeft;

        informacion.Controls.Add(lblEstado, 0, 0);
        informacion.Controls.Add(lblDetalle, 0, 1);

        resultado.Controls.Add(informacion);
        tabla.Controls.Add(resultado, 0, 7);

        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private Control CrearCampo(
        string titulo, ComboBox combo)
    {
        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 5, 0, 10),
            BackColor = Color.Transparent
        };

        contenedor.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 26));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenedor.Controls.Add(
            CrearEtiqueta(
                titulo, 8, true,
                TransRedTheme.TextoSecundario),
            0, 0);

        UiStyles.AplicarCombo(combo);

        combo.Dock = DockStyle.Fill;
        combo.Margin = Padding.Empty;
        combo.DropDownWidth = 360;
        combo.IntegralHeight = false;
        combo.DropDownHeight = 280;

        contenedor.Controls.Add(combo, 0, 1);

        return contenedor;
    }

    private Control CrearModos()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 4, 0, 6),
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

        panel.Controls.Add(
            CrearEtiqueta(
                "TIPO DE RECORRIDO",
                8, true,
                TransRedTheme.TextoSecundario),
            0, 0);

        panel.SetColumnSpan(panel.Controls[0], 2);

        btnRapida.Text = "Más rápida";
        btnEconomica.Text = "Económica";

        foreach (Button boton in new[] {
            btnRapida, btnEconomica
        })
        {
            boton.Dock = DockStyle.Fill;
            boton.Margin = new Padding(0, 2, 5, 0);
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.Cursor = Cursors.Hand;
            boton.Font = new Font("Segoe UI", 9);
            boton.UseVisualStyleBackColor = false;
        }

        btnEconomica.Margin = new Padding(5, 2, 0, 0);

        btnRapida.Click += (_, _) => SeleccionarModo(false);
        btnEconomica.Click += (_, _) => SeleccionarModo(true);

        panel.Controls.Add(btnRapida, 0, 1);
        panel.Controls.Add(btnEconomica, 1, 1);

        return panel;
    }

    private Control CrearMapa()
    {
        var superficie = new GradientSurface(
            Color.FromArgb(237, 244, 235),
            Color.FromArgb(229, 238, 228),
            22)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(13)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 46));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.Controls.Add(
            CrearEtiqueta(
                "RED REGIONAL   /   MAPA INTERACTIVO",
                9,
                true,
                TransRedTheme.Bosque),
            0, 0);

        tabla.Controls.Add(mapa, 0, 1);

        superficie.Controls.Add(tabla);
        return superficie;
    }

    private void CargarParadas()
    {
        cmbOrigen.Items.Clear();
        cmbDestino.Items.Clear();

        Parada[] paradas = gestor.ObtenerTodasParadas();

        foreach (Parada parada in paradas)
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

    private void CalcularRuta()
    {
        if (cmbOrigen.SelectedItem is not Parada origen ||
            cmbDestino.SelectedItem is not Parada destino)
        {
            lblEstado.Text = "Selecciona ambas paradas";
            lblDetalle.Text =
                "Necesitamos un origen y un destino";
            mapa.MostrarRuta(Array.Empty<string>());
            return;
        }

        if (origen.Codigo == destino.Codigo)
        {
            lblEstado.Text = "Elige un destino diferente";
            lblDetalle.Text =
                "Origen y destino no pueden ser iguales";
            mapa.MostrarRuta(Array.Empty<string>());
            return;
        }

        // Utiliza el mismo cálculo del Core
        ResultadoRuta? resultado =
            gestor.ObtenerResultadoRuta(
                origen.Codigo,
                destino.Codigo,
                usarCosto);

        if (resultado == null ||
            resultado.Paradas.Length == 0)
        {
            lblEstado.Text = "No se encontró un recorrido";
            lblDetalle.Text =
                "Prueba con otras paradas";
            mapa.MostrarRuta(Array.Empty<string>());
            return;
        }

        string[] codigos = new string[
            resultado.Paradas.Length];

        for (int i = 0; i < codigos.Length; i++)
            codigos[i] = resultado.Paradas[i].Codigo;

        lblEstado.Text = usarCosto
            ? $"Q {resultado.CostoTotal:0.##} · Ruta económica"
            : $"{resultado.TiempoTotal:0.##} min · Ruta rápida";

        lblDetalle.Text =
            $"{resultado.Paradas.Length} paradas\n" +
            $"{resultado.TiempoTotal:0.##} min  ·  " +
            $"Q {resultado.CostoTotal:0.##}\n" +
            $"{origen.Nombre} → {destino.Nombre}";

        mapa.MostrarRuta(codigos);
    }

    private static Label CrearEtiqueta(
        string texto,
        float tamano,
        bool negrita,
        Color color)
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
