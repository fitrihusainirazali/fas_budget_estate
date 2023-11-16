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
    public class ExpensesMedicalController : Controller
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
        private readonly string screenCode = "E7";

        // GET: ExpenMedreciation
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ExpMedListing()
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

            var records = new ViewingModels.PagedList<bgt_expenses_MedicalView>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        private List<bgt_expenses_MedicalView> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_Medical
                        where o.abem_SyarikatID == syarikatId && o.abem_LadangID == ladangId && o.abem_WilayahID == wilayahId && o.abem_Deleted == false
                        group o by new { o.abem_budgeting_year, o.abem_cost_center, o.abem_cost_center_name } into g
                        select new
                        {
                            BudgetYear = g.Key.abem_budgeting_year,
                            CostCenterCode = g.Key.abem_cost_center,
                            CostCenterDesc = g.Key.abem_cost_center_name,
                            Total = g.Sum(o => o.abem_jumlah_keseluruhan)
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
                          select new bgt_expenses_MedicalView
                          {
                              abem_budgeting_year = q.BudgetYear,
                              abem_cost_center = q.CostCenterCode,
                              abem_cost_center_name = q.CostCenterDesc,

                              abem_jumlah_keseluruhan = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult ExpMedCreateDtail()
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
                ViewBag.abem_budgeting_year = yearlist;

                var IncludeCC = dbEst.bgt_expenses_Medical
                    .Where(w => w.abem_budgeting_year == yearlist && w.abem_LadangID == LadangID && w.abem_WilayahID == WilayahID && w.abem_SyarikatID == SyarikatID && w.abem_NegaraID == NegaraID && w.abem_Deleted == false)
                    .Select(s => s.abem_cost_center)
                    .ToList();

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abem_cost_center = CostCenters;

                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpMedCreateDtail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpMedCreateDtail(bgt_expenses_Medical[] ExpenMed, int abem_budgeting_year, string abem_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abem_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenMed)
                    {
                        var checkdata = dbEst.bgt_expenses_Medical.Where(x => x.abem_budgeting_year == abem_budgeting_year && x.abem_cost_center == abem_cost_center && x.abem_Deleted == false).ToList();
                        if (checkdata == null || checkdata.Count() == 0)
                        {
                            bgt_expenses_Medical bis = new bgt_expenses_Medical()
                            {

                                abem_budgeting_year = abem_budgeting_year,
                                abem_cost_center = abem_cost_center,
                                abem_cost_center_name = cc_name,
                                abem_gl_code = val.abem_gl_code,
                                abem_gl_desc = val.abem_gl_desc,
                                abem_uom = val.abem_uom,
                                abem_proration = val.abem_proration,
                                abem_jumlah_setahun = val.abem_jumlah_setahun.HasValue ? val.abem_jumlah_setahun.Value : 0m,
                                abem_month_1 = val.abem_month_1.HasValue ? val.abem_month_1.Value : 0m,
                                abem_month_2 = val.abem_month_2.HasValue ? val.abem_month_2.Value : 0m,
                                abem_month_3 = val.abem_month_3.HasValue ? val.abem_month_3.Value : 0m,
                                abem_month_4 = val.abem_month_4.HasValue ? val.abem_month_4.Value : 0m,
                                abem_month_5 = val.abem_month_5.HasValue ? val.abem_month_5.Value : 0m,
                                abem_month_6 = val.abem_month_6.HasValue ? val.abem_month_6.Value : 0m,
                                abem_month_7 = val.abem_month_7.HasValue ? val.abem_month_7.Value : 0m,
                                abem_month_8 = val.abem_month_8.HasValue ? val.abem_month_8.Value : 0m,
                                abem_month_9 = val.abem_month_9.HasValue ? val.abem_month_9.Value : 0m,
                                abem_month_10 = val.abem_month_10.HasValue ? val.abem_month_10.Value : 0m,
                                abem_month_11 = val.abem_month_11.HasValue ? val.abem_month_11.Value : 0m,
                                abem_month_12 = val.abem_month_12.HasValue ? val.abem_month_12.Value : 0m,
                                abem_jumlah_RM = val.abem_jumlah_RM.HasValue ? val.abem_jumlah_RM.Value : 0m,
                                abem_jumlah_keseluruhan = val.abem_jumlah_keseluruhan.HasValue ? val.abem_jumlah_keseluruhan.Value : 0m,
                                last_modified = DateTime.Now,
                                abem_Deleted = false,
                                abem_NegaraID = NegaraID,
                                abem_SyarikatID = SyarikatID,
                                abem_WilayahID = WilayahID,
                                abem_LadangID = LadangID,
                                abem_revision = 0,
                                abem_history = false,
                                abem_status = "DRAFT"
                            };
                            dbEst.bgt_expenses_Medical.Add(bis);
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpMedListing", new { searchString = abem_cost_center, CostCenter = abem_cost_center, yearlist = abem_budgeting_year });

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

            var IncludeCC = dbEst.bgt_expenses_Medical
                    .Where(w => w.abem_budgeting_year == abem_budgeting_year && w.abem_LadangID == LadangID && w.abem_WilayahID == WilayahID && w.abem_SyarikatID == SyarikatID && w.abem_NegaraID == NegaraID && w.abem_Deleted == false)
                    .Select(s => s.abem_cost_center)
                    .FirstOrDefault();

            List<SelectListItem> CostCenters = new List<SelectListItem>();
            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.abem_cost_center = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExpMedEditDetail(int abem_budgeting_year, string abem_cost_center)
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
                ViewBag.abem_budgeting_year = yearlist;

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abem_cost_center).ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abem_cost_center = CostCenters;

                bgt_expenses_Medical[] bgtExpensesMedical = dbEst.bgt_expenses_Medical.OrderBy(o => o.abem_gl_code).Where(x => x.abem_cost_center == abem_cost_center && x.abem_budgeting_year == abem_budgeting_year && x.abem_WilayahID == WilayahID && x.abem_SyarikatID == SyarikatID && x.abem_NegaraID == NegaraID && x.abem_LadangID == LadangID && x.abem_Deleted == false).ToArray();
                var getdata = dbEst.bgt_expenses_Medical.OrderBy(o => o.abem_gl_code).Where(w => w.abem_cost_center == abem_cost_center && w.abem_budgeting_year == abem_budgeting_year).ToList();
                ViewBag.GetData = getdata;

                return PartialView(bgtExpensesMedical);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpMedEditDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpMedEditDetail(bgt_expenses_Medical[] ExpenMed, int abem_budgeting_year, string abem_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abem_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenMed)
                    {
                        var getdata = dbEst.bgt_expenses_Medical.OrderBy(o => o.abem_gl_code).Where(x => x.abem_budgeting_year == abem_budgeting_year && x.abem_cost_center == abem_cost_center && x.abem_gl_code == val.abem_gl_code && x.abem_LadangID == LadangID && x.abem_WilayahID == WilayahID && x.abem_SyarikatID == SyarikatID && x.abem_NegaraID == NegaraID && x.abem_Deleted == false).FirstOrDefault();
                        getdata.abem_budgeting_year = abem_budgeting_year;
                        getdata.abem_cost_center = abem_cost_center;
                        getdata.abem_cost_center_name = cc_name;
                        getdata.abem_gl_code = val.abem_gl_code;
                        getdata.abem_gl_desc = val.abem_gl_desc;
                        getdata.abem_uom = val.abem_uom;
                        getdata.abem_proration = val.abem_proration;
                        getdata.abem_jumlah_setahun = val.abem_jumlah_setahun.HasValue ? val.abem_jumlah_setahun.Value : 0m;
                        getdata.abem_month_1 = val.abem_month_1.HasValue ? val.abem_month_1.Value : 0m;
                        getdata.abem_month_2 = val.abem_month_2.HasValue ? val.abem_month_2.Value : 0m;
                        getdata.abem_month_3 = val.abem_month_3.HasValue ? val.abem_month_3.Value : 0m;
                        getdata.abem_month_4 = val.abem_month_4.HasValue ? val.abem_month_4.Value : 0m;
                        getdata.abem_month_5 = val.abem_month_5.HasValue ? val.abem_month_5.Value : 0m;
                        getdata.abem_month_6 = val.abem_month_6.HasValue ? val.abem_month_6.Value : 0m;
                        getdata.abem_month_7 = val.abem_month_7.HasValue ? val.abem_month_7.Value : 0m;
                        getdata.abem_month_8 = val.abem_month_8.HasValue ? val.abem_month_8.Value : 0m;
                        getdata.abem_month_9 = val.abem_month_9.HasValue ? val.abem_month_9.Value : 0m;
                        getdata.abem_month_10 = val.abem_month_10.HasValue ? val.abem_month_10.Value : 0m;
                        getdata.abem_month_11 = val.abem_month_11.HasValue ? val.abem_month_11.Value : 0m;
                        getdata.abem_month_12 = val.abem_month_12.HasValue ? val.abem_month_12.Value : 0m;
                        getdata.abem_jumlah_RM = val.abem_jumlah_RM.HasValue ? val.abem_jumlah_RM.Value : 0m;
                        getdata.abem_jumlah_keseluruhan = val.abem_jumlah_keseluruhan.HasValue ? val.abem_jumlah_keseluruhan.Value : 0m;
                        getdata.last_modified = DateTime.Now;
                        getdata.abem_Deleted = false;
                        getdata.abem_NegaraID = NegaraID;
                        getdata.abem_SyarikatID = SyarikatID;
                        getdata.abem_WilayahID = WilayahID;
                        getdata.abem_LadangID = LadangID;
                        getdata.abem_revision = val.abem_revision;
                        getdata.abem_status = val.abem_status;
                        getdata.abem_history = val.abem_history;
                        dbEst.Entry(getdata).State = EntityState.Modified;
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpMedListing", new { searchString = abem_cost_center, CostCenter = abem_cost_center, yearlist = abem_budgeting_year });

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
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abem_cost_center).ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.CostCenter = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpMedDeleteDetail(int abem_budgeting_year, string abem_cost_center)
        {


            var year = int.Parse(abem_budgeting_year.ToString());
            var ExpMed = await dbEst.bgt_expenses_Medical.Where(c => c.abem_budgeting_year == year && c.abem_cost_center == abem_cost_center).FirstOrDefaultAsync();
            if (ExpMed == null)
            {
                return HttpNotFound();
            }

            var getdata = dbEst.bgt_expenses_Medical
                .Where(c => c.abem_budgeting_year == abem_budgeting_year && c.abem_cost_center == abem_cost_center && c.abem_proration != null && c.abem_Deleted == false)
                .ToList();

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbEst.bgt_expenses_Medical.Where(x => x.abem_Deleted == false).DistinctBy(x => x.abem_cost_center).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abem_cost_center).Select(s => new SelectListItem { Value = s.abem_cost_center, Text = s.abem_cost_center + " - " + s.abem_cost_center_name }), "Value", "Text", abem_cost_center).ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.abem_budgeting_year = abem_budgeting_year;
            ViewBag.abem_cost_center = CostCenters;

            return PartialView("ExpMedDeleteDetail", getdata);
        }

        [HttpPost, ActionName("ExpMedDeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExpMedDeleteDetailConfirmed(int abem_budgeting_year, string abem_cost_center)
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

                var year = int.Parse(abem_budgeting_year.ToString());

                var ExpMed = dbEst.bgt_expenses_Medical
                    .Where(c => c.abem_budgeting_year == year && c.abem_cost_center.Equals(abem_cost_center) && !c.abem_Deleted)
                    .ToList();

                if (ExpMed.Count() > 0)
                {
                    foreach (var item in ExpMed)
                    {
                        item.abem_Deleted = true;
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
                        controller = "ExpensesMedical",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abem_budgeting_year
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesMedical",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abem_budgeting_year
                    });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult ExpMedViewKeseluruhan()
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


                var CostCenterList = dbEst.bgt_expenses_Medical.Where(x => x.abem_Deleted == false).DistinctBy(x => x.abem_cost_center).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abem_cost_center).Select(s => new SelectListItem { Value = s.abem_cost_center, Text = s.abem_cost_center + " - " + s.abem_cost_center_name }), "Value", "Text").ToList();
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMed_List.rdlc");
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

            List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMed_ExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpMed(string costcenter, int year)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);


            bgt_expenses_Medical[] bgt_expenses_Medical = dbEst.bgt_expenses_Medical.Where(x => x.abem_cost_center == costcenter && x.abem_budgeting_year == year && x.abem_Deleted == false).ToArray();


            //Get Syarikat
            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            //Get Budget Year
            ViewBag.year = bgt_expenses_Medical.Select(s => s.abem_budgeting_year).FirstOrDefault();

            //Get GL Pendapatan
            ViewBag.fld_GLCode = bgt_expenses_Medical.Select(s => s.abem_gl_code).FirstOrDefault();
            ViewBag.fld_GLDesc = bgt_expenses_Medical.Select(s => s.abem_gl_desc).FirstOrDefault();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

            return PartialView(bgt_expenses_Medical);
        }

        public ActionResult ViewExpMedPDF(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMed_view.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpMedEXCEL(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMed_Excelview.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult _ExpMedSetahunRpt(bgt_expenses_Medical[] bgt_expenses_Medical, string costcenter, string year)
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
                    bgt_expenses_Medical = dbEst.bgt_expenses_Medical.Where(x => x.abem_budgeting_year.ToString() == year && x.abem_Deleted == false).OrderBy(o => o.abem_gl_code).ThenBy(x => x.abem_cost_center).ToArray();

                }
                else
                {
                    bgt_expenses_Medical = dbEst.bgt_expenses_Medical.Where(x => x.abem_cost_center == costcenter && x.abem_budgeting_year.ToString() == year && x.abem_Deleted == false).OrderBy(o => o.abem_gl_code).ToArray();
                }


                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Medical.Select(s => s.abem_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Medical.Select(s => s.abem_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Medical.Select(s => s.abem_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Medical.Where(x => x.abem_cost_center == costcenter).Select(o => o.abem_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

                return PartialView(bgt_expenses_Medical);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpMedSetahunRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewtahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewtahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult ExpMedSetahunRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewExceltahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewExceltahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult _ExpMedSebulanRpt(bgt_expenses_Medical[] bgt_expenses_Medical, string costcenter, int year)
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
                    bgt_expenses_Medical = dbEst.bgt_expenses_Medical.Where(x => x.abem_budgeting_year == year && x.abem_Deleted == false).OrderBy(o => o.abem_gl_code).ToArray();

                }
                else
                {
                    bgt_expenses_Medical = dbEst.bgt_expenses_Medical.Where(x => x.abem_cost_center == costcenter && x.abem_budgeting_year == year && x.abem_Deleted == false).OrderBy(o => o.abem_gl_code).ToArray();
                }

                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Medical.Select(s => s.abem_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Medical.Select(s => s.abem_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Medical.Select(s => s.abem_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Medical.Where(x => x.abem_cost_center == costcenter).Select(o => o.abem_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

                return PartialView(bgt_expenses_Medical);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpMedSebulanRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }
        public ActionResult ExpMedSebulanRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewExcelbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpMedViewExcelbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_MedicalView> data = dbEst.Database.SqlQuery<bgt_expenses_MedicalView>("exec sp_ExpMedView {0}, {1}", year, costcenter).ToList();
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
