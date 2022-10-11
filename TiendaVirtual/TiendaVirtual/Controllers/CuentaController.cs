using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using TiendaVirtual.Ayudadores;
using TiendaVirtual.Data;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Enum;
using TiendaVirtual.Models;
using static TiendaVirtual.Models.CambiarContraseñaVistaModelo;

namespace TiendaVirtual.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IayudasUsuario _ayudaUsuario;
        private readonly DatosTienda _context;
        private readonly ICombosAyudas _combosAyudas;
        private readonly IBlobAyudas _blobAyudas;

        public CuentaController(IayudasUsuario ayudaUsuario, DatosTienda context, ICombosAyudas combosAyudas, IBlobAyudas blobAyudas)
        {
            _ayudaUsuario = ayudaUsuario;
            _context = context;
            _combosAyudas = combosAyudas;
            _blobAyudas = blobAyudas;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginVistaModelo());
        }
        [HttpPost]

        public async Task<IActionResult> Login(LoginVistaModelo model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _ayudaUsuario.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _ayudaUsuario.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NoAutorizado()
        {
            return View();
        }

        public async Task<IActionResult> Registro()
        {
            AgregarUsuarioVistaModelo model = new()
            {
                Id = Guid.Empty.ToString(),
                Estados = await _combosAyudas.GetComboEstadosAsync(),
                Ciudades= await _combosAyudas.GetComboCiudadAsync(0),
                TipoUsuario = TipoUsuario.Usuario,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(AgregarUsuarioVistaModelo model)
        {
            if (ModelState.IsValid)
            {
                Guid imagenId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imagenId = await _blobAyudas.UploadBlobAsync(model.ImageFile, "usuarios");
                }
                model.ImagenId = imagenId;
                Usuario usuario = await _ayudaUsuario.AddUsuarioAsync(model);
                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    model.Estados = await _combosAyudas.GetComboEstadosAsync();
                    model.Ciudades = await _combosAyudas.GetComboCiudadAsync(model.EstadoId);
                    return View(model);
                }

                LoginVistaModelo loginVistaModelo = new LoginVistaModelo
                {
                    Password = model.Password,
                    RememberMe = false,
                    Username = model.Username
                };

                var result2 = await _ayudaUsuario.LoginAsync(loginVistaModelo);

                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
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

        public async Task<IActionResult> CambioUsuario()
        {
            Usuario usuario = await _ayudaUsuario.GetUsuarioAsync(User.Identity.Name);
            if (usuario == null)
            {
                return NotFound();
            }

            EditarUsuarioVistaModelo model = new()
            {
                Direccion = usuario.Direccion,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                PhoneNumber = usuario.PhoneNumber,
                ImagenId = usuario.ImagenId,
                Ciudades = await _combosAyudas.GetComboCiudadAsync(usuario.Ciudad.Estado.Id),
                CiudadId = usuario.Ciudad.Id,
                Estados = await _combosAyudas.GetComboEstadosAsync(),
                EstadoId = usuario.Ciudad.Estado.Id,
                Id = usuario.Id,
                Documento = usuario.Documento
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambioUsuario(EditarUsuarioVistaModelo model)
        {
            if (ModelState.IsValid)
            {
                Guid imagenId = model.ImagenId;

                if (model.ImageFile != null)
                {
                    imagenId = await _blobAyudas.UploadBlobAsync(model.ImageFile, "usuarios");
                }

                Usuario usuario = await _ayudaUsuario.GetUsuarioAsync(User.Identity.Name);

                usuario.Nombre = model.Nombre;
                usuario.Apellido = model.Apellido;
                usuario.Direccion = model.Direccion;
                usuario.PhoneNumber = model.PhoneNumber;
                usuario.ImagenId = imagenId;
                usuario.Ciudad = await _context.Ciudades.FindAsync(model.CiudadId);
                usuario.Documento = model.Documento;

                await _ayudaUsuario.UpdateUsuarioAsync(usuario);
                return RedirectToAction("Index", "Home");
            }

            model.Estados = await _combosAyudas.GetComboEstadosAsync();
            model.Ciudades = await _combosAyudas.GetComboCiudadAsync(model.EstadoId);
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(CambiarContraseñaVistaModelo model)
        {
            if (ModelState.IsValid)
            {
                if(model.OldPassword == model.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "Debes ingresar una contraseña diferente.");
                    return View(model);
                }

                Usuario? usuario = await _ayudaUsuario.GetUsuarioAsync(User.Identity.Name);
                if (usuario != null)
                {
                    IdentityResult? result = await _ayudaUsuario.ChangePasswordAsync(usuario, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("CambioUsuario");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                }
            }

            return View(model);
        }


    }
}
