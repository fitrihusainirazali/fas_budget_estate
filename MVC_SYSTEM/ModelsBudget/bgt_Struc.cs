using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Struc")]
    public partial class bgt_Struc
    {
        [Key]
        public int str_ID { get; set; }

        [StringLength(50)]
        public string str_User_ID { get; set; }

        [StringLength(2)]
        public string str_Struc_ID { get; set; }

        [StringLength(2)]
        public string str_CodeLL { get; set; }

        [StringLength(20)]
        public string str_Station { get; set; }

        [StringLength(10)]
        public string str_CostCenter { get; set; }
    }
}