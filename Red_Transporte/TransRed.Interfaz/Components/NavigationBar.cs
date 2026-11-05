using System.ComponentModel;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Navegación vertical de Mobility Studio
public class NavigationBar : UserControl
{
    private readonly Action<string> cambiarVista;
    private readonly FlowLayoutPanel lista;

    public NavigationBar(Action<string> alCambiar)
    {
        cambiarVista = alCambiar;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.SuperficieAlterna;
        Padding = new Padding(14, 18, 12, 16);

        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.SuperficieAlterna
        };

        contenedor.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 32));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 68));

        var etiqueta = new Label
        {
            Text = "ESPACIO DE TRABAJO",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = TransRedTheme.TextoTenue,
            TextAlign = ContentAlignment.MiddleLeft
        };

        lista = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = false,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.SuperficieAlterna
        };

        Agregar("Explorar red", "◎");
        Agregar("Paradas", "⌖");
        Agregar("Conexiones", "◇");
        Agregar("Planificar", "↗");
        Agregar("Algoritmos", "≡");

        lista.Resize += (_, _) =>
        {
            foreach (Control control in lista.Controls)
                control.Width = Math.Max(80, lista.ClientSize.Width - 4);
        };

        var pie = new Label
        {
            Text = "TRANSRED  /  2026\nMovilidad del suroccidente",
            Dock = DockStyle.Fill,
            ForeColor = TransRedTheme.TextoTenue,
            Font = new Font("Segoe UI", 8.5f),
            TextAlign = ContentAlignment.BottomLeft
        };

        contenedor.Controls.Add(etiqueta, 0, 0);
        contenedor.Controls.Add(lista, 0, 1);
        contenedor.Controls.Add(pie, 0, 2);

        Controls.Add(contenedor);

        Seleccionar("Explorar red");
    }

    private void Agregar(string nombre, string icono)
    {
        var item = new ElementoNavegacion(
            nombre, icono)
        {
            Width = 174,
            Height = 48,
            Margin = new Padding(0, 0, 0, 8)
        };

        item.Click += (_, _) =>
        {
            Seleccionar(nombre);
            cambiarVista(nombre);
        };

        lista.Controls.Add(item);
    }

    public void Seleccionar(string nombre)
    {
        foreach (Control control in lista.Controls)
        {
            if (control is ElementoNavegacion item)
            {
                item.Activo = item.Nombre == nombre;
                item.Invalidate();
            }
        }
    }
}

// Elemento dibujado con estados normal, hover y activo
internal sealed class ElementoNavegacion : Control
{
    private readonly string nombre;
    private readonly string icono;
    private bool activo;
    private bool encima;

    public string Nombre => nombre;

    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Activo
    {
        get => activo;
        set
        {
            activo = value;
            Invalidate();
        }
    }


    public ElementoNavegacion(
        string nombre, string icono)
    {
        this.nombre = nombre;
        this.icono = icono;

        Cursor = Cursors.Hand;
        DoubleBuffered = true;
        BackColor = TransRedTheme.SuperficieAlterna;
        TabStop = true;

        AccessibleName = nombre;
        AccessibleRole = AccessibleRole.PushButton;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        encima = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        encima = false;
        Invalidate();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.Enter ||
            e.KeyCode == Keys.Space)
        {
            OnClick(EventArgs.Empty);
            e.Handled = true;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        Rectangle area = new Rectangle(
            1, 1,
            Math.Max(1, Width - 3),
            Math.Max(1, Height - 3)
        );

        if (activo)
        {
            using var fondo = new LinearGradientBrush(
                area,
                TransRedTheme.Acento,
                TransRedTheme.Bosque,
                LinearGradientMode.Horizontal
            );

            using var forma = FormaRedondeada(area, 13);
            g.FillPath(fondo, forma);
        }
        else if (encima || Focused)
        {
            using var fondo = new SolidBrush(
                TransRedTheme.AcentoSuave);

            using var forma = FormaRedondeada(area, 13);
            g.FillPath(fondo, forma);
        }

        Color color = activo
            ? Color.White
            : TransRedTheme.Tinta;

        using var fuenteIcono =
            new Font("Segoe UI Symbol", 16);

        using var fuenteTexto = new Font(
            "Segoe UI",
            9.5f,
            activo ? FontStyle.Bold : FontStyle.Regular
        );

        TextRenderer.DrawText(
            g,
            icono,
            fuenteIcono,
            new Rectangle(13, 0, 29, Height),
            color,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter
        );

        TextRenderer.DrawText(
            g,
            nombre,
            fuenteTexto,
            new Rectangle(
                50, 0,
                Math.Max(1, Width - 55),
                Height
            ),
            color,
            TextFormatFlags.Left |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );
    }

    private static GraphicsPath FormaRedondeada(
        Rectangle r, int radio)
    {
        var p = new GraphicsPath();
        int d = radio * 2;

        p.AddArc(r.Left, r.Top, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();

        return p;
    }
}
