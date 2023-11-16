namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("bgt_expenses_CtrlHQ")]
    public partial class bgt_expenses_CtrlHQ
    {
        [Key]
        public int abhq_id { get; set; }

         public int? abhq_budgeting_year { get; set; }

       
        [StringLength(100)]
        public string abhq_cost_center { get; set; }


        [StringLength(100)]
        public string abhq_cost_center_name { get; set; }


        [StringLength(100)]
        public string abhq_gl_code { get; set; }

        [StringLength(100)]
        public string abhq_gl_desc { get; set; }


        [StringLength(100)]
        public string abhq_uom { get; set; }


        [StringLength(100)]
        public string abhq_proration { get; set; }


        [DisplayFormat(DataFormatString = "{0:#,##0.00}")]
        public decimal? abhq_jumlah_setahun { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_month_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abhq_month_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abhq_month_3 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_month_4 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_month_5 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_month_6 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_month_7 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abhq_month_8 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abhq_month_9 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? abhq_month_10 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_month_11 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_month_12 { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_jumlah_RM { get; set; }


        [Column(TypeName = "numeric")]
        public decimal? abhq_jumlah_keseluruhan { get; set; }

       
        public DateTime? last_modified { get; set; }


        public int? abhq_NegaraID { get; set; }



        public int? abhq_SyarikatID { get; set; }


        public int? abhq_WilayahID { get; set; }


        public int? abhq_LadangID { get; set; }

        public bool abhq_Deleted { get; set; }

        public int abhq_revision { get; set; }

        public string abhq_status { get; set; }

        public bool? abhq_history { get; set; }

    }

}

