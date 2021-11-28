using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesApp.Models
{
    public class Empleado
    {
        [Key]
        public int idEmpleado { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Dni { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Clave { get; set; }
        public Byte Estado { get; set; }
    }
}

