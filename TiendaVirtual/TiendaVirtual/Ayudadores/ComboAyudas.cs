using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Data;

namespace TiendaVirtual.Ayudadores
{
    public class ComboAyudas : ICombosAyudas
    {
        private readonly DatosTienda _context;

        public ComboAyudas(DatosTienda context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SelectListItem>> GetComboCategoriasAsync()
        {
            List<SelectListItem> list = await _context.Categorias.Select(c => new SelectListItem
            {
                Text = c.Nombre,
                Value = c.Id.ToString()
            })
                .OrderBy(c => c.Text)
                .ToListAsync();
            list.Insert(0, new SelectListItem { Text = "[Seleccione una categoria...", Value = "0" });
            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboEstadosAsync()
        {
            List<SelectListItem> list = await _context.Estados.Select(e => new SelectListItem
            {
                Text = e.Nombre,
                Value = e.Id.ToString()
            })
                .OrderBy(e => e.Text)
                .ToListAsync();
            list.Insert(0, new SelectListItem { Text = "[Seleccione un departamento...", Value = "0" });
            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboCiudadAsync(int estadoId)
        {
            List<SelectListItem> list = await _context.Ciudades
                .Where(e => e.Estado.Id == estadoId)
                .Select(e => new SelectListItem
            {
                Text = e.Nombre,
                Value = e.Id.ToString()
            })
                .OrderBy(e => e.Text)
                .ToListAsync();
            list.Insert(0, new SelectListItem { Text = "[Seleccione una ciudad...", Value = "0" });
            return list;
        }

        
    }
}
