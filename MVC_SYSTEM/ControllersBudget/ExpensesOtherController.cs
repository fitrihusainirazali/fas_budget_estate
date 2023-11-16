using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorize(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ExpensesOtherController : Controller
    {
        private MVC_SYSTEM_ModelsBudgetEst db = new ConnectionBudget().GetConnection();
        private MVC_SYSTEM_ModelsBudget dbc = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_MasterModels dbm = new MVC_SYSTEM_MasterModels();
        private GetConfig config = new GetConfig();
        private Screen scr = new Screen();
        private CostCenter cc = new CostCenter();
        private GL gl = new GL();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private errorlog errlog = new errorlog();
        private readonly string screenCode = "E15";

        // GET: ExpensesOther
        public ActionResult Index()
        {
            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString()
            }), "Value", "Text").ToList();
            var selectedYear = GetYears().FirstOrDefault().ToString();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Years = years;
            ViewBag.SelectedYear = selectedYear;

            return View();
        }

        public ActionResult Records(string BudgetYear, string CostCenter, int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<ExpensesOtherListViewModel>
            {
                Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        public ActionResult RecordsDetail(string BudgetYear, string CostCenter, int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<ExpensesOtherListDetailViewModel>
            {
                Content = GetRecordsDetail(budgetYear, CostCenter, null, null, page, pageSize),
                TotalRecords = GetRecordsDetail(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_RecordsDetail", records);
        }

        // GET: ExpensesOther/Details/5
        public ActionResult Details(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_others = GetRecordsDetail(year, CostCenter);

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Details", expenses_others);
        }

        // GET: ExpensesOther/Create
        public ActionResult Create()
        {
            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString()
            }), "Value", "Text").ToList();
            var selectedYear = GetYears().FirstOrDefault().ToString();
            var year = int.Parse(selectedYear);

            var costCenters = new SelectList(GetCostCenters(year).Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter.TrimStart(new char[] { '0' }),
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = selectedYear;
            ViewBag.Years = years;
            ViewBag.CostCenters = costCenters;

            return View();
        }

        // POST: ExpensesOther/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Edit", new
            {
                BudgetYear = BudgetYear,
                CostCenter = CostCenter
            });
        }

        public ActionResult CreateDetail(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var gls = new SelectList(GetGLs().Select(s => new SelectListItem
            {
                Value = s.Code.TrimStart(new char[] { '0' }),
                Text = s.Code.TrimStart(new char[] { '0' }) + " - " + s.Description
            }), "Value", "Text").ToList();

            var uoms = new SelectList(GetUOMs().Select(s => new SelectListItem
            {
                Value = s.UOM,
                Text = s.UOM
            }), "Value", "Text").ToList();

            var operations = new SelectList(GetOperations().Select(s => new SelectListItem
            {
                Value = s.Opr_Sign,
                Text = s.Opr_Sign
            }), "Value", "Text").ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
            ViewBag.GLs = gls;
            ViewBag.UOMs = uoms;
            ViewBag.Operations = operations;

            return PartialView("_CreateDetail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDetail(ExpensesOtherCreateDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GetIdentity getIdentity = new GetIdentity();

                    var resultUnit1 = 0m;
                    var resultUnit2 = 0m;
                    var resultUnit3 = 0m;
                    var resultRate = 0m;
                    var resultTotal = 0m;
                    var resultMonth1 = 0m;
                    var resultMonth2 = 0m;
                    var resultMonth3 = 0m;
                    var resultMonth4 = 0m;
                    var resultMonth5 = 0m;
                    var resultMonth6 = 0m;
                    var resultMonth7 = 0m;
                    var resultMonth8 = 0m;
                    var resultMonth9 = 0m;
                    var resultMonth10 = 0m;
                    var resultMonth11 = 0m;
                    var resultMonth12 = 0m;

                    var syarikatObj = syarikat.GetSyarikat();
                    var ladangObj = ladang.GetLadang();
                    var wilayahObj = wilayah.GetWilayah();

                    var expenses_other = new bgt_expenses_other
                    {
                        abeo_budgeting_year = model.abeo_budgeting_year,
                        abeo_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null,
                        abeo_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                        abeo_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null,
                        abeo_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                        abeo_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                        abeo_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null,
                        abeo_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                        abeo_company_category = model.abeo_company_category,
                        abeo_company_code = model.abeo_company_code,
                        abeo_company_name = model.abeo_company_name,
                        abeo_cost_center_code = model.abeo_cost_center_code,
                        abeo_cost_center_desc = model.abeo_cost_center_desc,
                        abeo_gl_expenses_code = model.abeo_gl_expenses_code,
                        abeo_gl_expenses_name = model.abeo_gl_expenses_name,
                        abeo_note = model.abeo_note,
                        abeo_unit_1 = !string.IsNullOrEmpty(model.abeo_unit_1) && decimal.TryParse(model.abeo_unit_1.Replace(",", ""), out resultUnit1) ? decimal.Parse(model.abeo_unit_1.Replace(",", "")) : (decimal?)null,
                        abeo_uom_1 = model.abeo_uom_1,
                        abeo_operation_1 = model.abeo_operation_1,
                        abeo_unit_2 = !string.IsNullOrEmpty(model.abeo_unit_2) && decimal.TryParse(model.abeo_unit_2.Replace(",", ""), out resultUnit2) ? decimal.Parse(model.abeo_unit_2.Replace(",", "")) : (decimal?)null,
                        abeo_uom_2 = model.abeo_uom_2,
                        abeo_operation_2 = model.abeo_operation_2,
                        abeo_unit_3 = !string.IsNullOrEmpty(model.abeo_unit_3) && decimal.TryParse(model.abeo_unit_3.Replace(",", ""), out resultUnit3) ? decimal.Parse(model.abeo_unit_3.Replace(",", "")) : (decimal?)null,
                        abeo_uom_3 = model.abeo_uom_3,
                        abeo_operation_3 = model.abeo_operation_3,
                        abeo_rate = !string.IsNullOrEmpty(model.abeo_rate) && decimal.TryParse(model.abeo_rate.Replace(",", ""), out resultRate) ? decimal.Parse(model.abeo_rate.Replace(",", "")) : (decimal?)null,
                        abeo_total = !string.IsNullOrEmpty(model.abeo_total) && decimal.TryParse(model.abeo_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abeo_total.Replace(",", "")) : (decimal?)null,
                        abeo_month_1 = !string.IsNullOrEmpty(model.abeo_month_1) && decimal.TryParse(model.abeo_month_1.Replace(",", ""), out resultMonth1) ? decimal.Parse(model.abeo_month_1.Replace(",", "")) : (decimal?)null,
                        abeo_month_2 = !string.IsNullOrEmpty(model.abeo_month_2) && decimal.TryParse(model.abeo_month_2.Replace(",", ""), out resultMonth2) ? decimal.Parse(model.abeo_month_2.Replace(",", "")) : (decimal?)null,
                        abeo_month_3 = !string.IsNullOrEmpty(model.abeo_month_3) && decimal.TryParse(model.abeo_month_3.Replace(",", ""), out resultMonth3) ? decimal.Parse(model.abeo_month_3.Replace(",", "")) : (decimal?)null,
                        abeo_month_4 = !string.IsNullOrEmpty(model.abeo_month_4) && decimal.TryParse(model.abeo_month_4.Replace(",", ""), out resultMonth4) ? decimal.Parse(model.abeo_month_4.Replace(",", "")) : (decimal?)null,
                        abeo_month_5 = !string.IsNullOrEmpty(model.abeo_month_5) && decimal.TryParse(model.abeo_month_5.Replace(",", ""), out resultMonth5) ? decimal.Parse(model.abeo_month_5.Replace(",", "")) : (decimal?)null,
                        abeo_month_6 = !string.IsNullOrEmpty(model.abeo_month_6) && decimal.TryParse(model.abeo_month_6.Replace(",", ""), out resultMonth6) ? decimal.Parse(model.abeo_month_6.Replace(",", "")) : (decimal?)null,
                        abeo_month_7 = !string.IsNullOrEmpty(model.abeo_month_7) && decimal.TryParse(model.abeo_month_7.Replace(",", ""), out resultMonth7) ? decimal.Parse(model.abeo_month_7.Replace(",", "")) : (decimal?)null,
                        abeo_month_8 = !string.IsNullOrEmpty(model.abeo_month_8) && decimal.TryParse(model.abeo_month_8.Replace(",", ""), out resultMonth8) ? decimal.Parse(model.abeo_month_8.Replace(",", "")) : (decimal?)null,
                        abeo_month_9 = !string.IsNullOrEmpty(model.abeo_month_9) && decimal.TryParse(model.abeo_month_9.Replace(",", ""), out resultMonth9) ? decimal.Parse(model.abeo_month_9.Replace(",", "")) : (decimal?)null,
                        abeo_month_10 = !string.IsNullOrEmpty(model.abeo_month_10) && decimal.TryParse(model.abeo_month_10.Replace(",", ""), out resultMonth10) ? decimal.Parse(model.abeo_month_10.Replace(",", "")) : (decimal?)null,
                        abeo_month_11 = !string.IsNullOrEmpty(model.abeo_month_11) && decimal.TryParse(model.abeo_month_11.Replace(",", ""), out resultMonth11) ? decimal.Parse(model.abeo_month_11.Replace(",", "")) : (decimal?)null,
                        abeo_month_12 = !string.IsNullOrEmpty(model.abeo_month_12) && decimal.TryParse(model.abeo_month_12.Replace(",", ""), out resultMonth12) ? decimal.Parse(model.abeo_month_12.Replace(",", "")) : (decimal?)null,
                        abeo_proration = model.abeo_proration,
                        abeo_status = "DRAFT",
                        abeo_created_by = getIdentity.ID(User.Identity.Name),
                        last_modified = DateTime.Now
                    };
                    db.bgt_expenses_others.Add(expenses_other);
                    await db.SaveChangesAsync();

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
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesOther",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abeo_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abeo_cost_center_code
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
                return Json(new
                {
                    success = false
                });
            }
        }

        // GET: ExpensesOther/Edit/5
        public ActionResult Edit(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return RedirectToAction("Create");
            }

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.COstCenterDesc = cc.GetCostCenterDesc(CostCenter);

            return View();
        }

        // POST: ExpensesOther/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "abeo_id,abeo_revision,abeo_budgeting_year,abeo_syarikat_id,abeo_syarikat_name,abeo_ladang_id,abeo_ladang_code,abeo_ladang_name,abeo_wilayah_id,abeo_wilayah_name,abeo_company_category,abeo_company_code,abeo_company_name,abeo_cost_center_code,abeo_cost_center_desc,abeo_gl_expenses_code,abeo_gl_expenses_name,abeo_note,abeo_unit_1,abeo_uom_1,abeo_operation_1,abeo_unit_2,abeo_uom_2,abeo_operation_2,abeo_unit_3,abeo_uom_3,abeo_operation_3,abeo_rate,abeo_total,abeo_month_1,abeo_month_2,abeo_month_3,abeo_month_4,abeo_month_5,abeo_month_6,abeo_month_7,abeo_month_8,abeo_month_9,abeo_month_10,abeo_month_11,abeo_month_12,abeo_proration,abeo_history,abeo_status,abeo_created_by,abeo_deleted,last_modified")] bgt_expenses_other bgt_expenses_other)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(bgt_expenses_other).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(bgt_expenses_other);
        //}

        public async Task<ActionResult> EditDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expenses_other = await db.bgt_expenses_others.FindAsync(id);
            if (expenses_other == null)
            {
                return HttpNotFound();
            }

            var gls = new SelectList(GetGLs().Select(s => new SelectListItem
            {
                Value = s.Code.TrimStart(new char[] { '0' }),
                Text = s.Code.TrimStart(new char[] { '0' }) + " - " + s.Description
            }), "Value", "Text").ToList();

            var uoms = new SelectList(GetUOMs().Select(s => new SelectListItem
            {
                Value = s.UOM,
                Text = s.UOM
            }), "Value", "Text").ToList();

            var operations = new SelectList(GetOperations().Select(s => new SelectListItem
            {
                Value = s.Opr_Sign,
                Text = s.Opr_Sign
            }), "Value", "Text").ToList();

            var model = new ExpensesOtherEditDetailViewModel
            {
                abeo_id = id.Value,
                abeo_budgeting_year = expenses_other.abeo_budgeting_year,
                abeo_syarikat_id = expenses_other.abeo_syarikat_id,
                abeo_syarikat_name = expenses_other.abeo_syarikat_name,
                abeo_ladang_id = expenses_other.abeo_ladang_id,
                abeo_ladang_code = expenses_other.abeo_ladang_code,
                abeo_ladang_name = expenses_other.abeo_ladang_name,
                abeo_wilayah_id = expenses_other.abeo_wilayah_id,
                abeo_wilayah_name = expenses_other.abeo_wilayah_name,
                abeo_company_category = expenses_other.abeo_company_category,
                abeo_company_code = expenses_other.abeo_company_code,
                abeo_company_name = expenses_other.abeo_company_name,
                abeo_cost_center_code = expenses_other.abeo_cost_center_code.TrimStart(new char[] { '0' }),
                abeo_cost_center_desc = expenses_other.abeo_cost_center_desc,
                abeo_gl_expenses_code = expenses_other.abeo_gl_expenses_code.TrimStart(new char[] { '0' }),
                abeo_gl_expenses_name = expenses_other.abeo_gl_expenses_name,
                abeo_note = expenses_other.abeo_note,
                abeo_unit_1 = expenses_other.abeo_unit_1.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_unit_1) : string.Empty,
                abeo_uom_1 = expenses_other.abeo_uom_1,
                abeo_operation_1 = expenses_other.abeo_operation_1,
                abeo_unit_2 = expenses_other.abeo_unit_2.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_unit_2) : string.Empty,
                abeo_uom_2 = expenses_other.abeo_uom_2,
                abeo_operation_2 = expenses_other.abeo_operation_2,
                abeo_unit_3 = expenses_other.abeo_unit_3.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_unit_3) : string.Empty,
                abeo_uom_3 = expenses_other.abeo_uom_3,
                abeo_operation_3 = expenses_other.abeo_operation_3,
                abeo_rate = expenses_other.abeo_rate.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_rate) : string.Empty,
                abeo_total = expenses_other.abeo_total.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_total) : string.Empty,
                abeo_month_1 = expenses_other.abeo_month_1.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_1) : string.Empty,
                abeo_month_2 = expenses_other.abeo_month_2.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_2) : string.Empty,
                abeo_month_3 = expenses_other.abeo_month_3.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_3) : string.Empty,
                abeo_month_4 = expenses_other.abeo_month_4.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_4) : string.Empty,
                abeo_month_5 = expenses_other.abeo_month_5.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_5) : string.Empty,
                abeo_month_6 = expenses_other.abeo_month_6.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_6) : string.Empty,
                abeo_month_7 = expenses_other.abeo_month_7.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_7) : string.Empty,
                abeo_month_8 = expenses_other.abeo_month_8.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_8) : string.Empty,
                abeo_month_9 = expenses_other.abeo_month_9.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_9) : string.Empty,
                abeo_month_10 = expenses_other.abeo_month_10.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_10) : string.Empty,
                abeo_month_11 = expenses_other.abeo_month_11.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_11) : string.Empty,
                abeo_month_12 = expenses_other.abeo_month_12.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_month_12) : string.Empty,
                abeo_proration = expenses_other.abeo_proration,
                abeo_history = expenses_other.abeo_history,
                abeo_status = expenses_other.abeo_status,
                abeo_created_by = expenses_other.abeo_created_by
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.GLs = gls;
            ViewBag.UOMs = uoms;
            ViewBag.Operations = operations;

            return PartialView("_EditDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDetail(int? id, ExpensesOtherEditDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var resultUnit1 = 0m;
                    var resultUnit2 = 0m;
                    var resultUnit3 = 0m;
                    var resultRate = 0m;
                    var resultTotal = 0m;
                    var resultMonth1 = 0m;
                    var resultMonth2 = 0m;
                    var resultMonth3 = 0m;
                    var resultMonth4 = 0m;
                    var resultMonth5 = 0m;
                    var resultMonth6 = 0m;
                    var resultMonth7 = 0m;
                    var resultMonth8 = 0m;
                    var resultMonth9 = 0m;
                    var resultMonth10 = 0m;
                    var resultMonth11 = 0m;
                    var resultMonth12 = 0m;

                    var syarikatObj = syarikat.GetSyarikat();
                    var ladangObj = ladang.GetLadang();
                    var wilayahObj = wilayah.GetWilayah();

                    var expenses_other = db.bgt_expenses_others.Find(id);
                    expenses_other.abeo_budgeting_year = model.abeo_budgeting_year;
                    expenses_other.abeo_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null;
                    expenses_other.abeo_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                    expenses_other.abeo_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null;
                    expenses_other.abeo_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                    expenses_other.abeo_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                    expenses_other.abeo_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null;
                    expenses_other.abeo_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null;
                    expenses_other.abeo_cost_center_code = model.abeo_cost_center_code;
                    expenses_other.abeo_cost_center_desc = model.abeo_cost_center_desc;
                    expenses_other.abeo_gl_expenses_code = model.abeo_gl_expenses_code;
                    expenses_other.abeo_gl_expenses_name = model.abeo_gl_expenses_name;
                    expenses_other.abeo_note = model.abeo_note;
                    expenses_other.abeo_unit_1 = !string.IsNullOrEmpty(model.abeo_unit_1) && decimal.TryParse(model.abeo_unit_1.Replace(",", ""), out resultUnit1) ? decimal.Parse(model.abeo_unit_1.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_uom_1 = model.abeo_uom_1;
                    expenses_other.abeo_operation_1 = model.abeo_operation_1;
                    expenses_other.abeo_unit_2 = !string.IsNullOrEmpty(model.abeo_unit_2) && decimal.TryParse(model.abeo_unit_2.Replace(",", ""), out resultUnit2) ? decimal.Parse(model.abeo_unit_2.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_uom_2 = model.abeo_uom_2;
                    expenses_other.abeo_operation_2 = model.abeo_operation_2;
                    expenses_other.abeo_unit_3 = !string.IsNullOrEmpty(model.abeo_unit_3) && decimal.TryParse(model.abeo_unit_3.Replace(",", ""), out resultUnit3) ? decimal.Parse(model.abeo_unit_3.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_uom_3 = model.abeo_uom_3;
                    expenses_other.abeo_operation_3 = model.abeo_operation_3;
                    expenses_other.abeo_rate = !string.IsNullOrEmpty(model.abeo_rate) && decimal.TryParse(model.abeo_rate.Replace(",", ""), out resultRate) ? decimal.Parse(model.abeo_rate.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_total = !string.IsNullOrEmpty(model.abeo_total) && decimal.TryParse(model.abeo_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abeo_total.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_1 = !string.IsNullOrEmpty(model.abeo_month_1) && decimal.TryParse(model.abeo_month_1.Replace(",", ""), out resultMonth1) ? decimal.Parse(model.abeo_month_1.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_2 = !string.IsNullOrEmpty(model.abeo_month_2) && decimal.TryParse(model.abeo_month_2.Replace(",", ""), out resultMonth2) ? decimal.Parse(model.abeo_month_2.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_3 = !string.IsNullOrEmpty(model.abeo_month_3) && decimal.TryParse(model.abeo_month_3.Replace(",", ""), out resultMonth3) ? decimal.Parse(model.abeo_month_3.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_4 = !string.IsNullOrEmpty(model.abeo_month_4) && decimal.TryParse(model.abeo_month_4.Replace(",", ""), out resultMonth4) ? decimal.Parse(model.abeo_month_4.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_5 = !string.IsNullOrEmpty(model.abeo_month_5) && decimal.TryParse(model.abeo_month_5.Replace(",", ""), out resultMonth5) ? decimal.Parse(model.abeo_month_5.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_6 = !string.IsNullOrEmpty(model.abeo_month_6) && decimal.TryParse(model.abeo_month_6.Replace(",", ""), out resultMonth6) ? decimal.Parse(model.abeo_month_6.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_7 = !string.IsNullOrEmpty(model.abeo_month_7) && decimal.TryParse(model.abeo_month_7.Replace(",", ""), out resultMonth7) ? decimal.Parse(model.abeo_month_7.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_8 = !string.IsNullOrEmpty(model.abeo_month_8) && decimal.TryParse(model.abeo_month_8.Replace(",", ""), out resultMonth8) ? decimal.Parse(model.abeo_month_8.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_9 = !string.IsNullOrEmpty(model.abeo_month_9) && decimal.TryParse(model.abeo_month_9.Replace(",", ""), out resultMonth9) ? decimal.Parse(model.abeo_month_9.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_10 = !string.IsNullOrEmpty(model.abeo_month_10) && decimal.TryParse(model.abeo_month_10.Replace(",", ""), out resultMonth10) ? decimal.Parse(model.abeo_month_10.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_11 = !string.IsNullOrEmpty(model.abeo_month_11) && decimal.TryParse(model.abeo_month_11.Replace(",", ""), out resultMonth11) ? decimal.Parse(model.abeo_month_11.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_month_12 = !string.IsNullOrEmpty(model.abeo_month_12) && decimal.TryParse(model.abeo_month_12.Replace(",", ""), out resultMonth12) ? decimal.Parse(model.abeo_month_12.Replace(",", "")) : (decimal?)null;
                    expenses_other.abeo_proration = model.abeo_proration;
                    expenses_other.last_modified = DateTime.Now;

                    db.Entry(expenses_other).State = EntityState.Modified;
                    await db.SaveChangesAsync();

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
                        msg = GlobalResEstate.msgUpdate,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesOther",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abeo_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abeo_cost_center_code
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
                return Json(new
                {
                    success = false,
                });
            }
        }

        // GET: ExpensesOther/Delete/5
        public async Task<ActionResult> Delete(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_other = await db.bgt_expenses_others.Where(o => o.abeo_budgeting_year == year && o.abeo_cost_center_code.Equals(CostCenter) && !o.abeo_deleted).FirstOrDefaultAsync();
            if (expenses_other == null)
            {
                return HttpNotFound();
            }
            var total_amount = db.bgt_expenses_others
                .Where(o => o.abeo_budgeting_year == expenses_other.abeo_budgeting_year && o.abeo_cost_center_code.Equals(expenses_other.abeo_cost_center_code) && !o.abeo_deleted)
                .Sum(o => o.abeo_total);
            var model = new ExpensesOtherDeleteViewModel
            {
                BudgetYear = expenses_other.abeo_budgeting_year,
                CostCenterCode = expenses_other.abeo_cost_center_code,
                CostCenterDesc = expenses_other.abeo_cost_center_desc,
                Total = string.Format("{0:#,##0.00}", total_amount)
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Delete", model);
        }

        // POST: ExpensesOther/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string BudgetYear, string CostCenter)
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

                var year = int.Parse(BudgetYear);
                var expenses_others = db.bgt_expenses_others
                    .Where(o => o.abeo_budgeting_year == year && o.abeo_cost_center_code.Equals(CostCenter) && !o.abeo_deleted)
                    .ToList();

                if (expenses_others.Count() > 0)
                {
                    foreach (var item in expenses_others)
                    {
                        //item.abeo_deleted = true;
                        //item.last_modified = DateTime.Now;
                        //db.Entry(item).State = EntityState.Modified;
                        db.bgt_expenses_others.Remove(item);
                    }
                    await db.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgDelete2,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesOther",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = BudgetYear
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesOther",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = BudgetYear
                    });
                }
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

        public async Task<ActionResult> DeleteDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var expenses_other = await db.bgt_expenses_others.FindAsync(id);
            if (expenses_other == null)
            {
                return HttpNotFound();
            }

            var model = new ExpensesOtherDeleteDetailViewModel
            {
                abeo_id = expenses_other.abeo_id,
                abeo_budgeting_year = expenses_other.abeo_budgeting_year,
                abeo_syarikat_id = expenses_other.abeo_syarikat_id,
                abeo_syarikat_name = expenses_other.abeo_syarikat_name,
                abeo_ladang_id = expenses_other.abeo_ladang_id,
                abeo_ladang_code = expenses_other.abeo_ladang_code,
                abeo_ladang_name = expenses_other.abeo_ladang_name,
                abeo_wilayah_id = expenses_other.abeo_wilayah_id,
                abeo_wilayah_name = expenses_other.abeo_wilayah_name,
                abeo_company_category = expenses_other.abeo_company_category,
                abeo_company_code = expenses_other.abeo_company_code,
                abeo_company_name = expenses_other.abeo_company_name,
                abeo_cost_center_code = expenses_other.abeo_cost_center_code,
                abeo_cost_center_desc = expenses_other.abeo_cost_center_desc,
                abeo_gl_expenses_code = expenses_other.abeo_gl_expenses_code,
                abeo_gl_expenses_name = expenses_other.abeo_gl_expenses_name,
                abeo_note = expenses_other.abeo_note,
                abeo_unit_1 = expenses_other.abeo_unit_1,
                abeo_uom_1 = expenses_other.abeo_uom_1,
                abeo_operation_1 = expenses_other.abeo_operation_1,
                abeo_unit_2 = expenses_other.abeo_unit_2,
                abeo_uom_2 = expenses_other.abeo_uom_2,
                abeo_operation_2 = expenses_other.abeo_operation_2,
                abeo_unit_3 = expenses_other.abeo_unit_3,
                abeo_uom_3 = expenses_other.abeo_uom_3,
                abeo_operation_3 = expenses_other.abeo_operation_3,
                abeo_rate = expenses_other.abeo_rate,
                abeo_total = expenses_other.abeo_total.HasValue ? string.Format("{0:#,##0.00}", expenses_other.abeo_total) : string.Empty,
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            return PartialView("_DeleteDetail", model);
        }

        [HttpPost, ActionName("DeleteDetail")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteDetailConfirmed(int id)
        {
            try
            {
                var expenses_other = db.bgt_expenses_others.Find(id);
                //expenses_other.abeo_deleted = true;
                //expenses_other.last_modified = DateTime.Now;
                //db.Entry(expenses_other).State = EntityState.Modified;
                db.bgt_expenses_others.Remove(expenses_other);
                await db.SaveChangesAsync();

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
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    div = "RecordsPartial",
                    rooturl = domain,
                    controller = "ExpensesOther",
                    action = "RecordsDetail",
                    paramName1 = "BudgetYear",
                    paramValue1 = expenses_other.abeo_budgeting_year.ToString(),
                    paramName2 = "CostCenter",
                    paramValue2 = expenses_other.abeo_cost_center_code
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

        public ActionResult PrintList(string BudgetYear, string CostCenter, string Format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesOthersList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesOtherListViewModel> data = db.Database.SqlQuery<ExpensesOtherListViewModel>("exec sp_BudgetExpensesOthersList {0}, {1}, {2}, {3}, {4}", BudgetYear, CostCenter, syarikat.GetSyarikatID().ToString(), ladang.GetLadangID().ToString(), wilayah.GetWilayahID().ToString()).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintView(string BudgetYear, string CostCenter, string Format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesOthersView.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (Format.ToLower().Contains("excel"))
            {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesOthersViewExcel.rdlc");
            }

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesOtherListDetailViewModel> data = db.Database.SqlQuery<ExpensesOtherListDetailViewModel>("exec sp_BudgetExpensesOthersView {0}, {1}, {2}, {3}, {4}", BudgetYear, CostCenter, syarikat.GetSyarikatID().ToString(), ladang.GetLadangID().ToString(), wilayah.GetWilayahID().ToString()).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Methods
        private List<int> GetYears()
        {
            return dbc.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }

        private List<tbl_SAPCCPUP> GetCostCenters(int budgetYear)
        {
            var expenses_others = GetRecords(budgetYear);
            var excludeCostCenters = new List<string>();

            for (int i = 0; i < expenses_others.Count(); i++)
            {
                excludeCostCenters.Add(expenses_others[i].CostCenterCode.PadLeft(10, '0'));
            }

            return cc.GetCostCenters()
                .Where(c => !excludeCostCenters.Contains(c.fld_CostCenter))
                .OrderBy(c => c.fld_CostCenter)
                .ToList();
        }

        private List<GLListViewModel> GetGLs(bool excludeOthers = true)
        {
            var screen = scr.GetScreen(screenCode);
            var gls = new List<GLListViewModel>();
            var excludeGLs = new List<string>();

            if (excludeOthers)
            {
                var glsI1 = gl.GetGLs(scr.GetScreen("I1").ScrID);
                var glsI2 = gl.GetGLs(scr.GetScreen("I2").ScrID);
                var glsI3 = gl.GetGLs(scr.GetScreen("I3").ScrID);
                var glsE1 = gl.GetGLs(scr.GetScreen("E1").ScrID);
                var glsE2 = gl.GetGLs(scr.GetScreen("E2").ScrID);
                var glsE3 = gl.GetGLs(scr.GetScreen("E3").ScrID);
                var glsE4 = gl.GetGLs(scr.GetScreen("E4").ScrID);
                var glsE5 = gl.GetGLs(scr.GetScreen("E5").ScrID);
                var glsE6 = gl.GetGLs(scr.GetScreen("E6").ScrID);
                var glsE7 = gl.GetGLs(scr.GetScreen("E7").ScrID);
                var glsE8 = gl.GetGLs(scr.GetScreen("E8").ScrID);
                var glsE9 = gl.GetGLs(scr.GetScreen("E9").ScrID);
                var glsE10 = gl.GetGLs(scr.GetScreen("E10").ScrID);
                var glsE11 = gl.GetGLs(scr.GetScreen("E11").ScrID);
                var glsE12 = gl.GetGLs(scr.GetScreen("E12").ScrID);
                var glsE13 = gl.GetGLs(scr.GetScreen("E13").ScrID);
                var glsE14 = gl.GetGLs(scr.GetScreen("E14").ScrID);

                for (int i = 0; i < glsI1.Count; i++)
                    excludeGLs.Add(glsI1[i].fld_GLCode);

                for (int i = 0; i < glsI2.Count; i++)
                    excludeGLs.Add(glsI2[i].fld_GLCode);

                for (int i = 0; i < glsI3.Count; i++)
                    excludeGLs.Add(glsI3[i].fld_GLCode);

                for (int i = 0; i < glsE1.Count; i++)
                    excludeGLs.Add(glsE1[i].fld_GLCode);

                for (int i = 0; i < glsE2.Count; i++)
                    excludeGLs.Add(glsE2[i].fld_GLCode);

                for (int i = 0; i < glsE3.Count; i++)
                    excludeGLs.Add(glsE3[i].fld_GLCode);

                for (int i = 0; i < glsE4.Count; i++)
                    excludeGLs.Add(glsE4[i].fld_GLCode);

                for (int i = 0; i < glsE5.Count; i++)
                    excludeGLs.Add(glsE5[i].fld_GLCode);

                for (int i = 0; i < glsE6.Count; i++)
                    excludeGLs.Add(glsE6[i].fld_GLCode);

                for (int i = 0; i < glsE7.Count; i++)
                    excludeGLs.Add(glsE7[i].fld_GLCode);

                for (int i = 0; i < glsE8.Count; i++)
                    excludeGLs.Add(glsE8[i].fld_GLCode);

                for (int i = 0; i < glsE9.Count; i++)
                    excludeGLs.Add(glsE9[i].fld_GLCode);

                for (int i = 0; i < glsE10.Count; i++)
                    excludeGLs.Add(glsE10[i].fld_GLCode);

                for (int i = 0; i < glsE11.Count; i++)
                    excludeGLs.Add(glsE11[i].fld_GLCode);

                for (int i = 0; i < glsE12.Count; i++)
                    excludeGLs.Add(glsE12[i].fld_GLCode);

                for (int i = 0; i < glsE13.Count; i++)
                    excludeGLs.Add(glsE13[i].fld_GLCode);

                for (int i = 0; i < glsE14.Count; i++)
                    excludeGLs.Add(glsE14[i].fld_GLCode);

                gls = dbm.tbl_SAPGLPUP
                    .Where(g => !excludeGLs.Contains(g.fld_GLCode))
                    .OrderBy(g => g.fld_GLCode)
                    .Select(g => new GLListViewModel { Id = g.fld_ID, Code = g.fld_GLCode, Description = g.fld_GLDesc })
                    .ToList();
            }
            else
            {
                gls = gl.GetGLs(screen.ScrID)
                    .Select(g => new GLListViewModel { Id = g.fld_ID, Code = g.fld_GLCode, Description = g.fld_GLDesc })
                    .ToList();
            }

            return gls;
        }

        private List<bgt_UOM> GetUOMs()
        {
            return dbc.bgt_UOM.OrderBy(u => u.UOM).ToList();
        }

        private List<bgt_Operation> GetOperations()
        {
            return dbc.bgt_Operations.ToList();
        }

        private List<ExpensesOtherListViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in db.bgt_expenses_others
                        where !o.abeo_deleted && o.abeo_syarikat_id == syarikatId && o.abeo_ladang_id == ladangId && o.abeo_wilayah_id == wilayahId
                        group o by new { o.abeo_budgeting_year, o.abeo_cost_center_code, o.abeo_cost_center_desc } into g
                        select new
                        {
                            BudgetYear = g.Key.abeo_budgeting_year,
                            CostCenterCode = g.Key.abeo_cost_center_code,
                            CostCenterDesc = g.Key.abeo_cost_center_desc,
                            Total = g.Sum(o => o.abeo_total)
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
                          select new ExpensesOtherListViewModel
                          {
                              BudgetYear = q.BudgetYear,
                              CostCenterCode = q.CostCenterCode.TrimStart(new char[] { '0' }),
                              CostCenterDesc = q.CostCenterDesc,
                              Total = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        private List<ExpensesOtherListDetailViewModel> GetRecordsDetail(int? budgeting_year = null, string cost_center = null, string gl = null, int? excludeId = null, int? page = null, int? pageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in db.bgt_expenses_others
                        where !o.abeo_deleted && o.abeo_syarikat_id == syarikatId && o.abeo_ladang_id == ladangId && o.abeo_wilayah_id == wilayahId
                        select o;

            if (budgeting_year.HasValue)
                query = query.Where(q => q.abeo_budgeting_year == budgeting_year);
            if (!string.IsNullOrEmpty(cost_center))
                query = query.Where(q => q.abeo_cost_center_code.Contains(cost_center) || q.abeo_cost_center_desc.Contains(cost_center));
            if (!string.IsNullOrEmpty(gl))
                query = query.Where(q => q.abeo_gl_expenses_code.Contains(gl) || q.abeo_gl_expenses_name.Contains(gl));
            if (excludeId.HasValue)
                query = query.Where(q => q.abeo_id != excludeId);

            var query2 = query.AsQueryable();

            if (page.HasValue && pageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abeo_budgeting_year)
                    .ThenBy(q => q.abeo_cost_center_code)
                    .ThenBy(q => q.abeo_gl_expenses_code)
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abeo_budgeting_year)
                    .ThenBy(q => q.abeo_cost_center_code)
                    .ThenBy(q => q.abeo_gl_expenses_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesOtherListDetailViewModel
                          {
                              abeo_id = q.abeo_id,
                              abeo_budgeting_year = q.abeo_budgeting_year,
                              abeo_syarikat_id = q.abeo_syarikat_id,
                              abeo_syarikat_name = q.abeo_syarikat_name,
                              abeo_ladang_id = q.abeo_ladang_id,
                              abeo_ladang_code = q.abeo_ladang_code,
                              abeo_ladang_name = q.abeo_ladang_name,
                              abeo_wilayah_id = q.abeo_wilayah_id,
                              abeo_wilayah_name = q.abeo_wilayah_name,
                              abeo_company_category = q.abeo_company_category,
                              abeo_company_code = q.abeo_company_code,
                              abeo_company_name = q.abeo_company_name,
                              abeo_cost_center_code = q.abeo_cost_center_code.TrimStart(new char[] { '0' }),
                              abeo_cost_center_desc = q.abeo_cost_center_desc,
                              abeo_gl_expenses_code = q.abeo_gl_expenses_code.TrimStart(new char[] { '0' }),
                              abeo_gl_expenses_name = q.abeo_gl_expenses_name,
                              abeo_note = q.abeo_note,
                              abeo_unit_1 = q.abeo_unit_1,
                              abeo_uom_1 = q.abeo_uom_1,
                              abeo_operation_1 = q.abeo_operation_1,
                              abeo_unit_2 = q.abeo_unit_2,
                              abeo_uom_2 = q.abeo_uom_2,
                              abeo_operation_2 = q.abeo_operation_2,
                              abeo_unit_3 = q.abeo_unit_3,
                              abeo_uom_3 = q.abeo_uom_3,
                              abeo_operation_3 = q.abeo_operation_3,
                              abeo_rate = q.abeo_rate,
                              abeo_total = q.abeo_total,
                              abeo_month_1 = q.abeo_month_1,
                              abeo_month_2 = q.abeo_month_2,
                              abeo_month_3 = q.abeo_month_3,
                              abeo_month_4 = q.abeo_month_4,
                              abeo_month_5 = q.abeo_month_5,
                              abeo_month_6 = q.abeo_month_6,
                              abeo_month_7 = q.abeo_month_7,
                              abeo_month_8 = q.abeo_month_8,
                              abeo_month_9 = q.abeo_month_9,
                              abeo_month_10 = q.abeo_month_10,
                              abeo_month_11 = q.abeo_month_11,
                              abeo_month_12 = q.abeo_month_12,
                              abeo_proration = q.abeo_proration,
                              abeo_history = q.abeo_history,
                              abeo_status = q.abeo_status,
                              abeo_created_by = q.abeo_created_by
                          }).Distinct();

            return query3.ToList();
        }
        #endregion
    }
}
