using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_expenses_capital")]
    public partial class bgt_expenses_capital
    {
        [Key]
        public int abce_id { get; set; }

        public int abce_revision { get; set; }

        public int abce_budgeting_year { get; set; }

        public int? abce_syarikat_id { get; set; }

        [StringLength(100)]
        public string abce_syarikat_name { get; set; }

        public int? abce_ladang_id { get; set; }

        [StringLength(5)]
        public string abce_ladang_code { get; set; }

        [StringLength(50)]
        public string abce_ladang_name { get; set; }

        public int? abce_wilayah_id { get; set; }

        [StringLength(50)]
        public string abce_wilayah_name { get; set; }

        [StringLength(15)]
        public string abce_cost_center_code { get; set; }

        [StringLength(100)]
        public string abce_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abce_gl_code { get; set; }

        [StringLength(100)]
        public string abce_gl_name { get; set; }

        [StringLength(255)]
        public string abce_desc { get; set; }

        [StringLength(20)]
        public string abce_jenis_code { get; set; }

        [StringLength(100)]
        public string abce_jenis_name { get; set; }

        [StringLength(255)]
        public string abce_reason { get; set; }

        [StringLength(100)]
        public string abce_material_details { get; set; }

        public decimal? abce_unit_1 { get; set; }

        public decimal? abce_uom_1 { get; set; }

        public decimal? abce_rate { get; set; }

        public decimal? abce_quantity_1 { get; set; }

        public decimal? abce_quantity_2 { get; set; }

        public decimal? abce_quantity_3 { get; set; }

        public decimal? abce_quantity_4 { get; set; }

        public decimal? abce_quantity_5 { get; set; }

        public decimal? abce_quantity_6 { get; set; }

        public decimal? abce_quantity_7 { get; set; }

        public decimal? abce_quantity_8 { get; set; }

        public decimal? abce_quantity_9 { get; set; }

        public decimal? abce_quantity_10 { get; set; }

        public decimal? abce_quantity_11 { get; set; }

        public decimal? abce_quantity_12 { get; set; }

        public decimal? abce_amount_1 { get; set; }

        public decimal? abce_amount_2 { get; set; }

        public decimal? abce_amount_3 { get; set; }

        public decimal? abce_amount_4 { get; set; }

        public decimal? abce_amount_5 { get; set; }

        public decimal? abce_amount_6 { get; set; }

        public decimal? abce_amount_7 { get; set; }

        public decimal? abce_amount_8 { get; set; }

        public decimal? abce_amount_9 { get; set; }

        public decimal? abce_amount_10 { get; set; }

        public decimal? abce_amount_11 { get; set; }

        public decimal? abce_amount_12 { get; set; }

        public decimal? abce_total { get; set; }

        public decimal? abce_total_quantity { get; set; }

        [StringLength(25)]
        public string abce_proration { get; set; }

        public bool abce_history { get; set; }

        [StringLength(50)]
        public string abce_status { get; set; }

        public int? abce_created_by { get; set; }

        public bool abce_deleted { get; set; }

        public DateTime? last_modified { get; set; }

        [StringLength(100)]
        public string abce_ast_code { get; set; }

        [StringLength(50)]
        public string abce_ast_category { get; set; }
    }
}