
using TransRed.Core.Servicios;

namespace TransRed.Interfaz.Views;

// Reutiliza el planificador y mapa de la pantalla principal
// Los cálculos siguen siendo responsabilidad del Core
public class VistaPlanificar : ExploradorPrincipal
{
    public VistaPlanificar(IGestorTransporte gestor)
        : base(gestor)
    {
    }
}
