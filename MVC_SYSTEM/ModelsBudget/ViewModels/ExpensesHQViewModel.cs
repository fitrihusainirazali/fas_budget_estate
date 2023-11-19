using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class ExpensesHQListViewModel
    {
        public int BudgetYear { get; set; }

        public string UploadedBy { get; set; }

        public string UploadedDate { get; set; }

        public int Version { get; set; }
    }

    public class ExpensesHQListDetailViewModel
    {
        public string ScreenName { get; set; }

        public int Id { get; set; }

        public int BudgetYear { get; set; }

        public string CostCenterCode { get; set; }

        public string CostCenterDesc { get; set; }

        public string GLCode { get; set; }

        public string GLDesc { get; set; }

        public decimal Month1 { get; set; }

        public decimal Month2 { get; set; }

        public decimal Month3 { get; set; }

        public decimal Month4 { get; set; }

        public decimal Month5 { get; set; }

        public decimal Month6 { get; set; }

        public decimal Month7 { get; set; }

        public decimal Month8 { get; set; }

        public decimal Month9 { get; set; }

        public decimal Month10 { get; set; }

        public decimal Month11 { get; set; }

        public decimal Month12 { get; set; }

        public decimal Total { get; set; }
    }
}