using System.ComponentModel.DataAnnotations;

namespace TiendaVirtual.Models
{
    public class CiudadVistaModelo
    {
        public int Id { get; set; }

        //Se especifican las validaciones para este campo.
        [Display(Name = "Ciudad")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe ser máximo de {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; }

        //Le adicciono la propiedad estadoid,
        //esta se necesita para poder buscar el estado y asi poder añadir la ciudad a ese estado.
        public int EstadoId { get; set; }
    }
}
