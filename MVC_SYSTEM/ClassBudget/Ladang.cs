using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class Ladang
    {
        public int? GetLadangID()
        {
            GetIdentity getIdentity = new GetIdentity();
            GetNSWL getNSWL = new GetNSWL();

            int? userid = getIdentity.ID(HttpContext.Current.User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            getNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, HttpContext.Current.User.Identity.Name);

            return LadangID;
        }

        public tbl_Ladang GetLadang(int? ladangId = null)
        {
            ladangId = ladangId.HasValue ? ladangId : GetLadangID();
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                var ladang = db.tbl_Ladang
                    .Where(l => l.fld_ID == ladangId)
                    .FirstOrDefault();
                if (ladang != null)
                    return ladang;
                return new tbl_Ladang();
            }
        }
    }
}