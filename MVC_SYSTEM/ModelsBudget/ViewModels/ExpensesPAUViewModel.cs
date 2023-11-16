using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesPAUListViewModel
    {
        public string ScreenName { get; set; }

        public int BudgetYear { get; set; }

        public string CostCenterCode { get; set; }

        public string CostCenterDesc { get; set; }

        public decimal? Total { get; set; }
    }

    public class ExpensesPAUListDetailViewModel
    {
        public string ScreenName { get; set; }

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
    }

    public class ExpensesPAUCreateViewModel
    {
        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string BudgetYear { get; set; }

        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string CostCenter { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string Unit { get; set; }
    }

    public class ExpensesPAUCreateOrEditDetailViewModel
    {
        public int? abep_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
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
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abep_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abep_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abep_gl_expenses_name { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Pendapatan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_gl_income_code { get; set; }

        [StringLength(100)]
        public string abep_gl_income_name { get; set; }

        [StringLength(20)]
        [Display(Name = "Produk")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_product_code { get; set; }

        [StringLength(100)]
        public string abep_product_name { get; set; }

        [StringLength(15)]
        [Display(Name = "Cost Center Jualan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_cost_center_jualan_code { get; set; }

        [StringLength(100)]
        public string abep_cost_center_jualan_desc { get; set; }

        [Display(Name = "Kuantiti Setahun")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_quantity { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_uom_1 { get; set; }

        [Display(Name = "Kos / Unit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_rate { get; set; }

        [Display(Name = "Bulan 1")]
        public string abep_quantity_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abep_quantity_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abep_quantity_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abep_quantity_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abep_quantity_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abep_quantity_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abep_quantity_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abep_quantity_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abep_quantity_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abep_quantity_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abep_quantity_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abep_quantity_12 { get; set; }

        public string abep_amount_1 { get; set; }

        public string abep_amount_2 { get; set; }

        public string abep_amount_3 { get; set; }

        public string abep_amount_4 { get; set; }

        public string abep_amount_5 { get; set; }

        public string abep_amount_6 { get; set; }

        public string abep_amount_7 { get; set; }

        public string abep_amount_8 { get; set; }

        public string abep_amount_9 { get; set; }

        public string abep_amount_10 { get; set; }

        public string abep_amount_11 { get; set; }

        public string abep_amount_12 { get; set; }

        [Display(Name = "Jumlah Kuantiti Tahunan")]
        public string abep_total_quantity { get; set; }

        [Display(Name = "Jumlah (RM) Tahunan")]
        public string abep_total { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_proration { get; set; }

        public bool abep_history { get; set; }

        [StringLength(50)]
        public string abep_status { get; set; }

        public int? abep_created_by { get; set; }
    }

    public class ExpensesPAUDeleteViewModel
    {
        [Display(Name = "Tahun Bajet")]
        public int BudgetYear { get; set; }

        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string CostCenterCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Keterangan Cost Center")]
        public string CostCenterDesc { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public string Total { get; set; }
    }

    public class ExpensesPAUDeleteDetailViewModel
    {
        public int? abep_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
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
        [Display(Name = "Kod Cost Center")]
        public string abep_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abep_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "Kod GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_gl_expenses_code { get; set; }

        [StringLength(100)]
        [Display(Name = "Penerangan GL Perbelanjaan")]
        public string abep_gl_expenses_name { get; set; }

        [StringLength(20)]
        [Display(Name = "Kod GL Pendapatan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_gl_income_code { get; set; }

        [StringLength(100)]
        public string abep_gl_income_name { get; set; }

        [StringLength(20)]
        [Display(Name = "Produk")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_product_code { get; set; }

        [StringLength(100)]
        public string abep_product_name { get; set; }

        [StringLength(15)]
        [Display(Name = "Kod Cost Center Jualan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_cost_center_jualan_code { get; set; }

        [StringLength(100)]
        public string abep_cost_center_jualan_desc { get; set; }

        [Display(Name = "Kuantiti Setahun")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abep_quantity { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abep_uom_1 { get; set; }

        [Display(Name = "Kos / Unit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abep_rate { get; set; }

        [Display(Name = "Bulan 1")]
        public decimal? abep_quantity_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public decimal? abep_quantity_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public decimal? abep_quantity_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public decimal? abep_quantity_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public decimal? abep_quantity_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public decimal? abep_quantity_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public decimal? abep_quantity_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public decimal? abep_quantity_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public decimal? abep_quantity_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public decimal? abep_quantity_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public decimal? abep_quantity_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public decimal? abep_quantity_12 { get; set; }

        public string abep_amount_1 { get; set; }

        public string abep_amount_2 { get; set; }

        public string abep_amount_3 { get; set; }

        public string abep_amount_4 { get; set; }

        public string abep_amount_5 { get; set; }

        public string abep_amount_6 { get; set; }

        public string abep_amount_7 { get; set; }

        public string abep_amount_8 { get; set; }

        public string abep_amount_9 { get; set; }

        public string abep_amount_10 { get; set; }

        public string abep_amount_11 { get; set; }

        public string abep_amount_12 { get; set; }

        [Display(Name = "Jumlah Kuantiti Tahunan")]
        public string abep_total_quantity { get; set; }

        [Display(Name = "Jumlah (RM) Tahunan")]
        public string abep_total { get; set; }
    }
}