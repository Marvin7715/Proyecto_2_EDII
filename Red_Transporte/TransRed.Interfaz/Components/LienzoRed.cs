
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Mapa interactivo con recorridos animados
public class LienzoRed : Control
{
    private readonly IGestorTransporte gestor;

    private ParadaVisual[] paradas = Array.Empty<ParadaVisual>();
    private ConexionVisual[] conexiones = Array.Empty<ConexionVisual>();

    private string[] ruta = Array.Empty<string>();

    private readonly System.Windows.Forms.Timer animador = new();

    private float progresoRuta = 1f;
    private float zoom = 1f;
    private PointF desplazamiento = PointF.Empty;

    private int seleccionado = -1;
    private int hover = -1;

    private bool arrastrando;
    private Point ultimoMouse;

    private readonly Color fondo = Color.FromArgb(237, 244, 235);
    private readonly Color lineaSuave = Color.FromArgb(165, 190, 172);

    public LienzoRed(IGestorTransporte gestor)
    {
        this.gestor = gestor;

        Dock = DockStyle.Fill;
        BackColor = fondo;
        DoubleBuffered = true;
        ResizeRedraw = true;
        TabStop = true;

        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true
        );

        animador.Interval = 20;
        animador.Tick += (_, _) =>
        {
            progresoRuta = Math.Min(1f, progresoRuta + 0.018f);
            Invalidate();

            if (progresoRuta >= 1f)
                animador.Stop();
        };

