using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_expenses_vehicle")]
    public class bgt_expenses_vehicle
    {
        [Key]
        public int abev_id { get; set; }

        public int? abev_revision { get; set; }

        public int abev_budgeting_year { get; set; }

        public int? abev_syarikat_id { get; set; }

        [StringLength(100)]
        public string abev_syarikat_name { get; set; }

        public int? abev_ladang_id { get; set; }

        [StringLength(5)]
        public string abev_ladang_code { get; set; }

        [StringLength(50)]
        public string abev_ladang_name { get; set; }

        public int? abev_wilayah_id { get; set; }

        [StringLength(50)]
        public string abev_wilayah_name { get; set; }

        [StringLength(15)]
        public string abev_cost_center_code { get; set; }

        [StringLength(100)]
        public string abev_cost_center_desc { get; set; }
        [StringLength(11)]
        public string abev_material_code { get; set; }
        [StringLength(100)]
        public string abev_material_name { get; set; }
        [StringLength(100)]
        public string abev_model { get; set; }
        [StringLength(100)]
        public string abev_registration_no { get; set; }

        [StringLength(20)]
        public string abev_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abev_gl_expenses_name { get; set; }

        [StringLength(100)]
        public string abev_note { get; set; }
        public decimal? abev_quantity { get; set; }
        public decimal? abev_unit_price { get; set; }

        [StringLength(25)]
        public string abev_uom { get; set; }

        public decimal? abev_quantity_1 { get; set; }

        public decimal? abev_quantity_2 { get; set; }

        public decimal? abev_quantity_3 { get; set; }

        public decimal? abev_quantity_4 { get; set; }

        public decimal? abev_quantity_5 { get; set; }

        public decimal? abev_quantity_6 { get; set; }

        public decimal? abev_quantity_7 { get; set; }

        public decimal? abev_quantity_8 { get; set; }

        public decimal? abev_quantity_9 { get; set; }

        public decimal? abev_quantity_10 { get; set; }

        public decimal? abev_quantity_11 { get; set; }

        public decimal? abev_quantity_12 { get; set; }

        public decimal? abev_amount_1 { get; set; }

        public decimal? abev_amount_2 { get; set; }

        public decimal? abev_amount_3 { get; set; }

        public decimal? abev_amount_4 { get; set; }

        public decimal? abev_amount_5 { get; set; }

        public decimal? abev_amount_6 { get; set; }

        public decimal? abev_amount_7 { get; set; }

        public decimal? abev_amount_8 { get; set; }

        public decimal? abev_amount_9 { get; set; }

        public decimal? abev_amount_10 { get; set; }

        public decimal? abev_amount_11 { get; set; }

        public decimal? abev_amount_12 { get; set; }

        public decimal? abev_total_quantity { get; set; }

        public decimal? abev_total { get; set; }

        [StringLength(25)]
        public string abev_proration { get; set; }

        public bool? abev_history { get; set; }

        [StringLength(50)]
        public string abev_status { get; set; }

        public int? abev_created_by { get; set; }

        public bool? abev_deleted { get; set; }

        public DateTime? last_modified { get; set; }
    }
}