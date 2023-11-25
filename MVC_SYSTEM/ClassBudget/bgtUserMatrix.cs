using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using MVC_SYSTEM.ModelsBudget.ViewModels;

namespace MVC_SYSTEM.ClassBudget
{
    public class BudgetUserMatrix
    {
        MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();

        public RoleScrActionUpdate RoleScreen(string userid, string screencode)
        {
            var getroleid = db.bgt_Users.Where(x => x.usr_ID.ToLower() == userid.ToLower()).Select(s => s.usr_Role).ToList();
            var getscreen = db.bgt_RoleScrAction.Where(x => getroleid.Contains(x.rol_roleID) && x.rol_ScrCode.ToUpper() == screencode.ToUpper()).ToList();
            var RSA = new RoleScrActionUpdate
            {
                add = getscreen.Any(x => x.rol_Add == true) ? true : false,
                edit = getscreen.Any(x => x.rol_Edit == true) ? true : false,
                view = getscreen.Any(x => x.rol_View == true) ? true : false,
                delete = getscreen.Any(x => x.rol_Delete == true) ? true : false,
                download = getscreen.Any(x => x.rol_Download == true) ? true : false,
                upload = getscreen.Any(x => x.rol_Upload == true) ? true : false,
                approve = getscreen.Any(x => x.rol_Approve == true) ? true : false,
                check = getscreen.Any(x => x.rol_Check == true) ? true : false,
                print = getscreen.Any(x => x.rol_Print == true) ? true : false,
                reject = getscreen.Any(x => x.rol_Reject == true) ? true : false
            };
            return RSA;
        }
        public List<bgt_Users> RoleCostCenter(string userid)
        {
            if (!string.IsNullOrEmpty(userid))
            {
                return db.bgt_Users.Where(w => w.usr_ID == userid && w.fld_SyarikatID == 1 && w.usr_Organization.ToUpper() == "CC").ToList();
            }
            return db.bgt_Users.Where(w => w.usr_ID == userid && w.fld_SyarikatID == 1 && w.usr_Organization.ToUpper() == "CC").ToList();
        }

        public List<bgt_Struc> RoleStructures(string userid, string strucid)
        {
            if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(strucid))
            {
                if(strucid == "8")
                {
                    return db.bgt_Struc.Where(w => w.str_User_ID == userid && w.str_Struc_ID == strucid).ToList();
                }
                else
                {
                    return db.bgt_Struc.Where(w => w.str_User_ID == userid && w.str_Struc_ID == strucid).GroupBy(g => new { g.str_Struc_ID, g.str_CodeLL, g.str_Station }).Select(s => s.FirstOrDefault()).ToList();
                }
            }
            return null;
        }
    }
}