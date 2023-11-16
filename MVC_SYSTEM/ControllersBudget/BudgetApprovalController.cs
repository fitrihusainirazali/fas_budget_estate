using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using System.Net;
using System.Data.Entity;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorize(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User,Exe Budget")]
    public class BudgetApprovalController : Controller
    {
        private MVC_SYSTEM_MasterModels dbm = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_ModelsBudget dbc = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_ModelsBudgetEst dbe = new ConnectionBudget().GetConnection();
        private GetConfig config = new GetConfig();
        private errorlog errlog = new errorlog();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        private CostCenter cc = new CostCenter();
        //GetIdentity GetIdentity = new GetIdentity();
        private bgtApprovalStatus bgtstatus = new bgtApprovalStatus();
        private GetBudgetClass budgetclass = new GetBudgetClass();
        private Screen scr = new Screen();

        private List<int> GetYears()
        {
            return dbc.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }
        //private List<tbl_SAPCCPUP> GetCostCenters(int budgetyear, string costcenter, bool delete = false, bool exclude = true)
        //{
        //    if (exclude)
        //    {
        //        var excludeCostCenters = new List<string>();
        //        //1.
        //        var cc_income_sawit = GetIncomeSawitRecords(budgetyear, delete);
        //        for (int i = 0; i < cc_income_sawit.Count(); i++)
        //        {
        //            excludeCostCenters.Add(cc_income_sawit[i].CostCenterCode);
        //        }
        //        //2.
        //        var cc_income_bijibenih = GetIncomeBijiBenihRecords(budgetyear, delete);
        //        for (int i = 0; i < cc_income_bijibenih.Count(); i++)
        //        {
        //            excludeCostCenters.Add(cc_income_bijibenih[i].CostCenterCode);
        //        }
        //        //3.
        //        var cc_income_product = GetIncomeProductRecords(budgetyear, delete);
        //        for (int i = 0; i < cc_income_product.Count(); i++)
        //        {
        //            excludeCostCenters.Add(cc_income_product[i].CostCenterCode);
        //        }
        //        //4.
        //        var cc_exp_insurance = GetExpInsuranceRecords(budgetyear, delete);
        //        for (int i = 0; i < cc_exp_insurance.Count(); i++)
        //        {
        //            excludeCostCenters.Add(cc_exp_insurance[i].CostCenterCode);
        //        }
        //        //5.
        //        var cc_exp_ctrlhq = GetExpCtrlHQRecords(budgetyear, delete);
        //        for (int i = 0; i < cc_exp_ctrlhq.Count(); i++)
        //        {
        //            excludeCostCenters.Add(cc_exp_ctrlhq[i].abhq_cost_center);
        //        }
        //        //last
        //        var includeCostCenters = new List<string>();
        //        var bgt_apprroval = GetRecords(budgetyear, costcenter, delete);
        //        for (int i = 0; i < bgt_apprroval.Count(); i++)
        //        {
        //            includeCostCenters.Add(bgt_apprroval[i].aprv_cost_center_code);
        //        }
        //        var getAllCC = cc.GetCostCenters().Where(c => excludeCostCenters.Contains(c.fld_CostCenter)).ToList();
        //        return getAllCC.Where(c => !includeCostCenters.Contains(c.fld_CostCenter)).OrderBy(c => c.fld_CostCenter).ToList();
        //    }
        //    else
        //    {
        //        return cc.GetCostCenters();
        //    }
        //}

        public ActionResult DialogMsg()
        {
            return PartialView("_DialogMsg");
        }
        public JsonResult GetCostCenterDesc(string costcentercode)
        {
            var CostCenterData = cc.GetCostCenters().Where(w => w.fld_CostCenter.Equals(costcentercode));

            if (CostCenterData.Count() > 0)
            {
                return Json(new { success = true, costcenterdesc = CostCenterData.FirstOrDefault().fld_CostCenterDesc });
            }
            return Json(new { success = false });
        }
        private int GetBudgetUserRoleID()
        {
            var username = User.Identity.Name.ToLower();
            var bgtusersdata = dbc.bgt_Users.Where(w => w.usr_ID.ToLower() == username.Replace(" ", "")).ToList();
            var bgtroleid = 0;
            if (bgtusersdata.Count() > 0)
            {
                bgtroleid = int.Parse(bgtusersdata.FirstOrDefault().usr_Role);
            }
            return bgtroleid;
        }
        public ActionResult Index()
        {
            var bgtroleid = GetBudgetUserRoleID();
            if (bgtroleid == 9) //Exe Budget
            {
                ViewBag.RoleID = bgtroleid;
                ViewBag.Title = "SEND FOR APPROVAL";
            }
            else
            {
                ViewBag.RoleID = bgtroleid;
                ViewBag.Title = "MANAGER APPROVAL";
            }

            List<SelectListItem> BudgetYear = new List<SelectListItem>();
            BudgetYear = new SelectList(dbc.bgt_Notification.Where(w => w.fld_Deleted == false).Select(s => new SelectListItem { Value = s.Bgt_Year.ToString(), Text = s.Bgt_Year.ToString() }).OrderByDescending(o => o.Value), "Value", "Text").ToList();
            ViewBag.BudgetYear = BudgetYear;

            var SelectedYear = GetYears().FirstOrDefault().ToString();
            ViewBag.SelectedYear = SelectedYear;

            return View();
        }
        public ActionResult Records(string BudgetYear, bool? Deleted = false, int page = 1)
        {
            int pageSize = 9999; //int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var records = new ViewingModels.PagedList<BudgetApprovalViewModel>
            {
                Content = GetIncomeExpRecords(budgetYear, Deleted, page, pageSize),
                TotalRecords = GetIncomeExpRecords(budgetYear, Deleted).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };
            ViewBag.RoleID = GetBudgetUserRoleID();
            return PartialView("_Records", records);
        }

        //public ActionResult AddDetails()
        //{
        //    var syktid = syarikat.GetSyarikatID();
        //    var wlyhid = wilayah.GetWilayahID();

        //    var SelectedYear = GetYears().FirstOrDefault().ToString();
        //    var year = int.Parse(SelectedYear);
        //    var costcenter = "";

        //    var cost_center = new SelectList(GetCostCenters(year, costcenter, false).Select(s => new SelectListItem
        //    {
        //        Value = s.fld_CostCenter,
        //        Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc
        //    }), "Value", "Text").ToList();

        //    ViewBag.CostCenter = cost_center;
        //    ViewBag.SelectedYear = SelectedYear;
        //    ViewBag.Title = "SEND FOR APPROVAL";

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddDetails(bgt_approval model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            int? getuserid = getidentity.ID(User.Identity.Name);
        //            var syktid = syarikat.GetSyarikatID();
        //            var wlyhid = wilayah.GetWilayahID();

        //            var ba = new bgt_approval
        //            {
        //                aprv_budgeting_year = model.aprv_budgeting_year,
        //                aprv_cost_center_code = model.aprv_cost_center_code,
        //                aprv_cost_center_desc = model.aprv_cost_center_desc,
        //                aprv_date = DateTime.Now,
        //                aprv_deleted = false,
        //                aprv_prepared_by = (int)getuserid,
        //                aprv_status_id = 1,
        //                last_modified = DateTime.Now
        //            };
        //            dbe.bgt_approval.Add(ba);
        //            dbe.SaveChanges();

        //            string appname = Request.ApplicationPath;
        //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //            var lang = Request.RequestContext.RouteData.Values["lang"];

        //            if (appname != "/")
        //            {
        //                domain = domain + appname;
        //            }

        //            return Json(new
        //            {
        //                success = true,
        //                msg = GlobalResEstate.msgAdd,
        //                status = "success",
        //                div = "RecordsPartial",
        //                rooturl = domain,
        //                controller = "BudgetApproval",
        //                action = "Records",
        //                paramName1 = "BudgetYear",
        //                paramValue1 = model.aprv_budgeting_year.ToString(),
        //                paramName2 = "CostCenter",
        //                paramValue2 = model.aprv_cost_center_code.ToString()
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new
        //            {
        //                success = false,
        //                msg = GlobalResEstate.msgError,
        //                status = "danger",
        //            });
        //        }
        //    }
        //    else
        //    {
        //        dbe.Dispose();
        //        return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }
        //}
        public ActionResult SendForApproval(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int? budgetyear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var costcenter = !string.IsNullOrEmpty(CostCenter) ? CostCenter : null;

            ViewBag.BudgetYear = budgetyear;
            ViewBag.CostCenter = costcenter;

            return PartialView("_SendForApproval");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendForApproval(string CostCenter)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? getuserid = getidentity.ID(User.Identity.Name);
                    var syktid = syarikat.GetSyarikatID();
                    //int? getuserroleid = GetBudgetUserRoleID();
                    //var wlyhid = wilayah.GetWilayahID();
                    var NewCostCenter = CostCenter.Split(',');
                    var year = GetYears().FirstOrDefault();
                    foreach (var val in NewCostCenter)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            string newcc = "";
                            string scode = "";

                            if (val.StartsWith("P"))
                            {
                                newcc = val.Substring(0, 10);

                                if (val.Length == 12)//P820105002I1
                                {
                                    scode = val.Substring(val.Length - 2).ToString();
                                }
                                else if (val.Length == 13)//P820105002E11
                                {
                                    scode = val.Substring(val.Length - 3).ToString();
                                }
                                else
                                {
                                    scode = val.Substring(val.Length - 4).ToString();
                                }
                            }
                            else
                            {
                                newcc = val.Substring(0, 9);

                                if (val.Length == 11)//820105002I1
                                {
                                    //scode = val[val.Length - 2].ToString() + val[val.Length - 1].ToString();
                                    scode = val.Substring(val.Length - 2).ToString();
                                }
                                else if (val.Length == 12)//820105002E11
                                {
                                    //scode = val[val.Length - 3].ToString() + val[val.Length - 2].ToString() + val[val.Length - 1].ToString();
                                    scode = val.Substring(val.Length - 3).ToString();
                                }
                                else
                                {
                                    //scode = val[val.Length - 4].ToString() + val[val.Length - 3].ToString() + val[val.Length - 2].ToString() + val[val.Length - 1].ToString();
                                    scode = val.Substring(val.Length - 4).ToString();
                                }
                            }

                            var budgetapprovaldata = dbe.bgt_approval
                                .Where(w => w.aprv_budgeting_year == year && w.aprv_cost_center_code.Contains(newcc.ToString()) && w.aprv_scrCode == scode)
                                .OrderByDescending(o => o.aprv_id)
                                .FirstOrDefault();

                            //if (budgetapprovaldata != null)
                            //{
                            //budgetapprovaldata.aprv_note = budgetapprovaldata.aprv_note;
                            //budgetapprovaldata.last_modified = DateTime.Now;
                            //budgetapprovaldata.aprv_status_id = 2;
                            //budgetapprovaldata.aprv_prepared_by = (int)getuserid;
                            //budgetapprovaldata.aprv_bgt_block = true;
                            //dbe.Entry(budgetapprovaldata).State = EntityState.Modified;

                            //UpdateBudgetStatus(year, CostCenter, false, "sawit", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "bijibenih", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "product", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "levi", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expcapital", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expctrlhq", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expdepreciation", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expgaji", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expinsurance", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expit", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expkhidcorp", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expmedical", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expothers", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "exppau", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expsalary", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expvehicle", "edit");
                            //UpdateBudgetStatus(year, CostCenter, false, "expwindfall", "edit");
                            //}
                            //else
                            //{
                            if (budgetapprovaldata != null)
                            {
                                var ba = new bgt_approval()
                                {
                                    aprv_budgeting_year = GetYears().FirstOrDefault(),
                                    aprv_cost_center_code = newcc.ToString(),
                                    aprv_cost_center_desc = cc.GetCostCenterDesc(newcc.ToString()),
                                    aprv_deleted = false,
                                    aprv_prepared_by = budgetapprovaldata.aprv_prepared_by,
                                    aprv_Send_date = budgetapprovaldata.aprv_Send_date,
                                    aprv_status_id = 2,//check
                                    last_modified = DateTime.Now,
                                    aprv_bgt_block = false,
                                    aprv_organization = 2,//CC
                                    aprv_scrCode = budgetapprovaldata.aprv_scrCode,
                                    aprv_syarikat_id = budgetapprovaldata.aprv_syarikat_id,
                                    aprv_revision = budgetapprovaldata.aprv_revision
                                };
                                dbe.bgt_approval.Add(ba);

                                //budgetapprovaldata.aprv_budgeting_year = GetYears().FirstOrDefault();
                                //budgetapprovaldata.aprv_cost_center_code = val.ToString();
                                //budgetapprovaldata.aprv_cost_center_desc = cc.GetCostCenterDesc(val.ToString());
                                //budgetapprovaldata.aprv_deleted = false;
                                //budgetapprovaldata.aprv_status_id = 2;
                                //budgetapprovaldata.last_modified = DateTime.Now;
                                //budgetapprovaldata.aprv_bgt_block = false;
                                //budgetapprovaldata.aprv_organization = 1;
                                //dbe.Entry(budgetapprovaldata).State = EntityState.Modified;
                            }
                            else
                            {
                                var ba = new bgt_approval()
                                {
                                    aprv_budgeting_year = GetYears().FirstOrDefault(),
                                    aprv_cost_center_code = newcc.ToString(),
                                    aprv_cost_center_desc = cc.GetCostCenterDesc(newcc.ToString()),//new aug
                                    aprv_deleted = false,
                                    aprv_prepared_by = (int)getuserid,
                                    aprv_Send_date = DateTime.Now,
                                    aprv_status_id = 2,//check
                                    last_modified = DateTime.Now,
                                    aprv_bgt_block = false,
                                    aprv_organization = 2,
                                    aprv_scrCode = scode,
                                    aprv_syarikat_id = syktid,
                                    aprv_revision = 1
                                };
                                dbe.bgt_approval.Add(ba);
                            }

                            //UpdateBudgetStatus(year, CostCenter, false, "sawit", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "bijibenih", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "product", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "levi", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expcapital", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expctrlhq", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expdepreciation", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expgaji", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expinsurance", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expit", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expkhidcorp", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expmedical", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expothers", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "exppau", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expsalary", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expvehicle", "add");
                            //UpdateBudgetStatus(year, CostCenter, false, "expwindfall", "add");
                            //}
                        }
                    }
                    dbe.SaveChanges();
                    //UpdateApprovalLog(year, CostCenter, 2, getuserid);//temp comment

                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg = "Record has been send for approval",
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "BudgetApproval",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = year.ToString(),
                        //paramName2 = "CostCenter",
                        //paramValue2 = model.aprv_cost_center_code.ToString()
                    });
                }
                catch (Exception ex)
                {
                    errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgError,
                        status = "danger",
                    });
                }
            }
            else
            {
                dbe.Dispose();
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult Approve(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int? budgetyear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var costcenter = !string.IsNullOrEmpty(CostCenter) ? CostCenter : null;

            ViewBag.BudgetYear = budgetyear;
            ViewBag.CostCenter = costcenter;

            return PartialView("_Approve");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(string CostCenter, BudgetApprovalViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? getuserid = getidentity.ID(User.Identity.Name);
                    var syktid = syarikat.GetSyarikatID();
                    var NewCostCenter = CostCenter.Split(',');

                    foreach (var val in NewCostCenter)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            //old code
                            //var budgetapprovaldata = dbe.bgt_approval.Where(w => w.aprv_budgeting_year == ba.aprv_budgeting_year && w.aprv_cost_center_code.Contains(val.ToString())).FirstOrDefault();
                            //budgetapprovaldata.aprv_note = ba.aprv_note;
                            //budgetapprovaldata.last_modified = DateTime.Now;
                            //budgetapprovaldata.aprv_status_id = 3;
                            //budgetapprovaldata.aprv_prepared_by = (int)getuserid;
                            //budgetapprovaldata.aprv_bgt_block = true;
                            //dbe.Entry(budgetapprovaldata).State = EntityState.Modified;

                            string newcc = "";
                            string scode = "";

                            if (val.StartsWith("P"))
                            {
                                newcc = val.Substring(0, 10);

                                if (val.Length == 12)//P820105002I1
                                {
                                    scode = val.Substring(val.Length - 2).ToString();
                                }
                                else if (val.Length == 13)//P820105002E11
                                {
                                    scode = val.Substring(val.Length - 3).ToString();
                                }
                                else
                                {
                                    scode = val.Substring(val.Length - 4).ToString();
                                }
                            }
                            else
                            {
                                newcc = val.Substring(0, 9);

                                if (val.Length == 11)//820105002I1
                                {
                                    scode = val.Substring(val.Length - 2).ToString();
                                }
                                else if (val.Length == 12)//820105002E11
                                {
                                    scode = val.Substring(val.Length - 3).ToString();
                                }
                                else
                                {
                                    scode = val.Substring(val.Length - 4).ToString();
                                }
                            }

                            var data = dbe.bgt_approval
                                .Where(w => w.aprv_budgeting_year == model.aprv_budgeting_year && w.aprv_cost_center_code == newcc.ToString() && w.aprv_scrCode == scode)
                                .OrderByDescending(o => o.aprv_id)
                                .ToList();

                            var ba = new bgt_approval()
                            {
                                aprv_budgeting_year = GetYears().FirstOrDefault(),
                                aprv_cost_center_code = newcc.ToString(),
                                aprv_cost_center_desc = cc.GetCostCenterDesc(newcc.ToString()),
                                aprv_date = DateTime.Now,//approve or reject date
                                aprv_Send_date = data.FirstOrDefault().aprv_Send_date,
                                aprv_deleted = false,
                                aprv_prepared_by = data.FirstOrDefault().aprv_prepared_by,
                                aprv_Approve_by = (int)getuserid,
                                aprv_status_id = 3,//approve cc
                                last_modified = DateTime.Now,
                                aprv_bgt_block = true,
                                aprv_note = model.aprv_note,
                                aprv_organization = 1, //HQ
                                aprv_scrCode = data.FirstOrDefault().aprv_scrCode,
                                aprv_syarikat_id = syktid,
                                aprv_revision = data.FirstOrDefault().aprv_revision
                            };
                            dbe.bgt_approval.Add(ba);

                            //if (!string.IsNullOrEmpty(val))
                            //{
                            //    var data = dbe.bgt_approval.Where(w => w.aprv_budgeting_year == model.aprv_budgeting_year && w.aprv_cost_center_code == val.ToString()).FirstOrDefault();

                            //    data.aprv_budgeting_year = (int)model.aprv_budgeting_year;
                            //    data.aprv_cost_center_code = val.ToString();
                            //    data.aprv_cost_center_desc = cc.GetCostCenterDesc(val.ToString());
                            //    data.aprv_date = DateTime.Now;//approve or reject date
                            //    data.aprv_deleted = false;
                            //    data.aprv_Approve_by = (int)getuserid;
                            //    data.aprv_status_id = 3;
                            //    data.last_modified = DateTime.Now;
                            //    data.aprv_bgt_block = false;
                            //    data.aprv_note = model.aprv_note;
                            //    data.aprv_organization = 1;
                            //    dbe.Entry(data).State = EntityState.Modified;
                            //}
                        }
                    }
                    dbe.SaveChanges();
                    //UpdateApprovalLog((int)ba.aprv_budgeting_year, CostCenter, 3, getuserid);//temp comment

                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg = "Record has been succesfully approved",
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "BudgetApproval",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.aprv_budgeting_year.ToString(),
                        //paramName2 = "CostCenter",
                        //paramValue2 = model.aprv_cost_center_code.ToString()
                    });
                }
                catch (Exception ex)
                {
                    errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgError,
                        status = "danger",
                    });
                }
            }
            else
            {
                dbe.Dispose();
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }
        public ActionResult Reject(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int? budgetyear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var costcenter = !string.IsNullOrEmpty(CostCenter) ? CostCenter : null;

            ViewBag.BudgetYear = budgetyear;
            ViewBag.CostCenter = costcenter;

            return PartialView("_Reject");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(string CostCenter, BudgetApprovalViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int? getuserid = getidentity.ID(User.Identity.Name);
                    var syktid = syarikat.GetSyarikatID();
                    var NewCostCenter = CostCenter.Split(',');

                    foreach (var val in NewCostCenter)
                    {
                        if (!string.IsNullOrEmpty(val))
                        {
                            //old code
                            //var budgetapprovaldata = dbe.bgt_approval.Where(w => w.aprv_budgeting_year == ba.aprv_budgeting_year && w.aprv_cost_center_code.Contains(val.ToString())).FirstOrDefault();
                            //budgetapprovaldata.aprv_note = ba.aprv_note;
                            //budgetapprovaldata.last_modified = DateTime.Now;
                            //budgetapprovaldata.aprv_status_id = 4;
                            //budgetapprovaldata.aprv_prepared_by = (int)getuserid;
                            //budgetapprovaldata.aprv_bgt_block = false;
                            //dbe.Entry(budgetapprovaldata).State = EntityState.Modified;

                            string newcc = "";
                            string scode = "";

                            if (val.StartsWith("P"))
                            {
                                newcc = val.Substring(0, 10);

                                if (val.Length == 12)//P820105002I1
                                {
                                    scode = val.Substring(val.Length - 2).ToString();
                                }
                                else if (val.Length == 13)//P820105002E11
                                {
                                    scode = val.Substring(val.Length - 3).ToString();
                                }
                                else
                                {
                                    scode = val.Substring(val.Length - 4).ToString();
                                }
                            }
                            else
                            {
                                newcc = val.Substring(0, 9);

                                if (val.Length == 11)//820105002I1
                                {
                                    scode = val.Substring(val.Length - 2).ToString();
                                }
                                else if (val.Length == 12)//820105002E11
                                {
                                    scode = val.Substring(val.Length - 3).ToString();
                                }
                                else
                                {
                                    scode = val.Substring(val.Length - 4).ToString();
                                }
                            }

                            var data = dbe.bgt_approval
                                .Where(w => w.aprv_budgeting_year == model.aprv_budgeting_year && w.aprv_cost_center_code == newcc.ToString() && w.aprv_scrCode == scode)
                                .OrderByDescending(o => o.aprv_id)
                                .ToList();

                            var ba = new bgt_approval()
                            {
                                aprv_budgeting_year = GetYears().FirstOrDefault(),
                                aprv_cost_center_code = newcc.ToString(),
                                aprv_cost_center_desc = cc.GetCostCenterDesc(newcc.ToString()),
                                aprv_date = DateTime.Now,//approve or reject date
                                aprv_Send_date = data.FirstOrDefault().aprv_Send_date,
                                aprv_deleted = false,
                                aprv_prepared_by = data.FirstOrDefault().aprv_prepared_by,
                                aprv_Approve_by = (int)getuserid,
                                aprv_status_id = 4,
                                last_modified = DateTime.Now,
                                aprv_bgt_block = false,
                                aprv_note = model.aprv_note,
                                aprv_organization = 2,//CC
                                aprv_scrCode = data.FirstOrDefault().aprv_scrCode,
                                aprv_syarikat_id = syktid,
                                aprv_revision = data.FirstOrDefault().aprv_revision
                            };
                            dbe.bgt_approval.Add(ba);

                            //if (!string.IsNullOrEmpty(val))
                            //{
                            //    var data = dbe.bgt_approval.Where(w => w.aprv_budgeting_year == model.aprv_budgeting_year && w.aprv_cost_center_code == val.ToString()).FirstOrDefault();

                            //    data.aprv_budgeting_year = (int)model.aprv_budgeting_year;
                            //    data.aprv_cost_center_code = val.ToString();
                            //    data.aprv_cost_center_desc = cc.GetCostCenterDesc(val.ToString());
                            //    data.aprv_date = DateTime.Now;//approve or reject date
                            //    data.aprv_deleted = false;
                            //    data.aprv_Approve_by = (int)getuserid;
                            //    data.aprv_status_id = 4;
                            //    data.last_modified = DateTime.Now;
                            //    data.aprv_bgt_block = false;
                            //    data.aprv_note = model.aprv_note;
                            //    data.aprv_organization = 2;
                            //    dbe.Entry(data).State = EntityState.Modified;
                            //}
                        }
                    }
                    dbe.SaveChanges();
                    //UpdateApprovalLog((int)model.aprv_budgeting_year, CostCenter, 4, getuserid, model.aprv_note);//temp comment

                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg = "Record has been succesfully rejected",
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "BudgetApproval",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.aprv_budgeting_year.ToString(),
                        //paramName2 = "CostCenter",
                        //paramValue2 = model.aprv_cost_center_code.ToString()
                    });
                }
                catch (Exception ex)
                {
                    errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgError,
                        status = "danger",
                    });
                }
            }
            else
            {
                dbe.Dispose();
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult Note(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bgtapproval = dbe.bgt_approval.Find(id);
            if (bgtapproval == null)
            {
                return HttpNotFound();
            }

            var model = new BudgetApprovalViewModel
            {
                aprv_note = bgtapproval.aprv_note
            };
            return PartialView("Note", model);
        }

        public ActionResult Legend()
        {
            return PartialView("Legend");
        }

        private List<BudgetApprovalViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var syktid = syarikat.GetSyarikatID();
            var wlyhid = wilayah.GetWilayahID();

            var query = from p in dbe.bgt_approval where p.aprv_deleted == Deleted select p;

            if (BudgetYear.HasValue)
                query = query.Where(q => q.aprv_budgeting_year == BudgetYear);
            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.aprv_cost_center_code.Equals(CostCenter) || q.aprv_cost_center_desc.Equals(CostCenter));

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.aprv_budgeting_year)
                    .ThenBy(q => q.aprv_cost_center_code)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.aprv_budgeting_year)
                    .ThenBy(q => q.aprv_cost_center_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_id = q.aprv_id,
                              aprv_cost_center_code = q.aprv_cost_center_code,
                              aprv_cost_center_desc = q.aprv_cost_center_desc,
                              aprv_status = q.aprv_status_id.ToString(),
                              aprv_budgeting_year = q.aprv_budgeting_year,
                              aprv_date = q.aprv_date,
                              last_modified = q.last_modified
                          }).Distinct();

            return query3.ToList();
        }

        private List<BudgetApprovalViewModel> GetIncomeExpRecords(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var listincomesawit = GetIncomeSawitRecords(BudgetYear, Deleted);   //I1
            var listincomebb = GetIncomeBijiBenihRecords(BudgetYear, Deleted);  //I2
            var listincomeprd = GetIncomeProductRecords(BudgetYear, Deleted);   //I3
            var listexpcapital = GetExpCapital(BudgetYear, Deleted);
            var listexpctrlhq = GetExpCtrlHQRecords(BudgetYear, Deleted);
            var listexpdepreciation = GetExpDepreciation(BudgetYear, Deleted);
            var listexpgaji = GetExpGaji(BudgetYear, Deleted);
            var listexpinsurance = GetExpInsuranceRecords(BudgetYear, Deleted);
            var listexpit = GetExpIT(BudgetYear, Deleted);
            var listexpkhidcorp = GetExpKhidCorp(BudgetYear, Deleted);
            var listexplevi = GetExpLevi(BudgetYear, Deleted);
            var listexpmedical = GetExpMedical(BudgetYear, Deleted);
            var listexpothers = GetExpOthers(BudgetYear, Deleted);
            var listexppau = GetExpPau(BudgetYear, Deleted);
            var listexpsalary = GetExpSalary(BudgetYear, Deleted);
            var listexpvehicle = GetExpVehicle(BudgetYear, Deleted);
            var listexpwindfall = GetExpWindfall(BudgetYear, Deleted);
            //var listbgtapproval = GetBudgetApprovalRecords(BudgetYear, Deleted);
            //var listexpvehiclereg = GetVehicleReg(BudgetYear, Deleted);

            var uniondata = listincomesawit
                            .Union(listincomebb)
                            .Union(listincomeprd)
                            .Union(listexpcapital)
                            .Union(listexpctrlhq)
                            .Union(listexpdepreciation)
                            .Union(listexpgaji)
                            .Union(listexpinsurance)
                            .Union(listexpit)
                            .Union(listexpkhidcorp)
                            .Union(listexplevi)
                            .Union(listexpmedical)
                            .Union(listexpothers)
                            .Union(listexppau)
                            .Union(listexpsalary)
                            .Union(listexpvehicle)
                            .Union(listexpwindfall)
                            //.OrderBy(o => o.aprv_cost_center_code)
                            //.GroupBy(g => g.aprv_cost_center_code)
                            //.Select(s => s.FirstOrDefault())
                            .ToList();

            var list = new List<BudgetApprovalViewModel>();

            if (uniondata.Count > 0)
            {
                foreach (var val in uniondata)
                {
                    string codell = "";
                    var rawcc = val.aprv_cost_center_code;
                    if (rawcc.StartsWith("P"))
                    {
                        codell = rawcc.Substring(5, 2);
                    }
                    else
                    {
                        codell = rawcc.Substring(4, 2);
                    }

                    var newlistbgtapproval = dbe.bgt_approval.Where(w => w.aprv_budgeting_year == val.aprv_budgeting_year && w.aprv_cost_center_code == val.aprv_cost_center_code && w.aprv_scrCode == val.ScreenName)
                                            //.GroupBy(g => new { g.aprv_cost_center_code, g.aprv_scrCode })
                                            //.Select(s => s.FirstOrDefault())
                                            .OrderByDescending(o => o.aprv_id)
                                            .ThenBy(o => o.aprv_scrCode)
                                            .ThenBy(o => o.aprv_status_id)
                                            .ThenBy(o => o.aprv_cost_center_code)
                                            //.ThenByDescending(o => o.aprv_id)
                                            .ToList();

                    if (newlistbgtapproval.Count > 0)
                    {
                        list.Add(new BudgetApprovalViewModel
                        {
                            aprv_id = newlistbgtapproval.FirstOrDefault().aprv_id,
                            aprv_cost_center_code = val.aprv_cost_center_code,
                            aprv_cost_center_desc = val.aprv_cost_center_desc,
                            aprv_status = newlistbgtapproval.FirstOrDefault().aprv_status_id.ToString(),
                            aprv_budgeting_year = newlistbgtapproval.FirstOrDefault().aprv_budgeting_year,
                            aprv_station = budgetclass.GetStationDesc(codell.ToString()),
                            aprv_Send_date = newlistbgtapproval.FirstOrDefault().aprv_Send_date,
                            aprv_prepared_by = budgetclass.MyNameFullName(newlistbgtapproval.FirstOrDefault().aprv_prepared_by),
                            aprv_Approve_by = budgetclass.MyNameFullName(newlistbgtapproval.FirstOrDefault().aprv_Approve_by),
                            aprv_date = newlistbgtapproval.FirstOrDefault().aprv_date,
                            sum_income = budgetclass.GetTotalIncome(val.aprv_cost_center_code, val.aprv_budgeting_year.ToString(), val.ScreenName.ToString()),
                            sum_expenses = budgetclass.GetTotalExpenses(val.aprv_cost_center_code, val.aprv_budgeting_year.ToString(), val.ScreenName.ToString()),
                            aprv_organization = newlistbgtapproval.FirstOrDefault().aprv_organization,
                            ScreenName = scr.GetScreen(newlistbgtapproval.FirstOrDefault().aprv_scrCode).ScrName.ToString(),
                            aprv_scrCode = newlistbgtapproval.FirstOrDefault().aprv_scrCode
                        });
                    }
                    else
                    {
                        list.Add(new BudgetApprovalViewModel
                        {
                            aprv_cost_center_code = val.aprv_cost_center_code,
                            aprv_cost_center_desc = val.aprv_cost_center_desc,
                            aprv_budgeting_year = null,
                            aprv_station = budgetclass.GetStationDesc(codell.ToString()),
                            aprv_date = null,
                            aprv_Send_date = null,
                            aprv_prepared_by = null,
                            aprv_Approve_by = null,
                            sum_income = budgetclass.GetTotalIncome(val.aprv_cost_center_code, val.aprv_budgeting_year.ToString(), val.ScreenName.ToString()),
                            sum_expenses = budgetclass.GetTotalExpenses(val.aprv_cost_center_code, val.aprv_budgeting_year.ToString(), val.ScreenName.ToString()),
                            ScreenName = scr.GetScreen(val.ScreenName).ScrName.ToString(),
                            aprv_scrCode = val.ScreenName
                        });
                    }
                }
            }
            return list;
            //var query = list.AsQueryable();
            //return query.ToList();


            //var query = from p in dbe.bgt_income_sawit where p.fld_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abio_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abio_budgeting_year)
            //        .ThenBy(q => q.abio_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abio_budgeting_year)
            //        .ThenBy(q => q.abio_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q
            //                  //aprv_cost_center_desc = q.abio_cost_center_desc,
            //                  //aprv_budgeting_year = (int)q.abio_budgeting_year
            //              }).Distinct();

            //return query3.ToList();

            //foreach(var val in query2)
            //{
            //    var query3 = new BudgetApprovalViewModel()
            //    {
            //        aprv_cost_center_code = val
            //    };
            //}
        }
        private List<BudgetApprovalViewModel> GetBudgetApprovalRecords(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var query = from p in dbe.bgt_approval where p.aprv_deleted == Deleted select p;

            if (BudgetYear.HasValue)
                query = query.Where(q => q.aprv_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.aprv_budgeting_year)
                    .ThenBy(q => q.aprv_cost_center_code)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.aprv_budgeting_year)
                    .ThenBy(q => q.aprv_cost_center_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_id = q.aprv_id,
                              aprv_status = q.aprv_status_id.ToString(),
                              aprv_budgeting_year = q.aprv_budgeting_year,
                              aprv_prepared_by = q.aprv_prepared_by.ToString(),
                              aprv_Send_date = q.aprv_Send_date,
                              aprv_Approve_by = q.aprv_Approve_by.ToString(),
                              aprv_date = q.aprv_date
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetIncomeSawitRecords(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_income_sawit where p.fld_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abio_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abio_budgeting_year)
            //        .ThenBy(q => q.abio_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abio_budgeting_year)
            //        .ThenBy(q => q.abio_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abio_cost_center,
            //                  aprv_cost_center_desc = q.abio_cost_center_desc,
            //                  aprv_budgeting_year = (int)q.abio_budgeting_year,
            //                  aprv_id = q.abio_id,
            //                  amt_sawit = (decimal)q.abio_grand_total_amount,
            //                  ScreenName = "I1"
            //              }).Distinct();

            var query = from p in dbe.bgt_income_sawit where p.fld_Deleted == Deleted group p by new { p.abio_budgeting_year, p.abio_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abio_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abio_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abio_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abio_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abio_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abio_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abio_cost_center_desc,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abio_budgeting_year,
                              aprv_id = q.FirstOrDefault().abio_id,
                              amt_sawit = (decimal)q.FirstOrDefault().abio_grand_total_amount,
                              ScreenName = "I1"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetIncomeBijiBenihRecords(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_income_bijibenih where p.fld_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abis_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abis_budgeting_year)
            //        .ThenBy(q => q.abis_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abis_budgeting_year)
            //        .ThenBy(q => q.abis_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abis_cost_center,
            //                  aprv_cost_center_desc = q.abis_cost_center_desc,
            //                  aprv_budgeting_year = (int)q.abis_budgeting_year,
            //                  aprv_id = q.abis_id,
            //                  amt_bijibenih = (decimal)q.abis_grand_total_amount,
            //                  ScreenName = "I2"
            //              }).Distinct();

            var query = from p in dbe.bgt_income_bijibenih where p.fld_Deleted == Deleted group p by new { p.abis_budgeting_year, p.abis_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abis_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abis_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abis_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abis_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abis_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abis_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abis_cost_center_desc,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abis_budgeting_year,
                              aprv_id = q.FirstOrDefault().abis_id,
                              amt_bijibenih = (decimal)q.FirstOrDefault().abis_grand_total_amount,
                              ScreenName = "I2"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetIncomeProductRecords(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_income_product where p.fld_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abip_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abip_budgeting_year)
            //        .ThenBy(q => q.abip_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abip_budgeting_year)
            //        .ThenBy(q => q.abip_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abip_cost_center,
            //                  aprv_cost_center_desc = q.abip_cost_center_desc,
            //                  aprv_budgeting_year = (int)q.abip_budgeting_year,
            //                  aprv_id = q.abip_id,
            //                  amt_produk = (decimal)q.abip_grand_total_amount,
            //                  ScreenName = "I3"
            //              }).Distinct();

            var query = from p in dbe.bgt_income_product where p.fld_Deleted == Deleted group p by new { p.abip_budgeting_year, p.abip_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abip_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abip_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abip_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abip_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abip_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abip_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abip_cost_center_desc,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abip_budgeting_year,
                              aprv_id = q.FirstOrDefault().abip_id,
                              amt_produk = (decimal)q.FirstOrDefault().abip_grand_total_amount,
                              ScreenName = "I3"
                          }).Distinct();

            return query3.ToList();
        }

        //==========EXPENSES==========//

        private List<BudgetApprovalViewModel> GetExpInsuranceRecords(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_Insurans where p.abei_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abei_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abei_budgeting_year)
            //        .ThenBy(q => q.abei_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abei_budgeting_year)
            //        .ThenBy(q => q.abei_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abei_cost_center,
            //                  aprv_cost_center_desc = q.abei_cost_center_name,
            //                  aprv_id = q.abei_id,
            //                  ScreenName = "E3"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_Insurans where p.abei_Deleted == Deleted group p by new { p.abei_budgeting_year, p.abei_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abei_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abei_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abei_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abei_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abei_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abei_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abei_cost_center_name,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abei_budgeting_year,
                              aprv_id = q.FirstOrDefault().abei_id,
                              ScreenName = "E3"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpCapital(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_capitals where p.abce_deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abce_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abce_budgeting_year)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abce_budgeting_year);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abce_cost_center_code,
            //                  aprv_cost_center_desc = q.abce_cost_center_desc,
            //                  aprv_id = q.abce_id,
            //                  aprv_budgeting_year = q.abce_budgeting_year,
            //                  ScreenName = "E9"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_capitals where p.abce_deleted == Deleted group p by new { p.abce_budgeting_year, p.abce_cost_center_code } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abce_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abce_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abce_cost_center_code)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abce_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abce_cost_center_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abce_cost_center_code,
                              aprv_cost_center_desc = q.FirstOrDefault().abce_cost_center_desc,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abce_budgeting_year,
                              aprv_id = q.FirstOrDefault().abce_id,
                              ScreenName = "E9"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpCtrlHQRecords(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_CtrlHQ where p.abhq_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abhq_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abhq_budgeting_year)
            //        .ThenBy(q => q.abhq_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abhq_budgeting_year)
            //        .ThenBy(q => q.abhq_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abhq_cost_center,
            //                  aprv_cost_center_desc = q.abhq_cost_center_name,
            //                  aprv_id = q.abhq_id,
            //                  aprv_budgeting_year = (int)q.abhq_budgeting_year,
            //                  ScreenName = "E5"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_CtrlHQ where p.abhq_Deleted == Deleted group p by new { p.abhq_budgeting_year, p.abhq_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abhq_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abhq_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abhq_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abhq_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abhq_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abhq_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abhq_cost_center_name,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abhq_budgeting_year,
                              aprv_id = q.FirstOrDefault().abhq_id,
                              ScreenName = "E5"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpDepreciation(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_Depreciation where p.abed_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abed_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abed_budgeting_year)
            //        .ThenBy(q => q.abed_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abed_budgeting_year)
            //        .ThenBy(q => q.abed_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abed_cost_center,
            //                  aprv_cost_center_desc = q.abed_cost_center_name,
            //                  aprv_id = q.abed_id,
            //                  aprv_budgeting_year = (int)q.abed_budgeting_year,
            //                  ScreenName = "E1"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_Depreciation where p.abed_Deleted == Deleted group p by new { p.abed_budgeting_year, p.abed_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abed_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abed_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abed_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abed_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abed_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abed_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abed_cost_center_name,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abed_budgeting_year,
                              aprv_id = q.FirstOrDefault().abed_id,
                              ScreenName = "E1"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpGaji(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var query = from p in dbe.bgt_expenses_gajis where p.abeg_Deleted == Deleted select p;

            if (BudgetYear.HasValue)
                query = query.Where(q => q.abeg_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abeg_budgeting_year)
                    .ThenBy(q => q.abeg_cost_center_code)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abeg_budgeting_year)
                    .ThenBy(q => q.abeg_cost_center_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.abeg_cost_center_code,
                              aprv_cost_center_desc = q.abeg_cost_center_desc,
                              aprv_id = q.abeg_id,
                              aprv_budgeting_year = q.abeg_budgeting_year,
                              ScreenName = "E12"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpIT(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_IT where p.abet_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abet_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abet_budgeting_year)
            //        .ThenBy(q => q.abet_cost_center)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abet_budgeting_year)
            //        .ThenBy(q => q.abet_cost_center);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abet_cost_center,
            //                  aprv_cost_center_desc = q.abet_cost_center_name,
            //                  aprv_id = q.abet_id,
            //                  aprv_budgeting_year = (int)q.abet_budgeting_year,
            //                  ScreenName = "E4"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_IT where p.abet_Deleted == Deleted group p by new { p.abet_budgeting_year, p.abet_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abet_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abet_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abet_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abet_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abet_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abet_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abet_cost_center_name,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abet_budgeting_year,
                              aprv_id = q.FirstOrDefault().abet_id,
                              ScreenName = "E4"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpKhidCorp(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_KhidCor where p.abcs_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abcs_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abcs_budgeting_year)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abcs_budgeting_year);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abcs_cost_center,
            //                  aprv_cost_center_desc = q.abcs_cost_center_name,
            //                  aprv_id = q.abcs_id,
            //                  aprv_budgeting_year = (int)q.abcs_budgeting_year,
            //                  ScreenName = "E6"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_KhidCor where p.abcs_Deleted == Deleted group p by new { p.abcs_budgeting_year, p.abcs_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abcs_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abcs_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abcs_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abcs_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abcs_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abcs_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abcs_cost_center_name,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abcs_budgeting_year,
                              aprv_id = q.FirstOrDefault().abcs_id,
                              ScreenName = "E6"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpLevi(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_levi where p.fld_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abel_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abel_budgeting_year)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abel_budgeting_year);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abel_cost_center_code,
            //                  aprv_cost_center_desc = q.abel_cost_center_desc,
            //                  aprv_id = q.abel_id,
            //                  aprv_budgeting_year = (int)q.abel_budgeting_year,
            //                  ScreenName = "E11"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_levi where p.fld_Deleted == Deleted group p by new { p.abel_budgeting_year, p.abel_cost_center_code } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abel_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abel_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abel_cost_center_code)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abel_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abel_cost_center_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abel_cost_center_code,
                              aprv_cost_center_desc = q.FirstOrDefault().abel_cost_center_desc,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abel_budgeting_year,
                              aprv_id = q.FirstOrDefault().abel_id,
                              ScreenName = "E11"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpMedical(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_Medical where p.abem_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abem_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abem_budgeting_year)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abem_budgeting_year);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abem_cost_center,
            //                  aprv_cost_center_desc = q.abem_cost_center_name,
            //                  aprv_id = q.abem_id,
            //                  aprv_budgeting_year = (int)q.abem_budgeting_year,
            //                  ScreenName = "E7"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_Medical where p.abem_Deleted == Deleted group p by new { p.abem_budgeting_year, p.abem_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abem_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abem_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abem_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abem_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abem_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abem_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abem_cost_center_name,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abem_budgeting_year,
                              aprv_id = q.FirstOrDefault().abem_id,
                              ScreenName = "E7"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpOthers(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var query = from p in dbe.bgt_expenses_others where p.abeo_deleted == Deleted select p;

            if (BudgetYear.HasValue)
                query = query.Where(q => q.abeo_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abeo_budgeting_year)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abeo_budgeting_year);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.abeo_cost_center_code,
                              aprv_cost_center_desc = q.abeo_cost_center_desc,
                              aprv_id = q.abeo_id,
                              aprv_budgeting_year = q.abeo_budgeting_year,
                              ScreenName = "E15"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpPau(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var query = from p in dbe.bgt_expenses_paus where p.abep_deleted == Deleted select p;

            if (BudgetYear.HasValue)
                query = query.Where(q => q.abep_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abep_budgeting_year)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abep_budgeting_year);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.abep_cost_center_code,
                              aprv_cost_center_desc = q.abep_cost_center_desc,
                              aprv_id = q.abep_id,
                              aprv_budgeting_year = q.abep_budgeting_year,
                              ScreenName = "E10"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpSalary(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            //var query = from p in dbe.bgt_expenses_Salary where p.abes_Deleted == Deleted select p;

            //if (BudgetYear.HasValue)
            //    query = query.Where(q => q.abes_budgeting_year == BudgetYear);

            //var query2 = query.AsQueryable();

            //if (Page.HasValue && PageSize.HasValue)
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abes_budgeting_year)
            //        .Skip((Page.Value - 1) * PageSize.Value)
            //        .Take(PageSize.Value);
            //}
            //else
            //{
            //    query2 = query2
            //        .OrderByDescending(q => q.abes_budgeting_year);
            //}

            //var query3 = (from q in query2.AsEnumerable()
            //              select new BudgetApprovalViewModel
            //              {
            //                  aprv_cost_center_code = q.abes_cost_center,
            //                  aprv_cost_center_desc = q.abes_cost_center_name,
            //                  aprv_id = q.abes_id,
            //                  aprv_budgeting_year = (int)q.abes_budgeting_year,
            //                  ScreenName = "E12"
            //              }).Distinct();

            var query = from p in dbe.bgt_expenses_Salary where p.abes_Deleted == Deleted group p by new { p.abes_budgeting_year, p.abes_cost_center } into g select g;

            if (BudgetYear.HasValue)
                query = query.Where(g => g.FirstOrDefault().abes_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abes_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abes_cost_center)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(g => g.FirstOrDefault().abes_budgeting_year)
                    .ThenBy(g => g.FirstOrDefault().abes_cost_center);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.FirstOrDefault().abes_cost_center,
                              aprv_cost_center_desc = q.FirstOrDefault().abes_cost_center_name,
                              aprv_budgeting_year = (int)q.FirstOrDefault().abes_budgeting_year,
                              aprv_id = q.FirstOrDefault().abes_id,
                              ScreenName = "E2"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpVehicle(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var query = from p in dbe.bgt_expenses_vehicle where p.abev_deleted == Deleted select p;

            if (BudgetYear.HasValue)
                query = query.Where(q => q.abev_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abev_budgeting_year)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abev_budgeting_year);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.abev_cost_center_code,
                              aprv_cost_center_desc = q.abev_cost_center_desc,
                              aprv_id = q.abev_id,
                              aprv_budgeting_year = q.abev_budgeting_year,
                              ScreenName = "E14"
                          }).Distinct();

            return query3.ToList();
        }
        private List<BudgetApprovalViewModel> GetExpWindfall(int? BudgetYear = null, bool? Deleted = true, int? Page = null, int? PageSize = null)
        {
            var query = from p in dbe.bgt_expenses_Windfall where p.abew_Deleted == Deleted select p;

            if (BudgetYear.HasValue)
                query = query.Where(q => q.abew_budgeting_year == BudgetYear);

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abew_budgeting_year)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abew_budgeting_year);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new BudgetApprovalViewModel
                          {
                              aprv_cost_center_code = q.abew_cost_center,
                              aprv_cost_center_desc = q.abew_cost_center_name,
                              aprv_id = q.abew_id,
                              aprv_budgeting_year = (int)q.abew_budgeting_year,
                              ScreenName = "E8"
                          }).Distinct();

            return query3.ToList();
        }

        //temp comment
        //public ActionResult UpdateApprovalLog(int year, string cc = null, int? status = null, int? user = null, string note = null)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var costcenter = cc.Split(',');
        //            foreach (var ccval in costcenter)
        //            {
        //                if (!string.IsNullOrEmpty(ccval))
        //                {
        //                    var model = new bgt_approval_log()
        //                    {
        //                        aprvlog_budgeting_year = year,
        //                        aprvlog_cost_center_code = ccval.ToString(),
        //                        aprvlog_status = bgtstatus.GetBudgetApprovalStatusDesc(status),
        //                        aprvlog_note = note,
        //                        aprvlog_prepared_by = user,
        //                        last_modified = DateTime.Now
        //                    };
        //                    dbe.bgt_approval_log.Add(model);
        //                }
        //            }
        //            dbe.SaveChanges();

        //            return Json(new
        //            {
        //                success = true
        //            });
        //        }
        //        catch (Exception exc)
        //        {
        //            errlog.catcherro(exc.Message, exc.StackTrace, exc.Source, exc.TargetSite.ToString());
        //            return Json(new
        //            {
        //                success = false,
        //                msg = GlobalResEstate.msgError,
        //                status = "danger",
        //            });
        //        }
        //    }
        //    else
        //    {
        //        dbe.Dispose();
        //        return Json( new { success = false } );
        //    }
        //}

        //public ActionResult UpdateBudgetStatus(int? BudgetYear = null, string CostCenter = null, bool? Deleted = true, string kes = null, string kes2 = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var NewCostCenter = CostCenter.Split(',');
        //            foreach (var val in NewCostCenter)
        //            {
        //                switch (kes)
        //                {
        //                    case "sawit":
        //                        var incomesawit = GetIncomeSawitRecords(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        foreach (var val2 in incomesawit)
        //                        {
        //                            if (incomesawit.Count > 0)
        //                            {
        //                                var incomesawitdata = dbe.bgt_income_sawit
        //                                    .Where(w => w.abio_id == val2.aprv_id && w.abio_cost_center == val.ToString() && w.abio_budgeting_year == BudgetYear && w.fld_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (incomesawitdata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            incomesawitdata.abio_status = "DRAFT";
        //                                            dbe.Entry(incomesawitdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            incomesawitdata.abio_status = "CHECK";
        //                                            dbe.Entry(incomesawitdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "bijibenih":
        //                        var bijibenih = GetIncomeBijiBenihRecords(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (bijibenih.Count > 0)
        //                        {
        //                            foreach (var val2 in bijibenih)
        //                            {
        //                                var bijibenihdata = dbe.bgt_income_bijibenih
        //                                    .Where(w => w.abis_id == val2.aprv_id && w.abis_cost_center == val.ToString() && w.abis_budgeting_year == BudgetYear && w.fld_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (bijibenihdata != null)
        //                                {
        //                                    //bijibenihdata.abis_status = "CHECK";
        //                                    //dbe.Entry(bijibenihdata).State = EntityState.Modified;
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            bijibenihdata.abis_status = "DRAFT";
        //                                            dbe.Entry(bijibenihdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            bijibenihdata.abis_status = "CHECK";
        //                                            dbe.Entry(bijibenihdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "product":
        //                        var product = GetIncomeProductRecords(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (product.Count > 0)
        //                        {
        //                            foreach (var val2 in product)
        //                            {
        //                                var productdata = dbe.bgt_income_product
        //                                    .Where(w => w.abip_id == val2.aprv_id && w.abip_cost_center == val.ToString() && w.abip_budgeting_year == BudgetYear && w.fld_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (productdata != null)
        //                                {
        //                                    //productdata.abip_status = "CHECK";
        //                                    //dbe.Entry(productdata).State = EntityState.Modified;
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            productdata.abip_status = "DRAFT";
        //                                            dbe.Entry(productdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            productdata.abip_status = "CHECK";
        //                                            dbe.Entry(productdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "levi":
        //                        var levi = GetExpLevi(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (levi.Count > 0)
        //                        {
        //                            foreach (var val2 in levi)
        //                            {
        //                                var levidata = dbe.bgt_expenses_levi
        //                                    .Where(w => w.abel_id == val2.aprv_id && w.abel_cost_center_code == val.ToString() && w.abel_budgeting_year == BudgetYear && w.fld_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (levidata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            levidata.abel_status = "DRAFT";
        //                                            dbe.Entry(levidata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            levidata.abel_status = "CHECK";
        //                                            dbe.Entry(levidata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expcapital":
        //                        //var expcapital = GetExpCapital(BudgetYear, false)
        //                        //    .Where(w => w.aprv_cost_center_code
        //                        //    .Contains(val.ToString()) && w.aprv_deleted == false)
        //                        //    .ToList();
        //                        //if (expcapital.Count > 0)
        //                        //{
        //                        //    foreach (var val2 in expcapital)
        //                        //    {
        //                                var expcapitaldata = dbe.bgt_expenses_capitals
        //                                    .Where(w => w.abce_cost_center_code == val.ToString() && w.abce_budgeting_year == BudgetYear && w.abce_deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expcapitaldata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expcapitaldata.abce_status = "DRAFT";
        //                                            dbe.Entry(expcapitaldata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expcapitaldata.abce_status = "CHECK";
        //                                            dbe.Entry(expcapitaldata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            //}
        //                        //}
        //                        break;

        //                    case "expctrlhq":
        //                        var expctrlhq = GetExpCtrlHQRecords(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expctrlhq.Count > 0)
        //                        {
        //                            foreach (var val2 in expctrlhq)
        //                            {
        //                                var expctrlhqdata = dbe.bgt_expenses_CtrlHQ
        //                                    .Where(w => w.abhq_id == val2.aprv_id && w.abhq_cost_center == val.ToString() && w.abhq_budgeting_year == BudgetYear && w.abhq_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expctrlhqdata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expctrlhqdata.abhq_status = "DRAFT";
        //                                            dbe.Entry(expctrlhqdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expctrlhqdata.abhq_status = "CHECK";
        //                                            dbe.Entry(expctrlhqdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expdepreciation":
        //                        var expdepreciation = GetExpDepreciation(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expdepreciation.Count > 0)
        //                        {
        //                            foreach (var val2 in expdepreciation)
        //                            {
        //                                var expdepreciationdata = dbe.bgt_expenses_Depreciation
        //                                    .Where(w => w.abed_id == val2.aprv_id && w.abed_cost_center == val.ToString() && w.abed_budgeting_year == BudgetYear && w.abed_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expdepreciationdata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expdepreciationdata.abed_status = "DRAFT";
        //                                            dbe.Entry(expdepreciationdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expdepreciationdata.abed_status = "CHECK";
        //                                            dbe.Entry(expdepreciationdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expgaji":
        //                        //var expgaji = GetExpGaji(BudgetYear, false)
        //                        //    .Where(w => w.aprv_cost_center_code
        //                        //    .Contains(val.ToString()) && w.aprv_deleted == false)
        //                        //    .ToList();
        //                        //if (expgaji.Count > 0)
        //                        //{
        //                            //foreach (var val2 in expgaji)
        //                            //{
        //                                var expgajidata = dbe.bgt_expenses_gajis
        //                                    .Where(w => w.abeg_cost_center_code == val.ToString() && w.abeg_budgeting_year == BudgetYear && w.abeg_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expgajidata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expgajidata.abeg_status = "DRAFT";
        //                                            dbe.Entry(expgajidata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expgajidata.abeg_status = "CHECK";
        //                                            dbe.Entry(expgajidata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            //}
        //                        //}
        //                        break;

        //                    case "expinsurance":
        //                        var expinsurance = GetExpInsuranceRecords(BudgetYear, false)
        //                            .Where(w =>  w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expinsurance.Count > 0)
        //                        {
        //                            foreach (var val2 in expinsurance)
        //                            {
        //                                var expinsurancedata = dbe.bgt_expenses_Insurans
        //                                    .Where(w => w.abei_id == val2.aprv_id && w.abei_cost_center == val.ToString() && w.abei_budgeting_year == BudgetYear && w.abei_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expinsurancedata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expinsurancedata.abei_status = "DRAFT";
        //                                            dbe.Entry(expinsurancedata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expinsurancedata.abei_status = "CHECK";
        //                                            dbe.Entry(expinsurancedata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expit":
        //                        var expit = GetExpIT(BudgetYear, false)
        //                            .Where(w =>  w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expit.Count > 0)
        //                        {
        //                            foreach (var val2 in expit)
        //                            {
        //                                var expitdata = dbe.bgt_expenses_IT
        //                                    .Where(w => w.abet_id == val2.aprv_id && w.abet_cost_center == val.ToString() && w.abet_budgeting_year == BudgetYear && w.abet_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expitdata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expitdata.abet_status = "DRAFT";
        //                                            dbe.Entry(expitdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expitdata.abet_status = "CHECK";
        //                                            dbe.Entry(expitdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expkhidcorp":
        //                        var expkhidcorp = GetExpKhidCorp(BudgetYear, false)
        //                            .Where(w =>  w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expkhidcorp.Count > 0)
        //                        {
        //                            foreach (var val2 in expkhidcorp)
        //                            {
        //                                var expkhidcorpdata = dbe.bgt_expenses_KhidCor
        //                                    .Where(w => w.abcs_id == val2.aprv_id && w.abcs_cost_center == val.ToString() && w.abcs_budgeting_year == BudgetYear && w.abcs_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expkhidcorpdata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expkhidcorpdata.abcs_status = "DRAFT";
        //                                            dbe.Entry(expkhidcorpdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expkhidcorpdata.abcs_status = "CHECK";
        //                                            dbe.Entry(expkhidcorpdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expmedical":
        //                        var expmedical = GetExpMedical(BudgetYear, false)
        //                            .Where(w =>  w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expmedical.Count > 0)
        //                        {
        //                            foreach (var val2 in expmedical)
        //                            {
        //                                var expmedicaldata = dbe.bgt_expenses_Medical
        //                                    .Where(w => w.abem_id == val2.aprv_id && w.abem_cost_center == val.ToString() && w.abem_budgeting_year == BudgetYear && w.abem_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expmedicaldata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expmedicaldata.abem_status = "DRAFT";
        //                                            dbe.Entry(expmedicaldata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expmedicaldata.abem_status = "CHECK";
        //                                            dbe.Entry(expmedicaldata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expothers":
        //                        var expothers = GetExpOthers(BudgetYear, false)
        //                            .Where(w =>  w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expothers.Count > 0)
        //                        {
        //                            foreach (var val2 in expothers)
        //                            {
        //                                var expothersdata = dbe.bgt_expenses_others
        //                                    .Where(w => w.abeo_id == val2.aprv_id && w.abeo_cost_center_code == val.ToString() && w.abeo_budgeting_year == BudgetYear && w.abeo_deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expothersdata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expothersdata.abeo_status = "DRAFT";
        //                                            dbe.Entry(expothersdata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expothersdata.abeo_status = "CHECK";
        //                                            dbe.Entry(expothersdata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "exppau":
        //                        var exppau = GetExpPau(BudgetYear, false)
        //                            .Where(w =>  w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (exppau.Count > 0)
        //                        {
        //                            foreach (var val2 in exppau)
        //                            {
        //                                var exppaudata = dbe.bgt_expenses_paus
        //                                    .Where(w => w.abep_id == val2.aprv_id && w.abep_cost_center_code == val.ToString() && w.abep_budgeting_year == BudgetYear && w.abep_deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (exppaudata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            exppaudata.abep_status = "DRAFT";
        //                                            dbe.Entry(exppaudata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            exppaudata.abep_status = "CHECK";
        //                                            dbe.Entry(exppaudata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expsalary":
        //                        var expsalary = GetExpSalary(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expsalary.Count > 0)
        //                        {
        //                            foreach (var val2 in expsalary)
        //                            {
        //                                var expsalarydata = dbe.bgt_expenses_Salary
        //                                    .Where(w => w.abes_id == val2.aprv_id && w.abes_cost_center == val.ToString() && w.abes_budgeting_year == BudgetYear && w.abes_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expsalarydata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expsalarydata.abes_status = "DRAFT";
        //                                            dbe.Entry(expsalarydata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expsalarydata.abes_status = "CHECK";
        //                                            dbe.Entry(expsalarydata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expvehicle":
        //                        var expvehicle = GetExpVehicle(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expvehicle.Count > 0)
        //                        {
        //                            foreach (var val2 in expvehicle)
        //                            {
        //                                var expvehicledata = dbe.bgt_expenses_vehicle
        //                                    .Where(w => w.abev_id == val2.aprv_id && w.abev_cost_center_code == val.ToString() && w.abev_budgeting_year == BudgetYear && w.abev_deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expvehicledata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expvehicledata.abev_status = "DRAFT";
        //                                            dbe.Entry(expvehicledata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expvehicledata.abev_status = "CHECK";
        //                                            dbe.Entry(expvehicledata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                    case "expwindfall":
        //                        var expwindfall = GetExpWindfall(BudgetYear, false)
        //                            .Where(w => w.aprv_cost_center_code
        //                            .Contains(val.ToString()))
        //                            .ToList();
        //                        if (expwindfall.Count > 0)
        //                        {
        //                            foreach (var val2 in expwindfall)
        //                            {
        //                                var expwindfalldata = dbe.bgt_expenses_Windfall
        //                                    .Where(w => w.abew_id == val2.aprv_id && w.abew_cost_center == val.ToString() && w.abew_budgeting_year == BudgetYear && w.abew_Deleted == Deleted)
        //                                    .FirstOrDefault();
        //                                if (expwindfalldata != null)
        //                                {
        //                                    switch (kes2)
        //                                    {
        //                                        case "edit":
        //                                            expwindfalldata.abew_status = "DRAFT";
        //                                            dbe.Entry(expwindfalldata).State = EntityState.Modified;
        //                                            break;
        //                                        case "add":
        //                                            expwindfalldata.abew_status = "CHECK";
        //                                            dbe.Entry(expwindfalldata).State = EntityState.Modified;
        //                                            break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;

        //                        //default:
        //                        //    var defaultincomesawit = dbe.bgt_income_sawit
        //                        //        .Where(w => w.abio_budgeting_year == BudgetYear && w.abio_cost_center
        //                        //        .Contains(val.ToString()) && w.fld_Deleted == false)
        //                        //        .ToList();
        //                        //    foreach (var val2 in defaultincomesawit)
        //                        //    {
        //                        //        var defaultincomesawitdata = dbe.bgt_income_sawit
        //                        //            .Where(w => w.abio_id == val2.abio_id && w.abio_cost_center == val.ToString() && w.abio_budgeting_year == BudgetYear && w.fld_Deleted == Deleted)
        //                        //            .FirstOrDefault();
        //                        //        defaultincomesawitdata.abio_status = "DRAF";
        //                        //        dbe.Entry(defaultincomesawitdata).State = EntityState.Modified;
        //                        //    }
        //                        //    break;
        //                }
        //            }

        //            return Json(new
        //            {
        //                success = true
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new
        //            {
        //                success = false
        //            });
        //        }
        //    }
        //    else
        //    {
        //        dbe.Dispose();
        //        return Json(new { success = false });
        //    }
        //}
    }
}
