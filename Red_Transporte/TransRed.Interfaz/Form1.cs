using TransRed.Interfaz.Design;
using TransRed.Interfaz.Layout;

namespace TransRed.Interfaz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Tema base definido en el commit 1.1
            UiStyles.PrepararVentana(this);
            Text = "TransRed | Red de transporte";
            ClientSize = new Size(1100, 700);

            // Distribución general de la interfaz
            MainShell.Construir(this);
        }
    }
}