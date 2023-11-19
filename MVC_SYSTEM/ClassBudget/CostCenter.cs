//using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ClassBudget;

namespace MVC_SYSTEM.ClassBudget
{
    public class CostCenter
    {
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        private BudgetUserMatrix usermatrix = new BudgetUserMatrix();

        public List<tbl_SAPCCPUP> GetCostCenters()
        {
            //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //int? getuserid = getidentity.ID(HttpContext.Current.User.Identity.Name);
            //string host, catalog, user, pass = "";
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, HttpContext.Current.User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            using (var db = new MVC_SYSTEM_MasterModels())
            {
                //return db.tbl_SAPCCPUP
                //    .Where(w => w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && ((w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue))
                //    .OrderBy(c => c.fld_CostCenter)
                //    .ToList();

                //fitri
                return db.tbl_SAPCCPUP
                    .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue)
                    .OrderBy(c => c.fld_CostCenter)
                    .ToList();

                //fitri nov 2023
                //var Users = usermatrix.RoleCostCenter(HttpContext.Current.User.Identity.Name).FirstOrDefault();
                //var Strucs = usermatrix.RoleStructures(Users.usr_ID, Users.usr_Structure_ID);

                //if (Users.usr_Structure_ID == "0")//All
                //{
                //    return db.tbl_SAPCCPUP
                //        .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue)
                //        .OrderBy(c => c.fld_CostCenter)
                //        .ToList();
                //}
                //else if(Users.usr_Structure_ID == "1")//semenanjung
                //{
                //    return db.tbl_SAPCCPUP
                //        .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue && w.fld_WilayahID == 1)
                //        .OrderBy(c => c.fld_CostCenter)
                //        .ToList();
                //}
                //else if (Users.usr_Structure_ID == "2")//sabah
                //{
                //    return db.tbl_SAPCCPUP
                //        .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue && w.fld_WilayahID == 2)
                //        .OrderBy(c => c.fld_CostCenter)
                //        .ToList();
                //}
                //else if (Users.usr_Structure_ID == "3")//sarawak
                //{
                //    return db.tbl_SAPCCPUP
                //        .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue && w.fld_WilayahID == 2)
                //        .OrderBy(c => c.fld_CostCenter)
                //        .ToList();
                //}
                //else if (Users.usr_Structure_ID == "7")//station
                //{
                //    var CodeLL = new List<string>();

                //    for (int i = 0; i < Strucs.Count(); i++)
                //    {
                //        CodeLL.Add(Strucs[i].str_CodeLL);
                //    }
                //    var data = db.tbl_SAPCCPUP
                //        .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue && w.fld_CostCenter.Substring(5, 2) = "05")
                //        .OrderBy(c => c.fld_CostCenter)
                //        .ToList();
                //    var testcc = data.FirstOrDefault().fld_CostCenter;
                //    var substringcc = data.FirstOrDefault().fld_CostCenter.Substring(5, 2);

                //    return data;
                //}
                //else if (Users.usr_Structure_ID == "8")//costcenter
                //{
                //    var CostCenter = new List<string>();

                //    for(int i = 0; i < Strucs.Count(); i++)
                //    {
                //        CostCenter.Add(Strucs[i].str_CostCenter);
                //    }

                //    return db.tbl_SAPCCPUP
                //        .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue && CostCenter.Contains(w.fld_CostCenter))
                //        .OrderBy(c => c.fld_CostCenter)
                //        .ToList();
                //}
                //else//All
                //{
                //    return db.tbl_SAPCCPUP
                //        .Where(w => (w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue)
                //        .OrderBy(c => c.fld_CostCenter)
                //        .ToList();
                //}
            }
        }

        public List<tbl_SAPCCPUP> GetCostCenters(string ccCode = null, string codePP = null, string codeLL = null)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(HttpContext.Current.User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, HttpContext.Current.User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            using (var db = new MVC_SYSTEM_MasterModels())
            {
                var costCenters = db.tbl_SAPCCPUP
                    .Where(w => w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && ((w.fld_Deleted.HasValue && !w.fld_Deleted.Value) || !w.fld_Deleted.HasValue));

                if (!string.IsNullOrEmpty(ccCode))
                    costCenters = costCenters.Where(c => c.fld_CostCenter.Trim().Equals(ccCode));
                if (!string.IsNullOrEmpty(codePP))
                    costCenters = costCenters.Where(c => c.fld_CostCenter.Trim().Substring(3, 2).Equals(codePP));
                if (!string.IsNullOrEmpty(codeLL))
                    costCenters = costCenters.Where(c => c.fld_CostCenter.Trim().Substring(5, 2).Equals(codeLL));

                return costCenters.OrderBy(c => c.fld_CostCenter).ToList();
            }
        }

        public string GetCostCenterDesc(string costCenterCode)
        {
            using (var db = new MVC_SYSTEM_MasterModels())
            {
                var costCenter = db.tbl_SAPCCPUP
                    .Where(c => c.fld_CostCenter.Contains(costCenterCode) && ((c.fld_Deleted.HasValue && !c.fld_Deleted.Value) || !c.fld_Deleted.HasValue))
                    .FirstOrDefault();
                if (costCenter != null)
                {
                    return costCenter.fld_CostCenterDesc;
                }
                return string.Empty;
            }
        }

        public int GetCostCenterLocID(string costCenterCode)
        {
            using (var db = new MVC_SYSTEM_ModelsBudget())
            {
                var costCenter = from cc in db.tbl_SAPCCPUPs
                                 join c in db.bgt_CostCenterExts on cc.fld_ID equals c.CC_ID into ccJoin
                                 from c in ccJoin.DefaultIfEmpty()
                                 where cc.fld_CostCenter.Contains(costCenterCode)
                                 select new { cc, c };
                if (costCenter != null)
                {
                    return costCenter.Select(c => c.c != null && c.c.CC_ZONE.HasValue ? c.c.CC_ZONE.Value : 0).First();
                }
                return 0;
            }
        }
    }
}