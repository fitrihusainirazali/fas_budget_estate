namespace MVC_SYSTEM.ModelsBudget
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_UpahAktiviti
    {
        [Key]
        public int fld_ID { get; set; }

        public string fld_KodAktvt { get; set; }

        public string fld_Desc { get; set; }

        public string fld_Unit { get; set; }

        public decimal? fld_Harga { get; set; }

        public string fld_KodJenisAktvt { get; set; }

        public int fld_DisabledFlag { get; set; }

        public int fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool fld_Deleted { get; set; }

        public string fld_Kategori { get; set; }

        public decimal? fld_MaxProduktiviti { get; set; }

        public string fld_KategoriAktvt { get; set; }
    }
}
