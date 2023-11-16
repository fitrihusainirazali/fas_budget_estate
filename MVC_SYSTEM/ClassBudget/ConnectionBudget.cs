using MVC_SYSTEM.Class;
using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class ConnectionBudget
    {
        public MVC_SYSTEM_ModelsBudgetEst GetConnection()
        {
            GetIdentity getIdentity = new GetIdentity();
            GetNSWL getNSWL = new GetNSWL();
            Connection connection = new Connection();

            int? userid = getIdentity.ID(HttpContext.Current.User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            getNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, HttpContext.Current.User.Identity.Name);
            connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            return MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
        }
    }
}