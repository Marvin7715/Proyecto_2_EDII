using TransRed.Core.Servicios;
using TransRed.Core.Servicios.Persistencia;
using TransRed.Interfaz.Design;
using TransRed.Interfaz.Layout;

namespace TransRed.Interfaz
{
    public partial class Form1 : Form
    {
        private readonly GestorTransportePersistente gestorTransporte;

        public Form1()
        {
            InitializeComponent();

            string rutaDatos = RutasTransRed.ObtenerArchivoPrincipal();

            gestorTransporte = new GestorTransportePersistente(rutaDatos);

            UiStyles.PrepararVentana(this);
            Text = "TransRed | Red de transporte";
            ClientSize = new Size(1100, 700);

            MainShell.Construir(this, gestorTransporte);
        }
    }
}