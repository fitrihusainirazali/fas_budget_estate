using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_HQExpXcelDtl")]
    public partial class bgt_HQExpXcelDtl
    {
        [Key]
        public int hqex_ID { get; set; }

        [StringLength(5)]
        public string hqex_ScrCode { get; set; }

        [StringLength(10)]
        public string hqex_Tab { get; set; }

        public int hqex_Sort { get; set; }

        [StringLength(4)]
        public string hqex_Code { get; set; }

        [StringLength(10)]
        public string hqex_GLCode { get; set; }

        [StringLength(50)]
        public string hqex_ItemDesc { get; set; }

        public DateTime? hqex_modified { get; set; }

        public int? hqex_SyarikatID { get; set; }

        [StringLength(20)]
        public string hqex_ModifiedByID { get; set; }
    }
}