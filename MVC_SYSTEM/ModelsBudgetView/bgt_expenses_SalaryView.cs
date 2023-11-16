using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace MVC_SYSTEM.ModelsBudgetView
{
   
    public  class bgt_expenses_SalaryView
    {


        public int abes_id { get; set; }

        public int? abes_budgeting_year { get; set; }

        public string abes_cost_center { get; set; }

        public string abes_cost_center_name { get; set; }

        public string abes_gl_code { get; set; }

        public string abes_gl_desc { get; set; }

        public string abes_uom { get; set; }

        public string abes_proration { get; set; }

        public decimal? abes_jumlah_setahun { get; set; }

        public decimal? abes_qty_1 { get; set; }


        public decimal? abes_qty_2 { get; set; }

        public decimal? abes_qty_3 { get; set; }

        public decimal? abes_qty_4 { get; set; }

        public decimal? abes_qty_5 { get; set; }

        public decimal? abes_qty_6 { get; set; }

        public decimal? abes_qty_7 { get; set; }

        public decimal? abes_qty_8 { get; set; }

        public decimal? abes_qty_9 { get; set; }

        public decimal? abes_qty_10 { get; set; }

        public decimal? abes_qty_11 { get; set; }

        public decimal? abes_qty_12 { get; set; }

        public decimal? abes_jumlah_keseluruhan_qty { get; set; }

        public decimal? abes_jumlah_RM { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.00}")]
        public decimal? abes_jumlah_keseluruhan { get; set; }

        
        public decimal? abes_month_1 { get; set; }

        public decimal? abes_month_2 { get; set; }

        public decimal? abes_month_3 { get; set; }


        public decimal? abes_month_4 { get; set; }


        public decimal? abes_month_5 { get; set; }


        public decimal? abes_month_6 { get; set; }


        public decimal? abes_month_7 { get; set; }

        public decimal? abes_month_8 { get; set; }

        public decimal? abes_month_9 { get; set; }

        public decimal? abes_month_10 { get; set; }


        public decimal? abes_month_11 { get; set; }


        public decimal? abes_month_12 { get; set; }


        public DateTime? last_modified { get; set; }


        public int? abes_NegaraID { get; set; }



        public int? abes_SyarikatID { get; set; }


        public int? abes_WilayahID { get; set; }


        public int? abes_LadangID { get; set; }

        public bool? abes_Deleted { get; set; }
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

