using TransRed.Core.Modelos;
using TransRed.Core.Servicios;

namespace TransRed.Interfaz.Views
{
    public class VistaPlanificar : UserControl
    {
        private readonly IGestorTransporte gestor;

        private readonly ComboBox cmbOrigen;
        private readonly ComboBox cmbDestino;
        private readonly Label lblTipoRuta;
        private readonly Label lblTiempo;
        private readonly Label lblCosto;
        private readonly Label lblRecorrido;
        private readonly Panel panelResultado;

        public VistaPlanificar(IGestorTransporte gestor)
        {
            this.gestor = gestor;

            Dock = DockStyle.Fill;
            BackColor = Color.White;
            Padding = new Padding(20);

            Label lblTitulo = new Label
            {
                Text = "Planificar ruta",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 45, 60),
                Dock = DockStyle.Top,
                Height = 45
            };

            Label lblDescripcion = new Label
            {
                Text = "Encuentra la ruta más rápida o la ruta más económica entre dos paradas.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 35
            };

            GroupBox grpPlanificacion = new GroupBox
            {
                Text = "Seleccionar recorrido",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 145,
                Padding = new Padding(12)
            };

            cmbOrigen = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 230,
                Font = new Font("Segoe UI", 10)
            };

            cmbDestino = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 230,
                Font = new Font("Segoe UI", 10)
            };

            Button btnRutaRapida = new Button
            {
                Text = "Ruta más rápida",
                Width = 145,
                Height = 35,
                BackColor = Color.FromArgb(35, 100, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnRutaRapida.FlatAppearance.BorderSize = 0;
            btnRutaRapida.Click += BtnRutaRapida_Click;

            Button btnRutaEconomica = new Button
            {
                Text = "Ruta más económica",
                Width = 165,
                Height = 35,
                BackColor = Color.FromArgb(45, 130, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnRutaEconomica.FlatAppearance.BorderSize = 0;
            btnRutaEconomica.Click += BtnRutaEconomica_Click;

            FlowLayoutPanel panelSeleccion = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(5, 10, 5, 5),
                AutoScroll = true
            };

            panelSeleccion.Controls.Add(
                CrearCampo("Parada de origen", cmbOrigen));

            panelSeleccion.Controls.Add(
                CrearCampo("Parada de destino", cmbDestino));

            panelSeleccion.Controls.Add(btnRutaRapida);
            panelSeleccion.Controls.Add(btnRutaEconomica);

            grpPlanificacion.Controls.Add(panelSeleccion);

            panelResultado = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 15, 0, 0)
            };

            GroupBox grpResultado = new GroupBox
            {
                Text = "Resultado de la planificación",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            lblTipoRuta = CrearLabelResultado(
                "Tipo de ruta: -",
                16,
                true);

            lblTiempo = CrearLabelResultado(
                "Tiempo total: -",
                12,
                false);

            lblCosto = CrearLabelResultado(
                "Costo total: -",
                12,
                false);

            lblRecorrido = CrearLabelResultado(
                "Recorrido: -",
                12,
                false);

            FlowLayoutPanel panelInformacion = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            panelInformacion.Controls.Add(lblTipoRuta);
            panelInformacion.Controls.Add(lblTiempo);
            panelInformacion.Controls.Add(lblCosto);
            panelInformacion.Controls.Add(lblRecorrido);

            grpResultado.Controls.Add(panelInformacion);

            panelResultado.Controls.Add(grpResultado);

            Controls.Add(panelResultado);
            Controls.Add(grpPlanificacion);
            Controls.Add(lblDescripcion);
            Controls.Add(lblTitulo);

            ActualizarParadas();
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

        private Label CrearLabelResultado(
            string texto,
            int tamano,
            bool negrita)
        {
            return new Label
            {
                Text = texto,
                Font = new Font(
                    "Segoe UI",
                    tamano,
                    negrita ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(45, 55, 70),
                AutoSize = true,
                MaximumSize = new Size(900, 0),
                Margin = new Padding(5, 8, 5, 8)
            };
        }

        private void ActualizarParadas()
        {
            string? origenActual =
                (cmbOrigen.SelectedItem as Parada)?.Codigo;

            string? destinoActual =
                (cmbDestino.SelectedItem as Parada)?.Codigo;

            cmbOrigen.Items.Clear();
            cmbDestino.Items.Clear();

            Parada[] paradas = gestor.ObtenerTodasParadas();

            foreach (Parada parada in paradas)
            {
                cmbOrigen.Items.Add(parada);
                cmbDestino.Items.Add(parada);
            }

            SeleccionarParada(cmbOrigen, origenActual);
            SeleccionarParada(cmbDestino, destinoActual);
        }

        private void SeleccionarParada(
            ComboBox combo,
            string? codigo)
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

        private void BtnRutaRapida_Click(
            object? sender,
            EventArgs e)
        {
            CalcularRuta(false);
        }

        private void BtnRutaEconomica_Click(
            object? sender,
            EventArgs e)
        {
            CalcularRuta(true);
        }

        private void CalcularRuta(bool usarCosto)
        {
            if (cmbOrigen.SelectedItem is not Parada origen ||
                cmbDestino.SelectedItem is not Parada destino)
            {
                MessageBox.Show(
                    "Debes seleccionar una parada de origen y una de destino.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (origen.Codigo == destino.Codigo)
            {
                MessageBox.Show(
                    "La parada de origen y destino deben ser diferentes.",
                    "Ruta no válida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            ResultadoRuta? resultado =
                gestor.ObtenerResultadoRuta(
                    origen.Codigo,
                    destino.Codigo,
                    usarCosto);

            if (resultado == null ||
                resultado.Paradas == null ||
                resultado.Paradas.Length == 0)
            {
                lblTipoRuta.Text = "Tipo de ruta: No encontrada";
                lblTiempo.Text = "Tiempo total: -";
                lblCosto.Text = "Costo total: -";
                lblRecorrido.Text = "Recorrido: No existe una ruta disponible.";

                MessageBox.Show(
                    "No existe una ruta disponible entre las paradas seleccionadas.",
                    "Ruta no encontrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            lblTipoRuta.Text = usarCosto
                ? "Tipo de ruta: Más económica"
                : "Tipo de ruta: Más rápida";

            lblTiempo.Text =
                $"Tiempo total: {resultado.TiempoTotal:0.##} minutos";

            lblCosto.Text =
                $"Costo total: Q {resultado.CostoTotal:0.##}";

            string[] nombres = new string[resultado.Paradas.Length];

            for (int i = 0; i < resultado.Paradas.Length; i++)
            {
                nombres[i] =
                    resultado.Paradas[i].Codigo +
                    " - " +
                    resultado.Paradas[i].Nombre;
            }

            lblRecorrido.Text =
                "Recorrido: " +
                string.Join(" → ", nombres);
        }
    }
}