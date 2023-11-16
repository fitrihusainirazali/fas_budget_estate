namespace MVC_SYSTEM.ModelsBudget
{
    using MVC_SYSTEM.App_LocalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_approval")]
    public partial class bgt_approval
    {
        [Key]
        public int aprv_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(15)]
        public string aprv_cost_center_code { get; set; }

        [StringLength(100)]
        public string aprv_cost_center_desc { get; set; }

        public int aprv_status_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        public int aprv_budgeting_year { get; set; }
        public int? aprv_prepared_by { get; set; }
        public int? aprv_Approve_by { get; set; }//edit oct
        public bool? aprv_deleted { get; set; }
        public DateTime? aprv_date { get; set; }
        public DateTime? aprv_Send_date { get; set; }//add oct
        public DateTime? last_modified { get; set; }
        //[StringLength(3000)]
        public string aprv_note { get; set; }
        public bool aprv_bgt_block { get; set; }
        public int? aprv_organization { get; set; }//add nov
        [StringLength(4)]
        public string aprv_scrCode { get; set; }//edit nov
        public int? aprv_approve_hq { get; set; }//add nov
        public DateTime? aprv_date_hq { get; set; }//add nov
        public int? aprv_syarikat_id { get; set; }//add nov
        public int? aprv_revision { get; set; }//add nov
    }
}
