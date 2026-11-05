
using System.Drawing;

namespace TransRed.Interfaz.Design;

// Identidad visual de TransRed Mobility Studio
public static class TransRedTheme
{
    // Fondos y superficies
    public static readonly Color Fondo =
        Color.FromArgb(246, 244, 238);

    public static readonly Color Superficie =
        Color.White;

    public static readonly Color SuperficieAlterna =
        Color.FromArgb(252, 250, 246);

    public static readonly Color FondoMapa =
        Color.FromArgb(238, 243, 235);

    // Textos
    public static readonly Color Tinta =
        Color.FromArgb(29, 59, 48);

    public static readonly Color TextoSecundario =
        Color.FromArgb(103, 120, 110);

    public static readonly Color TextoTenue =
        Color.FromArgb(135, 148, 137);

    // Identidad principal
    public static readonly Color Acento =
        Color.FromArgb(43, 112, 91);

    public static readonly Color AcentoHover =
        Color.FromArgb(33, 91, 73);

    public static readonly Color AcentoPresionado =
        Color.FromArgb(24, 72, 58);

    public static readonly Color AcentoSuave =
        Color.FromArgb(228, 240, 230);

    public static readonly Color Bosque =
        Color.FromArgb(25, 72, 56);

    public static readonly Color Salvia =
        Color.FromArgb(170, 193, 169);

    public static readonly Color Cobre =
        Color.FromArgb(190, 126, 70);

    // Estados
    public static readonly Color Advertencia =
        Color.FromArgb(170, 111, 53);

    public static readonly Color Error =
        Color.FromArgb(166, 72, 67);

    public static readonly Color Exito =
        Color.FromArgb(54, 125, 91);

    // Bordes y separadores
    public static readonly Color Borde =
        Color.FromArgb(225, 229, 220);

    public static readonly Color BordeSuave =
        Color.FromArgb(235, 237, 230);

    public static readonly Color Cuadricula =
        Color.FromArgb(220, 230, 218);

    // Espaciado
    public const int Espacio1 = 4;
    public const int Espacio2 = 8;
    public const int Espacio3 = 12;
    public const int Espacio4 = 16;
    public const int Espacio5 = 20;
    public const int Espacio6 = 24;
    public const int Espacio8 = 32;
    public const int Espacio10 = 40;

    // Fuentes del sistema
    public static Font Texto =>
        new Font("Segoe UI", 10f);

    public static Font TextoPequeno =>
        new Font("Segoe UI", 9f);

    public static Font Subtitulo =>
        new Font("Segoe UI", 12f, FontStyle.Bold);

    public static Font Titulo =>
        new Font("Segoe UI", 22f, FontStyle.Bold);

    public static Font TituloGrande =>
        new Font("Segoe UI", 26f, FontStyle.Bold);

    public static Font Etiqueta =>
        new Font("Segoe UI", 8.5f, FontStyle.Bold);
}
