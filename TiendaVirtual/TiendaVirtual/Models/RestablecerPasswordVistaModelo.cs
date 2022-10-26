using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TiendaVirtual.Models
{
    public class RestablecerPasswordVistaModelo
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "La nueva contraseña y la confirmación no son iguales.")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Token { get; set; }

    }
}