        ActualizarDatos();
        CrearControles();
    }

    private void CrearControles()
    {
        var controles = new FlowLayoutPanel
        {
            Width = 132,
            Height = 39,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BackColor = Color.Transparent,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = Padding.Empty
        };

        Button acercar = CrearBoton("+");
        Button alejar = CrearBoton("−");
        Button restaurar = CrearBoton("⌂");

        acercar.Click += (_, _) => CambiarZoom(1.2f);
        alejar.Click += (_, _) => CambiarZoom(1f / 1.2f);
        restaurar.Click += (_, _) => RestablecerVista();

        controles.Controls.Add(acercar);
        controles.Controls.Add(alejar);
        controles.Controls.Add(restaurar);

        Controls.Add(controles);

        void Ajustar()
        {
            controles.Location = new Point(
                Math.Max(8, Width - controles.Width - 12), 12
            );

            controles.BringToFront();
        }

        Resize += (_, _) => Ajustar();
        Ajustar();
    }

    private static Button CrearBoton(string simbolo)
    {
        var boton = new Button
        {
            Text = simbolo,
            Width = 38,
            Height = 35,
            Margin = new Padding(2, 0, 2, 0),
            BackColor = Color.White,
            ForeColor = TransRedTheme.Bosque,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };

        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.BorderColor = TransRedTheme.Borde;
        boton.FlatAppearance.MouseOverBackColor =
            TransRedTheme.AcentoSuave;

        return boton;
    }

    public void ActualizarDatos()
    {
        Parada[] datos = gestor.ObtenerTodasParadas();
        Conexion[] rutas = gestor.ObtenerTodasConexiones();

        paradas = Array.Empty<ParadaVisual>();
        conexiones = Array.Empty<ConexionVisual>();

        seleccionado = -1;
        hover = -1;
        ruta = Array.Empty<string>();
        animador.Stop();
        progresoRuta = 1f;

        if (datos.Length == 0)
        {
            Invalidate();
            return;
        }

        double latMin = datos.Min(p => p.Latitud);
        double latMax = datos.Max(p => p.Latitud);
        double lonMin = datos.Min(p => p.Longitud);
        double lonMax = datos.Max(p => p.Longitud);

        paradas = datos.Select(p =>
        {
            float x = lonMax == lonMin
                ? 0.5f
                : (float)((p.Longitud - lonMin) / (lonMax - lonMin));

            float y = latMax == latMin
                ? 0.5f
                : 1f - (float)((p.Latitud - latMin) / (latMax - latMin));

            return new ParadaVisual(
                p.Codigo,
                p.Nombre,
                0.08f + x * 0.84f,
                0.10f + y * 0.80f
            );
        }).ToArray();

        var lista = new List<ConexionVisual>();
        var pares = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Conexion conexion in rutas)
        {
            int a = BuscarIndice(conexion.CodigoOrigen);
            int b = BuscarIndice(conexion.CodigoDestino);

            if (a < 0 || b < 0)
                continue;

            bool inversa = rutas.Any(r =>
                r.CodigoOrigen == conexion.CodigoDestino &&
                r.CodigoDestino == conexion.CodigoOrigen
            );

            string clave = inversa
                ? $"{Math.Min(a, b)}:{Math.Max(a, b)}"
                : $"{a}:{b}";

            if (!pares.Add(clave))
                continue;

            lista.Add(new ConexionVisual(a, b, inversa));
        }

        conexiones = lista.ToArray();
        RestablecerVista();
    }

    public void MostrarRuta(string[] codigos)
    {
        animador.Stop();

        ruta = codigos?.ToArray() ?? Array.Empty<string>();
        progresoRuta = ruta.Length > 1 ? 0f : 1f;

        seleccionado = -1;

        if (ruta.Length > 1)
        {
            EnfocarRuta();
            animador.Start();
        }
        else
        {
            Invalidate();
        }
    }

    public void RestablecerVista()
    {
        zoom = 1f;
        desplazamiento = PointF.Empty;
        Invalidate();
    }

    private int BuscarIndice(string codigo)
    {
        for (int i = 0; i < paradas.Length; i++)
        {
            if (string.Equals(
                paradas[i].Codigo,
                codigo,
                StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }

    private PointF PosicionBase(int indice)
    {
        float margen = 44f;
        float ancho = Math.Max(1, Width - margen * 2);
        float alto = Math.Max(1, Height - margen * 2);

        return new PointF(
            margen + paradas[indice].X * ancho,
            margen + paradas[indice].Y * alto
        );
    }

    private PointF Posicion(int indice)
    {
        PointF p = PosicionBase(indice);

        return new PointF(
            Width / 2f + (p.X - Width / 2f) * zoom +
                desplazamiento.X,
            Height / 2f + (p.Y - Height / 2f) * zoom +
                desplazamiento.Y
        );
    }

    private void EnfocarRuta()
    {
        var indices = ruta.Select(BuscarIndice)
            .Where(i => i >= 0)
            .Distinct()
            .ToArray();

        if (indices.Length < 2 || Width < 200 || Height < 200)
        {
            Invalidate();
            return;
        }

        PointF[] puntos = indices.Select(PosicionBase).ToArray();

        float minX = puntos.Min(p => p.X);
        float maxX = puntos.Max(p => p.X);
        float minY = puntos.Min(p => p.Y);
        float maxY = puntos.Max(p => p.Y);

        float ancho = Math.Max(100f, maxX - minX);
        float alto = Math.Max(100f, maxY - minY);

        float disponibleX = Math.Max(100, Width - 190);
        float disponibleY = Math.Max(100, Height - 175);

        zoom = Math.Clamp(
            Math.Min(disponibleX / ancho, disponibleY / alto),
            0.85f,
            2.4f
        );

        float centroX = (minX + maxX) / 2f;
        float centroY = (minY + maxY) / 2f;

        desplazamiento = new PointF(
            -(centroX - Width / 2f) * zoom,
            -(centroY - Height / 2f) * zoom
        );

        Invalidate();
    }

    private void CambiarZoom(float factor)
    {
        zoom = Math.Clamp(zoom * factor, 0.7f, 5f);
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        float anterior = zoom;

        zoom = Math.Clamp(
            zoom * (e.Delta > 0 ? 1.15f : 0.87f),
            0.7f,
            5f
        );

        float cx = Width / 2f;
        float cy = Height / 2f;

        desplazamiento = new PointF(
            e.X - cx - (e.X - cx - desplazamiento.X) *
                zoom / anterior,
            e.Y - cy - (e.Y - cy - desplazamiento.Y) *
                zoom / anterior
        );

        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();

        if (e.Button == MouseButtons.Right ||
            e.Button == MouseButtons.Middle)
        {
            arrastrando = true;
            ultimoMouse = e.Location;
            Cursor = Cursors.SizeAll;
            Capture = true;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (arrastrando)
        {
            arrastrando = false;
            Capture = false;
            Cursor = Cursors.Default;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (arrastrando)
        {
            desplazamiento = new PointF(
                desplazamiento.X + e.X - ultimoMouse.X,
                desplazamiento.Y + e.Y - ultimoMouse.Y
            );

            ultimoMouse = e.Location;
            Invalidate();
            return;
        }

        int actual = BuscarNodo(e.Location);

        if (hover != actual)
        {
            hover = actual;
            Cursor = hover >= 0 ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        if (!arrastrando)
        {
            hover = -1;
            Cursor = Cursors.Default;
            Invalidate();
        }
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        if (e.Button == MouseButtons.Left)
        {
            seleccionado = BuscarNodo(e.Location);
            Invalidate();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.D0 ||
            e.KeyCode == Keys.NumPad0)
        {
            RestablecerVista();
            e.Handled = true;
        }
    }

    private int BuscarNodo(Point p)
    {
        for (int i = paradas.Length - 1; i >= 0; i--)
        {
            PointF posicion = Posicion(i);

            float dx = p.X - posicion.X;
            float dy = p.Y - posicion.Y;

            if (dx * dx + dy * dy <= 14 * 14)
                return i;
        }

        return -1;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(fondo);

        DibujarFondo(g);

        if (Width < 200 || Height < 180)
            return;

        if (paradas.Length == 0)
        {
            TextRenderer.DrawText(
                g, "No hay paradas registradas",
                Font, ClientRectangle,
                TransRedTheme.TextoSecundario,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter
            );
            return;
        }

        DibujarConexiones(g);
        DibujarRuta(g);
        DibujarNodos(g);
        DibujarEtiquetas(g);
        DibujarIndicadores(g);
        DibujarSeleccion(g);
    }

    private void DibujarFondo(Graphics g)
    {
        using var puntos = new SolidBrush(
            Color.FromArgb(210, 226, 212));

        for (int x = 16; x < Width; x += 30)
        {
            for (int y = 16; y < Height; y += 30)
            {
                g.FillEllipse(puntos, x, y, 1.5f, 1.5f);
            }
        }
    }

    // Curvas visuales: no representan carreteras reales
    private GraphicsPath CrearCurva(int a, int b, int semilla)
    {
        PointF origen = Posicion(a);
        PointF destino = Posicion(b);

        float dx = destino.X - origen.X;
        float dy = destino.Y - origen.Y;

        float distancia = MathF.Sqrt(dx * dx + dy * dy);

        var curva = new GraphicsPath();

        if (distancia < 2f)
            return curva;

        float curvatura = Math.Min(20f, distancia * 0.075f);
        float lado = semilla % 2 == 0 ? 1f : -1f;

        PointF control = new PointF(
            (origen.X + destino.X) / 2f -
                dy / distancia * curvatura * lado,
            (origen.Y + destino.Y) / 2f +
                dx / distancia * curvatura * lado
        );

        curva.AddBezier(origen, control, control, destino);

        return curva;
    }

    private void DibujarConexiones(Graphics g)
    {
        for (int i = 0; i < conexiones.Length; i++)
        {
            ConexionVisual conexion = conexiones[i];

            using GraphicsPath curva =
                CrearCurva(conexion.Origen, conexion.Destino, i);

            if (curva.PointCount == 0)
                continue;

            Color color = conexion.Bidireccional
                ? lineaSuave
                : TransRedTheme.Cobre;

            using var lapiz = new Pen(
                Color.FromArgb(ruta.Length > 1 ? 55 : 95, color),
                1.4f
            );

            g.DrawPath(lapiz, curva);
        }
    }

    private void DibujarRuta(Graphics g)
    {
        if (ruta.Length < 2)
            return;

        float partes = (ruta.Length - 1) * progresoRuta;

        for (int i = 0; i < ruta.Length - 1; i++)
        {
            float avance = Math.Clamp(partes - i, 0f, 1f);

            if (avance <= 0f)
                break;

            int a = BuscarIndice(ruta[i]);
            int b = BuscarIndice(ruta[i + 1]);

            if (a < 0 || b < 0)
                continue;

            // Dibuja exactamente la conexión visual existente
            int indiceConexion = -1;

            for (int j = 0; j < conexiones.Length; j++)
            {
                ConexionVisual conexion = conexiones[j];

                if ((conexion.Origen == a &&
                     conexion.Destino == b) ||
                    (conexion.Bidireccional &&
                     conexion.Origen == b &&
                     conexion.Destino == a))
                {
                    indiceConexion = j;
                    break;
                }
            }

            if (indiceConexion < 0)
                continue;

            ConexionVisual tramo = conexiones[indiceConexion];

            using GraphicsPath curva = CrearCurva(
                tramo.Origen, tramo.Destino, indiceConexion);

            if (curva.PointCount == 0)
                continue;

            using var halo = new Pen(
                Color.FromArgb(55, TransRedTheme.Cobre), 10f);

            using var linea = new Pen(
                TransRedTheme.Cobre, 4f);

            halo.StartCap = halo.EndCap = LineCap.Round;
            linea.StartCap = linea.EndCap = LineCap.Round;

            if (avance >= 1f)
            {
                g.DrawPath(halo, curva);
                g.DrawPath(linea, curva);
            }
            else
            {
                // Revela progresivamente una fracción de la curva
                using GraphicsPath fraccion =
                    ObtenerFraccion(curva, avance);

                if (fraccion.PointCount > 1)
                {
                    g.DrawPath(halo, fraccion);
                    g.DrawPath(linea, fraccion);
                }
            }
        }
    }

    private static GraphicsPath ObtenerFraccion(
        GraphicsPath original, float avance)
    {
        using var copia = (GraphicsPath)original.Clone();
        copia.Flatten(null, 1f);

        PointF[] puntos = copia.PathPoints;
        var resultado = new GraphicsPath();

        if (puntos.Length < 2)
            return resultado;

        float total = 0f;

        for (int i = 1; i < puntos.Length; i++)
            total += Distancia(puntos[i - 1], puntos[i]);

        float limite = total * avance;
        float acumulado = 0f;

        var recorrido = new List<PointF> { puntos[0] };

        for (int i = 1; i < puntos.Length; i++)
        {
            float tramo = Distancia(puntos[i - 1], puntos[i]);

            if (acumulado + tramo >= limite)
            {
                float t = tramo <= 0f
                    ? 0f
                    : (limite - acumulado) / tramo;

                recorrido.Add(new PointF(
                    puntos[i - 1].X +
                        (puntos[i].X - puntos[i - 1].X) * t,
                    puntos[i - 1].Y +
                        (puntos[i].Y - puntos[i - 1].Y) * t
                ));

                break;
            }

            recorrido.Add(puntos[i]);
            acumulado += tramo;
        }

        if (recorrido.Count > 1)
            resultado.AddLines(recorrido.ToArray());

        return resultado;
    }

    private static float Distancia(PointF a, PointF b)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;

        return MathF.Sqrt(dx * dx + dy * dy);
    }

    private void DibujarNodos(Graphics g)
    {
        for (int i = 0; i < paradas.Length; i++)
        {
            PointF p = Posicion(i);

            if (p.X < -20 || p.X > Width + 20 ||
                p.Y < -20 || p.Y > Height + 20)
                continue;

            bool activo = i == hover || i == seleccionado;
            float radio = activo ? 10f : 6f;

            using var relleno = new SolidBrush(Color.White);
            using var borde = new Pen(
                TransRedTheme.Acento, activo ? 2.6f : 1.6f);

            if (activo)
            {
                using var halo = new SolidBrush(
                    Color.FromArgb(45, TransRedTheme.Acento));

                g.FillEllipse(
                    halo, p.X - 16, p.Y - 16, 32, 32);
            }

            g.FillEllipse(
                relleno,
                p.X - radio, p.Y - radio,
                radio * 2, radio * 2);

            g.DrawEllipse(
                borde,
                p.X - radio, p.Y - radio,
                radio * 2, radio * 2);
        }

        // Origen, destino y paradas intermedias
        if (ruta.Length == 0)
            return;

        for (int i = 0; i < ruta.Length; i++)
        {
            int indice = BuscarIndice(ruta[i]);

            if (indice < 0)
                continue;

            PointF p = Posicion(indice);

            bool extremo = i == 0 || i == ruta.Length - 1;
            float radio = extremo ? 10f : 7.5f;

            Color color = i == 0
                ? TransRedTheme.Acento
                : i == ruta.Length - 1
                    ? TransRedTheme.Cobre
                    : Color.FromArgb(192, 145, 94);

            using var relleno = new SolidBrush(Color.White);
            using var borde = new Pen(color, 3f);

            g.FillEllipse(
                relleno, p.X - radio, p.Y - radio,
                radio * 2, radio * 2);

            g.DrawEllipse(
                borde, p.X - radio, p.Y - radio,
                radio * 2, radio * 2);
        }
    }

    private void DibujarEtiquetas(Graphics g)
    {
        using var fuente = new Font("Segoe UI", 8.5f);

        var ocupadas = new List<Rectangle>();

        int frecuencia = zoom > 2.2f
            ? 1
            : zoom > 1.4f ? 2 : Math.Max(4, paradas.Length / 7);

        for (int i = 0; i < paradas.Length; i++)
        {
            bool esRuta = ruta.Contains(
                paradas[i].Codigo,
                StringComparer.OrdinalIgnoreCase);

            bool prioridad = esRuta || i == hover || i == seleccionado;

            if (!prioridad && i % frecuencia != 0)
                continue;

            PointF p = Posicion(i);

            string nombre = paradas[i].Nombre
                .Replace("Parque Central - ", "");

            if (nombre.Length > 23)
                nombre = nombre[..20] + "...";

            string texto = prioridad
                ? $"{paradas[i].Codigo} · {nombre}"
                : nombre;

            Size medida = TextRenderer.MeasureText(texto, fuente);

            Rectangle rect = new Rectangle(
                (int)(p.X - medida.Width / 2f),
                (int)p.Y + 14,
                medida.Width + 5,
                medida.Height + 2
            );

            if (rect.Left < 4 || rect.Top < 4 ||
                rect.Right > Width - 4 ||
                rect.Bottom > Height - 34)
                continue;

            if (ocupadas.Any(x => x.IntersectsWith(rect)))
                continue;

            using var fondoTexto = new SolidBrush(
                Color.FromArgb(247, 249, 244));

            g.FillRectangle(fondoTexto, rect);

            TextRenderer.DrawText(
                g, texto, fuente, rect,
                prioridad ? TransRedTheme.Bosque
                          : TransRedTheme.TextoSecundario,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine
            );

            ocupadas.Add(rect);
        }
    }

    private void DibujarIndicadores(Graphics g)
    {
        using var titulo = new Font(
            "Segoe UI", 8.5f, FontStyle.Bold);

        using var ayuda = new Font("Segoe UI", 8f);

        TextRenderer.DrawText(
            g,
            $"RED REGIONAL · {paradas.Length} PARADAS",
            titulo,
            new Rectangle(14, 14, Math.Max(100, Width - 170), 28),
            TransRedTheme.Bosque
        );

        TextRenderer.DrawText(
            g,
            "Rueda: zoom · Clic derecho: mover · 0: restablecer",
            ayuda,
            new Rectangle(
                14, Height - 30,
                Math.Max(100, Width - 28), 23),
            TransRedTheme.TextoSecundario
        );
    }

    private void DibujarSeleccion(Graphics g)
    {
        if (seleccionado < 0 || seleccionado >= paradas.Length)
            return;

        ParadaVisual parada = paradas[seleccionado];

        int conexionesAsociadas = conexiones.Count(c =>
            c.Origen == seleccionado || c.Destino == seleccionado);

        int ancho = Math.Min(280, Width - 30);
        int x = Math.Max(12, Width - ancho - 14);
        int y = 58;

        using var fondoTarjeta = new SolidBrush(Color.White);
        using var borde = new Pen(TransRedTheme.Borde);

        g.FillRectangle(fondoTarjeta, x, y, ancho, 95);
        g.DrawRectangle(borde, x, y, ancho, 95);

        TextRenderer.DrawText(
            g,
            "PARADA SELECCIONADA",
            new Font("Segoe UI", 8f, FontStyle.Bold),
            new Rectangle(x + 12, y + 10, ancho - 24, 20),
            TransRedTheme.Acento
        );

        TextRenderer.DrawText(
            g,
            $"{parada.Codigo} · {parada.Nombre}",
            new Font("Segoe UI", 10f, FontStyle.Bold),
            new Rectangle(x + 12, y + 34, ancho - 24, 30),
            TransRedTheme.Tinta,
            TextFormatFlags.EndEllipsis
        );

        TextRenderer.DrawText(
            g,
            $"{conexionesAsociadas} conexiones asociadas",
            new Font("Segoe UI", 9f),
            new Rectangle(x + 12, y + 67, ancho - 24, 20),
            TransRedTheme.TextoSecundario
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            animador.Dispose();

        base.Dispose(disposing);
    }

    private readonly record struct ParadaVisual(
        string Codigo,
        string Nombre,
        float X,
        float Y
    );

    private readonly record struct ConexionVisual(
        int Origen,
        int Destino,
        bool Bidireccional
    );
}
