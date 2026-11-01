using System.Drawing;

namespace TransRed.Interfaz.Design;

// Colores, medidas y fuentes de TransRed
public static class TransRedTheme
{
    // Colores base
    public static readonly Color Fondo = Color.FromArgb(247, 248, 245);
    public static readonly Color Superficie = Color.White;
    public static readonly Color Tinta = Color.FromArgb(27, 46, 53);
    public static readonly Color TextoSecundario = Color.FromArgb(100, 117, 122);
    public static readonly Color Borde = Color.FromArgb(220, 228, 225);

    // Colores de interacción
    public static readonly Color Acento = Color.FromArgb(35, 125, 116);
    public static readonly Color AcentoHover = Color.FromArgb(27, 101, 94);
    public static readonly Color AcentoSuave = Color.FromArgb(230, 243, 239);
    public static readonly Color Advertencia = Color.FromArgb(173, 107, 42);
    public static readonly Color Error = Color.FromArgb(171, 72, 65);

    // Escala de espacios en píxeles
    public const int Espacio1 = 4;
    public const int Espacio2 = 8;
    public const int Espacio3 = 12;
    public const int Espacio4 = 16;
    public const int Espacio6 = 24;
    public const int Espacio8 = 32;

    // Fuentes del sistema para no depender de archivos externos
    public static Font Texto => new Font("Segoe UI", 10f, FontStyle.Regular);
    public static Font TextoPequeno => new Font("Segoe UI", 9f, FontStyle.Regular);
    public static Font Subtitulo => new Font("Segoe UI Semibold", 12f, FontStyle.Bold);
    public static Font Titulo => new Font("Segoe UI Semibold", 22f, FontStyle.Bold);
}