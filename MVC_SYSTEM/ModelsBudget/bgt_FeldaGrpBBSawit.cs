namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_FeldaGrpBBSawit
    {
        [Key]
        public int PriceID { get; set; }
        public int fld_ID { get; set; }
        public int PriceYear { get; set; }
        public decimal? PriceBBSawit { get; set; }
    }
}
