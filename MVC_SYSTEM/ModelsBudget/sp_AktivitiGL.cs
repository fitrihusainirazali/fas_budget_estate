using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
   
    public class sp_AktivitiGL
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(20)]
        public string fldGL { get; set; }

        [StringLength(100)]
        public string fldGLDesc { get; set; }

        [StringLength(20)]
        public string fldKodAktvt { get; set; }

        [StringLength(255)]
        public string fldAktvtDesc { get; set; }
    }

    
}