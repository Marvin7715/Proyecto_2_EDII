using System;
using System.Drawing;
using System.Windows.Forms;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Layout;

// Distribución visual del commit 1.2, sin conexión al Core
public static class MainShell
{
    private static readonly Color FondoRed = Color.FromArgb(241, 245, 241);

    public static void Construir(Form ventana)
    {
        var estructura = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        estructura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        estructura.RowStyles.Add(new RowStyle(SizeType.Absolute, 94));
        estructura.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        estructura.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));

        estructura.Controls.Add(CrearEncabezado(), 0, 0);
        estructura.Controls.Add(CrearContenido(), 0, 1);
        estructura.Controls.Add(CrearNavegacion(), 0, 2);
        ventana.Controls.Add(estructura);
    }

    
    private static Control CrearEncabezado()
    {
        var encabezado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(20, 12, 24, 12),
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Superficie
        };

        encabezado.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 54));

        encabezado.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 65));

        encabezado.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 35));

        var simbolo = new Label
        {
            Text = "●─●",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Acento,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var marca = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(14, 0, 0, 0),
            BackColor = TransRedTheme.Superficie
        };

        var nombre = new Label
        {
            Text = "transred",
            Dock = DockStyle.Top,
            Height = 37,
            Margin = Padding.Empty,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = TransRedTheme.Tinta,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var subtitulo = new Label
        {
            Text = "RED DE TRANSPORTE",
            Dock = DockStyle.Bottom,
            Height = 20,
            Margin = Padding.Empty,
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = TransRedTheme.TextoSecundario,
            TextAlign = ContentAlignment.MiddleLeft
        };

        marca.Controls.Add(nombre);
        marca.Controls.Add(subtitulo);

        var seccion = new Label
        {
            Text = "EXPLORADOR DE RED",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Font = new Font("Segoe UI", 9),
            ForeColor = TransRedTheme.TextoSecundario,
            TextAlign = ContentAlignment.MiddleRight
        };

        encabezado.Controls.Add(simbolo, 0, 0);
        encabezado.Controls.Add(marca, 1, 0);
        encabezado.Controls.Add(seccion, 2, 0);

        return encabezado;
    }


    private static Control CrearContenido()
    {
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = new Padding(20, 20, 20, 18),
            BackColor = TransRedTheme.Fondo
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 316)
        );

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        contenido.Controls.Add(CrearPlanificador(), 0, 0);
        contenido.Controls.Add(CrearLienzo(), 1, 0);

        return contenido;
    }

    private static Control CrearPlanificador()
    {
        var lateral = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Margin = new Padding(0, 0, 16, 0),
            Padding = new Padding(20, 22, 20, 18),
            BackColor = TransRedTheme.Superficie
        };

        lateral.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        lateral.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        lateral.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        lateral.RowStyles.Add(new RowStyle(SizeType.Absolute, 83));
        lateral.RowStyles.Add(new RowStyle(SizeType.Absolute, 83));
        lateral.RowStyles.Add(new RowStyle(SizeType.Absolute, 57));
        lateral.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        lateral.Controls.Add(
            Texto("PLANIFICAR RECORRIDO", 10, FontStyle.Bold,
                TransRedTheme.Acento),
            0, 0
        );

        var descripcion = Texto(
            "Consulta rutas entre paradas",
            9.5f,
            FontStyle.Regular,
            TransRedTheme.TextoSecundario
        );

        descripcion.TextAlign = ContentAlignment.TopLeft;
        lateral.Controls.Add(descripcion, 0, 1);

        lateral.Controls.Add(
            CrearCampoEjemplo("ORIGEN", "Elegir parada de origen"),
            0, 2
        );

        lateral.Controls.Add(
            CrearCampoEjemplo("DESTINO", "Elegir parada de destino"),
            0, 3
        );

        var accion = new Label
        {
            Text = "Próximamente: calcular ruta",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 5, 0, 9),
            Padding = new Padding(6, 2, 6, 2),
            BackColor = TransRedTheme.AcentoSuave,
            ForeColor = TransRedTheme.Acento,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        lateral.Controls.Add(accion, 0, 4);

        var contenedorResultado = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Superficie
        };

        var estado = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 95,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(12, 12, 12, 12),
            BackColor = TransRedTheme.Fondo
        };

        estado.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        estado.RowStyles.Add(new RowStyle(SizeType.Absolute, 31));
        estado.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        estado.Controls.Add(
            Texto("Sin recorrido seleccionado", 9, FontStyle.Bold,
                TransRedTheme.Tinta),
            0, 0
        );

        var detalle = Texto(
            "Aún no hay resultados",
            8.5f,
            FontStyle.Regular,
            TransRedTheme.TextoSecundario
        );

        detalle.TextAlign = ContentAlignment.TopLeft;
        estado.Controls.Add(detalle, 0, 1);

        contenedorResultado.Controls.Add(estado);
        lateral.Controls.Add(contenedorResultado, 0, 5);

        return lateral;
    }

    private static Control CrearCampoEjemplo(
        string titulo,
        string ayuda)
    {
        var bloque = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0, 1, 0, 12)
        };

        bloque.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        bloque.RowStyles.Add(new RowStyle(SizeType.Absolute, 23));
        bloque.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        bloque.Controls.Add(
            Texto(titulo, 8.5f, FontStyle.Bold,
                TransRedTheme.TextoSecundario),
            0, 0
        );

        bloque.Controls.Add(new Label
        {
            Text = ayuda,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(12, 0, 8, 0),
            BackColor = TransRedTheme.Fondo,
            ForeColor = TransRedTheme.TextoSecundario,
            Font = new Font("Segoe UI", 9, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BorderStyle = BorderStyle.FixedSingle,
            AutoEllipsis = true
        }, 0, 1);

        return bloque;
    }

    private static Control CrearLienzo()
    {
        var lienzo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = new Padding(24, 20, 24, 19),
            BackColor = FondoRed
        };

        lienzo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        lienzo.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        lienzo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        lienzo.RowStyles.Add(new RowStyle(SizeType.Absolute, 29));

        lienzo.Controls.Add(
            Texto("RED DE CONEXIONES", 9, FontStyle.Bold,
                TransRedTheme.TextoSecundario),
            0, 0
        );

        var centro = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = FondoRed
        };

        centro.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        centro.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        centro.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
        centro.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        var mensaje = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            BackColor = FondoRed
        };

        mensaje.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        mensaje.RowStyles.Add(new RowStyle(SizeType.Absolute, 53));
        mensaje.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titulo = Texto(
            "Tu red aparecerá aquí",
            17,
            FontStyle.Bold,
            TransRedTheme.Tinta
        );

        titulo.TextAlign = ContentAlignment.MiddleCenter;
        mensaje.Controls.Add(titulo, 0, 0);

        var explicacion = Texto(
            "Más adelante verás las paradas y sus conexiones",
            10,
            FontStyle.Regular,
            TransRedTheme.TextoSecundario
        );

        explicacion.TextAlign = ContentAlignment.TopCenter;
        mensaje.Controls.Add(explicacion, 0, 1);

        centro.Controls.Add(mensaje, 0, 1);
        lienzo.Controls.Add(centro, 0, 1);

        var nota = Texto(
            "VISTA INICIAL  ·  SIN DATOS CARGADOS",
            8,
            FontStyle.Regular,
            TransRedTheme.TextoSecundario
        );

        nota.TextAlign = ContentAlignment.MiddleRight;
        lienzo.Controls.Add(nota, 0, 2);

        return lienzo;
    }

    private static Control CrearNavegacion()
    {
        var contenedor = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20, 8, 20, 9),
            BackColor = TransRedTheme.Superficie
        };

        var linea = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = TransRedTheme.Borde
        };

        contenedor.Controls.Add(linea);

        var pestanas = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true,
            BackColor = TransRedTheme.Superficie
        };

        pestanas.Controls.Add(
            CrearPestana("●  Explorar red", true, 146)
        );

        pestanas.Controls.Add(
            CrearPestana("Paradas", false, 105)
        );

        pestanas.Controls.Add(
            CrearPestana("Conexiones", false, 122)
        );

        pestanas.Controls.Add(
            CrearPestana("Planificar", false, 111)
        );

        pestanas.Controls.Add(
            CrearPestana("Algoritmos", false, 115)
        );

        contenedor.Controls.Add(pestanas);
        pestanas.BringToFront();

        return contenedor;
    }

    private static Control CrearPestana(
        string nombre,
        bool activa,
        int ancho)
    {
        return new Label
        {
            Text = nombre,
            Size = new Size(ancho, 34),
            Margin = new Padding(0, 0, 10, 0),
            ForeColor = activa
                ? TransRedTheme.Acento
                : TransRedTheme.TextoSecundario,
            BackColor = activa
                ? TransRedTheme.AcentoSuave
                : TransRedTheme.Superficie,
            Font = new Font(
                "Segoe UI",
                9,
                activa ? FontStyle.Bold : FontStyle.Regular
            ),
            TextAlign = ContentAlignment.MiddleCenter
        };
    }

    private static Label Texto(
        string contenido,
        float tamano,
        FontStyle estilo,
        Color color)
    {
        return new Label
        {
            Text = contenido,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            ForeColor = color,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", tamano, estilo),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
    }
}
