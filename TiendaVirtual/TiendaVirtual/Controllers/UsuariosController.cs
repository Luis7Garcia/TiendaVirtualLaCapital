using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Ayudadores;
using TiendaVirtual.Common;
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
        private readonly IAyudaCorreo _ayudaCorreo;

        public UsuariosController(DatosTienda context, IayudasUsuario ayudasUsuario, IBlobAyudas blobAyudas, ICombosAyudas combosAyudas, IAyudaCorreo ayudaCorreo)
        {
            _context = context;
            _ayudasUsuario = ayudasUsuario;
            _blobAyudas = blobAyudas;
            _combosAyudas = combosAyudas;
            _ayudaCorreo = ayudaCorreo;
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


                string myToken = await _ayudasUsuario.GenerateEmailConfirmationTokenAsync(usuario);
                string tokenLink = Url.Action("ConfirmEmail", "Cuenta", new
                {
                    userid = usuario.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _ayudaCorreo.SendMail(
                    $"{model.Nombre} {model.Apellido}",
                    model.Username,
                    "Shopping - Confirmación de Email",
                    $"<h1>TiendaVirtual - Confirmación de Email</h1>" +
                        $"Para habilitar el usuario por favor hacer click en el siguiente link:, " +
                        $"<hr/><br/><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar el administrador han sido enviadas al correo.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);
            

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
