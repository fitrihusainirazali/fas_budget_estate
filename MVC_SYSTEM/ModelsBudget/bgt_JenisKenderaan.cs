namespace MVC_SYSTEM.ModelsBudget
{
    using MVC_SYSTEM.App_LocalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_JenisKenderaan
    {
        [Key]
        public int ccmt_id { get; set; }
        [StringLength(50)]
        public string ccmt_code { get; set; }
        [StringLength(100)]
        public string ccmt_name { get; set; }
        [StringLength(50)]
        public string ccmt_sap_code { get; set; }
        public bool? ccmt_active { get; set; }
        public bool? ccmt_Lowest { get; set; }//new sept
        public DateTime last_modified { get; set; }//new sept
        public bool? fld_Deleted { get; set; }//new sept
        public int? fld_NegaraID { get; set; }//new sept
        public int? fld_SyarikatID { get; set; }//new sept
        public int? fld_WilayahID { get; set; }//new sept
        public int? fld_LadangID { get; set; }//new sept
    }
}
