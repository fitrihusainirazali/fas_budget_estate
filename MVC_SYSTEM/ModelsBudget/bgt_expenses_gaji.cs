namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_gaji")]
    public partial class bgt_expenses_gaji
    {
        [Key]
        public int abeg_id { get; set; }

        public int? abeg_revision { get; set; }

        public int abeg_budgeting_year { get; set; }

        public int? abeg_syarikat_id { get; set; }

        [StringLength(100)]
        public string abeg_syarikat_name { get; set; }

        public int? abeg_ladang_id { get; set; }

        [StringLength(5)]
        public string abeg_ladang_code { get; set; }

        [StringLength(50)]
        public string abeg_ladang_name { get; set; }

        public int? abeg_wilayah_id { get; set; }

        [StringLength(50)]
        public string abeg_wilayah_name { get; set; }

        [StringLength(15)]
        public string abeg_cost_center_code { get; set; }


        [StringLength(100)]
        public string abeg_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abeg_hectare { get; set; }

        [StringLength(20)]
        public string abeg_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abeg_gl_expenses_name { get; set; }

        [StringLength(20)]
        public string abeg_akt_code { get; set; }

        [StringLength(500)]
        public string abeg_akt_name { get; set; }

        [StringLength(100)]
        public string abeg_note { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abeg_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abeg_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abeg_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abeg_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abeg_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abeg_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abeg_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abeg_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abeg_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abeg_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abeg_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abeg_month_12 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abeg_total { get; set; }


        [StringLength(25)]
        public string abeg_proration { get; set; }

        public int? abeg_created_by { get; set; }


        public DateTime? last_modified { get; set; }


        public bool abeg_Deleted { get; set; }

        
        public string abeg_status { get; set; }

        //public bool? abeg_history { get; set; }

    }

}

