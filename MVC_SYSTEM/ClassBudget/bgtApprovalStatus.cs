using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class bgtApprovalStatus
    {
        MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();

        public string GetBudgetApprovalStatusDesc(int? status = null)
        {
            var newstatus = status.HasValue ? status.Value : 1;

            var aprvstatusdata = db.bgt_Approval_Status.Where(w => w.Aprv_Status_ID == newstatus).FirstOrDefault();
            var statusdesc = aprvstatusdata.Aprv_Status_Desc;
            return statusdesc;
        }
    }
}