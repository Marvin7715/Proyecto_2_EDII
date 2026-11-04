using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

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

    public VistaParadas(IGestorTransporte gestor)
    {
        this.gestor = gestor;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.Fondo;
        Padding = new Padding(25, 20, 25, 20);

        txtCodigo = CrearEntrada("Código de parada");
        txtNombre = CrearEntrada("Nombre de la parada");
        txtLatitud = CrearEntrada("Ej. 14.6349");
        txtLongitud = CrearEntrada("Ej. -90.5069");
        txtBusqueda = CrearEntrada("Código de la parada");

        lblEstado = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = TransRedTheme.TextoSecundario,
            Font = new Font("Segoe UI", 9),
            Text = "Registra una nueva parada o consulta las existentes.",
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };

        tabla = CrearTabla();

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
            BackColor = TransRedTheme.Fondo
        };

        principal.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 65));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        principal.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42));

        var titulo = new Label
        {
            Text = "GESTIÓN DE PARADAS",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            ForeColor = TransRedTheme.Tinta,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = TransRedTheme.Fondo
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 340));

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.Controls.Add(CrearPanelFormulario(), 0, 0);
        contenido.Controls.Add(CrearPanelTabla(), 1, 0);

        principal.Controls.Add(titulo, 0, 0);
        principal.Controls.Add(contenido, 0, 1);
        principal.Controls.Add(lblEstado, 0, 2);

        return principal;
    }

    private Control CrearPanelFormulario()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 11,
            Padding = new Padding(18),
            Margin = new Padding(0, 0, 15, 0),
            BackColor = TransRedTheme.Superficie
        };

        panel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        float[] alturas =
        {
            35, 28, 43, 28, 43, 28, 43, 28, 43, 48, 48
        };

        foreach (float altura in alturas)
        {
            panel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, altura));
        }

        panel.Controls.Add(CrearEtiqueta("Nueva parada"), 0, 0);
        panel.Controls.Add(CrearEtiqueta("CÓDIGO"), 0, 1);
        panel.Controls.Add(txtCodigo, 0, 2);
        panel.Controls.Add(CrearEtiqueta("NOMBRE"), 0, 3);
        panel.Controls.Add(txtNombre, 0, 4);
        panel.Controls.Add(CrearEtiqueta("LATITUD"), 0, 5);
        panel.Controls.Add(txtLatitud, 0, 6);
        panel.Controls.Add(CrearEtiqueta("LONGITUD"), 0, 7);
        panel.Controls.Add(txtLongitud, 0, 8);

        var btnRegistrar = CrearBoton("Registrar parada", true);
        btnRegistrar.Click += (sender, e) => Registrar();

        var btnLimpiar = CrearBoton("Limpiar campos", false);
        btnLimpiar.Click += (sender, e) => LimpiarCampos();

        panel.Controls.Add(btnRegistrar, 0, 9);
        panel.Controls.Add(btnLimpiar, 0, 10);

        return panel;
    }

    private Control CrearPanelTabla()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(18),
            BackColor = TransRedTheme.Superficie
        };

        panel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 35));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 48));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 48));

        panel.Controls.Add(
            CrearEtiqueta("Paradas registradas"),
            0, 0);

        var busqueda = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };

        busqueda.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        busqueda.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 105));

        var btnBuscar = CrearBoton("Buscar", true);
        btnBuscar.Click += (sender, e) => Buscar();

        busqueda.Controls.Add(txtBusqueda, 0, 0);
        busqueda.Controls.Add(btnBuscar, 1, 0);

        panel.Controls.Add(busqueda, 0, 1);
        panel.Controls.Add(tabla, 0, 2);

        var btnEliminar = CrearBoton("Eliminar parada seleccionada", false);
        btnEliminar.Click += (sender, e) => EliminarSeleccionada();

        panel.Controls.Add(btnEliminar, 0, 3);

        return panel;
    }

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
            MostrarEstado("No se pudo registrar. Comprueba que el código y nombre no estén duplicados.");
            return;
        }

        ActualizarTabla();
        LimpiarCampos();
        MostrarEstado("Parada registrada correctamente.");
    }

    private void Buscar()
    {
        string codigo = txtBusqueda.Text.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            MostrarEstado("Escribe el código de la parada que deseas buscar.");
            return;
        }

        Parada? parada = gestor.BuscarParada(codigo);

        if (parada == null)
        {
            tabla.ClearSelection();
            MostrarEstado("No se encontró una parada con ese código.");
            return;
        }

        ActualizarTabla();

        foreach (DataGridViewRow fila in tabla.Rows)
        {
            if (fila.Cells[0].Value?.ToString() == parada.Codigo)
            {
                fila.Selected = true;
                tabla.CurrentCell = fila.Cells[0];
                break;
            }
        }

        MostrarEstado($"Parada encontrada: {parada.Nombre}.");
    }

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

    private static bool IntentarNumero(string texto, out double numero)
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

    private static TextBox CrearEntrada(string sugerencia)
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 3, 0, 5),
            Font = new Font("Segoe UI", 10),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(247, 249, 247),
            ForeColor = TransRedTheme.Tinta,
            PlaceholderText = sugerencia
        };
    }

    private static Label CrearEtiqueta(string texto)
    {
        return new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = TransRedTheme.TextoSecundario,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    private static Button CrearBoton(string texto, bool principal)
    {
        return new Button
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 0, 4),
            FlatStyle = FlatStyle.Flat,
            BackColor = principal
                ? TransRedTheme.Acento
                : TransRedTheme.AcentoSuave,
            ForeColor = principal
                ? Color.White
                : TransRedTheme.Acento,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand,
            FlatAppearance =
            {
                BorderSize = 0
            }
        };
    }

    private static DataGridView CrearTabla()
    {
        var tabla = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = TransRedTheme.Superficie,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            ReadOnly = true,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            GridColor = TransRedTheme.Borde,
            Font = new Font("Segoe UI", 9)
        };

        tabla.ColumnHeadersDefaultCellStyle.BackColor =
            TransRedTheme.AcentoSuave;

        tabla.ColumnHeadersDefaultCellStyle.ForeColor =
            TransRedTheme.Tinta;

        tabla.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 9, FontStyle.Bold);

        tabla.DefaultCellStyle.SelectionBackColor =
            TransRedTheme.AcentoSuave;

        tabla.DefaultCellStyle.SelectionForeColor =
            TransRedTheme.Tinta;

        tabla.Columns.Add("Codigo", "Código");
        tabla.Columns.Add("Nombre", "Nombre");
        tabla.Columns.Add("Latitud", "Latitud");
        tabla.Columns.Add("Longitud", "Longitud");

        return tabla;
    }
}