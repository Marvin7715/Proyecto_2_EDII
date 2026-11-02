
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Superficie gráfica para la red de transporte
public class LienzoRed : Control
{
    private readonly Color fondo =
        Color.FromArgb(241, 246, 242);

    private readonly Color cuadricula =
        Color.FromArgb(224, 234, 226);

    public LienzoRed()
    {
        Dock = DockStyle.Fill;
        BackColor = fondo;

        DoubleBuffered = true;
        ResizeRedraw = true;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true
        );
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        DibujarCuadricula(g);
        DibujarEstadoVacio(g);
    }

    // Dibuja una cuadrícula discreta
    private void DibujarCuadricula(Graphics g)
    {
        int separacion = 40;

        using Pen lapiz = new Pen(cuadricula, 1);

        for (int x = 0; x < Width; x += separacion)
        {
            g.DrawLine(lapiz, x, 0, x, Height);
        }

        for (int y = 0; y < Height; y += separacion)
        {
            g.DrawLine(lapiz, 0, y, Width, y);
        }
    }

    // Mensaje mostrado mientras no exista una red
    private void DibujarEstadoVacio(Graphics g)
    {
        int centroX = Width / 2;
        int centroY = Height / 2;

        DibujarSimbolo(g, centroX, centroY - 57);

        using Font titulo = new Font(
            "Segoe UI", 17, FontStyle.Bold
        );

        using Font descripcion = new Font(
            "Segoe UI", 9.5f
        );

        Rectangle areaTitulo = new Rectangle(
            0, centroY - 20, Width, 45
        );

        Rectangle areaDescripcion = new Rectangle(
            0, centroY + 23, Width, 36
        );

        TextRenderer.DrawText(
            g,
            "Tu red de transporte",
            titulo,
            areaTitulo,
            TransRedTheme.Tinta,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );

        TextRenderer.DrawText(
            g,
            "Aquí se mostrarán las paradas y conexiones",
            descripcion,
            areaDescripcion,
            TransRedTheme.TextoSecundario,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );
    }

    // Símbolo sencillo de tres nodos conectados
    private void DibujarSimbolo(
        Graphics g, int centroX, int centroY)
    {
        PointF[] puntos =
        {
            new PointF(centroX - 28, centroY + 13),
            new PointF(centroX, centroY - 9),
            new PointF(centroX + 28, centroY + 13)
        };

        using Pen linea = new Pen(
            TransRedTheme.Acento, 2
        );

        using Brush relleno = new SolidBrush(fondo);

        g.DrawLine(linea, puntos[0], puntos[1]);
        g.DrawLine(linea, puntos[1], puntos[2]);
        g.DrawLine(linea, puntos[0], puntos[2]);

        foreach (PointF punto in puntos)
        {
            g.FillEllipse(
                relleno,
                punto.X - 6,
                punto.Y - 6,
                12, 12
            );

            g.DrawEllipse(
                linea,
                punto.X - 6,
                punto.Y - 6,
                12, 12
            );
        }
    }
}
