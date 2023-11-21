namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_KhidCor")]
    public partial class bgt_expenses_KhidCor
    {
        [Key]
        public int abcs_id { get; set; }

         public int? abcs_budgeting_year { get; set; }

       
        [StringLength(100)]
        public string abcs_cost_center { get; set; }


        [StringLength(100)]
        public string abcs_cost_center_name { get; set; }


        [StringLength(100)]
        public string abcs_gl_code { get; set; }

        [StringLength(100)]
        public string abcs_gl_desc { get; set; }


        [StringLength(100)]
        public string abcs_uom { get; set; }


        [StringLength(100)]
        public string abcs_proration { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_jumlah_setahun { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abcs_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abcs_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abcs_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abcs_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abcs_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_month_12 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_jumlah_RM { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abcs_jumlah_keseluruhan { get; set; }

       
        public DateTime? last_modified { get; set; }


        public int? abcs_NegaraID { get; set; }



        public int? abcs_SyarikatID { get; set; }


        public int? abcs_WilayahID { get; set; }


        public int? abcs_LadangID { get; set; }

        public bool abcs_Deleted { get; set; }

        public int abcs_revision { get; set; }

        public string abcs_status { get; set; }

        public bool? abcs_history { get; set; }

        [StringLength(255)]
        public string abcs_createdby { get; set; }

        public DateTime? abcs_upload_date { get; set; }

        public int? abcs_version { get; set; }

    }

}

