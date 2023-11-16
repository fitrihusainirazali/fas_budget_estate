namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_Depreciation")]
    public partial class bgt_expenses_Depreciation
    {
        [Key]
        public int abed_id { get; set; }

         public int? abed_budgeting_year { get; set; }

       
        [StringLength(100)]
        public string abed_cost_center { get; set; }


        [StringLength(100)]
        public string abed_cost_center_name { get; set; }


        [StringLength(100)]
        public string abed_gl_code { get; set; }

        [StringLength(100)]
        public string abed_gl_desc { get; set; }


        [StringLength(100)]
        public string abed_uom { get; set; }


        [StringLength(100)]
        public string abed_proration { get; set; }

        
        [Column(TypeName = "numeric")]
        public decimal? abed_jumlah_setahun { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abed_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abed_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abed_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abed_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abed_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_month_12 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_jumlah_RM { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abed_jumlah_keseluruhan { get; set; }

       
        public DateTime? last_modified { get; set; }


        public int? abed_NegaraID { get; set; }



        public int? abed_SyarikatID { get; set; }


        public int? abed_WilayahID { get; set; }


        public int? abed_LadangID { get; set; }

        public bool abed_Deleted { get; set; }

        public int abed_revision { get; set; }

        public string abed_status { get; set; }

        public bool? abed_history { get; set; }

    }

}

