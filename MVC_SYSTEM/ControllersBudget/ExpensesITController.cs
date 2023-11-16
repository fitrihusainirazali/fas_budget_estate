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
    public class ExpensesITController : Controller
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
        private readonly string screenCode = "E4";

        // GET: ExpenITreciation
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ExpITListing()
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

            var records = new ViewingModels.PagedList<bgt_expenses_ITView>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        private List<bgt_expenses_ITView> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_IT
                        where o.abet_SyarikatID == syarikatId && o.abet_LadangID == ladangId && o.abet_WilayahID == wilayahId && o.abet_Deleted == false
                        group o by new { o.abet_budgeting_year, o.abet_cost_center, o.abet_cost_center_name } into g
                        select new
                        {
                            BudgetYear = g.Key.abet_budgeting_year,
                            CostCenterCode = g.Key.abet_cost_center,
                            CostCenterDesc = g.Key.abet_cost_center_name,
                            Total = g.Sum(o => o.abet_jumlah_keseluruhan)
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
                          select new bgt_expenses_ITView
                          {
                              abet_budgeting_year = q.BudgetYear,
                              abet_cost_center = q.CostCenterCode,
                              abet_cost_center_name = q.CostCenterDesc,

                              abet_jumlah_keseluruhan = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult ExpITCreateDtail()
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
                ViewBag.abet_budgeting_year = yearlist;

                var IncludeCC = dbEst.bgt_expenses_IT
                    .Where(w => w.abet_budgeting_year == yearlist && w.abet_LadangID == LadangID && w.abet_WilayahID == WilayahID && w.abet_SyarikatID == SyarikatID && w.abet_NegaraID == NegaraID && w.abet_Deleted == false)
                    .Select(s => s.abet_cost_center)
                    .ToList();

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abet_cost_center = CostCenters;

                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpITCreateDtail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpITCreateDtail(bgt_expenses_IT[] ExpenIT, int abet_budgeting_year, string abet_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abet_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenIT)
                    {
                        var checkdata = dbEst.bgt_expenses_IT.Where(x => x.abet_budgeting_year == abet_budgeting_year && x.abet_cost_center == abet_cost_center && x.abet_Deleted == false).ToList();
                        if (checkdata == null || checkdata.Count() == 0)
                        {
                            bgt_expenses_IT bis = new bgt_expenses_IT()
                            {

                                abet_budgeting_year = abet_budgeting_year,
                                abet_cost_center = abet_cost_center,
                                abet_cost_center_name = cc_name,
                                abet_gl_code = val.abet_gl_code,
                                abet_gl_desc = val.abet_gl_desc,
                                abet_uom = val.abet_uom,
                                abet_proration = val.abet_proration,
                                abet_jumlah_setahun = val.abet_jumlah_setahun.HasValue ? val.abet_jumlah_setahun.Value : 0m,
                                abet_month_1 = val.abet_month_1.HasValue ? val.abet_month_1.Value : 0m,
                                abet_month_2 = val.abet_month_2.HasValue ? val.abet_month_2.Value : 0m,
                                abet_month_3 = val.abet_month_3.HasValue ? val.abet_month_3.Value : 0m,
                                abet_month_4 = val.abet_month_4.HasValue ? val.abet_month_4.Value : 0m,
                                abet_month_5 = val.abet_month_5.HasValue ? val.abet_month_5.Value : 0m,
                                abet_month_6 = val.abet_month_6.HasValue ? val.abet_month_6.Value : 0m,
                                abet_month_7 = val.abet_month_7.HasValue ? val.abet_month_7.Value : 0m,
                                abet_month_8 = val.abet_month_8.HasValue ? val.abet_month_8.Value : 0m,
                                abet_month_9 = val.abet_month_9.HasValue ? val.abet_month_9.Value : 0m,
                                abet_month_10 = val.abet_month_10.HasValue ? val.abet_month_10.Value : 0m,
                                abet_month_11 = val.abet_month_11.HasValue ? val.abet_month_11.Value : 0m,
                                abet_month_12 = val.abet_month_12.HasValue ? val.abet_month_12.Value : 0m,
                                abet_jumlah_RM = val.abet_jumlah_RM.HasValue ? val.abet_jumlah_RM.Value : 0m,
                                abet_jumlah_keseluruhan = val.abet_jumlah_keseluruhan.HasValue ? val.abet_jumlah_keseluruhan.Value : 0m,
                                last_modified = DateTime.Now,
                                abet_Deleted = false,
                                abet_NegaraID = NegaraID,
                                abet_SyarikatID = SyarikatID,
                                abet_WilayahID = WilayahID,
                                abet_LadangID = LadangID,
                                abet_revision = 0,
                                abet_history = false,
                                abet_status = "DRAFT"
                            };
                            dbEst.bgt_expenses_IT.Add(bis);
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpITListing", new { searchString = abet_cost_center, CostCenter = abet_cost_center, yearlist = abet_budgeting_year });

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

            var IncludeCC = dbEst.bgt_expenses_IT
                    .Where(w => w.abet_budgeting_year == abet_budgeting_year && w.abet_LadangID == LadangID && w.abet_WilayahID == WilayahID && w.abet_SyarikatID == SyarikatID && w.abet_NegaraID == NegaraID && w.abet_Deleted == false)
                    .Select(s => s.abet_cost_center)
                    .FirstOrDefault();

            List<SelectListItem> CostCenters = new List<SelectListItem>();
            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.abet_cost_center = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExpITEditDetail(int abet_budgeting_year, string abet_cost_center)
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
                ViewBag.abet_budgeting_year = yearlist;

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abet_cost_center).ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abet_cost_center = CostCenters;

                bgt_expenses_IT[] bgtExpensesIT = dbEst.bgt_expenses_IT.OrderBy(o => o.abet_gl_code).Where(x => x.abet_cost_center == abet_cost_center && x.abet_budgeting_year == abet_budgeting_year && x.abet_WilayahID == WilayahID && x.abet_SyarikatID == SyarikatID && x.abet_NegaraID == NegaraID && x.abet_LadangID == LadangID && x.abet_Deleted == false).ToArray();
                var getdata = dbEst.bgt_expenses_IT.OrderBy(o => o.abet_gl_code).Where(w => w.abet_cost_center == abet_cost_center && w.abet_budgeting_year == abet_budgeting_year).ToList();
                ViewBag.GetData = getdata;

                return PartialView(bgtExpensesIT);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpITEditDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpITEditDetail(bgt_expenses_IT[] ExpenIT, int abet_budgeting_year, string abet_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abet_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenIT)
                    {
                        var getdata = dbEst.bgt_expenses_IT.OrderBy(o => o.abet_gl_code).Where(x => x.abet_budgeting_year == abet_budgeting_year && x.abet_cost_center == abet_cost_center && x.abet_gl_code == val.abet_gl_code && x.abet_LadangID == LadangID && x.abet_WilayahID == WilayahID && x.abet_SyarikatID == SyarikatID && x.abet_NegaraID == NegaraID && x.abet_Deleted == false).FirstOrDefault();
                        getdata.abet_budgeting_year = abet_budgeting_year;
                        getdata.abet_cost_center = abet_cost_center;
                        getdata.abet_cost_center_name = cc_name;
                        getdata.abet_gl_code = val.abet_gl_code;
                        getdata.abet_gl_desc = val.abet_gl_desc;
                        getdata.abet_uom = val.abet_uom;
                        getdata.abet_proration = val.abet_proration;
                        getdata.abet_jumlah_setahun = val.abet_jumlah_setahun.HasValue ? val.abet_jumlah_setahun.Value : 0m;
                        getdata.abet_month_1 = val.abet_month_1.HasValue ? val.abet_month_1.Value : 0m;
                        getdata.abet_month_2 = val.abet_month_2.HasValue ? val.abet_month_2.Value : 0m;
                        getdata.abet_month_3 = val.abet_month_3.HasValue ? val.abet_month_3.Value : 0m;
                        getdata.abet_month_4 = val.abet_month_4.HasValue ? val.abet_month_4.Value : 0m;
                        getdata.abet_month_5 = val.abet_month_5.HasValue ? val.abet_month_5.Value : 0m;
                        getdata.abet_month_6 = val.abet_month_6.HasValue ? val.abet_month_6.Value : 0m;
                        getdata.abet_month_7 = val.abet_month_7.HasValue ? val.abet_month_7.Value : 0m;
                        getdata.abet_month_8 = val.abet_month_8.HasValue ? val.abet_month_8.Value : 0m;
                        getdata.abet_month_9 = val.abet_month_9.HasValue ? val.abet_month_9.Value : 0m;
                        getdata.abet_month_10 = val.abet_month_10.HasValue ? val.abet_month_10.Value : 0m;
                        getdata.abet_month_11 = val.abet_month_11.HasValue ? val.abet_month_11.Value : 0m;
                        getdata.abet_month_12 = val.abet_month_12.HasValue ? val.abet_month_12.Value : 0m;
                        getdata.abet_jumlah_RM = val.abet_jumlah_RM.HasValue ? val.abet_jumlah_RM.Value : 0m;
                        getdata.abet_jumlah_keseluruhan = val.abet_jumlah_keseluruhan.HasValue ? val.abet_jumlah_keseluruhan.Value : 0m;
                        getdata.last_modified = DateTime.Now;
                        getdata.abet_Deleted = false;
                        getdata.abet_NegaraID = NegaraID;
                        getdata.abet_SyarikatID = SyarikatID;
                        getdata.abet_WilayahID = WilayahID;
                        getdata.abet_LadangID = LadangID;
                        getdata.abet_revision = val.abet_revision;
                        getdata.abet_status = val.abet_status;
                        getdata.abet_history = val.abet_history;
                        dbEst.Entry(getdata).State = EntityState.Modified;
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpITListing", new { searchString = abet_cost_center, CostCenter = abet_cost_center, yearlist = abet_budgeting_year });

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
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abet_cost_center).ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.CostCenter = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpITDeleteDetail(int abet_budgeting_year, string abet_cost_center)
        {


            var year = int.Parse(abet_budgeting_year.ToString());
            var ExpIT = await dbEst.bgt_expenses_IT.Where(c => c.abet_budgeting_year == year && c.abet_cost_center == abet_cost_center).FirstOrDefaultAsync();
            if (ExpIT == null)
            {
                return HttpNotFound();
            }

            var getdata = dbEst.bgt_expenses_IT
                .Where(c => c.abet_budgeting_year == abet_budgeting_year && c.abet_cost_center == abet_cost_center && c.abet_proration != null && c.abet_Deleted == false)
                .ToList();

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbEst.bgt_expenses_IT.Where(x => x.abet_Deleted == false).DistinctBy(x => x.abet_cost_center).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abet_cost_center).Select(s => new SelectListItem { Value = s.abet_cost_center, Text = s.abet_cost_center + " - " + s.abet_cost_center_name }), "Value", "Text", abet_cost_center).ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.abet_budgeting_year = abet_budgeting_year;
            ViewBag.abet_cost_center = CostCenters;

            return PartialView("ExpITDeleteDetail", getdata);
        }

        [HttpPost, ActionName("ExpITDeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExpITDeleteDetailConfirmed(int abet_budgeting_year, string abet_cost_center)
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

                var year = int.Parse(abet_budgeting_year.ToString());

                var ExpIT = dbEst.bgt_expenses_IT
                    .Where(c => c.abet_budgeting_year == year && c.abet_cost_center.Equals(abet_cost_center) && !c.abet_Deleted)
                    .ToList();

                if (ExpIT.Count() > 0)
                {
                    foreach (var item in ExpIT)
                    {
                        item.abet_Deleted = true;
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
                        controller = "ExpensesIT",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abet_budgeting_year
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesIT",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abet_budgeting_year
                    });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult ExpITViewKeseluruhan()
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


                var CostCenterList = dbEst.bgt_expenses_IT.Where(x => x.abet_Deleted == false).DistinctBy(x => x.abet_cost_center).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abet_cost_center).Select(s => new SelectListItem { Value = s.abet_cost_center, Text = s.abet_cost_center + " - " + s.abet_cost_center_name }), "Value", "Text").ToList();
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpIT_List.rdlc");
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

            List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpIT_ExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpIT(string costcenter, int year)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);


            bgt_expenses_IT[] bgt_expenses_IT = dbEst.bgt_expenses_IT.Where(x => x.abet_cost_center == costcenter && x.abet_budgeting_year == year && x.abet_Deleted == false).ToArray();


            //Get Syarikat
            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            //Get Budget Year
            ViewBag.year = bgt_expenses_IT.Select(s => s.abet_budgeting_year).FirstOrDefault();

            //Get GL Pendapatan
            ViewBag.fld_GLCode = bgt_expenses_IT.Select(s => s.abet_gl_code).FirstOrDefault();
            ViewBag.fld_GLDesc = bgt_expenses_IT.Select(s => s.abet_gl_desc).FirstOrDefault();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

            return PartialView(bgt_expenses_IT);
        }

        public ActionResult ViewExpITPDF(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpIT_view.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpITEXCEL(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpIT_Excelview.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult _ExpITSetahunRpt(bgt_expenses_IT[] bgt_expenses_IT, string costcenter, string year)
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
                    bgt_expenses_IT = dbEst.bgt_expenses_IT.Where(x => x.abet_budgeting_year.ToString() == year && x.abet_Deleted == false).OrderBy(o => o.abet_gl_code).ThenBy(x => x.abet_cost_center).ToArray();

                }
                else
                {
                    bgt_expenses_IT = dbEst.bgt_expenses_IT.Where(x => x.abet_cost_center == costcenter && x.abet_budgeting_year.ToString() == year && x.abet_Deleted == false).OrderBy(o => o.abet_gl_code).ToArray();
                }


                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_IT.Select(s => s.abet_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_IT.Select(s => s.abet_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_IT.Select(s => s.abet_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_IT.Where(x => x.abet_cost_center == costcenter).Select(o => o.abet_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_IT);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpITSetahunRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewtahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewtahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult ExpITSetahunRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewExceltahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewExceltahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult _ExpITSebulanRpt(bgt_expenses_IT[] bgt_expenses_IT, string costcenter, int year)
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
                    bgt_expenses_IT = dbEst.bgt_expenses_IT.Where(x => x.abet_budgeting_year == year && x.abet_Deleted == false).OrderBy(o => o.abet_gl_code).ToArray();

                }
                else
                {
                    bgt_expenses_IT = dbEst.bgt_expenses_IT.Where(x => x.abet_cost_center == costcenter && x.abet_budgeting_year == year && x.abet_Deleted == false).OrderBy(o => o.abet_gl_code).ToArray();
                }

                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_IT.Select(s => s.abet_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_IT.Select(s => s.abet_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_IT.Select(s => s.abet_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_IT.Where(x => x.abet_cost_center == costcenter).Select(o => o.abet_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_IT);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpITSebulanRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }
        public ActionResult ExpITSebulanRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewExcelbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpITViewExcelbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_ITView> data = dbEst.Database.SqlQuery<bgt_expenses_ITView>("exec sp_ExpITView {0}, {1}", year, costcenter).ToList();
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
