
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Layout;

public static class MainShell
{
    private static readonly Color FondoRed =
        Color.FromArgb(241, 246, 242);

    private static readonly Color FondoCampo =
        Color.FromArgb(247, 249, 247);

    private static readonly Color TextoTenue =
        Color.FromArgb(124, 143, 135);

    public static void Construir(Form ventana)
    {
        ventana.SuspendLayout();
        ventana.Controls.Clear();

        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 82));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 58));

        principal.Controls.Add(CrearEncabezado(), 0, 0);
        principal.Controls.Add(CrearContenido(), 0, 1);
        principal.Controls.Add(CrearNavegacion(), 0, 2);

        ventana.Controls.Add(principal);
        ventana.ResumeLayout(true);
    }

    // Encabezado fijo y sin elementos superpuestos
    private static Control CrearEncabezado()
    {
        var encabezado = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Superficie
        };

        var simbolo = new SimboloTransRed
        {
            Location = new Point(22, 17),
            Size = new Size(46, 46)
        };

        var nombre = new Label
        {
            Text = "transred",
            Location = new Point(84, 14),
            Size = new Size(240, 34),
            Font = new Font(
                "Segoe UI", 18, FontStyle.Bold),
            ForeColor = TransRedTheme.Tinta,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false
        };

        var subtitulo = new Label
        {
            Text = "MOVILIDAD URBANA",
            Location = new Point(86, 49),
            Size = new Size(240, 18),
            Font = new Font(
                "Segoe UI", 8, FontStyle.Regular),
            ForeColor = TextoTenue,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false
        };

        var contexto = new Label
        {
            Text = "EXPLORADOR DE RED",
            Dock = DockStyle.Right,
            Width = 205,
            Padding = new Padding(0, 0, 24, 0),
            ForeColor = TransRedTheme.TextoSecundario,
            Font = new Font(
                "Segoe UI", 8.5f, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleRight
        };

        var separador = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 1,
            BackColor = TransRedTheme.Borde
        };

        encabezado.Controls.Add(simbolo);
        encabezado.Controls.Add(nombre);
        encabezado.Controls.Add(subtitulo);
        encabezado.Controls.Add(contexto);
        encabezado.Controls.Add(separador);

        return encabezado;
    }

    // Distribución de panel izquierdo y lienzo central
    private static Control CrearContenido()
    {
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = new Padding(18, 18, 18, 18),
            BackColor = TransRedTheme.Fondo
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 310));

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.Controls.Add(CrearPlanificador(), 0, 0);
        contenido.Controls.Add(CrearLienzo(), 1, 0);

        return contenido;
    }

    // Panel lateral con espacio para futuros controles
    private static Control CrearPlanificador()
    {
        var lateral = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 7,
            Margin = new Padding(0, 0, 16, 0),
            Padding = new Padding(20, 22, 20, 18),
            BackColor = TransRedTheme.Superficie
        };

        lateral.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 24));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 43));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 44));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 86));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 86));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55));

        lateral.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        lateral.Controls.Add(
            CrearTexto(
                "01 / PLANIFICADOR",
                8,
                FontStyle.Bold,
                TransRedTheme.Acento),
            0, 0);

        lateral.Controls.Add(
            CrearTexto(
                "Nuevo recorrido",
                15,
                FontStyle.Bold,
                TransRedTheme.Tinta),
            0, 1);

        lateral.Controls.Add(
            CrearTexto(
                "Consulta rutas entre paradas",
                9,
                FontStyle.Regular,
                TransRedTheme.TextoSecundario),
            0, 2);

        lateral.Controls.Add(
            CrearCampoVisual(
                "ORIGEN", "Seleccionar origen"),
            0, 3);

        lateral.Controls.Add(
            CrearCampoVisual(
                "DESTINO", "Seleccionar destino"),
            0, 4);

        var aviso = new Label
        {
            Text = "Cálculo de rutas próximamente",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 8, 0, 7),
            BackColor = TransRedTheme.AcentoSuave,
            ForeColor = TransRedTheme.Acento,
            Font = new Font(
                "Segoe UI", 8.5f),
            TextAlign = ContentAlignment.MiddleCenter,
            AutoEllipsis = true
        };

        lateral.Controls.Add(aviso, 0, 5);

        var areaEstado = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Superficie
        };

        var estado = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 96,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = new Padding(13, 10, 13, 8),
            BackColor = FondoCampo
        };

        estado.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        estado.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 22));

        estado.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 29));

        estado.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        estado.Controls.Add(
            CrearTexto(
                "ESTADO ACTUAL", 7.5f,
                FontStyle.Bold, TextoTenue),
            0, 0);

        estado.Controls.Add(
            CrearTexto(
                "Sin recorrido activo", 10,
                FontStyle.Bold, TransRedTheme.Tinta),
            0, 1);

        estado.Controls.Add(
            CrearTexto(
                "Todavía no hay resultados", 8.5f,
                FontStyle.Regular,
                TransRedTheme.TextoSecundario),
            0, 2);

        areaEstado.Controls.Add(estado);
        lateral.Controls.Add(areaEstado, 0, 6);

        return lateral;
    }

    // Campos de muestra, sin funcionalidad todavía
    private static Control CrearCampoVisual(
        string titulo,
        string descripcion)
    {
        var bloque = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 5, 0, 12)
        };

        bloque.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        bloque.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 25));

        bloque.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        bloque.Controls.Add(
            CrearTexto(
                titulo, 8, FontStyle.Bold,
                TransRedTheme.TextoSecundario),
            0, 0);

        var campo = new Label
        {
            Text = descripcion,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(12, 0, 8, 0),
            BackColor = FondoCampo,
            ForeColor = TextoTenue,
            Font = new Font("Segoe UI", 9.5f),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };

        bloque.Controls.Add(campo, 0, 1);

        return bloque;
    }

    // Lienzo adaptable a cualquier tamaño de ventana
    private static Control CrearLienzo()
    {
        var lienzo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = new Padding(25, 20, 25, 18),
            BackColor = FondoRed
        };

        lienzo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        lienzo.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 38));

        lienzo.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        lienzo.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 28));

        lienzo.Controls.Add(
            CrearTexto(
                "02 / VISUALIZACIÓN DE RED",
                8,
                FontStyle.Bold,
                TransRedTheme.TextoSecundario),
            0, 0);

        // Todo el mensaje se dibuja en un solo control
        var estadoVacio = new EstadoRedVacia
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = FondoRed
        };

        lienzo.Controls.Add(estadoVacio, 0, 1);

        var pie = CrearTexto(
            "RED SIN DATOS CARGADOS",
            8,
            FontStyle.Regular,
            TextoTenue
        );

        pie.TextAlign = ContentAlignment.MiddleRight;

        lienzo.Controls.Add(pie, 0, 2);

        return lienzo;
    }

    // Navegación visual del commit 1.2
    private static Control CrearNavegacion()
    {
        var fondo = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(18, 4, 18, 4),
            BackColor = TransRedTheme.Superficie
        };

        var barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            AutoScroll = true,
            BackColor = TransRedTheme.Superficie
        };

        barra.Controls.Add(
            CrearPestana("Explorar red", true, 130));

        barra.Controls.Add(
            CrearPestana("Paradas", false, 94));

        barra.Controls.Add(
            CrearPestana("Conexiones", false, 116));

        barra.Controls.Add(
            CrearPestana("Planificar", false, 104));

        barra.Controls.Add(
            CrearPestana("Algoritmos", false, 108));

        fondo.Controls.Add(barra);

        return fondo;
    }

    private static Control CrearPestana(
        string texto, bool activa, int ancho)
    {
        var contenedor = new Panel
        {
            Size = new Size(ancho, 46),
            Margin = new Padding(0, 0, 8, 0),
            BackColor = TransRedTheme.Superficie
        };

        var etiqueta = new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            ForeColor = activa
                ? TransRedTheme.Acento
                : TransRedTheme.TextoSecundario,
            Font = new Font(
                "Segoe UI",
                9,
                activa ? FontStyle.Bold : FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleCenter
        };

        contenedor.Controls.Add(etiqueta);

        if (activa)
        {
            var linea = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 2,
                BackColor = TransRedTheme.Acento
            };

            contenedor.Controls.Add(linea);
            linea.BringToFront();
        }

        return contenedor;
    }

    private static Label CrearTexto(
        string texto,
        float tamano,
        FontStyle estilo,
        Color color)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            ForeColor = color,
            BackColor = Color.Transparent,
            Font = new Font(
                "Segoe UI", tamano, estilo),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
    }
}

