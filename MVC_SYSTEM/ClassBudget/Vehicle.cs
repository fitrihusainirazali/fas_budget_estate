using MVC_SYSTEM.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsBudget;

namespace MVC_SYSTEM.ClassBudget
{
    public class Vehicle
    {
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();

        public List<bgt_vehicle_register> GetCostCenters()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(HttpContext.Current.User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, HttpContext.Current.User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            using (var db = new MVC_SYSTEM_ModelsBudgetEst())
            {
                return db.bgt_vehicle_register
                    .Where(w => w.abvr_deleted == false)
                    .GroupBy(g => g.abvr_cost_center_code)
                    .Select(s => s.FirstOrDefault())
                    .OrderBy(o => o.abvr_cost_center_code)
                    .ToList();
            }
        }
    }
}