namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_FeldaGrp
    {
        [Key]
        public int fld_ID { get; set; }
        
        [StringLength(50)]
        public string Co_Category { get; set; }
        
        public int? CC_Sort { get; set; }

        [StringLength(100)]
        public string Co_List { get; set; }

        public int? CL_Sort { get; set; }
        
        public bool? Co_Active { get; set; }

        [StringLength(20)]
        public string GLSawit { get; set; }

        [StringLength(20)]
        public string GLBBSawit { get; set; }
        public decimal? PriceBBSawit { get; set; }
        public int? PriceYear { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
    }
}
