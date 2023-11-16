using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_expenses_others")]
    public partial class bgt_expenses_other
    {
        [Key]
        public int abeo_id { get; set; }

        public int abeo_revision { get; set; }

        public int abeo_budgeting_year { get; set; }

        public int? abeo_syarikat_id { get; set; }

        [StringLength(100)]
        public string abeo_syarikat_name { get; set; }

        public int? abeo_ladang_id { get; set; }

        [StringLength(5)]
        public string abeo_ladang_code { get; set; }

        [StringLength(50)]
        public string abeo_ladang_name { get; set; }

        public int? abeo_wilayah_id { get; set; }

        [StringLength(50)]
        public string abeo_wilayah_name { get; set; }

        [StringLength(50)]
        public string abeo_company_category { get; set; }

        [StringLength(50)]
        public string abeo_company_code { get; set; }

        [StringLength(100)]
        public string abeo_company_name { get; set; }

        [StringLength(15)]
        public string abeo_cost_center_code { get; set; }

        [StringLength(100)]
        public string abeo_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abeo_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abeo_gl_expenses_name { get; set; }

        [StringLength(100)]
        public string abeo_note { get; set; }

        public decimal? abeo_unit_1 { get; set; }

        [StringLength(25)]
        public string abeo_uom_1 { get; set; }

        [StringLength(25)]
        public string abeo_operation_1 { get; set; }

        public decimal? abeo_unit_2 { get; set; }

        [StringLength(25)]
        public string abeo_uom_2 { get; set; }

        [StringLength(25)]
        public string abeo_operation_2 { get; set; }

        public decimal? abeo_unit_3 { get; set; }

        [StringLength(25)]
        public string abeo_uom_3 { get; set; }

        [StringLength(25)]
        public string abeo_operation_3 { get; set; }

        public decimal? abeo_rate { get; set; }

        public decimal? abeo_total { get; set; }

        public decimal? abeo_month_1 { get; set; }

        public decimal? abeo_month_2 { get; set; }

        public decimal? abeo_month_3 { get; set; }

        public decimal? abeo_month_4 { get; set; }

        public decimal? abeo_month_5 { get; set; }

        public decimal? abeo_month_6 { get; set; }

        public decimal? abeo_month_7 { get; set; }

        public decimal? abeo_month_8 { get; set; }

        public decimal? abeo_month_9 { get; set; }

        public decimal? abeo_month_10 { get; set; }

        public decimal? abeo_month_11 { get; set; }

        public decimal? abeo_month_12 { get; set; }

        [StringLength(25)]
        public string abeo_proration { get; set; }

        public bool abeo_history { get; set; }

        [StringLength(50)]
        public string abeo_status { get; set; }

        public int? abeo_created_by { get; set; }

        public bool abeo_deleted { get; set; }

        public DateTime? last_modified { get; set; }
    }
}