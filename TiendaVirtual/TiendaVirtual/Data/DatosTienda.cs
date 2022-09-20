using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Data.Entities;

namespace TiendaVirtual.Data
{
    //Se hereda de IdentityDbContext, el cual contiene métodos que van ayudar para la configuración
    //y le paso la entidad usuario para que me la cree con las demas tablas que crea el identity.
    public class DatosTienda : IdentityDbContext<Usuario>
    {
        //Creo un constructor con un parametro el cual le paso al constructor de base al DbContext 
        public DatosTienda( DbContextOptions<DatosTienda> options): base(options)
        {
        }

        //Cada que se crea una entidad se debe pasar como propiedad para migrar a la base de datos
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Ciudad> Ciudades { get; set; }

        public DbSet<Estado> Estados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Aqui se especifica que el campo nombre no se va a repetir en la tabla espécificada.
            modelBuilder.Entity<Estado>().HasIndex(e => e.Nombre).IsUnique();
            modelBuilder.Entity<Categoria>().HasIndex(c => c.Nombre).IsUnique();
            modelBuilder.Entity<Ciudad>().HasIndex(c => c.Nombre).IsUnique();
        }
    }
}
