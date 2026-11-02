
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

public class LienzoRed : Control
{
    private readonly Color fondo =
        Color.FromArgb(241, 246, 242);

    private readonly Color cuadricula =
        Color.FromArgb(224, 234, 226);

    private readonly Color conexionNormal =
        Color.FromArgb(146, 180, 169);

    private readonly Color conexionDirigida =
        Color.FromArgb(186, 127, 77);

    // Datos visuales de ejemplo, no vienen del Core
    private readonly ParadaVisual[] paradas =
    {
        new("A", "Terminal", 0.12f, 0.50f),
        new("B", "Mercado", 0.37f, 0.17f),
        new("C", "Hospital", 0.43f, 0.72f),
        new("D", "Parque", 0.76f, 0.32f),
        new("E", "Campus", 0.86f, 0.79f)
    };

    // Bidireccional = true significa ambos sentidos
    private readonly ConexionVisual[] conexiones =
    {
        new(0, 1, true),
        new(0, 2, true),
        new(1, 2, true),
        new(1, 3, false),
        new(2, 3, true),
        new(2, 4, false),
        new(3, 4, true)
    };

    public LienzoRed()
    {
        Dock = DockStyle.Fill;
        BackColor = fondo;

        DoubleBuffered = true;
        ResizeRedraw = true;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true
        );
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        DibujarCuadricula(g);

        // Evita que el grafo se corte en ventanas pequeñas
        if (Width < 350 || Height < 280)
        {
            TextRenderer.DrawText(
                g,
                "Amplía la ventana para visualizar la red",
                Font,
                ClientRectangle,
                TransRedTheme.TextoSecundario,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.WordBreak
            );

            return;
        }

        DibujarInformacion(g);
        DibujarConexiones(g);
        DibujarParadas(g);
        DibujarLeyenda(g);
    }

    // Cuadrícula de fondo
    private void DibujarCuadricula(Graphics g)
    {
        const int separacion = 40;

        using Pen lapiz = new Pen(cuadricula, 1);

        for (int x = 0; x < Width; x += separacion)
            g.DrawLine(lapiz, x, 0, x, Height);

        for (int y = 0; y < Height; y += separacion)
            g.DrawLine(lapiz, 0, y, Width, y);
    }

    // Convierte posiciones relativas en píxeles
    private PointF ObtenerPosicion(ParadaVisual parada)
    {
        float margenX = Math.Min(115f, Width * 0.16f);
        float margenY = Math.Min(105f, Height * 0.18f);

        float ancho = Width - margenX * 2;
        float alto = Height - margenY * 2;

        return new PointF(
            margenX + parada.X * ancho,
            margenY + parada.Y * alto
        );
    }

    // Dibuja las conexiones antes que los nodos
    private void DibujarConexiones(Graphics g)
    {
        foreach (ConexionVisual conexion in conexiones)
        {
            PointF origen =
                ObtenerPosicion(paradas[conexion.Origen]);

            PointF destino =
                ObtenerPosicion(paradas[conexion.Destino]);

            Color color = conexion.Bidireccional
                ? conexionNormal
                : conexionDirigida;

            using Pen linea = new Pen(color, 2.6f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            g.DrawLine(linea, origen, destino);

            // Solo las rutas de un sentido llevan flecha
            if (!conexion.Bidireccional)
                DibujarFlecha(g, origen, destino, color);
        }
    }

    // Dibuja la dirección de una conexión
    private void DibujarFlecha(
        Graphics g,
        PointF origen,
        PointF destino,
        Color color)
    {
        float dx = destino.X - origen.X;
        float dy = destino.Y - origen.Y;

        float longitud = MathF.Sqrt(dx * dx + dy * dy);

        if (longitud < 1)
            return;

        float ux = dx / longitud;
        float uy = dy / longitud;

        // Posición de la flecha sobre la conexión
        float x = origen.X + dx * 0.65f;
        float y = origen.Y + dy * 0.65f;

        const float largo = 12;
        const float mitadAncho = 5;

        PointF punta = new PointF(x, y);

        PointF izquierda = new PointF(
            x - ux * largo - uy * mitadAncho,
            y - uy * largo + ux * mitadAncho
        );

        PointF derecha = new PointF(
            x - ux * largo + uy * mitadAncho,
            y - uy * largo - ux * mitadAncho
        );

        using Brush relleno = new SolidBrush(color);

        g.FillPolygon(
            relleno,
            new[] { punta, izquierda, derecha }
        );
    }

    // Dibuja cada parada y su nombre
    private void DibujarParadas(Graphics g)
    {
        using Font fuenteCodigo = new Font(
            "Segoe UI", 10, FontStyle.Bold);

        using Font fuenteNombre = new Font(
            "Segoe UI", 9, FontStyle.Regular);

        using Brush fondoNodo =
            new SolidBrush(Color.White);

        using Pen bordeNodo =
            new Pen(TransRedTheme.Acento, 2.5f);

        using Brush texto =
            new SolidBrush(TransRedTheme.Tinta);

        using Brush textoSecundario =
            new SolidBrush(TransRedTheme.TextoSecundario);

        using StringFormat centrado = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        foreach (ParadaVisual parada in paradas)
        {
            PointF punto = ObtenerPosicion(parada);

            const float radio = 19;

            g.FillEllipse(
                fondoNodo,
                punto.X - radio,
                punto.Y - radio,
                radio * 2,
                radio * 2
            );

            g.DrawEllipse(
                bordeNodo,
                punto.X - radio,
                punto.Y - radio,
                radio * 2,
                radio * 2
            );

            RectangleF areaCodigo = new RectangleF(
                punto.X - radio,
                punto.Y - radio,
                radio * 2,
                radio * 2
            );

            g.DrawString(
                parada.Codigo,
                fuenteCodigo,
                texto,
                areaCodigo,
                centrado
            );

            RectangleF areaNombre = new RectangleF(
                punto.X - 65,
                punto.Y + 23,
                130,
                25
            );

            g.DrawString(
                parada.Nombre,
                fuenteNombre,
                textoSecundario,
                areaNombre,
                centrado
            );
        }
    }

    // Encabezado informativo del lienzo
    private void DibujarInformacion(Graphics g)
    {
        using Font fuente = new Font(
            "Segoe UI", 8, FontStyle.Bold);

        using Brush tinta = new SolidBrush(
            TransRedTheme.TextoSecundario);

        g.DrawString(
            "RED DE DEMOSTRACIÓN  /  5 PARADAS · 7 CONEXIONES",
            fuente,
            tinta,
            new PointF(16, 16)
        );
    }

    // Explica el significado de las líneas
    private void DibujarLeyenda(Graphics g)
    {
        float y = Height - 29;

        using Pen normal = new Pen(conexionNormal, 3);
        using Pen dirigida = new Pen(conexionDirigida, 3);

        using Font fuente = new Font(
            "Segoe UI", 8, FontStyle.Regular);

        using Brush texto = new SolidBrush(
            TransRedTheme.TextoSecundario);

        g.DrawLine(normal, 16, y + 7, 43, y + 7);
        g.DrawString("Ambos sentidos", fuente, texto, 50, y);

        g.DrawLine(dirigida, 165, y + 7, 192, y + 7);
        g.DrawString("Un sentido", fuente, texto, 199, y);
    }

    // Modelo usado únicamente para dibujar
    private readonly record struct ParadaVisual(
        string Codigo,
        string Nombre,
        float X,
        float Y
    );

    // Relación entre dos paradas de demostración
    private readonly record struct ConexionVisual(
        int Origen,
        int Destino,
        bool Bidireccional
    );
}