using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_RoleScrAction")]
    public partial class bgt_RoleScrAction
    {
        [Key]
        public int rol_ID { get; set; }

        [Required]
        [StringLength(4)]
        public string rol_roleID { get; set; }

        [Required]
        [StringLength(4)]
        public string rol_ScrCode { get; set; }
        public bool rol_Add { get; set; }
        public bool rol_Approve { get; set; }
        public bool rol_Check { get; set; }
        public bool rol_Delete { get; set; }
        public bool rol_Download { get; set; }
        public bool rol_Edit { get; set; }
        public bool rol_Print { get; set; }
        public bool rol_Reject { get; set; }
        public bool rol_Upload { get; set; }
        public bool rol_View { get; set; }
    }
}