namespace MVC_SYSTEM.ModelsBudget
{
    using MVC_SYSTEM.App_LocalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_approval_log")]
    public partial class bgt_approval_log
    {
        [Key]
        public int aprvlog_id { get; set; }

        public int aprvlog_budgeting_year { get; set; }

        [StringLength(15)]
        public string aprvlog_cost_center_code { get; set; }

        [StringLength(50)]
        public string aprvlog_status { get; set; }

        [StringLength(1000)]
        public string aprvlog_note { get; set; }
        
        public int? aprvlog_prepared_by { get; set; }
        
        public DateTime? last_modified { get; set; }
    }
}
