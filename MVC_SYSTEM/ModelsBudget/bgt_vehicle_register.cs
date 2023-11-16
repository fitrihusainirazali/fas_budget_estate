namespace MVC_SYSTEM.ModelsBudget
{
    using MVC_SYSTEM.App_LocalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_vehicle_register
    {
        [Key]
        public int abvr_id { get; set; }
        public int abvr_revision { get; set; }
        public bool abvr_revision_history { get; set; }
        [StringLength(50)]
        public string abvr_revision_status { get; set; }
        public int? abvr_budgeting_year { get; set; }
        public int? abvr_syarikat_id { get; set; }
        [StringLength(100)]
        public string abvr_syarikat_name { get; set; }
        public int? abvr_ladang_id { get; set; }
        [StringLength(5)]
        public string abvr_ladang_code { get; set; }
        [StringLength(50)]
        public string abvr_ladang_name { get; set; }
        public int? abvr_wilayah_id { get; set; }
        [StringLength(50)]
        public string abvr_wilayah_name { get; set; }
        [StringLength(11)]
        public string abvr_material_code { get; set; }
        [StringLength(100)]
        public string abvr_material_name { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(15)]
        public string abvr_cost_center_code { get; set; }
        [StringLength(100)]
        public string abvr_cost_center_desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string abvr_jenis_code { get; set; }
        [StringLength(100)]
        public string abvr_jenis_name { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string abvr_model { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string abvr_register_no { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        public int? abvr_tahun_dibuat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        public bool abvr_status { get; set; }
        public int? abvr_created_by { get; set; }
        public DateTime? last_modified { get; set; }
        public bool abvr_deleted { get; set; }
    }
}
