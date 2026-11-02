using System.ComponentModel;
using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Presentación inicial de una sección
public class VistaSeccionVacia : Control
{
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string TituloSeccion { get; set; } = "";

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Descripcion { get; set; } = "";

    public VistaSeccionVacia()
    {
        Dock = DockStyle.Fill;
        DoubleBuffered = true;
        ResizeRedraw = true;

        BackColor = Color.FromArgb(241, 246, 242);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        int centroY = Height / 2;

        using var fuenteTitulo = new Font(
            "Segoe UI", 18, FontStyle.Bold);

        using var fuenteDescripcion = new Font(
            "Segoe UI", 9.5f, FontStyle.Regular);

        var areaTitulo = new Rectangle(
            15,
            centroY - 48,
            Math.Max(0, Width - 30),
            55
        );

        var areaDescripcion = new Rectangle(
            15,
            centroY + 10,
            Math.Max(0, Width - 30),
            58
        );

        TextRenderer.DrawText(
            e.Graphics,
            TituloSeccion,
            fuenteTitulo,
            areaTitulo,
            TransRedTheme.Tinta,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );

        TextRenderer.DrawText(
            e.Graphics,
            Descripcion,
            fuenteDescripcion,
            areaDescripcion,
            TransRedTheme.TextoSecundario,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.WordBreak
        );
    }
}
