namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_expenses_levi
    {
        [Key]
        public int abel_id { get; set; }

        public int? abel_revision { get; set; }

        [StringLength(20)]
        public string abel_status { get; set; }

        public bool? abel_history { get; set; }

        //[Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        public int? abel_budgeting_year { get; set; }

        public int? abel_syarikat_id { get; set; }
        
        [StringLength(100)]
        public string abel_syarikat_name { get; set; }
        
        public int? abel_ladang_id { get; set; }
        
        [StringLength(5)]
        public string abel_ladang_code { get; set; }
        
        [StringLength(50)]
        public string abel_ladang_name { get; set; }
        
        public int? abel_wilayah_id { get; set; }
        
        [StringLength(50)]
        public string abel_wilayah_name { get; set; }
        
        [StringLength(50)]
        public string abel_company_category { get; set; }

        [StringLength(50)]
        public string abel_company_code { get; set; }

        [StringLength(100)]
        public string abel_company_name { get; set; }

        [StringLength(50)]
        public string abel_product_code { get; set; }

        [StringLength(100)]
        public string abel_product_name { get; set; }

        [StringLength(50)]
        public string abel_station_code { get; set; }

        [StringLength(100)]
        public string abel_station_name { get; set; }

        [StringLength(15)]
        public string abel_cost_center_code { get; set; }

        [StringLength(100)]
        public string abel_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abel_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abel_gl_expenses_name { get; set; }

        [StringLength(100)]
        public string abel_uom { get; set; }

        //[Column(TypeName = "numeric")]
        public decimal? abel_price_1 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_2 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_3 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_4 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_5 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_6 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_7 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_8 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_9 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_10 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_11 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_price_12 { get; set; }

        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_1 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_2 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_3 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_4 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_5 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_6 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_7 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_8 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_9 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_10 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_11 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_qty_12 { get; set; }

        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_1 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_2 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_3 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_4 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_5 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_6 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_7 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_8 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_9 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_10 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_11 { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_amount_12 { get; set; }

        //[Column(TypeName = "numeric")]
        public decimal? abel_total_qty_a { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_total_amount_a { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_total_qty_b { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_total_amount_b { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_grand_total_quantity { get; set; }
        
        //[Column(TypeName = "numeric")]
        public decimal? abel_grand_total_amount { get; set; }
        public int? abel_created_by { get; set; }
        public DateTime? last_modified { get; set; }
        public bool? fld_Deleted { get; set; }
    }
}
