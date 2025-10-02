using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.Models.Entities
{
    public class ProductModel
    {
        [Key]
        public int ProductId { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string ProductName { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string ProductDescription { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
