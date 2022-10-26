using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using TiendaVirtual.Ayudadores;
using TiendaVirtual.Common;
using TiendaVirtual.Data;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Enum;
using TiendaVirtual.Models;
using static TiendaVirtual.Models.CambiarContraseñaVistaModelo;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TiendaVirtual.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IayudasUsuario _ayudaUsuario;
        private readonly DatosTienda _context;
        private readonly ICombosAyudas _combosAyudas;
        private readonly IBlobAyudas _blobAyudas;
        private readonly IAyudaCorreo _ayudaCorreo;

        public CuentaController(IayudasUsuario ayudaUsuario, DatosTienda context, ICombosAyudas combosAyudas, IBlobAyudas blobAyudas, IAyudaCorreo ayudaCorreo)
        {
            _ayudaUsuario = ayudaUsuario;
            _context = context;
            _combosAyudas = combosAyudas;
            _blobAyudas = blobAyudas;
            _ayudaCorreo = ayudaCorreo;
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
                SignInResult result = await _ayudaUsuario.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Ha superado el máximo número de intentos, su cuenta está bloqueada, intente de nuevo en 5 minutos.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "El usuario no ha sido habilitado, debes de seguir las instrucciones del correo enviado para poder habilitarte en el sistema.");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                }

            }
            return View(model);
        }

        public IActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarPassword(RecuperarContraseñaVistaModelo model)
        {
            if (ModelState.IsValid)
            {
                Usuario user = await _ayudaUsuario.GetUsuarioAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El email no corresponde a ningún usuario registrado.");
                    return View(model);
                }

                string myToken = await _ayudaUsuario.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "RestablecerPassword",
                    "Cuenta",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _ayudaCorreo.SendMail(
                    $"{user.NombreCompleto}",
                    model.Email,
                    "Tienda Virtual - Recuperación de Contraseña",
                    $"<h1>Tienda Virtual - Recuperación de Contraseña</h1>" +
                    $"Para recuperar la contraseña haga click en el siguiente enlace:" +
                    $"<p><a href = \"{link}\">Restablecer Contraseña</a></p>");
                ViewBag.Message = "Las instrucciones para recuperar la contraseña han sido enviadas a su correo.";
                return View();
            }

            return View(model);
        }

        public IActionResult RestablecerPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerPassword(RestablecerPasswordVistaModelo model)
        {
            Usuario user = await _ayudaUsuario.GetUsuarioAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _ayudaUsuario.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Contraseña cambiada con éxito.";
                    return View();
                }

                ViewBag.Message = "Error cambiando la contraseña.";
                return View(model);
            }

            ViewBag.Message = "Usuario no encontrado.";
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
                string myToken = await _ayudaUsuario.GenerateEmailConfirmationTokenAsync(usuario);
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
                    ViewBag.Message = "Las instrucciones para habilitar el usuario han sido enviadas al correo.";
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

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            Usuario user = await _ayudaUsuario.GetUsuarioAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _ayudaUsuario.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
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
