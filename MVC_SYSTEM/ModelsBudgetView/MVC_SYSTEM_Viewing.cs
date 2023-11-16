using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.ModelsBudgetView
{
    public class MVC_SYSTEM_ModelsBudgetView : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";
        public MVC_SYSTEM_ModelsBudgetView()
            : base(nameOrConnectionString: "MVC_SYSTEM_CONN")
        {
            if (host1 != "" && catalog1 != "" && user1 != "" && pass1 != "")
            {
                base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
            }

            host1 = "";
            catalog1 = "";
            user1 = "";
            pass1 = "";
        }

       

        public static MVC_SYSTEM_ModelsBudgetView ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
             host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_ModelsBudgetView();

        }


        public virtual DbSet<bgt_expenses_DepreciationView> bgt_expenses_DepreciationView { get; set; }
        //public virtual List<ExpDepDeleteListViewModel> ExpDepDeleteListViewModel { get; set; }

    }
}