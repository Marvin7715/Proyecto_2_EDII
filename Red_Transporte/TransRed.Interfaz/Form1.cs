using TransRed.Interfaz.Design;

namespace TransRed.Interfaz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Aplicamos la identidad base a la ventana
            UiStyles.PrepararVentana(this);
            Text = "TransRed | Red de transporte";
            ClientSize = new Size(1100, 700);
        }
    }
}