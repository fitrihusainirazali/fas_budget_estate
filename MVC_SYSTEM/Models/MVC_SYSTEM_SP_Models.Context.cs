﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using System.Data.SqlClient;
    using System.Data.Entity.Core.EntityClient;
    using System.Collections.Generic;

    public partial class MVC_SYSTEM_SP_Models : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";

        public MVC_SYSTEM_SP_Models(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 0; //added by shah 26.01.2021
        }

        public static MVC_SYSTEM_SP_Models ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = host;
            sqlBuilder.InitialCatalog = catalog;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.UserID = user;
            sqlBuilder.Password = pass;
            sqlBuilder.ConnectTimeout = 0; //modified by shah 26.01.2021 - changed 100 to 0
            sqlBuilder.PersistSecurityInfo = true;
            sqlBuilder.IntegratedSecurity = false;

            var entityConnectionStringBuilder = new EntityConnectionStringBuilder();
            entityConnectionStringBuilder.Provider = "System.Data.SqlClient";
            entityConnectionStringBuilder.ProviderConnectionString = sqlBuilder.ConnectionString;
            entityConnectionStringBuilder.Metadata = "res://*/Models.MVC_SYSTEM_SP_Models.csdl|res://*/Models.MVC_SYSTEM_SP_Models.ssdl|res://*/Models.MVC_SYSTEM_SP_Models.msl";

            return new MVC_SYSTEM_SP_Models(entityConnectionStringBuilder.ConnectionString);

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }


        public virtual IEnumerable<sp_KerjaInfoDetails_Result> sp_KerjaInfoDetails(Nullable<int> kategoriPilih, string pilihanCari, Nullable<int> year, Nullable<int> month, Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID)
        {
            var kategoriPilihParameter = kategoriPilih.HasValue ?
                new ObjectParameter("KategoriPilih", kategoriPilih) :
                new ObjectParameter("KategoriPilih", typeof(int));
    
            var pilihanCariParameter = pilihanCari != null ?
                new ObjectParameter("PilihanCari", pilihanCari) :
                new ObjectParameter("PilihanCari", typeof(string));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("year", year) :
                new ObjectParameter("year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_KerjaInfoDetails_Result>("sp_KerjaInfoDetails", kategoriPilihParameter, pilihanCariParameter, yearParameter, monthParameter, negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter);
        }
    
        public virtual IEnumerable<sp_Payslip_Result> sp_Payslip(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> month, Nullable<int> year, string nopkj)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var nopkjParameter = nopkj != null ?
                new ObjectParameter("Nopkj", nopkj) :
                new ObjectParameter("Nopkj", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_Payslip_Result>("sp_Payslip", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, monthParameter, yearParameter, nopkjParameter);
        }
    
        public virtual int sp_RptBulPenPekLad(string dataSource, string databaseName, Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string levelAccess, string kdrytan, Nullable<int> month, Nullable<int> year, Nullable<int> userID, Nullable<short> selectionCategory)
        {
            var dataSourceParameter = dataSource != null ?
                new ObjectParameter("DataSource", dataSource) :
                new ObjectParameter("DataSource", typeof(string));
    
            var databaseNameParameter = databaseName != null ?
                new ObjectParameter("DatabaseName", databaseName) :
                new ObjectParameter("DatabaseName", typeof(string));
    
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var levelAccessParameter = levelAccess != null ?
                new ObjectParameter("LevelAccess", levelAccess) :
                new ObjectParameter("LevelAccess", typeof(string));
    
            var kdrytanParameter = kdrytan != null ?
                new ObjectParameter("Kdrytan", kdrytan) :
                new ObjectParameter("Kdrytan", typeof(string));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var selectionCategoryParameter = selectionCategory.HasValue ?
                new ObjectParameter("SelectionCategory", selectionCategory) :
                new ObjectParameter("SelectionCategory", typeof(short));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_RptBulPenPekLad", dataSourceParameter, databaseNameParameter, negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, levelAccessParameter, kdrytanParameter, monthParameter, yearParameter, userIDParameter, selectionCategoryParameter);
        }
    
        public virtual int sp_RptMakPekTem(string dataSource, string databaseName, Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> aktifStatus, string levelAccess, Nullable<int> userID, Nullable<short> selectionCategory)
        {
            var dataSourceParameter = dataSource != null ?
                new ObjectParameter("DataSource", dataSource) :
                new ObjectParameter("DataSource", typeof(string));
    
            var databaseNameParameter = databaseName != null ?
                new ObjectParameter("DatabaseName", databaseName) :
                new ObjectParameter("DatabaseName", typeof(string));
    
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var aktifStatusParameter = aktifStatus.HasValue ?
                new ObjectParameter("AktifStatus", aktifStatus) :
                new ObjectParameter("AktifStatus", typeof(int));
    
            var levelAccessParameter = levelAccess != null ?
                new ObjectParameter("LevelAccess", levelAccess) :
                new ObjectParameter("LevelAccess", typeof(string));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var selectionCategoryParameter = selectionCategory.HasValue ?
                new ObjectParameter("SelectionCategory", selectionCategory) :
                new ObjectParameter("SelectionCategory", typeof(short));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_RptMakPekTem", dataSourceParameter, databaseNameParameter, negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, aktifStatusParameter, levelAccessParameter, userIDParameter, selectionCategoryParameter);
        }
    
        public virtual IEnumerable<sp_RptProduktiviti_Result> sp_RptProduktiviti(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> year, Nullable<int> month, string noPkj, string unit, string peringkat, string status)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var noPkjParameter = noPkj != null ?
                new ObjectParameter("NoPkj", noPkj) :
                new ObjectParameter("NoPkj", typeof(string));
    
            var unitParameter = unit != null ?
                new ObjectParameter("Unit", unit) :
                new ObjectParameter("Unit", typeof(string));
    
            var peringkatParameter = peringkat != null ?
                new ObjectParameter("Peringkat", peringkat) :
                new ObjectParameter("Peringkat", typeof(string));
    
            var statusParameter = status != null ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptProduktiviti_Result>("sp_RptProduktiviti", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, yearParameter, monthParameter, noPkjParameter, unitParameter, peringkatParameter, statusParameter);
        }
    
        public virtual IEnumerable<sp_RptPurataGajiBulanan_Result> sp_RptPurataGajiBulanan(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> year)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptPurataGajiBulanan_Result>("sp_RptPurataGajiBulanan", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, yearParameter);
        }
    
        public virtual int sp_RptRumKedKepPekLad(string dataSource, string databaseName, Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> aktifStatus, string levelAccess, Nullable<int> userID, Nullable<short> selectionCategory)
        {
            var dataSourceParameter = dataSource != null ?
                new ObjectParameter("DataSource", dataSource) :
                new ObjectParameter("DataSource", typeof(string));
    
            var databaseNameParameter = databaseName != null ?
                new ObjectParameter("DatabaseName", databaseName) :
                new ObjectParameter("DatabaseName", typeof(string));
    
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var aktifStatusParameter = aktifStatus.HasValue ?
                new ObjectParameter("AktifStatus", aktifStatus) :
                new ObjectParameter("AktifStatus", typeof(int));
    
            var levelAccessParameter = levelAccess != null ?
                new ObjectParameter("LevelAccess", levelAccess) :
                new ObjectParameter("LevelAccess", typeof(string));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var selectionCategoryParameter = selectionCategory.HasValue ?
                new ObjectParameter("SelectionCategory", selectionCategory) :
                new ObjectParameter("SelectionCategory", typeof(short));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_RptRumKedKepPekLad", dataSourceParameter, databaseNameParameter, negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, aktifStatusParameter, levelAccessParameter, userIDParameter, selectionCategoryParameter);
        }
    
        public virtual IEnumerable<sp_RptSkb_Result> sp_RptSkb(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> month, Nullable<int> year)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_RptSkb_Result>("sp_RptSkb", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, monthParameter, yearParameter);
        }
    
        public virtual int sp_RptTransPek(string dataSource, string databaseName, Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, string levelAccess, Nullable<int> month, Nullable<int> year, Nullable<int> userID, Nullable<short> selectionCategory)
        {
            var dataSourceParameter = dataSource != null ?
                new ObjectParameter("DataSource", dataSource) :
                new ObjectParameter("DataSource", typeof(string));
    
            var databaseNameParameter = databaseName != null ?
                new ObjectParameter("DatabaseName", databaseName) :
                new ObjectParameter("DatabaseName", typeof(string));
    
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var levelAccessParameter = levelAccess != null ?
                new ObjectParameter("LevelAccess", levelAccess) :
                new ObjectParameter("LevelAccess", typeof(string));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var userIDParameter = userID.HasValue ?
                new ObjectParameter("UserID", userID) :
                new ObjectParameter("UserID", typeof(int));
    
            var selectionCategoryParameter = selectionCategory.HasValue ?
                new ObjectParameter("SelectionCategory", selectionCategory) :
                new ObjectParameter("SelectionCategory", typeof(short));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_RptTransPek", dataSourceParameter, databaseNameParameter, negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, levelAccessParameter, monthParameter, yearParameter, userIDParameter, selectionCategoryParameter);
        }
    
        public virtual IEnumerable<sp_YieldBracketTable_Result> sp_YieldBracketTable(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> jnsPkt)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var jnsPktParameter = jnsPkt.HasValue ?
                new ObjectParameter("JnsPkt", jnsPkt) :
                new ObjectParameter("JnsPkt", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_YieldBracketTable_Result>("sp_YieldBracketTable", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, jnsPktParameter);
        }
    
        public virtual IEnumerable<sp_YieldBracketTableSum_Result> sp_YieldBracketTableSum(Nullable<int> negaraID, Nullable<int> syarikatID, Nullable<int> wilayahID, Nullable<int> ladangID, Nullable<int> jnsPkt)
        {
            var negaraIDParameter = negaraID.HasValue ?
                new ObjectParameter("NegaraID", negaraID) :
                new ObjectParameter("NegaraID", typeof(int));
    
            var syarikatIDParameter = syarikatID.HasValue ?
                new ObjectParameter("SyarikatID", syarikatID) :
                new ObjectParameter("SyarikatID", typeof(int));
    
            var wilayahIDParameter = wilayahID.HasValue ?
                new ObjectParameter("WilayahID", wilayahID) :
                new ObjectParameter("WilayahID", typeof(int));
    
            var ladangIDParameter = ladangID.HasValue ?
                new ObjectParameter("LadangID", ladangID) :
                new ObjectParameter("LadangID", typeof(int));
    
            var jnsPktParameter = jnsPkt.HasValue ?
                new ObjectParameter("JnsPkt", jnsPkt) :
                new ObjectParameter("JnsPkt", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_YieldBracketTableSum_Result>("sp_YieldBracketTableSum", negaraIDParameter, syarikatIDParameter, wilayahIDParameter, ladangIDParameter, jnsPktParameter);
        }
    }
}
