using TiendaVirtual.Ayudadores;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Enum;

namespace TiendaVirtual.Data
{
    //Esta clase se va a encargar de llenar los registros en la base de datos
    public class PrincipalDb
    {
        private readonly DatosTienda _context;
        private readonly IayudasUsuario _ayudaUsuario;

        //Se inyecta la conexion a la base de datos, por medio de un constructor.
        public PrincipalDb(DatosTienda context, IayudasUsuario ayudaUsuario)
        {
            _context = context;
            _ayudaUsuario = ayudaUsuario;
        }

        public async Task PrincipalAsync()

        {
            // Este metodo crea la base de datos y aplica las migraciones.
            await _context.Database.EnsureCreatedAsync();
            await CheckiarCategoriasAsync();
            await CheckiarEstadosAsync();
            await CheckiarRoles();
            await CheckiarusuarioAsync("1020", "Luis", "Henao", "luis@yopmail.com", "3409809654", "Calle 1 # pobla - 1", TipoUsuario.Admin);
            await CheckiarusuarioAsync("1030", "Josefina", "Fina", "fina@yopmail.com", "3800209386", "Calle 100 # envi - 1", TipoUsuario.Usuario);
        }

        private async Task CheckiarusuarioAsync(
            string documento,
            string nombre,
            string apellido,
            string email,
            string telefono,
            string direccion,
            TipoUsuario tipoUsuario
            )
        {
            Usuario usuario = await _ayudaUsuario.GetUsuarioAsync(email);
            if(usuario == null)
            {
                usuario = new Usuario
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    UserName = email,
                    PhoneNumber = telefono,
                    Direccion = direccion,
                    Documento = documento,
                    Ciudad = _context.Ciudades.FirstOrDefault(),
                    tipoUsuario = tipoUsuario,
                };
                await _ayudaUsuario.AddUsuarioAsync(usuario, "1234567");
                await _ayudaUsuario.AddUsuarioARolAsync(usuario, tipoUsuario.ToString());
            }
        }
        

        private async Task CheckiarRoles()
        {
            await _ayudaUsuario.CheckiarRolesAsync(TipoUsuario.Admin.ToString());
            await _ayudaUsuario.CheckiarRolesAsync(TipoUsuario.Usuario.ToString());
        }

        //Este metodo va a verificar si ya existe Estados en la base de datos.
        private async Task CheckiarEstadosAsync()
        {
            if (!_context.Estados.Any())
            {
                _context.Estados.Add(new Estado
                {
                    Nombre = "Bogotá",
                    Ciudades = new List<Ciudad>()
                    {
                        new Ciudad {Nombre = "Chapinero"},
                        new Ciudad {Nombre = "Torca"},
                        new Ciudad {Nombre = "Bosa"},
                        new Ciudad {Nombre = "Kennedy"}
                    }
                });
            }
            await _context.SaveChangesAsync();
        }

        //Este metodo va a averificar si ya exite categorias en la base de datos.
        private async Task CheckiarCategoriasAsync()
        {
            if (!_context.Categorias.Any())
            {
                _context.Categorias.Add(new Categoria { Nombre = "Frutas" });
                _context.Categorias.Add(new Categoria { Nombre = "Verduras" });
                await _context.SaveChangesAsync();
            }
        }
    }
}