// Símbolo del encabezado
internal class SimboloTransRed : Control
{
    public SimboloTransRed()
    {
        DoubleBuffered = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var fondo = new SolidBrush(
            TransRedTheme.Acento);

        using var blanco = new SolidBrush(Color.White);
        using var linea = new Pen(Color.White, 2.3f);

        g.FillRectangle(
            fondo, 0, 0, Width, Height);

        float centroY = Height / 2f;
        float x1 = Width * 0.27f;
        float x2 = Width * 0.73f;

        g.DrawLine(
            linea, x1, centroY, x2, centroY);

        g.FillEllipse(
            blanco, x1 - 4, centroY - 4, 8, 8);

        g.FillEllipse(
            blanco, x2 - 4, centroY - 4, 8, 8);
    }
}

// Estado vacío dibujado como un solo componente
internal class EstadoRedVacia : Control
{
    public EstadoRedVacia()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (Width <= 0 || Height <= 0)
            return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        float centroX = Width / 2f;
        float centroY = Height / 2f;

        // Posiciones relativas al centro del lienzo
        float iconoY = centroY - 57;

        using var linea = new Pen(
            Color.FromArgb(118, 171, 151), 2.3f);

        using var fondoNodo = new SolidBrush(
            Color.FromArgb(241, 246, 242));

