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
    public class ExpensesCorpServController : Controller
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
        private readonly string screenCode = "E6";

        // GET: ExpenCorpSreciation
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ExpCorpSListing()
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

            var records = new ViewingModels.PagedList<bgt_expenses_KhidCorView>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        private List<bgt_expenses_KhidCorView> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_KhidCor
                        where o.abcs_SyarikatID == syarikatId && o.abcs_LadangID == ladangId && o.abcs_WilayahID == wilayahId && o.abcs_Deleted == false
                        group o by new { o.abcs_budgeting_year, o.abcs_cost_center, o.abcs_cost_center_name } into g
                        select new
                        {
                            BudgetYear = g.Key.abcs_budgeting_year,
                            CostCenterCode = g.Key.abcs_cost_center,
                            CostCenterDesc = g.Key.abcs_cost_center_name,
                            Total = g.Sum(o => o.abcs_jumlah_keseluruhan)
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
                          select new bgt_expenses_KhidCorView
                          {
                              abcs_budgeting_year = q.BudgetYear,
                              abcs_cost_center = q.CostCenterCode,
                              abcs_cost_center_name = q.CostCenterDesc,

                              abcs_jumlah_keseluruhan = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult ExpCorpSCreateDtail()
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
                ViewBag.abcs_budgeting_year = yearlist;

                var IncludeCC = dbEst.bgt_expenses_KhidCor
                    .Where(w => w.abcs_budgeting_year == yearlist && w.abcs_LadangID == LadangID && w.abcs_WilayahID == WilayahID && w.abcs_SyarikatID == SyarikatID && w.abcs_NegaraID == NegaraID && w.abcs_Deleted == false)
                    .Select(s => s.abcs_cost_center)
                    .ToList();

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abcs_cost_center = CostCenters;

                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpCorpSCreateDtail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpCorpSCreateDtail(bgt_expenses_KhidCor[] ExpenCorpS, int abcs_budgeting_year, string abcs_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abcs_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenCorpS)
                    {
                        var checkdata = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_budgeting_year == abcs_budgeting_year && x.abcs_cost_center == abcs_cost_center && x.abcs_Deleted == false).ToList();
                        if (checkdata == null || checkdata.Count() == 0)
                        {
                            bgt_expenses_KhidCor bis = new bgt_expenses_KhidCor()
                            {

                                abcs_budgeting_year = abcs_budgeting_year,
                                abcs_cost_center = abcs_cost_center,
                                abcs_cost_center_name = cc_name,
                                abcs_gl_code = val.abcs_gl_code,
                                abcs_gl_desc = val.abcs_gl_desc,
                                abcs_uom = val.abcs_uom,
                                abcs_proration = val.abcs_proration,
                                abcs_jumlah_setahun = val.abcs_jumlah_setahun.HasValue ? val.abcs_jumlah_setahun.Value : 0m,
                                abcs_month_1 = val.abcs_month_1.HasValue ? val.abcs_month_1.Value : 0m,
                                abcs_month_2 = val.abcs_month_2.HasValue ? val.abcs_month_2.Value : 0m,
                                abcs_month_3 = val.abcs_month_3.HasValue ? val.abcs_month_3.Value : 0m,
                                abcs_month_4 = val.abcs_month_4.HasValue ? val.abcs_month_4.Value : 0m,
                                abcs_month_5 = val.abcs_month_5.HasValue ? val.abcs_month_5.Value : 0m,
                                abcs_month_6 = val.abcs_month_6.HasValue ? val.abcs_month_6.Value : 0m,
                                abcs_month_7 = val.abcs_month_7.HasValue ? val.abcs_month_7.Value : 0m,
                                abcs_month_8 = val.abcs_month_8.HasValue ? val.abcs_month_8.Value : 0m,
                                abcs_month_9 = val.abcs_month_9.HasValue ? val.abcs_month_9.Value : 0m,
                                abcs_month_10 = val.abcs_month_10.HasValue ? val.abcs_month_10.Value : 0m,
                                abcs_month_11 = val.abcs_month_11.HasValue ? val.abcs_month_11.Value : 0m,
                                abcs_month_12 = val.abcs_month_12.HasValue ? val.abcs_month_12.Value : 0m,
                                abcs_jumlah_RM = val.abcs_jumlah_RM.HasValue ? val.abcs_jumlah_RM.Value : 0m,
                                abcs_jumlah_keseluruhan = val.abcs_jumlah_keseluruhan.HasValue ? val.abcs_jumlah_keseluruhan.Value : 0m,
                                last_modified = DateTime.Now,
                                abcs_Deleted = false,
                                abcs_NegaraID = NegaraID,
                                abcs_SyarikatID = SyarikatID,
                                abcs_WilayahID = WilayahID,
                                abcs_LadangID = LadangID,
                                abcs_revision = 0,
                                abcs_history = false,
                                abcs_status = "DRAFT"
                            };
                            dbEst.bgt_expenses_KhidCor.Add(bis);
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpCorpSListing", new { searchString = abcs_cost_center, CostCenter = abcs_cost_center, yearlist = abcs_budgeting_year });

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

            var IncludeCC = dbEst.bgt_expenses_KhidCor
                    .Where(w => w.abcs_budgeting_year == abcs_budgeting_year && w.abcs_LadangID == LadangID && w.abcs_WilayahID == WilayahID && w.abcs_SyarikatID == SyarikatID && w.abcs_NegaraID == NegaraID && w.abcs_Deleted == false)
                    .Select(s => s.abcs_cost_center)
                    .FirstOrDefault();

            List<SelectListItem> CostCenters = new List<SelectListItem>();
            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.abcs_cost_center = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExpCorpSEditDetail(int abcs_budgeting_year, string abcs_cost_center)
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
                ViewBag.abcs_budgeting_year = yearlist;

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abcs_cost_center).ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abcs_cost_center = CostCenters;

                bgt_expenses_KhidCor[] bgtExpensesCorpServ = dbEst.bgt_expenses_KhidCor.OrderBy(o => o.abcs_gl_code).Where(x => x.abcs_cost_center == abcs_cost_center && x.abcs_budgeting_year == abcs_budgeting_year && x.abcs_WilayahID == WilayahID && x.abcs_SyarikatID == SyarikatID && x.abcs_NegaraID == NegaraID && x.abcs_LadangID == LadangID && x.abcs_Deleted == false).ToArray();
                var getdata = dbEst.bgt_expenses_KhidCor.OrderBy(o => o.abcs_gl_code).Where(w => w.abcs_cost_center == abcs_cost_center && w.abcs_budgeting_year == abcs_budgeting_year).ToList();
                ViewBag.GetData = getdata;

                return PartialView(bgtExpensesCorpServ);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpCorpSEditDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpCorpSEditDetail(bgt_expenses_KhidCor[] ExpenCorpS, int abcs_budgeting_year, string abcs_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abcs_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenCorpS)
                    {
                        var getdata = dbEst.bgt_expenses_KhidCor.OrderBy(o => o.abcs_gl_code).Where(x => x.abcs_budgeting_year == abcs_budgeting_year && x.abcs_cost_center == abcs_cost_center && x.abcs_gl_code == val.abcs_gl_code && x.abcs_LadangID == LadangID && x.abcs_WilayahID == WilayahID && x.abcs_SyarikatID == SyarikatID && x.abcs_NegaraID == NegaraID && x.abcs_Deleted == false).FirstOrDefault();
                        getdata.abcs_budgeting_year = abcs_budgeting_year;
                        getdata.abcs_cost_center = abcs_cost_center;
                        getdata.abcs_cost_center_name = cc_name;
                        getdata.abcs_gl_code = val.abcs_gl_code;
                        getdata.abcs_gl_desc = val.abcs_gl_desc;
                        getdata.abcs_uom = val.abcs_uom;
                        getdata.abcs_proration = val.abcs_proration;
                        getdata.abcs_jumlah_setahun = val.abcs_jumlah_setahun.HasValue ? val.abcs_jumlah_setahun.Value : 0m;
                        getdata.abcs_month_1 = val.abcs_month_1.HasValue ? val.abcs_month_1.Value : 0m;
                        getdata.abcs_month_2 = val.abcs_month_2.HasValue ? val.abcs_month_2.Value : 0m;
                        getdata.abcs_month_3 = val.abcs_month_3.HasValue ? val.abcs_month_3.Value : 0m;
                        getdata.abcs_month_4 = val.abcs_month_4.HasValue ? val.abcs_month_4.Value : 0m;
                        getdata.abcs_month_5 = val.abcs_month_5.HasValue ? val.abcs_month_5.Value : 0m;
                        getdata.abcs_month_6 = val.abcs_month_6.HasValue ? val.abcs_month_6.Value : 0m;
                        getdata.abcs_month_7 = val.abcs_month_7.HasValue ? val.abcs_month_7.Value : 0m;
                        getdata.abcs_month_8 = val.abcs_month_8.HasValue ? val.abcs_month_8.Value : 0m;
                        getdata.abcs_month_9 = val.abcs_month_9.HasValue ? val.abcs_month_9.Value : 0m;
                        getdata.abcs_month_10 = val.abcs_month_10.HasValue ? val.abcs_month_10.Value : 0m;
                        getdata.abcs_month_11 = val.abcs_month_11.HasValue ? val.abcs_month_11.Value : 0m;
                        getdata.abcs_month_12 = val.abcs_month_12.HasValue ? val.abcs_month_12.Value : 0m;
                        getdata.abcs_jumlah_RM = val.abcs_jumlah_RM.HasValue ? val.abcs_jumlah_RM.Value : 0m;
                        getdata.abcs_jumlah_keseluruhan = val.abcs_jumlah_keseluruhan.HasValue ? val.abcs_jumlah_keseluruhan.Value : 0m;
                        getdata.last_modified = DateTime.Now;
                        getdata.abcs_Deleted = false;
                        getdata.abcs_NegaraID = NegaraID;
                        getdata.abcs_SyarikatID = SyarikatID;
                        getdata.abcs_WilayahID = WilayahID;
                        getdata.abcs_LadangID = LadangID;
                        getdata.abcs_revision = val.abcs_revision;
                        getdata.abcs_status = val.abcs_status;
                        getdata.abcs_history = val.abcs_history;
                        dbEst.Entry(getdata).State = EntityState.Modified;
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpCorpSListing", new { searchString = abcs_cost_center, CostCenter = abcs_cost_center, yearlist = abcs_budgeting_year });

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
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abcs_cost_center).ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.CostCenter = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpCorpSDeleteDetail(int abcs_budgeting_year, string abcs_cost_center)
        {


            var year = int.Parse(abcs_budgeting_year.ToString());
            var ExpCorpS = await dbEst.bgt_expenses_KhidCor.Where(c => c.abcs_budgeting_year == year && c.abcs_cost_center == abcs_cost_center).FirstOrDefaultAsync();
            if (ExpCorpS == null)
            {
                return HttpNotFound();
            }

            var getdata = dbEst.bgt_expenses_KhidCor
                .Where(c => c.abcs_budgeting_year == abcs_budgeting_year && c.abcs_cost_center == abcs_cost_center && c.abcs_proration != null && c.abcs_Deleted == false)
                .ToList();

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_Deleted == false).DistinctBy(x => x.abcs_cost_center).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abcs_cost_center).Select(s => new SelectListItem { Value = s.abcs_cost_center, Text = s.abcs_cost_center + " - " + s.abcs_cost_center_name }), "Value", "Text", abcs_cost_center).ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.abcs_budgeting_year = abcs_budgeting_year;
            ViewBag.abcs_cost_center = CostCenters;

            return PartialView("ExpCorpSDeleteDetail", getdata);
        }

        [HttpPost, ActionName("ExpCorpSDeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExpCorpSDeleteDetailConfirmed(int abcs_budgeting_year, string abcs_cost_center)
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

                var year = int.Parse(abcs_budgeting_year.ToString());

                var ExpCorpS = dbEst.bgt_expenses_KhidCor
                    .Where(c => c.abcs_budgeting_year == year && c.abcs_cost_center.Equals(abcs_cost_center) && !c.abcs_Deleted)
                    .ToList();

                if (ExpCorpS.Count() > 0)
                {
                    foreach (var item in ExpCorpS)
                    {
                        item.abcs_Deleted = true;
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
                        controller = "ExpensesCorpServ",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abcs_budgeting_year
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesCorpServ",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abcs_budgeting_year
                    });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult ExpCorpSViewKeseluruhan()
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


                var CostCenterList = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_Deleted == false).DistinctBy(x => x.abcs_cost_center).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abcs_cost_center).Select(s => new SelectListItem { Value = s.abcs_cost_center, Text = s.abcs_cost_center + " - " + s.abcs_cost_center_name }), "Value", "Text").ToList();
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpS_List.rdlc");
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

            List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpS_ExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpCorpS(string costcenter, int year)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);


            bgt_expenses_KhidCor[] bgt_expenses_KhidCor = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_cost_center == costcenter && x.abcs_budgeting_year == year && x.abcs_Deleted == false).ToArray();


            //Get Syarikat
            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            //Get Budget Year
            ViewBag.year = bgt_expenses_KhidCor.Select(s => s.abcs_budgeting_year).FirstOrDefault();

            //Get GL Pendapatan
            ViewBag.fld_GLCode = bgt_expenses_KhidCor.Select(s => s.abcs_gl_code).FirstOrDefault();
            ViewBag.fld_GLDesc = bgt_expenses_KhidCor.Select(s => s.abcs_gl_desc).FirstOrDefault();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

            return PartialView(bgt_expenses_KhidCor);
        }

        public ActionResult ViewExpCorpSPDF(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpS_view.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpCorpSEXCEL(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpS_Excelview.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult _ExpCorpSSetahunRpt(bgt_expenses_KhidCor[] bgt_expenses_KhidCor, string costcenter, string year)
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
                    bgt_expenses_KhidCor = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_budgeting_year.ToString() == year && x.abcs_Deleted == false).OrderBy(o => o.abcs_gl_code).ThenBy(x => x.abcs_cost_center).ToArray();

                }
                else
                {
                    bgt_expenses_KhidCor = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_cost_center == costcenter && x.abcs_budgeting_year.ToString() == year && x.abcs_Deleted == false).OrderBy(o => o.abcs_gl_code).ToArray();
                }


                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_KhidCor.Select(s => s.abcs_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_KhidCor.Select(s => s.abcs_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_KhidCor.Select(s => s.abcs_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_cost_center == costcenter).Select(o => o.abcs_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_KhidCor);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpCorpSSetahunRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewtahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewtahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult ExpCorpSSetahunRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewExceltahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewExceltahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult _ExpCorpSSebulanRpt(bgt_expenses_KhidCor[] bgt_expenses_KhidCor, string costcenter, int year)
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
                    bgt_expenses_KhidCor = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_budgeting_year == year && x.abcs_Deleted == false).OrderBy(o => o.abcs_gl_code).ToArray();

                }
                else
                {
                    bgt_expenses_KhidCor = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_cost_center == costcenter && x.abcs_budgeting_year == year && x.abcs_Deleted == false).OrderBy(o => o.abcs_gl_code).ToArray();
                }

                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_KhidCor.Select(s => s.abcs_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_KhidCor.Select(s => s.abcs_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_KhidCor.Select(s => s.abcs_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_KhidCor.Where(x => x.abcs_cost_center == costcenter).Select(o => o.abcs_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_KhidCor);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpCorpSSebulanRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }
        public ActionResult ExpCorpSSebulanRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewExcelbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpCorpSViewExcelbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_KhidCorView> data = dbEst.Database.SqlQuery<bgt_expenses_KhidCorView>("exec sp_ExpCorpSView {0}, {1}", year, costcenter).ToList();
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
