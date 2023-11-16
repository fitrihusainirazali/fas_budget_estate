using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesVehicleListViewModel
    {
        public string ScreenName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterDesc { get; set; }
        public int BudgetYear { get; set; }
        public string Revision { get; set; }
    }

    public class ExpensesVehicleDetailViewModel
    {
        public string ScreenName { get; set; }
        public int abvr_id { get; set; }
        public int? abvr_budgeting_year { get; set; }
        public string abvr_cost_center_code { get; set; }
        public string abvr_cost_center_desc { get; set; }
        public string abvr_material_code { get; set; }
        public string abvr_material_name { get; set; }
        public string abvr_jenis_code { get; set; }
        public string abvr_jenis_name { get; set; }
        public string abvr_model { get; set; }
        public string abvr_register_no { get; set; }
        public int? abvr_tahun_dibuat { get; set; }
        public bool? abvr_status { get; set; }
        public string abvr_revision_status { get; set; }
        
    }
}