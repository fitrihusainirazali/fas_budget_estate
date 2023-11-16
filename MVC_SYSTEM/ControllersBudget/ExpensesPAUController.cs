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
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.App_LocalResources;
using PagedList;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorize(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ExpensesPAUController : Controller
    {
        private MVC_SYSTEM_ModelsBudgetEst db = new ConnectionBudget().GetConnection();
        private MVC_SYSTEM_ModelsBudget dbc = new MVC_SYSTEM_ModelsBudget();
        private GetConfig config = new GetConfig();
        private Screen scr = new Screen();
        private CostCenter cc = new CostCenter();
        private GL gl = new GL();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private errorlog errlog = new errorlog();
        private readonly string screenCode = "E10";

        // GET: ExpensesPAU
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

            var records = new ViewingModels.PagedList<ExpensesPAUListViewModel>
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

            var records = new ViewingModels.PagedList<ExpensesPAUListDetailViewModel>
            {
                Content = GetRecordsDetail(budgetYear, CostCenter, null, null, null, page, pageSize),
                TotalRecords = GetRecordsDetail(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_RecordsDetail", records);
        }

        //public ActionResult RecordsDetail(string sortOrder, string BudgetYear, string CostCenter, string BudgetYearFilter, string CostCenterFilter, int? page)
        //{
        //    if (!string.IsNullOrEmpty(BudgetYear) && !string.IsNullOrEmpty(CostCenter))
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        BudgetYear = BudgetYearFilter;
        //        CostCenter = CostCenterFilter;
        //    }

        //    int pageNumber = page ?? 1;
        //    int pageSize = 100; //int.Parse(config.GetData("paging"));
        //    int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

        //    var records = GetRecordsDetail(budgetYear, CostCenter);

        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.BudgetYear = BudgetYear;
        //    ViewBag.CostCenter = CostCenter;
        //    ViewBag.BudgetYearFilter = BudgetYearFilter;
        //    ViewBag.CostCenterFilter = CostCenterFilter;
        //    ViewBag.PageNumber = pageNumber;
        //    ViewBag.PageSize = pageSize;

        //    return PartialView("_RecordsDetail", records.ToPagedList(pageNumber, pageSize));
        //}

        // GET: ExpensesPAU/Details/5
        public ActionResult Details(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_paus = GetRecordsDetail(year, CostCenter);

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Details", expenses_paus);
        }

        // GET: ExpensesPAU/Create
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

        // POST: ExpensesPAU/Create
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
                CostCenter = CostCenter.TrimStart(new char[] { '0' })
            });
        }

        public ActionResult CreateDetail(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : GetYears().FirstOrDefault();

            var gl_buys = new SelectList(GetGLPAU().Select(s => new SelectListItem
            {
                Value = s.GLCode_Buy.TrimStart(new char[] { '0' }),
                Text = s.GLCode_Buy.TrimStart(new char[] { '0' }) + " - " + s.GLCode_Buy_Desc
            }), "Value", "Text").ToList();

            var wilayahId = wilayah.GetWilayahID();
            var locationId = cc.GetCostCenterLocID(CostCenter);
            var products = new SelectList(GetProducts(year, locationId).Select(s => new SelectListItem
            {
                Value = s.Code,
                Text = s.Code + " - " + s.Name
            }), "Value", "Text").ToList();

            var costCenter_sales = new SelectList(GetCostCenters(year, false, CostCenter).Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter.TrimStart(new char[] { '0' }),
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
            ViewBag.GLBuys = gl_buys;
            ViewBag.Products = products;
            ViewBag.CostCenterSales = costCenter_sales;

            return PartialView("_CreateDetail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDetail(ExpensesPAUCreateOrEditDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GetIdentity getIdentity = new GetIdentity();

                    var resultQuantity = 0m;
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

                    var expenses_pau = new bgt_expenses_pau
                    {
                        abep_budgeting_year = model.abep_budgeting_year,
                        abep_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null,
                        abep_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                        abep_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null,
                        abep_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                        abep_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                        abep_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null,
                        abep_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                        abep_cost_center_code = model.abep_cost_center_code,
                        abep_cost_center_desc = model.abep_cost_center_desc,
                        abep_gl_expenses_code = model.abep_gl_expenses_code,
                        abep_gl_expenses_name = model.abep_gl_expenses_name,
                        abep_gl_income_code = model.abep_gl_income_code,
                        abep_gl_income_name = model.abep_gl_income_name,
                        abep_product_code = model.abep_product_code,
                        abep_product_name = model.abep_product_name,
                        abep_cost_center_jualan_code = model.abep_cost_center_jualan_code,
                        abep_cost_center_jualan_desc = model.abep_cost_center_jualan_desc,
                        abep_quantity = !string.IsNullOrEmpty(model.abep_quantity) && decimal.TryParse(model.abep_quantity.Replace(",", ""), out resultQuantity) ? decimal.Parse(model.abep_quantity.Replace(",", "")) : (decimal?)null,
                        abep_uom_1 = model.abep_uom_1,
                        abep_rate = !string.IsNullOrEmpty(model.abep_rate) && decimal.TryParse(model.abep_rate.Replace(",", ""), out resultRate) ? decimal.Parse(model.abep_rate.Replace(",", "")) : (decimal?)null,
                        abep_quantity_1 = !string.IsNullOrEmpty(model.abep_quantity_1) && decimal.TryParse(model.abep_quantity_1.Replace(",", ""), out resultQuantity1) ? decimal.Parse(model.abep_quantity_1.Replace(",", "")) : (decimal?)null,
                        abep_quantity_2 = !string.IsNullOrEmpty(model.abep_quantity_2) && decimal.TryParse(model.abep_quantity_2.Replace(",", ""), out resultQuantity2) ? decimal.Parse(model.abep_quantity_2.Replace(",", "")) : (decimal?)null,
                        abep_quantity_3 = !string.IsNullOrEmpty(model.abep_quantity_3) && decimal.TryParse(model.abep_quantity_3.Replace(",", ""), out resultQuantity3) ? decimal.Parse(model.abep_quantity_3.Replace(",", "")) : (decimal?)null,
                        abep_quantity_4 = !string.IsNullOrEmpty(model.abep_quantity_4) && decimal.TryParse(model.abep_quantity_4.Replace(",", ""), out resultQuantity4) ? decimal.Parse(model.abep_quantity_4.Replace(",", "")) : (decimal?)null,
                        abep_quantity_5 = !string.IsNullOrEmpty(model.abep_quantity_5) && decimal.TryParse(model.abep_quantity_5.Replace(",", ""), out resultQuantity5) ? decimal.Parse(model.abep_quantity_5.Replace(",", "")) : (decimal?)null,
                        abep_quantity_6 = !string.IsNullOrEmpty(model.abep_quantity_6) && decimal.TryParse(model.abep_quantity_6.Replace(",", ""), out resultQuantity6) ? decimal.Parse(model.abep_quantity_6.Replace(",", "")) : (decimal?)null,
                        abep_quantity_7 = !string.IsNullOrEmpty(model.abep_quantity_7) && decimal.TryParse(model.abep_quantity_7.Replace(",", ""), out resultQuantity7) ? decimal.Parse(model.abep_quantity_7.Replace(",", "")) : (decimal?)null,
                        abep_quantity_8 = !string.IsNullOrEmpty(model.abep_quantity_8) && decimal.TryParse(model.abep_quantity_8.Replace(",", ""), out resultQuantity8) ? decimal.Parse(model.abep_quantity_8.Replace(",", "")) : (decimal?)null,
                        abep_quantity_9 = !string.IsNullOrEmpty(model.abep_quantity_9) && decimal.TryParse(model.abep_quantity_9.Replace(",", ""), out resultQuantity9) ? decimal.Parse(model.abep_quantity_9.Replace(",", "")) : (decimal?)null,
                        abep_quantity_10 = !string.IsNullOrEmpty(model.abep_quantity_10) && decimal.TryParse(model.abep_quantity_10.Replace(",", ""), out resultQuantity10) ? decimal.Parse(model.abep_quantity_10.Replace(",", "")) : (decimal?)null,
                        abep_quantity_11 = !string.IsNullOrEmpty(model.abep_quantity_11) && decimal.TryParse(model.abep_quantity_11.Replace(",", ""), out resultQuantity11) ? decimal.Parse(model.abep_quantity_11.Replace(",", "")) : (decimal?)null,
                        abep_quantity_12 = !string.IsNullOrEmpty(model.abep_quantity_12) && decimal.TryParse(model.abep_quantity_12.Replace(",", ""), out resultQuantity12) ? decimal.Parse(model.abep_quantity_12.Replace(",", "")) : (decimal?)null,
                        abep_amount_1 = !string.IsNullOrEmpty(model.abep_amount_1) && decimal.TryParse(model.abep_amount_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abep_amount_1.Replace(",", "")) : (decimal?)null,
                        abep_amount_2 = !string.IsNullOrEmpty(model.abep_amount_2) && decimal.TryParse(model.abep_amount_2.Replace(",", ""), out resultAmount2) ? decimal.Parse(model.abep_amount_2.Replace(",", "")) : (decimal?)null,
                        abep_amount_3 = !string.IsNullOrEmpty(model.abep_amount_3) && decimal.TryParse(model.abep_amount_3.Replace(",", ""), out resultAmount3) ? decimal.Parse(model.abep_amount_3.Replace(",", "")) : (decimal?)null,
                        abep_amount_4 = !string.IsNullOrEmpty(model.abep_amount_4) && decimal.TryParse(model.abep_amount_4.Replace(",", ""), out resultAmount4) ? decimal.Parse(model.abep_amount_4.Replace(",", "")) : (decimal?)null,
                        abep_amount_5 = !string.IsNullOrEmpty(model.abep_amount_5) && decimal.TryParse(model.abep_amount_5.Replace(",", ""), out resultAmount5) ? decimal.Parse(model.abep_amount_5.Replace(",", "")) : (decimal?)null,
                        abep_amount_6 = !string.IsNullOrEmpty(model.abep_amount_6) && decimal.TryParse(model.abep_amount_6.Replace(",", ""), out resultAmount6) ? decimal.Parse(model.abep_amount_6.Replace(",", "")) : (decimal?)null,
                        abep_amount_7 = !string.IsNullOrEmpty(model.abep_amount_7) && decimal.TryParse(model.abep_amount_7.Replace(",", ""), out resultAmount7) ? decimal.Parse(model.abep_amount_7.Replace(",", "")) : (decimal?)null,
                        abep_amount_8 = !string.IsNullOrEmpty(model.abep_amount_8) && decimal.TryParse(model.abep_amount_8.Replace(",", ""), out resultAmount8) ? decimal.Parse(model.abep_amount_8.Replace(",", "")) : (decimal?)null,
                        abep_amount_9 = !string.IsNullOrEmpty(model.abep_amount_9) && decimal.TryParse(model.abep_amount_9.Replace(",", ""), out resultAmount9) ? decimal.Parse(model.abep_amount_9.Replace(",", "")) : (decimal?)null,
                        abep_amount_10 = !string.IsNullOrEmpty(model.abep_amount_10) && decimal.TryParse(model.abep_amount_10.Replace(",", ""), out resultAmount10) ? decimal.Parse(model.abep_amount_10.Replace(",", "")) : (decimal?)null,
                        abep_amount_11 = !string.IsNullOrEmpty(model.abep_amount_11) && decimal.TryParse(model.abep_amount_11.Replace(",", ""), out resultAmount11) ? decimal.Parse(model.abep_amount_11.Replace(",", "")) : (decimal?)null,
                        abep_amount_12 = !string.IsNullOrEmpty(model.abep_amount_12) && decimal.TryParse(model.abep_amount_12.Replace(",", ""), out resultAmount12) ? decimal.Parse(model.abep_amount_12.Replace(",", "")) : (decimal?)null,
                        abep_total_quantity = !string.IsNullOrEmpty(model.abep_total_quantity) && decimal.TryParse(model.abep_total_quantity.Replace(",", ""), out resultTotalQty) ? decimal.Parse(model.abep_total_quantity.Replace(",", "")) : (decimal?)null,
                        abep_total = !string.IsNullOrEmpty(model.abep_total) && decimal.TryParse(model.abep_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abep_total.Replace(",", "")) : (decimal?)null,
                        abep_proration = model.abep_proration,
                        abep_status = "DRAFT",
                        abep_created_by = getIdentity.ID(User.Identity.Name),
                        last_modified = DateTime.Now
                    };
                    db.bgt_expenses_paus.Add(expenses_pau);
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
                        controller = "ExpensesPAU",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abep_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abep_cost_center_code
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

        // GET: ExpensesPAU/Edit/5
        public ActionResult Edit(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return RedirectToAction("Create");
            }

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter.TrimStart(new char[] { '0' });
            ViewBag.COstCenterDesc = cc.GetCostCenterDesc(CostCenter);

            return View();
        }

        public async Task<ActionResult> EditDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expenses_pau = await db.bgt_expenses_paus.FindAsync(id);
            if (expenses_pau == null)
            {
                return HttpNotFound();
            }

            var gl_buys = new SelectList(GetGLPAU().Select(s => new SelectListItem
            {
                Value = s.GLCode_Buy.TrimStart(new char[] { '0' }),
                Text = s.GLCode_Buy.TrimStart(new char[] { '0' }) + " - " + s.GLCode_Buy_Desc
            }), "Value", "Text").ToList();

            var locationId = cc.GetCostCenterLocID(expenses_pau.abep_cost_center_code);
            var products = new SelectList(GetProducts(expenses_pau.abep_budgeting_year, locationId).Select(s => new SelectListItem
            {
                Value = s.Code,
                Text = s.Code + " - " + s.Name
            }), "Value", "Text").ToList();

            var costCenter_sales = new SelectList(GetCostCenters(expenses_pau.abep_budgeting_year, false, expenses_pau.abep_cost_center_code).Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter.TrimStart(new char[] { '0' }),
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();

            var model = new ExpensesPAUCreateOrEditDetailViewModel
            {
                abep_id = id.Value,
                abep_budgeting_year = expenses_pau.abep_budgeting_year,
                abep_syarikat_id = expenses_pau.abep_syarikat_id,
                abep_syarikat_name = expenses_pau.abep_syarikat_name,
                abep_ladang_id = expenses_pau.abep_ladang_id,
                abep_ladang_code = expenses_pau.abep_ladang_code,
                abep_ladang_name = expenses_pau.abep_ladang_name,
                abep_wilayah_id = expenses_pau.abep_wilayah_id,
                abep_wilayah_name = expenses_pau.abep_wilayah_name,
                abep_cost_center_code = expenses_pau.abep_cost_center_code.TrimStart(new char[] { '0' }),
                abep_cost_center_desc = expenses_pau.abep_cost_center_desc,
                abep_gl_expenses_code = expenses_pau.abep_gl_expenses_code.TrimStart(new char[] { '0' }),
                abep_gl_expenses_name = expenses_pau.abep_gl_expenses_name,
                abep_gl_income_code = expenses_pau.abep_gl_income_code,
                abep_gl_income_name = expenses_pau.abep_gl_income_name,
                abep_product_code = expenses_pau.abep_product_code,
                abep_product_name = expenses_pau.abep_product_name,
                abep_cost_center_jualan_code = expenses_pau.abep_cost_center_jualan_code,
                abep_cost_center_jualan_desc = expenses_pau.abep_cost_center_jualan_desc,
                abep_quantity = expenses_pau.abep_quantity.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity) : string.Empty,
                abep_uom_1 = expenses_pau.abep_uom_1,
                abep_rate = expenses_pau.abep_rate.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_rate) : string.Empty,
                abep_quantity_1 = expenses_pau.abep_quantity_1.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_1) : string.Empty,
                abep_quantity_2 = expenses_pau.abep_quantity_2.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_2) : string.Empty,
                abep_quantity_3 = expenses_pau.abep_quantity_3.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_3) : string.Empty,
                abep_quantity_4 = expenses_pau.abep_quantity_4.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_4) : string.Empty,
                abep_quantity_5 = expenses_pau.abep_quantity_5.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_5) : string.Empty,
                abep_quantity_6 = expenses_pau.abep_quantity_6.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_6) : string.Empty,
                abep_quantity_7 = expenses_pau.abep_quantity_7.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_7) : string.Empty,
                abep_quantity_8 = expenses_pau.abep_quantity_8.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_8) : string.Empty,
                abep_quantity_9 = expenses_pau.abep_quantity_9.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_9) : string.Empty,
                abep_quantity_10 = expenses_pau.abep_quantity_10.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_10) : string.Empty,
                abep_quantity_11 = expenses_pau.abep_quantity_11.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_11) : string.Empty,
                abep_quantity_12 = expenses_pau.abep_quantity_12.HasValue ? string.Format("{0:#,##0.000}", expenses_pau.abep_quantity_12) : string.Empty,
                abep_amount_1 = expenses_pau.abep_amount_1.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_1) : string.Empty,
                abep_amount_2 = expenses_pau.abep_amount_2.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_2) : string.Empty,
                abep_amount_3 = expenses_pau.abep_amount_3.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_3) : string.Empty,
                abep_amount_4 = expenses_pau.abep_amount_4.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_4) : string.Empty,
                abep_amount_5 = expenses_pau.abep_amount_5.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_5) : string.Empty,
                abep_amount_6 = expenses_pau.abep_amount_6.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_6) : string.Empty,
                abep_amount_7 = expenses_pau.abep_amount_7.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_7) : string.Empty,
                abep_amount_8 = expenses_pau.abep_amount_8.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_8) : string.Empty,
                abep_amount_9 = expenses_pau.abep_amount_9.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_9) : string.Empty,
                abep_amount_10 = expenses_pau.abep_amount_10.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_10) : string.Empty,
                abep_amount_11 = expenses_pau.abep_amount_11.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_11) : string.Empty,
                abep_amount_12 = expenses_pau.abep_amount_12.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_12) : string.Empty,
                abep_total_quantity = expenses_pau.abep_total_quantity.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_total_quantity) : string.Empty,
                abep_total = expenses_pau.abep_total_quantity.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_total_quantity) : string.Empty,
                abep_proration = expenses_pau.abep_proration,
                abep_history = expenses_pau.abep_history,
                abep_status = expenses_pau.abep_status,
                abep_created_by = expenses_pau.abep_created_by
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.GLBuys = gl_buys;
            ViewBag.Products = products;
            ViewBag.CostCenterSales = costCenter_sales;

            return PartialView("_EditDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDetail(int? id, ExpensesPAUCreateOrEditDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var resultQuantity = 0m;
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

                    var expenses_pau = db.bgt_expenses_paus.Find(id);
                    expenses_pau.abep_budgeting_year = model.abep_budgeting_year;
                    expenses_pau.abep_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null;
                    expenses_pau.abep_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                    expenses_pau.abep_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null;
                    expenses_pau.abep_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                    expenses_pau.abep_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                    expenses_pau.abep_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null;
                    expenses_pau.abep_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null;
                    expenses_pau.abep_cost_center_code = model.abep_cost_center_code;
                    expenses_pau.abep_cost_center_desc = model.abep_cost_center_desc;
                    expenses_pau.abep_gl_expenses_code = model.abep_gl_expenses_code;
                    expenses_pau.abep_gl_expenses_name = model.abep_gl_expenses_name;
                    expenses_pau.abep_gl_income_code = model.abep_gl_income_code;
                    expenses_pau.abep_gl_income_name = model.abep_gl_income_name;
                    expenses_pau.abep_product_code = model.abep_product_code;
                    expenses_pau.abep_product_name = model.abep_product_name;
                    expenses_pau.abep_cost_center_jualan_code = model.abep_cost_center_jualan_code;
                    expenses_pau.abep_cost_center_jualan_desc = model.abep_cost_center_jualan_desc;
                    expenses_pau.abep_quantity = !string.IsNullOrEmpty(model.abep_quantity) && decimal.TryParse(model.abep_quantity.Replace(",", ""), out resultQuantity) ? decimal.Parse(model.abep_quantity.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_uom_1 = model.abep_uom_1;
                    expenses_pau.abep_rate = !string.IsNullOrEmpty(model.abep_rate) && decimal.TryParse(model.abep_rate.Replace(",", ""), out resultRate) ? decimal.Parse(model.abep_rate.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_1 = !string.IsNullOrEmpty(model.abep_quantity_1) && decimal.TryParse(model.abep_quantity_1.Replace(",", ""), out resultQuantity1) ? decimal.Parse(model.abep_quantity_1.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_2 = !string.IsNullOrEmpty(model.abep_quantity_2) && decimal.TryParse(model.abep_quantity_2.Replace(",", ""), out resultQuantity2) ? decimal.Parse(model.abep_quantity_2.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_3 = !string.IsNullOrEmpty(model.abep_quantity_3) && decimal.TryParse(model.abep_quantity_3.Replace(",", ""), out resultQuantity3) ? decimal.Parse(model.abep_quantity_3.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_4 = !string.IsNullOrEmpty(model.abep_quantity_4) && decimal.TryParse(model.abep_quantity_4.Replace(",", ""), out resultQuantity4) ? decimal.Parse(model.abep_quantity_4.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_5 = !string.IsNullOrEmpty(model.abep_quantity_5) && decimal.TryParse(model.abep_quantity_5.Replace(",", ""), out resultQuantity5) ? decimal.Parse(model.abep_quantity_5.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_6 = !string.IsNullOrEmpty(model.abep_quantity_6) && decimal.TryParse(model.abep_quantity_6.Replace(",", ""), out resultQuantity6) ? decimal.Parse(model.abep_quantity_6.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_7 = !string.IsNullOrEmpty(model.abep_quantity_7) && decimal.TryParse(model.abep_quantity_7.Replace(",", ""), out resultQuantity7) ? decimal.Parse(model.abep_quantity_7.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_8 = !string.IsNullOrEmpty(model.abep_quantity_8) && decimal.TryParse(model.abep_quantity_8.Replace(",", ""), out resultQuantity8) ? decimal.Parse(model.abep_quantity_8.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_9 = !string.IsNullOrEmpty(model.abep_quantity_9) && decimal.TryParse(model.abep_quantity_9.Replace(",", ""), out resultQuantity9) ? decimal.Parse(model.abep_quantity_9.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_10 = !string.IsNullOrEmpty(model.abep_quantity_10) && decimal.TryParse(model.abep_quantity_10.Replace(",", ""), out resultQuantity10) ? decimal.Parse(model.abep_quantity_10.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_11 = !string.IsNullOrEmpty(model.abep_quantity_11) && decimal.TryParse(model.abep_quantity_11.Replace(",", ""), out resultQuantity11) ? decimal.Parse(model.abep_quantity_11.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_quantity_12 = !string.IsNullOrEmpty(model.abep_quantity_12) && decimal.TryParse(model.abep_quantity_12.Replace(",", ""), out resultQuantity12) ? decimal.Parse(model.abep_quantity_12.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_1 = !string.IsNullOrEmpty(model.abep_amount_1) && decimal.TryParse(model.abep_amount_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abep_amount_1.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_2 = !string.IsNullOrEmpty(model.abep_amount_2) && decimal.TryParse(model.abep_amount_2.Replace(",", ""), out resultAmount2) ? decimal.Parse(model.abep_amount_2.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_3 = !string.IsNullOrEmpty(model.abep_amount_3) && decimal.TryParse(model.abep_amount_3.Replace(",", ""), out resultAmount3) ? decimal.Parse(model.abep_amount_3.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_4 = !string.IsNullOrEmpty(model.abep_amount_4) && decimal.TryParse(model.abep_amount_4.Replace(",", ""), out resultAmount4) ? decimal.Parse(model.abep_amount_4.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_5 = !string.IsNullOrEmpty(model.abep_amount_5) && decimal.TryParse(model.abep_amount_5.Replace(",", ""), out resultAmount5) ? decimal.Parse(model.abep_amount_5.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_6 = !string.IsNullOrEmpty(model.abep_amount_6) && decimal.TryParse(model.abep_amount_6.Replace(",", ""), out resultAmount6) ? decimal.Parse(model.abep_amount_6.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_7 = !string.IsNullOrEmpty(model.abep_amount_7) && decimal.TryParse(model.abep_amount_7.Replace(",", ""), out resultAmount7) ? decimal.Parse(model.abep_amount_7.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_8 = !string.IsNullOrEmpty(model.abep_amount_8) && decimal.TryParse(model.abep_amount_8.Replace(",", ""), out resultAmount8) ? decimal.Parse(model.abep_amount_8.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_9 = !string.IsNullOrEmpty(model.abep_amount_9) && decimal.TryParse(model.abep_amount_9.Replace(",", ""), out resultAmount9) ? decimal.Parse(model.abep_amount_9.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_10 = !string.IsNullOrEmpty(model.abep_amount_10) && decimal.TryParse(model.abep_amount_10.Replace(",", ""), out resultAmount10) ? decimal.Parse(model.abep_amount_10.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_11 = !string.IsNullOrEmpty(model.abep_amount_11) && decimal.TryParse(model.abep_amount_11.Replace(",", ""), out resultAmount11) ? decimal.Parse(model.abep_amount_11.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_amount_12 = !string.IsNullOrEmpty(model.abep_amount_12) && decimal.TryParse(model.abep_amount_12.Replace(",", ""), out resultAmount12) ? decimal.Parse(model.abep_amount_12.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_total_quantity = !string.IsNullOrEmpty(model.abep_total_quantity) && decimal.TryParse(model.abep_total_quantity.Replace(",", ""), out resultTotalQty) ? decimal.Parse(model.abep_total_quantity.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_total = !string.IsNullOrEmpty(model.abep_total) && decimal.TryParse(model.abep_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abep_total.Replace(",", "")) : (decimal?)null;
                    expenses_pau.abep_proration = model.abep_proration;
                    expenses_pau.last_modified = DateTime.Now;

                    db.Entry(expenses_pau).State = EntityState.Modified;
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
                        controller = "ExpensesPAU",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abep_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abep_cost_center_code
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

        public async Task<ActionResult> Delete(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_pau = await db.bgt_expenses_paus.Where(p => p.abep_budgeting_year == year && p.abep_cost_center_code.Contains(CostCenter) && !p.abep_deleted).FirstOrDefaultAsync();
            if (expenses_pau == null)
            {
                return HttpNotFound();
            }

            var total_amount = db.bgt_expenses_paus
                .Where(p => p.abep_budgeting_year == expenses_pau.abep_budgeting_year && p.abep_cost_center_code.Equals(expenses_pau.abep_cost_center_code) && !p.abep_deleted)
                .Sum(p => p.abep_total);
            var model = new ExpensesPAUDeleteViewModel
            {
                BudgetYear = expenses_pau.abep_budgeting_year,
                CostCenterCode = expenses_pau.abep_cost_center_code.TrimStart(new char[] { '0' }),
                CostCenterDesc = expenses_pau.abep_cost_center_desc,
                Total = string.Format("{0:#,##0.00}", total_amount)
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Delete", model);
        }

        // POST: ExpensesPAU/Delete/5
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
                var expenses_paus = db.bgt_expenses_paus
                    .Where(p => p.abep_budgeting_year == year && p.abep_cost_center_code.Contains(CostCenter) && !p.abep_deleted)
                    .ToList();

                if (expenses_paus.Count > 0)
                {
                    foreach (var item in expenses_paus)
                    {
                        //item.abep_deleted = true;
                        //item.last_modified = DateTime.Now;
                        //db.Entry(item).State = EntityState.Modified;
                        db.bgt_expenses_paus.Remove(item);
                    }
                    await db.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgDelete2,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesPAU",
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
                        controller = "ExpensesPAU",
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
            var expenses_pau = await db.bgt_expenses_paus.FindAsync(id);
            if (expenses_pau == null)
            {
                return HttpNotFound();
            }

            var model = new ExpensesPAUDeleteDetailViewModel
            {
                abep_id = id.Value,
                abep_budgeting_year = expenses_pau.abep_budgeting_year,
                abep_syarikat_id = expenses_pau.abep_syarikat_id,
                abep_syarikat_name = expenses_pau.abep_syarikat_name,
                abep_ladang_id = expenses_pau.abep_ladang_id,
                abep_ladang_code = expenses_pau.abep_ladang_code,
                abep_ladang_name = expenses_pau.abep_ladang_name,
                abep_wilayah_id = expenses_pau.abep_wilayah_id,
                abep_wilayah_name = expenses_pau.abep_wilayah_name,
                abep_cost_center_code = expenses_pau.abep_cost_center_code.TrimStart(new char[] { '0' }),
                abep_cost_center_desc = expenses_pau.abep_cost_center_desc,
                abep_gl_expenses_code = expenses_pau.abep_gl_expenses_code.TrimStart(new char[] { '0' }),
                abep_gl_expenses_name = expenses_pau.abep_gl_expenses_name,
                abep_gl_income_code = expenses_pau.abep_gl_income_code.TrimStart(new char[] { '0' }),
                abep_gl_income_name = expenses_pau.abep_gl_income_name,
                abep_product_code = expenses_pau.abep_product_code,
                abep_product_name = expenses_pau.abep_product_name,
                abep_cost_center_jualan_code = expenses_pau.abep_cost_center_jualan_code.TrimStart(new char[] { '0' }),
                abep_cost_center_jualan_desc = expenses_pau.abep_cost_center_jualan_desc,
                abep_quantity = expenses_pau.abep_quantity,
                abep_uom_1 = expenses_pau.abep_uom_1,
                abep_rate = expenses_pau.abep_rate,
                abep_quantity_1 = expenses_pau.abep_quantity_1,
                abep_quantity_2 = expenses_pau.abep_quantity_2,
                abep_quantity_3 = expenses_pau.abep_quantity_3,
                abep_quantity_4 = expenses_pau.abep_quantity_4,
                abep_quantity_5 = expenses_pau.abep_quantity_5,
                abep_quantity_6 = expenses_pau.abep_quantity_6,
                abep_quantity_7 = expenses_pau.abep_quantity_7,
                abep_quantity_8 = expenses_pau.abep_quantity_8,
                abep_quantity_9 = expenses_pau.abep_quantity_9,
                abep_quantity_10 = expenses_pau.abep_quantity_10,
                abep_quantity_11 = expenses_pau.abep_quantity_11,
                abep_quantity_12 = expenses_pau.abep_quantity_12,
                abep_amount_1 = expenses_pau.abep_amount_1.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_1) : string.Empty,
                abep_amount_2 = expenses_pau.abep_amount_2.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_2) : string.Empty,
                abep_amount_3 = expenses_pau.abep_amount_3.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_3) : string.Empty,
                abep_amount_4 = expenses_pau.abep_amount_4.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_4) : string.Empty,
                abep_amount_5 = expenses_pau.abep_amount_5.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_5) : string.Empty,
                abep_amount_6 = expenses_pau.abep_amount_6.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_6) : string.Empty,
                abep_amount_7 = expenses_pau.abep_amount_7.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_7) : string.Empty,
                abep_amount_8 = expenses_pau.abep_amount_8.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_8) : string.Empty,
                abep_amount_9 = expenses_pau.abep_amount_9.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_9) : string.Empty,
                abep_amount_10 = expenses_pau.abep_amount_10.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_10) : string.Empty,
                abep_amount_11 = expenses_pau.abep_amount_11.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_11) : string.Empty,
                abep_amount_12 = expenses_pau.abep_amount_12.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_amount_12) : string.Empty,
                abep_total_quantity = expenses_pau.abep_total_quantity.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_total_quantity) : string.Empty,
                abep_total = expenses_pau.abep_total.HasValue ? string.Format("{0:#,##0.00}", expenses_pau.abep_total) : string.Empty
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
                var expenses_pau = db.bgt_expenses_paus.Find(id);
                //expenses_pau.abep_deleted = true;
                //expenses_pau.last_modified = DateTime.Now;
                //db.Entry(expenses_pau).State = EntityState.Modified;
                db.bgt_expenses_paus.Remove(expenses_pau);
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
                    controller = "ExpensesPAU",
                    action = "RecordsDetail",
                    paramName1 = "BudgetYear",
                    paramValue1 = expenses_pau.abep_budgeting_year.ToString(),
                    paramName2 = "CostCenter",
                    paramValue2 = expenses_pau.abep_cost_center_code
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesPAUList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesPAUListViewModel> data = db.Database.SqlQuery<ExpensesPAUListViewModel>("exec sp_BudgetExpensesPAUList {0}, {1}, {2}, {3}, {4}", BudgetYear, CostCenter, syarikat.GetSyarikatID().ToString(), ladang.GetLadangID().ToString(), wilayah.GetWilayahID().ToString()).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintView(string BudgetYear, string CostCenter, string Format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesPAUView.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            if (Format.ToLower().Contains("excel"))
            {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesPAUViewExcel.rdlc");
            }

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesPAUListDetailViewModel> data = db.Database.SqlQuery<ExpensesPAUListDetailViewModel>("exec sp_BudgetExpensesPAUView {0}, {1}, {2}, {3}, {4}", BudgetYear, CostCenter, syarikat.GetSyarikatID().ToString(), ladang.GetLadangID().ToString(), wilayah.GetWilayahID().ToString()).ToList();
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

        private List<tbl_SAPCCPUP> GetCostCenters(int budgetYear, bool exclude = true, string excludedCostCenterCode = null)
        {
            if (exclude)
            {
                var expenses_paus = GetRecords(budgetYear);
                var excludeCostCenters = new List<string>();

                for (int i = 0; i < expenses_paus.Count(); i++)
                {
                    excludeCostCenters.Add(expenses_paus[i].CostCenterCode.PadLeft(10, '0'));
                }

                return cc.GetCostCenters()
                    .Where(c => !excludeCostCenters.Contains(c.fld_CostCenter))
                    .OrderBy(c => c.fld_CostCenter)
                    .ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(excludedCostCenterCode))
                {
                    return cc.GetCostCenters()
                        .Where(c => !c.fld_CostCenter.Contains(excludedCostCenterCode))
                        .OrderBy(c => c.fld_CostCenter)
                        .ToList();
                }
                else
                {
                    return cc.GetCostCenters();
                }
            }
        }

        private List<GLListViewModel> GetGLs(int? budgetYear = null, string costCenter = null, int? excludeRecordId = null)
        {
            var screen = scr.GetScreen(screenCode);
            var expenses_paus = GetRecordsDetail(budgetYear, costCenter, null, null, excludeRecordId);
            var excludeGLs = new List<string>();

            for (int i = 0; i < expenses_paus.Count(); i++)
            {
                excludeGLs.Add(expenses_paus[i].abep_gl_expenses_code);
            }

            return gl.GetGLs(screen.ScrID)
                .Where(g => !excludeGLs.Contains(g.fld_GLCode.TrimStart(new char[] { '0' })))
                .Select(g => new GLListViewModel { Id = g.fld_ID, Code = g.fld_GLCode, Description = g.fld_GLDesc })
                .ToList();
        }

        private List<bgt_PAU> GetGLPAU()
        {
            return dbc.bgt_PAU.ToList();
        }

        public JsonResult GetGLPAUSaleByCodeBuy(string glcode_buy)
        {
            var gl = dbc.bgt_PAU
                .Where(p => p.GLCode_Buy.Contains(glcode_buy))
                .FirstOrDefault();
            if (gl != null)
            {
                return Json(new
                {
                    gl = gl,
                    glCodeSale = gl.GLCode_Sale.TrimStart(new char[] { '0' })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    gl = new bgt_PAU(),
                    glCodeSale = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //private List<bgt_Product> GetProducts(int budgetYear, int? wilayahId = null)
        //{
        //    if (wilayahId == 1)
        //    {
        //        return dbc.bgt_Product
        //            .Where(p => p.YearBgt == budgetYear && p.Lowest && (p.LocID == 1 || p.LocID == 2))
        //            .OrderBy(p => p.PrdCode)
        //            .ToList();
        //    }
        //    else if (wilayahId == 2)
        //    {
        //        return dbc.bgt_Product
        //            .Where(p => p.YearBgt == budgetYear && p.Lowest && (p.LocID != 1 && p.LocID != 2))
        //            .OrderBy(p => p.PrdCode)
        //            .ToList();
        //    }
        //    return dbc.bgt_Product
        //        .Where(p => p.YearBgt == budgetYear && p.Lowest)
        //        .OrderBy(p => p.PrdCode)
        //        .ToList();
        //}
        private List<ProductListViewModel> GetProducts(int budgetYear, int? locationId = null)
        {
            var locCode = -1;
            var location = dbc.bgt_Location.Find(locationId);
            if (location != null)
            {
                locCode = int.Parse(location.LocCode);
            }
            return dbc.bgt_Product
                .Where(p => p.YearBgt == budgetYear && p.Lowest && (p.LocID == 0 || p.LocID == 5 || p.LocID == locCode))
                .GroupBy(p => new { p.PrdCode, p.PrdName, p.LocID, p.PricePAU, p.Active })
                .OrderBy(g => g.Key.PrdCode)
                .Select(g => new ProductListViewModel
                {
                    Year = budgetYear,
                    Code = g.Key.PrdCode,
                    Name = g.Key.PrdName,
                    LocCode = g.Key.LocID,
                    PricePAU = g.Key.PricePAU
                })
                .ToList();
        }

        public JsonResult GetProductByCode(string code)
        {
            var product = dbc.bgt_Product
                .Where(p => p.PrdCode.Equals(code) && p.Active)
                .FirstOrDefault();
            if (product != null)
            {
                return Json(new
                {
                    product = product
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                product = new bgt_Product()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductUOMByCode(string productcode)
        {
            var UOMname = string.Empty;
            var product = dbc.bgt_Product
                .Where(p => p.PrdCode.Equals(productcode) && p.Active)
                .FirstOrDefault();
            if (product != null)
            {
                if (product.UOMID.HasValue)
                {
                    var uom = dbc.bgt_UOM.Find(product.UOMID);
                    if (uom != null)
                        UOMname = uom.UOM;
                }
            }
            return Json(new
            {
                UOM = UOMname
            }, JsonRequestBehavior.AllowGet);
        }

        private List<ExpensesPAUListViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from p in db.bgt_expenses_paus
                        where !p.abep_deleted
                        group p by new { p.abep_budgeting_year, p.abep_syarikat_id, p.abep_ladang_id, p.abep_wilayah_id, p.abep_cost_center_code, p.abep_cost_center_desc } into g
                        select new
                        {
                            BudgetYear = g.Key.abep_budgeting_year,
                            SyarikatId = g.Key.abep_syarikat_id,
                            LadangId = g.Key.abep_ladang_id,
                            WilayahId = g.Key.abep_wilayah_id,
                            CostCenterCode = g.Key.abep_cost_center_code,
                            CostCenterDesc = g.Key.abep_cost_center_desc,
                            Total = g.Sum(p => p.abep_total)
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
                          select new ExpensesPAUListViewModel
                          {
                              BudgetYear = q.BudgetYear,
                              CostCenterCode = q.CostCenterCode.TrimStart(new char[] { '0' }),
                              CostCenterDesc = q.CostCenterDesc,
                              Total = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        private List<ExpensesPAUListDetailViewModel> GetRecordsDetail(int? budgeting_year = null, string cost_center = null, string gl_expenses = null, string gl_income = null, int? excludeId = null, int? page = null, int? pageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from p in db.bgt_expenses_paus
                        where !p.abep_deleted
                        select p;

            if (budgeting_year.HasValue)
            {
                query = query.Where(q => q.abep_budgeting_year == budgeting_year);
                if (budgeting_year.Value > 2023)
                {
                    query = query.Where(q => q.abep_syarikat_id == syarikatId && q.abep_ladang_id == ladangId && q.abep_wilayah_id == wilayahId);
                }
            }
            if (!string.IsNullOrEmpty(cost_center))
                query = query.Where(q => q.abep_cost_center_code.Contains(cost_center) || q.abep_cost_center_code.Contains(cost_center));
            if (!string.IsNullOrEmpty(gl_expenses))
                query = query.Where(q => q.abep_gl_expenses_code.Contains(gl_expenses) || q.abep_gl_expenses_name.Contains(gl_expenses));
            if (!string.IsNullOrEmpty(gl_income))
                query = query.Where(q => q.abep_gl_income_code.Contains(gl_income) || q.abep_gl_income_name.Contains(gl_income));
            if (excludeId.HasValue)
                query = query.Where(q => q.abep_id != excludeId);

            var query2 = query.AsQueryable();

            if (page.HasValue && pageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abep_budgeting_year)
                    .ThenBy(q => q.abep_cost_center_code)
                    .ThenBy(q => q.abep_product_name)
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abep_budgeting_year)
                    .ThenBy(q => q.abep_cost_center_code)
                    .ThenBy(q => q.abep_product_name);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesPAUListDetailViewModel
                          {
                              abep_id = q.abep_id,
                              abep_revision = q.abep_revision,
                              abep_budgeting_year = q.abep_budgeting_year,
                              abep_syarikat_id = q.abep_syarikat_id,
                              abep_syarikat_name = q.abep_syarikat_name,
                              abep_ladang_id = q.abep_ladang_id,
                              abep_ladang_code = q.abep_ladang_code,
                              abep_ladang_name = q.abep_ladang_name,
                              abep_wilayah_id = q.abep_wilayah_id,
                              abep_wilayah_name = q.abep_wilayah_name,
                              abep_cost_center_code = q.abep_cost_center_code.TrimStart(new char[] { '0' }),
                              abep_cost_center_desc = q.abep_cost_center_desc,
                              abep_gl_expenses_code = q.abep_gl_expenses_code.TrimStart(new char[] { '0' }),
                              abep_gl_expenses_name = q.abep_gl_expenses_name,
                              abep_gl_income_code = q.abep_gl_income_code.TrimStart(new char[] { '0' }),
                              abep_gl_income_name = q.abep_gl_income_name,
                              abep_product_code = q.abep_product_code,
                              abep_product_name = q.abep_product_name,
                              abep_cost_center_jualan_code = q.abep_cost_center_jualan_code.TrimStart(new char[] { '0' }),
                              abep_cost_center_jualan_desc = q.abep_cost_center_jualan_desc,
                              abep_quantity = q.abep_quantity,
                              abep_uom_1 = q.abep_uom_1,
                              abep_rate = q.abep_rate,
                              abep_quantity_1 = q.abep_quantity_1,
                              abep_quantity_2 = q.abep_quantity_2,
                              abep_quantity_3 = q.abep_quantity_3,
                              abep_quantity_4 = q.abep_quantity_4,
                              abep_quantity_5 = q.abep_quantity_5,
                              abep_quantity_6 = q.abep_quantity_6,
                              abep_quantity_7 = q.abep_quantity_7,
                              abep_quantity_8 = q.abep_quantity_8,
                              abep_quantity_9 = q.abep_quantity_9,
                              abep_quantity_10 = q.abep_quantity_10,
                              abep_quantity_11 = q.abep_quantity_11,
                              abep_quantity_12 = q.abep_quantity_12,
                              abep_amount_1 = q.abep_amount_1,
                              abep_amount_2 = q.abep_amount_2,
                              abep_amount_3 = q.abep_amount_3,
                              abep_amount_4 = q.abep_amount_4,
                              abep_amount_5 = q.abep_amount_5,
                              abep_amount_6 = q.abep_amount_6,
                              abep_amount_7 = q.abep_amount_7,
                              abep_amount_8 = q.abep_amount_8,
                              abep_amount_9 = q.abep_amount_9,
                              abep_amount_10 = q.abep_amount_10,
                              abep_amount_11 = q.abep_amount_11,
                              abep_amount_12 = q.abep_amount_12,
                              abep_total_quantity = q.abep_total_quantity,
                              abep_total = q.abep_total,
                              abep_proration = q.abep_proration,
                              abep_history = q.abep_history,
                              abep_status = q.abep_status,
                              abep_created_by = q.abep_created_by
                          }).Distinct();

            return query3.ToList();
        }
        #endregion
    }
}
