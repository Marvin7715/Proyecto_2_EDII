
using System.Drawing;
using System.Windows.Forms;

namespace TransRed.Interfaz.Design;

// Estilos reutilizables de la interfaz TransRed
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
        boton.TextAlign = ContentAlignment.MiddleCenter;
        boton.Padding = new Padding(6, 3, 6, 3);
        boton.MinimumSize = new Size(90, 36);
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

        boton.Font = new Font(
            "Segoe UI", 9.5f);

        boton.Cursor = Cursors.Hand;
        boton.TextAlign = ContentAlignment.MiddleCenter;
        boton.Padding = new Padding(5, 2, 5, 2);
        boton.MinimumSize = new Size(90, 36);
    }

    public static void AplicarCampo(TextBox campo)
    {
        campo.BorderStyle = BorderStyle.FixedSingle;
        campo.BackColor = TransRedTheme.Superficie;
        campo.ForeColor = TransRedTheme.Tinta;
        campo.Font = TransRedTheme.Texto;

        campo.Margin = new Padding(0, 5, 0, 9);
    }

    public static void AplicarCombo(ComboBox combo)
    {
        combo.DropDownStyle =
            ComboBoxStyle.DropDownList;

        combo.FlatStyle = FlatStyle.Flat;
        combo.BackColor = TransRedTheme.Superficie;
        combo.ForeColor = TransRedTheme.Tinta;
        combo.Font = TransRedTheme.Texto;

        combo.IntegralHeight = false;
        combo.DropDownHeight = 280;
    }

    public static void AplicarTabla(DataGridView tabla)
    {
        tabla.Dock = DockStyle.Fill;

        tabla.BackgroundColor = TransRedTheme.Superficie;
        tabla.BorderStyle = BorderStyle.None;

        tabla.EnableHeadersVisualStyles = false;
        tabla.RowHeadersVisible = false;

        tabla.AllowUserToAddRows = false;
        tabla.AllowUserToDeleteRows = false;
        tabla.AllowUserToResizeRows = false;

        tabla.ReadOnly = true;
        tabla.MultiSelect = false;

        tabla.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;

        tabla.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        tabla.ColumnHeadersHeight = 43;
        tabla.RowTemplate.Height = 36;
        tabla.GridColor = TransRedTheme.BordeSuave;

        tabla.ColumnHeadersDefaultCellStyle.BackColor =
            TransRedTheme.SuperficieAlterna;

        tabla.ColumnHeadersDefaultCellStyle.ForeColor =
            TransRedTheme.Tinta;

        tabla.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 9, FontStyle.Bold);

        tabla.ColumnHeadersDefaultCellStyle.SelectionBackColor =
            TransRedTheme.SuperficieAlterna;

        tabla.ColumnHeadersDefaultCellStyle.SelectionForeColor =
            TransRedTheme.Tinta;

        tabla.DefaultCellStyle.BackColor =
            TransRedTheme.Superficie;

        tabla.DefaultCellStyle.ForeColor =
            TransRedTheme.Tinta;

        tabla.DefaultCellStyle.Font =
            new Font("Segoe UI", 9.5f);

        tabla.DefaultCellStyle.SelectionBackColor =
            TransRedTheme.AcentoSuave;

        tabla.DefaultCellStyle.SelectionForeColor =
            TransRedTheme.Tinta;

        tabla.AlternatingRowsDefaultCellStyle.BackColor =
            TransRedTheme.SuperficieAlterna;

        tabla.AlternatingRowsDefaultCellStyle.ForeColor =
            TransRedTheme.Tinta;

        tabla.AlternatingRowsDefaultCellStyle.SelectionBackColor =
            TransRedTheme.AcentoSuave;

        tabla.AlternatingRowsDefaultCellStyle.SelectionForeColor =
            TransRedTheme.Tinta;

        tabla.DefaultCellStyle.Padding =
            new Padding(6, 3, 6, 3);

        tabla.CellBorderStyle =
            DataGridViewCellBorderStyle.SingleHorizontal;

        tabla.ColumnHeadersBorderStyle =
            DataGridViewHeaderBorderStyle.Single;
    }
}
