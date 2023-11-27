namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bgt_log_audit_trail
    {
        [Key]
        public int alat_id { get; set; }

        [StringLength(255)]
        public string alat_preparer_id { get; set; }

        [StringLength(255)]
        public string alat_preparer_name { get; set; }

        [StringLength(10)]
        public string alat_screen_code { get; set; }


        [StringLength(100)]
        public string alat_screen_name { get; set; }

        public int alat_budgeting_year { get; set; }

        public DateTime? alat_last_access_time { get; set; }

        [StringLength(255)]
        public string alat_session_id { get; set; }

        public DateTime? alat_login_date { get; set; }

        public DateTime? alat_logout_date { get; set; }

        public int alat_syarikatId { get; set; }

        public DateTime? last_modified { get; set; }

    }
}
