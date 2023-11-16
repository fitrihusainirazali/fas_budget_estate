using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_UOM")]
    public partial class bgt_UOM
    {
        [Key]
        public int UOMID { get; set; }

        [Required]
        [StringLength(50)]
        public string UOM { get; set; }

        public virtual ICollection<bgt_Product> bgt_Products { get; set; }
    }
}