        using var bordeNodo = new Pen(
            TransRedTheme.Acento, 2.3f);

        PointF[] nodos =
        {
            new PointF(centroX - 30, iconoY + 14),
            new PointF(centroX, iconoY - 8),
            new PointF(centroX + 30, iconoY + 14)
        };

        // Dibuja las tres conexiones
        g.DrawLine(linea, nodos[0], nodos[1]);
        g.DrawLine(linea, nodos[1], nodos[2]);
        g.DrawLine(linea, nodos[0], nodos[2]);

        // Dibuja los tres nodos
        foreach (PointF nodo in nodos)
        {
            g.FillEllipse(
                fondoNodo,
                nodo.X - 6,
                nodo.Y - 6,
                12, 12);

            g.DrawEllipse(
                bordeNodo,
                nodo.X - 6,
                nodo.Y - 6,
                12, 12);
        }

        // Textos centrados bajo el símbolo
        using var fuenteTitulo = new Font(
            "Segoe UI", 17, FontStyle.Bold);

        using var fuenteDetalle = new Font(
            "Segoe UI", 9.5f, FontStyle.Regular);

        var rectTitulo = new Rectangle(
            0,
            (int)(centroY - 17),
            Width,
            43
        );

        var rectDetalle = new Rectangle(
            0,
            (int)(centroY + 24),
            Width,
            32
        );

        TextRenderer.DrawText(
            g,
            "Tu red de transporte",
            fuenteTitulo,
            rectTitulo,
            TransRedTheme.Tinta,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );

        TextRenderer.DrawText(
            g,
            "Aquí se visualizarán las paradas y conexiones",
            fuenteDetalle,
            rectDetalle,
            TransRedTheme.TextoSecundario,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );
    }
}
