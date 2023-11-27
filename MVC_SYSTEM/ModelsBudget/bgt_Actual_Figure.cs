namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_Actual_Figure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int actu_ID { get; set; }

        public int actu_Year { get; set; }

        [StringLength(50)]
        public string actu_GLCode { get; set; }

        [StringLength(50)]
        public string actu_CostCenter { get; set; }

        public decimal? actu_ActualAmt { get; set; }

        public DateTime? actu_modified { get; set; }

        [StringLength(10)]
        public string actu_SyarikatID { get; set; }
    }
}
