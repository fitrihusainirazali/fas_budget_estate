using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_User_Role")]
    public partial class bgt_User_Role
    {
        [Key]
        public int bgtrole_id { get; set; }

        public int? bgtrole_user_id { get; set; }

        [StringLength(50)]
        public string bgtrole_name { get; set; }
    }
}