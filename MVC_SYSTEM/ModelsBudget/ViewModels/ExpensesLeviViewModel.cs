using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesLeviListViewModel
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

    public class ExpensesLeviDetailViewModel
    {
        public string ScreenName { get; set; }
        public int? abel_budgeting_year { get; set; }
        public string abel_company_category { get; set; }
        public string abel_product_code { get; set; }
        public string abel_product_name { get; set; }
        public string abel_company_code { get; set; }
        public string abel_company_name { get; set; }
        public string abel_cost_center_code { get; set; }
        public string abel_cost_center_desc { get; set; }
        public string abel_gl_expenses_code { get; set; }
        public string abel_gl_expenses_name { get; set; }
        public decimal? abel_price_1 { get; set; }
        public decimal? abel_price_2 { get; set; }
        public decimal? abel_price_3 { get; set; }
        public decimal? abel_price_4 { get; set; }
        public decimal? abel_price_5 { get; set; }
        public decimal? abel_price_6 { get; set; }
        public decimal? abel_price_7 { get; set; }
        public decimal? abel_price_8 { get; set; }
        public decimal? abel_price_9 { get; set; }
        public decimal? abel_price_10 { get; set; }
        public decimal? abel_price_11 { get; set; }
        public decimal? abel_price_12 { get; set; }
        public decimal? abel_qty_1 { get; set; }
        public decimal? abel_qty_2 { get; set; }
        public decimal? abel_qty_3 { get; set; }
        public decimal? abel_qty_4 { get; set; }
        public decimal? abel_qty_5 { get; set; }
        public decimal? abel_qty_6 { get; set; }
        public decimal? abel_qty_7 { get; set; }
        public decimal? abel_qty_8 { get; set; }
        public decimal? abel_qty_9 { get; set; }
        public decimal? abel_qty_10 { get; set; }
        public decimal? abel_qty_11 { get; set; }
        public decimal? abel_qty_12 { get; set; }
        public decimal? abel_amount_1 { get; set; }
        public decimal? abel_amount_2 { get; set; }
        public decimal? abel_amount_3 { get; set; }
        public decimal? abel_amount_4 { get; set; }
        public decimal? abel_amount_5 { get; set; }
        public decimal? abel_amount_6 { get; set; }
        public decimal? abel_amount_7 { get; set; }
        public decimal? abel_amount_8 { get; set; }
        public decimal? abel_amount_9 { get; set; }
        public decimal? abel_amount_10 { get; set; }
        public decimal? abel_amount_11 { get; set; }
        public decimal? abel_amount_12 { get; set; }
        public decimal? abel_total_qty_a { get; set; }
        public decimal? abel_total_amount_a { get; set; }
        public decimal? abel_total_qty_b { get; set; }
        public decimal? abel_total_amount_b { get; set; }
        public decimal? abel_grand_total_quantity { get; set; }
        public decimal? abel_grand_total_amount { get; set; }
        public string abel_status { get; set; }
    }
}