using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;//Fitri add 7.9.2023

namespace MVC_SYSTEM.ModelsBudget
{
    public class MVC_SYSTEM_ModelsBudgetEst : DbContext
    {
        //fitri comment
        //public MVC_SYSTEM_ModelsBudgetEst() : base("name=MVC_SYSTEM_CONN")
        //{
        //}

        //fitri add
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";
        public MVC_SYSTEM_ModelsBudgetEst()
            : base(nameOrConnectionString: "BYOWN")
        {
            base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public static MVC_SYSTEM_ModelsBudgetEst ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;
            return new MVC_SYSTEM_ModelsBudgetEst();
        }

        public virtual DbSet<bgt_expenses_capital> bgt_expenses_capitals { get; set; }
        public virtual DbSet<bgt_income_sawit> bgt_income_sawit { get; set; } //fitri add
        public virtual DbSet<bgt_expenses_levi> bgt_expenses_levi { get; set; } //fitri add
        public virtual DbSet<bgt_income_bijibenih> bgt_income_bijibenih { get; set; } //fitri add
        public virtual DbSet<bgt_expenses_other> bgt_expenses_others { get; set; }
        public virtual DbSet<bgt_income_product> bgt_income_product { get; set; }
        public virtual DbSet<bgt_expenses_Depreciation> bgt_expenses_Depreciation { get; set; } //atun add
        public virtual DbSet<bgt_expenses_Salary> bgt_expenses_Salary { get; set; } //atun add
        public virtual DbSet<bgt_expenses_Insurans> bgt_expenses_Insurans { get; set; } //atun add
        public virtual DbSet<bgt_vehicle_register> bgt_vehicle_register { get; set; } //fitri add
        public virtual DbSet<bgt_expenses_IT> bgt_expenses_IT { get; set; } //atun add
        public virtual DbSet<bgt_expenses_CtrlHQ> bgt_expenses_CtrlHQ { get; set; } //atun add
        public virtual DbSet<bgt_expenses_pau> bgt_expenses_paus { get; set; }
        public virtual DbSet<bgt_expenses_KhidCor> bgt_expenses_KhidCor { get; set; } //
        public virtual DbSet<bgt_expenses_Medical> bgt_expenses_Medical { get; set; } //
        public virtual DbSet<bgt_expenses_Windfall> bgt_expenses_Windfall { get; set; } //
        public virtual DbSet<bgt_expenses_gaji> bgt_expenses_gajis { get; set; } //
        public virtual DbSet<bgt_approval> bgt_approval { get; set; }
        public virtual DbSet<bgt_approval_log> bgt_approval_log { get; set; }
        public virtual DbSet<bgt_expenses_vehicle> bgt_expenses_vehicle { get; set; } //
        public virtual DbSet<sp_AktivitiGL> sp_AktivitiGL { get; set; }//atun add
        public virtual DbSet<sp_CCdetailsRecs> sp_CCdetailsRecs { get; set; }//atun add


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_unit_1).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_1).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_2).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_3).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_4).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_5).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_6).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_7).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_8).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_9).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_10).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_11).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_quantity_12).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_capital>().Property(x => x.abce_total_quantity).HasPrecision(18, 3);

            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_1).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_2).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_3).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_4).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_5).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_6).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_7).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_8).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_9).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_10).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_11).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_quantity_12).HasPrecision(18, 3);
            modelBuilder.Entity<bgt_expenses_pau>().Property(x => x.abep_total_quantity).HasPrecision(18, 3);

            //Fitri add 7.9.2023
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 18));
        }
    }
}