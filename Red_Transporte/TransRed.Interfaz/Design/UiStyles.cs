using System.Drawing;
using System.Windows.Forms;

namespace TransRed.Interfaz.Design;

// Estilos reutilizables para los controles de la interfaz
public static class UiStyles
{
    public static void PrepararVentana(Form ventana)
    {
        ventana.BackColor = TransRedTheme.Fondo;
        ventana.ForeColor = TransRedTheme.Tinta;
        ventana.Font = TransRedTheme.Texto;
        ventana.StartPosition = FormStartPosition.CenterScreen;
        ventana.MinimumSize = new Size(950, 620);
    }

    public static void AplicarTitulo(Label etiqueta)
    {
        etiqueta.Font = TransRedTheme.Titulo;
        etiqueta.ForeColor = TransRedTheme.Tinta;
        etiqueta.AutoSize = true;
    }

    public static void AplicarTextoSecundario(Label etiqueta)
    {
        etiqueta.Font = TransRedTheme.Texto;
        etiqueta.ForeColor = TransRedTheme.TextoSecundario;
        etiqueta.AutoSize = true;
    }

    public static void AplicarBotonPrincipal(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = TransRedTheme.AcentoHover;
        boton.BackColor = TransRedTheme.Acento;
        boton.ForeColor = Color.White;
        boton.Font = TransRedTheme.Texto;
        boton.Cursor = Cursors.Hand;
        boton.Padding = new Padding(TransRedTheme.Espacio4, TransRedTheme.Espacio2, TransRedTheme.Espacio4, TransRedTheme.Espacio2);
        boton.MinimumSize = new Size(115, 40);
    }

    public static void AplicarBotonSecundario(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderColor = TransRedTheme.Borde;
        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.MouseOverBackColor = TransRedTheme.AcentoSuave;
        boton.BackColor = TransRedTheme.Superficie;
        boton.ForeColor = TransRedTheme.Tinta;
        boton.Font = TransRedTheme.Texto;
        boton.Cursor = Cursors.Hand;
        boton.Padding = new Padding(TransRedTheme.Espacio4, TransRedTheme.Espacio2, TransRedTheme.Espacio4, TransRedTheme.Espacio2);
        boton.MinimumSize = new Size(115, 40);
    }

    public static void AplicarCampo(TextBox campo)
    {
        campo.BorderStyle = BorderStyle.FixedSingle;
        campo.BackColor = TransRedTheme.Superficie;
        campo.ForeColor = TransRedTheme.Tinta;
        campo.Font = TransRedTheme.Texto;
        campo.Margin = new Padding(0, TransRedTheme.Espacio2, 0, TransRedTheme.Espacio4);
    }
}