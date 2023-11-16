using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesCapitalListViewModel
    {
        public string ScreenName { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        public int BudgetYear { get; set; }

        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string CostCenterCode { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string CostCenterDesc { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public decimal? Total { get; set; }
    }

    public class ExpensesCapitalListDetailViewModel
    {
        public string ScreenName { get; set; }

        public int? abce_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        public int abce_budgeting_year { get; set; }

        public int? abce_syarikat_id { get; set; }

        [StringLength(100)]
        public string abce_syarikat_name { get; set; }

        public int? abce_ladang_id { get; set; }

        [StringLength(5)]
        public string abce_ladang_code { get; set; }

        [StringLength(50)]
        public string abce_ladang_name { get; set; }

        public int? abce_wilayah_id { get; set; }

        [StringLength(50)]
        public string abce_wilayah_name { get; set; }

        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Aset")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_gl_code { get; set; }

        [StringLength(100)]
        public string abce_gl_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Jenis Keterangan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "Jenis Belian")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_jenis_code { get; set; }

        [StringLength(100)]
        public string abce_jenis_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Justifikasi")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_reason { get; set; }

        [StringLength(100)]
        public string abce_material_details { get; set; }

        [Display(Name = "hdrQuantity", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abce_unit_1 { get; set; }

        public decimal? abce_uom_1 { get; set; }

        [Display(Name = "Harga Seunit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abce_rate { get; set; }

        [Display(Name = "Bulan 1")]
        public decimal? abce_quantity_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public decimal? abce_quantity_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public decimal? abce_quantity_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public decimal? abce_quantity_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public decimal? abce_quantity_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public decimal? abce_quantity_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public decimal? abce_quantity_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public decimal? abce_quantity_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public decimal? abce_quantity_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public decimal? abce_quantity_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public decimal? abce_quantity_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public decimal? abce_quantity_12 { get; set; }

        public decimal? abce_amount_1 { get; set; }

        public decimal? abce_amount_2 { get; set; }

        public decimal? abce_amount_3 { get; set; }

        public decimal? abce_amount_4 { get; set; }

        public decimal? abce_amount_5 { get; set; }

        public decimal? abce_amount_6 { get; set; }

        public decimal? abce_amount_7 { get; set; }

        public decimal? abce_amount_8 { get; set; }

        public decimal? abce_amount_9 { get; set; }

        public decimal? abce_amount_10 { get; set; }

        public decimal? abce_amount_11 { get; set; }

        public decimal? abce_amount_12 { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public decimal? abce_total { get; set; }

        [Display(Name = "Jumlah Kuantiti Tahunan")]
        public decimal? abce_total_quantity { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_proration { get; set; }

        public bool abce_history { get; set; }

        [StringLength(50)]
        public string abce_status { get; set; }

        [StringLength(100)]
        [Display(Name = "Kategori Aset")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_ast_code { get; set; }

        [StringLength(50)]
        public string abce_ast_category { get; set; }
    }

    public class ExpensesCapitalCreateHeaderViewModel
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

    public class ExpensesCapitalCreateViewModel
    {
        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        public int abce_budgeting_year { get; set; }

        public int? abce_syarikat_id { get; set; }

        [StringLength(100)]
        public string abce_syarikat_name { get; set; }

        public int? abce_ladang_id { get; set; }

        [StringLength(5)]
        public string abce_ladang_code { get; set; }

        [StringLength(50)]
        public string abce_ladang_name { get; set; }

        public int? abce_wilayah_id { get; set; }

        [StringLength(50)]
        public string abce_wilayah_name { get; set; }

        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Aset")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_gl_code { get; set; }

        [StringLength(100)]
        public string abce_gl_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Jenis Keterangan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "Jenis Belian")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_jenis_code { get; set; }

        [StringLength(100)]
        public string abce_jenis_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Justifikasi")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_reason { get; set; }

        [StringLength(100)]
        public string abce_material_details { get; set; }

        [Display(Name = "hdrQuantity", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_unit_1 { get; set; }

        public decimal? abce_uom_1 { get; set; }

        [Display(Name = "Harga Seunit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_rate { get; set; }

        [Display(Name = "Bulan 1")]
        public string abce_quantity_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abce_quantity_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abce_quantity_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abce_quantity_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abce_quantity_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abce_quantity_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abce_quantity_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abce_quantity_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abce_quantity_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abce_quantity_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abce_quantity_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abce_quantity_12 { get; set; }

        public string abce_amount_1 { get; set; }

        public string abce_amount_2 { get; set; }

        public string abce_amount_3 { get; set; }

        public string abce_amount_4 { get; set; }

        public string abce_amount_5 { get; set; }

        public string abce_amount_6 { get; set; }

        public string abce_amount_7 { get; set; }

        public string abce_amount_8 { get; set; }

        public string abce_amount_9 { get; set; }

        public string abce_amount_10 { get; set; }

        public string abce_amount_11 { get; set; }

        public string abce_amount_12 { get; set; }

        [Display(Name = "Jumlah (RM) Tahunan")]
        public string abce_total { get; set; }

        [Display(Name = "Jumlah Kuantiti Tahunan")]
        public string abce_total_quantity { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_proration { get; set; }

        public bool abce_history { get; set; }

        [StringLength(50)]
        public string abce_status { get; set; }

        [StringLength(100)]
        [Display(Name = "Kategori Aset")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_ast_code { get; set; }

        [StringLength(50)]
        public string abce_ast_category { get; set; }
    }

    public class ExpensesCapitalEditViewModel
    {
        public int? abce_id { get; set; }

        [Display(Name = "lblYear", ResourceType = typeof(GlobalResEstate))]
        public int abce_budgeting_year { get; set; }

        public int? abce_syarikat_id { get; set; }

        [StringLength(100)]
        public string abce_syarikat_name { get; set; }

        public int? abce_ladang_id { get; set; }

        [StringLength(5)]
        public string abce_ladang_code { get; set; }

        [StringLength(50)]
        public string abce_ladang_name { get; set; }

        public int? abce_wilayah_id { get; set; }

        [StringLength(50)]
        public string abce_wilayah_name { get; set; }

        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "GL Aset")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_gl_code { get; set; }

        [StringLength(100)]
        public string abce_gl_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Jenis Keterangan")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "Jenis Belian")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_jenis_code { get; set; }

        [StringLength(100)]
        public string abce_jenis_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Justifikasi")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_reason { get; set; }

        [StringLength(100)]
        public string abce_material_details { get; set; }

        [Display(Name = "hdrQuantity", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_unit_1 { get; set; }

        public decimal? abce_uom_1 { get; set; }

        [Display(Name = "Harga Seunit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_rate { get; set; }

        [Display(Name = "Bulan 1")]
        public string abce_quantity_1 { get; set; }

        [Display(Name = "Bulan 2")]
        public string abce_quantity_2 { get; set; }

        [Display(Name = "Bulan 3")]
        public string abce_quantity_3 { get; set; }

        [Display(Name = "Bulan 4")]
        public string abce_quantity_4 { get; set; }

        [Display(Name = "Bulan 5")]
        public string abce_quantity_5 { get; set; }

        [Display(Name = "Bulan 6")]
        public string abce_quantity_6 { get; set; }

        [Display(Name = "Bulan 7")]
        public string abce_quantity_7 { get; set; }

        [Display(Name = "Bulan 8")]
        public string abce_quantity_8 { get; set; }

        [Display(Name = "Bulan 9")]
        public string abce_quantity_9 { get; set; }

        [Display(Name = "Bulan 10")]
        public string abce_quantity_10 { get; set; }

        [Display(Name = "Bulan 11")]
        public string abce_quantity_11 { get; set; }

        [Display(Name = "Bulan 12")]
        public string abce_quantity_12 { get; set; }

        public string abce_amount_1 { get; set; }

        public string abce_amount_2 { get; set; }

        public string abce_amount_3 { get; set; }

        public string abce_amount_4 { get; set; }

        public string abce_amount_5 { get; set; }

        public string abce_amount_6 { get; set; }

        public string abce_amount_7 { get; set; }

        public string abce_amount_8 { get; set; }

        public string abce_amount_9 { get; set; }

        public string abce_amount_10 { get; set; }

        public string abce_amount_11 { get; set; }

        public string abce_amount_12 { get; set; }

        [Display(Name = "Jumlah (RM) Tahunan")]
        public string abce_total { get; set; }

        [Display(Name = "Jumlah Kuantiti Tahunan")]
        public string abce_total_quantity { get; set; }

        [StringLength(25)]
        [Display(Name = "Proration")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_proration { get; set; }

        public bool abce_history { get; set; }

        [StringLength(50)]
        public string abce_status { get; set; }

        [StringLength(100)]
        [Display(Name = "Kategori Aset")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_ast_code { get; set; }

        [StringLength(50)]
        public string abce_ast_category { get; set; }
    }

    public class ExpensesCapitalDeleteViewModel
    {
        [Display(Name = "Tahun Bajet")]
        public int BudgetYear { get; set; }

        [StringLength(15)]
        [Display(Name = "Kod Cost Center")]
        public string CostCenterCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Keterangan Cost Center")]
        public string CostCenterDesc { get; set; }

        [Display(Name = "Jumlah (RM)")]
        public string Total { get; set; }
    }

    public class ExpensesCapitalDeleteDetailViewModel
    {
        public int? abce_id { get; set; }

        [Display(Name = "Tahun Bajet")]
        public int abce_budgeting_year { get; set; }

        public int? abce_syarikat_id { get; set; }

        [StringLength(100)]
        public string abce_syarikat_name { get; set; }

        public int? abce_ladang_id { get; set; }

        [StringLength(5)]
        public string abce_ladang_code { get; set; }

        [StringLength(50)]
        public string abce_ladang_name { get; set; }

        public int? abce_wilayah_id { get; set; }

        [StringLength(50)]
        public string abce_wilayah_name { get; set; }

        [StringLength(15)]
        [Display(Name = "lblCostCenter", ResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_code { get; set; }

        [StringLength(100)]
        [Display(Name = "hdrUnit", ResourceType = typeof(GlobalResEstate))]
        public string abce_cost_center_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "lblGLCode", ResourceType = typeof(GlobalResEstate))]
        public string abce_gl_code { get; set; }

        [StringLength(100)]
        [Display(Name = "lblGLDesc", ResourceType = typeof(GlobalResEstate))]
        public string abce_gl_name { get; set; }

        [StringLength(255)]
        [Display(Name = "Jenis Keterangan")]
        public string abce_desc { get; set; }

        [StringLength(20)]
        [Display(Name = "Jenis Belian")]
        public string abce_jenis_code { get; set; }

        [StringLength(100)]
        public string abce_jenis_name { get; set; }

        [Display(Name = "hdrQuantity", ResourceType = typeof(GlobalResEstate))]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abce_unit_1 { get; set; }

        [Display(Name = "Harga Seunit")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public decimal? abce_rate { get; set; }

        [Display(Name = "Jumlah (RM) Tahunan")]
        public string abce_total { get; set; }

        [StringLength(100)]
        [Display(Name = "Kategori Aset")]
        [Required(ErrorMessageResourceName = "msgModelValidation", ErrorMessageResourceType = typeof(GlobalResEstate))]
        public string abce_ast_code { get; set; }

        [StringLength(50)]
        public string abce_ast_category { get; set; }

        [StringLength(150)]
        public string abce_ast_code_category { get; set; }
    }
}