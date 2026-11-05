
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
            BackColor = TransRedTheme.Fondo
        };

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

        // Conserva las cinco vistas funcionales
        void MostrarSeccion(string seccion)
        {
            Control nuevaVista = seccion switch
            {
                "Explorar red" => new ExploradorPrincipal(gestor),

                "Paradas" => new VistaParadas(gestor),

                "Conexiones" => new VistaConexiones(gestor),

                "Planificar" => new VistaPlanificar(gestor),

                "Algoritmos" => new VistaAlgoritmos(gestor),

                _ => new Label
                {
                    Text = "Sección no disponible",
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
            Padding = new Padding(24, 16, 26, 16)
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 62));

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        tabla.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 260));

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
                "Segoe UI Symbol", 11, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var marca = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        marca.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        marca.RowStyles.Add(
            new RowStyle(SizeType.Percent, 60));

        marca.RowStyles.Add(
            new RowStyle(SizeType.Percent, 40));

        var nombre = Texto(
            "TransRed",
            21,
            FontStyle.Bold,
            TransRedTheme.Tinta
        );

        var subtitulo = Texto(
            "Movilidad urbana  ·  Mobility Studio",
            9,
            FontStyle.Regular,
            TransRedTheme.TextoSecundario
        );

        marca.Controls.Add(nombre, 0, 0);
        marca.Controls.Add(subtitulo, 0, 1);

        var ubicacion = Texto(
            "RED DEL SUROCCIDENTE\nGUATEMALA",
            8.5f,
            FontStyle.Bold,
            TransRedTheme.TextoSecundario
        );

        ubicacion.TextAlign =
            ContentAlignment.MiddleRight;

        tabla.Controls.Add(icono, 0, 0);
        tabla.Controls.Add(marca, 1, 0);
        tabla.Controls.Add(ubicacion, 2, 0);

        fondo.Controls.Add(tabla);

        return fondo;
    }

    private static Control CrearExplorador(
        IGestorTransporte gestor,
        Action abrirPlanificador)
    {
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = new Padding(16, 18, 18, 18),
            BackColor = TransRedTheme.Fondo
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 338));

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.Controls.Add(
            CrearPanelInicio(gestor, abrirPlanificador),
            0, 0);

        contenido.Controls.Add(
            CrearPanelMapa(gestor),
            1, 0);

        return contenido;
    }

    private static Control CrearPanelInicio(
        IGestorTransporte gestor,
        Action abrirPlanificador)
    {
        var tarjeta = new GradientSurface(
            Color.White,
            Color.FromArgb(250, 249, 244),
            22)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 14, 0),
            Padding = new Padding(22, 24, 22, 20)
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
            new RowStyle(SizeType.Absolute, 32));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 52));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 88));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 30));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 112));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 62));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 95));

        tabla.Controls.Add(
            Texto(
                "PLANIFICACIÓN DE VIAJE",
                8.5f,
                FontStyle.Bold,
                TransRedTheme.Cobre),
            0, 0);

        tabla.Controls.Add(
            Texto(
                "Explora la red",
                19,
                FontStyle.Bold,
                TransRedTheme.Tinta),
            0, 1);

        var descripcion = Texto(
            "Descubre las paradas, consulta sus conexiones y encuentra el mejor camino para tu próximo recorrido.",
            10,
            FontStyle.Regular,
            TransRedTheme.TextoSecundario
        );

        descripcion.TextAlign =
            ContentAlignment.TopLeft;

        tabla.Controls.Add(descripcion, 0, 2);

        tabla.Controls.Add(
            Texto(
                "TU RED EN MOVIMIENTO",
                8,
                FontStyle.Bold,
                TransRedTheme.TextoTenue),
            0, 3);

        int totalParadas =
            gestor.ObtenerTodasParadas().Length;

        int totalConexiones =
            gestor.ObtenerTodasConexiones().Length;

        var estadisticas = new GradientSurface(
            Color.FromArgb(235, 244, 232),
            Color.FromArgb(249, 245, 235),
            15)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 0, 12),
            Padding = new Padding(16, 12, 16, 12)
        };

        var estadisticasTexto = new Label
        {
            Text =
                $"{totalParadas} paradas registradas\n" +
                $"{totalConexiones} conexiones en la red",
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = TransRedTheme.Bosque,
            Font = new Font(
                "Segoe UI", 11, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        estadisticas.Controls.Add(estadisticasTexto);
        tabla.Controls.Add(estadisticas, 0, 4);

        var boton = new Button
        {
            Text = "Planificar recorrido   →",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 2, 0, 12)
        };

        UiStyles.AplicarBotonPrincipal(boton);

        boton.Click += (_, _) =>
            abrirPlanificador();

        tabla.Controls.Add(boton, 0, 5);

        var instruccion = Texto(
            "Selecciona una parada en el mapa para consultar sus conexiones. También puedes explorar las demás secciones desde la navegación.",
            9,
            FontStyle.Regular,
            TransRedTheme.TextoSecundario
        );

        instruccion.TextAlign =
            ContentAlignment.TopLeft;

        instruccion.Padding =
            new Padding(0, 16, 0, 0);

        tabla.Controls.Add(instruccion, 0, 6);

        var pie = new GradientSurface(
            Color.FromArgb(249, 246, 238),
            Color.FromArgb(231, 240, 230),
            14)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(14, 10, 14, 10)
        };

        var pieTexto = new Label
        {
            Text =
                "EXPLORACIÓN INTELIGENTE\n" +
                "Red de transporte del suroccidente",
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = TransRedTheme.Tinta,
            Font = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleLeft
        };

        pie.Controls.Add(pieTexto);
        tabla.Controls.Add(pie, 0, 7);

        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private static Control CrearPanelMapa(
        IGestorTransporte gestor)
    {
        var superficie = new GradientSurface(
            Color.FromArgb(237, 244, 235),
            Color.FromArgb(229, 238, 228),
            22)
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(16)
        };

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
            new RowStyle(SizeType.Absolute, 50));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        tabla.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 53));

        var titulo = Texto(
            "RED DE TRANSPORTE  /  EXPLORADOR",
            9,
            FontStyle.Bold,
            TransRedTheme.Bosque
        );

        titulo.Padding = new Padding(10, 0, 0, 0);

        // Conserva la conexión a los datos reales
        var mapa = new LienzoRed(gestor)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(2),
            BackColor = TransRedTheme.FondoMapa
        };

        var pie = new GradientSurface(
            Color.FromArgb(252, 251, 247),
            Color.FromArgb(243, 247, 240),
            12)
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(2, 9, 2, 0),
            Padding = new Padding(14, 6, 14, 6)
        };

        var pieTexto = new Label
        {
            Text =
                "RED INTERACTIVA  ·  " +
                "Selecciona una parada para conocer sus conexiones",
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = TransRedTheme.TextoSecundario,
            Font = new Font("Segoe UI", 8.5f),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };

        pie.Controls.Add(pieTexto);

        tabla.Controls.Add(titulo, 0, 0);
        tabla.Controls.Add(mapa, 0, 1);
        tabla.Controls.Add(pie, 0, 2);

        superficie.Controls.Add(tabla);

        return superficie;
    }

    private static Label Texto(
        string contenido,
        float tamano,
        FontStyle estilo,
        Color color)
    {
        return new Label
        {
            Text = contenido,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = Color.Transparent,
            ForeColor = color,
            Font = new Font(
                "Segoe UI", tamano, estilo),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = false
        };
    }
}
