using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Ayudadores;
using TiendaVirtual.Data;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Enum;
using TiendaVirtual.Models;

namespace TiendaVirtual.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly DatosTienda _context;
        private readonly IayudasUsuario _ayudasUsuario;
        private readonly IBlobAyudas _blobAyudas;
        private readonly ICombosAyudas _combosAyudas;

        public UsuariosController(DatosTienda context, IayudasUsuario ayudasUsuario, IBlobAyudas blobAyudas, ICombosAyudas combosAyudas)
        {
            _context = context;
            _ayudasUsuario = ayudasUsuario;
            _blobAyudas = blobAyudas;
            _combosAyudas = combosAyudas;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.Ciudad)
                .ThenInclude(c => c.Estado)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            AgregarUsuarioVistaModelo model = new()
            {
                Id = Guid.Empty.ToString(),
                Estados = await _combosAyudas.GetComboEstadosAsync(),
                Ciudades = await _combosAyudas.GetComboCiudadAsync(0),
                TipoUsuario = TipoUsuario.Admin,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AgregarUsuarioVistaModelo model)
        {
            if (ModelState.IsValid)
            {
                Guid imagenId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imagenId = await _blobAyudas.UploadBlobAsync(model.ImageFile, "usuarios");
                }
                model.ImagenId = imagenId;
                Usuario usuario = await _ayudasUsuario.AddUsuarioAsync(model);
                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    model.Estados = await _combosAyudas.GetComboEstadosAsync();
                    model.Ciudades = await _combosAyudas.GetComboCiudadAsync(model.EstadoId);
                    return View(model);
                }

              
                    return RedirectToAction("Index", "Home");
                
            }
            model.Estados = await _combosAyudas.GetComboEstadosAsync();
            model.Ciudades = await _combosAyudas.GetComboCiudadAsync(model.EstadoId);
            return View(model);
        }

        public JsonResult GetCiudades(int estadoId)
        {
            Estado estado = _context.Estados
                .Include(e => e.Ciudades)
                .FirstOrDefault(e => e.Id == estadoId);
            if (estado == null)
            {
                return null;
            }

            return Json(estado.Ciudades.OrderBy(d => d.Nombre));
        }
    }
}
