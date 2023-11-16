using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesVehicleListViewModel2
    {
        public string ScreenName { get; set; }

        public int BudgetYear { get; set; }

        public string CostCenterCode { get; set; }

        public string CostCenterDesc { get; set; }

        public decimal? Total { get; set; }

        public string Revision { get; set; }
    }

    public class ExpensesVehicleListDetailViewModel2
    {
        public string ScreenName { get; set; }

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
        
        [StringLength(25)]
        public string abev_uom { get; set; }

        public decimal? abev_unit_price { get; set; }

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
    }

    public class ExpensesVehiclereateViewModel
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

    public class ExpensesVehicleCreateOrEditDetailViewModel
    {
        public int? abev_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
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
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abev_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abev_cost_center_desc { get; set; }

        [StringLength(11)]
        [Display(Name = "Kod Kenderaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_material_code { get; set; }

        [StringLength(100)]
        public string abev_material_name { get; set; }

        [StringLength(100)]
        [Display(Name = "Jenis Kenderaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_model { get; set; }

        [StringLength(100)]
        [Display(Name = "No Pendaftaran")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_registration_no { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_gl_expenses_code { get; set; }

        [StringLength(100)]
        public string abev_gl_expenses_name { get; set; }

        [Display(Name = "Kuantiti Setahun")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_quantity { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_uom { get; set; }

        [Display(Name = "Kos / Unit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_unit_price { get; set; }

        [Display(Name = "Jenis / Keterangan")]
        public string abev_note { get; set; }

        [Display(Name = "Bulan 1")]
        public string abev_quantity_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abev_quantity_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abev_quantity_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abev_quantity_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abev_quantity_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abev_quantity_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abev_quantity_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abev_quantity_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abev_quantity_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abev_quantity_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abev_quantity_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abev_quantity_12 { get; set; }

        public string abev_amount_1 { get; set; }

        public string abev_amount_2 { get; set; }

        public string abev_amount_3 { get; set; }

        public string abev_amount_4 { get; set; }

        public string abev_amount_5 { get; set; }

        public string abev_amount_6 { get; set; }

        public string abev_amount_7 { get; set; }

        public string abev_amount_8 { get; set; }

        public string abev_amount_9 { get; set; }

        public string abev_amount_10 { get; set; }

        public string abev_amount_11 { get; set; }

        public string abev_amount_12 { get; set; }

        [Display(Name = "Jumlah Kuantiti Tahunan")]
        public string abev_total_quantity { get; set; }

        [Display(Name = "Jumlah (RM) Tahunan")]
        public string abev_total { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_proration { get; set; }

        public bool abev_history { get; set; }

        [StringLength(50)]
        public string abev_status { get; set; }

        public int? abev_created_by { get; set; }

        public int? abev_revision { get; set; }
    }

    public class ExpensesVehicleDeleteViewModel
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

    public class ExpensesVehicleDeleteDetailViewModel
    {
        public int? abev_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
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
        [Display(Name = "Kod Cost Center")]
        public string abev_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abev_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "Kod GL Perbelanjaan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_gl_expenses_code { get; set; }

        [StringLength(100)]
        [Display(Name = "Penerangan GL Perbelanjaan")]
        public string abev_gl_expenses_name { get; set; }

        [Display(Name = "Kuantiti Setahun")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abev_quantity { get; set; }

        [StringLength(25)]
        [Display(Name = "UOM")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abev_uom { get; set; }

        [Display(Name = "Kos / Unit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abev_unit_price { get; set; }

        [Display(Name = "Bulan 1")]
        public decimal? abev_quantity_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public decimal? abev_quantity_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public decimal? abev_quantity_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public decimal? abev_quantity_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public decimal? abev_quantity_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public decimal? abev_quantity_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public decimal? abev_quantity_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public decimal? abev_quantity_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public decimal? abev_quantity_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public decimal? abev_quantity_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public decimal? abev_quantity_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public decimal? abev_quantity_12 { get; set; }

        public string abev_amount_1 { get; set; }

        public string abev_amount_2 { get; set; }

        public string abev_amount_3 { get; set; }

        public string abev_amount_4 { get; set; }

        public string abev_amount_5 { get; set; }

        public string abev_amount_6 { get; set; }

        public string abev_amount_7 { get; set; }

        public string abev_amount_8 { get; set; }

        public string abev_amount_9 { get; set; }

        public string abev_amount_10 { get; set; }

        public string abev_amount_11 { get; set; }

        public string abev_amount_12 { get; set; }

        [Display(Name = "Jumlah Kuantiti Tahunan")]
        public string abev_total_quantity { get; set; }

        [Display(Name = "Jumlah (RM) Tahunan")]
        public string abev_total { get; set; }
    }
}