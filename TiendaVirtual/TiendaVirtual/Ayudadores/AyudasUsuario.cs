using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Data;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Models;

namespace TiendaVirtual.Ayudadores
{
    public class AyudasUsuario : IayudasUsuario
    {
        private readonly DatosTienda _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;

        public AyudasUsuario(DatosTienda context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task AddUsuarioARolAsync(Usuario user, string rolNombre)
        {
            await _userManager.AddToRoleAsync(user, rolNombre);
        }

        public async Task<IdentityResult> AddUsuarioAsync(Usuario user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<Usuario> AddUsuarioAsync(AgregarUsuarioVistaModelo model)
        {
            Usuario usuario = new()
            {
                Documento = model.Documento,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Email = model.Username,
                Direccion = model.Direccion,
                ImagenId = model.ImagenId,
                PhoneNumber = model.PhoneNumber,
                Ciudad = await _context.Ciudades.FindAsync(model.CiudadId),
                UserName = model.Username,
                tipoUsuario = model.TipoUsuario,

            };

            IdentityResult result = await _userManager.CreateAsync(usuario, model.Password);
            if(result != IdentityResult.Success)
            {
                return null;
            }

            Usuario newUsuario = await GetUsuarioAsync(model.Username);
            await AddUsuarioARolAsync(newUsuario, usuario.tipoUsuario.ToString());
            return newUsuario;
        }

        public async Task<IdentityResult> ChangePasswordAsync(Usuario user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckiarRolesAsync(string roleName)
        {
            bool roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName,
                });
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(Usuario user, string token)
        {
           return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Usuario user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Usuario user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<Usuario> GetUsuarioAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Ciudad)
                .ThenInclude(c => c.Estado)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario> GetUsuarioAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Ciudad)
                .ThenInclude(c => c.Estado)
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }

        public async Task<bool> IsUsuariounRolAsync(Usuario user, string rolNombre)
        {
            return await _userManager.IsInRoleAsync(user, rolNombre);
        }

        public async Task<SignInResult> LoginAsync(LoginVistaModelo model)
        {
            return await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }


        public async Task<IdentityResult> UpdateUsuarioAsync(Usuario user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
