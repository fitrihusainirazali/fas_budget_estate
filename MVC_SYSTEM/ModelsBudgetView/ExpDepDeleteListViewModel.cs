using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsBudget;

namespace MVC_SYSTEM.ModelsBudgetView
{
    public class ExpDepDeleteListViewModel
    {

        public List<bgt_expenses_Depreciation> bgt_expenses_Depreciation { get; set; }
        public int? abed_budgeting_year { get; set; }
        public string abed_cost_center_name { get; set; }
        public string abed_cost_center { get; set; }
        public string abed_gl_code { get; set; }
        public string abed_gl_desc { get; set; }
        public decimal? abed_jumlah_keseluruhan { get; set; }
        public decimal? abed_jumlah_setahun { get; set; }
    }
}