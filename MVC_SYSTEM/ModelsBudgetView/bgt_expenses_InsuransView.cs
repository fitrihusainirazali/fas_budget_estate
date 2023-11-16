﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace MVC_SYSTEM.ModelsBudgetView
{
   
    public  class bgt_expenses_InsuransView
    {
            
      
        public int abei_id { get; set; }

         public int? abei_budgeting_year { get; set; }

        public string abei_cost_center { get; set; }

        public string abei_cost_center_name { get; set; }

        public string abei_gl_code { get; set; }

        public string abei_gl_desc { get; set; }

        public string abei_uom { get; set; }

        public string abei_proration { get; set; }

        public decimal? abei_jumlah_setahun { get; set; }

        public decimal? abei_month_1 { get; set; }

        public decimal? abei_month_2 { get; set; }

        public decimal? abei_month_3 { get; set; }

        public decimal? abei_month_4 { get; set; }

        public decimal? abei_month_5 { get; set; }

        public decimal? abei_month_6 { get; set; }

        public decimal? abei_month_7 { get; set; }

        public decimal? abei_month_8 { get; set; }

        public decimal? abei_month_9 { get; set; }

        public decimal? abei_month_10 { get; set; }

        public decimal? abei_month_11 { get; set; }

        public decimal? abei_month_12 { get; set; }

        public decimal? abei_jumlah_RM { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00}")]
        public decimal? abei_jumlah_keseluruhan { get; set; }

        public DateTime? last_modified { get; set; }

        public int? abei_NegaraID { get; set; }

        public int? abei_SyarikatID { get; set; }

        public int? abei_WilayahID { get; set; }

        public int? abei_LadangID { get; set; }

        public bool? abei_Deleted { get; set; }

        //********

        public int fld_ScrID { get; set; }

        public Guid? fld_GLID { get; set; }

        //*********
        public string fld_GLCode { get; set; }

        public string fld_GLDesc { get; set; }

        //*********
        public string fld_CostCenter { get; set; }

        public string fld_CostCenterDesc { get; set; }
    }

}

