using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsBudget
{
    [Table("bgt_Screen")]
    public partial class bgt_Screen
    {
        [Key]
        public int ScrID { get; set; }

        public int? ScrSort { get; set; }

        [StringLength(2)]
        public string ScrSystem { get; set; }

        [StringLength(4)]
        public string ScrHdrCode { get; set; }

        [Required]
        [StringLength(4)]
        public string ScrCode { get; set; }

        public bool? GLTick { get; set; }

        [Required]
        [StringLength(25)]
        public string ScrName { get; set; }

        [StringLength(60)]
        public string ScrNameLongDesc { get; set; }

        [ForeignKey("ScrHdrCode")]
        public virtual bgt_ScrHdr bgt_ScrHdr { get; set; }

        public virtual ICollection<bgt_ScreenGL> bgt_ScreenGLs { get; set; }
    }
}