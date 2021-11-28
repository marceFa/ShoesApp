using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesApp.Models
{
    public class DetalleVenta
    {
        [Key]
        public int idDetalleVenta{ get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Required]
        public int Cantidad { get; set; }
        public int idVenta { get; set; }

        [ForeignKey("idVenta")]
        public Venta Venta { get; set; }
        public int idZapato { get; set; }
        [ForeignKey("idZapato")]
        public Zapato Zapato { get; set; }
    }
}

