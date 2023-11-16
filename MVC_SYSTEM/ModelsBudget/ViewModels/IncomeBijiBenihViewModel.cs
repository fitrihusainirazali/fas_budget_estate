using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class IncomeBijiBenihListViewModel
    {
        public string ScreenName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterDesc { get; set; }
        public string JenisProduk { get; set; }
        public decimal? Quantity1 { get; set; }
        public decimal? Quantity2 { get; set; }
        public decimal? AllQuantity { get; set; }
        public decimal? Amount1 { get; set; }
        public decimal? Amount2 { get; set; }
        public decimal? AllAmount { get; set; }
        public int BudgetYear { get; set; }
        public string GlCode { get; set; }
        public string Revision { get; set; }
    }
    public class IncomeBijiBenihCreateEditViewModel //new sept
    {
        public string ScreenName { get; set; }
        public int abis_id { get; set; }
        public int? abis_budgeting_year { get; set; }
        public int? abis_syarikat_id { get; set; }
        public string abis_syarikat_name { get; set; }
        public int? abis_ladang_id { get; set; }
        public string abis_ladang_code { get; set; }
        public string abis_ladang_name { get; set; }
        public int? abis_wilayah_id { get; set; }
        public string abis_wilayah_name { get; set; }
        public int? abis_fld_ID { get; set; }//new sept
        public string abis_company_category { get; set; }
        public string abis_company_code { get; set; }
        public string abis_company_name { get; set; }
        public string abis_product_code { get; set; }
        public string abis_product_name { get; set; }
        public string abis_station_code { get; set; }
        public string abis_station_name { get; set; }
        public string abis_cost_center { get; set; }
        public string abis_cost_center_desc { get; set; }
        public string abis_gl_code { get; set; }
        public string abis_gl_desc { get; set; }
        public string abis_uom { get; set; }

        public string abis_unit_price { get; set; }
        public string abis_qty_1 { get; set; }
        public string abis_qty_2 { get; set; }
        public string abis_qty_3 { get; set; }
        public string abis_qty_4 { get; set; }
        public string abis_qty_5 { get; set; }
        public string abis_qty_6 { get; set; }
        public string abis_qty_7 { get; set; }
        public string abis_qty_8 { get; set; }
        public string abis_qty_9 { get; set; }
        public string abis_qty_10 { get; set; }
        public string abis_qty_11 { get; set; }
        public string abis_qty_12 { get; set; }
        public string abis_amount_1 { get; set; }
        public string abis_amount_2 { get; set; }
        public string abis_amount_3 { get; set; }
        public string abis_amount_4 { get; set; }
        public string abis_amount_5 { get; set; }
        public string abis_amount_6 { get; set; }
        public string abis_amount_7 { get; set; }
        public string abis_amount_8 { get; set; }
        public string abis_amount_9 { get; set; }
        public string abis_amount_10 { get; set; }
        public string abis_amount_11 { get; set; }
        public string abis_amount_12 { get; set; }
        public string abis_total_qty_a { get; set; }
        public string abis_total_qty_b { get; set; }
        public string abis_total_amount_a { get; set; }
        public string abis_total_amount_b { get; set; }
        public string abis_grand_amount1 { get; set; }
        public string abis_grand_amount2 { get; set; }
        public string abis_grand_quantity1 { get; set; }
        public string abis_grand_quantity2 { get; set; }
        public string abis_grand_total_quantity { get; set; }
        public string abis_grand_total_amount { get; set; }
        public string abis_status { get; set; }
    }
    public class IncomeBijiBenihDetailViewModel
    {
        public string ScreenName { get; set; }
        public int abis_id { get; set; }
        public int? abis_budgeting_year { get; set; }
        public int? abis_syarikat_id { get; set; }
        public string abis_syarikat_name { get; set; }
        public int? abis_ladang_id { get; set; }
        public string abis_ladang_code { get; set; }
        public string abis_ladang_name { get; set; }
        public int? abis_wilayah_id { get; set; }
        public string abis_wilayah_name { get; set; }
        public string abis_company_category { get; set; }
        public string abis_company_code { get; set; }
        public string abis_company_name { get; set; }
        public string abis_product_code { get; set; }
        public string abis_product_name { get; set; }
        public string abis_station_code { get; set; }
        public string abis_station_name { get; set; }
        public string abis_cost_center { get; set; }
        public string abis_cost_center_desc { get; set; }
        public string abis_gl_code { get; set; }
        public string abis_gl_desc { get; set; }
        public string abis_uom { get; set; }
        public decimal? abis_unit_price { get; set; }
        public decimal? abis_qty_1 { get; set; }
        public decimal? abis_qty_2 { get; set; }
        public decimal? abis_qty_3 { get; set; }
        public decimal? abis_qty_4 { get; set; }
        public decimal? abis_qty_5 { get; set; }
        public decimal? abis_qty_6 { get; set; }
        public decimal? abis_qty_7 { get; set; }
        public decimal? abis_qty_8 { get; set; }
        public decimal? abis_qty_9 { get; set; }
        public decimal? abis_qty_10 { get; set; }
        public decimal? abis_qty_11 { get; set; }
        public decimal? abis_qty_12 { get; set; }
        public decimal? abis_amount_1 { get; set; }
        public decimal? abis_amount_2 { get; set; }
        public decimal? abis_amount_3 { get; set; }
        public decimal? abis_amount_4 { get; set; }
        public decimal? abis_amount_5 { get; set; }
        public decimal? abis_amount_6 { get; set; }
        public decimal? abis_amount_7 { get; set; }
        public decimal? abis_amount_8 { get; set; }
        public decimal? abis_amount_9 { get; set; }
        public decimal? abis_amount_10 { get; set; }
        public decimal? abis_amount_11 { get; set; }
        public decimal? abis_amount_12 { get; set; }
        public decimal? abis_total_qty_a { get; set; }
        public decimal? abis_total_qty_b { get; set; }
        public decimal? abis_total_amount_a { get; set; }
        public decimal? abis_total_amount_b { get; set; }
        public decimal? abis_grand_amount1 { get; set; }
        public decimal? abis_grand_amount2 { get; set; }
        public decimal? abis_grand_quantity1 { get; set; }
        public decimal? abis_grand_quantity2 { get; set; }
        public decimal? abis_grand_total_quantity { get; set; }
        public decimal? abis_grand_total_amount { get; set; }
        public string abis_status { get; set; }
    }
}