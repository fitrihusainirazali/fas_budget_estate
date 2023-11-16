using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_ScreenGL")]
    public partial class bgt_ScreenGL
    {
        [Key]
        [Column(Order = 1)]
        public int fld_ScrID { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid fld_GLID { get; set; }

        [ForeignKey("fld_ScrID")]
        public virtual bgt_Screen bgt_Screen { get; set; }

        [ForeignKey("fld_GLID")]
        public virtual tbl_SAPGLPUP tbl_SAPGLPUP { get; set; }
    }
}