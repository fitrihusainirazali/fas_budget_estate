using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class IntegrationListViewModel
    {
        public int BudgetYear { get; set; }

        public string DownloadBy { get; set; }

        public string DownloadDate { get; set; }
    }

    public class IntegrationListDetailViewModel
    {
        public int revision { get; set; }

        public int budgeting_year { get; set; }

        public int syarikat_id { get; set; }

        public string cost_center_code { get; set; }

        public string gl_code { get; set; }

        public decimal amount_1 { get; set; }

        public decimal amount_2 { get; set; }

        public decimal amount_3 { get; set; }

        public decimal amount_4 { get; set; }

        public decimal amount_5 { get; set; }

        public decimal amount_6 { get; set; }

        public decimal amount_7 { get; set; }

        public decimal amount_8 { get; set; }

        public decimal amount_9 { get; set; }

        public decimal amount_10 { get; set; }

        public decimal amount_11 { get; set; }

        public decimal amount_12 { get; set; }

        public decimal total { get; set; }

        public string download_by { get; set; }

        public string download_date { get; set; }
    }

    public class IntegrationUploadFigureListModel
    {
        public int actu_Year { get; set; }
        public string actu_GLCode { get; set; }
        public string actu_CostCenter { get; set; }
        public decimal actu_ActualAmt { get; set; }
    }
}