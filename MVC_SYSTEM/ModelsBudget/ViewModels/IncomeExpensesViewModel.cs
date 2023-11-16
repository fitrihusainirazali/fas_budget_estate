using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    //public class IncomeListViewModel
    //{
    //    public int? Year { get; set; }
    //    public string CostCenter { get; set; }
    //    public List<string> AmountSawit { get; set; }
    //    public List<string> AmountBijiBenih { get; set; }
    //    public List<string> AmountProduk { get; set; }
    //}
    public class IncomeViewModel
    {
        public int? Year { get; set; }
        public string CostCenter { get; set; }
        public decimal AmountSawit { get; set; }
        public decimal AmountBijiBenih { get; set; }
        public decimal AmountProduk { get; set; }
        public string ScreenCode { get; set; }
    }
    public class ExpensesViewModel
    {
        public int? Year { get; set; }
        public string CostCenter { get; set; }
        public decimal AmountExpCapital { get; set; }
        public decimal AmountExpCtrlHQRecords { get; set; }
        public decimal AmountExpDepreciation { get; set; }
        public decimal AmountExpGaji { get; set; }
        public decimal AmountExpInsuranceRecords { get; set; }
        public decimal AmountExpIT { get; set; }
        public decimal AmountExpKhidCorp { get; set; }
        public decimal AmountExpLevi { get; set; }
        public decimal AmountExpMedical { get; set; }
        public decimal AmountExpOthers { get; set; }
        public decimal AmountExpPau { get; set; }
        public decimal AmountGetExpSalary { get; set; }
        public decimal AmountGetExpVehicle { get; set; }
        public decimal AmountGetExpWindfall { get; set; }
        public string ScreenCode { get; set; }

    }
}