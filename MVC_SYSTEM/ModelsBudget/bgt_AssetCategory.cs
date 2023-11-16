using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_AssetCategory")]
    public partial class bgt_AssetCategory
    {
        [Key]
        public int ast_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string ast_Category { get; set; }

        [Required]
        [StringLength(20)]
        public string ast_GL { get; set; }
    }
}