using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
   
    public partial class sp_CCdetailsRecs
    {
        [Key]
        public int fldID { get; set; }

        public string fldTahun { get; set; }

        
        [StringLength(15)]
        public string fldCostCenter { get; set; }

        [StringLength(200)]
        public string fldCostCenterDesc { get; set; }

        
        [StringLength(15)]
        public string fldGL { get; set; }

        [StringLength(200)]
        public string fldGLDesc { get; set; }

        public decimal? fldAmount1 { get; set; }
        public decimal? fldAmount2 { get; set; }
        public decimal? fldAmount3 { get; set; }
        public decimal? fldAmount4 { get; set; }
        public decimal? fldAmount5 { get; set; }
        public decimal? fldAmount6 { get; set; }
        public decimal? fldAmount7 { get; set; }
        public decimal? fldAmount8 { get; set; }
        public decimal? fldAmount9 { get; set; }
        public decimal? fldAmount10 { get; set; }
        public decimal? fldAmount11 { get; set; }
        public decimal? fldAmount12 { get; set; }

        public decimal? fldTotal { get; set; }

        
        [StringLength(200)]
        public string fldScreen { get; set; }
    }
}