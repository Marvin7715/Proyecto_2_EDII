
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

    // Guarda el nodo que tiene el cursor encima
    private int nodoHover = -1;

    // Guarda el nodo seleccionado con un clic
    private int nodoSeleccionado = -1;
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

        // Detecta el movimiento del cursor
        MouseMove += LienzoRed_MouseMove;

        // Detecta cuando el cursor sale del lienzo
        MouseLeave += LienzoRed_MouseLeave;

        // Detecta la selección de una parada
        MouseClick += LienzoRed_MouseClick;
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
        DibujarInformacionNodo(g);
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
    
    private int BuscarNodo(Point posicion)
    {
        for (int i = 0; i < paradas.Length; i++)
        {
            PointF punto = ObtenerPosicion(paradas[i]);

            float dx = posicion.X - punto.X;
            float dy = posicion.Y - punto.Y;

            float distanciaCuadrada = dx * dx + dy * dy;

            // Verifica si el cursor está dentro del nodo
            if (distanciaCuadrada <= 22 * 22)
            {
                return i;
            }
        }

        return -1;
    }

    private void LienzoRed_MouseMove(object? sender, MouseEventArgs e)
    {
        int nodoActual = BuscarNodo(e.Location);

        if (nodoHover != nodoActual)
        {
            nodoHover = nodoActual;

            Cursor = nodoHover >= 0
                ? Cursors.Hand
                : Cursors.Default;

            Invalidate();
        }
    }

    private void LienzoRed_MouseLeave(object? sender, EventArgs e)
    {
        nodoHover = -1;
        Cursor = Cursors.Default;

        Invalidate();
    }

    private void LienzoRed_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        int nodo = BuscarNodo(e.Location);

        // Clic fuera de los nodos limpia la selección
        nodoSeleccionado = nodo;

        Invalidate();
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

        using Brush fondoNormal =
            new SolidBrush(Color.White);

        using Brush fondoSeleccionado =
            new SolidBrush(TransRedTheme.Acento);

        using Brush textoNormal =
            new SolidBrush(TransRedTheme.Tinta);

        using Brush textoBlanco =
            new SolidBrush(Color.White);

        using Brush textoSecundario =
            new SolidBrush(TransRedTheme.TextoSecundario);

        using StringFormat centrado = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        for (int i = 0; i < paradas.Length; i++)
        {
            ParadaVisual parada = paradas[i];
            PointF punto = ObtenerPosicion(parada);

            bool seleccionada = i == nodoSeleccionado;
            bool encima = i == nodoHover;

            float radio = seleccionada ? 23 : encima ? 21 : 19;

            Color colorBorde = seleccionada || encima
                ? TransRedTheme.Acento
                : Color.FromArgb(104, 153, 139);

            using Pen borde = new Pen(
                colorBorde,
                seleccionada ? 3.5f : 2.5f
            );

            Brush fondoNodo = seleccionada
                ? fondoSeleccionado
                : fondoNormal;

            g.FillEllipse(
                fondoNodo,
                punto.X - radio,
                punto.Y - radio,
                radio * 2,
                radio * 2
            );

            g.DrawEllipse(
                borde,
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
                seleccionada ? textoBlanco : textoNormal,
                areaCodigo,
                centrado
            );

            RectangleF areaNombre = new RectangleF(
                punto.X - 65,
                punto.Y + 27,
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

    private void DibujarInformacionNodo(Graphics g)
    {
        if (nodoSeleccionado < 0)
            return;

        ParadaVisual parada = paradas[nodoSeleccionado];

        // Cuenta las conexiones asociadas a la parada
        int cantidadConexiones = 0;

        foreach (ConexionVisual conexion in conexiones)
        {
            if (conexion.Origen == nodoSeleccionado ||
                conexion.Destino == nodoSeleccionado)
            {
                cantidadConexiones++;
            }
        }

        int ancho = Math.Min(250, Width - 32);
        int alto = 112;

        int x = Width - ancho - 16;
        int y = 44;

        Rectangle tarjeta = new Rectangle(
            x, y, ancho, alto
        );

        using Brush fondoTarjeta =
            new SolidBrush(Color.White);

        using Pen borde =
            new Pen(Color.FromArgb(213, 228, 219));

        g.FillRectangle(fondoTarjeta, tarjeta);
        g.DrawRectangle(borde, tarjeta);

        using Font fuenteEtiqueta =
            new Font("Segoe UI", 8, FontStyle.Bold);

        using Font fuenteTitulo =
            new Font("Segoe UI", 12, FontStyle.Bold);

        using Font fuenteDetalle =
            new Font("Segoe UI", 9);

        using Brush verde =
            new SolidBrush(TransRedTheme.Acento);

        using Brush tinta =
            new SolidBrush(TransRedTheme.Tinta);

        using Brush secundario =
            new SolidBrush(TransRedTheme.TextoSecundario);

        g.DrawString(
            "PARADA SELECCIONADA",
            fuenteEtiqueta,
            verde,
            x + 14,
            y + 12
        );

        g.DrawString(
            $"{parada.Codigo} · {parada.Nombre}",
            fuenteTitulo,
            tinta,
            x + 14,
            y + 36
        );

        g.DrawString(
            $"Conexiones asociadas: {cantidadConexiones}",
            fuenteDetalle,
            secundario,
            x + 14,
            y + 70
        );

        g.DrawString(
            "Datos de demostración",
            fuenteDetalle,
            secundario,
            x + 14,
            y + 89
        );
    }

}