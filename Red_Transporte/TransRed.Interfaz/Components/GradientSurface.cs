
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TransRed.Interfaz.Components;

// Panel con degradado y esquinas redondeadas
public class GradientSurface : Panel
{
    private readonly Color inicio;
    private readonly Color final;
    private readonly int radio;

    public GradientSurface(
        Color colorInicio,
        Color colorFinal,
        int radioEsquinas = 20)
    {
        inicio = colorInicio;
        final = colorFinal;
        radio = Math.Max(0, radioEsquinas);

        DoubleBuffered = true;
        ResizeRedraw = true;
        Margin = Padding.Empty;

        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true
        );
    }

    // Obtiene un fondo opaco para evitar esquinas negras
    private Color ObtenerFondo()
    {
        Control? actual = Parent;

        while (actual != null)
        {
            Color color = actual.BackColor;

            if (color.A == 255)
                return color;

            actual = actual.Parent;
        }

        return Color.FromArgb(246, 244, 238);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var fondo = new SolidBrush(ObtenerFondo());

        g.FillRectangle(
            fondo,
            0, 0, Width, Height
        );

        if (Width <= 1 || Height <= 1)
            return;

        Rectangle area = new Rectangle(
            0, 0, Width - 1, Height - 1
        );

        if (radio == 0)
        {
            using var degradado = new LinearGradientBrush(
                area,
                inicio,
                final,
                LinearGradientMode.Horizontal
            );

            g.FillRectangle(degradado, area);
            return;
        }

        using GraphicsPath forma =
            CrearForma(area, radio);

        using var pincel = new LinearGradientBrush(
            area,
            inicio,
            final,
            LinearGradientMode.ForwardDiagonal
        );

        g.FillPath(pincel, forma);
    }

    private static GraphicsPath CrearForma(
        Rectangle area, int radio)
    {
        GraphicsPath camino = new GraphicsPath();

        int diametro = Math.Min(
            radio * 2,
            Math.Min(area.Width, area.Height)
        );

        if (diametro < 2)
        {
            camino.AddRectangle(area);
            return camino;
        }

        camino.AddArc(
            area.Left,
            area.Top,
            diametro,
            diametro,
            180, 90
        );

        camino.AddArc(
            area.Right - diametro,
            area.Top,
            diametro,
            diametro,
            270, 90
        );

        camino.AddArc(
            area.Right - diametro,
            area.Bottom - diametro,
            diametro,
            diametro,
            0, 90
        );

        camino.AddArc(
            area.Left,
            area.Bottom - diametro,
            diametro,
            diametro,
            90, 90
        );

        camino.CloseFigure();
        return camino;
    }
}
