
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

// Gestión visual de las paradas existentes
public class VistaParadas : UserControl
{
    private readonly IGestorTransporte gestor;

    private readonly TextBox txtCodigo;
    private readonly TextBox txtNombre;
    private readonly TextBox txtLatitud;
    private readonly TextBox txtLongitud;
    private readonly TextBox txtBusqueda;

    private readonly DataGridView tabla;
    private readonly Label lblEstado;
    private readonly Label lblCantidad;

    public VistaParadas(IGestorTransporte gestor)
    {
        this.gestor = gestor;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.Fondo;
        Padding = new Padding(24, 20, 24, 20);

        txtCodigo = CrearEntrada("Ej. XELA");
        txtNombre = CrearEntrada("Nombre de la parada");
        txtLatitud = CrearEntrada("Ej. 14.6349");
        txtLongitud = CrearEntrada("Ej. -90.5069");
        txtBusqueda = CrearEntrada("Buscar por código");

        tabla = CrearTabla();

        lblEstado = new Label
        {
            Text = "Selecciona una parada o registra una nueva",
            Dock = DockStyle.Fill,
            ForeColor = TransRedTheme.TextoSecundario,
            Font = new Font("Segoe UI", 9),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };

        lblCantidad = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = TransRedTheme.Acento,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };

