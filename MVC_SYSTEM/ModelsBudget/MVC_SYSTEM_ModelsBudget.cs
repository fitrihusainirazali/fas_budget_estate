using MVC_SYSTEM.MasterModels;
using System.Data.Entity;

namespace MVC_SYSTEM.ModelsBudget
{
    public class MVC_SYSTEM_ModelsBudget : DbContext
    {
        public MVC_SYSTEM_ModelsBudget()
            : base("name=MVC_SYSTEM_HQ_CONN")
        {
        }

        public virtual DbSet<bgt_Screen> bgt_Screens { get; set; }
        public virtual DbSet<bgt_ScreenGL> bgt_ScreenGLs { get; set; }
        public virtual DbSet<bgt_ScrHdr> bgt_ScrHdrs { get; set; }
        public virtual DbSet<bgt_UOM> bgt_UOM { get; set; }
        public virtual DbSet<bgt_Stesen> bgt_Stesen { get; set; }
        public virtual DbSet<bgt_Product> bgt_Product { get; set; }
        public virtual DbSet<bgt_PriceBTS> bgt_PriceBTS { get; set; }
        public virtual DbSet<bgt_Notification> bgt_Notification { get; set; }
        public virtual DbSet<bgt_Location> bgt_Location { get; set; }
        public virtual DbSet<bgt_FeldaGrp> bgt_FeldaGrp { get; set; }
        public virtual DbSet<vw_FeldaGrpPriceBTS> vw_FeldaGrpPriceBTS { get; set; }
        public virtual DbSet<bgt_LeviBTS> bgt_LeviBTS { get; set; }
        public virtual DbSet<bgt_Operation> bgt_Operations { get; set; }
        public virtual DbSet<bgt_FeldaGrpBBSawit> bgt_FeldaGrpBBSawit { get; set; }
        public virtual DbSet<vw_FeldaGrpPriceBBSawit> vw_FeldaGrpPriceBBSawit { get; set; }
        public virtual DbSet<bgt_JenisKdrn> bgt_JenisKdrn { get; set; }
        public virtual DbSet<bgt_PAU> bgt_PAU { get; set; }
        public virtual DbSet<bgt_Station_NC> bgt_Station_NC { get; set; }
        public virtual DbSet<bgt_Approval_Status> bgt_Approval_Status { get; set; }
        public virtual DbSet<bgt_Users> bgt_Users { get; set; }//new oct
        public virtual DbSet<bgt_AssetCategory> bgt_AssetCategories { get; set; }
        public virtual DbSet<bgt_CostCenterExt> bgt_CostCenterExts { get; set; }
        public virtual DbSet<tbl_SAPCCPUP> tbl_SAPCCPUPs { get; set; }
        public virtual DbSet<bgt_JenisKenderaan> bgt_JenisKenderaan { get; set; }
        public virtual DbSet<tbl_UpahAktiviti> tbl_UpahAktiviti { get; set; }
        public virtual DbSet<bgt_RoleScrAction> bgt_RoleScrAction { get; set; }//user matrix
        public virtual DbSet<bgt_Struc> bgt_Struc { get; set; }//new nov
        public virtual DbSet<bgt_PrdAct_NC> bgt_PrdAct_NCs { get; set; }

        public virtual DbSet<bgt_HQExpXcel> bgt_HQExpXcels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<bgt_Screen>()
                .HasMany(s => s.bgt_ScreenGLs)
                .WithRequired(sg => sg.bgt_Screen);

            modelBuilder.Entity<tbl_SAPGLPUP>()
                .HasMany(g => g.bgt_ScreenGLs)
                .WithRequired(sg => sg.tbl_SAPGLPUP);

            modelBuilder.Entity<bgt_ScreenGL>()
                .HasKey(sg => new { sg.fld_ScrID, sg.fld_GLID });
        }
    }
}