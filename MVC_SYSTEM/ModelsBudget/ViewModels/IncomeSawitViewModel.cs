using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class IncomeSawitListViewModel
    {
        public string ScreenName { get; set; }

        public string CostCenterCode { get; set; }

        public string CostCenterDesc { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Total { get; set; }

        public string JenisProduk { get; set; }

        public int BudgetYear { get; set; }

        public string Revision { get; set; }
    }

    public class IncomeSawitDetailViewModel
    {
        public string ScreenName { get; set; }
        public int? abio_budgeting_year { get; set; }
        public string abio_company_category { get; set; }
        public string abio_product_code { get; set; }
        public string abio_product_name { get; set; }
        public string abio_company_code { get; set; }
        public string abio_company_name { get; set; }
        public string abio_cost_center { get; set; }
        public string abio_cost_center_desc { get; set; }
        public string abio_gl_code { get; set; }
        public string abio_gl_desc { get; set; }
        public decimal? abio_price_1 { get; set; }
        public decimal? abio_price_2 { get; set; }
        public decimal? abio_price_3 { get; set; }
        public decimal? abio_price_4 { get; set; }
        public decimal? abio_price_5 { get; set; }
        public decimal? abio_price_6 { get; set; }
        public decimal? abio_price_7 { get; set; }
        public decimal? abio_price_8 { get; set; }
        public decimal? abio_price_9 { get; set; }
        public decimal? abio_price_10 { get; set; }
        public decimal? abio_price_11 { get; set; }
        public decimal? abio_price_12 { get; set; }

        public decimal? abio_qty_1 { get; set; }
        public decimal? abio_qty_2 { get; set; }
        public decimal? abio_qty_3 { get; set; }
        public decimal? abio_qty_4 { get; set; }
        public decimal? abio_qty_5 { get; set; }
        public decimal? abio_qty_6 { get; set; }
        public decimal? abio_qty_7 { get; set; }
        public decimal? abio_qty_8 { get; set; }
        public decimal? abio_qty_9 { get; set; }
        public decimal? abio_qty_10 { get; set; }
        public decimal? abio_qty_11 { get; set; }
        public decimal? abio_qty_12 { get; set; }
        public decimal? abio_amount_1 { get; set; }
        public decimal? abio_amount_2 { get; set; }
        public decimal? abio_amount_3 { get; set; }
        public decimal? abio_amount_4 { get; set; }
        public decimal? abio_amount_5 { get; set; }
        public decimal? abio_amount_6 { get; set; }
        public decimal? abio_amount_7 { get; set; }
        public decimal? abio_amount_8 { get; set; }
        public decimal? abio_amount_9 { get; set; }

        //[Column(TypeName = "numeric")]
        public decimal? abio_amount_10 { get; set; }

        //[Column(TypeName = "numeric")]
        public decimal? abio_amount_11 { get; set; }
        public decimal? abio_amount_12 { get; set; }
        public decimal? abio_total_qty_a { get; set; }
        public decimal? abio_total_amount_a { get; set; }
        public decimal? abio_total_qty_b { get; set; }
        public decimal? abio_total_amount_b { get; set; }
        public decimal? abio_grand_total_quantity { get; set; }
        public decimal? abio_grand_total_amount { get; set; }
        public string abio_status { get; set; }
    }

    public class IncomeSawitCreateViewModel
    {
        public int abio_id { get; set; }
        public int? abio_revision { get; set; }
        [StringLength(20)]
        public string abio_status { get; set; }
        public bool? abio_history { get; set; }
        public int? abio_budgeting_year { get; set; }

        [StringLength(50)]
        public string abio_company_category { get; set; }

        [StringLength(50)]
        public string abio_company_code { get; set; }

        [StringLength(100)]
        public string abio_company_name { get; set; }

        [StringLength(50)]
        public string abio_product_code { get; set; }

        [StringLength(100)]
        public string abio_product_name { get; set; }

        [StringLength(50)]
        public string abio_station_code { get; set; }

        [StringLength(100)]
        public string abio_station_name { get; set; }

        [StringLength(15)]
        public string abio_cost_center { get; set; }

        [StringLength(100)]
        public string abio_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abio_gl_code { get; set; }

        [StringLength(100)]
        public string abio_gl_desc { get; set; }

        [StringLength(100)]
        public string abio_uom { get; set; }

        public string abio_price_1 { get; set; }

        public string abio_price_2 { get; set; }

        public string abio_price_3 { get; set; }

        public string abio_price_4 { get; set; }

        public string abio_price_5 { get; set; }

        public string abio_price_6 { get; set; }

        public string abio_price_7 { get; set; }

        public string abio_price_8 { get; set; }

        public string abio_price_9 { get; set; }

        public string abio_price_10 { get; set; }

        public string abio_price_11 { get; set; }

        public string abio_price_12 { get; set; }

        public string abio_qty_1 { get; set; }

        public string abio_qty_2 { get; set; }

        public string abio_qty_3 { get; set; }

        public string abio_qty_4 { get; set; }

        public string abio_qty_5 { get; set; }

        public string abio_qty_6 { get; set; }

        public string abio_qty_7 { get; set; }

        public string abio_qty_8 { get; set; }

        public string abio_qty_9 { get; set; }

        public string abio_qty_10 { get; set; }

        public string abio_qty_11 { get; set; }

        public string abio_qty_12 { get; set; }

        public string abio_amount_1 { get; set; }

        public string abio_amount_2 { get; set; }

        public string abio_amount_3 { get; set; }

        public string abio_amount_4 { get; set; }

        public string abio_amount_5 { get; set; }

        public string abio_amount_6 { get; set; }

        public string abio_amount_7 { get; set; }

        public string abio_amount_8 { get; set; }

        public string abio_amount_9 { get; set; }

        public string abio_amount_10 { get; set; }

        public string abio_amount_11 { get; set; }

        public string abio_amount_12 { get; set; }

        public string abio_total_qty_a { get; set; }

        public string abio_total_amount_a { get; set; }

        public string abio_total_qty_b { get; set; }

        public string abio_total_amount_b { get; set; }

        public string abio_grand_total_quantity { get; set; }

        public string abio_grand_total_amount { get; set; }

        public DateTime? last_modified { get; set; }
        public bool? fld_Deleted { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }
        public int? abio_fld_ID { get; set; }//add aug
    }

    public class IncomeSawitUpdateViewModel
    {
        public int abio_id { get; set; }
        public int? abio_revision { get; set; }

        [StringLength(20)]
        public string abio_status { get; set; }

        public bool? abio_history { get; set; }

        public int? abio_budgeting_year { get; set; }

        [StringLength(50)]
        public string abio_company_category { get; set; }

        [StringLength(50)]
        public string abio_company_code { get; set; }

        [StringLength(100)]
        public string abio_company_name { get; set; }

        [StringLength(50)]
        public string abio_product_code { get; set; }

        [StringLength(100)]
        public string abio_product_name { get; set; }

        [StringLength(50)]
        public string abio_station_code { get; set; }

        [StringLength(100)]
        public string abio_station_name { get; set; }

        [StringLength(15)]
        public string abio_cost_center { get; set; }

        [StringLength(100)]
        public string abio_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abio_gl_code { get; set; }

        [StringLength(100)]
        public string abio_gl_desc { get; set; }

        [StringLength(100)]
        public string abio_uom { get; set; }

        public decimal? abio_price_1 { get; set; }

        public decimal? abio_price_2 { get; set; }

        public decimal? abio_price_3 { get; set; }

        public decimal? abio_price_4 { get; set; }

        public decimal? abio_price_5 { get; set; }

        public decimal? abio_price_6 { get; set; }

        public decimal? abio_price_7 { get; set; }

        public decimal? abio_price_8 { get; set; }

        public decimal? abio_price_9 { get; set; }

        public decimal? abio_price_10 { get; set; }

        public decimal? abio_price_11 { get; set; }

        public decimal? abio_price_12 { get; set; }

        public string abio_qty_1 { get; set; }

        public string abio_qty_2 { get; set; }

        public string abio_qty_3 { get; set; }

        public string abio_qty_4 { get; set; }

        public string abio_qty_5 { get; set; }

        public string abio_qty_6 { get; set; }

        public string abio_qty_7 { get; set; }

        public string abio_qty_8 { get; set; }

        public string abio_qty_9 { get; set; }

        public string abio_qty_10 { get; set; }

        public string abio_qty_11 { get; set; }

        public string abio_qty_12 { get; set; }

        public string abio_amount_1 { get; set; }

        public string abio_amount_2 { get; set; }

        public string abio_amount_3 { get; set; }

        public string abio_amount_4 { get; set; }

        public string abio_amount_5 { get; set; }

        public string abio_amount_6 { get; set; }

        public string abio_amount_7 { get; set; }

        public string abio_amount_8 { get; set; }

        public string abio_amount_9 { get; set; }

        public string abio_amount_10 { get; set; }

        public string abio_amount_11 { get; set; }

        public string abio_amount_12 { get; set; }

        public string abio_total_qty_a { get; set; }

        public string abio_total_amount_a { get; set; }

        public string abio_total_qty_b { get; set; }

        public string abio_total_amount_b { get; set; }

        public string abio_grand_total_quantity { get; set; }

        public string abio_grand_total_amount { get; set; }

        public DateTime? last_modified { get; set; }
        public bool? fld_Deleted { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }
        public int? abio_fld_ID { get; set; }//add aug
    }
}