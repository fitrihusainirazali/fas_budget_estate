using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_ScrHdr")]
    public partial class bgt_ScrHdr
    {
        [Key]
        [StringLength(4)]
        public string ScrHdrCode { get; set; }

        [Required]
        [StringLength(30)]
        public string ScrHdrName { get; set; }

        [Required]
        [StringLength(60)]
        public string ScrHdrDesc { get; set; }

        public virtual ICollection<bgt_Screen> bgt_Screens { get; set; }
    }
}