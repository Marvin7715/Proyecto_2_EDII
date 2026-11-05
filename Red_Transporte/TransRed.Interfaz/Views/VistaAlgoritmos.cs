
using TransRed.Core.Modelos;
using TransRed.Core.Servicios;

namespace TransRed.Interfaz.Views
{
    public class VistaAlgoritmos : UserControl
    {
        private readonly IGestorTransporte gestor;

        private readonly ComboBox cmbInicioBFS;
        private readonly ComboBox cmbInicioDFS;
        private readonly ComboBox cmbOrigen;
        private readonly ComboBox cmbDestino;

        private readonly Label lblResultadoBFS;
        private readonly Label lblResultadoDFS;
        private readonly Label lblResultadoConectividad;

        public VistaAlgoritmos(IGestorTransporte gestor)
        {
            this.gestor = gestor;

            Dock = DockStyle.Fill;
            BackColor = Color.White;
            Padding = new Padding(20);

            Label lblTitulo = new Label
            {
                Text = "Algoritmos de red",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 45, 60),
                Dock = DockStyle.Top,
                Height = 45
            };

            Label lblDescripcion = new Label
            {
                Text = "Explora la red mediante recorridos y comprueba la conectividad entre paradas.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 35
            };

            FlowLayoutPanel panelPrincipal = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            GroupBox grpBFS = CrearGrupo(
                "BFS - Búsqueda en anchura");

            cmbInicioBFS = CrearCombo();

            Button btnBFS = CrearBoton(
                "Ejecutar BFS",
                Color.FromArgb(35, 100, 160));

            lblResultadoBFS = CrearResultado();

            btnBFS.Click += BtnBFS_Click;

            grpBFS.Controls.Add(
                CrearPanelAlgoritmo(
                    "Parada inicial",
                    cmbInicioBFS,
                    btnBFS,
                    lblResultadoBFS));

            GroupBox grpDFS = CrearGrupo(
                "DFS - Búsqueda en profundidad");

            cmbInicioDFS = CrearCombo();

            Button btnDFS = CrearBoton(
                "Ejecutar DFS",
                Color.FromArgb(45, 130, 90));

            lblResultadoDFS = CrearResultado();

            btnDFS.Click += BtnDFS_Click;

            grpDFS.Controls.Add(
                CrearPanelAlgoritmo(
                    "Parada inicial",
                    cmbInicioDFS,
                    btnDFS,
                    lblResultadoDFS));

            GroupBox grpUnionFind = CrearGrupo(
                "Union-Find - Comprobar conectividad");

            cmbOrigen = CrearCombo();
            cmbDestino = CrearCombo();

            Button btnConectividad = CrearBoton(
                "Comprobar conectividad",
                Color.FromArgb(120, 85, 155));

            lblResultadoConectividad = CrearResultado();

            btnConectividad.Click += BtnConectividad_Click;

