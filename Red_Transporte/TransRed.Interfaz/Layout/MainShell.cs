
using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;
using TransRed.Interfaz.Views;

namespace TransRed.Interfaz.Layout;

public static class MainShell
{
    public static void Construir(
        Form ventana,
        IGestorTransporte gestor)
    {
        ventana.SuspendLayout();
        ventana.Controls.Clear();

        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 96));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var cuerpo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        // Ancho restaurado para evitar textos recortados.
        cuerpo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 218));

        cuerpo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        cuerpo.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var areaContenido = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        NavigationBar? navegacion = null;

        void MostrarSeccion(string seccion)
        {
            Control nuevaVista = seccion switch
            {
                "Explorar red" =>
                    new ExploradorPrincipal(gestor),

                "Paradas" =>
                    new VistaParadas(gestor),

                "Conexiones" =>
                    new VistaConexiones(gestor),

                "Algoritmos" =>
                    new VistaAlgoritmos(gestor),

                _ => new Label
                {
                    Text = "Sección no disponible",
                    Dock = DockStyle.Fill,
                    ForeColor = TransRedTheme.Tinta,
                    TextAlign = ContentAlignment.MiddleCenter
                }
            };

            nuevaVista.Dock = DockStyle.Fill;

            areaContenido.SuspendLayout();

            Control? anterior =
                areaContenido.Controls.Count > 0
                    ? areaContenido.Controls[0]
                    : null;

            areaContenido.Controls.Add(nuevaVista);
            nuevaVista.BringToFront();

            if (anterior != null)
            {
                areaContenido.Controls.Remove(anterior);
                anterior.Dispose();
            }

            areaContenido.ResumeLayout(true);
            navegacion?.Seleccionar(seccion);
        }

        navegacion = new NavigationBar(MostrarSeccion);

        cuerpo.Controls.Add(navegacion, 0, 0);
        cuerpo.Controls.Add(areaContenido, 1, 0);

        principal.Controls.Add(CrearEncabezado(), 0, 0);
        principal.Controls.Add(cuerpo, 0, 1);

        ventana.Controls.Add(principal);

        MostrarSeccion("Explorar red");

        ventana.ResumeLayout(true);
    }

    private static Control CrearEncabezado()
    {
        var fondo = new GradientSurface(
            Color.FromArgb(255, 252, 247),
            Color.FromArgb(231, 241, 231),
            0)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(20, 13, 25, 13)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 60));

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 215));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var icono = new Label
        {
            Text = "●━●",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 10, 4),
            BackColor = TransRedTheme.Bosque,
            ForeColor = Color.White,
            Font = new Font(
                "Segoe UI Symbol",
                11,
                FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var marca = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.Transparent
        };

        marca.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        marca.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42));

        marca.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        var nombre = new Label
        {
            Text = "TransRed",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.Transparent,
            ForeColor = TransRedTheme.Tinta,
            Font = new Font(
                "Segoe UI",
                21,
                FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = false,
            UseCompatibleTextRendering = true
        };

        var subtitulo = new Label
        {
            Text = "Movilidad urbana  ·  Mobility Studio",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = Color.Transparent,
            ForeColor = TransRedTheme.TextoSecundario,
            Font = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = false
        };

        marca.Controls.Add(nombre, 0, 0);
        marca.Controls.Add(subtitulo, 0, 1);

        var ubicacion = new Label
        {
            Text = "RED DEL SUROCCIDENTE\nGUATEMALA",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = Color.Transparent,
            ForeColor = TransRedTheme.TextoSecundario,
            Font = new Font(
                "Segoe UI",
                8.5f,
                FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };

        tabla.Controls.Add(icono, 0, 0);
        tabla.Controls.Add(marca, 1, 0);
        tabla.Controls.Add(ubicacion, 2, 0);

        fondo.Controls.Add(tabla);

        return fondo;
    }
}
