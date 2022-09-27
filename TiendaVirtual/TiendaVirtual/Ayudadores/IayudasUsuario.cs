using Microsoft.AspNetCore.Identity;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Models;

namespace TiendaVirtual.Ayudadores
{
    public interface IayudasUsuario
    {
        //Este método permite pasarle el email del usuario y devuelve el usuario
        Task<Usuario> GetUsuarioAsync(string email);

        //Este método permite crear el usuario se le pasa el usuario y la contraseña 
        Task<IdentityResult> AddUsuarioAsync(Usuario user, string password);
        Task<Usuario> AddUsuarioAsync(AgregarUsuarioVistaModelo model); 
        //Este método verifica si un rol existe y si no existe lo crea.
        Task CheckiarRolesAsync(string roleName);

        //Este método permite asignar a que tipo de usuario pertenece.
        Task AddUsuarioARolAsync(Usuario user, string rolNombre);

        //Este método devuelve un boolean diciendo si es de un rol o no.
        Task<bool> IsUsuariounRolAsync(Usuario user, string rolNombre);

        //Este método verifica si el email o la contraseña son válidos.
        public Task<SignInResult> LoginAsync(LoginVistaModelo model);

        // Este método hace el cierre de sesión.
        public Task LogoutAsync();
    }
}
