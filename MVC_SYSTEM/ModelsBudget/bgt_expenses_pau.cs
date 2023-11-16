using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_expenses_pau")]
    public class bgt_expenses_pau
    {
        [Key]
        public int abep_id { get; set; }

        public int abep_revision { get; set; }

        public int abep_budgeting_year { get; set; }

        public int? abep_syarikat_id { get; set; }

        [StringLength(100)]
        public string abep_syarikat_name { get; set; }

        public int? abep_ladang_id { get; set; }

        [StringLength(5)]
        public string abep_ladang_code { get; set; }

        [StringLength(50)]
        public string abep_ladang_name { get; set; }

        public int? abep_wilayah_id { get; set; }

        [StringLength(50)]
        public string abep_wilayah_name { get; set; }

        [StringLength(15)]
        public string abep_cost_center_code { get; set; }

        [StringLength(100)]
        public string abep_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abep_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abep_gl_expenses_name { get; set; }

        [StringLength(20)]
        public string abep_gl_income_code { get; set; }

        [StringLength(100)]
        public string abep_gl_income_name { get; set; }

        [StringLength(20)]
        public string abep_product_code { get; set; }

        [StringLength(100)]
        public string abep_product_name { get; set; }

        [StringLength(15)]
        public string abep_cost_center_jualan_code { get; set; }

        [StringLength(100)]
        public string abep_cost_center_jualan_desc { get; set; }

        public decimal? abep_quantity { get; set; }

        [StringLength(25)]
        public string abep_uom_1 { get; set; }

        public decimal? abep_rate { get; set; }

        public decimal? abep_quantity_1 { get; set; }

        public decimal? abep_quantity_2 { get; set; }

        public decimal? abep_quantity_3 { get; set; }

        public decimal? abep_quantity_4 { get; set; }

        public decimal? abep_quantity_5 { get; set; }

        public decimal? abep_quantity_6 { get; set; }

        public decimal? abep_quantity_7 { get; set; }

        public decimal? abep_quantity_8 { get; set; }

        public decimal? abep_quantity_9 { get; set; }

        public decimal? abep_quantity_10 { get; set; }

        public decimal? abep_quantity_11 { get; set; }

        public decimal? abep_quantity_12 { get; set; }

        public decimal? abep_amount_1 { get; set; }

        public decimal? abep_amount_2 { get; set; }

        public decimal? abep_amount_3 { get; set; }

        public decimal? abep_amount_4 { get; set; }

        public decimal? abep_amount_5 { get; set; }

        public decimal? abep_amount_6 { get; set; }

        public decimal? abep_amount_7 { get; set; }

        public decimal? abep_amount_8 { get; set; }

        public decimal? abep_amount_9 { get; set; }

        public decimal? abep_amount_10 { get; set; }

        public decimal? abep_amount_11 { get; set; }

        public decimal? abep_amount_12 { get; set; }

        public decimal? abep_total_quantity { get; set; }

        public decimal? abep_total { get; set; }

        [StringLength(25)]
        public string abep_proration { get; set; }

        public bool abep_history { get; set; }

        [StringLength(50)]
        public string abep_status { get; set; }

        public int? abep_created_by { get; set; }

        public bool abep_deleted { get; set; }

        public DateTime? last_modified { get; set; }
    }
}