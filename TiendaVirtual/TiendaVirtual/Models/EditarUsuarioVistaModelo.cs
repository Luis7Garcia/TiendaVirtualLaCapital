using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Enum;

namespace TiendaVirtual.Models
{
    public class EditarUsuarioVistaModelo
    {
        public string Id { get; set; }

        [Display(Name = "Documento")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe ser de máximo {1} carácteres.")]
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

        [Display(Name = "Teléfono")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Dirección")]
        [MaxLength(200, ErrorMessage = "El campo {0} debe ser de máximo {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Direccion { get; set; }

        [Display(Name = "Foto")]
        public Guid ImagenId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImagenId == Guid.Empty
             ? $"https://localhost:7061/imagenes/noimage.png"
             : $"https://tiendalacapital.blob.core.windows.net/usuarios/{ImagenId}";

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }


        [Display(Name = "Departmento / Estado")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes de seleccionar un departamento/estado.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public int EstadoId { get; set; }

        public IEnumerable<SelectListItem> Estados { get; set; }

        [Display(Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes de seleccionar una ciudad.")]
        public int CiudadId { get; set; }

        public IEnumerable<SelectListItem> Ciudades { get; set; }

    }
}
