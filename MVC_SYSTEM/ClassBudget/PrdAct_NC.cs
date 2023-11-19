using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class PrdAct_NC
    {
        public List<bgt_PrdAct_NC> GetPrdActs()
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                return db.bgt_PrdAct_NCs.OrderBy(p => p.Code_PP).ToList();
            }
        }

        public bgt_PrdAct_NC GetPrdActByCode(string code)
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                var prdAct = db.bgt_PrdAct_NCs.FirstOrDefault(p => p.Code_PP.ToLower().Equals(code.ToLower()));
                if (prdAct != null)
                {
                    return prdAct;
                }
                return new bgt_PrdAct_NC();
            }
        }
    }
}