using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Station_NC")]
    public partial class bgt_Station_NC
    {
        [Key]
        public int Station_ID { get; set; }

        [Required]
        [StringLength(3)]
        public string Code_LL { get; set; }

        [StringLength(40)]
        public string Station_Desc { get; set; }
    }
}