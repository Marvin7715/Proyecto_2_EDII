using TransRed.Core.Servicios;
using TransRed.Interfaz.Design;
using TransRed.Interfaz.Layout;

namespace TransRed.Interfaz
{
    public partial class Form1 : Form
    {
        private readonly GestorTransporte gestorTransporte;

        public Form1()
        {
            InitializeComponent();

            gestorTransporte = new GestorTransporte();

            UiStyles.PrepararVentana(this);
            Text = "TransRed | Red de transporte";
            ClientSize = new Size(1100, 700);

            MainShell.Construir(this, gestorTransporte);
        }
    }
}