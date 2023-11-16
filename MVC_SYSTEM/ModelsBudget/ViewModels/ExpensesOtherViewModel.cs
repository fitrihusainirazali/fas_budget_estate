using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesOtherListViewModel
    {
        public string ScreenName { get; set; }

        public int BudgetYear { get; set; }

        public string CostCenterCode { get; set; }

        public string CostCenterDesc { get; set; }

        public decimal? Total { get; set; }
    }

    public class ExpensesOtherListDetailViewModel
    {
        public string ScreenName { get; set; }

        public int? abeo_id { get; set; }

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
    }

    public class ExpensesOtherCreateViewModel
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

    public class ExpensesOtherCreateDetailViewModel
    {
        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
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
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abeo_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abeo_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abeo_gl_expenses_name { get; set; }

        [StringLength(100)]
        [Display(Name = "Nota")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_note { get; set; }

        [Display(Name = "Unit 1")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_unit_1 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 1")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_uom_1 { get; set; }

        [StringLength(25)]
        [Display(Name = "Operation 1")]
        public string abeo_operation_1 { get; set; }

        [Display(Name = "Unit 2")]
        public string abeo_unit_2 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 2")]
        public string abeo_uom_2 { get; set; }

        [StringLength(25)]
        [Display(Name = "Operation 2")]
        public string abeo_operation_2 { get; set; }

        [Display(Name = "Unit 3")]
        public string abeo_unit_3 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 3")]
        public string abeo_uom_3 { get; set; }

        [StringLength(25)]
        [Display(Name = "@")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_operation_3 { get; set; }

        [Display(Name = "Rate (RM)")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_rate { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public string abeo_total { get; set; }

        [Display(Name = "Bulan 1")]
        public string abeo_month_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abeo_month_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abeo_month_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abeo_month_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abeo_month_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abeo_month_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abeo_month_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abeo_month_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abeo_month_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abeo_month_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abeo_month_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abeo_month_12 { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_proration { get; set; }

        public bool abeo_history { get; set; }

        [StringLength(50)]
        public string abeo_status { get; set; }

        public int? abeo_created_by { get; set; }
    }

    public class ExpensesOtherEditDetailViewModel
    {
        public int? abeo_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
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
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abeo_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abeo_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abeo_gl_expenses_name { get; set; }

        [StringLength(100)]
        [Display(Name = "Nota")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_note { get; set; }

        [Display(Name = "Unit 1")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_unit_1 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 1")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_uom_1 { get; set; }

        [StringLength(25)]
        [Display(Name = "Operation 1")]
        public string abeo_operation_1 { get; set; }

        [Display(Name = "Unit 2")]
        public string abeo_unit_2 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 2")]
        public string abeo_uom_2 { get; set; }

        [StringLength(25)]
        [Display(Name = "Operation 2")]
        public string abeo_operation_2 { get; set; }

        [Display(Name = "Unit 3")]
        public string abeo_unit_3 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 3")]
        public string abeo_uom_3 { get; set; }

        [StringLength(25)]
        [Display(Name = "@")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_operation_3 { get; set; }

        [Display(Name = "Rate (RM)")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_rate { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public string abeo_total { get; set; }

        [Display(Name = "Bulan 1")]
        public string abeo_month_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abeo_month_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abeo_month_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abeo_month_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abeo_month_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abeo_month_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abeo_month_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abeo_month_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abeo_month_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abeo_month_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abeo_month_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abeo_month_12 { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_proration { get; set; }

        public bool abeo_history { get; set; }

        [StringLength(50)]
        public string abeo_status { get; set; }

        public int? abeo_created_by { get; set; }
    }

    public class ExpensesOtherDeleteViewModel
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

    public class ExpensesOtherDeleteDetailViewModel
    {
        public int? abeo_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
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
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abeo_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abeo_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_gl_expenses_code { get; set; }

        [StringLength(100)]
        [Display(Name = "lblGLDesc", ResourceType = typeof(GlobalResEstate))]
        public string abeo_gl_expenses_name { get; set; }

        [StringLength(100)]
        [Display(Name = "Nota")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_note { get; set; }

        [Display(Name = "Unit 1")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abeo_unit_1 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 1")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_uom_1 { get; set; }

        [StringLength(25)]
        [Display(Name = "Operation 1")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_operation_1 { get; set; }

        [Display(Name = "Unit 2")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abeo_unit_2 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 2")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_uom_2 { get; set; }

        [StringLength(25)]
        [Display(Name = "Operation 2")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_operation_2 { get; set; }

        [Display(Name = "Unit 3")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abeo_unit_3 { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM 3")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_uom_3 { get; set; }

        [StringLength(25)]
        [Display(Name = "@")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeo_operation_3 { get; set; }

        [Display(Name = "Rate (RM)")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abeo_rate { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public string abeo_total { get; set; }
    }
}