using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Operation")]
    public partial class bgt_Operation
    {
        [Key]
        public int Opr_ID { get; set; }

        [StringLength(1)]
        [Required]
        public string Opr_Sign { get; set; }

        [StringLength(15)]
        public string Opr_Desc { get; set; }
    }
}