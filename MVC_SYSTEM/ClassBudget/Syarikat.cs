using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class Syarikat
    {
        public int? GetSyarikatID()
        {
            GetIdentity getIdentity = new GetIdentity();
            GetNSWL getNSWL = new GetNSWL();

            int? userid = getIdentity.ID(HttpContext.Current.User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            getNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, HttpContext.Current.User.Identity.Name);

            return SyarikatID;
        }

        public tbl_Syarikat GetSyarikat(int? syarikatId = null)
        {
            syarikatId = syarikatId.HasValue ? syarikatId : GetSyarikatID();
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                var syarikat = db.tbl_Syarikat
                    .Where(s => s.fld_SyarikatID == syarikatId)
                    .FirstOrDefault();
                if (syarikat != null)
                    return syarikat;
                return new tbl_Syarikat();
            }
        }
    }
}