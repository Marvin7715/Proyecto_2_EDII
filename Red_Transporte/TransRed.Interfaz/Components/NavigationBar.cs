using System.ComponentModel;
using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Barra de navegación principal
public class NavigationBar : UserControl
{
    private readonly FlowLayoutPanel barra;
    private readonly Action<string> cambiarVista;

    public NavigationBar(Action<string> alCambiar)
    {
        cambiarVista = alCambiar;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.Superficie;
        Padding = new Padding(18, 4, 18, 4);

        barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = false,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = TransRedTheme.Superficie
        };

        Controls.Add(barra);

        AgregarPestana("Explorar red", 130);
        AgregarPestana("Paradas", 94);
        AgregarPestana("Conexiones", 116);
        AgregarPestana("Planificar", 104);
        AgregarPestana("Algoritmos", 108);

        Seleccionar("Explorar red");
    }

    private void AgregarPestana(string nombre, int ancho)
    {
        var pestana = new PestanaNavegacion
        {
            Nombre = nombre,
            Width = ancho,
            Height = 46,
            Margin = new Padding(0, 0, 8, 0)
        };

        pestana.Click += (sender, e) =>
        {
            Seleccionar(nombre);
            cambiarVista(nombre);
        };

        barra.Controls.Add(pestana);
    }

    // Actualiza el indicador de la sección seleccionada
    private void Seleccionar(string nombre)
    {
        foreach (Control control in barra.Controls)
        {
            if (control is PestanaNavegacion pestana)
            {
                pestana.Activa = pestana.Nombre == nombre;
                pestana.Invalidate();
            }
        }
    }
}

// Pestaña con estados visuales personalizados
internal class PestanaNavegacion : Control
{
    private bool encima;
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Nombre { get; set; } = "";

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Activa { get; set; }

    public PestanaNavegacion()
    {
        Cursor = Cursors.Hand;
        DoubleBuffered = true;
        BackColor = TransRedTheme.Superficie;
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

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Color texto = Activa
            ? TransRedTheme.Acento
            : encima
                ? TransRedTheme.Tinta
                : TransRedTheme.TextoSecundario;

        using var fuente = new Font(
            "Segoe UI",
            9,
            Activa ? FontStyle.Bold : FontStyle.Regular
        );

        var rectangulo = new Rectangle(
            0, 0, Width, Height - 3
        );

        TextRenderer.DrawText(
            e.Graphics,
            Nombre,
            fuente,
            rectangulo,
            texto,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.SingleLine
        );

        // Línea que identifica la sección activa
        if (Activa)
        {
            using var pincel = new SolidBrush(
                TransRedTheme.Acento
            );

            e.Graphics.FillRectangle(
                pincel,
                0,
                Height - 3,
                Width,
                2
            );
        }
    }
}