            FlowLayoutPanel panelConectividad = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10),
                AutoScroll = true
            };

            panelConectividad.Controls.Add(
                CrearCampo("Parada de origen", cmbOrigen));

            panelConectividad.Controls.Add(
                CrearCampo("Parada de destino", cmbDestino));

            panelConectividad.Controls.Add(btnConectividad);
            panelConectividad.Controls.Add(lblResultadoConectividad);

            grpUnionFind.Controls.Add(panelConectividad);

            panelPrincipal.Controls.Add(grpBFS);
            panelPrincipal.Controls.Add(grpDFS);
            panelPrincipal.Controls.Add(grpUnionFind);

            Controls.Add(panelPrincipal);
            Controls.Add(lblDescripcion);
            Controls.Add(lblTitulo);

            ActualizarParadas();
        }

        private GroupBox CrearGrupo(string titulo)
        {
            return new GroupBox
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 45, 60),
                Width = 950,
                Height = 175,
                Padding = new Padding(12),
                Margin = new Padding(5, 5, 5, 15)
            };
        }

        private ComboBox CrearCombo()
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 220,
                Font = new Font("Segoe UI", 10)
            };
        }

        private Button CrearBoton(string texto, Color color)
        {
            Button boton = new Button
            {
                Text = texto,
                Width = 180,
                Height = 35,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(8, 20, 8, 5)
            };

            boton.FlatAppearance.BorderSize = 0;

            return boton;
        }

        private Label CrearResultado()
        {
            return new Label
            {
                Text = "Resultado: pendiente",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(45, 55, 70),
                AutoSize = true,
                MaximumSize = new Size(850, 0),
                Margin = new Padding(8, 12, 8, 8)
            };
        }

        private Control CrearCampo(string titulo, Control control)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Width = control.Width + 15,
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

        private FlowLayoutPanel CrearPanelAlgoritmo(
            string titulo,
            ComboBox combo,
            Button boton,
            Label resultado)
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10),
                AutoScroll = true
            };

            panel.Controls.Add(CrearCampo(titulo, combo));
            panel.Controls.Add(boton);
            panel.Controls.Add(resultado);

            return panel;
        }

        private void ActualizarParadas()
        {
            string? codigoBFS =
                (cmbInicioBFS.SelectedItem as Parada)?.Codigo;

            string? codigoDFS =
                (cmbInicioDFS.SelectedItem as Parada)?.Codigo;

            string? codigoOrigen =
                (cmbOrigen.SelectedItem as Parada)?.Codigo;

            string? codigoDestino =
                (cmbDestino.SelectedItem as Parada)?.Codigo;

            Parada[] paradas = gestor.ObtenerTodasParadas();

            cmbInicioBFS.Items.Clear();
            cmbInicioDFS.Items.Clear();
            cmbOrigen.Items.Clear();
            cmbDestino.Items.Clear();

            foreach (Parada parada in paradas)
            {
                cmbInicioBFS.Items.Add(parada);
                cmbInicioDFS.Items.Add(parada);
                cmbOrigen.Items.Add(parada);
                cmbDestino.Items.Add(parada);
            }

            SeleccionarParada(cmbInicioBFS, codigoBFS);
            SeleccionarParada(cmbInicioDFS, codigoDFS);
            SeleccionarParada(cmbOrigen, codigoOrigen);
            SeleccionarParada(cmbDestino, codigoDestino);
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

        private void BtnBFS_Click(object? sender, EventArgs e)
        {
            if (cmbInicioBFS.SelectedItem is not Parada inicio)
            {
                MessageBox.Show(
                    "Registra y selecciona una parada inicial.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            Parada[] resultado = gestor.RecorrerBFS(inicio.Codigo);

            MostrarRecorrido(
                resultado,
                lblResultadoBFS,
                "BFS");
        }

        private void BtnDFS_Click(object? sender, EventArgs e)
        {
            if (cmbInicioDFS.SelectedItem is not Parada inicio)
            {
                MessageBox.Show(
                    "Registra y selecciona una parada inicial.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            Parada[] resultado = gestor.RecorrerDFS(inicio.Codigo);

            MostrarRecorrido(
                resultado,
                lblResultadoDFS,
                "DFS");
        }

        private void MostrarRecorrido(
            Parada[] resultado,
            Label etiqueta,
            string algoritmo)
        {
            if (resultado.Length == 0)
            {
                etiqueta.Text = "Resultado: No hay paradas para recorrer.";
                return;
            }

            string[] nombres = new string[resultado.Length];

            for (int i = 0; i < resultado.Length; i++)
            {
                nombres[i] =
                    resultado[i].Codigo +
                    " - " +
                    resultado[i].Nombre;
            }

            etiqueta.Text =
                "Resultado (" + algoritmo + "): " +
                string.Join(" → ", nombres);
        }

        private void BtnConectividad_Click(
            object? sender,
            EventArgs e)
        {
            if (cmbOrigen.SelectedItem is not Parada origen ||
                cmbDestino.SelectedItem is not Parada destino)
            {
                MessageBox.Show(
                    "Selecciona las dos paradas que deseas comparar.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (origen.Codigo == destino.Codigo)
            {
                lblResultadoConectividad.Text =
                    "Resultado: Es la misma parada.";

                return;
            }

            bool conectadas = gestor.EstanConectadas(
                origen.Codigo,
                destino.Codigo);

            if (conectadas)
            {
                lblResultadoConectividad.Text =
                    $"Resultado: {origen.Codigo} y {destino.Codigo} " +
                    "pertenecen a la misma componente conectada.";
            }
            else
            {
                lblResultadoConectividad.Text =
                    $"Resultado: {origen.Codigo} y {destino.Codigo} " +
                    "no pertenecen a la misma componente conectada.";
            }
        }
    }
}
