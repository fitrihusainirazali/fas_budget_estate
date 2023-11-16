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
    public class ExpensesDepreciationController : Controller
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
        private readonly string screenCode = "E1";

        // GET: ExpenDepreciation
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult ExpDepListing()
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

        public ActionResult Records(string BudgetYear, string CostCenter,  int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<bgt_expenses_DepreciationView>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        private List<bgt_expenses_DepreciationView> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_Depreciation
                        where o.abed_SyarikatID == syarikatId && o.abed_LadangID == ladangId && o.abed_WilayahID == wilayahId && o.abed_Deleted == false
                        group o by new { o.abed_budgeting_year, o.abed_cost_center, o.abed_cost_center_name } into g
                        select new
                        {
                            BudgetYear = g.Key.abed_budgeting_year,
                            CostCenterCode = g.Key.abed_cost_center,
                            CostCenterDesc = g.Key.abed_cost_center_name,
                            Total = g.Sum(o => o.abed_jumlah_keseluruhan)
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
                          select new bgt_expenses_DepreciationView
                          {
                              abed_budgeting_year = q.BudgetYear,
                              abed_cost_center = q.CostCenterCode,
                              abed_cost_center_name = q.CostCenterDesc,
                              
                              abed_jumlah_keseluruhan = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult ExpDepCreateDtail()
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
                ViewBag.abed_budgeting_year = yearlist;

                var IncludeCC = dbEst.bgt_expenses_Depreciation
                    .Where(w => w.abed_budgeting_year == yearlist && w.abed_LadangID == LadangID && w.abed_WilayahID == WilayahID && w.abed_SyarikatID == SyarikatID && w.abed_NegaraID == NegaraID && w.abed_Deleted == false)
                    .Select(s => s.abed_cost_center)
                    .ToList();

                List<SelectListItem> CostCenters = new List<SelectListItem>();
                
                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abed_cost_center = CostCenters;
                
                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpDepCreateDtail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpDepCreateDtail(bgt_expenses_Depreciation[] ExpenDep, int abed_budgeting_year, string abed_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abed_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenDep)
                    {
                        var checkdata = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_budgeting_year == abed_budgeting_year && x.abed_cost_center == abed_cost_center && x.abed_Deleted == false).ToList();
                        if (checkdata == null || checkdata.Count() == 0)
                        {
                            bgt_expenses_Depreciation bis = new bgt_expenses_Depreciation()
                            {
                                
                                abed_budgeting_year = abed_budgeting_year,
                                abed_cost_center = abed_cost_center,
                                abed_cost_center_name = cc_name,
                                abed_gl_code = val.abed_gl_code,
                                abed_gl_desc = val.abed_gl_desc,
                                abed_uom = val.abed_uom,
                                abed_proration = val.abed_proration,
                                abed_jumlah_setahun = val.abed_jumlah_setahun.HasValue ? val.abed_jumlah_setahun.Value : 0m,
                                abed_month_1 = val.abed_month_1.HasValue ? val.abed_month_1.Value : 0m,
                                abed_month_2 = val.abed_month_2.HasValue ? val.abed_month_2.Value : 0m,
                                abed_month_3 = val.abed_month_3.HasValue ? val.abed_month_3.Value : 0m,
                                abed_month_4 = val.abed_month_4.HasValue ? val.abed_month_4.Value : 0m,
                                abed_month_5 = val.abed_month_5.HasValue ? val.abed_month_5.Value : 0m,
                                abed_month_6 = val.abed_month_6.HasValue ? val.abed_month_6.Value : 0m,
                                abed_month_7 = val.abed_month_7.HasValue ? val.abed_month_7.Value : 0m,
                                abed_month_8 = val.abed_month_8.HasValue ? val.abed_month_8.Value : 0m,
                                abed_month_9 = val.abed_month_9.HasValue ? val.abed_month_9.Value : 0m,
                                abed_month_10 = val.abed_month_10.HasValue ? val.abed_month_10.Value : 0m,
                                abed_month_11 = val.abed_month_11.HasValue ? val.abed_month_11.Value : 0m,
                                abed_month_12 = val.abed_month_12.HasValue ? val.abed_month_12.Value : 0m,
                                abed_jumlah_RM = val.abed_jumlah_RM.HasValue ? val.abed_jumlah_RM.Value : 0m,
                                abed_jumlah_keseluruhan = val.abed_jumlah_keseluruhan.HasValue ? val.abed_jumlah_keseluruhan.Value : 0m,
                                last_modified = DateTime.Now,
                                abed_Deleted = false,
                                abed_NegaraID = NegaraID,
                                abed_SyarikatID = SyarikatID,
                                abed_WilayahID = WilayahID,
                                abed_LadangID = LadangID,
                                abed_revision = 0,
                                abed_history = false,
                                abed_status = "DRAFT"
                            };
                            dbEst.bgt_expenses_Depreciation.Add(bis);
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpDepListing", new { searchString = abed_cost_center, CostCenter = abed_cost_center, yearlist = abed_budgeting_year });
                    
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

            var IncludeCC = dbEst.bgt_expenses_Depreciation
                    .Where(w => w.abed_budgeting_year == abed_budgeting_year && w.abed_LadangID == LadangID && w.abed_WilayahID == WilayahID && w.abed_SyarikatID == SyarikatID && w.abed_NegaraID == NegaraID && w.abed_Deleted == false)
                    .Select(s => s.abed_cost_center)
                    .FirstOrDefault();

            List<SelectListItem> CostCenters = new List<SelectListItem>();
            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.abed_cost_center = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExpDepEditDetail(int abed_budgeting_year, string abed_cost_center)
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
                ViewBag.abed_budgeting_year = yearlist;

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abed_cost_center).ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abed_cost_center = CostCenters;

                bgt_expenses_Depreciation[] bgtexpensesDepreciation = dbEst.bgt_expenses_Depreciation.OrderBy(o => o.abed_gl_code).Where(x => x.abed_cost_center == abed_cost_center && x.abed_budgeting_year == abed_budgeting_year && x.abed_WilayahID == WilayahID && x.abed_SyarikatID == SyarikatID && x.abed_NegaraID == NegaraID && x.abed_LadangID == LadangID && x.abed_Deleted == false).ToArray();
                var getdata = dbEst.bgt_expenses_Depreciation.OrderBy(o => o.abed_gl_code).Where(w => w.abed_cost_center == abed_cost_center && w.abed_budgeting_year == abed_budgeting_year).ToList();
                ViewBag.GetData = getdata;

                return PartialView(bgtexpensesDepreciation);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpDepEditDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpDepEditDetail(bgt_expenses_Depreciation[] ExpenDep, int abed_budgeting_year, string abed_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abed_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenDep)
                    {
                        var getdata = dbEst.bgt_expenses_Depreciation.OrderBy(o => o.abed_gl_code).Where(x => x.abed_budgeting_year == abed_budgeting_year && x.abed_cost_center == abed_cost_center && x.abed_gl_code == val.abed_gl_code && x.abed_LadangID == LadangID && x.abed_WilayahID == WilayahID && x.abed_SyarikatID == SyarikatID && x.abed_NegaraID == NegaraID && x.abed_Deleted == false).FirstOrDefault();
                        getdata.abed_budgeting_year = abed_budgeting_year;
                        getdata.abed_cost_center = abed_cost_center;
                        getdata.abed_cost_center_name = cc_name;
                        getdata.abed_gl_code = val.abed_gl_code;
                        getdata.abed_gl_desc = val.abed_gl_desc;
                        getdata.abed_uom = val.abed_uom;
                        getdata.abed_proration = val.abed_proration;
                        getdata.abed_jumlah_setahun = val.abed_jumlah_setahun.HasValue ? val.abed_jumlah_setahun.Value : 0m;
                        getdata.abed_month_1 = val.abed_month_1.HasValue ? val.abed_month_1.Value : 0m;
                        getdata.abed_month_2 = val.abed_month_2.HasValue ? val.abed_month_2.Value : 0m;
                        getdata.abed_month_3 = val.abed_month_3.HasValue ? val.abed_month_3.Value : 0m;
                        getdata.abed_month_4 = val.abed_month_4.HasValue ? val.abed_month_4.Value : 0m;
                        getdata.abed_month_5 = val.abed_month_5.HasValue ? val.abed_month_5.Value : 0m;
                        getdata.abed_month_6 = val.abed_month_6.HasValue ? val.abed_month_6.Value : 0m;
                        getdata.abed_month_7 = val.abed_month_7.HasValue ? val.abed_month_7.Value : 0m;
                        getdata.abed_month_8 = val.abed_month_8.HasValue ? val.abed_month_8.Value : 0m;
                        getdata.abed_month_9 = val.abed_month_9.HasValue ? val.abed_month_9.Value : 0m;
                        getdata.abed_month_10 = val.abed_month_10.HasValue ? val.abed_month_10.Value : 0m;
                        getdata.abed_month_11 = val.abed_month_11.HasValue ? val.abed_month_11.Value : 0m;
                        getdata.abed_month_12 = val.abed_month_12.HasValue ? val.abed_month_12.Value : 0m;
                        getdata.abed_jumlah_RM = val.abed_jumlah_RM.HasValue ? val.abed_jumlah_RM.Value : 0m;
                        getdata.abed_jumlah_keseluruhan = val.abed_jumlah_keseluruhan.HasValue ? val.abed_jumlah_keseluruhan.Value : 0m;
                        getdata.last_modified = DateTime.Now;
                        getdata.abed_Deleted = false;
                        getdata.abed_NegaraID = NegaraID;
                        getdata.abed_SyarikatID = SyarikatID;
                        getdata.abed_WilayahID = WilayahID;
                        getdata.abed_LadangID = LadangID;
                        getdata.abed_revision = val.abed_revision;
                        getdata.abed_status = val.abed_status;
                        getdata.abed_history = val.abed_history;
                        dbEst.Entry(getdata).State = EntityState.Modified;
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpDepListing", new { searchString = abed_cost_center, CostCenter = abed_cost_center, yearlist = abed_budgeting_year });

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
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abed_cost_center).ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.CostCenter = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpDepDeleteDetail(int abed_budgeting_year, string abed_cost_center)
        {

                        
            var year = int.Parse(abed_budgeting_year.ToString());
            var ExpDep = await dbEst.bgt_expenses_Depreciation.Where(c => c.abed_budgeting_year == year && c.abed_cost_center == abed_cost_center).FirstOrDefaultAsync();
            if (ExpDep == null)
            {
                return HttpNotFound();
            }

            var getdata = dbEst.bgt_expenses_Depreciation
                .Where(c => c.abed_budgeting_year == abed_budgeting_year && c.abed_cost_center == abed_cost_center && c.abed_proration != null && c.abed_Deleted == false)
                .ToList();

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_Deleted == false).DistinctBy(x => x.abed_cost_center).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abed_cost_center).Select(s => new SelectListItem { Value = s.abed_cost_center, Text = s.abed_cost_center + " - " + s.abed_cost_center_name }), "Value", "Text", abed_cost_center).ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.abed_budgeting_year = abed_budgeting_year;
            ViewBag.abed_cost_center = CostCenters;
            
            return PartialView("ExpDepDeleteDetail", getdata);
        }

        [HttpPost, ActionName("ExpDepDeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExpDepDeleteDetailConfirmed(int abed_budgeting_year, string abed_cost_center)
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

                var year = int.Parse(abed_budgeting_year.ToString());
               
                var expdep = dbEst.bgt_expenses_Depreciation
                    .Where(c => c.abed_budgeting_year == year && c.abed_cost_center.Equals(abed_cost_center) && !c.abed_Deleted)
                    .ToList();

                if (expdep.Count() > 0)
                {
                    foreach (var item in expdep)
                    {
                        item.abed_Deleted = true;
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
                        controller = "ExpensesDepreciation",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abed_budgeting_year
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesDepreciation",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abed_budgeting_year
                    });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult ExpDepViewKeseluruhan()
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

                
                var CostCenterList = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_Deleted == false).DistinctBy(x => x.abed_cost_center).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abed_cost_center).Select(s => new SelectListItem { Value = s.abed_cost_center, Text = s.abed_cost_center + " - " + s.abed_cost_center_name }), "Value", "Text").ToList();
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDep_List.rdlc");
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

            List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDep_ExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpDep(string costcenter, int year)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            
            bgt_expenses_Depreciation[] bgt_expenses_Depreciation = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_cost_center == costcenter && x.abed_budgeting_year == year && x.abed_Deleted == false).ToArray();


            //Get Syarikat
            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            //Get Budget Year
            ViewBag.year = bgt_expenses_Depreciation.Select(s => s.abed_budgeting_year).FirstOrDefault();

            //Get GL Pendapatan
            ViewBag.fld_GLCode = bgt_expenses_Depreciation.Select(s => s.abed_gl_code).FirstOrDefault();
            ViewBag.fld_GLDesc = bgt_expenses_Depreciation.Select(s => s.abed_gl_desc).FirstOrDefault();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

            return PartialView(bgt_expenses_Depreciation);
        }

        public ActionResult ViewExpDepPDF(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDep_view.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpDepEXCEL(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDep_Excelview.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult _ExpDepSetahunRpt(bgt_expenses_Depreciation[] bgt_expenses_Depreciation, string costcenter, string year)
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
                    bgt_expenses_Depreciation = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_budgeting_year.ToString() == year && x.abed_Deleted == false).OrderBy(o => o.abed_gl_code).ThenBy(x => x.abed_cost_center).ToArray();
                    
                }
                else
                {
                    bgt_expenses_Depreciation = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_cost_center == costcenter && x.abed_budgeting_year.ToString() == year && x.abed_Deleted == false).OrderBy(o => o.abed_gl_code).ToArray();
                }


                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Depreciation.Select(s => s.abed_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Depreciation.Select(s => s.abed_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Depreciation.Select(s => s.abed_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_cost_center == costcenter).Select(o => o.abed_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

                return PartialView(bgt_expenses_Depreciation);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpDepSetahunRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewtahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";
                
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewtahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            
        }

        public ActionResult ExpDepSetahunRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewExceltahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewExceltahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult _ExpDepSebulanRpt(bgt_expenses_Depreciation[] bgt_expenses_Depreciation, string costcenter, int year)
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
                    bgt_expenses_Depreciation = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_budgeting_year == year && x.abed_Deleted == false).OrderBy(o => o.abed_gl_code).ToArray();

                }
                else
                {
                    bgt_expenses_Depreciation = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_cost_center == costcenter && x.abed_budgeting_year == year && x.abed_Deleted == false).OrderBy(o => o.abed_gl_code).ToArray();
                }

                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Depreciation.Select(s => s.abed_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Depreciation.Select(s => s.abed_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Depreciation.Select(s => s.abed_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Depreciation.Where(x => x.abed_cost_center == costcenter).Select(o => o.abed_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

                return PartialView(bgt_expenses_Depreciation);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpDepSebulanRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }
        public ActionResult ExpDepSebulanRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewExcelbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpDepViewExcelbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_DepreciationView> data = dbEst.Database.SqlQuery<bgt_expenses_DepreciationView>("exec sp_ExpDepView {0}, {1}", year, costcenter).ToList();
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
