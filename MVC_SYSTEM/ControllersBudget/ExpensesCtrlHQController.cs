using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.App_LocalResources;
using System.IO;
using MVC_SYSTEM.Attributes;
using System.Text.RegularExpressions;
using static MVC_SYSTEM.Class.GlobalFunction;
using System.Globalization;
using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ModelsBudgetView;
using MVC_SYSTEM.ClassBudget;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Web.Security;
using Rotativa;
using System.Threading.Tasks;
using System.Data;
using System.Web.UI;
using PagedList;
using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ExpensesCtrlHQController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_ModelsBudgetEst dbEst = new ConnectionBudget().GetConnection();
        private MVC_SYSTEM_ModelsBudgetView dbBview = new MVC_SYSTEM_ModelsBudgetView();

        private MVC_SYSTEM_MasterModels dbM = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();

        errorlog geterror = new errorlog();
        private GetConfig config = new GetConfig();
        private Screen scr = new Screen();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private errorlog errlog = new errorlog();
        GetIdentity GetIdentity = new GetIdentity();

        Connection Connection = new Connection();

        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        GetBudgetClass GetBudgetClass = new GetBudgetClass();
        private readonly string screenCode = "E5";

        // GET: ExpenChqreciation
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ExpChqListing()
        {

            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString()
            }), "Value", "Text").ToList();
            var selectedYear = GetYears().FirstOrDefault().ToString();

            ViewBag.Years = years;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;


            return View();
        }

        public ActionResult Records(string BudgetYear, string CostCenter, int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<bgt_expenses_CtrlHQView>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        private List<bgt_expenses_CtrlHQView> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_CtrlHQ
                        where o.abhq_SyarikatID == syarikatId && o.abhq_LadangID == ladangId && o.abhq_WilayahID == wilayahId && o.abhq_Deleted == false
                        group o by new { o.abhq_budgeting_year, o.abhq_cost_center, o.abhq_cost_center_name } into g
                        select new
                        {
                            BudgetYear = g.Key.abhq_budgeting_year,
                            CostCenterCode = g.Key.abhq_cost_center,
                            CostCenterDesc = g.Key.abhq_cost_center_name,
                            Total = g.Sum(o => o.abhq_jumlah_keseluruhan)
                        };

            if (BudgetYear.HasValue)
                query = query.Where(q => q.BudgetYear == BudgetYear);
            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.CostCenterCode.Contains(CostCenter) || q.CostCenterDesc.Contains(CostCenter));

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.BudgetYear)
                    .ThenBy(q => q.CostCenterCode)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.BudgetYear)
                    .ThenBy(q => q.CostCenterCode);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new bgt_expenses_CtrlHQView
                          {
                              abhq_budgeting_year = q.BudgetYear,
                              abhq_cost_center = q.CostCenterCode,
                              abhq_cost_center_name = q.CostCenterDesc,

                              abhq_jumlah_keseluruhan = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult ExpChqCreateDtail()
        {
            try
            {
                ViewBag.IncomeBudget = "class = active";
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                var yearlist = DateTime.Now.Year + 1;
                ViewBag.abhq_budgeting_year = yearlist;

                var IncludeCC = dbEst.bgt_expenses_CtrlHQ
                    .Where(w => w.abhq_budgeting_year == yearlist && w.abhq_LadangID == LadangID && w.abhq_WilayahID == WilayahID && w.abhq_SyarikatID == SyarikatID && w.abhq_NegaraID == NegaraID && w.abhq_Deleted == false)
                    .Select(s => s.abhq_cost_center)
                    .ToList();

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abhq_cost_center = CostCenters;

                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpChqCreateDtail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpChqCreateDtail(bgt_expenses_CtrlHQ[] ExpenChq, int abhq_budgeting_year, string abhq_cost_center)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            string hdrmsg = "";
            string msg = "";
            string status = "";
            bool leavestatus = true;

            if (ModelState.IsValid)
            {
                try
                {
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abhq_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenChq)
                    {
                        var checkdata = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_budgeting_year == abhq_budgeting_year && x.abhq_cost_center == abhq_cost_center && x.abhq_Deleted == false).ToList();
                        if (checkdata == null || checkdata.Count() == 0)
                        {
                            bgt_expenses_CtrlHQ bis = new bgt_expenses_CtrlHQ()
                            {

                                abhq_budgeting_year = abhq_budgeting_year,
                                abhq_cost_center = abhq_cost_center,
                                abhq_cost_center_name = cc_name,
                                abhq_gl_code = val.abhq_gl_code,
                                abhq_gl_desc = val.abhq_gl_desc,
                                abhq_uom = val.abhq_uom,
                                abhq_proration = val.abhq_proration,
                                abhq_jumlah_setahun = val.abhq_jumlah_setahun.HasValue ? val.abhq_jumlah_setahun.Value : 0m,
                                abhq_month_1 = val.abhq_month_1.HasValue ? val.abhq_month_1.Value : 0m,
                                abhq_month_2 = val.abhq_month_2.HasValue ? val.abhq_month_2.Value : 0m,
                                abhq_month_3 = val.abhq_month_3.HasValue ? val.abhq_month_3.Value : 0m,
                                abhq_month_4 = val.abhq_month_4.HasValue ? val.abhq_month_4.Value : 0m,
                                abhq_month_5 = val.abhq_month_5.HasValue ? val.abhq_month_5.Value : 0m,
                                abhq_month_6 = val.abhq_month_6.HasValue ? val.abhq_month_6.Value : 0m,
                                abhq_month_7 = val.abhq_month_7.HasValue ? val.abhq_month_7.Value : 0m,
                                abhq_month_8 = val.abhq_month_8.HasValue ? val.abhq_month_8.Value : 0m,
                                abhq_month_9 = val.abhq_month_9.HasValue ? val.abhq_month_9.Value : 0m,
                                abhq_month_10 = val.abhq_month_10.HasValue ? val.abhq_month_10.Value : 0m,
                                abhq_month_11 = val.abhq_month_11.HasValue ? val.abhq_month_11.Value : 0m,
                                abhq_month_12 = val.abhq_month_12.HasValue ? val.abhq_month_12.Value : 0m,
                                abhq_jumlah_RM = val.abhq_jumlah_RM.HasValue ? val.abhq_jumlah_RM.Value : 0m,
                                abhq_jumlah_keseluruhan = val.abhq_jumlah_keseluruhan.HasValue ? val.abhq_jumlah_keseluruhan.Value : 0m,
                                last_modified = DateTime.Now,
                                abhq_Deleted = false,
                                abhq_NegaraID = NegaraID,
                                abhq_SyarikatID = SyarikatID,
                                abhq_WilayahID = WilayahID,
                                abhq_LadangID = LadangID,
                                abhq_revision = 0,
                                abhq_history = false,
                                abhq_status = "DRAFT"
                            };
                            dbEst.bgt_expenses_CtrlHQ.Add(bis);
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpChqListing", new { searchString = abhq_cost_center, CostCenter = abhq_cost_center, yearlist = abhq_budgeting_year });

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }

            }
            else
            {
                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                });
            }

            var IncludeCC = dbEst.bgt_expenses_CtrlHQ
                    .Where(w => w.abhq_budgeting_year == abhq_budgeting_year && w.abhq_LadangID == LadangID && w.abhq_WilayahID == WilayahID && w.abhq_SyarikatID == SyarikatID && w.abhq_NegaraID == NegaraID && w.abhq_Deleted == false)
                    .Select(s => s.abhq_cost_center)
                    .FirstOrDefault();

            List<SelectListItem> CostCenters = new List<SelectListItem>();
            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.abhq_cost_center = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExpChqEditDetail(int abhq_budgeting_year, string abhq_cost_center)
        {
            try
            {
                ViewBag.IncomeBudget = "class = active";
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                var yearlist = DateTime.Now.Year + 1;
                ViewBag.abhq_budgeting_year = yearlist;

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abhq_cost_center).ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abhq_cost_center = CostCenters;

                bgt_expenses_CtrlHQ[] bgtExpensesCtrlHQ = dbEst.bgt_expenses_CtrlHQ.OrderBy(o => o.abhq_gl_code).Where(x => x.abhq_cost_center == abhq_cost_center && x.abhq_budgeting_year == abhq_budgeting_year && x.abhq_WilayahID == WilayahID && x.abhq_SyarikatID == SyarikatID && x.abhq_NegaraID == NegaraID && x.abhq_LadangID == LadangID && x.abhq_Deleted == false).ToArray();
                var getdata = dbEst.bgt_expenses_CtrlHQ.OrderBy(o => o.abhq_gl_code).Where(w => w.abhq_cost_center == abhq_cost_center && w.abhq_budgeting_year == abhq_budgeting_year).ToList();
                ViewBag.GetData = getdata;

                return PartialView(bgtExpensesCtrlHQ);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpChqEditDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpChqEditDetail(bgt_expenses_CtrlHQ[] ExpenChq, int abhq_budgeting_year, string abhq_cost_center)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            string hdrmsg = "";
            string msg = "";
            string status = "";
            bool leavestatus = true;

            if (ModelState.IsValid)
            {
                try
                {
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abhq_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenChq)
                    {
                        var getdata = dbEst.bgt_expenses_CtrlHQ.OrderBy(o => o.abhq_gl_code).Where(x => x.abhq_budgeting_year == abhq_budgeting_year && x.abhq_cost_center == abhq_cost_center && x.abhq_gl_code == val.abhq_gl_code && x.abhq_LadangID == LadangID && x.abhq_WilayahID == WilayahID && x.abhq_SyarikatID == SyarikatID && x.abhq_NegaraID == NegaraID && x.abhq_Deleted == false).FirstOrDefault();
                        getdata.abhq_budgeting_year = abhq_budgeting_year;
                        getdata.abhq_cost_center = abhq_cost_center;
                        getdata.abhq_cost_center_name = cc_name;
                        getdata.abhq_gl_code = val.abhq_gl_code;
                        getdata.abhq_gl_desc = val.abhq_gl_desc;
                        getdata.abhq_uom = val.abhq_uom;
                        getdata.abhq_proration = val.abhq_proration;
                        getdata.abhq_jumlah_setahun = val.abhq_jumlah_setahun.HasValue ? val.abhq_jumlah_setahun.Value : 0m;
                        getdata.abhq_month_1 = val.abhq_month_1.HasValue ? val.abhq_month_1.Value : 0m;
                        getdata.abhq_month_2 = val.abhq_month_2.HasValue ? val.abhq_month_2.Value : 0m;
                        getdata.abhq_month_3 = val.abhq_month_3.HasValue ? val.abhq_month_3.Value : 0m;
                        getdata.abhq_month_4 = val.abhq_month_4.HasValue ? val.abhq_month_4.Value : 0m;
                        getdata.abhq_month_5 = val.abhq_month_5.HasValue ? val.abhq_month_5.Value : 0m;
                        getdata.abhq_month_6 = val.abhq_month_6.HasValue ? val.abhq_month_6.Value : 0m;
                        getdata.abhq_month_7 = val.abhq_month_7.HasValue ? val.abhq_month_7.Value : 0m;
                        getdata.abhq_month_8 = val.abhq_month_8.HasValue ? val.abhq_month_8.Value : 0m;
                        getdata.abhq_month_9 = val.abhq_month_9.HasValue ? val.abhq_month_9.Value : 0m;
                        getdata.abhq_month_10 = val.abhq_month_10.HasValue ? val.abhq_month_10.Value : 0m;
                        getdata.abhq_month_11 = val.abhq_month_11.HasValue ? val.abhq_month_11.Value : 0m;
                        getdata.abhq_month_12 = val.abhq_month_12.HasValue ? val.abhq_month_12.Value : 0m;
                        getdata.abhq_jumlah_RM = val.abhq_jumlah_RM.HasValue ? val.abhq_jumlah_RM.Value : 0m;
                        getdata.abhq_jumlah_keseluruhan = val.abhq_jumlah_keseluruhan.HasValue ? val.abhq_jumlah_keseluruhan.Value : 0m;
                        getdata.last_modified = DateTime.Now;
                        getdata.abhq_Deleted = false;
                        getdata.abhq_NegaraID = NegaraID;
                        getdata.abhq_SyarikatID = SyarikatID;
                        getdata.abhq_WilayahID = WilayahID;
                        getdata.abhq_LadangID = LadangID;
                        getdata.abhq_revision = val.abhq_revision;
                        getdata.abhq_status = val.abhq_status;
                        getdata.abhq_history = val.abhq_history;
                        dbEst.Entry(getdata).State = EntityState.Modified;
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpChqListing", new { searchString = abhq_cost_center, CostCenter = abhq_cost_center, yearlist = abhq_budgeting_year });

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }

            }
            else
            {
                ModelState.AddModelError("", "Invalid!");
            }

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abhq_cost_center).ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.CostCenter = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpChqDeleteDetail(int abhq_budgeting_year, string abhq_cost_center)
        {


            var year = int.Parse(abhq_budgeting_year.ToString());
            var ExpChq = await dbEst.bgt_expenses_CtrlHQ.Where(c => c.abhq_budgeting_year == year && c.abhq_cost_center == abhq_cost_center).FirstOrDefaultAsync();
            if (ExpChq == null)
            {
                return HttpNotFound();
            }

            var getdata = dbEst.bgt_expenses_CtrlHQ
                .Where(c => c.abhq_budgeting_year == abhq_budgeting_year && c.abhq_cost_center == abhq_cost_center && c.abhq_proration != null && c.abhq_Deleted == false)
                .ToList();

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_Deleted == false).DistinctBy(x => x.abhq_cost_center).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abhq_cost_center).Select(s => new SelectListItem { Value = s.abhq_cost_center, Text = s.abhq_cost_center + " - " + s.abhq_cost_center_name }), "Value", "Text", abhq_cost_center).ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.abhq_budgeting_year = abhq_budgeting_year;
            ViewBag.abhq_cost_center = CostCenters;

            return PartialView("ExpChqDeleteDetail", getdata);
        }

        [HttpPost, ActionName("ExpChqDeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExpChqDeleteDetailConfirmed(int abhq_budgeting_year, string abhq_cost_center)
        {
            try
            {
                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

                var year = int.Parse(abhq_budgeting_year.ToString());

                var ExpChq = dbEst.bgt_expenses_CtrlHQ
                    .Where(c => c.abhq_budgeting_year == year && c.abhq_cost_center.Equals(abhq_cost_center) && !c.abhq_Deleted)
                    .ToList();

                if (ExpChq.Count() > 0)
                {
                    foreach (var item in ExpChq)
                    {
                        item.abhq_Deleted = true;
                        item.last_modified = DateTime.Now;
                        dbEst.Entry(item).State = EntityState.Modified;
                    }
                    await dbEst.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgDelete2,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesCtrlHQ",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abhq_budgeting_year
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesCtrlHQ",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abhq_budgeting_year
                    });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult ExpChqViewKeseluruhan()
        {
            try
            {
                ViewBag.IncomeBudget = "class = active";
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                List<SelectListItem> SearchStringYearS = new List<SelectListItem>();

                SearchStringYearS.Insert(0, (new SelectListItem { Text = "2023", Value = "2023", Selected = true }));
                SearchStringYearS.Insert(0, (new SelectListItem { Text = "2024", Value = "2024" }));


                ViewBag.SearchStringYear = SearchStringYearS;

                List<SelectListItem> CostCenters = new List<SelectListItem>();


                var CostCenterList = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_Deleted == false).DistinctBy(x => x.abhq_cost_center).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abhq_cost_center).Select(s => new SelectListItem { Value = s.abhq_cost_center, Text = s.abhq_cost_center + " - " + s.abhq_cost_center_name }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "All", Value = "", Selected = true }));
                ViewBag.SearchStringCC = CostCenters;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult PrintList(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChq_List.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";
            //string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - Listing";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChq_ExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpChq(string costcenter, int year)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);


            bgt_expenses_CtrlHQ[] bgt_expenses_CtrlHQ = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_cost_center == costcenter && x.abhq_budgeting_year == year && x.abhq_Deleted == false).ToArray();


            //Get Syarikat
            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            //Get Budget Year
            ViewBag.year = bgt_expenses_CtrlHQ.Select(s => s.abhq_budgeting_year).FirstOrDefault();

            //Get GL Pendapatan
            ViewBag.fld_GLCode = bgt_expenses_CtrlHQ.Select(s => s.abhq_gl_code).FirstOrDefault();
            ViewBag.fld_GLDesc = bgt_expenses_CtrlHQ.Select(s => s.abhq_gl_desc).FirstOrDefault();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

            return PartialView(bgt_expenses_CtrlHQ);
        }

        public ActionResult ViewExpChqPDF(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChq_view.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpChqEXCEL(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChq_Excelview.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult _ExpChqSetahunRpt(bgt_expenses_CtrlHQ[] bgt_expenses_CtrlHQ, string costcenter, string year)
        {
            try
            {
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

                if (costcenter == "0")
                {
                    bgt_expenses_CtrlHQ = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_budgeting_year.ToString() == year && x.abhq_Deleted == false).OrderBy(o => o.abhq_gl_code).ThenBy(x => x.abhq_cost_center).ToArray();

                }
                else
                {
                    bgt_expenses_CtrlHQ = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_cost_center == costcenter && x.abhq_budgeting_year.ToString() == year && x.abhq_Deleted == false).OrderBy(o => o.abhq_gl_code).ToArray();
                }


                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_CtrlHQ.Select(s => s.abhq_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_CtrlHQ.Select(s => s.abhq_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_CtrlHQ.Select(s => s.abhq_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_cost_center == costcenter).Select(o => o.abhq_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_CtrlHQ);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpChqSetahunRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewtahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewtahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult ExpChqSetahunRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewExceltahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewExceltahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult _ExpChqSebulanRpt(bgt_expenses_CtrlHQ[] bgt_expenses_CtrlHQ, string costcenter, int year)
        {
            try
            {
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

                if (costcenter == "0")
                {
                    bgt_expenses_CtrlHQ = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_budgeting_year == year && x.abhq_Deleted == false).OrderBy(o => o.abhq_gl_code).ToArray();

                }
                else
                {
                    bgt_expenses_CtrlHQ = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_cost_center == costcenter && x.abhq_budgeting_year == year && x.abhq_Deleted == false).OrderBy(o => o.abhq_gl_code).ToArray();
                }

                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_CtrlHQ.Select(s => s.abhq_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_CtrlHQ.Select(s => s.abhq_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_CtrlHQ.Select(s => s.abhq_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_CtrlHQ.Where(x => x.abhq_cost_center == costcenter).Select(o => o.abhq_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_CtrlHQ);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpChqSebulanRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }
        public ActionResult ExpChqSebulanRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewExcelbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpChqViewExcelbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_CtrlHQView> data = dbEst.Database.SqlQuery<bgt_expenses_CtrlHQView>("exec sp_ExpChqView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }
    }

}
