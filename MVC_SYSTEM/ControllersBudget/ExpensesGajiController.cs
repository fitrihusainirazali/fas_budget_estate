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
    public class ExpensesGajiController : Controller
    {
        
        private MVC_SYSTEM_ModelsBudgetEst dbEst = new ConnectionBudget().GetConnection();
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
        private readonly string screenCode = "E12";

        // GET: ExpensesGaji
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
            if (BudgetYear == "" || BudgetYear == null)
            {
                int pageSize = int.Parse(config.GetData("paging"));
                var selectedYear = GetYears().FirstOrDefault().ToString();
                int? budgetYear = !string.IsNullOrEmpty(selectedYear) ? int.Parse(selectedYear) : (int?)null;

                var records = new ViewingModels.PagedList<ExpensesGajiListViewModel>
                {
                    Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                    TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                    CurrentPage = page,
                    PageSize = pageSize
                };
                return PartialView("_Records", records);
            }
            else
            {
                int pageSize = int.Parse(config.GetData("paging"));
                int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

                var records = new ViewingModels.PagedList<ExpensesGajiListViewModel>
                {
                    Content = GetRecords(budgetYear, CostCenter, page, pageSize),
                    TotalRecords = GetRecords(budgetYear, CostCenter).Count(),
                    CurrentPage = page,
                    PageSize = pageSize
                };
                return PartialView("_Records", records);
            }
             

            
        }

        public ActionResult RecordsDetail(string BudgetYear, string CostCenter, int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<ExpensesGajiListDetailViewModel>
            {
                Content = GetRecordsDetail(budgetYear, CostCenter, null, null, page, pageSize),
                TotalRecords = GetRecordsDetail(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_RecordsDetail", records);
        }

        // GET: ExpensesGaji/Details/5
        public ActionResult Details(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_gajis = GetRecordsDetail(year, CostCenter);

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;

            return PartialView("_Details", expenses_gajis);
        }

        // GET: ExpensesGaji/Create
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
                Value = s.fld_CostCenter,
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();



            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = selectedYear;
            ViewBag.Years = years;
            ViewBag.CostCenters = costCenters;

            return View();
        }

        // POST: ExpensesGaji/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string BudgetYear, string CostCenter, string Hectare)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Edit", new
            {
                BudgetYear = BudgetYear,
                CostCenter = CostCenter.TrimStart(new char[] { '0' }),
                COstCenterDesc = cc.GetCostCenterDesc(CostCenter),
                Hectare = Hectare

                
        });
        }

        public ActionResult CreateDetail(string BudgetYear, string CostCenter, string Hectare)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
                        
            var gls = new SelectList(gl.GetGLs(scr.GetScreen(screenCode).ScrID).Select(s => new SelectListItem
            {
                Value = s.fld_GLCode,
                Text = s.fld_GLCode.TrimStart(new char[] { '0' }) + " - " + s.fld_GLDesc
            }), "Value", "Text").ToList();

            List<SelectListItem> Aktvtlist = new List<SelectListItem>();
            List<sp_AktivitiGL> AktivitiGL = dbEst.Database.SqlQuery<sp_AktivitiGL>("exec sp_AktivitiGL").ToList();
            Aktvtlist = new SelectList(AktivitiGL.Select(s => new SelectListItem { Value = s.fldKodAktvt, Text = s.fldKodAktvt + " - " + s.fldAktvtDesc }).OrderBy(o => o.Text), "Value", "Text").Distinct().ToList();
           
            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Year = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
            ViewBag.GLs = gls;
            ViewBag.Hectare = Hectare;
            ViewBag.Aktvtlist = Aktvtlist;

            return PartialView("_CreateDetail");
        }

        public JsonResult GetAktvtList(String GLList)
        {
            List<SelectListItem> Aktvtlist = new List<SelectListItem>();
            List<sp_AktivitiGL> AktivitiGL = dbEst.Database.SqlQuery<sp_AktivitiGL>("exec sp_AktivitiGL").ToList();

            Aktvtlist = new SelectList(AktivitiGL.Where(x => x.fldGL == GLList.TrimStart(new char[] { '0' })).Select(s => new SelectListItem { Value = s.fldKodAktvt, Text = s.fldKodAktvt + " - " + s.fldAktvtDesc }).OrderBy(o => o.Text), "Value", "Text").ToList();

            return Json(Aktvtlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDetail(ExpensesGajiCreateDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var akt_name = dbc.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == model.abeg_akt_code).Select(o => o.fld_Desc).FirstOrDefault();

                    GetIdentity getIdentity = new GetIdentity();


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

                    var expenses_gaji = new bgt_expenses_gaji
                    {
                        abeg_budgeting_year = model.abeg_budgeting_year,
                        //abeg_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null,
                        //abeg_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                        //abeg_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null,
                        //abeg_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                        //abeg_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                        //abeg_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null,
                        //abeg_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                        abeg_syarikat_id = syarikatObj.fld_SyarikatID,
                        abeg_syarikat_name = syarikatObj.fld_NamaSyarikat,
                        abeg_ladang_id = ladangObj.fld_ID,
                        abeg_ladang_code = ladangObj.fld_LdgCode,
                        abeg_ladang_name = ladangObj.fld_LdgName,
                        abeg_wilayah_id = wilayahObj.fld_ID,
                        abeg_wilayah_name = wilayahObj.fld_WlyhName,
                        abeg_cost_center_code = model.abeg_cost_center_code.TrimStart(new char[] { '0' }),
                        abeg_cost_center_desc = model.abeg_cost_center_desc,
                        abeg_gl_expenses_code = model.abeg_gl_expenses_code.TrimStart(new char[] { '0' }),
                        abeg_gl_expenses_name = model.abeg_gl_expenses_name,
                        abeg_akt_code = model.abeg_akt_code,
                        abeg_akt_name = akt_name,
                        abeg_note = model.abeg_note,
                        abeg_hectare = model.abeg_hectare,
                        abeg_total = !string.IsNullOrEmpty(model.abeg_total) && decimal.TryParse(model.abeg_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abeg_total.Replace(",", "")) : (decimal?)null,
                        abeg_month_1 = !string.IsNullOrEmpty(model.abeg_month_1) && decimal.TryParse(model.abeg_month_1.Replace(",", ""), out resultMonth1) ? decimal.Parse(model.abeg_month_1.Replace(",", "")) : (decimal?)null,
                        abeg_month_2 = !string.IsNullOrEmpty(model.abeg_month_2) && decimal.TryParse(model.abeg_month_2.Replace(",", ""), out resultMonth2) ? decimal.Parse(model.abeg_month_2.Replace(",", "")) : (decimal?)null,
                        abeg_month_3 = !string.IsNullOrEmpty(model.abeg_month_3) && decimal.TryParse(model.abeg_month_3.Replace(",", ""), out resultMonth3) ? decimal.Parse(model.abeg_month_3.Replace(",", "")) : (decimal?)null,
                        abeg_month_4 = !string.IsNullOrEmpty(model.abeg_month_4) && decimal.TryParse(model.abeg_month_4.Replace(",", ""), out resultMonth4) ? decimal.Parse(model.abeg_month_4.Replace(",", "")) : (decimal?)null,
                        abeg_month_5 = !string.IsNullOrEmpty(model.abeg_month_5) && decimal.TryParse(model.abeg_month_5.Replace(",", ""), out resultMonth5) ? decimal.Parse(model.abeg_month_5.Replace(",", "")) : (decimal?)null,
                        abeg_month_6 = !string.IsNullOrEmpty(model.abeg_month_6) && decimal.TryParse(model.abeg_month_6.Replace(",", ""), out resultMonth6) ? decimal.Parse(model.abeg_month_6.Replace(",", "")) : (decimal?)null,
                        abeg_month_7 = !string.IsNullOrEmpty(model.abeg_month_7) && decimal.TryParse(model.abeg_month_7.Replace(",", ""), out resultMonth7) ? decimal.Parse(model.abeg_month_7.Replace(",", "")) : (decimal?)null,
                        abeg_month_8 = !string.IsNullOrEmpty(model.abeg_month_8) && decimal.TryParse(model.abeg_month_8.Replace(",", ""), out resultMonth8) ? decimal.Parse(model.abeg_month_8.Replace(",", "")) : (decimal?)null,
                        abeg_month_9 = !string.IsNullOrEmpty(model.abeg_month_9) && decimal.TryParse(model.abeg_month_9.Replace(",", ""), out resultMonth9) ? decimal.Parse(model.abeg_month_9.Replace(",", "")) : (decimal?)null,
                        abeg_month_10 = !string.IsNullOrEmpty(model.abeg_month_10) && decimal.TryParse(model.abeg_month_10.Replace(",", ""), out resultMonth10) ? decimal.Parse(model.abeg_month_10.Replace(",", "")) : (decimal?)null,
                        abeg_month_11 = !string.IsNullOrEmpty(model.abeg_month_11) && decimal.TryParse(model.abeg_month_11.Replace(",", ""), out resultMonth11) ? decimal.Parse(model.abeg_month_11.Replace(",", "")) : (decimal?)null,
                        abeg_month_12 = !string.IsNullOrEmpty(model.abeg_month_12) && decimal.TryParse(model.abeg_month_12.Replace(",", ""), out resultMonth12) ? decimal.Parse(model.abeg_month_12.Replace(",", "")) : (decimal?)null,
                        abeg_proration = model.abeg_proration,
                        abeg_revision = 0,
                        abeg_Deleted = false,
                        abeg_status = "DRAFT",
                        abeg_created_by = getIdentity.ID(User.Identity.Name),
                        last_modified = DateTime.Now
                    };
                    dbEst.bgt_expenses_gajis.Add(expenses_gaji);
                    await dbEst.SaveChangesAsync();

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
                        controller = "ExpensesGaji",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abeg_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abeg_cost_center_code,
                        paramName3 = "Hectare",
                        paramValue3 = model.abeg_hectare
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



        // GET: ExpensesGaji/Edit/5
        public ActionResult Edit(string BudgetYear, string CostCenter, string Hectare)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return RedirectToAction("Create");
            }

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter.TrimStart(new char[] { '0' });
            ViewBag.COstCenterDesc = cc.GetCostCenterDesc(CostCenter);
            ViewBag.Hectare = Hectare;
            return View();
        }

       
        public async Task<ActionResult> EditDetail(int? id)
        
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var expenses_gaji = await dbEst.bgt_expenses_gajis.FindAsync(id);
            if (expenses_gaji == null)
            {
                return HttpNotFound();
            }

            
            var gls = new SelectList(gl.GetGLs(scr.GetScreen(screenCode).ScrID).Select(s => new SelectListItem
            {
                Value = s.fld_GLCode.TrimStart(new char[] { '0' }),
                Text = s.fld_GLCode.TrimStart(new char[] { '0' }) + " - " + s.fld_GLDesc
            }), "Value", "Text").ToList();

            List<SelectListItem> Aktvtlist = new List<SelectListItem>();
            List<sp_AktivitiGL> AktivitiGL = dbEst.Database.SqlQuery<sp_AktivitiGL>("exec sp_AktivitiGL").ToList();
            Aktvtlist = new SelectList(AktivitiGL.Where(w => w.fldGL == expenses_gaji.abeg_gl_expenses_code).Select(s => new SelectListItem { Value = s.fldKodAktvt, Text = s.fldKodAktvt + " - " + s.fldAktvtDesc }).OrderBy(o => o.Text), "Value", "Text").Distinct().ToList();
      
            var model = new ExpensesGajiEditDetailViewModel
            {
                abeg_id = id.Value,
                abeg_budgeting_year = expenses_gaji.abeg_budgeting_year,
                abeg_syarikat_id = expenses_gaji.abeg_syarikat_id,
                abeg_syarikat_name = expenses_gaji.abeg_syarikat_name,
                abeg_ladang_id = expenses_gaji.abeg_ladang_id,
                abeg_ladang_code = expenses_gaji.abeg_ladang_code,
                abeg_ladang_name = expenses_gaji.abeg_ladang_name,
                abeg_wilayah_id = expenses_gaji.abeg_wilayah_id,
                abeg_wilayah_name = expenses_gaji.abeg_wilayah_name,
                abeg_cost_center_code = expenses_gaji.abeg_cost_center_code,
                abeg_cost_center_desc = expenses_gaji.abeg_cost_center_desc,
                abeg_gl_expenses_code = expenses_gaji.abeg_gl_expenses_code,
                abeg_gl_expenses_name = expenses_gaji.abeg_gl_expenses_name,
                abeg_akt_code = expenses_gaji.abeg_akt_code,
                abeg_akt_name = expenses_gaji.abeg_akt_name,
                abeg_note = expenses_gaji.abeg_note,
                abeg_hectare = expenses_gaji.abeg_hectare,
                abeg_total = expenses_gaji.abeg_total.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_total) : string.Empty,
                abeg_month_1 = expenses_gaji.abeg_month_1.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_1) : string.Empty,
                abeg_month_2 = expenses_gaji.abeg_month_2.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_2) : string.Empty,
                abeg_month_3 = expenses_gaji.abeg_month_3.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_3) : string.Empty,
                abeg_month_4 = expenses_gaji.abeg_month_4.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_4) : string.Empty,
                abeg_month_5 = expenses_gaji.abeg_month_5.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_5) : string.Empty,
                abeg_month_6 = expenses_gaji.abeg_month_6.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_6) : string.Empty,
                abeg_month_7 = expenses_gaji.abeg_month_7.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_7) : string.Empty,
                abeg_month_8 = expenses_gaji.abeg_month_8.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_8) : string.Empty,
                abeg_month_9 = expenses_gaji.abeg_month_9.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_9) : string.Empty,
                abeg_month_10 = expenses_gaji.abeg_month_10.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_10) : string.Empty,
                abeg_month_11 = expenses_gaji.abeg_month_11.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_11) : string.Empty,
                abeg_month_12 = expenses_gaji.abeg_month_12.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_month_12) : string.Empty,
                abeg_revision = expenses_gaji.abeg_revision,
                abeg_status = expenses_gaji.abeg_status,
                abeg_proration = expenses_gaji.abeg_proration,
                abeg_created_by = expenses_gaji.abeg_created_by
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.GLs = gls;
            ViewBag.Aktvtlist = Aktvtlist;

            return PartialView("_EditDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDetail(int? id, ExpensesGajiEditDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var akt_name = dbc.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == model.abeg_akt_code).Select(o => o.fld_Desc).FirstOrDefault();

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

                    var expenses_gaji = dbEst.bgt_expenses_gajis.Find(id);
                    expenses_gaji.abeg_budgeting_year = model.abeg_budgeting_year;
                    expenses_gaji.abeg_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null;
                    expenses_gaji.abeg_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                    expenses_gaji.abeg_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null;
                    expenses_gaji.abeg_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                    expenses_gaji.abeg_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                    expenses_gaji.abeg_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null;
                    expenses_gaji.abeg_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null;
                    expenses_gaji.abeg_cost_center_code = model.abeg_cost_center_code.TrimStart(new char[] { '0' });
                    expenses_gaji.abeg_cost_center_desc = model.abeg_cost_center_desc;
                    expenses_gaji.abeg_gl_expenses_code = model.abeg_gl_expenses_code.TrimStart(new char[] { '0' });
                    expenses_gaji.abeg_gl_expenses_name = model.abeg_gl_expenses_name;
                    expenses_gaji.abeg_akt_code = model.abeg_akt_code;
                    expenses_gaji.abeg_akt_name = akt_name;
                    expenses_gaji.abeg_note = model.abeg_note;
                    expenses_gaji.abeg_hectare = model.abeg_hectare;
                    expenses_gaji.abeg_total = !string.IsNullOrEmpty(model.abeg_total) && decimal.TryParse(model.abeg_total.Replace(",", ""), out resultTotal) ? decimal.Parse(model.abeg_total.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_1 = !string.IsNullOrEmpty(model.abeg_month_1) && decimal.TryParse(model.abeg_month_1.Replace(",", ""), out resultMonth1) ? decimal.Parse(model.abeg_month_1.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_2 = !string.IsNullOrEmpty(model.abeg_month_2) && decimal.TryParse(model.abeg_month_2.Replace(",", ""), out resultMonth2) ? decimal.Parse(model.abeg_month_2.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_3 = !string.IsNullOrEmpty(model.abeg_month_3) && decimal.TryParse(model.abeg_month_3.Replace(",", ""), out resultMonth3) ? decimal.Parse(model.abeg_month_3.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_4 = !string.IsNullOrEmpty(model.abeg_month_4) && decimal.TryParse(model.abeg_month_4.Replace(",", ""), out resultMonth4) ? decimal.Parse(model.abeg_month_4.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_5 = !string.IsNullOrEmpty(model.abeg_month_5) && decimal.TryParse(model.abeg_month_5.Replace(",", ""), out resultMonth5) ? decimal.Parse(model.abeg_month_5.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_6 = !string.IsNullOrEmpty(model.abeg_month_6) && decimal.TryParse(model.abeg_month_6.Replace(",", ""), out resultMonth6) ? decimal.Parse(model.abeg_month_6.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_7 = !string.IsNullOrEmpty(model.abeg_month_7) && decimal.TryParse(model.abeg_month_7.Replace(",", ""), out resultMonth7) ? decimal.Parse(model.abeg_month_7.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_8 = !string.IsNullOrEmpty(model.abeg_month_8) && decimal.TryParse(model.abeg_month_8.Replace(",", ""), out resultMonth8) ? decimal.Parse(model.abeg_month_8.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_9 = !string.IsNullOrEmpty(model.abeg_month_9) && decimal.TryParse(model.abeg_month_9.Replace(",", ""), out resultMonth9) ? decimal.Parse(model.abeg_month_9.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_10 = !string.IsNullOrEmpty(model.abeg_month_10) && decimal.TryParse(model.abeg_month_10.Replace(",", ""), out resultMonth10) ? decimal.Parse(model.abeg_month_10.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_11 = !string.IsNullOrEmpty(model.abeg_month_11) && decimal.TryParse(model.abeg_month_11.Replace(",", ""), out resultMonth11) ? decimal.Parse(model.abeg_month_11.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_month_12 = !string.IsNullOrEmpty(model.abeg_month_12) && decimal.TryParse(model.abeg_month_12.Replace(",", ""), out resultMonth12) ? decimal.Parse(model.abeg_month_12.Replace(",", "")) : (decimal?)null;
                    expenses_gaji.abeg_proration = model.abeg_proration;
                    expenses_gaji.last_modified = DateTime.Now;

                    dbEst.Entry(expenses_gaji).State = EntityState.Modified;
                    await dbEst.SaveChangesAsync();

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
                        controller = "ExpensesGaji",
                        action = "RecordsDetail",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abeg_budgeting_year.ToString(),
                        paramName2 = "CostCenter",
                        paramValue2 = model.abeg_cost_center_code,
                        paramName3 = "Hectare",
                        paramValue3 = model.abeg_hectare
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

        // GET: ExpensesGaji/Delete/5
        public async Task<ActionResult> Delete(string BudgetYear, string CostCenter, string Hectare)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var year = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            var expenses_gaji = await dbEst.bgt_expenses_gajis.Where(o => o.abeg_budgeting_year == year && o.abeg_cost_center_code.Equals(CostCenter) && !o.abeg_Deleted).FirstOrDefaultAsync();
            if (expenses_gaji == null)
            {
                return HttpNotFound();
            }
            var total_amount = dbEst.bgt_expenses_gajis
                .Where(o => o.abeg_budgeting_year == expenses_gaji.abeg_budgeting_year && o.abeg_cost_center_code.Equals(expenses_gaji.abeg_cost_center_code) && !o.abeg_Deleted)
                .Sum(o => o.abeg_total);
            var model = new ExpensesGajiDeleteViewModel
            {
                BudgetYear = expenses_gaji.abeg_budgeting_year,
                //CostCenterCode = expenses_gaji.abeg_cost_center_code.TrimStart(new char[] { '0' }),
                CostCenterCode = expenses_gaji.abeg_cost_center_code,
                CostCenterDesc = expenses_gaji.abeg_cost_center_desc,
                Total = string.Format("{0:#,##0.00}", total_amount)
            };

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.BudgetYear = BudgetYear;
            //ViewBag.CostCenter = CostCenter.TrimStart(new char[] { '0' });
            ViewBag.CostCenter = CostCenter;
            ViewBag.Hectare = Hectare;

            return PartialView("_Delete", model);
        }

        // POST: ExpensesGaji/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string BudgetYear, string CostCenter, string Hectare)
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
                var selectedYear = GetYears().FirstOrDefault().ToString();
                var expenses_gajis = dbEst.bgt_expenses_gajis
                    .Where(o => o.abeg_budgeting_year == year && o.abeg_cost_center_code.Equals(CostCenter) && !o.abeg_Deleted)
                    .ToList();

                if (expenses_gajis.Count() > 0)
                {
                    foreach (var item in expenses_gajis)
                    {
                        //item.abeg_Deleted = true;
                        //item.last_modified = DateTime.Now;
                        //dbEst.Entry(item).State = EntityState.Modified;
                        dbEst.bgt_expenses_gajis.Remove(item);
                    }
                    await dbEst.SaveChangesAsync();

                    

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgDelete2,
                        status = "success",
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesGaji",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = year,
                        //paramName2 = "CostCenter",
                        //paramValue2 = CostCenter,
                        //paramName3 = "Hectare",
                        //paramValue3 = Hectare
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        div = "RecordsPartial",
                        rooturl = domain,
                        controller = "ExpensesGaji",
                        action = "Records",
                        paramName1 = "BudgetYear",
                        paramValue1 = year,
                        //paramName2 = "CostCenter",
                        //paramValue2 = CostCenter,
                        //paramName3 = "Hectare",
                        //paramValue3 = Hectare
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
            var expenses_gaji = await dbEst.bgt_expenses_gajis.FindAsync(id);
            if (expenses_gaji == null)
            {
                return HttpNotFound();
            }

            var model = new ExpensesGajiDeleteDetailViewModel
            {
                abeg_id = expenses_gaji.abeg_id,
                abeg_budgeting_year = expenses_gaji.abeg_budgeting_year,
                abeg_syarikat_id = expenses_gaji.abeg_syarikat_id,
                abeg_syarikat_name = expenses_gaji.abeg_syarikat_name,
                abeg_ladang_id = expenses_gaji.abeg_ladang_id,
                abeg_ladang_code = expenses_gaji.abeg_ladang_code,
                abeg_ladang_name = expenses_gaji.abeg_ladang_name,
                abeg_wilayah_id = expenses_gaji.abeg_wilayah_id,
                abeg_wilayah_name = expenses_gaji.abeg_wilayah_name,
                abeg_cost_center_code = expenses_gaji.abeg_cost_center_code,
                abeg_cost_center_desc = expenses_gaji.abeg_cost_center_desc,
                abeg_gl_expenses_code = expenses_gaji.abeg_gl_expenses_code,
                abeg_gl_expenses_name = expenses_gaji.abeg_gl_expenses_name,
                abeg_note = expenses_gaji.abeg_note,
                abeg_total = expenses_gaji.abeg_total.HasValue ? string.Format("{0:#,##0.00}", expenses_gaji.abeg_total) : string.Empty,
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
                var expenses_gaji = dbEst.bgt_expenses_gajis.Find(id);

                if (expenses_gaji == null)
                {
                    return Json(new { success = true, msg = GlobalResEstate.msgNoRecord, status = "alert", checkingdata = "0", method = "2", btn = "" });
                }
                else
                {
                   dbEst.bgt_expenses_gajis.Remove(expenses_gaji);
                }
                dbEst.SaveChanges();

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/") { domain = domain + appname; }

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    div = "RecordsPartial",
                    rooturl = domain,
                    controller = "ExpensesGaji",
                    action = "RecordsDetail",
                    paramName1 = "BudgetYear",
                    paramValue1 = expenses_gaji.abeg_budgeting_year.ToString(),
                    paramName2 = "CostCenter",
                    paramValue2 = expenses_gaji.abeg_cost_center_code,
                    paramName3 = "Hectare",
                    paramValue3 = expenses_gaji.abeg_hectare
                });

                //var expenses_gaji = dbEst.bgt_expenses_gajis.Find(id);
                //expenses_gaji.abeg_Deleted = true;
                //expenses_gaji.last_modified = DateTime.Now;
                //dbEst.Entry(expenses_gaji).State = EntityState.Modified;
                //await dbEst.SaveChangesAsync();

                //string appname = Request.ApplicationPath;
                //string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                //var lang = Request.RequestContext.RouteData.Values["lang"];

                //if (appname != "/")
                //{
                //    domain = domain + appname;
                //}

                //return Json(new
                //{
                //success = true,
                //msg = GlobalResEstate.msgDelete2,
                //status = "success",
                //div = "RecordsPartial",
                //rooturl = domain,
                //controller = "ExpensesGaji",
                //action = "RecordsDetail",
                //paramName1 = "BudgetYear",
                //paramValue1 = expenses_gaji.abeg_budgeting_year.ToString(),
                //paramName2 = "CostCenter",
                //paramValue2 = expenses_gaji.abeg_cost_center_code
                //});
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
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesGajisList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesGajiViewRptModel> data = dbEst.Database.SqlQuery<ExpensesGajiViewRptModel>("exec sp_BudgetExpensesGajisList {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string BudgetYear, string CostCenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesGajisExcelList.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesGajiViewRptModel> data = dbEst.Database.SqlQuery<ExpensesGajiViewRptModel>("exec sp_BudgetExpensesGajisList {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintView(string BudgetYear, string CostCenter, string Format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesGajisView.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesGajiViewRptModel> data = dbEst.Database.SqlQuery<ExpensesGajiViewRptModel>("exec sp_BudgetExpensesGajisView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelView(string BudgetYear, string CostCenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "ExpensesGajisExcelView.rdlc");
            string filename = scr.GetScreen(screenCode).ScrNameLongDesc + " - VIEW";

            try
            {
                lr.ReportPath = path;
            }
            catch (Exception ex)
            {
                errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            List<ExpensesGajiViewRptModel> data = dbEst.Database.SqlQuery<ExpensesGajiViewRptModel>("exec sp_BudgetExpensesGajisView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbEst.Dispose();
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
            var expenses_gajis = GetRecords(budgetYear);
            var excludeCostCenters = new List<string>();

            for (int i = 0; i < expenses_gajis.Count(); i++)
            {
                excludeCostCenters.Add(expenses_gajis[i].CostCenterCode);
            }

            return cc.GetCostCenters()
                .Where(c => !excludeCostCenters.Contains(c.fld_CostCenter.TrimStart(new char[] { '0' })))
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
                    .Where(g => !excludeGLs.Contains(g.fld_GLCode.TrimStart(new char[] { '0' })))
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

        private List<ExpensesGajiListViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_gajis
                        where o.abeg_syarikat_id == syarikatId && o.abeg_ladang_id == ladangId && o.abeg_wilayah_id == wilayahId && o.abeg_Deleted == false
                        //group o by new { o.abeg_budgeting_year, o.abeg_cost_center_code, o.abeg_cost_center_desc,o.abeg_hectare,o.abeg_akt_code,o.abeg_akt_name } into g
                        group o by new { o.abeg_budgeting_year, o.abeg_cost_center_code, o.abeg_cost_center_desc, o.abeg_hectare } into g
                        select new
                        {
                            BudgetYear = g.Key.abeg_budgeting_year,
                            CostCenterCode = g.Key.abeg_cost_center_code,
                            CostCenterDesc = g.Key.abeg_cost_center_desc,
                            //KodAktvt = g.Key.abeg_akt_code,
                            //AktvtDesc = g.Key.abeg_akt_name,
                            Hectare = g.Key.abeg_hectare,
                            Total = g.Sum(o => o.abeg_total)
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
                          select new ExpensesGajiListViewModel
                          {
                              BudgetYear = q.BudgetYear,
                              CostCenterCode = q.CostCenterCode,
                              CostCenterDesc = q.CostCenterDesc,
                              //KodAktvt = q.KodAktvt,
                              //AktvtDesc = q.AktvtDesc,
                              Hectare = q.Hectare,
                              Total = q.Total
                          }).Distinct();

            return query3.ToList();
        }

        private List<ExpensesGajiListDetailViewModel> GetRecordsDetail(int? budgeting_year = null, string cost_center = null, string gl = null, int? excludeId = null, int? page = null, int? pageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from o in dbEst.bgt_expenses_gajis
                        where !o.abeg_Deleted && o.abeg_syarikat_id == syarikatId && o.abeg_ladang_id == ladangId && o.abeg_wilayah_id == wilayahId
                        select o;

            if (budgeting_year.HasValue)
                query = query.Where(q => q.abeg_budgeting_year == budgeting_year);
            if (!string.IsNullOrEmpty(cost_center))
                query = query.Where(q => q.abeg_cost_center_code.Contains(cost_center) || q.abeg_cost_center_desc.Contains(cost_center));
            if (!string.IsNullOrEmpty(gl))
                query = query.Where(q => q.abeg_gl_expenses_code.Contains(gl) || q.abeg_gl_expenses_name.Contains(gl));
            if (excludeId.HasValue)
                query = query.Where(q => q.abeg_id != excludeId);

            var query2 = query.AsQueryable();

            if (page.HasValue && pageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abeg_budgeting_year)
                    .ThenBy(q => q.abeg_cost_center_code)
                    .ThenBy(q => q.abeg_gl_expenses_code)
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abeg_budgeting_year)
                    .ThenBy(q => q.abeg_cost_center_code)
                    .ThenBy(q => q.abeg_gl_expenses_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesGajiListDetailViewModel
                          {
                              abeg_id = q.abeg_id,
                              abeg_budgeting_year = q.abeg_budgeting_year,
                              abeg_syarikat_id = q.abeg_syarikat_id,
                              abeg_syarikat_name = q.abeg_syarikat_name,
                              abeg_ladang_id = q.abeg_ladang_id,
                              abeg_ladang_code = q.abeg_ladang_code,
                              abeg_ladang_name = q.abeg_ladang_name,
                              abeg_wilayah_id = q.abeg_wilayah_id,
                              abeg_wilayah_name = q.abeg_wilayah_name,
                              abeg_cost_center_code = q.abeg_cost_center_code,
                              abeg_cost_center_desc = q.abeg_cost_center_desc,
                              abeg_gl_expenses_code = q.abeg_gl_expenses_code,
                              abeg_gl_expenses_name = q.abeg_gl_expenses_name,
                              abeg_akt_code = q.abeg_akt_code,
                              abeg_akt_name = q.abeg_akt_name,
                              abeg_revision = q.abeg_revision,
                              abeg_note = q.abeg_note,
                              abeg_hectare = q.abeg_hectare,
                              abeg_total = q.abeg_total,
                              abeg_month_1 = q.abeg_month_1,
                              abeg_month_2 = q.abeg_month_2,
                              abeg_month_3 = q.abeg_month_3,
                              abeg_month_4 = q.abeg_month_4,
                              abeg_month_5 = q.abeg_month_5,
                              abeg_month_6 = q.abeg_month_6,
                              abeg_month_7 = q.abeg_month_7,
                              abeg_month_8 = q.abeg_month_8,
                              abeg_month_9 = q.abeg_month_9,
                              abeg_month_10 = q.abeg_month_10,
                              abeg_month_11 = q.abeg_month_11,
                              abeg_month_12 = q.abeg_month_12,
                              abeg_proration = q.abeg_proration,
                             
                              abeg_created_by = q.abeg_created_by
                          }).Distinct();

            return query3.ToList();
        }
        #endregion
    }
}
