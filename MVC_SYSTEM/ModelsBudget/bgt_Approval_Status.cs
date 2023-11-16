using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Approval_Status")]
    public partial class bgt_Approval_Status
    {
        [Key]
        public int Aprv_Status_ID { get; set; }

        [StringLength(50)]
        public string Aprv_Status_Desc { get; set; }

        [StringLength(50)]
        public string Aprv_Hdr_Title { get; set; }
    }
}