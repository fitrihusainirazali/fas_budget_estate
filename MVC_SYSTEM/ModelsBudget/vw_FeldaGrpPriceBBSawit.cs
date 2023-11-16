namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_FeldaGrpPriceBBSawit
    {
        [Key]
        public int fld_ID { get; set; }
        public int CC_Sort { get; set; }
        public int CL_Sort { get; set; }
        [StringLength(100)]
        public string Co_List { get; set; }
        public bool? Co_Active { get; set; }
        public int PriceYear { get; set; }
        public decimal? PriceBBSawit { get; set; }
    }
}
