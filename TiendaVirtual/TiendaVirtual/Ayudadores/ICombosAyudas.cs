using Microsoft.AspNetCore.Mvc.Rendering;

namespace TiendaVirtual.Ayudadores
{
    public interface ICombosAyudas
    {
        Task<IEnumerable<SelectListItem>> GetComboCategoriasAsync();

        Task<IEnumerable<SelectListItem>> GetComboEstadosAsync();

        Task<IEnumerable<SelectListItem>> GetComboCiudadAsync(int estadoId);

    }
}
