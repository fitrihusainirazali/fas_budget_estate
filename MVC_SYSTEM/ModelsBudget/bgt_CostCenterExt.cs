using MVC_SYSTEM.MasterModels;
//using MVC_SYSTEM.ModelsSAPPUP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_CostCenterExt")]
    public partial class bgt_CostCenterExt
    {
        [Key]
        public Guid CC_ExtID { get; set; }

        public Guid? CC_ID { get; set; }

        [StringLength(50)]
        public string CC_REGION { get; set; }

        [StringLength(50)]
        public string ZC { get; set; }

        public int? CC_ZONE { get; set; }

        [StringLength(50)]
        public string CC_TYPE { get; set; }

        [StringLength(2)]
        public string CA { get; set; }

        [ForeignKey("CC_ID")]
        public virtual tbl_SAPCCPUP tbl_SAPCCPUP { get; set; }

        [ForeignKey("CC_ZONE")]
        public virtual bgt_Location bgt_Location { get; set; }

        public bgt_CostCenterExt()
        {
            CC_ExtID = Guid.NewGuid();
        }
    }
}