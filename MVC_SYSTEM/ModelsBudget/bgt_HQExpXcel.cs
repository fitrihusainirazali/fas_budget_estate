using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_HQExpXcel")]
    public partial class bgt_HQExpXcel
    {
        [Key]
        public int hqxc_ID { get; set; }

        public bool? hqxc_GLTick { get; set; }

        [StringLength(5)]
        public string hqxc_ScrCode { get; set; }

        [StringLength(30)]
        public string hqxc_ScrName { get; set; }

        [StringLength(2)]
        public string hqxc_TabCode { get; set; }

        [StringLength(30)]
        public string hqxc_TabName { get; set; }

        public DateTime? hqxc_modified { get; set; }

        public int? hqxc_SyarikatID { get; set; }

        [StringLength(20)]
        public string hqxc_ModifiedByID { get; set; }
    }
}