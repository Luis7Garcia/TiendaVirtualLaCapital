using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TiendaVirtual.Enum;

namespace TiendaVirtual.Data.Entities
{
    public class Usuario :IdentityUser
    {
        [Display(Name ="Documento")]
        [MaxLength(20, ErrorMessage ="El campo {0} debe ser de máximo {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Documento { get; set; }

        [Display(Name = "Nombres")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe ser de máximo {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe ser de máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Apellido { get; set; }

        [Display(Name = "Ciudad")]
        public Ciudad Ciudad { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(200, ErrorMessage = "El campo {0} debe ser de máximo {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Direccion { get; set; }

        [Display(Name ="Foto")]
        public Guid ImagenId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImagenId == Guid.Empty
             ? $"https://localhost:7061/imagenes/noimage.png"
             : $"https://TiendaVirtual.blob.core.windows.net/usuarios/{ImagenId}";

        [Display(Name ="Tipo de usuario")]
        public TipoUsuario tipoUsuario { get; set; }

        [Display(Name = "Usuario")]
        public string NombreCompleto => $"{Nombre} {Apellido}";

        public string NombreCompletoConDocumento => $"{Nombre} {Apellido} {Documento}";
    }
}
