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

        public string GetGLDesc(string glCode)
        {
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                var gl = db.tbl_SAPGLPUP.Where(g => g.fld_GLCode.Contains(glCode) && ((g.fld_Deleted.HasValue && !g.fld_Deleted.Value) || !g.fld_Deleted.HasValue)).FirstOrDefault();
                if (gl != null)
                {
                    return gl.fld_GLDesc;
                }
                return string.Empty;
            }
        }
    }
}