
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TiendaVirtual.Data.Entities
{
    public class Ciudad
    {
        public int Id { get; set; }

        //Se especifican las validaciones para este campo.
        [Display(Name = ("Ciudad"))]
        [MaxLength(50, ErrorMessage = "El campo {0} debe ser máximo de {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; }

        //Aqui se conecta la relacion con estado.
        [JsonIgnore]
        public Estado Estado { get; set; }

        //Relación de uno a muchos.
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
