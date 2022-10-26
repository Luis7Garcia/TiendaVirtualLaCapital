using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TiendaVirtual.Models
{
    public class RecuperarContraseñaVistaModelo
    {

        [Display(Name = "Email")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        public string Email { get; set; }


    }
}
