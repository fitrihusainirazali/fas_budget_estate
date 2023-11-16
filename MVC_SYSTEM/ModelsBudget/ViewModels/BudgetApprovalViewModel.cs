using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
    public class BudgetApprovalViewModel
    {
        public string ScreenName { get; set; }
        public string icon { get; set; }
        public int aprv_id { get; set; }
        public int? aprv_budgeting_year { get; set; }
        public string aprv_cost_center_code { get; set; }
        public string aprv_cost_center_desc { get; set; }
        public string aprv_status { get; set; }
        public string aprv_prepared_by { get; set; }
        public bool? aprv_deleted { get; set; }
        public DateTime? aprv_date { get; set; }
        public DateTime? last_modified { get; set; }
        public string aprv_note { get; set; }
        public bool aprv_bgt_block { get; set; }
        public string aprv_Approve_by { get; set; }// new oct
        public DateTime? aprv_Send_date { get; set; }// new oct
        public string aprv_station { get; set; }//new oct
        public decimal amt_sawit { get; set; }//new oct
        public decimal amt_bijibenih { get; set; }//new oct
        public decimal amt_produk { get; set; }//new oct
        public decimal sum_income { get; set; }//new oct
        public decimal sum_expenses { get; set; }//new oct
        public int? aprv_organization { get; set; }//new oct
        public string aprv_scrCode { get; set; }//add nov
        public int? aprv_approve_hq { get; set; }//add nov
        public DateTime? aprv_date_hq { get; set; }//add nov
        public int? aprv_syarikat_id { get; set; }//add nov
        public int? aprv_revision { get; set; }//add nov
        //public string aprv_statusHQ { get; set; }//add nov
    }
}