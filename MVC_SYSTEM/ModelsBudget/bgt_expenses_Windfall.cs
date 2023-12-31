﻿namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_Windfall")]
    public partial class bgt_expenses_Windfall
    {
        [Key]
        public int abew_id { get; set; }

         public int? abew_budgeting_year { get; set; }

       
        [StringLength(100)]
        public string abew_cost_center { get; set; }


        [StringLength(100)]
        public string abew_cost_center_name { get; set; }


        [StringLength(100)]
        public string abew_gl_code { get; set; }

        [StringLength(100)]
        public string abew_gl_desc { get; set; }


        [StringLength(100)]
        public string abew_uom { get; set; }


        [StringLength(100)]
        public string abew_proration { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_jumlah_setahun { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abew_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abew_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abew_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abew_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abew_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_month_12 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_jumlah_RM { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abew_jumlah_keseluruhan { get; set; }

       
        public DateTime? last_modified { get; set; }


        public int? abew_NegaraID { get; set; }



        public int? abew_SyarikatID { get; set; }


        public int? abew_WilayahID { get; set; }


        public int? abew_LadangID { get; set; }

        public bool abew_Deleted { get; set; }

        public int abew_revision { get; set; }

        public string abew_status { get; set; }

        public bool? abew_history { get; set; }
    }

}

