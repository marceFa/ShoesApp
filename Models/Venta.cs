using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesApp.Models
{
    public class Venta
    {
        [Key]
        public int idVenta { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public decimal MontoTotal { get; set; }
        [Required]
        public string Cliente { get; set; }
        public Byte Estado { get; set; }
        public int idEmpleado { get; set; }
        [ForeignKey("idEmpleado")]
        public Empleado Empleado { get; set; }
        public List<DetalleVenta> Detalles { get; set; }

    }
}

