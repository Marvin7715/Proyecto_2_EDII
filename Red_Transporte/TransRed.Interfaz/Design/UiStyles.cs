
using System.Drawing;
using System.Windows.Forms;

namespace TransRed.Interfaz.Design;

// Apariencia compartida de controles WinForms
public static class UiStyles
{
    public static void PrepararVentana(Form ventana)
    {
        ventana.BackColor = TransRedTheme.Fondo;
        ventana.ForeColor = TransRedTheme.Tinta;
        ventana.Font = TransRedTheme.Texto;
        ventana.StartPosition =
            FormStartPosition.CenterScreen;
        ventana.MinimumSize = new Size(1080, 680);
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
        etiqueta.ForeColor =
            TransRedTheme.TextoSecundario;
        etiqueta.AutoSize = true;
    }

    public static void AplicarBotonPrincipal(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor =
            TransRedTheme.AcentoHover;
        boton.FlatAppearance.MouseDownBackColor =
            TransRedTheme.AcentoPresionado;

        boton.UseVisualStyleBackColor = false;
        boton.BackColor = TransRedTheme.Acento;
        boton.ForeColor = Color.White;
        boton.Font = new Font(
            "Segoe UI", 9.5f, FontStyle.Bold);
        boton.Cursor = Cursors.Hand;
        boton.Padding = new Padding(12, 7, 12, 7);
        boton.MinimumSize = new Size(120, 42);
    }

    public static void AplicarBotonSecundario(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.BorderColor =
            TransRedTheme.Borde;
        boton.FlatAppearance.MouseOverBackColor =
            TransRedTheme.AcentoSuave;

        boton.UseVisualStyleBackColor = false;
        boton.BackColor = TransRedTheme.Superficie;
        boton.ForeColor = TransRedTheme.Tinta;
        boton.Font = new Font("Segoe UI", 9.5f);
        boton.Cursor = Cursors.Hand;
        boton.Padding = new Padding(12, 7, 12, 7);
        boton.MinimumSize = new Size(120, 42);
    }

    public static void AplicarCampo(TextBox campo)
    {
        campo.BorderStyle = BorderStyle.FixedSingle;
        campo.BackColor = TransRedTheme.Superficie;
        campo.ForeColor = TransRedTheme.Tinta;
        campo.Font = TransRedTheme.Texto;
        campo.Margin = new Padding(0, 6, 0, 12);
    }

    public static void AplicarCombo(ComboBox combo)
    {
        combo.DropDownStyle = ComboBoxStyle.DropDownList;
        combo.FlatStyle = FlatStyle.Flat;
        combo.BackColor = TransRedTheme.Superficie;
        combo.ForeColor = TransRedTheme.Tinta;
        combo.Font = TransRedTheme.Texto;
    }

    public static void AplicarTabla(DataGridView tabla)
    {
        tabla.BackgroundColor = TransRedTheme.Superficie;
        tabla.BorderStyle = BorderStyle.None;
        tabla.EnableHeadersVisualStyles = false;
        tabla.RowHeadersVisible = false;
        tabla.AllowUserToAddRows = false;
        tabla.AllowUserToDeleteRows = false;
        tabla.ReadOnly = true;
        tabla.MultiSelect = false;
        tabla.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        tabla.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        tabla.ColumnHeadersHeight = 42;
        tabla.RowTemplate.Height = 38;
        tabla.GridColor = TransRedTheme.BordeSuave;

        tabla.ColumnHeadersDefaultCellStyle.BackColor =
            TransRedTheme.SuperficieAlterna;
        tabla.ColumnHeadersDefaultCellStyle.ForeColor =
            TransRedTheme.Tinta;
        tabla.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 9f, FontStyle.Bold);

        tabla.DefaultCellStyle.BackColor =
            TransRedTheme.Superficie;
        tabla.DefaultCellStyle.ForeColor =
            TransRedTheme.Tinta;
        tabla.DefaultCellStyle.SelectionBackColor =
            TransRedTheme.AcentoSuave;
        tabla.DefaultCellStyle.SelectionForeColor =
            TransRedTheme.Tinta;

        tabla.AlternatingRowsDefaultCellStyle.BackColor =
            TransRedTheme.SuperficieAlterna;
    }
}
