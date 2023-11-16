using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ClassBudget
{
    public class bgtApproval
    {
        private MVC_SYSTEM_ModelsBudgetEst db = new MVC_SYSTEM_ModelsBudgetEst();
        MVC_SYSTEM_ModelsBudget dbc = new MVC_SYSTEM_ModelsBudget();
        public List<int> GetYear()
        {
            return dbc.bgt_Notification.Where(w => w.fld_Deleted == false).OrderByDescending(o => o.Bgt_Year).Select(s => s.Bgt_Year).ToList();
        }
        public bool GetBlockStatus(string CostCenter, int BudgetYear, string ScreenCode)
        {
            bool screenblocks = false;
            var approvaldata = db.bgt_approval
                               .Where(w => w.aprv_budgeting_year == BudgetYear && w.aprv_cost_center_code.Equals(CostCenter) && ((w.aprv_deleted.HasValue && !w.aprv_deleted.Value) || !w.aprv_deleted.HasValue) && w.aprv_scrCode == ScreenCode)
                               .OrderByDescending(o => o.aprv_id)
                               .ThenBy(o => o.aprv_cost_center_code)
                               .FirstOrDefault();
            
            if (approvaldata != null)
            {
                screenblocks = approvaldata.aprv_bgt_block;
            }
            return screenblocks;
        }

        public string IncomeSawitStatus(string CostCenter, int BudgetYear)
        {
            bgt_income_sawit bisdata;
            string status = "";
            bisdata = db.bgt_income_sawit.Where(w => w.abio_cost_center.Equals(CostCenter) && w.abio_budgeting_year == BudgetYear).FirstOrDefault();
            if(bisdata != null)
            {
                status = bisdata.abio_status.ToUpper();
            }
            return status;
        }
        public string BijiBenihStatus(string CostCenter, int BudgetYear)
        {
            bgt_income_bijibenih bibdata;
            string status = "";
            bibdata = db.bgt_income_bijibenih.Where(w => w.abis_cost_center.Equals(CostCenter) && w.abis_budgeting_year == BudgetYear).FirstOrDefault();
            if (bibdata != null)
            {
                status = bibdata.abis_status.ToUpper();
            }
            return status;
        }
        public string IncomeProductStatus(string CostCenter, int BudgetYear)
        {
            bgt_income_product bipdata;
            string status = "";
            bipdata = db.bgt_income_product.Where(w => w.abip_cost_center.Equals(CostCenter) && w.abip_budgeting_year == BudgetYear).FirstOrDefault();
            if (bipdata != null)
            {
                status = bipdata.abip_status.ToUpper();
            }
            return status;
        }
        public string ExpensesVehicleStatus(string CostCenter, int BudgetYear)
        {
            bgt_expenses_vehicle bevdata;
            string status = "";
            bevdata = db.bgt_expenses_vehicle.Where(w => w.abev_cost_center_code.Equals(CostCenter) && w.abev_budgeting_year == BudgetYear).FirstOrDefault();
            if (bevdata != null)
            {
                status = bevdata.abev_status.ToUpper();
            }
            return status;
        }
    }
}