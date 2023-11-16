namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_Stesen
    {
        [Key]
        public int fld_StesenID { get; set; }
        [StringLength(50)]
        public string fld_StesenCode { get; set; }
        [StringLength(100)]
        public string fld_StesenName { get; set; }
        public bool? fldDeleted { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
    }
}
