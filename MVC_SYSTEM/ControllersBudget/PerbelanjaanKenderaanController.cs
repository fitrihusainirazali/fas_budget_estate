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
    public class PerbelanjaanKenderaanController : Controller
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
        private readonly string screenCode = "E14";
        private Vehicle vehicle = new Vehicle();
        GetBudgetClass GetBudgetClass = new GetBudgetClass();

        // GET: PerbelanjaanKenderaan
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

            var records = new ViewingModels.PagedList<ExpensesVehicleListViewModel2>
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
            var costcenter = !string.IsNullOrEmpty(CostCenter) ? CostCenter : null;

            var records = new ViewingModels.PagedList<ExpensesVehicleListDetailViewModel2>
            {
                Content = GetRecordsDetail(budgetYear, CostCenter, null, null, /*null,*/ page, pageSize),
                TotalRecords = GetRecordsDetail(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };
            ViewBag.CostCenter = costcenter;
            return PartialView("_RecordsDetail", records);
        }

        // GET: PerbelanjaanKenderaan/Details/5
        public ActionResult Details(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_vehicle = GetRecordsDetail(year, CostCenter);

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Details", expenses_vehicle);
        }

        // GET: PerbelanjaanKenderaan/Create
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
                Value = s.abvr_cost_center_code.TrimStart(new char[] { '0' }),
                Text = s.abvr_cost_center_code.TrimStart(new char[] { '0' }) + " - " + s.abvr_cost_center_desc
            }), "Value", "Text").ToList();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = selectedYear;
            ViewBag.Years = years;
            ViewBag.CostCenters = costCenters;

            return View();
        }

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

            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : GetYears().FirstOrDefault();

            //Kod Kenderaan
            var vehicle_list = new SelectList(GetVehicle(year, CostCenter, false).Select(s => new SelectListItem
            {
                Value = s.abvr_material_code,
                Text = s.abvr_material_code + " - " + s.abvr_model.ToUpper() + " ("+ s.abvr_register_no.ToUpper() +")"
            }), "Value", "Text").ToList();

            //GL Perbelanjaan
            var gl_list = new SelectList(GetGL((int)syarikat.GetSyarikatID(), false, year, CostCenter).Select(s => new SelectListItem
            {
                Value = s.fld_GLCode.TrimStart(new char[] { '0' }),
                Text = s.fld_GLCode.TrimStart(new char[] { '0' }) + " - " + s.fld_GLDesc
            }), "Value", "Text").ToList();

            //UOM 
            var uom_list = new SelectList(GetUOM().Select(s => new SelectListItem
            {
                Value = s.UOM,
                Text = s.UOM
            }), "Value", "Text").ToList();


            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
            ViewBag.VehicleList = vehicle_list;
            ViewBag.GLList = gl_list;
            ViewBag.UOM = uom_list;

            return PartialView("_CreateDetail");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDetail(ExpensesVehicleCreateOrEditDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GetIdentity getIdentity = new GetIdentity();

                    var resultRate = 0m;
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

                    var expenses_vehicle = new bgt_expenses_vehicle
                    { 
                        abev_budgeting_year = model.abev_budgeting_year,
                        abev_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null,
                        abev_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                        abev_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null,
                        abev_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                        abev_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                        abev_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null,
                        abev_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                        abev_cost_center_code = model.abev_cost_center_code,
                        abev_cost_center_desc = model.abev_cost_center_desc,
                        abev_gl_expenses_code = model.abev_gl_expenses_code,
                        abev_gl_expenses_name = model.abev_gl_expenses_name,
                        abev_material_code = model.abev_material_code,
                        abev_material_name = model.abev_material_name,
                        abev_model = model.abev_model,
                        abev_registration_no = model.abev_registration_no,
                        abev_note = model.abev_note,
                        abev_quantity = !string.IsNullOrEmpty(model.abev_quantity) && decimal.TryParse(model.abev_quantity.Replace(",", ""), out resultRate) ? decimal.Parse(model.abev_quantity.Replace(",", "")) : (decimal?)null,
                        abev_uom = model.abev_uom,
                        abev_unit_price = !string.IsNullOrEmpty(model.abev_unit_price) && decimal.TryParse(model.abev_unit_price.Replace(",", ""), out resultRate) ? decimal.Parse(model.abev_unit_price.Replace(",", "")) : (decimal?)null,
                        //abev_quantity_1 = model.abev_quantity_1,
                        //abev_quantity_2 = model.abev_quantity_2,
                        //abev_quantity_3 = model.abev_quantity_3,
                        //abev_quantity_4 = model.abev_quantity_4,
                        //abev_quantity_5 = model.abev_quantity_5,
                        //abev_quantity_6 = model.abev_quantity_6,
                        //abev_quantity_7 = model.abev_quantity_7,
                        //abev_quantity_8 = model.abev_quantity_8,
                        //abev_quantity_9 = model.abev_quantity_9,
                        //abev_quantity_10 = model.abev_quantity_10,
                        //abev_quantity_11 = model.abev_quantity_11,
                        //abev_quantity_12 = model.abev_quantity_12,
                    abev_quantity_1 = !string.IsNullOrEmpty(model.abev_quantity_1) && decimal.TryParse(model.abev_quantity_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_1.Replace(",", "")) : (decimal?)null,
                    abev_quantity_2 = !string.IsNullOrEmpty(model.abev_quantity_2) && decimal.TryParse(model.abev_quantity_2.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_2.Replace(",", "")) : (decimal?)null,
                    abev_quantity_3 = !string.IsNullOrEmpty(model.abev_quantity_3) && decimal.TryParse(model.abev_quantity_3.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_3.Replace(",", "")) : (decimal?)null,
                    abev_quantity_4 = !string.IsNullOrEmpty(model.abev_quantity_4) && decimal.TryParse(model.abev_quantity_4.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_4.Replace(",", "")) : (decimal?)null,
                    abev_quantity_5 = !string.IsNullOrEmpty(model.abev_quantity_5) && decimal.TryParse(model.abev_quantity_5.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_5.Replace(",", "")) : (decimal?)null,
                    abev_quantity_6 = !string.IsNullOrEmpty(model.abev_quantity_6) && decimal.TryParse(model.abev_quantity_6.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_6.Replace(",", "")) : (decimal?)null,
                    abev_quantity_7 = !string.IsNullOrEmpty(model.abev_quantity_7) && decimal.TryParse(model.abev_quantity_7.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_7.Replace(",", "")) : (decimal?)null,
                    abev_quantity_8 = !string.IsNullOrEmpty(model.abev_quantity_8) && decimal.TryParse(model.abev_quantity_8.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_8.Replace(",", "")) : (decimal?)null,
                    abev_quantity_9 = !string.IsNullOrEmpty(model.abev_quantity_9) && decimal.TryParse(model.abev_quantity_9.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_9.Replace(",", "")) : (decimal?)null,
                    abev_quantity_10 = !string.IsNullOrEmpty(model.abev_quantity_10) && decimal.TryParse(model.abev_quantity_10.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_10.Replace(",", "")) : (decimal?)null,
                    abev_quantity_11 = !string.IsNullOrEmpty(model.abev_quantity_11) && decimal.TryParse(model.abev_quantity_11.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_11.Replace(",", "")) : (decimal?)null,
                    abev_quantity_12 = !string.IsNullOrEmpty(model.abev_quantity_12) && decimal.TryParse(model.abev_quantity_12.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_12.Replace(",", "")) : (decimal?)null,
                    
                        abev_amount_1 = !string.IsNullOrEmpty(model.abev_amount_1) && decimal.TryParse(model.abev_amount_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_1.Replace(",", "")) : (decimal?)null,
                        abev_amount_2 = !string.IsNullOrEmpty(model.abev_amount_2) && decimal.TryParse(model.abev_amount_2.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_2.Replace(",", "")) : (decimal?)null,
                        abev_amount_3 = !string.IsNullOrEmpty(model.abev_amount_3) && decimal.TryParse(model.abev_amount_3.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_3.Replace(",", "")) : (decimal?)null,
                        abev_amount_4 = !string.IsNullOrEmpty(model.abev_amount_4) && decimal.TryParse(model.abev_amount_4.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_4.Replace(",", "")) : (decimal?)null,
                        abev_amount_5 = !string.IsNullOrEmpty(model.abev_amount_5) && decimal.TryParse(model.abev_amount_5.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_5.Replace(",", "")) : (decimal?)null,
                        abev_amount_6 = !string.IsNullOrEmpty(model.abev_amount_6) && decimal.TryParse(model.abev_amount_6.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_6.Replace(",", "")) : (decimal?)null,
                        abev_amount_7 = !string.IsNullOrEmpty(model.abev_amount_7) && decimal.TryParse(model.abev_amount_7.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_7.Replace(",", "")) : (decimal?)null,
                        abev_amount_8 = !string.IsNullOrEmpty(model.abev_amount_8) && decimal.TryParse(model.abev_amount_8.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_8.Replace(",", "")) : (decimal?)null,
                        abev_amount_9 = !string.IsNullOrEmpty(model.abev_amount_9) && decimal.TryParse(model.abev_amount_9.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_9.Replace(",", "")) : (decimal?)null,
                        abev_amount_10 = !string.IsNullOrEmpty(model.abev_amount_10) && decimal.TryParse(model.abev_amount_10.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_10.Replace(",", "")) : (decimal?)null,
                        abev_amount_11 = !string.IsNullOrEmpty(model.abev_amount_11) && decimal.TryParse(model.abev_amount_11.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_11.Replace(",", "")) : (decimal?)null,
                        abev_amount_12 = !string.IsNullOrEmpty(model.abev_amount_12) && decimal.TryParse(model.abev_amount_12.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_12.Replace(",", "")) : (decimal?)null,
                        abev_total_quantity = !string.IsNullOrEmpty(model.abev_total_quantity) && decimal.TryParse(model.abev_total_quantity.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_total_quantity.Replace(",", "")) : (decimal?)null,
                        abev_total = !string.IsNullOrEmpty(model.abev_total) && decimal.TryParse(model.abev_total.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_total.Replace(",", "")) : (decimal?)null,
                        abev_proration = model.abev_proration,
                        abev_revision = 0,//
                        abev_history = false,//
                        abev_status = "DRAFT",
                        abev_created_by = getIdentity.ID(User.Identity.Name),
                        abev_deleted = false,//
                        last_modified = DateTime.Now
                    };
                    db.bgt_expenses_vehicle.Add(expenses_vehicle);
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
                        controller = "PerbelanjaanKenderaan",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abev_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abev_cost_center_code
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

        // GET: PerbelanjaanKenderaan/Edit/5
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

        public async Task<ActionResult> EditDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expenses_vehicle = await db.bgt_expenses_vehicle.FindAsync(id);
            if (expenses_vehicle == null)
            {
                return HttpNotFound();
            }

            //Kod Kenderaan
            var vehicle_list = new SelectList(GetVehicle(expenses_vehicle.abev_budgeting_year, expenses_vehicle.abev_cost_center_code, false).Select(s => new SelectListItem
            {
                Value = s.abvr_material_code,
                Text = s.abvr_material_code + " - " + s.abvr_model.ToUpper() + " (" + s.abvr_register_no.ToUpper() + ")"
            }), "Value", "Text").ToList();

            //GL Perbelanjaan
            var gl_list = new SelectList(GetGL((int)expenses_vehicle.abev_syarikat_id, false, expenses_vehicle.abev_budgeting_year, expenses_vehicle.abev_cost_center_code).Select(s => new SelectListItem
            {
                Value = s.fld_GLCode.TrimStart(new char[] { '0' }),
                Text = s.fld_GLCode.TrimStart(new char[] { '0' }) + " - " + s.fld_GLDesc
            }), "Value", "Text", expenses_vehicle.abev_gl_expenses_code).ToList();

            //UOM 
            var uom_list = new SelectList(GetUOM().Select(s => new SelectListItem
            {
                Value = s.UOM,
                Text = s.UOM
            }), "Value", "Text").ToList();

            var model = new ExpensesVehicleCreateOrEditDetailViewModel
            {   
                abev_revision = expenses_vehicle.abev_revision,//
                abev_id = expenses_vehicle.abev_id,//
                abev_budgeting_year = expenses_vehicle.abev_budgeting_year,
                abev_syarikat_id = expenses_vehicle.abev_syarikat_id,
                abev_syarikat_name = expenses_vehicle.abev_syarikat_name,
                abev_ladang_id = expenses_vehicle.abev_ladang_id,
                abev_ladang_code = expenses_vehicle.abev_ladang_code,
                abev_ladang_name = expenses_vehicle.abev_ladang_name,
                abev_wilayah_id = expenses_vehicle.abev_wilayah_id,
                abev_wilayah_name = expenses_vehicle.abev_wilayah_name,
                abev_cost_center_code = expenses_vehicle.abev_cost_center_code,
                abev_cost_center_desc = expenses_vehicle.abev_cost_center_desc,
                abev_gl_expenses_code = expenses_vehicle.abev_gl_expenses_code,
                abev_gl_expenses_name = expenses_vehicle.abev_gl_expenses_name,
                abev_material_code = expenses_vehicle.abev_material_code,
                abev_material_name = expenses_vehicle.abev_material_name,
                abev_model = expenses_vehicle.abev_model,
                abev_registration_no = expenses_vehicle.abev_registration_no,
                abev_note = expenses_vehicle.abev_note,
                abev_quantity = expenses_vehicle.abev_quantity.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity) : string.Empty,
                abev_uom = expenses_vehicle.abev_uom,
                abev_unit_price = expenses_vehicle.abev_unit_price.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_unit_price) : string.Empty,
                //abev_quantity_1 = expenses_vehicle.abev_quantity_1,
                //abev_quantity_2 = expenses_vehicle.abev_quantity_2,
                //abev_quantity_3 = expenses_vehicle.abev_quantity_3,
                //abev_quantity_4 = expenses_vehicle.abev_quantity_4,
                //abev_quantity_5 = expenses_vehicle.abev_quantity_5,
                //abev_quantity_6 = expenses_vehicle.abev_quantity_6,
                //abev_quantity_7 = expenses_vehicle.abev_quantity_7,
                //abev_quantity_8 = expenses_vehicle.abev_quantity_8,
                //abev_quantity_9 = expenses_vehicle.abev_quantity_9,
                //abev_quantity_10 = expenses_vehicle.abev_quantity_10,
                //abev_quantity_11 = expenses_vehicle.abev_quantity_11,
                //abev_quantity_12 = expenses_vehicle.abev_quantity_12,
                abev_quantity_1 = expenses_vehicle.abev_quantity_1.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_1) : string.Empty,
                abev_quantity_2 = expenses_vehicle.abev_quantity_2.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_2) : string.Empty,
                abev_quantity_3 = expenses_vehicle.abev_quantity_3.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_3) : string.Empty,
                abev_quantity_4 = expenses_vehicle.abev_quantity_4.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_4) : string.Empty,
                abev_quantity_5 = expenses_vehicle.abev_quantity_5.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_5) : string.Empty,
                abev_quantity_6 = expenses_vehicle.abev_quantity_6.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_6) : string.Empty,
                abev_quantity_7 = expenses_vehicle.abev_quantity_7.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_7) : string.Empty,
                abev_quantity_8 = expenses_vehicle.abev_quantity_8.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_8) : string.Empty,
                abev_quantity_9 = expenses_vehicle.abev_quantity_9.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_9) : string.Empty,
                abev_quantity_10 = expenses_vehicle.abev_quantity_10.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_10) : string.Empty,
                abev_quantity_11 = expenses_vehicle.abev_quantity_11.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_11) : string.Empty,
                abev_quantity_12 = expenses_vehicle.abev_quantity_12.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_quantity_12) : string.Empty,
                abev_amount_1 = expenses_vehicle.abev_amount_1.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_1) : string.Empty,
                abev_amount_2 = expenses_vehicle.abev_amount_2.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_2) : string.Empty,
                abev_amount_3 = expenses_vehicle.abev_amount_3.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_3) : string.Empty,
                abev_amount_4 = expenses_vehicle.abev_amount_4.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_4) : string.Empty,
                abev_amount_5 = expenses_vehicle.abev_amount_5.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_5) : string.Empty,
                abev_amount_6 = expenses_vehicle.abev_amount_6.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_6) : string.Empty,
                abev_amount_7 = expenses_vehicle.abev_amount_7.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_7) : string.Empty,
                abev_amount_8 = expenses_vehicle.abev_amount_8.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_8) : string.Empty,
                abev_amount_9 = expenses_vehicle.abev_amount_9.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_9) : string.Empty,
                abev_amount_10 = expenses_vehicle.abev_amount_10.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_10) : string.Empty,
                abev_amount_11 = expenses_vehicle.abev_amount_11.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_11) : string.Empty,
                abev_amount_12 = expenses_vehicle.abev_amount_12.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_12) : string.Empty,
                abev_total_quantity = expenses_vehicle.abev_total_quantity.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_total_quantity) : string.Empty,
                abev_total = expenses_vehicle.abev_total_quantity.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_total_quantity) : string.Empty,
                abev_proration = expenses_vehicle.abev_proration,
                abev_history = (bool)expenses_vehicle.abev_history,
                abev_status = expenses_vehicle.abev_status,
                abev_created_by = expenses_vehicle.abev_created_by
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.VehicleList = vehicle_list;
            ViewBag.GLList = gl_list;
            ViewBag.UOM = uom_list;

            return PartialView("_EditDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDetail(int? id, ExpensesVehicleCreateOrEditDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var resultRate = 0m;
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

                    var expenses_vehicle = db.bgt_expenses_vehicle.Find(id);
                    expenses_vehicle.abev_budgeting_year = model.abev_budgeting_year;
                    expenses_vehicle.abev_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null;
                    expenses_vehicle.abev_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                    expenses_vehicle.abev_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null;
                    expenses_vehicle.abev_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                    expenses_vehicle.abev_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                    expenses_vehicle.abev_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null;
                    expenses_vehicle.abev_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null;
                    expenses_vehicle.abev_cost_center_code = model.abev_cost_center_code;
                    expenses_vehicle.abev_cost_center_desc = model.abev_cost_center_desc;
                    expenses_vehicle.abev_gl_expenses_code = model.abev_gl_expenses_code;
                    expenses_vehicle.abev_gl_expenses_name = model.abev_gl_expenses_name;

                    expenses_vehicle.abev_material_code = model.abev_material_code;
                    expenses_vehicle.abev_material_name = model.abev_material_name;
                    expenses_vehicle.abev_model = model.abev_model;
                    expenses_vehicle.abev_registration_no = model.abev_registration_no;
                    expenses_vehicle.abev_note = model.abev_note;
                    
                    expenses_vehicle.abev_quantity = !string.IsNullOrEmpty(model.abev_quantity) && decimal.TryParse(model.abev_quantity.Replace(",", ""), out resultRate) ? decimal.Parse(model.abev_quantity.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_uom = model.abev_uom;
                    expenses_vehicle.abev_unit_price = !string.IsNullOrEmpty(model.abev_unit_price) && decimal.TryParse(model.abev_unit_price.Replace(",", ""), out resultRate) ? decimal.Parse(model.abev_unit_price.Replace(",", "")) : (decimal?)null;
                    //expenses_vehicle.abev_quantity_1 = model.abev_quantity_1;
                    //expenses_vehicle.abev_quantity_2 = model.abev_quantity_2;
                    //expenses_vehicle.abev_quantity_3 = model.abev_quantity_3;
                    //expenses_vehicle.abev_quantity_4 = model.abev_quantity_4;
                    //expenses_vehicle.abev_quantity_5 = model.abev_quantity_5;
                    //expenses_vehicle.abev_quantity_6 = model.abev_quantity_6;
                    //expenses_vehicle.abev_quantity_7 = model.abev_quantity_7;
                    //expenses_vehicle.abev_quantity_8 = model.abev_quantity_8;
                    //expenses_vehicle.abev_quantity_9 = model.abev_quantity_9;
                    //expenses_vehicle.abev_quantity_10 = model.abev_quantity_10;
                    //expenses_vehicle.abev_quantity_11 = model.abev_quantity_11;
                    //expenses_vehicle.abev_quantity_12 = model.abev_quantity_12;
                    expenses_vehicle.abev_quantity_1 = !string.IsNullOrEmpty(model.abev_quantity_1) && decimal.TryParse(model.abev_quantity_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_1.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_2 = !string.IsNullOrEmpty(model.abev_quantity_2) && decimal.TryParse(model.abev_quantity_2.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_2.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_3 = !string.IsNullOrEmpty(model.abev_quantity_3) && decimal.TryParse(model.abev_quantity_3.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_3.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_4 = !string.IsNullOrEmpty(model.abev_quantity_4) && decimal.TryParse(model.abev_quantity_4.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_4.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_5 = !string.IsNullOrEmpty(model.abev_quantity_5) && decimal.TryParse(model.abev_quantity_5.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_5.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_6 = !string.IsNullOrEmpty(model.abev_quantity_6) && decimal.TryParse(model.abev_quantity_6.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_6.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_7 = !string.IsNullOrEmpty(model.abev_quantity_7) && decimal.TryParse(model.abev_quantity_7.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_7.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_8 = !string.IsNullOrEmpty(model.abev_quantity_8) && decimal.TryParse(model.abev_quantity_8.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_8.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_9 = !string.IsNullOrEmpty(model.abev_quantity_9) && decimal.TryParse(model.abev_quantity_9.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_9.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_10 = !string.IsNullOrEmpty(model.abev_quantity_10) && decimal.TryParse(model.abev_quantity_10.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_10.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_11 = !string.IsNullOrEmpty(model.abev_quantity_11) && decimal.TryParse(model.abev_quantity_11.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_11.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_quantity_12 = !string.IsNullOrEmpty(model.abev_quantity_12) && decimal.TryParse(model.abev_quantity_12.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_quantity_12.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_1 = !string.IsNullOrEmpty(model.abev_amount_1) && decimal.TryParse(model.abev_amount_1.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_1.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_2 = !string.IsNullOrEmpty(model.abev_amount_2) && decimal.TryParse(model.abev_amount_2.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_2.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_3 = !string.IsNullOrEmpty(model.abev_amount_3) && decimal.TryParse(model.abev_amount_3.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_3.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_4 = !string.IsNullOrEmpty(model.abev_amount_4) && decimal.TryParse(model.abev_amount_4.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_4.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_5 = !string.IsNullOrEmpty(model.abev_amount_5) && decimal.TryParse(model.abev_amount_5.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_5.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_6 = !string.IsNullOrEmpty(model.abev_amount_6) && decimal.TryParse(model.abev_amount_6.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_6.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_7 = !string.IsNullOrEmpty(model.abev_amount_7) && decimal.TryParse(model.abev_amount_7.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_7.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_8 = !string.IsNullOrEmpty(model.abev_amount_8) && decimal.TryParse(model.abev_amount_8.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_8.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_9 = !string.IsNullOrEmpty(model.abev_amount_9) && decimal.TryParse(model.abev_amount_9.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_9.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_10 = !string.IsNullOrEmpty(model.abev_amount_10) && decimal.TryParse(model.abev_amount_10.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_10.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_11 = !string.IsNullOrEmpty(model.abev_amount_11) && decimal.TryParse(model.abev_amount_11.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_11.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_amount_12 = !string.IsNullOrEmpty(model.abev_amount_12) && decimal.TryParse(model.abev_amount_12.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_amount_12.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_total_quantity = !string.IsNullOrEmpty(model.abev_total_quantity) && decimal.TryParse(model.abev_total_quantity.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_total_quantity.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_total = !string.IsNullOrEmpty(model.abev_total) && decimal.TryParse(model.abev_total.Replace(",", ""), out resultAmount1) ? decimal.Parse(model.abev_total.Replace(",", "")) : (decimal?)null;
                    expenses_vehicle.abev_proration = model.abev_proration;
                    expenses_vehicle.last_modified = DateTime.Now;

                    db.Entry(expenses_vehicle).State = EntityState.Modified;
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
                        controller = "PerbelanjaanKenderaan",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abev_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abev_cost_center_code
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
            var expenses_vehicle = await db.bgt_expenses_vehicle.Where(p => p.abev_budgeting_year == year && p.abev_cost_center_code.Equals(CostCenter) && p.abev_deleted == false).FirstOrDefaultAsync();
            if (expenses_vehicle == null)
            {
                return HttpNotFound();
            }

            var total_amount = db.bgt_expenses_vehicle
                .Where(p => p.abev_budgeting_year == expenses_vehicle.abev_budgeting_year && p.abev_cost_center_code.Equals(expenses_vehicle.abev_cost_center_code) && p.abev_deleted == false)
                .Sum(p => p.abev_total);
            var model = new ExpensesVehicleDeleteViewModel
            {
                BudgetYear = expenses_vehicle.abev_budgeting_year,
                CostCenterCode = expenses_vehicle.abev_cost_center_code,
                CostCenterDesc = expenses_vehicle.abev_cost_center_desc,
                Total = string.Format("{0:#,##0.00}", total_amount)
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Delete", model);
        }

        // POST: PerbelanjaanKenderaan/Delete/5
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
                var expenses_vehicle = db.bgt_expenses_vehicle
                    .Where(p => p.abev_budgeting_year == year && p.abev_cost_center_code.Equals(CostCenter) && p.abev_deleted == false)
                    .ToList();

                if (expenses_vehicle.Count > 0)
                {
                    foreach (var item in expenses_vehicle)
                    {
                        //item.abev_deleted = true;
                        //item.last_modified = DateTime.Now;
                        //db.Entry(item).State = EntityState.Modified;
                        db.bgt_expenses_vehicle.Remove(item);
                    }
                    await db.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgDelete2,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "PerbelanjaanKenderaan",
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
                        controller = "PerbelanjaanKenderaan",
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
            var expenses_vehicle = await db.bgt_expenses_vehicle.FindAsync(id);
            if (expenses_vehicle == null)
            {
                return HttpNotFound();
            }

            var model = new ExpensesVehicleDeleteDetailViewModel
            {
                abev_id = id.Value,
                abev_budgeting_year = expenses_vehicle.abev_budgeting_year,
                abev_syarikat_id = expenses_vehicle.abev_syarikat_id,
                abev_syarikat_name = expenses_vehicle.abev_syarikat_name,
                abev_ladang_id = expenses_vehicle.abev_ladang_id,
                abev_ladang_code = expenses_vehicle.abev_ladang_code,
                abev_ladang_name = expenses_vehicle.abev_ladang_name,
                abev_wilayah_id = expenses_vehicle.abev_wilayah_id,
                abev_wilayah_name = expenses_vehicle.abev_wilayah_name,
                abev_cost_center_code = expenses_vehicle.abev_cost_center_code,
                abev_cost_center_desc = expenses_vehicle.abev_cost_center_desc,
                abev_gl_expenses_code = expenses_vehicle.abev_gl_expenses_code,
                abev_gl_expenses_name = expenses_vehicle.abev_gl_expenses_name,
                abev_quantity = expenses_vehicle.abev_quantity,
                abev_uom = expenses_vehicle.abev_uom,
                abev_unit_price = expenses_vehicle.abev_unit_price,
                abev_quantity_1 = expenses_vehicle.abev_quantity_1,
                abev_quantity_2 = expenses_vehicle.abev_quantity_2,
                abev_quantity_3 = expenses_vehicle.abev_quantity_3,
                abev_quantity_4 = expenses_vehicle.abev_quantity_4,
                abev_quantity_5 = expenses_vehicle.abev_quantity_5,
                abev_quantity_6 = expenses_vehicle.abev_quantity_6,
                abev_quantity_7 = expenses_vehicle.abev_quantity_7,
                abev_quantity_8 = expenses_vehicle.abev_quantity_8,
                abev_quantity_9 = expenses_vehicle.abev_quantity_9,
                abev_quantity_10 = expenses_vehicle.abev_quantity_10,
                abev_quantity_11 = expenses_vehicle.abev_quantity_11,
                abev_quantity_12 = expenses_vehicle.abev_quantity_12,
                abev_amount_1 = expenses_vehicle.abev_amount_1.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_1) : string.Empty,
                abev_amount_2 = expenses_vehicle.abev_amount_2.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_2) : string.Empty,
                abev_amount_3 = expenses_vehicle.abev_amount_3.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_3) : string.Empty,
                abev_amount_4 = expenses_vehicle.abev_amount_4.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_4) : string.Empty,
                abev_amount_5 = expenses_vehicle.abev_amount_5.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_5) : string.Empty,
                abev_amount_6 = expenses_vehicle.abev_amount_6.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_6) : string.Empty,
                abev_amount_7 = expenses_vehicle.abev_amount_7.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_7) : string.Empty,
                abev_amount_8 = expenses_vehicle.abev_amount_8.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_8) : string.Empty,
                abev_amount_9 = expenses_vehicle.abev_amount_9.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_9) : string.Empty,
                abev_amount_10 = expenses_vehicle.abev_amount_10.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_10) : string.Empty,
                abev_amount_11 = expenses_vehicle.abev_amount_11.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_11) : string.Empty,
                abev_amount_12 = expenses_vehicle.abev_amount_12.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_amount_12) : string.Empty,
                abev_total_quantity = expenses_vehicle.abev_total_quantity.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_total_quantity) : string.Empty,
                abev_total = expenses_vehicle.abev_total.HasValue ? string.Format("{0:#,##0.00}", expenses_vehicle.abev_total) : string.Empty
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
                var expenses_vehicle = db.bgt_expenses_vehicle.Find(id);
                //expenses_vehicle.abev_deleted = true;
                //expenses_vehicle.last_modified = DateTime.Now;
                //db.Entry(expenses_vehicle).State = EntityState.Modified;
                db.bgt_expenses_vehicle.Remove(expenses_vehicle);
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
                    controller = "PerbelanjaanKenderaan",
                    action = "RecordsDetail",
                    paramName1 = "BudgetYear",
                    paramValue1 = expenses_vehicle.abev_budgeting_year.ToString(),
                    paramName2 = "CostCenter",
                    paramValue2 = expenses_vehicle.abev_cost_center_code
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesVehicleList2.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesVehicleListViewModel2> data = db.Database.SqlQuery<ExpensesVehicleListViewModel2>("exec sp_BudgetExpensesVehicleList {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintView(string BudgetYear, string CostCenter, string Format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesVehicleView2.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            //if (Format.ToLower().Contains("excel"))
            //{
            //    path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesVehicleViewExcel2.rdlc");
            //}

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesVehicleListDetailViewModel2> data = db.Database.SqlQuery<ExpensesVehicleListDetailViewModel2>("exec sp_BudgetExpensesVehicleView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintViewExcel(string BudgetYear, string CostCenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesVehicleViewExcel2.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesVehicleListDetailViewModel2> data = db.Database.SqlQuery<ExpensesVehicleListDetailViewModel2>("exec sp_BudgetExpensesVehicleView {0}, {1}", BudgetYear, CostCenter).ToList();
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

        private List<bgt_vehicle_register> GetCostCenters(int budgetYear, bool exclude = true)
        {
            if (exclude)
            {
                var expenses_vehicles = GetRecords(budgetYear);
                var excludeCostCenters = new List<string>();

                for (int i = 0; i < expenses_vehicles.Count(); i++)
                {
                    excludeCostCenters.Add(expenses_vehicles[i].CostCenterCode);
                }

                return vehicle.GetCostCenters().Where(w => !excludeCostCenters.Contains(w.abvr_cost_center_code.TrimStart(new char[] { '0' })))
                    .OrderBy(w => w.abvr_cost_center_code)
                    .ToList();
            }
            else
            {
                return vehicle.GetCostCenters();
            }
        }

        private List<GLListViewModel> GetGLs(int? budgetYear = null, string costCenter = null, int? excludeRecordId = null)
        {
            var screen = scr.GetScreen(screenCode);
            var expenses_vehicles = GetRecordsDetail(budgetYear, costCenter, null, null, excludeRecordId);
            var excludeGLs = new List<string>();

            for (int i = 0; i < expenses_vehicles.Count(); i++)
            {
                excludeGLs.Add(expenses_vehicles[i].abev_gl_expenses_code);
            }

            return gl.GetGLs(screen.ScrID)
                .Where(g => !excludeGLs.Contains(g.fld_GLCode))
                .Select(g => new GLListViewModel { Id = g.fld_ID, Code = g.fld_GLCode, Description = g.fld_GLDesc })
                .ToList();
        }

        private List<bgt_vehicle_register> GetVehicle(int year, string costcenter, bool deleted = true)
        {
            return db.bgt_vehicle_register.Where(w => w.abvr_budgeting_year == year && w.abvr_cost_center_code.Contains(costcenter) && w.abvr_status == true && w.abvr_deleted == deleted).OrderBy(o => o.abvr_material_code).ToList();
        }

        private List<tbl_SAPGLPUP> GetGL(int syarikatid, bool deleted = true, int year = 0, string costcenter = null)
        {
            var ID = GetBudgetClass.GetScreenID("E14");
            var getAllID = dbc.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            List<Guid> GLID = new List<Guid>();
            GLID = getAllID.Select(s => s.fld_GLID).ToList();

            return dbm.tbl_SAPGLPUP.Where(w => GLID.Contains(w.fld_ID) && w.fld_SyarikatID == syarikatid && w.fld_Deleted == deleted)
                .OrderBy(o => o.fld_GLCode)
                .ToList();
        }
        private List<bgt_UOM> GetUOM()
        {
            return dbc.bgt_UOM.ToList();
        }
        public JsonResult GetMaterialName(string materialcode, string costcenter, string year)
        {
            var syktid = syarikat.GetSyarikatID();
            var wlyhid = wilayah.GetWilayahID();
            var ldgid = ladang.GetLadangID();

            var material = db.bgt_vehicle_register
                .Where(p => p.abvr_material_code.Equals(materialcode) && 
                p.abvr_cost_center_code.Contains(costcenter) && p.abvr_budgeting_year.ToString() == year &&
                p.abvr_deleted == false && 
                p.abvr_syarikat_id == syktid && p.abvr_wilayah_id == wlyhid && p.abvr_ladang_id == ldgid).FirstOrDefault();
            if (material != null)
            {
                return Json(new
                {
                    mdesc = material.abvr_material_name,
                    model = material.abvr_model,
                    regno  = material.abvr_register_no,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    mcode = new bgt_vehicle_register()
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetGLName(string glcode)
        {
            var syktid = syarikat.GetSyarikatID();
            var gl = dbm.tbl_SAPGLPUP
                .Where(p => p.fld_GLCode.EndsWith(glcode) && p.fld_Deleted == false && p.fld_SyarikatID == syktid) //new sept (EndsWith)
                .FirstOrDefault();
            if (gl != null)
            {
                return Json(new
                {
                    gl = gl.fld_GLDesc
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    gl = new tbl_SAPGLPUP()
                }, JsonRequestBehavior.AllowGet);
            }
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

        private List<ExpensesVehicleListViewModel2> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syktid = syarikat.GetSyarikatID();
            var wlyhid = wilayah.GetWilayahID();
            var ldgid = ladang.GetLadangID();

            if (BudgetYear >= 2024)
            {
                var query = from p in db.bgt_expenses_vehicle
                            where p.abev_deleted == false && p.abev_syarikat_id == syktid && p.abev_wilayah_id == wlyhid && p.abev_ladang_id == ldgid
                            group p by new { p.abev_budgeting_year, p.abev_cost_center_code, p.abev_cost_center_desc } into g
                            select new
                            {
                                BudgetYear = g.Key.abev_budgeting_year,
                                CostCenterCode = g.Key.abev_cost_center_code,
                                CostCenterDesc = g.Key.abev_cost_center_desc,
                                Total = g.Sum(p => p.abev_total)
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
                              select new ExpensesVehicleListViewModel2
                              {
                                  BudgetYear = q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  Total = q.Total
                              }).Distinct();

                return query3.ToList();
            }
            else
            {
                var query = from p in db.bgt_expenses_vehicle
                            group p by new { p.abev_budgeting_year, p.abev_cost_center_code, p.abev_cost_center_desc } into g
                            select new
                            {
                                BudgetYear = g.Key.abev_budgeting_year,
                                CostCenterCode = g.Key.abev_cost_center_code,
                                CostCenterDesc = g.Key.abev_cost_center_desc,
                                Total = g.Sum(p => p.abev_total)
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
                              select new ExpensesVehicleListViewModel2
                              {
                                  BudgetYear = q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  Total = q.Total
                              }).Distinct();

                return query3.ToList();
            }
        }

        private List<ExpensesVehicleListDetailViewModel2> GetRecordsDetail(int? budgeting_year = null, string cost_center = null, string gl_expenses = null, /*string gl_income = null,*/ int? excludeId = null, int? page = null, int? pageSize = null)
        {
            var syktid = syarikat.GetSyarikatID();
            var wlyhid = wilayah.GetWilayahID();
            var ldgid = ladang.GetLadangID();

            if (budgeting_year >= 2024)
            {
                var query = from p in db.bgt_expenses_vehicle where p.abev_deleted == false select p;

                if (budgeting_year.HasValue)
                    query = query.Where(q => q.abev_budgeting_year == budgeting_year);
                if (!string.IsNullOrEmpty(cost_center))
                    query = query.Where(q => q.abev_cost_center_code.Contains(cost_center) || q.abev_cost_center_code.Contains(cost_center));
                if (!string.IsNullOrEmpty(gl_expenses))
                    query = query.Where(q => q.abev_gl_expenses_code.Contains(gl_expenses) || q.abev_gl_expenses_name.Contains(gl_expenses));
                if (excludeId.HasValue)
                    query = query.Where(q => q.abev_id != excludeId);

                var query2 = query.AsQueryable();

                if (page.HasValue && pageSize.HasValue)
                {
                    query2 = query2
                        .OrderByDescending(q => q.abev_budgeting_year)
                        .ThenBy(q => q.abev_cost_center_code)
                        .ThenBy(q => q.abev_material_code)
                        .Skip((page.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);
                }
                else
                {
                    query2 = query2
                        .OrderByDescending(q => q.abev_budgeting_year)
                        .ThenBy(q => q.abev_cost_center_code)
                        .ThenBy(q => q.abev_material_code);
                }

                var query3 = (from q in query2.AsEnumerable()
                              select new ExpensesVehicleListDetailViewModel2
                              {
                                  abev_id = q.abev_id,
                                  abev_revision = q.abev_revision,
                                  abev_budgeting_year = q.abev_budgeting_year,
                                  abev_syarikat_id = q.abev_syarikat_id,
                                  abev_syarikat_name = q.abev_syarikat_name,
                                  abev_ladang_id = q.abev_ladang_id,
                                  abev_ladang_code = q.abev_ladang_code,
                                  abev_ladang_name = q.abev_ladang_name,
                                  abev_wilayah_id = q.abev_wilayah_id,
                                  abev_wilayah_name = q.abev_wilayah_name,
                                  abev_cost_center_code = q.abev_cost_center_code,
                                  abev_cost_center_desc = q.abev_cost_center_desc,
                                  abev_gl_expenses_code = q.abev_gl_expenses_code,
                                  abev_gl_expenses_name = q.abev_gl_expenses_name,
                                  abev_material_code = q.abev_material_code,
                                  abev_material_name = q.abev_material_name,
                                  abev_model = q.abev_model,
                                  abev_registration_no = q.abev_registration_no,
                                  abev_note = q.abev_note,
                                  abev_quantity = q.abev_quantity,
                                  abev_uom = q.abev_uom,
                                  abev_unit_price = q.abev_unit_price,
                                  abev_quantity_1 = q.abev_quantity_1,
                                  abev_quantity_2 = q.abev_quantity_2,
                                  abev_quantity_3 = q.abev_quantity_3,
                                  abev_quantity_4 = q.abev_quantity_4,
                                  abev_quantity_5 = q.abev_quantity_5,
                                  abev_quantity_6 = q.abev_quantity_6,
                                  abev_quantity_7 = q.abev_quantity_7,
                                  abev_quantity_8 = q.abev_quantity_8,
                                  abev_quantity_9 = q.abev_quantity_9,
                                  abev_quantity_10 = q.abev_quantity_10,
                                  abev_quantity_11 = q.abev_quantity_11,
                                  abev_quantity_12 = q.abev_quantity_12,
                                  abev_amount_1 = q.abev_amount_1,
                                  abev_amount_2 = q.abev_amount_2,
                                  abev_amount_3 = q.abev_amount_3,
                                  abev_amount_4 = q.abev_amount_4,
                                  abev_amount_5 = q.abev_amount_5,
                                  abev_amount_6 = q.abev_amount_6,
                                  abev_amount_7 = q.abev_amount_7,
                                  abev_amount_8 = q.abev_amount_8,
                                  abev_amount_9 = q.abev_amount_9,
                                  abev_amount_10 = q.abev_amount_10,
                                  abev_amount_11 = q.abev_amount_11,
                                  abev_amount_12 = q.abev_amount_12,
                                  abev_total_quantity = q.abev_total_quantity,
                                  abev_total = q.abev_total,
                                  abev_proration = q.abev_proration,
                                  abev_history = q.abev_history,
                                  abev_status = q.abev_status,
                                  abev_created_by = q.abev_created_by
                              }).Distinct();

                return query3.ToList();
            }
            else
            {
                var query = from p in db.bgt_expenses_vehicle select p;

                if (budgeting_year.HasValue)
                    query = query.Where(q => q.abev_budgeting_year == budgeting_year);
                if (!string.IsNullOrEmpty(cost_center))
                    query = query.Where(q => q.abev_cost_center_code.Contains(cost_center) || q.abev_cost_center_code.Contains(cost_center));
                if (!string.IsNullOrEmpty(gl_expenses))
                    query = query.Where(q => q.abev_gl_expenses_code.Contains(gl_expenses) || q.abev_gl_expenses_name.Contains(gl_expenses));
                if (excludeId.HasValue)
                    query = query.Where(q => q.abev_id != excludeId);

                var query2 = query.AsQueryable();

                if (page.HasValue && pageSize.HasValue)
                {
                    query2 = query2
                        .OrderByDescending(q => q.abev_budgeting_year)
                        .ThenBy(q => q.abev_cost_center_code)
                        .ThenBy(q => q.abev_material_code)
                        .Skip((page.Value - 1) * pageSize.Value)
                        .Take(pageSize.Value);
                }
                else
                {
                    query2 = query2
                        .OrderByDescending(q => q.abev_budgeting_year)
                        .ThenBy(q => q.abev_cost_center_code)
                        .ThenBy(q => q.abev_material_code);
                }

                var query3 = (from q in query2.AsEnumerable()
                              select new ExpensesVehicleListDetailViewModel2
                              {
                                  abev_id = q.abev_id,
                                  abev_revision = q.abev_revision,
                                  abev_budgeting_year = q.abev_budgeting_year,
                                  abev_syarikat_id = q.abev_syarikat_id,
                                  abev_syarikat_name = q.abev_syarikat_name,
                                  abev_ladang_id = q.abev_ladang_id,
                                  abev_ladang_code = q.abev_ladang_code,
                                  abev_ladang_name = q.abev_ladang_name,
                                  abev_wilayah_id = q.abev_wilayah_id,
                                  abev_wilayah_name = q.abev_wilayah_name,
                                  abev_cost_center_code = q.abev_cost_center_code,
                                  abev_cost_center_desc = q.abev_cost_center_desc,
                                  abev_gl_expenses_code = q.abev_gl_expenses_code,
                                  abev_gl_expenses_name = q.abev_gl_expenses_name,
                                  abev_material_code = q.abev_material_code,
                                  abev_material_name = q.abev_material_name,
                                  abev_model = q.abev_model,
                                  abev_registration_no = q.abev_registration_no,
                                  abev_note = q.abev_note,
                                  abev_quantity = q.abev_quantity,
                                  abev_uom = q.abev_uom,
                                  abev_unit_price = q.abev_unit_price,
                                  abev_quantity_1 = q.abev_quantity_1,
                                  abev_quantity_2 = q.abev_quantity_2,
                                  abev_quantity_3 = q.abev_quantity_3,
                                  abev_quantity_4 = q.abev_quantity_4,
                                  abev_quantity_5 = q.abev_quantity_5,
                                  abev_quantity_6 = q.abev_quantity_6,
                                  abev_quantity_7 = q.abev_quantity_7,
                                  abev_quantity_8 = q.abev_quantity_8,
                                  abev_quantity_9 = q.abev_quantity_9,
                                  abev_quantity_10 = q.abev_quantity_10,
                                  abev_quantity_11 = q.abev_quantity_11,
                                  abev_quantity_12 = q.abev_quantity_12,
                                  abev_amount_1 = q.abev_amount_1,
                                  abev_amount_2 = q.abev_amount_2,
                                  abev_amount_3 = q.abev_amount_3,
                                  abev_amount_4 = q.abev_amount_4,
                                  abev_amount_5 = q.abev_amount_5,
                                  abev_amount_6 = q.abev_amount_6,
                                  abev_amount_7 = q.abev_amount_7,
                                  abev_amount_8 = q.abev_amount_8,
                                  abev_amount_9 = q.abev_amount_9,
                                  abev_amount_10 = q.abev_amount_10,
                                  abev_amount_11 = q.abev_amount_11,
                                  abev_amount_12 = q.abev_amount_12,
                                  abev_total_quantity = q.abev_total_quantity,
                                  abev_total = q.abev_total,
                                  abev_proration = q.abev_proration,
                                  abev_history = q.abev_history,
                                  abev_status = q.abev_status,
                                  abev_created_by = q.abev_created_by
                              }).Distinct();

                return query3.ToList();
            }
        }
        #endregion
    }
}