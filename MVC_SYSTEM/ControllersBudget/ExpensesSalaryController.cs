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
    public class ExpensesSalaryController : Controller
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
        private readonly string screenCode = "E2";

        // GET: ExpenSalary
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ExpSalListing()
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

            var records = new ViewingModels.PagedList<bgt_expenses_SalaryView>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        private List<bgt_expenses_SalaryView> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_Salary
                        where o.abes_SyarikatID == syarikatId && o.abes_LadangID == ladangId && o.abes_WilayahID == wilayahId && o.abes_Deleted == false
                        group o by new { o.abes_budgeting_year, o.abes_cost_center, o.abes_cost_center_name } into g
                        select new
                        {
                            BudgetYear = g.Key.abes_budgeting_year,
                            CostCenterCode = g.Key.abes_cost_center,
                            CostCenterDesc = g.Key.abes_cost_center_name,
                            Total = g.Sum(o => o.abes_jumlah_keseluruhan)
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
                          select new bgt_expenses_SalaryView
                          {
                              abes_budgeting_year = q.BudgetYear,
                              abes_cost_center = q.CostCenterCode,
                              abes_cost_center_name = q.CostCenterDesc,

                              abes_jumlah_keseluruhan = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult ExpSalCreateDtail()
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
                ViewBag.abes_budgeting_year = yearlist;

                var IncludeCC = dbEst.bgt_expenses_Salary
                    .Where(w => w.abes_budgeting_year == yearlist && w.abes_LadangID == LadangID && w.abes_WilayahID == WilayahID && w.abes_SyarikatID == SyarikatID && w.abes_NegaraID == NegaraID && w.abes_Deleted == false)
                    .Select(s => s.abes_cost_center)
                    .ToList();

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abes_cost_center = CostCenters;

                return View();

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpSalCreateDtail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpSalCreateDtail(bgt_expenses_Salary[] ExpenSal, int abes_budgeting_year, string abes_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abes_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenSal)
                    {
                        var checkdata = dbEst.bgt_expenses_Salary.Where(x => x.abes_budgeting_year == abes_budgeting_year && x.abes_cost_center == abes_cost_center && x.abes_Deleted == false).ToList();
                        if (checkdata == null || checkdata.Count() == 0)
                        {
                            bgt_expenses_Salary bis = new bgt_expenses_Salary()
                            {

                                abes_budgeting_year = abes_budgeting_year,
                                abes_cost_center = abes_cost_center,
                                abes_cost_center_name = cc_name,
                                abes_gl_code = val.abes_gl_code,
                                abes_gl_desc = val.abes_gl_desc,
                                abes_uom = val.abes_uom,
                                abes_proration = val.abes_proration,
                                abes_jumlah_setahun = val.abes_jumlah_setahun.HasValue ? val.abes_jumlah_setahun.Value : 0m,
                                abes_qty_1 = val.abes_qty_1,
                                abes_qty_2 = val.abes_qty_2,
                                abes_qty_3 = val.abes_qty_3,
                                abes_qty_4 = val.abes_qty_4,
                                abes_qty_5 = val.abes_qty_5,
                                abes_qty_6 = val.abes_qty_6,
                                abes_qty_7 = val.abes_qty_7,
                                abes_qty_8 = val.abes_qty_8,
                                abes_qty_9 = val.abes_qty_9,
                                abes_qty_10 = val.abes_qty_10,
                                abes_qty_11 = val.abes_qty_11,
                                abes_qty_12 = val.abes_qty_12,
                                abes_jumlah_keseluruhan_qty = val.abes_jumlah_keseluruhan_qty,
                                abes_jumlah_RM = val.abes_jumlah_RM.HasValue ? val.abes_jumlah_RM.Value : 0m,
                                abes_jumlah_keseluruhan = val.abes_jumlah_keseluruhan.HasValue ? val.abes_jumlah_keseluruhan.Value : 0m,
                                abes_month_1 = val.abes_month_1.HasValue ? val.abes_month_1.Value : 0m,
                                abes_month_2 = val.abes_month_2.HasValue ? val.abes_month_2.Value : 0m,
                                abes_month_3 = val.abes_month_3.HasValue ? val.abes_month_3.Value : 0m,
                                abes_month_4 = val.abes_month_4.HasValue ? val.abes_month_4.Value : 0m,
                                abes_month_5 = val.abes_month_5.HasValue ? val.abes_month_5.Value : 0m,
                                abes_month_6 = val.abes_month_6.HasValue ? val.abes_month_6.Value : 0m,
                                abes_month_7 = val.abes_month_7.HasValue ? val.abes_month_7.Value : 0m,
                                abes_month_8 = val.abes_month_8.HasValue ? val.abes_month_8.Value : 0m,
                                abes_month_9 = val.abes_month_9.HasValue ? val.abes_month_9.Value : 0m,
                                abes_month_10 = val.abes_month_10.HasValue ? val.abes_month_10.Value : 0m,
                                abes_month_11 = val.abes_month_11.HasValue ? val.abes_month_11.Value : 0m,
                                abes_month_12 = val.abes_month_12.HasValue ? val.abes_month_12.Value : 0m,
                                
                                last_modified = DateTime.Now,
                                abes_Deleted = false,
                                abes_NegaraID = NegaraID,
                                abes_SyarikatID = SyarikatID,
                                abes_WilayahID = WilayahID,
                                abes_LadangID = LadangID,
                                abes_revision = 0,
                                abes_history = false,
                                abes_status = "DRAFT"
                            };
                            dbEst.bgt_expenses_Salary.Add(bis);
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpSalListing", new { searchString = abes_cost_center, CostCenter = abes_cost_center, yearlist = abes_budgeting_year });

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

            var IncludeCC = dbEst.bgt_expenses_Salary
                    .Where(w => w.abes_budgeting_year == abes_budgeting_year && w.abes_LadangID == LadangID && w.abes_WilayahID == WilayahID && w.abes_SyarikatID == SyarikatID && w.abes_NegaraID == NegaraID && w.abes_Deleted == false)
                    .Select(s => s.abes_cost_center)
                    .FirstOrDefault();

            List<SelectListItem> CostCenters = new List<SelectListItem>();
            var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text").ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.abes_cost_center = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExpSalEditDetail(int abes_budgeting_year, string abes_cost_center)
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
                ViewBag.abes_budgeting_year = yearlist;

                List<SelectListItem> CostCenters = new List<SelectListItem>();

                var CostCenterList = dbM.tbl_SAPCCPUP.Where(x => x.fld_Deleted == false).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abes_cost_center).ToList();
                CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

                ViewBag.abes_cost_center = CostCenters;

                bgt_expenses_Salary[] bgtexpensesSalary = dbEst.bgt_expenses_Salary.OrderBy(o => o.abes_gl_code).Where(x => x.abes_cost_center == abes_cost_center && x.abes_budgeting_year == abes_budgeting_year && x.abes_WilayahID == WilayahID && x.abes_SyarikatID == SyarikatID && x.abes_NegaraID == NegaraID && x.abes_LadangID == LadangID && x.abes_Deleted == false).ToArray();
                var getdata = dbEst.bgt_expenses_Salary.OrderBy(o => o.abes_gl_code).Where(w => w.abes_cost_center == abes_cost_center && w.abes_budgeting_year == abes_budgeting_year).ToList();
                ViewBag.GetData = getdata;

                return PartialView(bgtexpensesSalary);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        [HttpPost, ActionName("ExpSalEditDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult ExpSalEditDetail(bgt_expenses_Salary[] ExpenSal, int abes_budgeting_year, string abes_cost_center)
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
                    var cc_name = dbM.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == abes_cost_center).Select(o => o.fld_CostCenterDesc).FirstOrDefault();

                    foreach (var val in ExpenSal)
                    {
                        var getdata = dbEst.bgt_expenses_Salary.OrderBy(o => o.abes_gl_code).Where(x => x.abes_budgeting_year == abes_budgeting_year && x.abes_cost_center == abes_cost_center && x.abes_gl_code == val.abes_gl_code && x.abes_LadangID == LadangID && x.abes_WilayahID == WilayahID && x.abes_SyarikatID == SyarikatID && x.abes_NegaraID == NegaraID && x.abes_Deleted == false).FirstOrDefault();
                        getdata.abes_budgeting_year = abes_budgeting_year;
                        getdata.abes_cost_center = abes_cost_center;
                        getdata.abes_cost_center_name = cc_name;
                        getdata.abes_gl_code = val.abes_gl_code;
                        getdata.abes_gl_desc = val.abes_gl_desc;
                        getdata.abes_uom = val.abes_uom;
                        getdata.abes_proration = val.abes_proration;
                        getdata.abes_jumlah_setahun = val.abes_jumlah_setahun.HasValue ? val.abes_jumlah_setahun.Value : 0m;
                        getdata.abes_qty_1 = val.abes_qty_1;
                        getdata.abes_qty_2 = val.abes_qty_2;
                        getdata.abes_qty_3 = val.abes_qty_3;
                        getdata.abes_qty_4 = val.abes_qty_4;
                        getdata.abes_qty_5 = val.abes_qty_5;
                        getdata.abes_qty_6 = val.abes_qty_6;
                        getdata.abes_qty_7 = val.abes_qty_7;
                        getdata.abes_qty_8 = val.abes_qty_8;
                        getdata.abes_qty_9 = val.abes_qty_9;
                        getdata.abes_qty_10 = val.abes_qty_10;
                        getdata.abes_qty_11 = val.abes_qty_11;
                        getdata.abes_qty_12 = val.abes_qty_12;
                        getdata.abes_jumlah_keseluruhan_qty = val.abes_jumlah_keseluruhan_qty;
                        getdata.abes_jumlah_RM = val.abes_jumlah_RM.HasValue ? val.abes_jumlah_RM.Value : 0m;
                        getdata.abes_jumlah_keseluruhan = val.abes_jumlah_keseluruhan.HasValue ? val.abes_jumlah_keseluruhan.Value : 0m;
                        
                        getdata.abes_month_1 = val.abes_month_1.HasValue ? val.abes_month_1.Value : 0m;
                        getdata.abes_month_2 = val.abes_month_2.HasValue ? val.abes_month_2.Value : 0m;
                        getdata.abes_month_3 = val.abes_month_3.HasValue ? val.abes_month_3.Value : 0m;
                        getdata.abes_month_4 = val.abes_month_4.HasValue ? val.abes_month_4.Value : 0m;
                        getdata.abes_month_5 = val.abes_month_5.HasValue ? val.abes_month_5.Value : 0m;
                        getdata.abes_month_6 = val.abes_month_6.HasValue ? val.abes_month_6.Value : 0m;
                        getdata.abes_month_7 = val.abes_month_7.HasValue ? val.abes_month_7.Value : 0m;
                        getdata.abes_month_8 = val.abes_month_8.HasValue ? val.abes_month_8.Value : 0m;
                        getdata.abes_month_9 = val.abes_month_9.HasValue ? val.abes_month_9.Value : 0m;
                        getdata.abes_month_10 = val.abes_month_10.HasValue ? val.abes_month_10.Value : 0m;
                        getdata.abes_month_11 = val.abes_month_11.HasValue ? val.abes_month_11.Value : 0m;
                        getdata.abes_month_12 = val.abes_month_12.HasValue ? val.abes_month_12.Value : 0m;
                        
                        getdata.last_modified = DateTime.Now;
                        getdata.abes_Deleted = false;
                        getdata.abes_NegaraID = NegaraID;
                        getdata.abes_SyarikatID = SyarikatID;
                        getdata.abes_WilayahID = WilayahID;
                        getdata.abes_LadangID = LadangID;
                        getdata.abes_revision = val.abes_revision;
                        getdata.abes_status = val.abes_status;
                        getdata.abes_history = val.abes_history;
                        dbEst.Entry(getdata).State = EntityState.Modified;
                    }
                    dbEst.SaveChanges();
                    return RedirectToAction("ExpSalListing", new { searchString = abes_cost_center, CostCenter = abes_cost_center, yearlist = abes_budgeting_year });

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
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }), "Value", "Text", abes_cost_center).ToList();
            CostCenters.Insert(0, (new SelectListItem { Text = "Please Select", Value = "", Selected = true }));

            ViewBag.CostCenter = CostCenters;

            return Json(new { hdrmsg, msg, status }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpSalDeleteDetail(int abes_budgeting_year, string abes_cost_center)
        {


            var year = int.Parse(abes_budgeting_year.ToString());
            var ExpSal = await dbEst.bgt_expenses_Salary.Where(c => c.abes_budgeting_year == year && c.abes_cost_center == abes_cost_center).FirstOrDefaultAsync();
            if (ExpSal == null)
            {
                return HttpNotFound();
            }

            var getdata = dbEst.bgt_expenses_Salary
                .Where(c => c.abes_budgeting_year == abes_budgeting_year && c.abes_cost_center == abes_cost_center && c.abes_proration != null && c.abes_Deleted == false)
                .ToList();

            List<SelectListItem> CostCenters = new List<SelectListItem>();

            var CostCenterList = dbEst.bgt_expenses_Salary.Where(x => x.abes_Deleted == false).DistinctBy(x => x.abes_cost_center).ToList();
            CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abes_cost_center).Select(s => new SelectListItem { Value = s.abes_cost_center, Text = s.abes_cost_center + " - " + s.abes_cost_center_name }), "Value", "Text", abes_cost_center).ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.abes_budgeting_year = abes_budgeting_year;
            ViewBag.abes_cost_center = CostCenters;

            return PartialView("ExpSalDeleteDetail", getdata);
        }

        [HttpPost, ActionName("ExpSalDeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExpSalDeleteDetailConfirmed(int abes_budgeting_year, string abes_cost_center)
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

                var year = int.Parse(abes_budgeting_year.ToString());

                var ExpSal = dbEst.bgt_expenses_Salary
                    .Where(c => c.abes_budgeting_year == year && c.abes_cost_center.Equals(abes_cost_center) && !c.abes_Deleted)
                    .ToList();

                if (ExpSal.Count() > 0)
                {
                    foreach (var item in ExpSal)
                    {
                        item.abes_Deleted = true;
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
                        controller = "ExpensesSalary",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abes_budgeting_year
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesSalary",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = abes_budgeting_year
                    });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult ExpSalViewKeseluruhan()
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


                var CostCenterList = dbEst.bgt_expenses_Salary.Where(x => x.abes_Deleted == false).DistinctBy(x => x.abes_cost_center).ToList();
                CostCenters = new SelectList(CostCenterList.OrderBy(o => o.abes_cost_center).Select(s => new SelectListItem { Value = s.abes_cost_center, Text = s.abes_cost_center + " - " + s.abes_cost_center_name }), "Value", "Text").ToList();
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSal_List.rdlc");
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

            List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSal_ExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalList {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpSal(string costcenter, int year)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbEst = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);


            bgt_expenses_Salary[] bgt_expenses_Salary = dbEst.bgt_expenses_Salary.Where(x => x.abes_cost_center == costcenter && x.abes_budgeting_year == year && x.abes_Deleted == false).ToArray();


            //Get Syarikat
            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            //Get Budget Year
            ViewBag.year = bgt_expenses_Salary.Select(s => s.abes_budgeting_year).FirstOrDefault();

            //Get GL Pendapatan
            ViewBag.fld_GLCode = bgt_expenses_Salary.Select(s => s.abes_gl_code).FirstOrDefault();
            ViewBag.fld_GLDesc = bgt_expenses_Salary.Select(s => s.abes_gl_desc).FirstOrDefault();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;

            return PartialView(bgt_expenses_Salary);
        }

        public ActionResult ViewExpSalPDF(string year, string costcenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSal_view.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult ViewExpSalEXCEL(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSal_Excelview.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalView {0}, {1}", year, costcenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult _ExpSalSetahunRpt(bgt_expenses_Salary[] bgt_expenses_Salary, string costcenter, string year)
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
                    bgt_expenses_Salary = dbEst.bgt_expenses_Salary.Where(x => x.abes_budgeting_year.ToString() == year && x.abes_Deleted == false).OrderBy(o => o.abes_gl_code).ThenBy(x => x.abes_cost_center).ToArray();

                }
                else
                {
                    bgt_expenses_Salary = dbEst.bgt_expenses_Salary.Where(x => x.abes_cost_center == costcenter && x.abes_budgeting_year.ToString() == year && x.abes_Deleted == false).OrderBy(o => o.abes_gl_code).ToArray();
                }


                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Salary.Select(s => s.abes_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Salary.Select(s => s.abes_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Salary.Select(s => s.abes_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Salary.Where(x => x.abes_cost_center == costcenter).Select(o => o.abes_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_Salary);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpSalSetahunRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewtahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewtahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult ExpSalSetahunRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewExceltahunAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewExceltahun.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Setahun)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }

        public ActionResult _ExpSalSebulanRpt(bgt_expenses_Salary[] bgt_expenses_Salary, string costcenter, int year)
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
                    bgt_expenses_Salary = dbEst.bgt_expenses_Salary.Where(x => x.abes_budgeting_year == year && x.abes_Deleted == false).OrderBy(o => o.abes_gl_code).ToArray();

                }
                else
                {
                    bgt_expenses_Salary = dbEst.bgt_expenses_Salary.Where(x => x.abes_cost_center == costcenter && x.abes_budgeting_year == year && x.abes_Deleted == false).OrderBy(o => o.abes_gl_code).ToArray();
                }

                //Get Syarikat
                ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.year = bgt_expenses_Salary.Select(s => s.abes_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_expenses_Salary.Select(s => s.abes_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_expenses_Salary.Select(s => s.abes_gl_desc).FirstOrDefault();

                if (costcenter == "0")
                {
                    ViewBag.CostCenterS = "All";
                }
                else
                {
                    var cc_name = dbEst.bgt_expenses_Salary.Where(x => x.abes_cost_center == costcenter).Select(o => o.abes_cost_center_name).FirstOrDefault();
                    ViewBag.CostCenterS = costcenter + " - " + cc_name;
                }

                ViewBag.CostCenter = costcenter;
                ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
                return PartialView(bgt_expenses_Salary);

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult ExpSalSebulanRptPDF(string year, string costcenter, string format = "PDF")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalView {0}, {1}", year, costcenter).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }

        }
        public ActionResult ExpSalSebulanRptEXCEL(string year, string costcenter, string format = "EXCEL")
        {
            if (costcenter == "0")
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewExcelbulanAll.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalViewAll {0}", year).ToList();
                ReportDataSource rd = new ReportDataSource("DataSet1", data);
                lr.DataSources.Add(rd);

                return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
            }
            else
            {
                LocalReport lr = new LocalReport();
                string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpSalViewExcelbulan.rdlc");
                string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - View Keseluruhan(Mengikut bulan)";

                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //show error
                }

                List<bgt_expenses_SalaryView> data = dbEst.Database.SqlQuery<bgt_expenses_SalaryView>("exec sp_ExpSalView {0}, {1}", year, costcenter).ToList();
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
