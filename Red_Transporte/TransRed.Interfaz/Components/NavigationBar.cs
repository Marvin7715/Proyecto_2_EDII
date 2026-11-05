
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Menú lateral principal de TransRed
public class NavigationBar : UserControl
{
    private readonly Action<string> cambiarVista;
    private readonly FlowLayoutPanel lista;

    public NavigationBar(Action<string> alCambiar)
    {
        cambiarVista = alCambiar;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.SuperficieAlterna;
        Padding = new Padding(13, 18, 10, 15);

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
            new RowStyle(SizeType.Absolute, 34));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenedor.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 70));

        var etiqueta = new Label
        {
            Text = "ESPACIO DE TRABAJO",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            ForeColor = TransRedTheme.TextoTenue,
            Font = new Font(
                "Segoe UI", 8, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = false
        };

        lista = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = TransRedTheme.SuperficieAlterna
        };

        // Cuatro secciones, sin duplicar la planificación
        Agregar("Explorar red", "◎");
        Agregar("Paradas", "⌖");
        Agregar("Conexiones", "◇");
        Agregar("Algoritmos", "≡");

        lista.Resize += (_, _) => AjustarElementos();

        var pie = new Label
        {
            Text = "TRANSRED  /  2026\nMovilidad del suroccidente",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
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
            Width = 150,
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

    private void AjustarElementos()
    {
        int ancho = Math.Max(
            100, lista.ClientSize.Width - 3);

        foreach (Control control in lista.Controls)
        {
            control.Width = ancho;
        }
    }

    public void Seleccionar(string nombre)
    {
        foreach (Control control in lista.Controls)
        {
            if (control is ElementoNavegacion item)
            {
                item.Activo = item.Nombre == nombre;
            }
        }
    }
}

// Elemento con selección, hover y teclado
internal sealed class ElementoNavegacion : Control
{
    private readonly string nombre;
    private readonly string icono;

    private bool activo;
    private bool encima;

    public string Nombre => nombre;

    [DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)]
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

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
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
            e.SuppressKeyPress = true;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        g.Clear(BackColor);

        var area = new Rectangle(
            1, 1,
            Math.Max(1, Width - 3),
            Math.Max(1, Height - 3)
        );

        using var forma = CrearForma(area, 12);

        if (activo)
        {
            using var fondo = new LinearGradientBrush(
                area,
                TransRedTheme.Acento,
                TransRedTheme.Bosque,
                LinearGradientMode.Horizontal
            );

            g.FillPath(fondo, forma);
        }
        else if (encima || Focused)
        {
            using var fondo = new SolidBrush(
                TransRedTheme.AcentoSuave);

            g.FillPath(fondo, forma);
        }

        Color tinta = activo
            ? Color.White
            : TransRedTheme.Tinta;

        using var fuenteIcono = new Font(
            "Segoe UI Symbol", 15);

        using var fuenteTexto = new Font(
            "Segoe UI",
            9.3f,
            activo
                ? FontStyle.Bold
                : FontStyle.Regular
        );

        TextRenderer.DrawText(
            g,
            icono,
            fuenteIcono,
            new Rectangle(10, 0, 28, Height),
            tinta,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.NoPadding
        );

        TextRenderer.DrawText(
            g,
            nombre,
            fuenteTexto,
            new Rectangle(
                43, 0,
                Math.Max(1, Width - 49),
                Height
            ),
            tinta,
            TextFormatFlags.Left |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );
    }

    private static GraphicsPath CrearForma(
        Rectangle rectangulo, int radio)
    {
        var camino = new GraphicsPath();

        int diametro = radio * 2;

        camino.AddArc(
            rectangulo.Left,
            rectangulo.Top,
            diametro, diametro,
            180, 90);

        camino.AddArc(
            rectangulo.Right - diametro,
            rectangulo.Top,
            diametro, diametro,
            270, 90);

        camino.AddArc(
            rectangulo.Right - diametro,
            rectangulo.Bottom - diametro,
            diametro, diametro,
            0, 90);

        camino.AddArc(
            rectangulo.Left,
            rectangulo.Bottom - diametro,
            diametro, diametro,
            90, 90);

        camino.CloseFigure();

        return camino;
    }
}
