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
    public class ExpensesWindfallTaxController : Controller
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
        private readonly string screenCode = "E8";

        // GET: ExpenWindreciation
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ExpWindListing()
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

            var records = new ViewingModels.PagedList<bgt_expenses_WindfallView>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        private List<bgt_expenses_WindfallView> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_Windfall
                        where o.abew_SyarikatID == syarikatId && o.abew_LadangID == ladangId && o.abew_WilayahID == wilayahId && o.abew_Deleted == false
                        group o by new { o.abew_budgeting_year, o.abew_cost_center, o.abew_cost_center_name } into g
                        select new
                        {
                            BudgetYear = g.Key.abew_budgeting_year,
                            CostCenterCode = g.Key.abew_cost_center,
                            CostCenterDesc = g.Key.abew_cost_center_name,
                            Total = g.Sum(o => o.abew_jumlah_keseluruhan)
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
                          select new bgt_expenses_WindfallView
                          {
                              abew_budgeting_year = q.BudgetYear,
                              abew_cost_center = q.CostCenterCode,
                              abew_cost_center_name = q.CostCenterDesc,

                              abew_jumlah_keseluruhan = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult ExpWindCreateDtail()
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
                ViewBag.abew_budgeting_year = yearlist;

                var IncludeCC = dbEst.bgt_expenses_Windfall
                    .Where(w => w.abew_budgeting_year == yearlist && w.abew_LadangID == LadangID && w.abew_WilayahID == WilayahID && w.abew_SyarikatID == SyarikatID && w.abew_NegaraID == NegaraID && w.abew_Deleted == false)
                    .Select(s => s.abew_cost_center)
                    .ToList();

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abew_cost_center = CostCenters;

                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpWindCreateDtail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpWindCreateDtail(bgt_expenses_Windfall[] ExpenWind, int abew_budgeting_year, string abew_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abew_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenWind)
                    {
                        var checkdata = dbEst.bgt_expenses_Windfall.Where(x => x.abew_budgeting_year == abew_budgeting_year && x.abew_cost_center == abew_cost_center && x.abew_Deleted == false).ToList();
                        if (checkdata == null || checkdata.Count() == 0)
                        {
                            bgt_expenses_Windfall bis = new bgt_expenses_Windfall()
                            {

                                abew_budgeting_year = abew_budgeting_year,
                                abew_cost_center = abew_cost_center,
                                abew_cost_center_name = cc_name,
                                abew_gl_code = val.abew_gl_code,
                                abew_gl_desc = val.abew_gl_desc,
                                abew_uom = val.abew_uom,
                                abew_proration = val.abew_proration,
                                abew_jumlah_setahun = val.abew_jumlah_setahun.HasValue ? val.abew_jumlah_setahun.Value : 0m,
                                abew_month_1 = val.abew_month_1.HasValue ? val.abew_month_1.Value : 0m,
                                abew_month_2 = val.abew_month_2.HasValue ? val.abew_month_2.Value : 0m,
                                abew_month_3 = val.abew_month_3.HasValue ? val.abew_month_3.Value : 0m,
                                abew_month_4 = val.abew_month_4.HasValue ? val.abew_month_4.Value : 0m,
                                abew_month_5 = val.abew_month_5.HasValue ? val.abew_month_5.Value : 0m,
                                abew_month_6 = val.abew_month_6.HasValue ? val.abew_month_6.Value : 0m,
                                abew_month_7 = val.abew_month_7.HasValue ? val.abew_month_7.Value : 0m,
                                abew_month_8 = val.abew_month_8.HasValue ? val.abew_month_8.Value : 0m,
                                abew_month_9 = val.abew_month_9.HasValue ? val.abew_month_9.Value : 0m,
                                abew_month_10 = val.abew_month_10.HasValue ? val.abew_month_10.Value : 0m,
                                abew_month_11 = val.abew_month_11.HasValue ? val.abew_month_11.Value : 0m,
                                abew_month_12 = val.abew_month_12.HasValue ? val.abew_month_12.Value : 0m,
                                abew_jumlah_RM = val.abew_jumlah_RM.HasValue ? val.abew_jumlah_RM.Value : 0m,
                                abew_jumlah_keseluruhan = val.abew_jumlah_keseluruhan.HasValue ? val.abew_jumlah_keseluruhan.Value : 0m,
                                last_modified = DateTime.Now,
                                abew_Deleted = false,
                                abew_NegaraID = NegaraID,
                                abew_SyarikatID = SyarikatID,
                                abew_WilayahID = WilayahID,
                                abew_LadangID = LadangID,
                                abew_revision = 0,
                                abew_history = false,
                                abew_status = "DRAFT"
                            };
                            dbEst.bgt_expenses_Windfall.Add(bis);
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpWindListing", new { searchString = abew_cost_center, CostCenter = abew_cost_center, yearlist = abew_budgeting_year });

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

            var IncludeCC = dbEst.bgt_expenses_Windfall
                    .Where(w => w.abew_budgeting_year == abew_budgeting_year && w.abew_LadangID == LadangID && w.abew_WilayahID == WilayahID && w.abew_SyarikatID == SyarikatID && w.abew_NegaraID == NegaraID && w.abew_Deleted == false)
                    .Select(s => s.abew_cost_center)
                    .FirstOrDefault();

            List<SelectListItem> CostCenters = new List<SelectListItem>();
            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.abew_cost_center = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExpWindEditDetail(int abew_budgeting_year, string abew_cost_center)
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
                ViewBag.abew_budgeting_year = yearlist;

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abew_cost_center).ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abew_cost_center = CostCenters;

                bgt_expenses_Windfall[] bgtExpensesWindfallTax = dbEst.bgt_expenses_Windfall.OrderBy(o => o.abew_gl_code).Where(x => x.abew_cost_center == abew_cost_center && x.abew_budgeting_year == abew_budgeting_year && x.abew_WilayahID == WilayahID && x.abew_SyarikatID == SyarikatID && x.abew_NegaraID == NegaraID && x.abew_LadangID == LadangID && x.abew_Deleted == false).ToArray();
                var getdata = dbEst.bgt_expenses_Windfall.OrderBy(o => o.abew_gl_code).Where(w => w.abew_cost_center == abew_cost_center && w.abew_budgeting_year == abew_budgeting_year).ToList();
                ViewBag.GetData = getdata;

                return PartialView(bgtExpensesWindfallTax);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpWindEditDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpWindEditDetail(bgt_expenses_Windfall[] ExpenWind, int abew_budgeting_year, string abew_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abew_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenWind)
                    {
                        var getdata = dbEst.bgt_expenses_Windfall.OrderBy(o => o.abew_gl_code).Where(x => x.abew_budgeting_year == abew_budgeting_year && x.abew_cost_center == abew_cost_center && x.abew_gl_code == val.abew_gl_code && x.abew_LadangID == LadangID && x.abew_WilayahID == WilayahID && x.abew_SyarikatID == SyarikatID && x.abew_NegaraID == NegaraID && x.abew_Deleted == false).FirstOrDefault();
                        getdata.abew_budgeting_year = abew_budgeting_year;
                        getdata.abew_cost_center = abew_cost_center;
                        getdata.abew_cost_center_name = cc_name;
                        getdata.abew_gl_code = val.abew_gl_code;
                        getdata.abew_gl_desc = val.abew_gl_desc;
                        getdata.abew_uom = val.abew_uom;
                        getdata.abew_proration = val.abew_proration;
                        getdata.abew_jumlah_setahun = val.abew_jumlah_setahun.HasValue ? val.abew_jumlah_setahun.Value : 0m;
                        getdata.abew_month_1 = val.abew_month_1.HasValue ? val.abew_month_1.Value : 0m;
                        getdata.abew_month_2 = val.abew_month_2.HasValue ? val.abew_month_2.Value : 0m;
                        getdata.abew_month_3 = val.abew_month_3.HasValue ? val.abew_month_3.Value : 0m;
                        getdata.abew_month_4 = val.abew_month_4.HasValue ? val.abew_month_4.Value : 0m;
                        getdata.abew_month_5 = val.abew_month_5.HasValue ? val.abew_month_5.Value : 0m;
                        getdata.abew_month_6 = val.abew_month_6.HasValue ? val.abew_month_6.Value : 0m;
                        getdata.abew_month_7 = val.abew_month_7.HasValue ? val.abew_month_7.Value : 0m;
                        getdata.abew_month_8 = val.abew_month_8.HasValue ? val.abew_month_8.Value : 0m;
                        getdata.abew_month_9 = val.abew_month_9.HasValue ? val.abew_month_9.Value : 0m;
                        getdata.abew_month_10 = val.abew_month_10.HasValue ? val.abew_month_10.Value : 0m;
                        getdata.abew_month_11 = val.abew_month_11.HasValue ? val.abew_month_11.Value : 0m;
                        getdata.abew_month_12 = val.abew_month_12.HasValue ? val.abew_month_12.Value : 0m;
                        getdata.abew_jumlah_RM = val.abew_jumlah_RM.HasValue ? val.abew_jumlah_RM.Value : 0m;
                        getdata.abew_jumlah_keseluruhan = val.abew_jumlah_keseluruhan.HasValue ? val.abew_jumlah_keseluruhan.Value : 0m;
                        getdata.last_modified = DateTime.Now;
                        getdata.abew_Deleted = false;
                        getdata.abew_NegaraID = NegaraID;
                        getdata.abew_SyarikatID = SyarikatID;
                        getdata.abew_WilayahID = WilayahID;
                        getdata.abew_LadangID = LadangID;
                        getdata.abew_revision = val.abew_revision;
                        getdata.abew_status = val.abew_status;
                        getdata.abew_history = val.abew_history;
                        dbEst.Entry(getdata).State = EntityState.Modified;
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpWindListing", new { searchString = abew_cost_center, CostCenter = abew_cost_center, yearlist = abew_budgeting_year });

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
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abew_cost_center).ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.CostCenter = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpWindDeleteDetail(int abew_budgeting_year, string abew_cost_center)
        {


            var year = int.Parse(abew_budgeting_year.ToString());
            var ExpWind = await dbEst.bgt_expenses_Windfall.Where(c => c.abew_budgeting_year == year && c.abew_cost_center == abew_cost_center).FirstOrDefaultAsync();
            if (ExpWind == null)
            {
                return HttpNotFound();
            }

            var getdata = dbEst.bgt_expenses_Windfall
                .Where(c => c.abew_budgeting_year == abew_budgeting_year && c.abew_cost_center == abew_cost_center && c.abew_proration != null && c.abew_Deleted == false)
                .ToList();

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbEst.bgt_expenses_Windfall.Where(x => x.abew_Deleted == false).DistinctBy(x => x.abew_cost_center).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abew_cost_center).Select(s => new SelectListItem { Value = s.abew_cost_center, Text = s.abew_cost_center + " - " + s.abew_cost_center_name }), "Value", "Text", abew_cost_center).ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.abew_budgeting_year = abew_budgeting_year;
            ViewBag.abew_cost_center = CostCenters;

            return PartialView("ExpWindDeleteDetail", getdata);
        }

        [HttpPost, ActionName("ExpWindDeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExpWindDeleteDetailConfirmed(int abew_budgeting_year, string abew_cost_center)
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

                var year = int.Parse(abew_budgeting_year.ToString());

                var ExpWind = dbEst.bgt_expenses_Windfall
                    .Where(c => c.abew_budgeting_year == year && c.abew_cost_center.Equals(abew_cost_center) && !c.abew_Deleted)
                    .ToList();

                if (ExpWind.Count() > 0)
                {
                    foreach (var item in ExpWind)
                    {
                        item.abew_Deleted = true;
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
                        controller = "ExpensesWindfallTax",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abew_budgeting_year
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesWindfallTax",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abew_budgeting_year
                    });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult ExpWindViewKeseluruhan()
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

                List<SelectListItem> SearchStringYearS = new List<SelectListItem>();

                SearchStringYearS.Insert(0, (new SelectListItem { Text = "2023", Value = "2023", Selected = true }));
                SearchStringYearS.Insert(0, (new SelectListItem { Text = "2024", Value = "2024" }));


                ViewBag.SearchStringYear = SearchStringYearS;

                List<SelectListItem> CostCenters = new List<SelectListItem>();


                var CostCenterList = dbEst.bgt_expenses_Windfall.Where(x => x.abew_Deleted == false).DistinctBy(x => x.abew_cost_center).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abew_cost_center).Select(s => new SelectListItem { Value = s.abew_cost_center, Text = s.abew_cost_center + " - " + s.abew_cost_center_name }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "All", Value = "", Selected = true }));
                ViewBag.SearchStringCC = CostCenters;

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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWind_List.rdlc");
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

            List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWind_ExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpWind(string costcenter, int year)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);


            bgt_expenses_Windfall[] bgt_expenses_Windfall = dbEst.bgt_expenses_Windfall.Where(x => x.abew_cost_center == costcenter && x.abew_budgeting_year == year && x.abew_Deleted == false).ToArray();


            //Get Syarikat
            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            //Get Budget Year
            ViewBag.year = bgt_expenses_Windfall.Select(s => s.abew_budgeting_year).FirstOrDefault();

            //Get GL Pendapatan
            ViewBag.fld_GLCode = bgt_expenses_Windfall.Select(s => s.abew_gl_code).FirstOrDefault();
            ViewBag.fld_GLDesc = bgt_expenses_Windfall.Select(s => s.abew_gl_desc).FirstOrDefault();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

            return PartialView(bgt_expenses_Windfall);
        }

        public ActionResult ViewExpWindPDF(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWind_view.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpWindEXCEL(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWind_Excelview.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult _ExpWindSetahunRpt(bgt_expenses_Windfall[] bgt_expenses_Windfall, string costcenter, string year)
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
                    bgt_expenses_Windfall = dbEst.bgt_expenses_Windfall.Where(x => x.abew_budgeting_year.ToString() == year && x.abew_Deleted == false).OrderBy(o => o.abew_cost_center).ToArray();

                }
                else
                {
                    bgt_expenses_Windfall = dbEst.bgt_expenses_Windfall.Where(x => x.abew_cost_center == costcenter && x.abew_budgeting_year.ToString() == year && x.abew_Deleted == false).OrderBy(o => o.abew_cost_center).ToArray();
                }


                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Windfall.Select(s => s.abew_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Windfall.Select(s => s.abew_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Windfall.Select(s => s.abew_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Windfall.Where(x => x.abew_cost_center == costcenter).Select(o => o.abew_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

                return PartialView(bgt_expenses_Windfall);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpWindSetahunRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewtahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewtahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult ExpWindSetahunRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewExceltahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewExceltahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult _ExpWindSebulanRpt(bgt_expenses_Windfall[] bgt_expenses_Windfall, string costcenter, int year)
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
                    bgt_expenses_Windfall = dbEst.bgt_expenses_Windfall.Where(x => x.abew_budgeting_year == year && x.abew_Deleted == false).OrderBy(o => o.abew_cost_center).ToArray();

                }
                else
                {
                    bgt_expenses_Windfall = dbEst.bgt_expenses_Windfall.Where(x => x.abew_cost_center == costcenter && x.abew_budgeting_year == year && x.abew_Deleted == false).OrderBy(o => o.abew_cost_center).ToArray();
                }

                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Windfall.Select(s => s.abew_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Windfall.Select(s => s.abew_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Windfall.Select(s => s.abew_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Windfall.Where(x => x.abew_cost_center == costcenter).Select(o => o.abew_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

                return PartialView(bgt_expenses_Windfall);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpWindSebulanRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }
        public ActionResult ExpWindSebulanRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewExcelbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpWindViewExcelbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_WindfallView> data = dbEst.Database.SqlQuery<bgt_expenses_WindfallView>("exec sp_ExpWindView {0}, {1}", year, costcenter).ToList();
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
