namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_Salary")]
    public partial class bgt_expenses_Salary
    {
        [Key]
        public int abes_id { get; set; }

         public int? abes_budgeting_year { get; set; }

       
        [StringLength(100)]
        public string abes_cost_center { get; set; }


        [StringLength(100)]
        public string abes_cost_center_name { get; set; }


        [StringLength(100)]
        public string abes_gl_code { get; set; }

        [StringLength(100)]
        public string abes_gl_desc { get; set; }


        [StringLength(100)]
        public string abes_uom { get; set; }


        [StringLength(100)]
        public string abes_proration { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_jumlah_setahun { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_1 { get; set; }

        
        [Column(TypeName = "numeric")]
        public int? abes_qty_2 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_3 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_4 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_5 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_6 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_7 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_8 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_9 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_10 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_11 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_qty_12 { get; set; }

        [Column(TypeName = "numeric")]
        public int? abes_jumlah_keseluruhan_qty { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_jumlah_RM { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_jumlah_keseluruhan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abes_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abes_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abes_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abes_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abes_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abes_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abes_month_12 { get; set; }


        public DateTime? last_modified { get; set; }


        public int? abes_NegaraID { get; set; }



        public int? abes_SyarikatID { get; set; }


        public int? abes_WilayahID { get; set; }


        public int? abes_LadangID { get; set; }

        public bool abes_Deleted { get; set; }

        public int abes_revision { get; set; }

        public string abes_status { get; set; }

        public bool? abes_history { get; set; }
    }

}

