
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;
using TransRed.Interfaz.Components;
using TransRed.Interfaz.Design;

namespace TransRed.Interfaz.Views;

// Gestión visual de conexiones de transporte
public class VistaConexiones : UserControl
{
    private readonly IGestorTransporte gestor;

    private readonly ComboBox cmbOrigen = new();
    private readonly ComboBox cmbDestino = new();

    private readonly TextBox txtMinutos = new();
    private readonly TextBox txtCosto = new();

    private readonly CheckBox chkBidireccional = new();
    private readonly CheckBox chkEliminarInverso = new();

    private readonly DataGridView dgvConexiones = new();

    private readonly Label lblEstado = new();
    private readonly Label lblCantidad = new();

    public VistaConexiones(IGestorTransporte gestor)
    {
        this.gestor = gestor;

        Dock = DockStyle.Fill;
        BackColor = TransRedTheme.Fondo;
        Padding = new Padding(24, 20, 24, 20);

        PrepararCampos();
        PrepararTabla();

        Controls.Add(CrearContenido());

        ActualizarDatos();
    }

    private void PrepararCampos()
    {
        UiStyles.AplicarCombo(cmbOrigen);
        UiStyles.AplicarCombo(cmbDestino);

        cmbOrigen.DropDownWidth = 365;
        cmbDestino.DropDownWidth = 365;

        txtMinutos.PlaceholderText = "Ej. 45";
        txtCosto.PlaceholderText = "Ej. 25.00";

        UiStyles.AplicarCampo(txtMinutos);
        UiStyles.AplicarCampo(txtCosto);

        chkBidireccional.Text = "Conexión bidireccional";
        chkBidireccional.Checked = true;
        chkBidireccional.Dock = DockStyle.Fill;
        chkBidireccional.ForeColor = TransRedTheme.Tinta;
        chkBidireccional.Font =
            new Font("Segoe UI", 9.5f);

        chkEliminarInverso.Text =
            "Eliminar también el sentido inverso";

        chkEliminarInverso.AutoSize = true;
        chkEliminarInverso.ForeColor =
            TransRedTheme.TextoSecundario;
        chkEliminarInverso.Font =
            new Font("Segoe UI", 9);

        lblEstado.Text = "Selecciona una conexión para administrarla";
        lblEstado.Dock = DockStyle.Fill;
        lblEstado.ForeColor =
            TransRedTheme.TextoSecundario;
        lblEstado.Font = new Font("Segoe UI", 9);
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;
        lblEstado.AutoEllipsis = true;

        lblCantidad.Dock = DockStyle.Fill;
        lblCantidad.ForeColor = TransRedTheme.Acento;
        lblCantidad.Font =
            new Font("Segoe UI", 9, FontStyle.Bold);
        lblCantidad.TextAlign =
            ContentAlignment.MiddleRight;
    }

    private void PrepararTabla()
    {
        dgvConexiones.Dock = DockStyle.Fill;

        UiStyles.AplicarTabla(dgvConexiones);

        dgvConexiones.Columns.Add(
            "Origen", "Origen");

        dgvConexiones.Columns.Add(
            "Destino", "Destino");

        dgvConexiones.Columns.Add(
            "Minutos", "Tiempo (min)");

        dgvConexiones.Columns.Add(
            "Costo", "Costo (Q)");

        dgvConexiones.Columns[0].FillWeight = 28;
        dgvConexiones.Columns[1].FillWeight = 28;
        dgvConexiones.Columns[2].FillWeight = 22;
        dgvConexiones.Columns[3].FillWeight = 22;

        dgvConexiones.AllowUserToResizeRows = false;

        dgvConexiones.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
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
            Texto(
                "RED DE TRANSPORTE / CONEXIONES",
                8,
                TransRedTheme.Cobre,
                true), 0, 0);

        encabezado.Controls.Add(
            Texto(
                "Conexiones de transporte",
                22,
                TransRedTheme.Tinta,
                true), 0, 1);

        encabezado.Controls.Add(
            Texto(
                "Administra los trayectos entre paradas",
                9.5f,
                TransRedTheme.TextoSecundario),
            0, 2);

        var cuerpo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            BackColor = TransRedTheme.Fondo
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
            RowCount = 13,
            Margin = Padding.Empty,
            BackColor = Color.Transparent
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        int[] alturas =
        {
            52, 24, 48, 24, 48, 24,
            48, 24, 48, 54, 67, 54
        };

