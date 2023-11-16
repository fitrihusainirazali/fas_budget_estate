using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class Negara
    {
        public int? GetNegaraID()
        {
            GetIdentity getIdentity = new GetIdentity();
            GetNSWL getNSWL = new GetNSWL();

            int? userid = getIdentity.ID(HttpContext.Current.User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            getNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, HttpContext.Current.User.Identity.Name);

            return NegaraID;
        }

        public tbl_Negara GetNegara(int? negaraId = null)
        {
            negaraId = negaraId.HasValue ? negaraId : GetNegaraID();
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                var negara = db.tbl_Negara
                    .Where(n => n.fld_NegaraID == negaraId)
                    .FirstOrDefault();
                if (negara != null)
                    return negara;
                return new tbl_Negara();
            }
        }
    }
}