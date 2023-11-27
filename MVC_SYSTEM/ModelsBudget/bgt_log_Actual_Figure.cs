namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_log_Actual_Figure
    {
        public int actu_Year { get; set; }

        [Key]
        public DateTime? actu_modified { get; set; }

        [StringLength(20)]
        public string actu_modifiedByID { get; set; }

        public int actu_SyarikatID { get; set; }
    }
}
