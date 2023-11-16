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
using MVC_SYSTEM.ModelsBudget.ViewModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.MasterModels;
using Microsoft.Reporting.WebForms;
using System.IO;
using MVC_SYSTEM.Attributes;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorize(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ExpensesCapitalController : Controller
    {
        private MVC_SYSTEM_ModelsBudgetEst db = new ConnectionBudget().GetConnection();
        private MVC_SYSTEM_ModelsBudget dbc = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_MasterModels dbm = new MVC_SYSTEM_MasterModels();
        private GetConfig GetConfig = new GetConfig();
        private Screen scr = new Screen();
        private CostCenter cc = new CostCenter();
        private GL gl = new GL();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private errorlog errlog = new errorlog();
        private readonly string screenCode = "E9";

        // GET: ExpensesCapital
        public ActionResult Index()
        {
            var screen = scr.GetScreen(screenCode);

            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString()
            }), "Value", "Text").ToList();
            var selectedYear = GetYears().FirstOrDefault().ToString();

            ViewBag.Screen = screen;
            ViewBag.Years = years;
            ViewBag.SelectedYear = selectedYear;

            return View();
        }

        public ActionResult Records(string BudgetYear, string CostCenter, int page = 1)
        {
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<ExpensesCapitalListViewModel>
            {
                Content = GetRecordsIndex(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecordsIndex(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        public ActionResult RecordsGL(string BudgetYear, string CostCenter, int page = 1)
        {
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<bgt_expenses_capital>
            {
                Content = GetRecords(budgetYear, CostCenter, null, null, page, pageSize),
                TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_RecordsGL", records);
        }

        // GET: ExpensesCapital/Details/5
        public ActionResult Details(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_capitals = GetRecords(year, CostCenter);

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Details", expenses_capitals);
        }

        // GET: ExpensesCapital/Create
        public ActionResult Create()
        {
            var screen = scr.GetScreen(screenCode);

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

            ViewBag.Title = screen.ScrNameLongDesc;
            ViewBag.Year = selectedYear;
            ViewBag.Years = years;
            ViewBag.CostCenters = costCenters;

            return View();
        }

        // POST: ExpensesCapital/Create
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
            var gls = new SelectList(GetGLs(year, CostCenter).Select(s => new SelectListItem
            {
                Value = s.Code,
                Text = s.Code + " - " + s.Description
            }), "Value", "Text").ToList();

            var assetCategories = new SelectList(GetAssetCategories().Select(s => new SelectListItem
            {
                Value = s.ast_ID.ToString(),
                Text = s.ast_ID + " - " + s.ast_Category
            }), "Value", "Text").ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
            ViewBag.GLs = gls;
            ViewBag.AssetCategories = assetCategories;

            return PartialView("_CreateDetail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDetail(ExpensesCapitalCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GetIdentity getIdentity = new GetIdentity();

                    var resultUnit1 = 0m;
                    var resultRate = 0m;
                    var resultQuantity1 = 0m;
                    var resultQuantity2 = 0m;
                    var resultQuantity3 = 0m;
                    var resultQuantity4 = 0m;
                    var resultQuantity5 = 0m;
                    var resultQuantity6 = 0m;
                    var resultQuantity7 = 0m;
                    var resultQuantity8 = 0m;
                    var resultQuantity9 = 0m;
                    var resultQuantity10 = 0m;
                    var resultQuantity11 = 0m;
                    var resultQuantity12 = 0m;
                    var resultAmount1 = 0m;
                    var resultAmount2 = 0m;
                    var resultAmount3 = 0m;
                    var resultAmount4 = 0m;
                    var resultAmount5 = 0m;
                    var resultAmount6 = 0m;
                    var resultAmount7 = 0m;
                    var resultAmount8 = 0m;
                    var resultAmount9 = 0m;
                    var resultAmount10 = 0m;
                    var resultAmount11 = 0m;
                    var resultAmount12 = 0m;
                    var resultTotal = 0m;
                    var resultTotalQty = 0m;

                    var syarikatObj = syarikat.GetSyarikat();
                    var ladangObj = ladang.GetLadang();
                    var wilayahObj = wilayah.GetWilayah();

                    var expenses_capital = new bgt_expenses_capital
                    {
                        abce_budgeting_year = model.abce_budgeting_year,
                        abce_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null,
                        abce_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                        abce_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null,
                        abce_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                        abce_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                        abce_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null,
                        abce_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                        abce_cost_center_code = model.abce_cost_center_code,
                        abce_cost_center_desc = model.abce_cost_center_desc,
                        abce_gl_code = model.abce_gl_code,
                        abce_gl_name = model.abce_gl_name,
                        abce_desc = model.abce_desc,
                        abce_jenis_code = model.abce_jenis_code,
                        abce_jenis_name = model.abce_jenis_name,
                        abce_reason = model.abce_reason,
                        abce_material_details = model.abce_material_details,
                        abce_unit_1 = !string.IsNullOrEmpty(model.abce_unit_1) && decimal.TryParse(model.abce_unit_1.Replace(",", ""), out resultUnit1) ? decimal.Parse(model.abce_unit_1.Replace(",", "")) : (decimal?)null,
                        abce_uom_1 = model.abce_uom_1,
                        abce_rate = !string.IsNullOrEmpty(model.abce_rate) && decimal.TryParse(model.abce_rate.Replace(",", ""), out resultRate) ? decimal.Parse(model.abce_rate.Replace(",", "")) : (decimal?)null,
                        abce_quantity_1 = !string.IsNullOrEmpty(model.abce_quantity_1) && decimal.TryParse(model.abce_quantity_1.Replace(",", ""), out resultQuantity1) ? decimal.Parse(model.abce_quantity_1.Replace(",", "")) : (decimal?)null,
                        abce_quantity_2 = !string.IsNullOrEmpty(model.abce_quantity_2) && decimal.TryParse(model.abce_quantity_2.Replace(",", ""), out resultQuantity2) ? decimal.Parse(model.abce_quantity_2.Replace(",", "")) : (decimal?)null,
                        abce_quantity_3 = !string.IsNullOrEmpty(model.abce_quantity_3) && decimal.TryParse(model.abce_quantity_3.Replace(",", ""), out resultQuantity3) ? decimal.Parse(model.abce_quantity_3.Replace(",", "")) : (decimal?)null,
                        abce_quantity_4 = !string.IsNullOrEmpty(model.abce_quantity_4) && decimal.TryParse(model.abce_quantity_4.Replace(",", ""), out resultQuantity4) ? decimal.Parse(model.abce_quantity_4.Replace(",", "")) : (decimal?)null,
                        abce_quantity_5 = !string.IsNullOrEmpty(model.abce_quantity_5) && decimal.TryParse(model.abce_quantity_5.Replace(",", ""), out resultQuantity5) ? decimal.Parse(model.abce_quantity_5.Replace(",", "")) : (decimal?)null,
                        abce_quantity_6 = !string.IsNullOrEmpty(model.abce_quantity_6) && decimal.TryParse(model.abce_quantity_6.Replace(",", ""), out resultQuantity6) ? decimal.Parse(model.abce_quantity_6.Replace(",", "")) : (decimal?)null,
                        abce_quantity_7 = !string.IsNullOrEmpty(model.abce_quantity_7) && decimal.TryParse(model.abce_quantity_7.Replace(",", ""), out resultQuantity7) ? decimal.Parse(model.abce_quantity_7.Replace(",", "")) : (decimal?)null,
                        abce_quantity_8 = !string.IsNullOrEmpty(model.abce_quantity_8) && decimal.TryParse(model.abce_quantity_8.Replace(",", ""), out resultQuantity8) ? decimal.Parse(model.abce_quantity_8.Replace(",", "")) : (decimal?)null,
                        abce_quantity_9 = !string.IsNullOrEmpty(model.abce_quantity_9) && decimal.TryParse(model.abce_quantity_9.Replace(",", ""), out resultQuantity9) ? decimal.Parse(model.abce_quantity_9.Replace(",", "")) : (decimal?)null,
                        abce_quantity_10 = !string.IsNullOrEmpty(model.abce_quantity_10) && decimal.TryParse(model.abce_quantity_10.Replace(",", ""), out resultQuantity10) ? decimal.Parse(model.abce_quantity_10.Replace(",", "")) : (decimal?)null,
                        abce_quantity_11 = !string.IsNullOrEmpty(model.abce_quantity_11) && decimal.TryParse(model.abce_quantity_11.Replace(",", ""), out resultQuantity11) ? decimal.Parse(model.abce_quantity_11.Replace(",", "")) : (decimal?)null,
                        abce_quantity_12 = !string.IsNullOrEmpty(model.abce_quantity_12) && decimal.TryParse(model.abce_quantity_12.Replace(",", ""), out resultQuantity12) ? decimal.Parse(model.abce_quantity_12.Replace(",", "")) : (decimal?)null,
                        abce_amount_1 = !string.IsNullOrEmpty(model.abce_amount_1) && decimal.TryParse(model.abce_amount_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abce_amount_1.Replace(",", "")) : (decimal?)null,
                        abce_amount_2 = !string.IsNullOrEmpty(model.abce_amount_2) && decimal.TryParse(model.abce_amount_2.Replace(",", ""), out resultAmount2) ? decimal.Parse(model.abce_amount_2.Replace(",", "")) : (decimal?)null,
                        abce_amount_3 = !string.IsNullOrEmpty(model.abce_amount_3) && decimal.TryParse(model.abce_amount_3.Replace(",", ""), out resultAmount3) ? decimal.Parse(model.abce_amount_3.Replace(",", "")) : (decimal?)null,
                        abce_amount_4 = !string.IsNullOrEmpty(model.abce_amount_4) && decimal.TryParse(model.abce_amount_4.Replace(",", ""), out resultAmount4) ? decimal.Parse(model.abce_amount_4.Replace(",", "")) : (decimal?)null,
                        abce_amount_5 = !string.IsNullOrEmpty(model.abce_amount_5) && decimal.TryParse(model.abce_amount_5.Replace(",", ""), out resultAmount5) ? decimal.Parse(model.abce_amount_5.Replace(",", "")) : (decimal?)null,
                        abce_amount_6 = !string.IsNullOrEmpty(model.abce_amount_6) && decimal.TryParse(model.abce_amount_6.Replace(",", ""), out resultAmount6) ? decimal.Parse(model.abce_amount_6.Replace(",", "")) : (decimal?)null,
                        abce_amount_7 = !string.IsNullOrEmpty(model.abce_amount_7) && decimal.TryParse(model.abce_amount_7.Replace(",", ""), out resultAmount7) ? decimal.Parse(model.abce_amount_7.Replace(",", "")) : (decimal?)null,
                        abce_amount_8 = !string.IsNullOrEmpty(model.abce_amount_8) && decimal.TryParse(model.abce_amount_8.Replace(",", ""), out resultAmount8) ? decimal.Parse(model.abce_amount_8.Replace(",", "")) : (decimal?)null,
                        abce_amount_9 = !string.IsNullOrEmpty(model.abce_amount_9) && decimal.TryParse(model.abce_amount_9.Replace(",", ""), out resultAmount9) ? decimal.Parse(model.abce_amount_9.Replace(",", "")) : (decimal?)null,
                        abce_amount_10 = !string.IsNullOrEmpty(model.abce_amount_10) && decimal.TryParse(model.abce_amount_10.Replace(",", ""), out resultAmount10) ? decimal.Parse(model.abce_amount_10.Replace(",", "")) : (decimal?)null,
                        abce_amount_11 = !string.IsNullOrEmpty(model.abce_amount_11) && decimal.TryParse(model.abce_amount_11.Replace(",", ""), out resultAmount11) ? decimal.Parse(model.abce_amount_11.Replace(",", "")) : (decimal?)null,
                        abce_amount_12 = !string.IsNullOrEmpty(model.abce_amount_12) && decimal.TryParse(model.abce_amount_12.Replace(",", ""), out resultAmount12) ? decimal.Parse(model.abce_amount_12.Replace(",", "")) : (decimal?)null,
                        abce_total = !string.IsNullOrEmpty(model.abce_total) && decimal.TryParse(model.abce_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abce_total.Replace(",", "")) : (decimal?)null,
                        abce_total_quantity = !string.IsNullOrEmpty(model.abce_total_quantity) && decimal.TryParse(model.abce_total_quantity.Replace(",", ""), out resultTotalQty) ? decimal.Parse(model.abce_total_quantity.Replace(",", "")) : (decimal?)null,
                        abce_proration = model.abce_proration,
                        abce_status = "DRAFT",
                        abce_created_by = getIdentity.ID(User.Identity.Name),
                        last_modified = DateTime.Now,
                        abce_ast_code = model.abce_ast_code,
                        abce_ast_category = model.abce_ast_category
                    };
                    db.bgt_expenses_capitals.Add(expenses_capital);
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
                        controller = "ExpensesCapital",
                        action = "RecordsGL",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abce_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abce_cost_center_code
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

        // GET: ExpensesCapital/Edit/5
        public ActionResult Edit(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return RedirectToAction("Create");
            }

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);

            return View();
        }

        // POST: ExpensesCapital/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(int? id, ExpensesCapitalEditViewModel model)
        //{
        //    var screen = scr.GetScreen(screenCode);
        //    var year = GetYears().FirstOrDefault();

        //    var costCenters = new SelectList(cc.GetCostCenters().Select(s => new SelectListItem
        //    {
        //        Value = s.fld_CostCenter,
        //        Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc
        //    }), "Value", "Text").ToList();

        //    var gls = new SelectList(gl.GetGLs(screen.ScrID).Select(s => new SelectListItem
        //    {
        //        Value = s.fld_GLCode,
        //        Text = s.fld_GLCode + " - " + s.fld_GLDesc
        //    }), "Value", "Text").ToList();

        //    ViewBag.Title = screen.ScrName;
        //    ViewBag.Year = year;
        //    ViewBag.CostCenters = costCenters;
        //    ViewBag.GLs = gls;

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var resultAmount1 = 0m;
        //            var resultAmount2 = 0m;
        //            var resultAmount3 = 0m;
        //            var resultAmount4 = 0m;
        //            var resultAmount5 = 0m;
        //            var resultAmount6 = 0m;
        //            var resultAmount7 = 0m;
        //            var resultAmount8 = 0m;
        //            var resultAmount9 = 0m;
        //            var resultAmount10 = 0m;
        //            var resultAmount11 = 0m;
        //            var resultAmount12 = 0m;
        //            var resultTotal = 0m;
        //            var resultTotalQty = 0m;

        //            var expenses_capital = db.bgt_expenses_capitals.Find(id);
        //            expenses_capital.abce_budgeting_year = model.abce_budgeting_year;
        //            expenses_capital.abce_syarikat_id = model.abce_syarikat_id;
        //            expenses_capital.abce_syarikat_name = model.abce_syarikat_name;
        //            expenses_capital.abce_ladang_id = model.abce_ladang_id;
        //            expenses_capital.abce_ladang_code = model.abce_ladang_code;
        //            expenses_capital.abce_ladang_name = model.abce_ladang_name;
        //            expenses_capital.abce_wilayah_id = model.abce_wilayah_id;
        //            expenses_capital.abce_wilayah_name = model.abce_wilayah_name;
        //            expenses_capital.abce_cost_center_code = model.abce_cost_center_code;
        //            expenses_capital.abce_cost_center_desc = model.abce_cost_center_desc;
        //            expenses_capital.abce_gl_code = model.abce_gl_code;
        //            expenses_capital.abce_gl_name = model.abce_gl_name;
        //            expenses_capital.abce_desc = model.abce_desc;
        //            expenses_capital.abce_jenis_code = model.abce_jenis_code;
        //            expenses_capital.abce_jenis_name = model.abce_jenis_name;
        //            expenses_capital.abce_reason = model.abce_reason;
        //            expenses_capital.abce_material_details = model.abce_material_details;
        //            expenses_capital.abce_unit_1 = model.abce_unit_1;
        //            expenses_capital.abce_uom_1 = model.abce_uom_1;
        //            expenses_capital.abce_rate = model.abce_rate;
        //            expenses_capital.abce_quantity_1 = model.abce_quantity_1;
        //            expenses_capital.abce_quantity_2 = model.abce_quantity_2;
        //            expenses_capital.abce_quantity_3 = model.abce_quantity_3;
        //            expenses_capital.abce_quantity_4 = model.abce_quantity_4;
        //            expenses_capital.abce_quantity_5 = model.abce_quantity_5;
        //            expenses_capital.abce_quantity_6 = model.abce_quantity_6;
        //            expenses_capital.abce_quantity_7 = model.abce_quantity_7;
        //            expenses_capital.abce_quantity_8 = model.abce_quantity_8;
        //            expenses_capital.abce_quantity_9 = model.abce_quantity_9;
        //            expenses_capital.abce_quantity_10 = model.abce_quantity_10;
        //            expenses_capital.abce_quantity_11 = model.abce_quantity_11;
        //            expenses_capital.abce_quantity_12 = model.abce_quantity_12;
        //            expenses_capital.abce_amount_1 = !string.IsNullOrEmpty(model.abce_amount_1) && decimal.TryParse(model.abce_amount_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abce_amount_1.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_2 = !string.IsNullOrEmpty(model.abce_amount_2) && decimal.TryParse(model.abce_amount_2.Replace(",", ""), out resultAmount2) ? decimal.Parse(model.abce_amount_2.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_3 = !string.IsNullOrEmpty(model.abce_amount_3) && decimal.TryParse(model.abce_amount_3.Replace(",", ""), out resultAmount3) ? decimal.Parse(model.abce_amount_3.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_4 = !string.IsNullOrEmpty(model.abce_amount_4) && decimal.TryParse(model.abce_amount_4.Replace(",", ""), out resultAmount4) ? decimal.Parse(model.abce_amount_4.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_5 = !string.IsNullOrEmpty(model.abce_amount_5) && decimal.TryParse(model.abce_amount_5.Replace(",", ""), out resultAmount5) ? decimal.Parse(model.abce_amount_5.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_6 = !string.IsNullOrEmpty(model.abce_amount_6) && decimal.TryParse(model.abce_amount_6.Replace(",", ""), out resultAmount6) ? decimal.Parse(model.abce_amount_6.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_7 = !string.IsNullOrEmpty(model.abce_amount_7) && decimal.TryParse(model.abce_amount_7.Replace(",", ""), out resultAmount7) ? decimal.Parse(model.abce_amount_7.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_8 = !string.IsNullOrEmpty(model.abce_amount_8) && decimal.TryParse(model.abce_amount_8.Replace(",", ""), out resultAmount8) ? decimal.Parse(model.abce_amount_8.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_9 = !string.IsNullOrEmpty(model.abce_amount_9) && decimal.TryParse(model.abce_amount_9.Replace(",", ""), out resultAmount9) ? decimal.Parse(model.abce_amount_9.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_10 = !string.IsNullOrEmpty(model.abce_amount_10) && decimal.TryParse(model.abce_amount_10.Replace(",", ""), out resultAmount10) ? decimal.Parse(model.abce_amount_10.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_11 = !string.IsNullOrEmpty(model.abce_amount_11) && decimal.TryParse(model.abce_amount_11.Replace(",", ""), out resultAmount11) ? decimal.Parse(model.abce_amount_11.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_amount_12 = !string.IsNullOrEmpty(model.abce_amount_12) && decimal.TryParse(model.abce_amount_12.Replace(",", ""), out resultAmount12) ? decimal.Parse(model.abce_amount_12.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_total = !string.IsNullOrEmpty(model.abce_total) && decimal.TryParse(model.abce_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abce_total.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_total_quantity = !string.IsNullOrEmpty(model.abce_total_quantity) && decimal.TryParse(model.abce_total_quantity.Replace(",", ""), out resultTotalQty) ? decimal.Parse(model.abce_total_quantity.Replace(",", "")) : (decimal?)null;
        //            expenses_capital.abce_proration = model.abce_proration;
        //            expenses_capital.last_modified = DateTime.Now;

        //            db.Entry(expenses_capital).State = EntityState.Modified;
        //            await db.SaveChangesAsync();

        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return View(model);
        //        }
        //    }

        //    return View(model);
        //}

        public async Task<ActionResult> EditDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expenses_capital = await db.bgt_expenses_capitals.FindAsync(id);
            if (expenses_capital == null)
            {
                return HttpNotFound();
            }

            var gls = new SelectList(GetGLs(expenses_capital.abce_budgeting_year, expenses_capital.abce_cost_center_code, expenses_capital.abce_id).Select(s => new SelectListItem
            {
                Value = s.Code,
                Text = s.Code + " - " + s.Description
            }), "Value", "Text").ToList();

            var assetCategories = new SelectList(GetAssetCategories().Select(s => new SelectListItem
            {
                Value = s.ast_ID.ToString(),
                Text = s.ast_ID + " - " + s.ast_Category
            }), "Value", "Text").ToList();

            var model = new ExpensesCapitalEditViewModel
            {
                abce_id = id.Value,
                abce_budgeting_year = expenses_capital.abce_budgeting_year,
                abce_syarikat_id = expenses_capital.abce_syarikat_id,
                abce_syarikat_name = expenses_capital.abce_syarikat_name,
                abce_ladang_id = expenses_capital.abce_ladang_id,
                abce_ladang_code = expenses_capital.abce_ladang_code,
                abce_ladang_name = expenses_capital.abce_ladang_name,
                abce_wilayah_id = expenses_capital.abce_wilayah_id,
                abce_wilayah_name = expenses_capital.abce_wilayah_name,
                abce_cost_center_code = expenses_capital.abce_cost_center_code,
                abce_cost_center_desc = expenses_capital.abce_cost_center_desc,
                abce_gl_code = expenses_capital.abce_gl_code,
                abce_gl_name = expenses_capital.abce_gl_name,
                abce_desc = expenses_capital.abce_desc,
                abce_jenis_code = expenses_capital.abce_jenis_code,
                abce_jenis_name = expenses_capital.abce_jenis_name,
                abce_reason = expenses_capital.abce_reason,
                abce_material_details = expenses_capital.abce_material_details,
                abce_unit_1 = expenses_capital.abce_unit_1.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_unit_1) : "",
                abce_uom_1 = expenses_capital.abce_uom_1,
                abce_rate = expenses_capital.abce_rate.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_rate) : "",
                abce_quantity_1 = expenses_capital.abce_quantity_1.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_1) : "",
                abce_quantity_2 = expenses_capital.abce_quantity_2.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_2) : "",
                abce_quantity_3 = expenses_capital.abce_quantity_3.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_3) : "",
                abce_quantity_4 = expenses_capital.abce_quantity_4.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_4) : "",
                abce_quantity_5 = expenses_capital.abce_quantity_5.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_5) : "",
                abce_quantity_6 = expenses_capital.abce_quantity_6.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_6) : "",
                abce_quantity_7 = expenses_capital.abce_quantity_7.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_7) : "",
                abce_quantity_8 = expenses_capital.abce_quantity_8.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_8) : "",
                abce_quantity_9 = expenses_capital.abce_quantity_9.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_9) : "",
                abce_quantity_10 = expenses_capital.abce_quantity_10.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_10) : "",
                abce_quantity_11 = expenses_capital.abce_quantity_11.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_11) : "",
                abce_quantity_12 = expenses_capital.abce_quantity_12.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_quantity_12) : "",
                abce_amount_1 = expenses_capital.abce_amount_1.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_1) : "0.00",
                abce_amount_2 = expenses_capital.abce_amount_2.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_2) : "0.00",
                abce_amount_3 = expenses_capital.abce_amount_3.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_3) : "0.00",
                abce_amount_4 = expenses_capital.abce_amount_4.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_4) : "0.00",
                abce_amount_5 = expenses_capital.abce_amount_5.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_5) : "0.00",
                abce_amount_6 = expenses_capital.abce_amount_6.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_6) : "0.00",
                abce_amount_7 = expenses_capital.abce_amount_7.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_7) : "0.00",
                abce_amount_8 = expenses_capital.abce_amount_8.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_8) : "0.00",
                abce_amount_9 = expenses_capital.abce_amount_9.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_9) : "0.00",
                abce_amount_10 = expenses_capital.abce_amount_10.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_10) : "0.00",
                abce_amount_11 = expenses_capital.abce_amount_11.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_11) : "0.00",
                abce_amount_12 = expenses_capital.abce_amount_12.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_amount_12) : "0.00",
                abce_total = expenses_capital.abce_total.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_total) : "0.00",
                abce_total_quantity = expenses_capital.abce_total_quantity.HasValue ? string.Format("{0:#,##0.000}", expenses_capital.abce_total_quantity) : "0.00",
                abce_proration = expenses_capital.abce_proration,
                abce_history = expenses_capital.abce_history,
                abce_status = expenses_capital.abce_status,
                abce_ast_code = expenses_capital.abce_ast_code,
                abce_ast_category = expenses_capital.abce_ast_category
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.GLs = gls;
            ViewBag.AssetCategories = assetCategories;

            return PartialView("_EditDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDetail(int? id, ExpensesCapitalEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var resultUnit1 = 0m;
                    var resultRate = 0m;
                    var resultQuantity1 = 0m;
                    var resultQuantity2 = 0m;
                    var resultQuantity3 = 0m;
                    var resultQuantity4 = 0m;
                    var resultQuantity5 = 0m;
                    var resultQuantity6 = 0m;
                    var resultQuantity7 = 0m;
                    var resultQuantity8 = 0m;
                    var resultQuantity9 = 0m;
                    var resultQuantity10 = 0m;
                    var resultQuantity11 = 0m;
                    var resultQuantity12 = 0m;
                    var resultAmount1 = 0m;
                    var resultAmount2 = 0m;
                    var resultAmount3 = 0m;
                    var resultAmount4 = 0m;
                    var resultAmount5 = 0m;
                    var resultAmount6 = 0m;
                    var resultAmount7 = 0m;
                    var resultAmount8 = 0m;
                    var resultAmount9 = 0m;
                    var resultAmount10 = 0m;
                    var resultAmount11 = 0m;
                    var resultAmount12 = 0m;
                    var resultTotal = 0m;
                    var resultTotalQty = 0m;

                    var syarikatObj = syarikat.GetSyarikat();
                    var ladangObj = ladang.GetLadang();
                    var wilayahObj = wilayah.GetWilayah();

                    var expenses_capital = db.bgt_expenses_capitals.Find(id);
                    expenses_capital.abce_budgeting_year = model.abce_budgeting_year;
                    expenses_capital.abce_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null;
                    expenses_capital.abce_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                    expenses_capital.abce_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null;
                    expenses_capital.abce_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                    expenses_capital.abce_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                    expenses_capital.abce_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null;
                    expenses_capital.abce_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null;
                    expenses_capital.abce_cost_center_code = model.abce_cost_center_code;
                    expenses_capital.abce_cost_center_desc = model.abce_cost_center_desc;
                    expenses_capital.abce_gl_code = model.abce_gl_code;
                    expenses_capital.abce_gl_name = model.abce_gl_name;
                    expenses_capital.abce_desc = model.abce_desc;
                    expenses_capital.abce_jenis_code = model.abce_jenis_code;
                    expenses_capital.abce_jenis_name = model.abce_jenis_name;
                    expenses_capital.abce_reason = model.abce_reason;
                    expenses_capital.abce_material_details = model.abce_material_details;
                    expenses_capital.abce_unit_1 = !string.IsNullOrEmpty(model.abce_unit_1) && decimal.TryParse(model.abce_unit_1.Replace(",", ""), out resultUnit1) ? decimal.Parse(model.abce_unit_1.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_uom_1 = model.abce_uom_1;
                    expenses_capital.abce_rate = !string.IsNullOrEmpty(model.abce_rate) && decimal.TryParse(model.abce_rate.Replace(",", ""), out resultRate) ? decimal.Parse(model.abce_rate.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_1 = !string.IsNullOrEmpty(model.abce_quantity_1) && decimal.TryParse(model.abce_quantity_1.Replace(",", ""), out resultQuantity1) ? decimal.Parse(model.abce_quantity_1.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_2 = !string.IsNullOrEmpty(model.abce_quantity_2) && decimal.TryParse(model.abce_quantity_2.Replace(",", ""), out resultQuantity2) ? decimal.Parse(model.abce_quantity_2.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_3 = !string.IsNullOrEmpty(model.abce_quantity_3) && decimal.TryParse(model.abce_quantity_3.Replace(",", ""), out resultQuantity3) ? decimal.Parse(model.abce_quantity_3.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_4 = !string.IsNullOrEmpty(model.abce_quantity_4) && decimal.TryParse(model.abce_quantity_4.Replace(",", ""), out resultQuantity4) ? decimal.Parse(model.abce_quantity_4.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_5 = !string.IsNullOrEmpty(model.abce_quantity_5) && decimal.TryParse(model.abce_quantity_5.Replace(",", ""), out resultQuantity5) ? decimal.Parse(model.abce_quantity_5.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_6 = !string.IsNullOrEmpty(model.abce_quantity_6) && decimal.TryParse(model.abce_quantity_6.Replace(",", ""), out resultQuantity6) ? decimal.Parse(model.abce_quantity_6.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_7 = !string.IsNullOrEmpty(model.abce_quantity_7) && decimal.TryParse(model.abce_quantity_7.Replace(",", ""), out resultQuantity7) ? decimal.Parse(model.abce_quantity_7.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_8 = !string.IsNullOrEmpty(model.abce_quantity_8) && decimal.TryParse(model.abce_quantity_8.Replace(",", ""), out resultQuantity8) ? decimal.Parse(model.abce_quantity_8.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_9 = !string.IsNullOrEmpty(model.abce_quantity_9) && decimal.TryParse(model.abce_quantity_9.Replace(",", ""), out resultQuantity9) ? decimal.Parse(model.abce_quantity_9.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_10 = !string.IsNullOrEmpty(model.abce_quantity_10) && decimal.TryParse(model.abce_quantity_10.Replace(",", ""), out resultQuantity10) ? decimal.Parse(model.abce_quantity_10.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_11 = !string.IsNullOrEmpty(model.abce_quantity_11) && decimal.TryParse(model.abce_quantity_11.Replace(",", ""), out resultQuantity11) ? decimal.Parse(model.abce_quantity_11.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_quantity_12 = !string.IsNullOrEmpty(model.abce_quantity_12) && decimal.TryParse(model.abce_quantity_12.Replace(",", ""), out resultQuantity12) ? decimal.Parse(model.abce_quantity_12.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_1 = !string.IsNullOrEmpty(model.abce_amount_1) && decimal.TryParse(model.abce_amount_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abce_amount_1.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_2 = !string.IsNullOrEmpty(model.abce_amount_2) && decimal.TryParse(model.abce_amount_2.Replace(",", ""), out resultAmount2) ? decimal.Parse(model.abce_amount_2.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_3 = !string.IsNullOrEmpty(model.abce_amount_3) && decimal.TryParse(model.abce_amount_3.Replace(",", ""), out resultAmount3) ? decimal.Parse(model.abce_amount_3.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_4 = !string.IsNullOrEmpty(model.abce_amount_4) && decimal.TryParse(model.abce_amount_4.Replace(",", ""), out resultAmount4) ? decimal.Parse(model.abce_amount_4.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_5 = !string.IsNullOrEmpty(model.abce_amount_5) && decimal.TryParse(model.abce_amount_5.Replace(",", ""), out resultAmount5) ? decimal.Parse(model.abce_amount_5.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_6 = !string.IsNullOrEmpty(model.abce_amount_6) && decimal.TryParse(model.abce_amount_6.Replace(",", ""), out resultAmount6) ? decimal.Parse(model.abce_amount_6.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_7 = !string.IsNullOrEmpty(model.abce_amount_7) && decimal.TryParse(model.abce_amount_7.Replace(",", ""), out resultAmount7) ? decimal.Parse(model.abce_amount_7.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_8 = !string.IsNullOrEmpty(model.abce_amount_8) && decimal.TryParse(model.abce_amount_8.Replace(",", ""), out resultAmount8) ? decimal.Parse(model.abce_amount_8.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_9 = !string.IsNullOrEmpty(model.abce_amount_9) && decimal.TryParse(model.abce_amount_9.Replace(",", ""), out resultAmount9) ? decimal.Parse(model.abce_amount_9.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_10 = !string.IsNullOrEmpty(model.abce_amount_10) && decimal.TryParse(model.abce_amount_10.Replace(",", ""), out resultAmount10) ? decimal.Parse(model.abce_amount_10.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_11 = !string.IsNullOrEmpty(model.abce_amount_11) && decimal.TryParse(model.abce_amount_11.Replace(",", ""), out resultAmount11) ? decimal.Parse(model.abce_amount_11.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_amount_12 = !string.IsNullOrEmpty(model.abce_amount_12) && decimal.TryParse(model.abce_amount_12.Replace(",", ""), out resultAmount12) ? decimal.Parse(model.abce_amount_12.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_total = !string.IsNullOrEmpty(model.abce_total) && decimal.TryParse(model.abce_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abce_total.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_total_quantity = !string.IsNullOrEmpty(model.abce_total_quantity) && decimal.TryParse(model.abce_total_quantity.Replace(",", ""), out resultTotalQty) ? decimal.Parse(model.abce_total_quantity.Replace(",", "")) : (decimal?)null;
                    expenses_capital.abce_proration = model.abce_proration;
                    expenses_capital.last_modified = DateTime.Now;
                    expenses_capital.abce_ast_code = model.abce_ast_code;
                    expenses_capital.abce_ast_category = model.abce_ast_category;

                    db.Entry(expenses_capital).State = EntityState.Modified;
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
                        controller = "ExpensesCapital",
                        action = "RecordsGL",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abce_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abce_cost_center_code
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

        // GET: ExpensesCapital/Delete/5
        public async Task<ActionResult> Delete(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_capital = await db.bgt_expenses_capitals.Where(c => c.abce_budgeting_year == year && c.abce_cost_center_code.Equals(CostCenter) && !c.abce_deleted).FirstOrDefaultAsync();
            if (expenses_capital == null)
            {
                return HttpNotFound();
            }

            var total_amount = db.bgt_expenses_capitals
                .Where(c => c.abce_budgeting_year == expenses_capital.abce_budgeting_year && c.abce_cost_center_code.Equals(expenses_capital.abce_cost_center_code) && !c.abce_deleted)
                .Sum(c => c.abce_total);
            var model = new ExpensesCapitalDeleteViewModel
            {
                BudgetYear = expenses_capital.abce_budgeting_year,
                CostCenterCode = expenses_capital.abce_cost_center_code,
                CostCenterDesc = expenses_capital.abce_cost_center_desc,
                Total = string.Format("{0:#,##0.00}", total_amount)
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Delete", model);
        }

        // POST: ExpensesCapital/Delete/5
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
                var expenses_capitals = db.bgt_expenses_capitals
                    .Where(c => c.abce_budgeting_year == year && c.abce_cost_center_code.Equals(CostCenter) && !c.abce_deleted)
                    .ToList();

                if (expenses_capitals.Count() > 0)
                {
                    foreach (var item in expenses_capitals)
                    {
                        //item.abce_deleted = true;
                        //item.last_modified = DateTime.Now;
                        //db.Entry(item).State = EntityState.Modified;
                        db.bgt_expenses_capitals.Remove(item);
                    }
                    await db.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgDelete2,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesCapital",
                        action = "Records",
                        paramName = "BudgetYear",
                        paramValue = BudgetYear
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesCapital",
                        action = "Records",
                        paramName = "BudgetYear",
                        paramValue = BudgetYear
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
            var expenses_capital = await db.bgt_expenses_capitals.FindAsync(id);
            if (expenses_capital == null)
            {
                return HttpNotFound();
            }

            var model = new ExpensesCapitalDeleteDetailViewModel
            {
                abce_id = expenses_capital.abce_id,
                abce_budgeting_year = expenses_capital.abce_budgeting_year,
                abce_syarikat_id = expenses_capital.abce_syarikat_id,
                abce_syarikat_name = expenses_capital.abce_syarikat_name,
                abce_ladang_id = expenses_capital.abce_ladang_id,
                abce_ladang_code = expenses_capital.abce_ladang_code,
                abce_ladang_name = expenses_capital.abce_ladang_name,
                abce_wilayah_id = expenses_capital.abce_wilayah_id,
                abce_wilayah_name = expenses_capital.abce_wilayah_name,
                abce_cost_center_code = expenses_capital.abce_cost_center_code,
                abce_cost_center_desc = expenses_capital.abce_cost_center_desc,
                abce_gl_code = expenses_capital.abce_gl_code,
                abce_gl_name = expenses_capital.abce_gl_name,
                abce_desc = expenses_capital.abce_desc,
                abce_jenis_code = expenses_capital.abce_jenis_code,
                abce_jenis_name = expenses_capital.abce_jenis_name,
                abce_unit_1 = expenses_capital.abce_unit_1,
                abce_rate = expenses_capital.abce_rate,
                abce_total = expenses_capital.abce_total.HasValue ? string.Format("{0:#,##0.00}", expenses_capital.abce_total) : "0.00",
                abce_ast_code = expenses_capital.abce_ast_code,
                abce_ast_category = expenses_capital.abce_ast_category,
                abce_ast_code_category = expenses_capital.abce_ast_code + " - " + expenses_capital.abce_ast_category
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
                var expenses_capital = db.bgt_expenses_capitals.Find(id);
                //expenses_capital.abce_deleted = true;
                //expenses_capital.last_modified = DateTime.Now;
                //db.Entry(expenses_capital).State = EntityState.Modified;
                db.bgt_expenses_capitals.Remove(expenses_capital);
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
                    controller = "ExpensesCapital",
                    action = "RecordsGL",
                    paramName1 = "BudgetYear",
                    paramValue1 = expenses_capital.abce_budgeting_year.ToString(),
                    paramName2 = "CostCenter",
                    paramValue2 = expenses_capital.abce_cost_center_code
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

        public ActionResult Print(string year, string costCenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesCapitalList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc.ToUpper() + " - LISTING";

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesCapitalListViewModel> data = db.Database.SqlQuery<ExpensesCapitalListViewModel>("exec sp_BudgetExpensesCapitalList {0}, {1}, {2}, {3}, {4}", year, costCenter, syarikat.GetSyarikatID().ToString(), ladang.GetLadangID().ToString(), wilayah.GetWilayahID().ToString()).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintView(string year, string costCenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesCapitalView.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc.ToUpper() + " - VIEW";

            if (format.ToLower().Contains("excel"))
            {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesCapitalViewExcel.rdlc");
            }

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesCapitalListDetailViewModel> data = db.Database.SqlQuery<ExpensesCapitalListDetailViewModel>("exec sp_BudgetExpensesCapitalView {0}, {1}, {2}, {3}, {4}", year, costCenter, syarikat.GetSyarikatID().ToString(), ladang.GetLadangID().ToString(), wilayah.GetWilayahID().ToString()).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
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

        private List<tbl_SAPCCPUP> GetCostCenters(int budgetYear, bool completeGL = false)
        {
            var expenses_capitals = GetRecords(budgetYear);
            var excludeCostCenters = new List<string>();

            if (completeGL)
            {
                var gls = gl.GetGLs(scr.GetScreen(screenCode).ScrID);

                for (int i = 0; i < expenses_capitals.Count(); i++)
                {
                    for (int j = 0; j < gls.Count(); j++)
                    {
                        if (expenses_capitals[i].abce_gl_code.Equals(gls[j].fld_GLCode) && j == gls.Count())
                        {
                            excludeCostCenters.Add(expenses_capitals[i].abce_cost_center_code.PadLeft(10, '0'));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < expenses_capitals.Count(); i++)
                {
                    excludeCostCenters.Add(expenses_capitals[i].abce_cost_center_code.PadLeft(10, '0'));
                }
            }

            return cc.GetCostCenters()
                .Where(c => !excludeCostCenters.Contains(c.fld_CostCenter))
                .OrderBy(c => c.fld_CostCenter)
                .ToList();
        }

        public List<GLListViewModel> GetGLs(int? budgetYear = null, string costCenter = null, int? excludeRecordId = null, bool exclude = false)
        {
            var screen = scr.GetScreen(screenCode);

            if (exclude)
            {
                var expenses_capitals = GetRecords(budgetYear, costCenter, null, null, excludeRecordId);
                var excludeGLs = new List<string>();

                for (int i = 0; i < expenses_capitals.Count(); i++)
                {
                    excludeGLs.Add(expenses_capitals[i].abce_gl_code);
                }

                return dbm.tbl_SAPGLPUP
                    .Where(g => g.bgt_ScreenGLs.Any(s => s.fld_ScrID == screen.ScrID))
                    .Where(g => !excludeGLs.Contains(g.fld_GLCode))
                    .OrderBy(g => g.fld_GLCode)
                    .Select(g => new GLListViewModel { Id = g.fld_ID, Code = g.fld_GLCode, Description = g.fld_GLDesc })
                    .ToList();
            }
            else
            {
                return dbm.tbl_SAPGLPUP
                    .Where(g => g.bgt_ScreenGLs.Any(s => s.fld_ScrID == screen.ScrID))
                    .OrderBy(g => g.fld_GLCode)
                    .Select(g => new GLListViewModel { Id = g.fld_ID, Code = g.fld_GLCode, Description = g.fld_GLDesc })
                    .ToList();
            }
        }

        private List<bgt_expenses_capital> GetRecords(int? budgeting_year = null, string cost_center = null, string gl = null, string jenis = null, int? excludeId = null, int? page = null, int? pageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from c in db.bgt_expenses_capitals
                        where !c.abce_deleted
                        select c;

            if (budgeting_year.HasValue)
            {
                query = query.Where(q => q.abce_budgeting_year == budgeting_year);
                if (budgeting_year.Value > 2023)
                {
                    query = query.Where(q => q.abce_syarikat_id == syarikatId && q.abce_ladang_id == ladangId && q.abce_wilayah_id == wilayahId);
                }
            }
            if (!string.IsNullOrEmpty(cost_center))
                query = query.Where(q => q.abce_cost_center_code.Contains(cost_center) || q.abce_cost_center_desc.Contains(cost_center));
            if (!string.IsNullOrEmpty(gl))
                query = query.Where(q => q.abce_gl_code.Contains(gl) || q.abce_gl_name.Contains(gl));
            if (!string.IsNullOrEmpty(jenis))
                query = query.Where(q => q.abce_jenis_code.Equals(jenis) || q.abce_jenis_name.Contains(jenis));
            if (excludeId.HasValue)
                query = query.Where(q => q.abce_id != excludeId);

            var query2 = query.AsQueryable();

            if (page.HasValue && pageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abce_budgeting_year)
                    .ThenBy(q => q.abce_cost_center_code)
                    .ThenBy(q => q.abce_gl_code)
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abce_budgeting_year)
                    .ThenBy(q => q.abce_cost_center_code)
                    .ThenBy(q => q.abce_gl_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new bgt_expenses_capital
                          {
                              abce_id = q.abce_id,
                              abce_budgeting_year = q.abce_budgeting_year,
                              abce_syarikat_id = q.abce_syarikat_id,
                              abce_syarikat_name = q.abce_syarikat_name,
                              abce_ladang_id = q.abce_ladang_id,
                              abce_ladang_code = q.abce_ladang_code,
                              abce_ladang_name = q.abce_ladang_name,
                              abce_wilayah_id = q.abce_wilayah_id,
                              abce_wilayah_name = q.abce_wilayah_name,
                              abce_cost_center_code = q.abce_cost_center_code,
                              abce_cost_center_desc = q.abce_cost_center_desc,
                              abce_gl_code = q.abce_gl_code,
                              abce_gl_name = q.abce_gl_name,
                              abce_desc = q.abce_desc,
                              abce_jenis_code = q.abce_jenis_code,
                              abce_jenis_name = q.abce_jenis_name,
                              abce_reason = q.abce_reason,
                              abce_material_details = q.abce_material_details,
                              abce_unit_1 = q.abce_unit_1,
                              abce_uom_1 = q.abce_uom_1,
                              abce_rate = q.abce_rate,
                              abce_quantity_1 = q.abce_quantity_1,
                              abce_quantity_2 = q.abce_quantity_2,
                              abce_quantity_3 = q.abce_quantity_3,
                              abce_quantity_4 = q.abce_quantity_4,
                              abce_quantity_5 = q.abce_quantity_5,
                              abce_quantity_6 = q.abce_quantity_6,
                              abce_quantity_7 = q.abce_quantity_7,
                              abce_quantity_8 = q.abce_quantity_8,
                              abce_quantity_9 = q.abce_quantity_9,
                              abce_quantity_10 = q.abce_quantity_10,
                              abce_quantity_11 = q.abce_quantity_11,
                              abce_quantity_12 = q.abce_quantity_12,
                              abce_amount_1 = q.abce_amount_1,
                              abce_amount_2 = q.abce_amount_2,
                              abce_amount_3 = q.abce_amount_3,
                              abce_amount_4 = q.abce_amount_4,
                              abce_amount_5 = q.abce_amount_5,
                              abce_amount_6 = q.abce_amount_6,
                              abce_amount_7 = q.abce_amount_7,
                              abce_amount_8 = q.abce_amount_8,
                              abce_amount_9 = q.abce_amount_9,
                              abce_amount_10 = q.abce_amount_10,
                              abce_amount_11 = q.abce_amount_11,
                              abce_amount_12 = q.abce_amount_12,
                              abce_total = q.abce_total,
                              abce_total_quantity = q.abce_total_quantity,
                              abce_proration = q.abce_proration,
                              abce_history = q.abce_history,
                              abce_status = q.abce_status,
                              last_modified = q.last_modified,
                              abce_ast_code = q.abce_ast_code,
                              abce_ast_category = q.abce_ast_category
                          }).Distinct();

            return query3.ToList();
        }

        private List<ExpensesCapitalListViewModel> GetRecordsIndex(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from c in db.bgt_expenses_capitals
                        where !c.abce_deleted
                        group c by new { c.abce_budgeting_year, c.abce_syarikat_id, c.abce_ladang_id, c.abce_wilayah_id, c.abce_cost_center_code, c.abce_cost_center_desc } into g
                        select new
                        {
                            BudgetYear = g.Key.abce_budgeting_year,
                            SyarikatId = g.Key.abce_syarikat_id,
                            LadangId = g.Key.abce_ladang_id,
                            WilayahId = g.Key.abce_wilayah_id,
                            CostCenterCode = g.Key.abce_cost_center_code,
                            CostCenterDesc = g.Key.abce_cost_center_desc,
                            Total = g.Sum(c => c.abce_total)
                        };

            if (BudgetYear.HasValue)
            {
                query = query.Where(q => q.BudgetYear == BudgetYear);
                if (BudgetYear.Value > 2023)
                {
                    query = query.Where(q => q.SyarikatId == syarikatId && q.LadangId == ladangId && q.WilayahId == wilayahId);
                }
            }
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
                          select new ExpensesCapitalListViewModel
                          {
                              BudgetYear = q.BudgetYear,
                              CostCenterCode = q.CostCenterCode,
                              CostCenterDesc = q.CostCenterDesc,
                              Total = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        private List<bgt_AssetCategory> GetAssetCategories()
        {
            return dbc.bgt_AssetCategories.ToList();
        }

        public JsonResult GetAssetCategoryByID(int id)
        {
            var assetCategory = dbc.bgt_AssetCategories.FirstOrDefault(a => a.ast_ID == id);
            if (assetCategory != null)
            {
                return Json(new
                {
                    assetCategory = assetCategory
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                assetCategory = new bgt_AssetCategory()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlByCode(string code)
        {
            var sapgl = dbm.tbl_SAPGLPUP.FirstOrDefault(g => g.fld_GLCode.Contains(code));
            if (sapgl != null)
            {
                var gl = new GLListViewModel
                {
                    Id = sapgl.fld_ID,
                    Code = sapgl.fld_GLCode.TrimStart(new char[] { '0' }),
                    Description = sapgl.fld_GLDesc
                };
                return Json(new
                {
                    gl = gl
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                gl = new GLListViewModel()
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
