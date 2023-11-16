using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Notification")]
    public partial class bgt_Notification
    {
        [Key]
        public int Notify_ID { get; set; }
        public int Bgt_Year { get; set; }
        [StringLength(200)]
        public string noti_Letter { get; set; }
        [StringLength(50)]
        public string noti_Ref { get; set; }
        public DateTime? noti_Date { get; set; }
        public string noti_Content { get; set; }
        public bool? fld_Deleted { get; set; }
        public int fld_NegaraID { get; set; }
        public int fld_SyarikatID { get; set; }
    }
}