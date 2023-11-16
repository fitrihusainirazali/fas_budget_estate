namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_Insurans")]
    public partial class bgt_expenses_Insurans
    {
        [Key]
        public int abei_id { get; set; }

         public int? abei_budgeting_year { get; set; }

       
        [StringLength(100)]
        public string abei_cost_center { get; set; }


        [StringLength(100)]
        public string abei_cost_center_name { get; set; }


        [StringLength(100)]
        public string abei_gl_code { get; set; }

        [StringLength(100)]
        public string abei_gl_desc { get; set; }


        [StringLength(100)]
        public string abei_uom { get; set; }


        [StringLength(100)]
        public string abei_proration { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_jumlah_setahun { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abei_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abei_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abei_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abei_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abei_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_month_12 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_jumlah_RM { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abei_jumlah_keseluruhan { get; set; }

       
        public DateTime? last_modified { get; set; }


        public int? abei_NegaraID { get; set; }



        public int? abei_SyarikatID { get; set; }


        public int? abei_WilayahID { get; set; }


        public int? abei_LadangID { get; set; }

        public bool abei_Deleted { get; set; }

        public int abei_revision { get; set; }

        public string abei_status { get; set; }

        public bool? abei_history { get; set; }

    }

}

