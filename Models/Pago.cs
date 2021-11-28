using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesApp.Models
{
    public class Pago
    {
        [Key]
        public int idPago { get; set; }
        [Required]
        public int CantidadCuotas { get; set; }
        [Required]
        public string Estado { get; set; }
        public int i { get; set; }
        [ForeignKey("idVenta")]
        public Venta Venta { get; set; }
    }
}
