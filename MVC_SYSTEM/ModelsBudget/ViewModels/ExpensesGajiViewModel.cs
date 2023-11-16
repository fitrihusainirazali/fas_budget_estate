using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesGajiListViewModel
    {
        public string ScreenName { get; set; }

        public int BudgetYear { get; set; }

        public string CostCenterCode { get; set; }

        public string CostCenterDesc { get; set; }

        public string KodAktvt { get; set; }

        public string AktvtDesc { get; set; }

        public string Hectare { get; set; }

        public decimal? Total { get; set; }
    }

    public class ExpensesGajiListDetailViewModel
    {
        public string ScreenName { get; set; }

        public int abeg_id { get; set; }

        public int? abeg_revision { get; set; }

        public int abeg_budgeting_year { get; set; }

        public int? abeg_syarikat_id { get; set; }

        [StringLength(100)]
        public string abeg_syarikat_name { get; set; }

        public int? abeg_ladang_id { get; set; }

        [StringLength(5)]
        public string abeg_ladang_code { get; set; }

        [StringLength(50)]
        public string abeg_ladang_name { get; set; }

        public int? abeg_wilayah_id { get; set; }

        [StringLength(50)]
        public string abeg_wilayah_name { get; set; }

        [StringLength(15)]
        public string abeg_cost_center_code { get; set; }


        [StringLength(100)]
        public string abeg_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abeg_hectare { get; set; }

        [StringLength(20)]
        public string abeg_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abeg_gl_expenses_name { get; set; }

        [StringLength(20)]
        public string abeg_akt_code { get; set; }

        [StringLength(255)]
        public string abeg_akt_name { get; set; }

        [StringLength(100)]
        public string abeg_note { get; set; }

        public decimal? abeg_month_1 { get; set; }

        public decimal? abeg_month_2 { get; set; }

        public decimal? abeg_month_3 { get; set; }

        public decimal? abeg_month_4 { get; set; }

        public decimal? abeg_month_5 { get; set; }

        public decimal? abeg_month_6 { get; set; }


        public decimal? abeg_month_7 { get; set; }

        public decimal? abeg_month_8 { get; set; }

        public decimal? abeg_month_9 { get; set; }

        public decimal? abeg_month_10 { get; set; }


        public decimal? abeg_month_11 { get; set; }

        public decimal? abeg_month_12 { get; set; }

        public decimal? abeg_total { get; set; }

        public string abeg_proration { get; set; }

        public int? abeg_created_by { get; set; }


        public DateTime? last_modified { get; set; }


        public bool abeg_Deleted { get; set; }
    }

    public class ExpensesGajiCreateViewModel
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

        [StringLength(100)]
        [Display(Name = "txthectare", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string Hectare { get; set; }
    }

    public class ExpensesGajiCreateDetailViewModel
    {
        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        public int abeg_budgeting_year { get; set; }

        public int? abeg_syarikat_id { get; set; }

        [StringLength(100)]
        public string abeg_syarikat_name { get; set; }

        public int? abeg_ladang_id { get; set; }

        [StringLength(5)]
        public string abeg_ladang_code { get; set; }

        [StringLength(50)]
        public string abeg_ladang_name { get; set; }

        public int? abeg_wilayah_id { get; set; }

        [StringLength(50)]
        public string abeg_wilayah_name { get; set; }

        
        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abeg_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abeg_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abeg_hectare { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abeg_gl_expenses_name { get; set; }

        [StringLength(20)]
        [Display(Name = "Aktiviti")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_akt_code { get; set; }

        [StringLength(255)]
        public string abeg_akt_name { get; set; }

        [StringLength(100)]
        [Display(Name = "Nota")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_note { get; set; }

        
        [Display(Name = "Jumlah (RM)")]
        public string abeg_total { get; set; }

        [Display(Name = "Bulan 1")]
        public string abeg_month_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abeg_month_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abeg_month_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abeg_month_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abeg_month_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abeg_month_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abeg_month_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abeg_month_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abeg_month_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abeg_month_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abeg_month_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abeg_month_12 { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_proration { get; set; }

       
        public int? abeg_created_by { get; set; }

        public int? abeg_revision { get; set; }

        public string abeg_status { get; set; }

        //public bool? abeg_history { get; set; }
    }

    public class ExpensesGajiEditDetailViewModel
    {
        public int? abeg_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        public int abeg_budgeting_year { get; set; }

        public int? abeg_syarikat_id { get; set; }

        [StringLength(100)]
        public string abeg_syarikat_name { get; set; }

        public int? abeg_ladang_id { get; set; }

        [StringLength(5)]
        public string abeg_ladang_code { get; set; }

        [StringLength(50)]
        public string abeg_ladang_name { get; set; }

        public int? abeg_wilayah_id { get; set; }

        [StringLength(50)]
        public string abeg_wilayah_name { get; set; }

        
        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abeg_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abeg_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abeg_gl_expenses_name { get; set; }

        [StringLength(20)]
        [Display(Name = "Aktiviti")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_akt_code { get; set; }

        [StringLength(255)]
        public string abeg_akt_name { get; set; }

        [StringLength(100)]
        [Display(Name = "Nota")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_note { get; set; }

        [StringLength(20)]
        public string abeg_hectare { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public string abeg_total { get; set; }

        [Display(Name = "Bulan 1")]
        public string abeg_month_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abeg_month_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abeg_month_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abeg_month_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abeg_month_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abeg_month_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abeg_month_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abeg_month_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abeg_month_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abeg_month_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abeg_month_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abeg_month_12 { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_proration { get; set; }

        public int? abeg_created_by { get; set; }

        public int? abeg_revision { get; set; }

        public string abeg_status { get; set; }

        //public bool? abeg_history { get; set; }


    }

    public class ExpensesGajiDeleteViewModel
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

    public class ExpensesGajiDeleteDetailViewModel
    {
        public int? abeg_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        public int abeg_budgeting_year { get; set; }

        public int? abeg_syarikat_id { get; set; }

        [StringLength(100)]
        public string abeg_syarikat_name { get; set; }

        public int? abeg_ladang_id { get; set; }

        [StringLength(5)]
        public string abeg_ladang_code { get; set; }

        [StringLength(50)]
        public string abeg_ladang_name { get; set; }

        public int? abeg_wilayah_id { get; set; }

        [StringLength(50)]
        public string abeg_wilayah_name { get; set; }

       
        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abeg_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abeg_cost_center_desc { get; set; }

        [StringLength(20)]
        public string abeg_hectare { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_gl_expenses_code { get; set; }

        [StringLength(100)]
        [Display(Name = "lblGLDesc", ResourceType = typeof(GlobalResEstate))]
        public string abeg_gl_expenses_name { get; set; }

        [StringLength(20)]
        [Display(Name = "Aktiviti")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_akt_code { get; set; }

        [StringLength(255)]
        [Display(Name = "lblAktvtDesc", ResourceType = typeof(GlobalResEstate))]
        public string abeg_akt_name { get; set; }

        [StringLength(100)]
        [Display(Name = "Nota")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_note { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abeg_uom { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public string abeg_total { get; set; }
    }
}