using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Users")]
    public partial class bgt_Users
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(10)]
        public string usr_ID { get; set; }

        [StringLength(50)]
        public string usr_Name { get; set; }

        [StringLength(2)]
        public string usr_Structure_ID { get; set; }

        [StringLength(20)]
        public string usr_Structure_Level { get; set; }

        [StringLength(50)]
        public string usr_Role { get; set; }

        [StringLength(2)]
        public string usr_Organization { get; set; }

        public int fld_SyarikatID { get; set; }

        public DateTime Last_Modified { get; set; }
    }
}