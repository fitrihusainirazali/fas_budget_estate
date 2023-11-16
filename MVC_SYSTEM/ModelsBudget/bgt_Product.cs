namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_Product
    {
        [Key]
        public int PrdID { get; set; }

        [Required]
        [StringLength(4)]
        public string CoCode { get; set; }

        public int YearBgt { get; set; }

        [Required]
        [StringLength(20)]
        public string PrdCode { get; set; }

        [Required]
        [StringLength(50)]
        public string PrdName { get; set; }
        
        public int? LocID { get; set; }

        public bool Lowest { get; set; }

        public int? UOMID { get; set; }

        public decimal? PriceSale { get; set; }

        public decimal? PricePAU { get; set; }

        public bool Active { get; set; }

        //public decimal? OldPriceSale { get; set; }

        public DateTime? DateModify { get; set; }

        [StringLength(20)]
        public string Unit { get; set; }

        [StringLength(20)]
        public string CstID { get; set; }

        [StringLength(250)]
        public string CstName { get; set; }

        [StringLength(10)]
        public string UsrID { get; set; }

        //public bool? fld_Deleted { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }
    }
}
