
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Components;

// Mapa esquemático interactivo de TransRed
public class LienzoRed : Control
{
    private readonly IGestorTransporte gestor;

    private ParadaVisual[] paradas =
        Array.Empty<ParadaVisual>();

    private ConexionVisual[] conexiones =
        Array.Empty<ConexionVisual>();

    private int nodoHover = -1;
    private int nodoSeleccionado = -1;

    private float zoom = 1f;
    private PointF desplazamiento = PointF.Empty;

    private bool arrastrando;
    private Point ultimoMouse;

    private readonly Color fondo =
        Color.FromArgb(238, 243, 235);

    private readonly Color conexionSuave =
        Color.FromArgb(155, 178, 163);

    private readonly Color colorDirigido =
        Color.FromArgb(190, 133, 85);

    // Códigos del recorrido que se está mostrando
    private string[] rutaVisible = Array.Empty<string>();
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

        ActualizarDatos();
    }

    // Lee únicamente los datos existentes del Core
    public void ActualizarDatos()
    {
        Parada[] datosParadas =
            gestor.ObtenerTodasParadas();

        Conexion[] datosConexiones =
            gestor.ObtenerTodasConexiones();

        paradas = new ParadaVisual[datosParadas.Length];

        nodoHover = -1;
        nodoSeleccionado = -1;

        if (datosParadas.Length == 0)
        {
            conexiones = Array.Empty<ConexionVisual>();
            Invalidate();
            return;
        }

        double latMin = datosParadas[0].Latitud;
        double latMax = datosParadas[0].Latitud;
        double lonMin = datosParadas[0].Longitud;
        double lonMax = datosParadas[0].Longitud;

        foreach (Parada parada in datosParadas)
        {
            latMin = Math.Min(latMin, parada.Latitud);
            latMax = Math.Max(latMax, parada.Latitud);

            lonMin = Math.Min(lonMin, parada.Longitud);
            lonMax = Math.Max(lonMax, parada.Longitud);
        }

        for (int i = 0; i < datosParadas.Length; i++)
        {
            Parada parada = datosParadas[i];

            float x = lonMax == lonMin
                ? 0.5f
                : (float)(
                    (parada.Longitud - lonMin) /
                    (lonMax - lonMin));

            float y = latMax == latMin
                ? 0.5f
                : 1f - (float)(
                    (parada.Latitud - latMin) /
                    (latMax - latMin));

            paradas[i] = new ParadaVisual(
                parada.Codigo,
                parada.Nombre,
                0.08f + x * 0.84f,
                0.09f + y * 0.82f
            );
        }

        var lista = new List<ConexionVisual>();

        for (int i = 0; i < datosConexiones.Length; i++)
        {
            Conexion conexion = datosConexiones[i];

            int origen =
                BuscarIndice(conexion.CodigoOrigen);

            int destino =
                BuscarIndice(conexion.CodigoDestino);

            if (origen < 0 || destino < 0)
                continue;

            bool regreso = false;

            foreach (Conexion otra in datosConexiones)
            {
                if (otra.CodigoOrigen ==
                        conexion.CodigoDestino &&
                    otra.CodigoDestino ==
                        conexion.CodigoOrigen)
                {
                    regreso = true;
                    break;
                }
            }

            if (regreso && origen > destino)
                continue;

            lista.Add(new ConexionVisual(
                origen, destino, regreso));
        }

        conexiones = lista.ToArray();
        Invalidate();
    }

    private int BuscarIndice(string codigo)
    {
        for (int i = 0; i < paradas.Length; i++)
        {
            if (paradas[i].Codigo == codigo)
                return i;
        }

        return -1;
    }

    // Posiciones calculadas a partir de coordenadas
    private PointF ObtenerPosicion(int indice)
    {
        ParadaVisual parada = paradas[indice];

        float margen = 44f;

        float ancho = Math.Max(1, Width - margen * 2);
        float alto = Math.Max(1, Height - margen * 2);

        float baseX = margen + parada.X * ancho;
        float baseY = margen + parada.Y * alto;

        return new PointF(
            Width / 2f +
                (baseX - Width / 2f) * zoom +
                desplazamiento.X,
            Height / 2f +
                (baseY - Height / 2f) * zoom +
                desplazamiento.Y
        );
    }

    private int BuscarNodo(Point punto)
    {
        for (int i = paradas.Length - 1; i >= 0; i--)
        {
            PointF centro = ObtenerPosicion(i);

            float dx = punto.X - centro.X;
            float dy = punto.Y - centro.Y;

            float radio = Math.Max(12f, 7f * zoom);

            if (dx * dx + dy * dy <= radio * radio)
                return i;
        }

        return -1;
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        float anterior = zoom;

        zoom = Math.Clamp(
            zoom * (e.Delta > 0 ? 1.15f : 0.87f),
            0.75f, 4f
        );

        // Conserva el punto bajo el cursor al acercar
        float cx = Width / 2f;
        float cy = Height / 2f;

        desplazamiento = new PointF(
            e.X - cx -
                (e.X - cx - desplazamiento.X)
                * zoom / anterior,
            e.Y - cy -
                (e.Y - cy - desplazamiento.Y)
                * zoom / anterior
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
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (arrastrando)
        {
            arrastrando = false;
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

        if (actual != nodoHover)
        {
            nodoHover = actual;
            Cursor = actual >= 0
                ? Cursors.Hand
                : Cursors.Default;

            Invalidate();
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        if (!arrastrando)
        {
            nodoHover = -1;
            Cursor = Cursors.Default;
            Invalidate();
        }
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        if (e.Button == MouseButtons.Left)
        {
            nodoSeleccionado = BuscarNodo(e.Location);
            Invalidate();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.D0 ||
            e.KeyCode == Keys.NumPad0)
        {
            zoom = 1f;
            desplazamiento = PointF.Empty;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        g.Clear(fondo);

        DibujarFondo(g);

        if (Width < 320 || Height < 240)
            return;

        if (paradas.Length == 0)
        {
            DibujarCentrado(
                g, "Sin paradas registradas");
            return;
        }

        DibujarConexiones(g);
        DibujarNodos(g);
        DibujarRutaSeleccionada(g);
        DibujarEtiquetas(g);
        DibujarEncabezado(g);
        DibujarAyuda(g);
        DibujarTarjetaSeleccion(g);
    }

    // Fondo discreto de puntos
    private void DibujarFondo(Graphics g)
    {
        using var pincel = new SolidBrush(
            Color.FromArgb(211, 225, 211)
        );

        const int paso = 28;

        for (int x = 12; x < Width; x += paso)
        {
            for (int y = 12; y < Height; y += paso)
            {
                g.FillEllipse(
                    pincel, x, y, 1.5f, 1.5f);
            }
        }
    }

    // Curvas visuales sobre las conexiones reales
    private GraphicsPath CrearCurva(
        PointF origen,
        PointF destino,
        int indice)
    {
        float dx = destino.X - origen.X;
        float dy = destino.Y - origen.Y;

        float longitud = MathF.Sqrt(dx * dx + dy * dy);

        var camino = new GraphicsPath();

        if (longitud < 2f)
            return camino;

        // Curvatura suave y estable entre extremos
        float curvatura =
            Math.Min(18f, longitud * 0.08f);

        float lado = indice % 2 == 0 ? 1f : -1f;

        float perpendicularX = -dy / longitud;
        float perpendicularY = dx / longitud;

        PointF control = new PointF(
            (origen.X + destino.X) / 2f +
                perpendicularX * curvatura * lado,
            (origen.Y + destino.Y) / 2f +
                perpendicularY * curvatura * lado
        );

        camino.AddBezier(
            origen,
            control,
            control,
            destino
        );

        return camino;
    }

    private void DibujarConexiones(Graphics g)
    {
        // Primero las conexiones de contexto
        for (int i = 0; i < conexiones.Length; i++)
        {
            ConexionVisual conexion = conexiones[i];

            bool destacada =
                nodoSeleccionado >= 0 &&
                (conexion.Origen == nodoSeleccionado ||
                 conexion.Destino == nodoSeleccionado);

            PointF origen =
                ObtenerPosicion(conexion.Origen);

            PointF destino =
                ObtenerPosicion(conexion.Destino);

            if (Math.Abs(origen.X - destino.X) < 1f &&
                Math.Abs(origen.Y - destino.Y) < 1f)
                continue;

            Color baseColor = conexion.Bidireccional
                ? conexionSuave
                : colorDirigido;

            Color color = destacada
                ? baseColor
                : Color.FromArgb(
                    nodoSeleccionado < 0 ? 105 : 48,
                    baseColor
                );

            using var lapiz = new Pen(
                color,
                destacada ? 3.5f : 1.35f
            );

            lapiz.StartCap = LineCap.Round;
            lapiz.EndCap = LineCap.Round;

            using GraphicsPath curva =
                CrearCurva(origen, destino, i);

            if (curva.PointCount > 0)
                g.DrawPath(lapiz, curva);
        }
    }

    
    private void DibujarRutaSeleccionada(Graphics g)
    {
        if (rutaVisible.Length < 2)
            return;

        using var halo = new Pen(
            Color.FromArgb(65, 190, 126, 70),
            9f);

        using var linea = new Pen(
            TransRedTheme.Cobre,
            4f);

        halo.StartCap = LineCap.Round;
        halo.EndCap = LineCap.Round;

        linea.StartCap = LineCap.Round;
        linea.EndCap = LineCap.Round;

        for (int i = 0; i < rutaVisible.Length - 1; i++)
        {
            int indiceOrigen = BuscarIndice(rutaVisible[i]);
            int indiceDestino = BuscarIndice(rutaVisible[i + 1]);

            if (indiceOrigen < 0 || indiceDestino < 0)
                continue;

            PointF origen = ObtenerPosicion(indiceOrigen);
            PointF destino = ObtenerPosicion(indiceDestino);

            // Busca la conexión real entre ambos nodos
            int indiceConexion = -1;

            for (int j = 0; j < conexiones.Length; j++)
            {
                ConexionVisual conexion = conexiones[j];

                if ((conexion.Origen == indiceOrigen &&
                    conexion.Destino == indiceDestino) ||
                    (conexion.Bidireccional &&
                    conexion.Origen == indiceDestino &&
                    conexion.Destino == indiceOrigen))
                {
                    indiceConexion = j;
                    break;
                }
            }

            if (indiceConexion < 0)
                continue;

            // Dibuja la misma curva que la conexión
            PointF inicio = ObtenerPosicion(
                conexiones[indiceConexion].Origen);

            PointF fin = ObtenerPosicion(
                conexiones[indiceConexion].Destino);

            using GraphicsPath curva = CrearCurva(
                inicio,
                fin,
                indiceConexion);

            if (curva.PointCount > 0)
            {
                g.DrawPath(halo, curva);
                g.DrawPath(linea, curva);
            }
        }

        // Destaca las paradas incluidas en el recorrido
        foreach (string codigo in rutaVisible)
        {
            int indice = BuscarIndice(codigo);

            if (indice < 0)
                continue;

            PointF punto = ObtenerPosicion(indice);

            using var relleno = new SolidBrush(Color.White);
            using var borde = new Pen(
                TransRedTheme.Cobre, 3f);

            g.FillEllipse(
                relleno,
                punto.X - 8, punto.Y - 8,
                16, 16);

            g.DrawEllipse(
                borde,
                punto.X - 8, punto.Y - 8,
                16, 16);
        }
    }


    // Nodos más pequeños para evitar saturación
    private void DibujarNodos(Graphics g)
    {
        for (int i = 0; i < paradas.Length; i++)
        {
            PointF punto = ObtenerPosicion(i);

            if (punto.X < -25 || punto.X > Width + 25 ||
                punto.Y < -25 || punto.Y > Height + 25)
                continue;

            bool elegido = i == nodoSeleccionado;
            bool hover = i == nodoHover;

            float radio = elegido
                ? 12f
                : hover ? 10f : 6.5f;

            if (zoom > 1.8f && !elegido && !hover)
                radio = 8f;

            if (elegido || hover)
            {
                using var halo = new SolidBrush(
                    Color.FromArgb(48, 43, 112, 91)
                );

                g.FillEllipse(
                    halo,
                    punto.X - radio - 7,
                    punto.Y - radio - 7,
                    (radio + 7) * 2,
                    (radio + 7) * 2
                );
            }

            using var relleno = new SolidBrush(
                elegido
                    ? TransRedTheme.Bosque
                    : Color.White
            );

            using var borde = new Pen(
                TransRedTheme.Acento,
                elegido ? 2.8f : 1.8f
            );

            g.FillEllipse(
                relleno,
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
        }
    }

    // Etiquetas selectivas para evitar superposiciones
    private void DibujarEtiquetas(Graphics g)
    {
        using Font fuente = new Font(
            "Segoe UI", 8.5f, FontStyle.Regular);

        var ocupados = new List<Rectangle>();

        int frecuencia = zoom > 2.2f
            ? 1
            : zoom > 1.5f
                ? 2
                : Math.Max(4, paradas.Length / 7);

        // Primera pasada: etiquetas generales
        for (int i = 0; i < paradas.Length; i++)
        {
            if (i != nodoHover &&
                i != nodoSeleccionado &&
                i % frecuencia != 0)
                continue;

            if (i == nodoHover || i == nodoSeleccionado)
                continue;

            DibujarEtiqueta(
                g, fuente, i, ocupados, false);
        }

        // Las etiquetas de interacción van encima
        if (nodoHover >= 0)
            DibujarEtiqueta(
                g, fuente, nodoHover,
                ocupados, true);

        if (nodoSeleccionado >= 0 &&
            nodoSeleccionado != nodoHover)
            DibujarEtiqueta(
                g, fuente, nodoSeleccionado,
                ocupados, true);
    }

    private void DibujarEtiqueta(
        Graphics g,
        Font fuente,
        int indice,
        List<Rectangle> ocupados,
        bool prioritaria)
    {
        PointF punto = ObtenerPosicion(indice);

        string nombre = paradas[indice].Nombre;

        // Evita repetir "Parque Central" en todo el mapa
        string corto = nombre.Replace(
            "Parque Central - ", "");

        if (corto.Length > 24)
            corto = corto.Substring(0, 21) + "...";

        string texto = prioritaria
            ? $"{paradas[indice].Codigo} · {corto}"
            : corto;

        Size tamano = TextRenderer.MeasureText(
            texto, fuente);

        Rectangle area = new Rectangle(
            (int)(punto.X - tamano.Width / 2f),
            (int)(punto.Y + 12),
            tamano.Width + 7,
            tamano.Height + 3
        );

        if (area.Left < 4 ||
            area.Right > Width - 4 ||
            area.Top < 4 ||
            area.Bottom > Height - 4)
            return;

        if (!prioritaria)
        {
            foreach (Rectangle otra in ocupados)
            {
                if (otra.IntersectsWith(area))
                    return;
            }
        }

        using var fondoEtiqueta = new SolidBrush(
            Color.FromArgb(246, 249, 244)
        );

        g.FillRectangle(fondoEtiqueta, area);

        TextRenderer.DrawText(
            g,
            texto,
            fuente,
            area,
            TransRedTheme.TextoSecundario,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.SingleLine
        );

        ocupados.Add(area);
    }

    private void DibujarEncabezado(Graphics g)
    {
        using Font fuente = new Font(
            "Segoe UI", 8, FontStyle.Bold);

        TextRenderer.DrawText(
            g,
            $"RED REGIONAL  ·  {paradas.Length} PARADAS",
            fuente,
            new Rectangle(16, 12, Width - 30, 25),
            TransRedTheme.Bosque
        );
    }

    private void DibujarAyuda(Graphics g)
    {
        using Font fuente = new Font(
            "Segoe UI", 8);

        TextRenderer.DrawText(
            g,
            "Rueda: zoom  ·  Clic derecho: mover  ·  0: restablecer",
            fuente,
            new Rectangle(
                15,
                Math.Max(0, Height - 28),
                Math.Max(0, Width - 30),
                22
            ),
            TransRedTheme.TextoSecundario
        );
    }

    private void DibujarTarjetaSeleccion(Graphics g)
    {
        if (nodoSeleccionado < 0)
            return;

        ParadaVisual parada = paradas[nodoSeleccionado];

        int cantidad = 0;

        foreach (ConexionVisual conexion in conexiones)
        {
            if (conexion.Origen == nodoSeleccionado ||
                conexion.Destino == nodoSeleccionado)
                cantidad++;
        }

        int ancho = Math.Min(280, Width - 32);

        Rectangle area = new Rectangle(
            Width - ancho - 14,
            42,
            ancho,
            112
        );

        using var fondoTarjeta =
            new SolidBrush(Color.White);

        using var borde =
            new Pen(TransRedTheme.Borde);

        g.FillRectangle(fondoTarjeta, area);
        g.DrawRectangle(borde, area);

        using Font etiqueta = new Font(
            "Segoe UI", 8, FontStyle.Bold);

        using Font titulo = new Font(
            "Segoe UI", 11, FontStyle.Bold);

        using Font detalle = new Font(
            "Segoe UI", 9);

        TextRenderer.DrawText(
            g,
            "PARADA SELECCIONADA",
            etiqueta,
            new Rectangle(
                area.X + 13, area.Y + 10,
                area.Width - 24, 22),
            TransRedTheme.Acento
        );

        TextRenderer.DrawText(
            g,
            $"{parada.Codigo} · {parada.Nombre}",
            titulo,
            new Rectangle(
                area.X + 13, area.Y + 35,
                area.Width - 24, 34),
            TransRedTheme.Tinta,
            TextFormatFlags.EndEllipsis |
            TextFormatFlags.WordBreak
        );

        TextRenderer.DrawText(
            g,
            $"Conexiones asociadas: {cantidad}",
            detalle,
            new Rectangle(
                area.X + 13, area.Y + 79,
                area.Width - 24, 22),
            TransRedTheme.TextoSecundario
        );
    }

    private void DibujarCentrado(
        Graphics g, string texto)
    {
        TextRenderer.DrawText(
            g,
            texto,
            Font,
            ClientRectangle,
            TransRedTheme.TextoSecundario,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter
        );
    }

    // Modelos utilizados exclusivamente para dibujar
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

    
    public void MostrarRuta(string[] codigos)
    {
        rutaVisible = codigos ?? Array.Empty<string>();

        // Actualiza el mapa después de recibir el recorrido
        Invalidate();
    }

}
