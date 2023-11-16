using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace MVC_SYSTEM.ModelsBudget.ViewModels
{
   
    public  class ExpensesGajiViewRptModel
    {

        public int abeg_id { get; set; }

        public int abeg_revision { get; set; }

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

}

