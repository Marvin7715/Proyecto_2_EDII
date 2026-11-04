using System.Globalization;
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;

namespace TransRed.Interfaz.Views
{
    public class VistaConexiones : UserControl
    {
        private readonly IGestorTransporte gestor;

        private readonly ComboBox cmbOrigen;
        private readonly ComboBox cmbDestino;
        private readonly TextBox txtMinutos;
        private readonly TextBox txtCosto;
        private readonly CheckBox chkBidireccional;
        private readonly CheckBox chkEliminarInverso;
        private readonly DataGridView dgvConexiones;

        public VistaConexiones(IGestorTransporte gestor)
        {
            this.gestor = gestor;

            Dock = DockStyle.Fill;
            BackColor = Color.White;
            Padding = new Padding(20);

            Label lblTitulo = new Label
            {
                Text = "Gestión de conexiones",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 45, 60),
                Dock = DockStyle.Top,
                Height = 45
            };

            Label lblDescripcion = new Label
            {
                Text = "Registra, consulta y administra las conexiones entre las paradas de la red.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 35
            };

            GroupBox grpRegistro = new GroupBox
            {
                Text = "Registrar nueva conexión",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 190,
                Padding = new Padding(12)
            };

            cmbOrigen = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 190,
                Font = new Font("Segoe UI", 10)
            };

            cmbDestino = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 190,
                Font = new Font("Segoe UI", 10)
            };

            txtMinutos = new TextBox
            {
                Width = 100,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Minutos"
            };

            txtCosto = new TextBox
            {
                Width = 100,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Costo"
            };

            chkBidireccional = new CheckBox
            {
                Text = "Conexión bidireccional",
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Checked = true
            };

            Button btnRegistrar = new Button
            {
                Text = "Registrar conexión",
                Width = 155,
                Height = 35,
                BackColor = Color.FromArgb(35, 100, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnRegistrar.FlatAppearance.BorderSize = 0;
            btnRegistrar.Click += BtnRegistrar_Click;

            FlowLayoutPanel panelRegistro = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(5, 12, 5, 5),
                AutoScroll = true
            };

            panelRegistro.Controls.Add(CrearCampo("Parada de origen", cmbOrigen));
            panelRegistro.Controls.Add(CrearCampo("Parada de destino", cmbDestino));
            panelRegistro.Controls.Add(CrearCampo("Tiempo (minutos)", txtMinutos));
            panelRegistro.Controls.Add(CrearCampo("Costo (Q)", txtCosto));
            panelRegistro.Controls.Add(CrearCampo("Tipo de conexión", chkBidireccional));
            panelRegistro.Controls.Add(btnRegistrar);

            grpRegistro.Controls.Add(panelRegistro);

            dgvConexiones = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 }
            };

            dgvConexiones.EnableHeadersVisualStyles = false;
            dgvConexiones.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(35, 45, 60);
            dgvConexiones.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvConexiones.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvConexiones.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(210, 230, 245);
            dgvConexiones.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvConexiones.Columns.Add("Origen", "Parada de origen");
            dgvConexiones.Columns.Add("Destino", "Parada de destino");
            dgvConexiones.Columns.Add("Minutos", "Tiempo (minutos)");
            dgvConexiones.Columns.Add("Costo", "Costo (Q)");

            chkEliminarInverso = new CheckBox
            {
                Text = "Eliminar también el sentido inverso",
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            Button btnEliminar = new Button
            {
                Text = "Eliminar conexión seleccionada",
                Width = 220,
                Height = 35,
                BackColor = Color.FromArgb(180, 55, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += BtnEliminar_Click;

            Button btnActualizar = new Button
            {
                Text = "Actualizar lista",
                Width = 130,
                Height = 35,
                BackColor = Color.FromArgb(70, 80, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnActualizar.FlatAppearance.BorderSize = 0;
            btnActualizar.Click += (sender, e) => ActualizarDatos();

            FlowLayoutPanel panelAcciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0)
            };

            panelAcciones.Controls.Add(btnEliminar);
            panelAcciones.Controls.Add(chkEliminarInverso);
            panelAcciones.Controls.Add(btnActualizar);

            Panel panelTabla = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            Label lblConexiones = new Label
            {
                Text = "Conexiones registradas",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 45, 60),
                Dock = DockStyle.Top,
                Height = 35
            };

            panelTabla.Controls.Add(dgvConexiones);
            panelTabla.Controls.Add(panelAcciones);
            panelTabla.Controls.Add(lblConexiones);

            Controls.Add(panelTabla);
            Controls.Add(grpRegistro);
            Controls.Add(lblDescripcion);
            Controls.Add(lblTitulo);

            ActualizarDatos();
        }

        private Control CrearCampo(string titulo, Control control)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Width = control.Width + 10,
                Height = 65,
                WrapContents = false,
                Margin = new Padding(5)
            };

            Label etiqueta = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 80),
                AutoSize = true,
                Height = 23
            };

            panel.Controls.Add(etiqueta);
            panel.Controls.Add(control);

            return panel;
        }

        private void ActualizarDatos()
        {
            Parada[] paradas = gestor.ObtenerTodasParadas();

            string? codigoOrigenSeleccionado =
                (cmbOrigen.SelectedItem as Parada)?.Codigo;

            string? codigoDestinoSeleccionado =
                (cmbDestino.SelectedItem as Parada)?.Codigo;

            cmbOrigen.Items.Clear();
            cmbDestino.Items.Clear();

            foreach (Parada parada in paradas)
            {
                cmbOrigen.Items.Add(parada);
                cmbDestino.Items.Add(parada);
            }

            SeleccionarParada(cmbOrigen, codigoOrigenSeleccionado);
            SeleccionarParada(cmbDestino, codigoDestinoSeleccionado);

            dgvConexiones.Rows.Clear();

            Conexion[] conexiones = gestor.ObtenerTodasConexiones();

            foreach (Conexion conexion in conexiones)
            {
                int fila = dgvConexiones.Rows.Add(
                    conexion.CodigoOrigen,
                    conexion.CodigoDestino,
                    conexion.Minutos.ToString("0.##"),
                    conexion.Costo.ToString("0.##"));

                dgvConexiones.Rows[fila].Tag = conexion;
            }
        }

        private void SeleccionarParada(ComboBox combo, string? codigo)
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

        private void BtnRegistrar_Click(object? sender, EventArgs e)
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

            if (!IntentarLeerNumero(txtMinutos.Text, out double minutos) ||
                !IntentarLeerNumero(txtCosto.Text, out double costo))
            {
                MessageBox.Show(
                    "Ingresa valores numéricos válidos para el tiempo y el costo.",
                    "Datos no válidos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (minutos < 0 || costo < 0 ||
                double.IsNaN(minutos) || double.IsInfinity(minutos) ||
                double.IsNaN(costo) || double.IsInfinity(costo))
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

        private void BtnEliminar_Click(object? sender, EventArgs e)
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

            if (dgvConexiones.SelectedRows[0].Tag is not Conexion conexion)
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

        private bool IntentarLeerNumero(string texto, out double numero)
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
}