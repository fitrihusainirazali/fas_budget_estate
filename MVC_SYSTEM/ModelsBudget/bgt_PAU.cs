using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_PAU")]
    public partial class bgt_PAU
    {
        [Key]
        public int PAU_ID { get; set; }

        [Required]
        [StringLength(10)]
        public string GLCode_Buy { get; set; }

        [StringLength(50)]
        public string GLCode_Buy_Desc { get; set; }

        [Required]
        [StringLength(10)]
        public string GLCode_Sale { get; set; }

        [StringLength(50)]
        public string GLCode_Sale_Desc { get; set; }
    }
}