using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Location")]
    public partial class bgt_Location
    {
        [Key]
        public int LocID { get; set; }

        [StringLength(10)]
        public string LocCode { get; set; }

        [Required]
        [StringLength(50)]
        public string LocDesc { get; set; }

        public virtual ICollection<bgt_Product> bgt_Products { get; set; }
    }
}