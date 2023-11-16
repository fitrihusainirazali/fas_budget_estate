namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_Medical")]
    public partial class bgt_expenses_Medical
    {
        [Key]
        public int abem_id { get; set; }

         public int? abem_budgeting_year { get; set; }

       
        [StringLength(100)]
        public string abem_cost_center { get; set; }


        [StringLength(100)]
        public string abem_cost_center_name { get; set; }


        [StringLength(100)]
        public string abem_gl_code { get; set; }

        [StringLength(100)]
        public string abem_gl_desc { get; set; }


        [StringLength(100)]
        public string abem_uom { get; set; }


        [StringLength(100)]
        public string abem_proration { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_jumlah_setahun { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abem_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abem_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abem_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abem_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abem_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_month_12 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_jumlah_RM { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abem_jumlah_keseluruhan { get; set; }

       
        public DateTime? last_modified { get; set; }


        public int? abem_NegaraID { get; set; }



        public int? abem_SyarikatID { get; set; }


        public int? abem_WilayahID { get; set; }


        public int? abem_LadangID { get; set; }

        public bool abem_Deleted { get; set; }

        public int abem_revision { get; set; }

        public string abem_status { get; set; }

        public bool? abem_history { get; set; }

    }

}

