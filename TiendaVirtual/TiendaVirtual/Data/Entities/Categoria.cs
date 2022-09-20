using System.ComponentModel.DataAnnotations;

namespace TiendaVirtual.Data.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        //Se especifican las validaciones para este campo.
        [Display(Name =("Categoria"))]
        [MaxLength(50, ErrorMessage ="El campo {0} debe ser máximo de {1} carácteres")]
        [Required(ErrorMessage ="El campo {0} es obligatorio")]
        public string Nombre { get; set; }
    }
}
