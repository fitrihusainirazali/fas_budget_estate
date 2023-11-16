namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_FeldaGrpPriceBTS
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }
        public decimal? Price1 { get; set; }
        public decimal? Price2 { get; set; }
        public decimal? Price3 { get; set; }
        public decimal? Price4 { get; set; }
        public decimal? Price5 { get; set; }
        public decimal? Price6 { get; set; }
        public decimal? Price7 { get; set; }
        public decimal? Price8 { get; set; }
        public decimal? Price9 { get; set; }
        public decimal? Price10 { get; set; }
        public decimal? Price11 { get; set; }
        public decimal? Price12 { get; set; }
        [StringLength(100)]
        public string Co_List { get; set; }
        public bool? fld_Deleted { get; set; }
        public int PriceYear { get; set; }
        public int? CC_Sort { get; set; }
    }
}
