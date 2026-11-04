
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
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

    private readonly IGestorTransporte gestor;

    private ParadaVisual[] paradas =
        Array.Empty<ParadaVisual>();

    private ConexionVisual[] conexiones =
        Array.Empty<ConexionVisual>();

    private int nodoHover = -1;
    private int nodoSeleccionado = -1;

    public LienzoRed(IGestorTransporte gestor)
    {
        this.gestor = gestor;

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

        ActualizarDatos();

        MouseMove += LienzoRed_MouseMove;
        MouseLeave += LienzoRed_MouseLeave;
        MouseClick += LienzoRed_MouseClick;
    }

    // Obtiene las paradas y conexiones reales del gestor.
    public void ActualizarDatos()
    {
        Parada[] paradasReales =
            gestor.ObtenerTodasParadas();

        Conexion[] conexionesReales =
            gestor.ObtenerTodasConexiones();

        paradas = new ParadaVisual[paradasReales.Length];

        nodoSeleccionado = -1;
        nodoHover = -1;

        if (paradasReales.Length == 0)
        {
            conexiones = Array.Empty<ConexionVisual>();
            Invalidate();
            return;
        }

        double latitudMinima = paradasReales[0].Latitud;
        double latitudMaxima = paradasReales[0].Latitud;
        double longitudMinima = paradasReales[0].Longitud;
        double longitudMaxima = paradasReales[0].Longitud;

        foreach (Parada parada in paradasReales)
        {
            latitudMinima = Math.Min(
                latitudMinima,
                parada.Latitud
            );

            latitudMaxima = Math.Max(
                latitudMaxima,
                parada.Latitud
            );

            longitudMinima = Math.Min(
                longitudMinima,
                parada.Longitud
            );

            longitudMaxima = Math.Max(
                longitudMaxima,
                parada.Longitud
            );
        }

        // Convierte las coordenadas geográficas en posiciones relativas.
        for (int i = 0; i < paradasReales.Length; i++)
        {
            Parada parada = paradasReales[i];

            float x = longitudMaxima == longitudMinima
                ? 0.5f
                : (float)(
                    (parada.Longitud - longitudMinima) /
                    (longitudMaxima - longitudMinima)
                );

            float y = latitudMaxima == latitudMinima
                ? 0.5f
                : 1f - (float)(
                    (parada.Latitud - latitudMinima) /
                    (latitudMaxima - latitudMinima)
                );

            x = 0.08f + x * 0.84f;
            y = 0.08f + y * 0.84f;

            paradas[i] = new ParadaVisual(
                parada.Codigo,
                parada.Nombre,
                x,
                y
            );
        }

        // Prepara las conexiones que se dibujarán.
        ConexionVisual[] resultado =
            new ConexionVisual[conexionesReales.Length];

        int total = 0;

        for (int i = 0; i < conexionesReales.Length; i++)
        {
            Conexion conexion = conexionesReales[i];

            int origen = BuscarIndiceParada(
                conexion.CodigoOrigen
            );

            int destino = BuscarIndiceParada(
                conexion.CodigoDestino
            );

            if (origen == -1 || destino == -1)
                continue;

            bool existeRegreso = false;

            for (int j = 0; j < conexionesReales.Length; j++)
            {
                if (
                    conexionesReales[j].CodigoOrigen ==
                    conexion.CodigoDestino &&
                    conexionesReales[j].CodigoDestino ==
                    conexion.CodigoOrigen
                )
                {
                    existeRegreso = true;
                    break;
                }
            }

            // Evita dibujar dos veces una conexión bidireccional.
            if (existeRegreso && origen > destino)
                continue;

            resultado[total++] = new ConexionVisual(
                origen,
                destino,
                existeRegreso
            );
        }

        Array.Resize(ref resultado, total);
        conexiones = resultado;

        Invalidate();
    }

    // Busca el índice de una parada mediante su código.
    private int BuscarIndiceParada(string codigo)
    {
        for (int i = 0; i < paradas.Length; i++)
        {
            if (paradas[i].Codigo == codigo)
                return i;
        }

        return -1;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        DibujarCuadricula(g);

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

        if (paradas.Length == 0)
        {
            TextRenderer.DrawText(
                g,
                "Todavía no hay paradas registradas",
                Font,
                ClientRectangle,
                TransRedTheme.TextoSecundario,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter
            );

            return;
        }

        DibujarInformacion(g);
        DibujarConexiones(g);
        DibujarParadas(g);
        DibujarLeyenda(g);
        DibujarInformacionNodo(g);
    }

    // Dibuja la cuadrícula de fondo.
    private void DibujarCuadricula(Graphics g)
    {
        const int separacion = 40;

        using Pen lapiz = new Pen(cuadricula, 1);

        for (int x = 0; x < Width; x += separacion)
            g.DrawLine(lapiz, x, 0, x, Height);

        for (int y = 0; y < Height; y += separacion)
            g.DrawLine(lapiz, 0, y, Width, y);
    }

    // Convierte las posiciones relativas en píxeles.
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

    // Busca si el cursor está sobre una parada.
    private int BuscarNodo(Point posicion)
    {
        for (int i = 0; i < paradas.Length; i++)
        {
            PointF punto = ObtenerPosicion(paradas[i]);

            float dx = posicion.X - punto.X;
            float dy = posicion.Y - punto.Y;

            float distanciaCuadrada = dx * dx + dy * dy;

            if (distanciaCuadrada <= 22 * 22)
                return i;
        }

        return -1;
    }

    private void LienzoRed_MouseMove(
        object? sender,
        MouseEventArgs e)
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

    private void LienzoRed_MouseLeave(
        object? sender,
        EventArgs e)
    {
        nodoHover = -1;
        Cursor = Cursors.Default;

        Invalidate();
    }

    private void LienzoRed_MouseClick(
        object? sender,
        MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        nodoSeleccionado = BuscarNodo(e.Location);

        Invalidate();
    }

    // Dibuja las conexiones antes de dibujar las paradas.
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

            if (!conexion.Bidireccional)
                DibujarFlecha(g, origen, destino, color);
        }
    }

    // Dibuja la dirección de una conexión.
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

    // Dibuja cada parada y su nombre.
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

    // Encabezado informativo del lienzo.
    private void DibujarInformacion(Graphics g)
    {
        using Font fuente = new Font(
            "Segoe UI", 8, FontStyle.Bold);

        using Brush tinta = new SolidBrush(
            TransRedTheme.TextoSecundario);

        g.DrawString(
            $"RED DE TRANSPORTE  /  {paradas.Length} PARADAS · {conexiones.Length} CONEXIONES",
            fuente,
            tinta,
            new PointF(16, 16)
        );
    }

    // Explica el significado de las líneas.
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

    // Muestra la información de la parada seleccionada.
    private void DibujarInformacionNodo(Graphics g)
    {
        if (nodoSeleccionado < 0 ||
            nodoSeleccionado >= paradas.Length)
            return;

        ParadaVisual parada = paradas[nodoSeleccionado];

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

        Rectangle tarjeta = new Rectangle(x, y, ancho, alto);

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
            "Información de la red",
            fuenteDetalle,
            secundario,
            x + 14,
            y + 89
        );
    }

    // Modelo visual de una parada.
    private readonly record struct ParadaVisual(
        string Codigo,
        string Nombre,
        float X,
        float Y
    );

    // Modelo visual de una conexión.
    private readonly record struct ConexionVisual(
        int Origen,
        int Destino,
        bool Bidireccional
    );
}
