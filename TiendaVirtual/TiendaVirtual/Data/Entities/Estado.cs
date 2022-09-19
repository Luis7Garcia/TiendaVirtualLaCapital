﻿using System.ComponentModel.DataAnnotations;

namespace TiendaVirtual.Data.Entities
{
    public class Estado
    {
        public int Id { get; set; }

        //Se especifican las validaciones para este campo.
        [Display(Name = "Departamento/Estado")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe ser de máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; }

        //Se agrega la relación uno a muchos con la tabla ciudad.
        public ICollection<Ciudad> Ciudades { get; set; }

        [Display(Name ="Número ciudades")]
        public int NumeroCiudades => Ciudades == null ? 0 : Ciudades.Count;
    }
}
