using Microsoft.AspNetCore.Mvc;
using TiendaVirtual.Ayudadores;
using TiendaVirtual.Models;

namespace TiendaVirtual.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IayudasUsuario _ayudaUsuario;

        public CuentaController(IayudasUsuario ayudaUsuario)
        {
            _ayudaUsuario = ayudaUsuario;
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
    }
}
