using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class GL
    {
        public List<tbl_SAPGLPUP> GetGLs(int? screenId = null)
        {
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                if (screenId.HasValue && screenId != 0)
                    return db.tbl_SAPGLPUP.Where(g => g.bgt_ScreenGLs.Any(s => s.fld_ScrID == screenId)).OrderBy(g => g.fld_GLCode).ToList();
                else
                    return db.tbl_SAPGLPUP.OrderBy(g => g.fld_GLCode).ToList();
            }
        }
    }
}