namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_LeviBTS
    {
        [Key]
        public int LeviID { get; set; }
        public int Levi_Year { get; set; }
        public int Levi_Month { get; set; }
        public int Location { get; set; }
        public decimal? Threshold_Value { get; set; }
        public decimal? PercentRate { get; set; }
        //public decimal? BTSAvgPrice { get; set; }
        public decimal? CPOPrice { get; set; }//new
        public decimal? LeviValue { get; set; }
        public bool? LeviCaj { get; set; }
        public DateTime? last_modified { get; set; }
        public bool? fld_Deleted { get; set; }

        [StringLength(20)]
        public string Levi_ModifiedByID { get; set; }//new
    }
}
