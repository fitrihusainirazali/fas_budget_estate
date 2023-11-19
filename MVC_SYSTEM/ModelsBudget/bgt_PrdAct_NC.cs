using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_PrdAct_NC")]
    public partial class bgt_PrdAct_NC
    {
        [Key]
        public int PrdAct_ID { get; set; }

        [Required]
        [StringLength(2)]
        public string Code_PP { get; set; }

        [Required]
        [StringLength(50)]
        public string PrdAct_Desc { get; set; }

        [Required]
        [StringLength(1)]
        public string Ldg_NonLdg { get; set; }
    }
}