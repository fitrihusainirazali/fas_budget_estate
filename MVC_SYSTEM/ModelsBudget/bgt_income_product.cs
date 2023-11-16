namespace MVC_SYSTEM.ModelsBudget
{
    using MVC_SYSTEM.App_LocalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_income_product
    {
        [Key]
        public int abip_id { get; set; }
        public int? abip_revision { get; set; }
        [StringLength(20)]
        public string abip_status { get; set; }
        public bool? abip_history { get; set; }
        public int? abip_fld_ID { get; set; }//add sept
        public int? abip_budgeting_year { get; set; }
        public int? abip_syarikat_id { get; set; }
        [StringLength(100)]
        public string abip_syarikat_name { get; set; }
        public int? abip_ladang_id { get; set; }
        [StringLength(5)]
        public string abip_ladang_code { get; set; }
        [StringLength(50)]
        public string abip_ladang_name { get; set; }
        public int? abip_wilayah_id { get; set; }
        [StringLength(50)]
        public string abip_wilayah_name { get; set; }
        [StringLength(50)]
        public string abip_company_category { get; set; }
        
        [StringLength(50)]
        public string abip_company_code { get; set; }
        public int? abip_company_list_sort { get; set; }
        [StringLength(100)]
        public string abip_company_name { get; set; }
        
        public int? abip_product_id { get; set; } //add new
        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string abip_product_code { get; set; }
        [StringLength(100)]
        public string abip_product_name { get; set; }
        [StringLength(50)]
        public string abip_station_code { get; set; }
        [StringLength(100)]
        public string abip_station_name { get; set; }
        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(15)]
        public string abip_cost_center { get; set; }
        [StringLength(100)]
        public string abip_cost_center_desc { get; set; }
        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(20)]
        public string abip_gl_code { get; set; }
        [StringLength(100)]
        public string abip_gl_desc { get; set; }
        [StringLength(100)]
        public string abip_uom { get; set; }
        public decimal? abip_unit_price { get; set; }
        public decimal? abip_qty_1 { get; set; }
        public decimal? abip_qty_2 { get; set; }
        public decimal? abip_qty_3 { get; set; }
        public decimal? abip_qty_4 { get; set; }
        public decimal? abip_qty_5 { get; set; }
        public decimal? abip_qty_6 { get; set; }
        public decimal? abip_qty_7 { get; set; }
        public decimal? abip_qty_8 { get; set; }
        public decimal? abip_qty_9 { get; set; }
        public decimal? abip_qty_10 { get; set; }
        public decimal? abip_qty_11 { get; set; }
        public decimal? abip_qty_12 { get; set; }
        public decimal? abip_amount_1 { get; set; }
        public decimal? abip_amount_2 { get; set; }
        public decimal? abip_amount_3 { get; set; }
        public decimal? abip_amount_4 { get; set; }
        public decimal? abip_amount_5 { get; set; }
        public decimal? abip_amount_6 { get; set; }
        public decimal? abip_amount_7 { get; set; }
        public decimal? abip_amount_8 { get; set; }
        public decimal? abip_amount_9 { get; set; }
        public decimal? abip_amount_10 { get; set; }
        public decimal? abip_amount_11 { get; set; }
        public decimal? abip_amount_12 { get; set; }
        public decimal? abip_total_qty_a { get; set; }
        public decimal? abip_total_qty_b { get; set; }
        public decimal? abip_total_amount_a { get; set; }
        public decimal? abip_total_amount_b { get; set; }
        public decimal? abip_grand_total_quantity { get; set; }
        public decimal? abip_grand_total_amount { get; set; }
        public int? abip_created_by { get; set; }
        public DateTime? last_modified { get; set; }
        public bool? fld_Deleted { get; set; }
    }
}
