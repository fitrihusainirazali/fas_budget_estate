using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class Wilayah
    {
        public int? GetWilayahID()
        {
            GetIdentity getIdentity = new GetIdentity();
            GetNSWL getNSWL = new GetNSWL();

            int? userid = getIdentity.ID(HttpContext.Current.User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            getNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, HttpContext.Current.User.Identity.Name);

            return WilayahID;
        }

        public tbl_Wilayah GetWilayah(int? wilayahId = null)
        {
            wilayahId = wilayahId.HasValue ? wilayahId : GetWilayahID();
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                var wilayah = db.tbl_Wilayah
                    .Where(w => w.fld_ID == wilayahId)
                    .FirstOrDefault();
                if (wilayah != null)
                    return wilayah;
                return new tbl_Wilayah();
            }
        }
    }
}