        foreach (int alto in alturas)
        {
            contenido.RowStyles.Add(
                new RowStyle(SizeType.Absolute, alto));
        }

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.Controls.Add(
            Texto(
                "Nueva conexión",
                16,
                TransRedTheme.Tinta,
                true),
            0, 0);

        AgregarCampo(
            contenido, "ORIGEN",
            cmbOrigen, 1);

        AgregarCampo(
            contenido, "DESTINO",
            cmbDestino, 3);

        AgregarCampo(
            contenido, "TIEMPO (MINUTOS)",
            txtMinutos, 5);

        AgregarCampo(
            contenido, "COSTO (Q)",
            txtCosto, 7);

        contenido.Controls.Add(
            chkBidireccional, 0, 9);

        var btnRegistrar = new Button
        {
            Text = "Registrar conexión",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 5, 0, 12)
        };

        UiStyles.AplicarBotonPrincipal(btnRegistrar);
        btnRegistrar.Click += BtnRegistrar_Click;

        contenido.Controls.Add(btnRegistrar, 0, 10);

        var btnLimpiar = new Button
        {
            Text = "Limpiar campos",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 0, 8)
        };

        UiStyles.AplicarBotonSecundario(btnLimpiar);

        btnLimpiar.Click += (_, _) =>
        {
            txtMinutos.Clear();
            txtCosto.Clear();
            chkBidireccional.Checked = true;
            lblEstado.Text = "Formulario limpio";
        };

        contenido.Controls.Add(btnLimpiar, 0, 11);

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
            BackColor = Color.Transparent,
            Margin = Padding.Empty
        };

        contenido.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 58));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 73));

        contenido.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 62));

        var encabezado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent
        };

        encabezado.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        encabezado.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 165));

        encabezado.Controls.Add(
            Texto(
                "Conexiones registradas",
                15,
                TransRedTheme.Tinta,
                true),
            0, 0);

        encabezado.Controls.Add(
            lblCantidad,
            1, 0);

        var opciones = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };

        chkEliminarInverso.Dock = DockStyle.Left;
        opciones.Controls.Add(chkEliminarInverso);

        var acciones = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0, 9, 0, 0),
            BackColor = Color.Transparent
        };

        acciones.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        acciones.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 160));

        var btnEliminar = new Button
        {
            Text = "Eliminar conexión",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 12, 0),
            BackColor = Color.FromArgb(250, 236, 231),
            ForeColor = TransRedTheme.Error,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(
                "Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        btnEliminar.FlatAppearance.BorderSize = 0;
        btnEliminar.Click += BtnEliminar_Click;

        var btnActualizar = new Button
        {
            Text = "Actualizar",
            Dock = DockStyle.Fill
        };

        UiStyles.AplicarBotonSecundario(btnActualizar);

        btnActualizar.Click += (_, _) =>
        {
            ActualizarDatos();
            lblEstado.Text = "Listado actualizado";
        };

        acciones.Controls.Add(btnEliminar, 0, 0);
        acciones.Controls.Add(btnActualizar, 1, 0);

        contenido.Controls.Add(encabezado, 0, 0);
        contenido.Controls.Add(dgvConexiones, 0, 1);
        contenido.Controls.Add(opciones, 0, 2);
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
            Padding = padding,
            Margin = margen
        };
    }

    private static void AgregarCampo(
        TableLayoutPanel tabla,
        string nombre,
        Control control,
        int fila)
    {
        tabla.Controls.Add(
            Texto(
                nombre, 8,
                TransRedTheme.TextoSecundario,
                true),
            0, fila);

        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(0, 1, 0, 10);

        tabla.Controls.Add(control, 0, fila + 1);
    }

    private static Label Texto(
        string contenido,
        float tamano,
        Color color,
        bool negrita = false)
    {
        return new Label
        {
            Text = contenido,
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

    private void ActualizarDatos()
    {
        Parada[] paradas =
            gestor.ObtenerTodasParadas();

        string? codigoOrigen =
            (cmbOrigen.SelectedItem as Parada)?.Codigo;

        string? codigoDestino =
            (cmbDestino.SelectedItem as Parada)?.Codigo;

        cmbOrigen.Items.Clear();
        cmbDestino.Items.Clear();

        foreach (Parada parada in paradas)
        {
            cmbOrigen.Items.Add(parada);
            cmbDestino.Items.Add(parada);
        }

        SeleccionarParada(cmbOrigen, codigoOrigen);
        SeleccionarParada(cmbDestino, codigoDestino);

        dgvConexiones.Rows.Clear();

        Conexion[] conexiones =
            gestor.ObtenerTodasConexiones();

        foreach (Conexion conexion in conexiones)
        {
            int fila = dgvConexiones.Rows.Add(
                conexion.CodigoOrigen,
                conexion.CodigoDestino,
                conexion.Minutos.ToString("0.##"),
                conexion.Costo.ToString("0.##"));

            dgvConexiones.Rows[fila].Tag = conexion;
        }

        lblCantidad.Text =
            $"{conexiones.Length} conexiones";

        dgvConexiones.ClearSelection();
    }

    private static void SeleccionarParada(
        ComboBox combo, string? codigo)
    {
        if (combo.Items.Count == 0)
            return;

        if (!string.IsNullOrWhiteSpace(codigo))
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is Parada parada &&
                    parada.Codigo == codigo)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        combo.SelectedIndex = 0;
    }

    // Se conserva el registro existente del Core
    private void BtnRegistrar_Click(
        object? sender, EventArgs e)
    {
        if (cmbOrigen.SelectedItem is not Parada origen ||
            cmbDestino.SelectedItem is not Parada destino)
        {
            MessageBox.Show(
                "Primero debes registrar y seleccionar las paradas de origen y destino.",
                "Datos incompletos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (origen.Codigo == destino.Codigo)
        {
            MessageBox.Show(
                "La parada de origen y destino deben ser diferentes.",
                "Conexión no válida",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (!IntentarLeerNumero(
                txtMinutos.Text, out double minutos) ||
            !IntentarLeerNumero(
                txtCosto.Text, out double costo))
        {
            MessageBox.Show(
                "Ingresa valores numéricos válidos para el tiempo y el costo.",
                "Datos no válidos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (minutos < 0 || costo < 0 ||
            double.IsNaN(minutos) ||
            double.IsInfinity(minutos) ||
            double.IsNaN(costo) ||
            double.IsInfinity(costo))
        {
            MessageBox.Show(
                "El tiempo y el costo no pueden ser negativos ni valores infinitos.",
                "Datos no válidos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        bool registrada = gestor.RegistrarRuta(
            origen.Codigo,
            destino.Codigo,
            minutos,
            costo,
            chkBidireccional.Checked);

        if (!registrada)
        {
            MessageBox.Show(
                "No se pudo registrar la conexión. Puede que ya exista una conexión entre esas paradas.",
                "Registro no realizado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(
            "La conexión se registró correctamente.",
            "Registro exitoso",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        txtMinutos.Clear();
        txtCosto.Clear();

        ActualizarDatos();
    }

    // Se conserva la eliminación existente del Core
    private void BtnEliminar_Click(
        object? sender, EventArgs e)
    {
        if (dgvConexiones.SelectedRows.Count == 0)
        {
            MessageBox.Show(
                "Selecciona una conexión de la tabla.",
                "Sin selección",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (dgvConexiones.SelectedRows[0].Tag
            is not Conexion conexion)
            return;

        string mensaje = chkEliminarInverso.Checked
            ? $"¿Deseas eliminar la conexión {conexion.CodigoOrigen} → {conexion.CodigoDestino} y también el sentido inverso?"
            : $"¿Deseas eliminar únicamente la conexión {conexion.CodigoOrigen} → {conexion.CodigoDestino}?";

        DialogResult confirmacion = MessageBox.Show(
            mensaje,
            "Confirmar eliminación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes)
            return;

        bool eliminada = gestor.EliminarRuta(
            conexion.CodigoOrigen,
            conexion.CodigoDestino,
            chkEliminarInverso.Checked);

        if (!eliminada)
        {
            MessageBox.Show(
                "No se pudo eliminar la conexión.",
                "Operación no realizada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(
            "La conexión se eliminó correctamente.",
            "Eliminación exitosa",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        chkEliminarInverso.Checked = false;
        ActualizarDatos();
    }

    private static bool IntentarLeerNumero(
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
}