        Controls.Add(CrearContenido());
        ActualizarTabla();
    }

    private Control CrearContenido()
    {
        var principal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42));

        var encabezado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = TransRedTheme.Fondo
        };

        encabezado.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 22));

        encabezado.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 47));

        encabezado.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        encabezado.Controls.Add(
            CrearTexto(
                "RED DE TRANSPORTE / PARADAS",
                8,
                TransRedTheme.Cobre,
                true), 0, 0);

        encabezado.Controls.Add(
            CrearTexto(
                "Paradas de la red",
                22,
                TransRedTheme.Tinta,
                true), 0, 1);

        encabezado.Controls.Add(
            CrearTexto(
                "Consulta y organiza los puntos de la red",
                9.5f,
                TransRedTheme.TextoSecundario),
            0, 2);

        var cuerpo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = TransRedTheme.Fondo,
            Margin = Padding.Empty
        };

        cuerpo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 335));

        cuerpo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        cuerpo.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        cuerpo.Controls.Add(CrearFormulario(), 0, 0);
        cuerpo.Controls.Add(CrearListado(), 1, 0);

        principal.Controls.Add(encabezado, 0, 0);
        principal.Controls.Add(cuerpo, 0, 1);
        principal.Controls.Add(lblEstado, 0, 2);

        return principal;
    }

    private Control CrearFormulario()
    {
        var tarjeta = CrearTarjeta(
            new Padding(20),
            new Padding(0, 0, 14, 0)
        );

        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 12,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        int[] alturas =
        {
            52, 24, 45, 24, 45, 24,
            45, 24, 45, 62, 54
        };

        foreach (int alto in alturas)
        {
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, alto));
        }

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.Controls.Add(
            CrearTexto(
                "Nueva parada", 16,
                TransRedTheme.Tinta, true),
            0, 0);

        AgregarCampo(contenido, "CÓDIGO", txtCodigo, 1);
        AgregarCampo(contenido, "NOMBRE", txtNombre, 3);
        AgregarCampo(contenido, "LATITUD", txtLatitud, 5);
        AgregarCampo(contenido, "LONGITUD", txtLongitud, 7);

        var btnRegistrar = new Button
        {
            Text = "Registrar parada",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 8, 0, 8)
        };

        UiStyles.AplicarBotonPrincipal(btnRegistrar);
        btnRegistrar.Click += (_, _) => Registrar();

        var btnLimpiar = new Button
        {
            Text = "Limpiar campos",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 0, 8)
        };

        UiStyles.AplicarBotonSecundario(btnLimpiar);
        btnLimpiar.Click += (_, _) => LimpiarCampos();

        contenido.Controls.Add(btnRegistrar, 0, 9);
        contenido.Controls.Add(btnLimpiar, 0, 10);

        tarjeta.Controls.Add(contenido);
        return tarjeta;
    }

    private Control CrearListado()
    {
        var tarjeta = CrearTarjeta(
            new Padding(20),
            Padding.Empty
        );

        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 61));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 62));

        var titulo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent
        };

        titulo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        titulo.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 160));

        titulo.Controls.Add(
            CrearTexto(
                "Paradas registradas", 15,
                TransRedTheme.Tinta, true),
            0, 0);

        titulo.Controls.Add(lblCantidad, 1, 0);

        var busqueda = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 0, 0, 10)
        };

        busqueda.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        busqueda.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 110));

        txtBusqueda.Dock = DockStyle.Fill;
        txtBusqueda.Margin = new Padding(0, 4, 8, 2);

        txtBusqueda.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                Buscar();
                e.SuppressKeyPress = true;
            }
        };

        var btnBuscar = new Button
        {
            Text = "Buscar",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 2, 0, 2)
        };

        UiStyles.AplicarBotonPrincipal(btnBuscar);
        btnBuscar.Click += (_, _) => Buscar();

        busqueda.Controls.Add(txtBusqueda, 0, 0);
        busqueda.Controls.Add(btnBuscar, 1, 0);

        var acciones = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 9, 0, 0)
        };

        acciones.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        acciones.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 170));

        var btnEliminar = new Button
        {
            Text = "Eliminar seleccionada",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 12, 0),
            BackColor = Color.FromArgb(250, 236, 231),
            ForeColor = TransRedTheme.Error,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font(
                "Segoe UI", 9, FontStyle.Bold)
        };

        btnEliminar.FlatAppearance.BorderSize = 0;
        btnEliminar.Click += (_, _) => EliminarSeleccionada();

        var btnActualizar = new Button
        {
            Text = "Actualizar lista",
            Dock = DockStyle.Fill
        };

        UiStyles.AplicarBotonSecundario(btnActualizar);

        btnActualizar.Click += (_, _) =>
        {
            ActualizarTabla();
            MostrarEstado("Listado actualizado");
        };

        acciones.Controls.Add(btnEliminar, 0, 0);
        acciones.Controls.Add(btnActualizar, 1, 0);

        contenido.Controls.Add(titulo, 0, 0);
        contenido.Controls.Add(busqueda, 0, 1);
        contenido.Controls.Add(tabla, 0, 2);
        contenido.Controls.Add(acciones, 0, 3);

        tarjeta.Controls.Add(contenido);
        return tarjeta;
    }

    private static GradientSurface CrearTarjeta(
        Padding padding,
        Padding margen)
    {
        return new GradientSurface(
            Color.White,
            Color.FromArgb(252, 251, 247),
            18)
        {
            Dock = DockStyle.Fill,
            Margin = margen,
            Padding = padding
        };
    }

    private static void AgregarCampo(
        TableLayoutPanel tabla,
        string etiqueta,
        TextBox campo,
        int fila)
    {
        tabla.Controls.Add(
            CrearTexto(
                etiqueta, 8,
                TransRedTheme.TextoSecundario,
                true),
            0, fila);

        campo.Dock = DockStyle.Fill;
        campo.Margin = new Padding(0, 1, 0, 9);

        tabla.Controls.Add(campo, 0, fila + 1);
    }

    private static Label CrearTexto(
        string texto,
        float tamano,
        Color color,
        bool negrita = false)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = color,
            Font = new Font(
                "Segoe UI",
                tamano,
                negrita
                    ? FontStyle.Bold
                    : FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };
    }

    private static TextBox CrearEntrada(string sugerencia)
    {
        return new TextBox
        {
            PlaceholderText = sugerencia,
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = TransRedTheme.SuperficieAlterna,
            ForeColor = TransRedTheme.Tinta
        };
    }

    private static DataGridView CrearTabla()
    {
        var tabla = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false
        };

        UiStyles.AplicarTabla(tabla);

        tabla.Columns.Add("Codigo", "Código");
        tabla.Columns.Add("Nombre", "Nombre");
        tabla.Columns.Add("Latitud", "Latitud");
        tabla.Columns.Add("Longitud", "Longitud");

        tabla.Columns[0].FillWeight = 15;
        tabla.Columns[1].FillWeight = 49;
        tabla.Columns[2].FillWeight = 18;
        tabla.Columns[3].FillWeight = 18;

        tabla.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        return tabla;
    }

    // Lógica original de registro
    private void Registrar()
    {
        string codigo = txtCodigo.Text.Trim();
        string nombre = txtNombre.Text.Trim();

        if (string.IsNullOrWhiteSpace(codigo) ||
            string.IsNullOrWhiteSpace(nombre) ||
            string.IsNullOrWhiteSpace(txtLatitud.Text) ||
            string.IsNullOrWhiteSpace(txtLongitud.Text))
        {
            MostrarEstado("Completa todos los campos.");
            return;
        }

        if (!IntentarNumero(txtLatitud.Text, out double latitud) ||
            !IntentarNumero(txtLongitud.Text, out double longitud))
        {
            MostrarEstado("Las coordenadas deben ser números válidos.");
            return;
        }

        if (latitud < -90 || latitud > 90 ||
            longitud < -180 || longitud > 180)
        {
            MostrarEstado("Las coordenadas están fuera de los rangos permitidos.");
            return;
        }

        var parada = new Parada(
            codigo, nombre, latitud, longitud);

        if (!gestor.RegistrarParada(parada))
        {
            MostrarEstado(
                "No se pudo registrar. Comprueba que el código y nombre no estén duplicados.");
            return;
        }

        ActualizarTabla();
        LimpiarCampos();
        MostrarEstado("Parada registrada correctamente.");
    }

    // Lógica original de búsqueda
    private void Buscar()
    {
        string codigo = txtBusqueda.Text.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            MostrarEstado(
                "Escribe el código de la parada que deseas buscar.");
            return;
        }

        Parada? parada = gestor.BuscarParada(codigo);

        if (parada == null)
        {
            tabla.ClearSelection();
            MostrarEstado(
                "No se encontró una parada con ese código.");
            return;
        }

        ActualizarTabla();

        foreach (DataGridViewRow fila in tabla.Rows)
        {
            if (fila.Cells[0].Value?.ToString() ==
                parada.Codigo)
            {
                fila.Selected = true;
                tabla.CurrentCell = fila.Cells[0];
                break;
            }
        }

        MostrarEstado(
            $"Parada encontrada: {parada.Nombre}.");
    }

    // Lógica original de eliminación
    private void EliminarSeleccionada()
    {
        if (tabla.SelectedRows.Count == 0)
        {
            MostrarEstado("Selecciona una parada de la tabla.");
            return;
        }

        string codigo = tabla.SelectedRows[0]
            .Cells[0].Value?.ToString() ?? "";

        DialogResult respuesta = MessageBox.Show(
            $"¿Deseas eliminar la parada {codigo}?",
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (respuesta != DialogResult.Yes)
            return;

        if (!gestor.EliminarParada(codigo))
        {
            MostrarEstado("No se pudo eliminar la parada.");
            return;
        }

        ActualizarTabla();
        MostrarEstado("Parada eliminada correctamente.");
    }

    private void ActualizarTabla()
    {
        tabla.Rows.Clear();

        Parada[] paradas = gestor.ObtenerTodasParadas();

        foreach (Parada parada in paradas)
        {
            tabla.Rows.Add(
                parada.Codigo,
                parada.Nombre,
                parada.Latitud.ToString("F4"),
                parada.Longitud.ToString("F4"));
        }

        lblCantidad.Text = $"{paradas.Length} paradas";
        tabla.ClearSelection();
    }

    private void LimpiarCampos()
    {
        txtCodigo.Clear();
        txtNombre.Clear();
        txtLatitud.Clear();
        txtLongitud.Clear();
        txtBusqueda.Clear();
        tabla.ClearSelection();
        txtCodigo.Focus();
    }

    private static bool IntentarNumero(
        string texto, out double numero)
    {
        return double.TryParse(
                   texto,
                   NumberStyles.Float,
                   CultureInfo.CurrentCulture,
                   out numero)
               ||
               double.TryParse(
                   texto,
                   NumberStyles.Float,
                   CultureInfo.InvariantCulture,
                   out numero);
    }

    private void MostrarEstado(string mensaje)
    {
        lblEstado.Text = mensaje;
    }
}
