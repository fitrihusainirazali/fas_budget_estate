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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Web.Security;
using Rotativa;
using System.Threading.Tasks;
using System.Data;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using System.Net;
using MVC_SYSTEM.ClassBudget;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class PendapatanSawitController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_MasterModels dbc = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_ModelsBudgetEst dbe = new ConnectionBudget().GetConnection();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetConfig GetConfig = new GetConfig();
        Connection Connection = new Connection();
        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        GetBudgetClass GetBudgetClass = new GetBudgetClass();
        CostCenter CostCenter = new CostCenter();
        GL gl = new GL();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        BudgetUserMatrix UserMatrix = new BudgetUserMatrix();//new oct

        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }
        public DateTime GetDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Singapore Standard Time");
            return _localTime;
        }
        public string GetScreenName(string ScreenCode)
        {
            bgt_Screen Screen;
            string value = "";
            Screen = db.bgt_Screens.Where(x => x.ScrCode == ScreenCode).FirstOrDefault();
            if (Screen != null)
            {
                value = Screen.ScrNameLongDesc.ToString();
            }
            return value;
        }

        private List<tbl_SAPCCPUP> GetCostCenters(int budgetYear, bool exclude = true)
        {
            if (exclude)
            {
                var income_sawit = GetRecords(budgetYear);
                var excludeCostCenters = new List<string>();

                for (int i = 0; i < income_sawit.Count(); i++)
                {
                    excludeCostCenters.Add(income_sawit[i].CostCenterCode);
                }

                return CostCenter.GetCostCenters()
                    .Where(c => !excludeCostCenters.Contains(c.fld_CostCenter.TrimStart(new char[] { '0' })))
                    .OrderBy(c => c.fld_CostCenter)
                    .ToList();
            }
            else
            {
                return CostCenter.GetCostCenters();
            }
        }
        private List<IncomeSawitListViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null)
        {
            var syarikatId = syarikat.GetSyarikatID();

            var query = from p in dbe.bgt_income_sawit
                        where p.fld_Deleted == false && p.fld_SyarikatID == syarikatId
                        group p by new { p.abio_budgeting_year, p.abio_cost_center, p.abio_cost_center_desc } into g
                        select new
                        {
                            BudgetYear = g.Key.abio_budgeting_year,
                            CostCenterCode = g.Key.abio_cost_center,
                            CostCenterDesc = g.Key.abio_cost_center_desc
                        };

            if (BudgetYear.HasValue)
                query = query.Where(q => q.BudgetYear == BudgetYear);
            if (!string.IsNullOrEmpty(CostCenter))
                query = query.Where(q => q.CostCenterCode.Contains(CostCenter) || q.CostCenterDesc.Contains(CostCenter));

            var query2 = query.AsQueryable();

            query2 = query2
                .OrderByDescending(q => q.BudgetYear)
                .ThenBy(q => q.CostCenterCode);

            var query3 = (from q in query2.AsEnumerable()
                          select new IncomeSawitListViewModel
                          {
                              BudgetYear = (int)q.BudgetYear,
                              CostCenterCode = q.CostCenterCode,
                              CostCenterDesc = q.CostCenterDesc
                          }).Distinct();

            return query3.ToList();
        }

        /*private List<IncomeSawitListViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null) 
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from p in dbe.bgt_income_sawit
                        where p.fld_Deleted == false && p.fld_SyarikatID == syarikatId && p.fld_LadangID == ladangId && p.fld_WilayahID == wilayahId
                        group p by new { p.abio_budgeting_year, p.abio_cost_center, p.abio_cost_center_desc } into g
                        select new
                        {
                            BudgetYear = g.Key.abio_budgeting_year,
                            CostCenterCode = g.Key.abio_cost_center,
                            CostCenterDesc = g.Key.abio_cost_center_desc
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
                          select new IncomeSawitListViewModel
                          {
                              BudgetYear = (int)q.BudgetYear,
                              CostCenterCode = q.CostCenterCode,
                              CostCenterDesc = q.CostCenterDesc
                          }).Distinct();

            return query3.ToList();
        }*/

        public ActionResult Index()
        {
            return RedirectToAction("Sawit");
        }

        public ActionResult Sawit()
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.view)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }

            List<SelectListItem> BudgetYear = new List<SelectListItem>();
            BudgetYear = new SelectList(db.bgt_Notification.Where(w => w.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.Bgt_Year.ToString(), Text = s.Bgt_Year.ToString() })
                .OrderByDescending(o => o.Text), "Value", "Text").ToList();
            ViewBag.BudgetYear = BudgetYear;

            var SelectedYear = GetYears().FirstOrDefault().ToString();
            ViewBag.SelectedYear = SelectedYear;

            return View();
        }
        public ActionResult _Sawit(string BudgetYear, string CostCenter, int page = 1)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;
            ViewBag.CostCenter = CostCenter;

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            int role = getidentity.RoleID(getuserid).Value;

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<IncomeSawitListViewModel>
            {
                Content = GetRecordsIndex(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecordsIndex(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(records);
        }

        //---NEW----
        public ActionResult Create()
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.add)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }

            ViewBag.IncomeBudget = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.SyarikatID = SyarikatID;
            int Year = GetBudgetClass.GetBudgetYear();

            //Get Screen => GL
            //var ID = GetBudgetClass.GetScreenID("I1");
            //var getAllID = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            //List<Guid> GLID = new List<Guid>();
            //GLID = getAllID.Select(s => s.fld_GLID).ToList();

            var gl_list = dbc.tbl_SAPGLPUP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).ToList();

            var GetGLList = GetBudgetClass.GetFeldaGrp((int)SyarikatID).ToList();
            if (GetGLList.Count > 0)
            {
                var GLcodeSK = GetGLList.Where(w => w.CC_Sort == 1).Select(s => s.GLSawit).FirstOrDefault();
                var GLcodeBL = GetGLList.Where(w => w.CC_Sort == 2).Select(s => s.GLSawit).FirstOrDefault();

                ViewBag.GLcodeSK = GLcodeSK;
                ViewBag.GLcodeBL = GLcodeBL;

                var newGLCodeSK = GLcodeSK;
                var newGLCodeBL = GLcodeBL;

                var gldescsk = gl_list.Where(w => w.fld_GLCode.EndsWith(newGLCodeSK)).Select(s => s.fld_GLDesc).FirstOrDefault();
                var gldescbl = gl_list.Where(w => w.fld_GLCode.EndsWith(newGLCodeBL)).Select(s => s.fld_GLDesc).FirstOrDefault();

                ViewBag.GLdescSK = gldescsk;
                ViewBag.GLdescBL = gldescbl;
            }


            List<SelectListItem> Stesen = new List<SelectListItem>();
            Stesen = new SelectList(db.bgt_Stesen.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fld_StesenID).Select(s => new SelectListItem { Value = s.fld_StesenCode, Text = s.fld_StesenName }).Distinct(), "Value", "Text").ToList();
            ViewBag.StesenList = Stesen;

            var SelectedYear = GetYears().FirstOrDefault().ToString();
            var year = int.Parse(SelectedYear);

            var costCenters = new SelectList(GetCostCenters(year).Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter.TrimStart(new char[] { '0' }),
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();

            ViewBag.CostCenterList = costCenters;

            List<SelectListItem> Product = new List<SelectListItem>();
            Product = new SelectList(db.bgt_Product.Where(x => x.PrdCode == "1801" && x.YearBgt == Year && x.Active == true).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }).Distinct(), "Value", "Text").ToList();
            ViewBag.ProductList = Product;

            var ProductList = db.bgt_Product.Where(x => x.PrdCode == "1801" && x.YearBgt == Year && x.Active == true).ToList();
            if (ProductList.Count() > 0)
            {
                ViewBag.PrdCode = ProductList.FirstOrDefault().PrdCode;
                ViewBag.PrdName = ProductList.FirstOrDefault().PrdName;
                var UomID = ProductList.FirstOrDefault().UOMID;
                var UOMList = db.bgt_UOM.Where(x => x.UOMID == UomID).ToList();
                if (UOMList.Count() > 0)
                {
                    ViewBag.UOM = UOMList.FirstOrDefault().UOM;
                }
            }

            //var getFeldaGrpPriceBTS = db.vw_FeldaGrpPriceBTS.Where(x => x.PriceYear == Year && x.fld_Deleted == false).OrderBy(o => o.fld_ID).ToList();
            //if (getFeldaGrpPriceBTS.Count() > 0)
            //{
            //    ViewBag.getFeldaGrpPriceBTS = getFeldaGrpPriceBTS;
            //    ViewBag.PriceA = getFeldaGrpPriceBTS.Where(x => x.CC_Sort == 1).ToList();
            //    ViewBag.PriceB = getFeldaGrpPriceBTS.Where(x => x.CC_Sort == 2).ToList();
            //}

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IncomeSawitCreateViewModel[] bgtincomesawit, string stationcode, string stationname, string costcenter, string abio_cost_center_desc, string bgtlocation)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.add)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            var BudgetYear = bgtincomesawit.FirstOrDefault().abio_budgeting_year;

            if (ModelState.IsValid)
            {
                try
                {
                    var resultAmount = 0m;
                    var syarikatObj = syarikat.GetSyarikat();
                    var ladangObj = ladang.GetLadang();
                    var wilayahObj = wilayah.GetWilayah();

                    foreach (var val in bgtincomesawit)
                    {
                        var checkdata = dbr.bgt_income_sawit
                        .Where(x => x.abio_budgeting_year == val.abio_budgeting_year && x.abio_cost_center.Equals(costcenter) && x.fld_Deleted == false)
                        .FirstOrDefault();

                        if (checkdata == null)
                        {
                            bgt_income_sawit bis = new bgt_income_sawit()
                            {
                                abio_revision = 0,
                                abio_history = false,
                                abio_status = "DRAFT",
                                abio_budgeting_year = val.abio_budgeting_year,
                                abio_fld_ID = val.abio_fld_ID,
                                abio_company_category = val.abio_company_category,
                                abio_company_code = val.abio_company_code,
                                abio_company_name = val.abio_company_name,
                                abio_product_code = val.abio_product_code,
                                abio_product_name = val.abio_product_name,
                                abio_station_code = stationcode.ToString(),
                                abio_station_name = stationname.ToString(),
                                abio_cost_center = costcenter,
                                abio_cost_center_desc = abio_cost_center_desc,
                                abio_gl_code = val.abio_gl_code,
                                abio_gl_desc = val.abio_gl_desc,
                                abio_uom = val.abio_uom,

                                abio_amount_1 = !string.IsNullOrEmpty(val.abio_amount_1) && decimal.TryParse(val.abio_amount_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_1.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_2 = !string.IsNullOrEmpty(val.abio_amount_2) && decimal.TryParse(val.abio_amount_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_2.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_3 = !string.IsNullOrEmpty(val.abio_amount_3) && decimal.TryParse(val.abio_amount_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_3.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_4 = !string.IsNullOrEmpty(val.abio_amount_4) && decimal.TryParse(val.abio_amount_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_4.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_5 = !string.IsNullOrEmpty(val.abio_amount_5) && decimal.TryParse(val.abio_amount_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_5.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_6 = !string.IsNullOrEmpty(val.abio_amount_6) && decimal.TryParse(val.abio_amount_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_6.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_7 = !string.IsNullOrEmpty(val.abio_amount_7) && decimal.TryParse(val.abio_amount_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_7.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_8 = !string.IsNullOrEmpty(val.abio_amount_8) && decimal.TryParse(val.abio_amount_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_8.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_9 = !string.IsNullOrEmpty(val.abio_amount_9) && decimal.TryParse(val.abio_amount_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_9.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_10 = !string.IsNullOrEmpty(val.abio_amount_10) && decimal.TryParse(val.abio_amount_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_10.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_11 = !string.IsNullOrEmpty(val.abio_amount_11) && decimal.TryParse(val.abio_amount_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_11.Replace(",", "")) : (decimal?)0.00,
                                abio_amount_12 = !string.IsNullOrEmpty(val.abio_amount_12) && decimal.TryParse(val.abio_amount_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_amount_12.Replace(",", "")) : (decimal?)0.00,
                                abio_total_amount_a = !string.IsNullOrEmpty(val.abio_total_amount_a) && decimal.TryParse(val.abio_total_amount_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_amount_a.Replace(",", "")) : (decimal?)0.00,
                                abio_total_amount_b = !string.IsNullOrEmpty(val.abio_total_amount_b) && decimal.TryParse(val.abio_total_amount_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_amount_b.Replace(",", "")) : (decimal?)0.00,
                                abio_grand_total_amount = !string.IsNullOrEmpty(val.abio_grand_total_amount) && decimal.TryParse(val.abio_grand_total_amount.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_grand_total_amount.Replace(",", "")) : (decimal?)0.00,

                                abio_qty_1 = !string.IsNullOrEmpty(val.abio_qty_1) && decimal.TryParse(val.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_1.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_2 = !string.IsNullOrEmpty(val.abio_qty_2) && decimal.TryParse(val.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_2.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_3 = !string.IsNullOrEmpty(val.abio_qty_3) && decimal.TryParse(val.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_3.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_4 = !string.IsNullOrEmpty(val.abio_qty_4) && decimal.TryParse(val.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_4.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_5 = !string.IsNullOrEmpty(val.abio_qty_5) && decimal.TryParse(val.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_5.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_6 = !string.IsNullOrEmpty(val.abio_qty_6) && decimal.TryParse(val.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_6.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_7 = !string.IsNullOrEmpty(val.abio_qty_7) && decimal.TryParse(val.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_7.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_8 = !string.IsNullOrEmpty(val.abio_qty_8) && decimal.TryParse(val.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_8.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_9 = !string.IsNullOrEmpty(val.abio_qty_9) && decimal.TryParse(val.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_9.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_10 = !string.IsNullOrEmpty(val.abio_qty_10) && decimal.TryParse(val.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_10.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_11 = !string.IsNullOrEmpty(val.abio_qty_11) && decimal.TryParse(val.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_11.Replace(",", "")) : (decimal?)0.000,
                                abio_qty_12 = !string.IsNullOrEmpty(val.abio_qty_12) && decimal.TryParse(val.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_12.Replace(",", "")) : (decimal?)0.000,
                                abio_total_qty_a = !string.IsNullOrEmpty(val.abio_total_qty_a) && decimal.TryParse(val.abio_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_qty_a.Replace(",", "")) : (decimal?)0.000,
                                abio_total_qty_b = !string.IsNullOrEmpty(val.abio_total_qty_b) && decimal.TryParse(val.abio_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_qty_b.Replace(",", "")) : (decimal?)0.000,
                                abio_grand_total_quantity = !string.IsNullOrEmpty(val.abio_grand_total_quantity) && decimal.TryParse(val.abio_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_grand_total_quantity.Replace(",", "")) : (decimal?)0.000,

                                abio_price_1 = !string.IsNullOrEmpty(val.abio_price_1) && decimal.TryParse(val.abio_price_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_1.Replace(",", "")) : (decimal?)0.00,
                                abio_price_2 = !string.IsNullOrEmpty(val.abio_price_2) && decimal.TryParse(val.abio_price_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_2.Replace(",", "")) : (decimal?)0.00,
                                abio_price_3 = !string.IsNullOrEmpty(val.abio_price_3) && decimal.TryParse(val.abio_price_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_3.Replace(",", "")) : (decimal?)0.00,
                                abio_price_4 = !string.IsNullOrEmpty(val.abio_price_4) && decimal.TryParse(val.abio_price_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_4.Replace(",", "")) : (decimal?)0.00,
                                abio_price_5 = !string.IsNullOrEmpty(val.abio_price_5) && decimal.TryParse(val.abio_price_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_5.Replace(",", "")) : (decimal?)0.00,
                                abio_price_6 = !string.IsNullOrEmpty(val.abio_price_6) && decimal.TryParse(val.abio_price_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_6.Replace(",", "")) : (decimal?)0.00,
                                abio_price_7 = !string.IsNullOrEmpty(val.abio_price_7) && decimal.TryParse(val.abio_price_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_7.Replace(",", "")) : (decimal?)0.00,
                                abio_price_8 = !string.IsNullOrEmpty(val.abio_price_8) && decimal.TryParse(val.abio_price_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_8.Replace(",", "")) : (decimal?)0.00,
                                abio_price_9 = !string.IsNullOrEmpty(val.abio_price_9) && decimal.TryParse(val.abio_price_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_9.Replace(",", "")) : (decimal?)0.00,
                                abio_price_10 = !string.IsNullOrEmpty(val.abio_price_10) && decimal.TryParse(val.abio_price_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_10.Replace(",", "")) : (decimal?)0.00,
                                abio_price_11 = !string.IsNullOrEmpty(val.abio_price_11) && decimal.TryParse(val.abio_price_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_11.Replace(",", "")) : (decimal?)0.00,
                                abio_price_12 = !string.IsNullOrEmpty(val.abio_price_12) && decimal.TryParse(val.abio_price_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_price_12.Replace(",", "")) : (decimal?)0.00,

                                last_modified = DateTime.Now,
                                fld_Deleted = false,
                                fld_NegaraID = NegaraID,
                                fld_SyarikatID = SyarikatID,
                                fld_WilayahID = val.fld_WilayahID,
                                fld_LadangID = LadangID
                            };
                            dbr.bgt_income_sawit.Add(bis);


                            //========LEVI==========//


                            //if (!string.IsNullOrEmpty(bgtlocation) && bgtlocation  == "1")
                            //{
                            var wilayahname = GetBudgetClass.GetWilayahName(val.fld_WilayahID);
                            var getLeviBtsValue = db.bgt_LeviBTS.Where(w => w.Levi_Year == val.abio_budgeting_year && w.LeviCaj == true && w.Location.ToString() == bgtlocation && w.fld_Deleted == false).ToList();

                            if (getLeviBtsValue.Count > 0)
                            {
                                var getMaxMonth = getLeviBtsValue.Max(m => m.Levi_Month);
                                var leviValue = getLeviBtsValue.Where(w => w.Levi_Month == getMaxMonth).Select(s => s.LeviValue).FirstOrDefault();
                                //var isLeviCharged = getLeviBtsValue.Where(w => w.LeviCaj == true).ToList();
                                //decimal leviValue = 0;
                                //if (isLeviCharged.Count > 0)
                                //{
                                //    var getMaxMonth = isLeviCharged.Max(m => m.Levi_Month);
                                //    leviValue = (decimal)isLeviCharged.Where(w => w.Levi_Month == getMaxMonth).Select(s => s.LeviValue).FirstOrDefault();

                                //    if (leviValue < 0)
                                //    {
                                //        leviValue = 0;
                                //    }
                                //}

                                if (leviValue < 0)
                                {
                                    leviValue = 0;
                                }

                                var ScrID = GetBudgetClass.GetScreenID("E11");
                                var getScr = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ScrID).ToList();
                                Guid GLID = Guid.NewGuid();
                                GLID = getScr.Select(s => s.fld_GLID).FirstOrDefault();
                                var GL = dbc.tbl_SAPGLPUP.Where(x => x.fld_ID == GLID).ToList();

                                var getGLCode = GL.FirstOrDefault().fld_GLCode;
                                var trimGLCode = getGLCode.Remove(0, 2);

                                var GLCode = trimGLCode;
                                var GLDesc = GL.Select(s => s.fld_GLDesc).FirstOrDefault();

                                //old code
                                //var levi1 = bgtincomesawit.Sum(s => s.abio_qty_1) * leviValue;
                                //var levi2 = bgtincomesawit.Sum(s => s.abio_qty_2) * leviValue;
                                //var levi3 = bgtincomesawit.Sum(s => s.abio_qty_3) * leviValue;
                                //var levi4 = bgtincomesawit.Sum(s => s.abio_qty_4) * leviValue;
                                //var levi5 = bgtincomesawit.Sum(s => s.abio_qty_5) * leviValue;
                                //var levi6 = bgtincomesawit.Sum(s => s.abio_qty_6) * leviValue;
                                //var levi7 = bgtincomesawit.Sum(s => s.abio_qty_7) * leviValue;
                                //var levi8 = bgtincomesawit.Sum(s => s.abio_qty_8) * leviValue;
                                //var levi9 = bgtincomesawit.Sum(s => s.abio_qty_9) * leviValue;
                                //var levi10 = bgtincomesawit.Sum(s => s.abio_qty_10) * leviValue;
                                //var levi11 = bgtincomesawit.Sum(s => s.abio_qty_11) * leviValue;
                                //var levi12 = bgtincomesawit.Sum(s => s.abio_qty_12) * leviValue;
                                //var totallevi = Convert.ToDecimal(levi1) + Convert.ToDecimal(levi2) + Convert.ToDecimal(levi3) + Convert.ToDecimal(levi4)
                                //            + Convert.ToDecimal(levi5) + Convert.ToDecimal(levi6) + Convert.ToDecimal(levi7) + Convert.ToDecimal(levi8) + Convert.ToDecimal(levi9)
                                //            + Convert.ToDecimal(levi10) + Convert.ToDecimal(levi11) + Convert.ToDecimal(levi12);

                                //new code
                                var levi1 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_1)) * leviValue;
                                var levi2 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_2)) * leviValue;
                                var levi3 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_3)) * leviValue;
                                var levi4 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_4)) * leviValue;
                                var levi5 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_5)) * leviValue;
                                var levi6 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_6)) * leviValue;
                                var levi7 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_7)) * leviValue;
                                var levi8 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_8)) * leviValue;
                                var levi9 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_9)) * leviValue;
                                var levi10 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_10)) * leviValue;
                                var levi11 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_11)) * leviValue;
                                var levi12 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_12)) * leviValue;
                                var totallevi = levi1 + levi2 + levi3 + levi4 + levi5 + levi6 + levi7 + levi8 + levi9 + levi10 + levi11 + levi12;

                                if (val.abio_company_code == "1")
                                {
                                    //old code
                                    //var amtlevi1 = (val.abio_qty_1.HasValue ? val.abio_qty_1.Value : 0) * leviValue;
                                    //var amtlevi2 = (val.abio_qty_2.HasValue ? val.abio_qty_2.Value : 0) * leviValue;
                                    //var amtlevi3 = (val.abio_qty_3.HasValue ? val.abio_qty_3.Value : 0) * leviValue;
                                    //var amtlevi4 = (val.abio_qty_4.HasValue ? val.abio_qty_4.Value : 0) * leviValue;
                                    //var amtlevi5 = (val.abio_qty_5.HasValue ? val.abio_qty_5.Value : 0) * leviValue;
                                    //var amtlevi6 = (val.abio_qty_6.HasValue ? val.abio_qty_6.Value : 0) * leviValue;
                                    //var amtlevi7 = (val.abio_qty_7.HasValue ? val.abio_qty_7.Value : 0) * leviValue;
                                    //var amtlevi8 = (val.abio_qty_8.HasValue ? val.abio_qty_8.Value : 0) * leviValue;
                                    //var amtlevi9 = (val.abio_qty_9.HasValue ? val.abio_qty_9.Value : 0) * leviValue;
                                    //var amtlevi10 = (val.abio_qty_10.HasValue ? val.abio_qty_10.Value : 0) * leviValue;
                                    //var amtlevi11 = (val.abio_qty_11.HasValue ? val.abio_qty_11.Value : 0) * leviValue;
                                    //var amtlevi12 = (val.abio_qty_12.HasValue ? val.abio_qty_12.Value : 0) * leviValue;
                                    //var totalamtleviA = amtlevi1 + amtlevi2 + amtlevi3 + amtlevi4 + amtlevi5 + amtlevi6 + amtlevi7 + amtlevi8 + amtlevi9 + amtlevi10 + amtlevi11 + amtlevi12;

                                    //new code
                                    var amtlevi1 = (!string.IsNullOrEmpty(val.abio_qty_1) && decimal.TryParse(val.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_1.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi2 = (!string.IsNullOrEmpty(val.abio_qty_2) && decimal.TryParse(val.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_2.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi3 = (!string.IsNullOrEmpty(val.abio_qty_3) && decimal.TryParse(val.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_3.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi4 = (!string.IsNullOrEmpty(val.abio_qty_4) && decimal.TryParse(val.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_4.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi5 = (!string.IsNullOrEmpty(val.abio_qty_5) && decimal.TryParse(val.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_5.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi6 = (!string.IsNullOrEmpty(val.abio_qty_6) && decimal.TryParse(val.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_6.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi7 = (!string.IsNullOrEmpty(val.abio_qty_7) && decimal.TryParse(val.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_7.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi8 = (!string.IsNullOrEmpty(val.abio_qty_8) && decimal.TryParse(val.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_8.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi9 = (!string.IsNullOrEmpty(val.abio_qty_9) && decimal.TryParse(val.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_9.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi10 = (!string.IsNullOrEmpty(val.abio_qty_10) && decimal.TryParse(val.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_10.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi11 = (!string.IsNullOrEmpty(val.abio_qty_11) && decimal.TryParse(val.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_11.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi12 = (!string.IsNullOrEmpty(val.abio_qty_12) && decimal.TryParse(val.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_12.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var totalamtleviA = amtlevi1 + amtlevi2 + amtlevi3 + amtlevi4 + amtlevi5 + amtlevi6 + amtlevi7 + amtlevi8 + amtlevi9 + amtlevi10 + amtlevi11 + amtlevi12;

                                    bgt_expenses_levi bel = new bgt_expenses_levi();
                                    bel.abel_revision = 0;
                                    bel.abel_status = "DRAFT";
                                    bel.abel_history = false;
                                    bel.abel_budgeting_year = val.abio_budgeting_year;
                                    bel.abel_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null;
                                    bel.abel_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                                    bel.abel_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null;
                                    bel.abel_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                                    bel.abel_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                                    //bel.abel_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null;
                                    //bel.abel_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null;
                                    bel.abel_wilayah_id = val.fld_WilayahID != 0 ? val.fld_WilayahID : (int?)null;
                                    bel.abel_wilayah_name = !string.IsNullOrEmpty(wilayahname.ToString()) ? wilayahname.ToString() : null;
                                    bel.abel_company_category = val.abio_company_category;
                                    bel.abel_company_code = val.abio_company_code;
                                    bel.abel_company_name = val.abio_company_name;
                                    bel.abel_uom = val.abio_uom;
                                    bel.abel_product_code = val.abio_product_code;
                                    bel.abel_product_name = val.abio_product_name;
                                    bel.abel_cost_center_code = costcenter;
                                    bel.abel_cost_center_desc = abio_cost_center_desc;
                                    bel.abel_station_code = stationcode.ToString();
                                    bel.abel_station_name = stationname.ToString();
                                    bel.abel_gl_expenses_code = GLCode;
                                    bel.abel_gl_expenses_name = GLDesc;
                                    bel.abel_amount_1 = Math.Round(amtlevi1.Value, 2);
                                    bel.abel_amount_2 = Math.Round(amtlevi2.Value, 2);
                                    bel.abel_amount_3 = Math.Round(amtlevi3.Value, 2);
                                    bel.abel_amount_4 = Math.Round(amtlevi4.Value, 2);
                                    bel.abel_amount_5 = Math.Round(amtlevi5.Value, 2);
                                    bel.abel_amount_6 = Math.Round(amtlevi6.Value, 2);
                                    bel.abel_amount_7 = Math.Round(amtlevi7.Value, 2);
                                    bel.abel_amount_8 = Math.Round(amtlevi8.Value, 2);
                                    bel.abel_amount_9 = Math.Round(amtlevi9.Value, 2);
                                    bel.abel_amount_10 = Math.Round(amtlevi10.Value, 2);
                                    bel.abel_amount_11 = Math.Round(amtlevi11.Value, 2);
                                    bel.abel_amount_12 = Math.Round(amtlevi12.Value, 2);

                                    //new code
                                    bel.abel_qty_1 = !string.IsNullOrEmpty(val.abio_qty_1) && decimal.TryParse(val.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_1.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_2 = !string.IsNullOrEmpty(val.abio_qty_2) && decimal.TryParse(val.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_2.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_3 = !string.IsNullOrEmpty(val.abio_qty_3) && decimal.TryParse(val.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_3.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_4 = !string.IsNullOrEmpty(val.abio_qty_4) && decimal.TryParse(val.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_4.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_5 = !string.IsNullOrEmpty(val.abio_qty_5) && decimal.TryParse(val.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_5.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_6 = !string.IsNullOrEmpty(val.abio_qty_6) && decimal.TryParse(val.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_6.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_7 = !string.IsNullOrEmpty(val.abio_qty_7) && decimal.TryParse(val.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_7.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_8 = !string.IsNullOrEmpty(val.abio_qty_8) && decimal.TryParse(val.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_8.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_9 = !string.IsNullOrEmpty(val.abio_qty_9) && decimal.TryParse(val.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_9.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_10 = !string.IsNullOrEmpty(val.abio_qty_10) && decimal.TryParse(val.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_10.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_11 = !string.IsNullOrEmpty(val.abio_qty_11) && decimal.TryParse(val.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_11.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_qty_12 = !string.IsNullOrEmpty(val.abio_qty_12) && decimal.TryParse(val.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_12.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_total_qty_a = !string.IsNullOrEmpty(val.abio_total_qty_a) && decimal.TryParse(val.abio_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_qty_a.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_total_qty_b = !string.IsNullOrEmpty(val.abio_total_qty_b) && decimal.TryParse(val.abio_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_qty_b.Replace(",", "")) : (decimal?)0.00;
                                    bel.abel_grand_total_quantity = !string.IsNullOrEmpty(val.abio_grand_total_quantity) && decimal.TryParse(val.abio_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_grand_total_quantity.Replace(",", "")) : (decimal?)0.00;

                                    bel.abel_price_1 = leviValue;
                                    bel.abel_price_2 = leviValue;
                                    bel.abel_price_3 = leviValue;
                                    bel.abel_price_4 = leviValue;
                                    bel.abel_price_5 = leviValue;
                                    bel.abel_price_6 = leviValue;
                                    bel.abel_price_7 = leviValue;
                                    bel.abel_price_8 = leviValue;
                                    bel.abel_price_9 = leviValue;
                                    bel.abel_price_10 = leviValue;
                                    bel.abel_price_11 = leviValue;
                                    bel.abel_price_12 = leviValue;
                                    bel.abel_total_amount_a = Math.Round(totalamtleviA.Value, 2);
                                    bel.abel_total_amount_b = 0m;
                                    bel.abel_grand_total_amount = Math.Round(totalamtleviA.Value, 2);
                                    bel.last_modified = DateTime.Now;
                                    bel.abel_created_by = getidentity.ID(User.Identity.Name);
                                    bel.fld_Deleted = false;
                                    dbr.bgt_expenses_levi.Add(bel);
                                }
                                else
                                {
                                    //old
                                    //var amtlevi1 = (val.abio_qty_1.HasValue ? val.abio_qty_1.Value : 0) * leviValue;
                                    //var amtlevi2 = (val.abio_qty_2.HasValue ? val.abio_qty_2.Value : 0) * leviValue;
                                    //var amtlevi3 = (val.abio_qty_3.HasValue ? val.abio_qty_3.Value : 0) * leviValue;
                                    //var amtlevi4 = (val.abio_qty_4.HasValue ? val.abio_qty_4.Value : 0) * leviValue;
                                    //var amtlevi5 = (val.abio_qty_5.HasValue ? val.abio_qty_5.Value : 0) * leviValue;
                                    //var amtlevi6 = (val.abio_qty_6.HasValue ? val.abio_qty_6.Value : 0) * leviValue;
                                    //var amtlevi7 = (val.abio_qty_7.HasValue ? val.abio_qty_7.Value : 0) * leviValue;
                                    //var amtlevi8 = (val.abio_qty_8.HasValue ? val.abio_qty_8.Value : 0) * leviValue;
                                    //var amtlevi9 = (val.abio_qty_9.HasValue ? val.abio_qty_9.Value : 0) * leviValue;
                                    //var amtlevi10 = (val.abio_qty_10.HasValue ? val.abio_qty_10.Value : 0) * leviValue;
                                    //var amtlevi11 = (val.abio_qty_11.HasValue ? val.abio_qty_11.Value : 0) * leviValue;
                                    //var amtlevi12 = (val.abio_qty_12.HasValue ? val.abio_qty_12.Value : 0) * leviValue;
                                    //var totalamtleviB = Convert.ToDecimal(amtlevi1) + Convert.ToDecimal(amtlevi2) + Convert.ToDecimal(amtlevi3) + Convert.ToDecimal(amtlevi4) + Convert.ToDecimal(amtlevi5)
                                    // + Convert.ToDecimal(amtlevi6) + Convert.ToDecimal(amtlevi7) + Convert.ToDecimal(amtlevi8) + Convert.ToDecimal(amtlevi9) + Convert.ToDecimal(amtlevi10)
                                    // + Convert.ToDecimal(amtlevi11) + Convert.ToDecimal(amtlevi12);

                                    //new code
                                    var amtlevi1 = (!string.IsNullOrEmpty(val.abio_qty_1) && decimal.TryParse(val.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_1.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi2 = (!string.IsNullOrEmpty(val.abio_qty_2) && decimal.TryParse(val.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_2.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi3 = (!string.IsNullOrEmpty(val.abio_qty_3) && decimal.TryParse(val.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_3.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi4 = (!string.IsNullOrEmpty(val.abio_qty_4) && decimal.TryParse(val.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_4.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi5 = (!string.IsNullOrEmpty(val.abio_qty_5) && decimal.TryParse(val.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_5.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi6 = (!string.IsNullOrEmpty(val.abio_qty_6) && decimal.TryParse(val.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_6.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi7 = (!string.IsNullOrEmpty(val.abio_qty_7) && decimal.TryParse(val.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_7.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi8 = (!string.IsNullOrEmpty(val.abio_qty_8) && decimal.TryParse(val.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_8.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi9 = (!string.IsNullOrEmpty(val.abio_qty_9) && decimal.TryParse(val.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_9.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi10 = (!string.IsNullOrEmpty(val.abio_qty_10) && decimal.TryParse(val.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_10.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi11 = (!string.IsNullOrEmpty(val.abio_qty_11) && decimal.TryParse(val.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_11.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var amtlevi12 = (!string.IsNullOrEmpty(val.abio_qty_12) && decimal.TryParse(val.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_12.Replace(",", "")) : (decimal?)0.00) * leviValue;
                                    var totalamtleviB = amtlevi1 + amtlevi2 + amtlevi3 + amtlevi4 + amtlevi5 + amtlevi6 + amtlevi7 + amtlevi8 + amtlevi9 + amtlevi10 + amtlevi11 + amtlevi12;

                                    bgt_expenses_levi bel = new bgt_expenses_levi()
                                    {
                                        abel_revision = 0,
                                        abel_status = "DRAFT",
                                        abel_history = false,
                                        abel_budgeting_year = val.abio_budgeting_year,
                                        abel_syarikat_id = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null,
                                        abel_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                                        abel_ladang_id = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null,
                                        abel_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                                        abel_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                                        //abel_wilayah_id = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null,
                                        //abel_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                                        abel_wilayah_id = val.fld_WilayahID != 0 ? val.fld_WilayahID : (int?)null,
                                        abel_wilayah_name = !string.IsNullOrEmpty(wilayahname.ToString()) ? wilayahname.ToString() : null,
                                        abel_company_category = val.abio_company_category,
                                        abel_company_code = val.abio_company_code,
                                        abel_company_name = val.abio_company_name,
                                        abel_uom = val.abio_uom,
                                        abel_product_code = val.abio_product_code,
                                        abel_product_name = val.abio_product_name,
                                        abel_cost_center_code = costcenter,
                                        abel_cost_center_desc = abio_cost_center_desc,
                                        abel_station_code = stationcode.ToString(),
                                        abel_station_name = stationname.ToString(),
                                        abel_gl_expenses_code = GLCode,
                                        abel_gl_expenses_name = GLDesc,
                                        abel_amount_1 = Math.Round(amtlevi1.Value, 2),
                                        abel_amount_2 = Math.Round(amtlevi2.Value, 2),
                                        abel_amount_3 = Math.Round(amtlevi3.Value, 2),
                                        abel_amount_4 = Math.Round(amtlevi4.Value, 2),
                                        abel_amount_5 = Math.Round(amtlevi5.Value, 2),
                                        abel_amount_6 = Math.Round(amtlevi6.Value, 2),
                                        abel_amount_7 = Math.Round(amtlevi7.Value, 2),
                                        abel_amount_8 = Math.Round(amtlevi8.Value, 2),
                                        abel_amount_9 = Math.Round(amtlevi9.Value, 2),
                                        abel_amount_10 = Math.Round(amtlevi10.Value, 2),
                                        abel_amount_11 = Math.Round(amtlevi11.Value, 2),
                                        abel_amount_12 = Math.Round(amtlevi12.Value, 2),

                                        //new code
                                        abel_qty_1 = !string.IsNullOrEmpty(val.abio_qty_1) && decimal.TryParse(val.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_1.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_2 = !string.IsNullOrEmpty(val.abio_qty_2) && decimal.TryParse(val.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_2.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_3 = !string.IsNullOrEmpty(val.abio_qty_3) && decimal.TryParse(val.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_3.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_4 = !string.IsNullOrEmpty(val.abio_qty_4) && decimal.TryParse(val.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_4.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_5 = !string.IsNullOrEmpty(val.abio_qty_5) && decimal.TryParse(val.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_5.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_6 = !string.IsNullOrEmpty(val.abio_qty_6) && decimal.TryParse(val.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_6.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_7 = !string.IsNullOrEmpty(val.abio_qty_7) && decimal.TryParse(val.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_7.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_8 = !string.IsNullOrEmpty(val.abio_qty_8) && decimal.TryParse(val.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_8.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_9 = !string.IsNullOrEmpty(val.abio_qty_9) && decimal.TryParse(val.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_9.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_10 = !string.IsNullOrEmpty(val.abio_qty_10) && decimal.TryParse(val.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_10.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_11 = !string.IsNullOrEmpty(val.abio_qty_11) && decimal.TryParse(val.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_11.Replace(",", "")) : (decimal?)0.00,
                                        abel_qty_12 = !string.IsNullOrEmpty(val.abio_qty_12) && decimal.TryParse(val.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_qty_12.Replace(",", "")) : (decimal?)0.00,
                                        abel_total_qty_a = !string.IsNullOrEmpty(val.abio_total_qty_a) && decimal.TryParse(val.abio_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_qty_a.Replace(",", "")) : (decimal?)0.00,
                                        abel_total_qty_b = !string.IsNullOrEmpty(val.abio_total_qty_b) && decimal.TryParse(val.abio_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_total_qty_b.Replace(",", "")) : (decimal?)0.00,
                                        abel_grand_total_quantity = !string.IsNullOrEmpty(val.abio_grand_total_quantity) && decimal.TryParse(val.abio_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abio_grand_total_quantity.Replace(",", "")) : (decimal?)0.00,

                                        abel_price_1 = leviValue,
                                        abel_price_2 = leviValue,
                                        abel_price_3 = leviValue,
                                        abel_price_4 = leviValue,
                                        abel_price_5 = leviValue,
                                        abel_price_6 = leviValue,
                                        abel_price_7 = leviValue,
                                        abel_price_8 = leviValue,
                                        abel_price_9 = leviValue,
                                        abel_price_10 = leviValue,
                                        abel_price_11 = leviValue,
                                        abel_price_12 = leviValue,
                                        abel_total_amount_a = 0m,
                                        abel_total_amount_b = Math.Round(totalamtleviB.Value, 2),
                                        abel_grand_total_amount = Math.Round(totalamtleviB.Value, 2),
                                        last_modified = DateTime.Now,
                                        abel_created_by = getidentity.ID(User.Identity.Name),
                                        fld_Deleted = false
                                    };
                                    dbr.bgt_expenses_levi.Add(bel);
                                }
                            }
                            //}
                        }
                        else
                        {
                            db.Dispose();
                            dbr.Dispose();
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "0" });
                        }
                    }
                    dbr.SaveChanges();
                    db.Dispose();
                    dbr.Dispose();

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
                        checkingdata = "0",
                        method = "2",
                        div = "sawit",
                        rootUrl = domain,
                        action = "_Sawit",
                        controller = "PendapatanSawit",
                        paramName = "BudgetYear",
                        paramValue = BudgetYear
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    db.Dispose();
                    dbr.Dispose();
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                db.Dispose();
                dbr.Dispose();
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }
        //---END NEW---



        public JsonResult GetProduct(string productcode)
        {
            var ProductData = db.bgt_Product.Where(x => x.PrdCode == productcode).FirstOrDefault();

            if (ProductData != null)
            {
                var GLData = dbc.tbl_SAPGLPUP.Where(x => x.fld_GLCode == ProductData.PrdCode).ToList();
                if (GLData.Count() > 0)
                {
                    return Json(new { success = true, productcode = ProductData.PrdCode, productname = ProductData.PrdName, glcode = GLData.FirstOrDefault().fld_GLCode, gldesc = GLData.FirstOrDefault().fld_GLDesc });
                }
                else
                {
                    return Json(new { success = true, productcode = ProductData.PrdCode, productname = ProductData.PrdName, glcode = "", gldesc = "" });
                }
            }
            return Json(new { success = false });
        }


        public JsonResult GetDataFromCostCenter(string costcentercode)
        {
            string newcostcentercode = "";

            if (costcentercode.StartsWith("P"))
            {
                newcostcentercode = costcentercode;
            }
            else
            {
                newcostcentercode = "0" + costcentercode;
            }

            var CostCenterData = dbc.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == newcostcentercode).ToList();

            if (CostCenterData.Count() > 0)
            {
                var cccode = CostCenterData.FirstOrDefault().fld_CostCenter;
                var trimcccode = cccode.Substring(5, 2);
                var StationNCData = db.bgt_Station_NC.Where(w => w.Code_LL == trimcccode.Replace(" ", "")).FirstOrDefault();

                string getFirstStr = "";
                string TrimedFirstCC = "";
                getFirstStr = cccode.Substring(0, 1);

                if (getFirstStr == "0")
                {
                    TrimedFirstCC = cccode.Remove(0, 1);
                }
                else
                {
                    TrimedFirstCC = cccode;
                }

                var SelectedYear = GetYears().FirstOrDefault().ToString();
                var year = int.Parse(SelectedYear);
                var wilayah_ID = CostCenterData.FirstOrDefault().fld_WilayahID;//
                var resultAmount = 0m;

                var DataTblPriceBTS = db.bgt_PriceBTS.Where(w => w.fld_ID == wilayah_ID && w.PriceYear == year && w.fld_Deleted == false);

                if (StationNCData != null)
                {
                    return Json(new
                    {
                        success = true,
                        //costcenter = CostCenterData.FirstOrDefault().fld_CostCenter,
                        costcenterdesc = CostCenterData.FirstOrDefault().fld_CostCenterDesc,
                        stationcode = StationNCData.Code_LL,
                        stationdesc = StationNCData.Station_Desc,
                        costcentertrim = TrimedFirstCC,
                        bgtlocation = DataTblPriceBTS.FirstOrDefault().fld_ID,
                        wilayahid = CostCenterData.FirstOrDefault().fld_WilayahID,
                        Price1 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price1.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price1.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price1.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price2 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price2.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price2.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price2.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price3 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price3.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price3.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price3.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price4 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price4.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price4.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price4.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price5 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price5.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price5.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price5.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price6 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price6.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price6.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price6.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price7 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price7.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price7.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price7.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price8 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price8.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price8.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price8.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price9 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price9.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price9.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price9.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price10 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price10.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price10.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price10.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price11 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price11.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price11.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price11.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                        Price12 = !string.IsNullOrEmpty(DataTblPriceBTS.FirstOrDefault().Price12.Value.ToString("#,##0.00")) && decimal.TryParse(DataTblPriceBTS.FirstOrDefault().Price12.Value.ToString("#,##0.00").Replace(",", ""), out resultAmount) ? decimal.Parse(DataTblPriceBTS.FirstOrDefault().Price12.Value.ToString("#,##0.00").Replace(",", "")) : (decimal?)0.00,
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        costcenter = CostCenterData.FirstOrDefault().fld_CostCenter,
                        costcenterdesc = CostCenterData.FirstOrDefault().fld_CostCenterDesc,
                        stationcode = "",
                        stationdesc = "",
                        costcentertrim = TrimedFirstCC
                    });
                }
            }
            return Json(new { success = false });
        }

        public ActionResult ViewSawit(string costcenter, int budgetyear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.view)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            if (budgetyear >= 2024)
            {
                bgt_income_sawit[] bgt_income_sawit = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter && x.abio_budgeting_year == budgetyear && x.fld_Deleted == false).ToArray();

                ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                ViewBag.BudgetYear = bgt_income_sawit.Select(s => s.abio_budgeting_year).FirstOrDefault();

                ViewBag.GLcodeSK = bgt_income_sawit.Where(w => w.abio_company_code == "1").Select(s => s.abio_gl_code).FirstOrDefault();
                ViewBag.GLdescSK = bgt_income_sawit.Where(w => w.abio_company_code == "1").Select(s => s.abio_gl_desc).FirstOrDefault().ToUpper();
                ViewBag.GLcodeBL = bgt_income_sawit.Where(w => w.abio_company_code == "2").Select(s => s.abio_gl_code).FirstOrDefault();
                ViewBag.GLdescBL = bgt_income_sawit.Where(w => w.abio_company_code == "2").Select(s => s.abio_gl_desc).FirstOrDefault().ToUpper();

                ViewBag.ProductCode = bgt_income_sawit.Select(s => s.abio_product_code).FirstOrDefault();
                ViewBag.ProductName = bgt_income_sawit.Select(s => s.abio_product_name).FirstOrDefault().ToUpper();

                int totalpage = 0;
                int page = 0;
                int pagesize = 0;
                int totalldata = 0;
                var bgtincomedata = bgt_income_sawit.GroupBy(g => new { g.abio_cost_center, g.abio_budgeting_year }).ToList();

                totalldata = bgtincomedata.Count();
                pagesize = 12;
                totalpage = totalldata / pagesize;
                page = totalldata / pagesize;

                if (totalpage == 0)
                {
                    int newtotalpage = totalpage + 1;
                    ViewBag.newtotalpage = newtotalpage;
                }
                else
                {
                    int newtotalpage = totalpage + 1;
                    ViewBag.newtotalpage = newtotalpage;
                }
                if (page == 0)
                {
                    int curretpage = 1;
                    ViewBag.curretpage = curretpage;
                }
                else
                {
                    int curretpage = 1;
                    ViewBag.curretpage = curretpage;
                }
                return PartialView(bgt_income_sawit);
            }
            else
            {
                bgt_income_sawit[] bgt_income_sawit = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter && x.abio_budgeting_year == budgetyear).ToArray();

                ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                ViewBag.BudgetYear = bgt_income_sawit.Select(s => s.abio_budgeting_year).FirstOrDefault();

                ViewBag.fld_GLCode = bgt_income_sawit.Select(s => s.abio_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_income_sawit.Select(s => s.abio_gl_desc).FirstOrDefault();

                ViewBag.ProductCode = bgt_income_sawit.Select(s => s.abio_product_code).FirstOrDefault();
                ViewBag.ProductName = bgt_income_sawit.Select(s => s.abio_product_name).FirstOrDefault();

                int totalpage = 0;
                int page = 0;
                int pagesize = 0;
                int totalldata = 0;
                var bgtincomedata = bgt_income_sawit.GroupBy(g => new { g.abio_cost_center, g.abio_budgeting_year }).ToList();

                totalldata = bgtincomedata.Count();
                pagesize = 12;
                totalpage = totalldata / pagesize;
                page = totalldata / pagesize;

                if (totalpage == 0)
                {
                    int newtotalpage = totalpage + 1;
                    ViewBag.newtotalpage = newtotalpage;
                }
                else
                {
                    int newtotalpage = totalpage + 1;
                    ViewBag.newtotalpage = newtotalpage;
                }
                if (page == 0)
                {
                    int curretpage = 1;
                    ViewBag.curretpage = curretpage;
                }
                else
                {
                    int curretpage = 1;
                    ViewBag.curretpage = curretpage;
                }
                return PartialView(bgt_income_sawit);
            }
        }

        public ActionResult UpdateSawit(string costcenter, string budgetyear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.edit)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            if (string.IsNullOrEmpty(costcenter) || string.IsNullOrEmpty(budgetyear))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //bgt_income_sawit[] bgt_income_sawit = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter && x.abio_budgeting_year.ToString() == budgetyear && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToArray();
            bgt_income_sawit[] bgt_income_sawit = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter && x.abio_budgeting_year.ToString() == budgetyear && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToArray();
            var getfldgrp = dbr.bgt_income_sawit.Where(w => w.abio_cost_center == costcenter && w.abio_budgeting_year.ToString() == budgetyear).ToList();

            ViewBag.SyarikatID = SyarikatID;
            ViewBag.GetData = getfldgrp;
            ViewBag.GroupCompanyA = getfldgrp.Where(w => w.abio_company_code == "1").ToList();
            ViewBag.GroupCompanyB = getfldgrp.Where(w => w.abio_company_code == "2").ToList();
            ViewBag.GroupCompanyACount = getfldgrp.Count();
            ViewBag.GroupCompanyBCount = getfldgrp.Count();

            var ProductList = db.bgt_Product.Where(x => x.PrdCode == "1801" && x.YearBgt.ToString() == budgetyear && x.Active == true).ToList();
            if (ProductList.Count() > 0)
            {
                ViewBag.PrdCode = ProductList.FirstOrDefault().PrdCode;
                ViewBag.PrdName = ProductList.FirstOrDefault().PrdName;
                var UomID = ProductList.FirstOrDefault().UOMID;
                var UOMList = db.bgt_UOM.Where(x => x.UOMID == UomID).ToList();
                if (UOMList.Count() > 0)
                {
                    ViewBag.UOM = UOMList.FirstOrDefault().UOM;
                }
            }

            var wilayah_ID = bgt_income_sawit.FirstOrDefault().fld_WilayahID;//

            var DataTblPriceBTS = db.bgt_PriceBTS.Where(w => w.fld_ID == wilayah_ID && w.PriceYear.ToString() == budgetyear && w.fld_Deleted == false).ToList();
            if (DataTblPriceBTS.Count > 0)
            {
                ViewBag.PriceBTS = DataTblPriceBTS;
            }

            return PartialView(bgt_income_sawit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSawit(IncomeSawitUpdateViewModel[] bgtincomesawit, string stationcode, string stationname, string cost_center, string cost_center_desc)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.edit)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            var cc = Request["CostCenter"];
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            int Year = DateTime.Now.Year + 1;
            var resultAmount = 0m;
            var bgtlocation = bgtincomesawit.FirstOrDefault().fld_WilayahID; //new sept

            try
            {
                foreach (var bis in bgtincomesawit)
                {
                    //var getdata = dbr.bgt_income_sawit.Where(w => w.abio_id == bis.abio_id && w.abio_cost_center == cost_center && w.abio_budgeting_year == Year && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
                    var getdata = dbr.bgt_income_sawit.Where(w => w.abio_id == bis.abio_id && w.abio_cost_center == cost_center && w.abio_budgeting_year == Year && w.fld_LadangID == LadangID && w.fld_WilayahID == bis.fld_WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();

                    getdata.abio_budgeting_year = bis.abio_budgeting_year;
                    getdata.abio_fld_ID = bis.abio_fld_ID;//
                    getdata.abio_company_category = bis.abio_company_category;
                    getdata.abio_company_code = bis.abio_company_code;
                    getdata.abio_company_name = bis.abio_company_name;
                    getdata.abio_product_code = bis.abio_product_code.ToUpper();
                    getdata.abio_product_name = bis.abio_product_name;
                    getdata.abio_gl_code = bis.abio_gl_code;
                    getdata.abio_gl_desc = bis.abio_gl_desc;
                    getdata.abio_uom = bis.abio_uom;
                    getdata.abio_revision = bis.abio_revision;
                    getdata.abio_status = bis.abio_status;
                    getdata.abio_history = bis.abio_history;
                    getdata.abio_cost_center = cost_center;//
                    getdata.abio_cost_center_desc = cost_center_desc;//
                    getdata.abio_station_code = stationcode;//
                    getdata.abio_station_name = stationname;//

                    getdata.abio_amount_1 = !string.IsNullOrEmpty(bis.abio_amount_1) && decimal.TryParse(bis.abio_amount_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_1.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_2 = !string.IsNullOrEmpty(bis.abio_amount_2) && decimal.TryParse(bis.abio_amount_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_2.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_3 = !string.IsNullOrEmpty(bis.abio_amount_3) && decimal.TryParse(bis.abio_amount_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_3.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_4 = !string.IsNullOrEmpty(bis.abio_amount_4) && decimal.TryParse(bis.abio_amount_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_4.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_5 = !string.IsNullOrEmpty(bis.abio_amount_5) && decimal.TryParse(bis.abio_amount_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_5.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_6 = !string.IsNullOrEmpty(bis.abio_amount_6) && decimal.TryParse(bis.abio_amount_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_6.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_7 = !string.IsNullOrEmpty(bis.abio_amount_7) && decimal.TryParse(bis.abio_amount_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_7.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_8 = !string.IsNullOrEmpty(bis.abio_amount_8) && decimal.TryParse(bis.abio_amount_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_8.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_9 = !string.IsNullOrEmpty(bis.abio_amount_9) && decimal.TryParse(bis.abio_amount_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_9.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_10 = !string.IsNullOrEmpty(bis.abio_amount_10) && decimal.TryParse(bis.abio_amount_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_10.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_11 = !string.IsNullOrEmpty(bis.abio_amount_11) && decimal.TryParse(bis.abio_amount_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_11.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_amount_12 = !string.IsNullOrEmpty(bis.abio_amount_12) && decimal.TryParse(bis.abio_amount_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_amount_12.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_total_amount_a = !string.IsNullOrEmpty(bis.abio_total_amount_a) && decimal.TryParse(bis.abio_total_amount_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_amount_a.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_total_amount_b = !string.IsNullOrEmpty(bis.abio_total_amount_b) && decimal.TryParse(bis.abio_total_amount_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_amount_b.Replace(",", "")) : (decimal?)0.00;
                    getdata.abio_grand_total_amount = !string.IsNullOrEmpty(bis.abio_grand_total_amount) && decimal.TryParse(bis.abio_grand_total_amount.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_grand_total_amount.Replace(",", "")) : (decimal?)0.00;

                    getdata.abio_qty_1 = !string.IsNullOrEmpty(bis.abio_qty_1) && decimal.TryParse(bis.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_1.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_2 = !string.IsNullOrEmpty(bis.abio_qty_2) && decimal.TryParse(bis.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_2.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_3 = !string.IsNullOrEmpty(bis.abio_qty_3) && decimal.TryParse(bis.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_3.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_4 = !string.IsNullOrEmpty(bis.abio_qty_4) && decimal.TryParse(bis.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_4.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_5 = !string.IsNullOrEmpty(bis.abio_qty_5) && decimal.TryParse(bis.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_5.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_6 = !string.IsNullOrEmpty(bis.abio_qty_6) && decimal.TryParse(bis.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_6.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_7 = !string.IsNullOrEmpty(bis.abio_qty_7) && decimal.TryParse(bis.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_7.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_8 = !string.IsNullOrEmpty(bis.abio_qty_8) && decimal.TryParse(bis.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_8.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_9 = !string.IsNullOrEmpty(bis.abio_qty_9) && decimal.TryParse(bis.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_9.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_10 = !string.IsNullOrEmpty(bis.abio_qty_10) && decimal.TryParse(bis.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_10.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_11 = !string.IsNullOrEmpty(bis.abio_qty_11) && decimal.TryParse(bis.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_11.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_qty_12 = !string.IsNullOrEmpty(bis.abio_qty_12) && decimal.TryParse(bis.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_12.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_total_qty_a = !string.IsNullOrEmpty(bis.abio_total_qty_a) && decimal.TryParse(bis.abio_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_qty_a.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_total_qty_b = !string.IsNullOrEmpty(bis.abio_total_qty_b) && decimal.TryParse(bis.abio_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_qty_b.Replace(",", "")) : (decimal?)0.000;
                    getdata.abio_grand_total_quantity = !string.IsNullOrEmpty(bis.abio_grand_total_quantity) && decimal.TryParse(bis.abio_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_grand_total_quantity.Replace(",", "")) : (decimal?)0.000;



                    getdata.abio_price_1 = bis.abio_price_1.HasValue ? bis.abio_price_1.Value : 0m;
                    getdata.abio_price_2 = bis.abio_price_2.HasValue ? bis.abio_price_2.Value : 0m;
                    getdata.abio_price_3 = bis.abio_price_3.HasValue ? bis.abio_price_3.Value : 0m;
                    getdata.abio_price_4 = bis.abio_price_4.HasValue ? bis.abio_price_4.Value : 0m;
                    getdata.abio_price_5 = bis.abio_price_5.HasValue ? bis.abio_price_5.Value : 0m;
                    getdata.abio_price_6 = bis.abio_price_6.HasValue ? bis.abio_price_6.Value : 0m;
                    getdata.abio_price_7 = bis.abio_price_7.HasValue ? bis.abio_price_7.Value : 0m;
                    getdata.abio_price_8 = bis.abio_price_8.HasValue ? bis.abio_price_8.Value : 0m;
                    getdata.abio_price_9 = bis.abio_price_9.HasValue ? bis.abio_price_9.Value : 0m;
                    getdata.abio_price_10 = bis.abio_price_10.HasValue ? bis.abio_price_10.Value : 0m;
                    getdata.abio_price_11 = bis.abio_price_11.HasValue ? bis.abio_price_11.Value : 0m;
                    getdata.abio_price_12 = bis.abio_price_12.HasValue ? bis.abio_price_12.Value : 0m;

                    getdata.fld_Deleted = false;
                    getdata.fld_NegaraID = NegaraID;
                    getdata.fld_SyarikatID = SyarikatID;
                    getdata.fld_WilayahID = bis.fld_WilayahID;
                    getdata.fld_LadangID = LadangID;
                    getdata.last_modified = DateTime.Now;
                    dbr.Entry(getdata).State = EntityState.Modified;

                    //=====LEVI=====//

                    //if (bis.fld_WilayahID == 1)
                    //{
                    var getLeviBtsValue = db.bgt_LeviBTS.Where(w => w.Levi_Year == bis.abio_budgeting_year && w.LeviCaj == true && w.Location == bis.fld_WilayahID && w.fld_Deleted == false).ToList();
                    if (getLeviBtsValue.Count > 0)
                    {
                        var getMaxMonth = getLeviBtsValue.Max(m => m.Levi_Month);
                        var leviValue = getLeviBtsValue.Where(w => w.Levi_Month == getMaxMonth).Select(s => s.LeviValue).FirstOrDefault();

                        var ScrID = GetBudgetClass.GetScreenID("E11");
                        var getScr = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ScrID).ToList();
                        Guid GLID = Guid.NewGuid();
                        GLID = getScr.Select(s => s.fld_GLID).FirstOrDefault();
                        var GL = dbc.tbl_SAPGLPUP.Where(x => x.fld_ID == GLID).ToList();
                        var GLCode = GL.Select(s => s.fld_GLCode).FirstOrDefault();
                        var GLDesc = GL.Select(s => s.fld_GLDesc).FirstOrDefault();

                        var levi1 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_1)) * leviValue;
                        var levi2 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_2)) * leviValue;
                        var levi3 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_3)) * leviValue;
                        var levi4 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_4)) * leviValue;
                        var levi5 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_5)) * leviValue;
                        var levi6 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_6)) * leviValue;
                        var levi7 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_7)) * leviValue;
                        var levi8 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_8)) * leviValue;
                        var levi9 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_9)) * leviValue;
                        var levi10 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_10)) * leviValue;
                        var levi11 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_11)) * leviValue;
                        var levi12 = bgtincomesawit.Sum(m => Convert.ToDecimal(m.abio_qty_12)) * leviValue;
                        var totalamtafterlevi = levi1 + levi2 + levi3 + levi4 + levi5 + levi6 + levi7 + levi8 + levi9 + levi10 + levi11 + levi12;

                        if (bis.abio_company_code == "1")
                        {
                            //var amtlevi1 = (bis.abio_qty_1.HasValue ? bis.abio_qty_1.Value : 0) * leviValue;
                            //var amtlevi2 = (bis.abio_qty_2.HasValue ? bis.abio_qty_2.Value : 0) * leviValue;
                            //var amtlevi3 = (bis.abio_qty_3.HasValue ? bis.abio_qty_3.Value : 0) * leviValue;
                            //var amtlevi4 = (bis.abio_qty_4.HasValue ? bis.abio_qty_4.Value : 0) * leviValue;
                            //var amtlevi5 = (bis.abio_qty_5.HasValue ? bis.abio_qty_5.Value : 0) * leviValue;
                            //var amtlevi6 = (bis.abio_qty_6.HasValue ? bis.abio_qty_6.Value : 0) * leviValue;
                            //var amtlevi7 = (bis.abio_qty_7.HasValue ? bis.abio_qty_7.Value : 0) * leviValue;
                            //var amtlevi8 = (bis.abio_qty_8.HasValue ? bis.abio_qty_8.Value : 0) * leviValue;
                            //var amtlevi9 = (bis.abio_qty_9.HasValue ? bis.abio_qty_9.Value : 0) * leviValue;
                            //var amtlevi10 = (bis.abio_qty_10.HasValue ? bis.abio_qty_10.Value : 0) * leviValue;
                            //var amtlevi11 = (bis.abio_qty_11.HasValue ? bis.abio_qty_11.Value : 0) * leviValue;
                            //var amtlevi12 = (bis.abio_qty_12.HasValue ? bis.abio_qty_12.Value : 0) * leviValue;

                            //new code
                            var amtlevi1 = (!string.IsNullOrEmpty(bis.abio_qty_1) && decimal.TryParse(bis.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_1.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi2 = (!string.IsNullOrEmpty(bis.abio_qty_2) && decimal.TryParse(bis.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_2.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi3 = (!string.IsNullOrEmpty(bis.abio_qty_3) && decimal.TryParse(bis.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_3.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi4 = (!string.IsNullOrEmpty(bis.abio_qty_4) && decimal.TryParse(bis.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_4.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi5 = (!string.IsNullOrEmpty(bis.abio_qty_5) && decimal.TryParse(bis.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_5.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi6 = (!string.IsNullOrEmpty(bis.abio_qty_6) && decimal.TryParse(bis.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_6.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi7 = (!string.IsNullOrEmpty(bis.abio_qty_7) && decimal.TryParse(bis.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_7.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi8 = (!string.IsNullOrEmpty(bis.abio_qty_8) && decimal.TryParse(bis.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_8.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi9 = (!string.IsNullOrEmpty(bis.abio_qty_9) && decimal.TryParse(bis.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_9.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi10 = (!string.IsNullOrEmpty(bis.abio_qty_10) && decimal.TryParse(bis.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_10.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi11 = (!string.IsNullOrEmpty(bis.abio_qty_11) && decimal.TryParse(bis.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_11.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi12 = (!string.IsNullOrEmpty(bis.abio_qty_12) && decimal.TryParse(bis.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_12.Replace(",", "")) : (decimal?)0.000) * leviValue;

                            var totalamtleviA = amtlevi1 + amtlevi2 + amtlevi3 + amtlevi4 + amtlevi5 + amtlevi6 + amtlevi7 + amtlevi8 + amtlevi9 + amtlevi10 + amtlevi11 + amtlevi12;

                            var getdatalevi = dbr.bgt_expenses_levi.Where(w => w.abel_company_code == bis.abio_company_code && w.abel_company_name == bis.abio_company_name
                                              && w.abel_cost_center_code == cost_center && w.abel_budgeting_year == bis.abio_budgeting_year
                                              && w.abel_ladang_id == bis.fld_LadangID && w.abel_wilayah_id == bis.fld_WilayahID && w.abel_syarikat_id == bis.fld_SyarikatID && w.fld_Deleted == false)
                                              .FirstOrDefault();
                            getdatalevi.abel_price_1 = getdatalevi.abel_price_1.HasValue ? getdatalevi.abel_price_1.Value : 0m;
                            getdatalevi.abel_price_2 = getdatalevi.abel_price_2.HasValue ? getdatalevi.abel_price_2.Value : 0m;
                            getdatalevi.abel_price_3 = getdatalevi.abel_price_3.HasValue ? getdatalevi.abel_price_3.Value : 0m;
                            getdatalevi.abel_price_4 = getdatalevi.abel_price_4.HasValue ? getdatalevi.abel_price_4.Value : 0m;
                            getdatalevi.abel_price_5 = getdatalevi.abel_price_5.HasValue ? getdatalevi.abel_price_5.Value : 0m;
                            getdatalevi.abel_price_6 = getdatalevi.abel_price_6.HasValue ? getdatalevi.abel_price_6.Value : 0m;
                            getdatalevi.abel_price_7 = getdatalevi.abel_price_7.HasValue ? getdatalevi.abel_price_7.Value : 0m;
                            getdatalevi.abel_price_8 = getdatalevi.abel_price_8.HasValue ? getdatalevi.abel_price_8.Value : 0m;
                            getdatalevi.abel_price_9 = getdatalevi.abel_price_9.HasValue ? getdatalevi.abel_price_9.Value : 0m;
                            getdatalevi.abel_price_10 = getdatalevi.abel_price_10.HasValue ? getdatalevi.abel_price_10.Value : 0m;
                            getdatalevi.abel_price_11 = getdatalevi.abel_price_11.HasValue ? getdatalevi.abel_price_11.Value : 0m;
                            getdatalevi.abel_price_12 = getdatalevi.abel_price_12.HasValue ? getdatalevi.abel_price_12.Value : 0m;

                            //getdatalevi.abel_qty_1 = bis.abio_qty_1.HasValue ? bis.abio_qty_1.Value : 0m;
                            //getdatalevi.abel_qty_2 = bis.abio_qty_2.HasValue ? bis.abio_qty_2.Value : 0m;
                            //getdatalevi.abel_qty_3 = bis.abio_qty_3.HasValue ? bis.abio_qty_3.Value : 0m;
                            //getdatalevi.abel_qty_4 = bis.abio_qty_4.HasValue ? bis.abio_qty_4.Value : 0m;
                            //getdatalevi.abel_qty_5 = bis.abio_qty_5.HasValue ? bis.abio_qty_5.Value : 0m;
                            //getdatalevi.abel_qty_6 = bis.abio_qty_6.HasValue ? bis.abio_qty_6.Value : 0m;
                            //getdatalevi.abel_qty_7 = bis.abio_qty_7.HasValue ? bis.abio_qty_7.Value : 0m;
                            //getdatalevi.abel_qty_8 = bis.abio_qty_8.HasValue ? bis.abio_qty_8.Value : 0m;
                            //getdatalevi.abel_qty_9 = bis.abio_qty_9.HasValue ? bis.abio_qty_9.Value : 0m;
                            //getdatalevi.abel_qty_10 = bis.abio_qty_10.HasValue ? bis.abio_qty_10.Value : 0m;
                            //getdatalevi.abel_qty_11 = bis.abio_qty_11.HasValue ? bis.abio_qty_11.Value : 0m;
                            //getdatalevi.abel_qty_12 = bis.abio_qty_12.HasValue ? bis.abio_qty_12.Value : 0m;

                            //new code
                            getdatalevi.abel_qty_1 = !string.IsNullOrEmpty(bis.abio_qty_1) && decimal.TryParse(bis.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_1.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_2 = !string.IsNullOrEmpty(bis.abio_qty_2) && decimal.TryParse(bis.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_2.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_3 = !string.IsNullOrEmpty(bis.abio_qty_3) && decimal.TryParse(bis.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_3.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_4 = !string.IsNullOrEmpty(bis.abio_qty_4) && decimal.TryParse(bis.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_4.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_5 = !string.IsNullOrEmpty(bis.abio_qty_5) && decimal.TryParse(bis.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_5.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_6 = !string.IsNullOrEmpty(bis.abio_qty_6) && decimal.TryParse(bis.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_6.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_7 = !string.IsNullOrEmpty(bis.abio_qty_7) && decimal.TryParse(bis.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_7.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_8 = !string.IsNullOrEmpty(bis.abio_qty_8) && decimal.TryParse(bis.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_8.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_9 = !string.IsNullOrEmpty(bis.abio_qty_9) && decimal.TryParse(bis.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_9.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_10 = !string.IsNullOrEmpty(bis.abio_qty_10) && decimal.TryParse(bis.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_10.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_11 = !string.IsNullOrEmpty(bis.abio_qty_11) && decimal.TryParse(bis.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_11.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_12 = !string.IsNullOrEmpty(bis.abio_qty_12) && decimal.TryParse(bis.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_12.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_total_qty_a = !string.IsNullOrEmpty(bis.abio_total_qty_a) && decimal.TryParse(bis.abio_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_qty_a.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_total_qty_b = !string.IsNullOrEmpty(bis.abio_total_qty_b) && decimal.TryParse(bis.abio_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_qty_b.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_grand_total_quantity = !string.IsNullOrEmpty(bis.abio_grand_total_quantity) && decimal.TryParse(bis.abio_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_grand_total_quantity.Replace(",", "")) : (decimal?)0.00;

                            getdatalevi.abel_amount_1 = Math.Round(amtlevi1.Value, 2);
                            getdatalevi.abel_amount_2 = Math.Round(amtlevi2.Value, 2);
                            getdatalevi.abel_amount_3 = Math.Round(amtlevi3.Value, 2);
                            getdatalevi.abel_amount_4 = Math.Round(amtlevi4.Value, 2);
                            getdatalevi.abel_amount_5 = Math.Round(amtlevi5.Value, 2);
                            getdatalevi.abel_amount_6 = Math.Round(amtlevi6.Value, 2);
                            getdatalevi.abel_amount_7 = Math.Round(amtlevi7.Value, 2);
                            getdatalevi.abel_amount_8 = Math.Round(amtlevi8.Value, 2);
                            getdatalevi.abel_amount_9 = Math.Round(amtlevi9.Value, 2);
                            getdatalevi.abel_amount_10 = Math.Round(amtlevi10.Value, 2);
                            getdatalevi.abel_amount_11 = Math.Round(amtlevi11.Value, 2);
                            getdatalevi.abel_amount_12 = Math.Round(amtlevi12.Value, 2);
                            getdatalevi.abel_total_amount_a = Math.Round(totalamtleviA.Value, 2);
                            getdatalevi.abel_total_amount_b = 0m;
                            getdatalevi.abel_grand_total_amount = Math.Round(totalamtleviA.Value, 2);
                            getdatalevi.last_modified = DateTime.Now;
                            dbr.Entry(getdatalevi).State = EntityState.Modified;
                        }
                        else
                        {
                            //Get total amt levi B
                            //old code
                            //var amtlevi1 = (bis.abio_qty_1.HasValue ? bis.abio_qty_1.Value : 0) * leviValue;
                            //var amtlevi2 = (bis.abio_qty_2.HasValue ? bis.abio_qty_2.Value : 0) * leviValue;
                            //var amtlevi3 = (bis.abio_qty_3.HasValue ? bis.abio_qty_3.Value : 0) * leviValue;
                            //var amtlevi4 = (bis.abio_qty_4.HasValue ? bis.abio_qty_4.Value : 0) * leviValue;
                            //var amtlevi5 = (bis.abio_qty_5.HasValue ? bis.abio_qty_5.Value : 0) * leviValue;
                            //var amtlevi6 = (bis.abio_qty_6.HasValue ? bis.abio_qty_6.Value : 0) * leviValue;
                            //var amtlevi7 = (bis.abio_qty_7.HasValue ? bis.abio_qty_7.Value : 0) * leviValue;
                            //var amtlevi8 = (bis.abio_qty_8.HasValue ? bis.abio_qty_8.Value : 0) * leviValue;
                            //var amtlevi9 = (bis.abio_qty_9.HasValue ? bis.abio_qty_9.Value : 0) * leviValue;
                            //var amtlevi10 = (bis.abio_qty_10.HasValue ? bis.abio_qty_10.Value : 0) * leviValue;
                            //var amtlevi11 = (bis.abio_qty_11.HasValue ? bis.abio_qty_11.Value : 0) * leviValue;
                            //var amtlevi12 = (bis.abio_qty_12.HasValue ? bis.abio_qty_12.Value : 0) * leviValue
                            //var totalamtleviB = amtlevi1 + amtlevi2 + amtlevi3 + amtlevi4 + amtlevi5 + amtlevi6 + amtlevi7 + amtlevi8 + amtlevi9 + amtlevi10 + amtlevi11 + amtlevi12;

                            //new code
                            var amtlevi1 = (!string.IsNullOrEmpty(bis.abio_qty_1) && decimal.TryParse(bis.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_1.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi2 = (!string.IsNullOrEmpty(bis.abio_qty_2) && decimal.TryParse(bis.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_2.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi3 = (!string.IsNullOrEmpty(bis.abio_qty_3) && decimal.TryParse(bis.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_3.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi4 = (!string.IsNullOrEmpty(bis.abio_qty_4) && decimal.TryParse(bis.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_4.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi5 = (!string.IsNullOrEmpty(bis.abio_qty_5) && decimal.TryParse(bis.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_5.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi6 = (!string.IsNullOrEmpty(bis.abio_qty_6) && decimal.TryParse(bis.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_6.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi7 = (!string.IsNullOrEmpty(bis.abio_qty_7) && decimal.TryParse(bis.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_7.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi8 = (!string.IsNullOrEmpty(bis.abio_qty_8) && decimal.TryParse(bis.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_8.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi9 = (!string.IsNullOrEmpty(bis.abio_qty_9) && decimal.TryParse(bis.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_9.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi10 = (!string.IsNullOrEmpty(bis.abio_qty_10) && decimal.TryParse(bis.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_10.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi11 = (!string.IsNullOrEmpty(bis.abio_qty_11) && decimal.TryParse(bis.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_11.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var amtlevi12 = (!string.IsNullOrEmpty(bis.abio_qty_12) && decimal.TryParse(bis.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_12.Replace(",", "")) : (decimal?)0.000) * leviValue;
                            var totalamtleviB = amtlevi1 + amtlevi2 + amtlevi3 + amtlevi4 + amtlevi5 + amtlevi6 + amtlevi7 + amtlevi8 + amtlevi9 + amtlevi10 + amtlevi11 + amtlevi12;

                            var getdatalevi = dbr.bgt_expenses_levi.Where(w => w.abel_company_code == bis.abio_company_code && w.abel_company_name == bis.abio_company_name
                                              && w.abel_cost_center_code == cost_center && w.abel_budgeting_year == bis.abio_budgeting_year
                                              && w.abel_ladang_id == bis.fld_LadangID && w.abel_wilayah_id == bis.fld_WilayahID && w.abel_syarikat_id == bis.fld_SyarikatID && w.fld_Deleted == false)
                                              .FirstOrDefault();

                            getdatalevi.abel_price_1 = getdatalevi.abel_price_1.HasValue ? getdatalevi.abel_price_1.Value : 0m;
                            getdatalevi.abel_price_2 = getdatalevi.abel_price_2.HasValue ? getdatalevi.abel_price_2.Value : 0m;
                            getdatalevi.abel_price_3 = getdatalevi.abel_price_3.HasValue ? getdatalevi.abel_price_3.Value : 0m;
                            getdatalevi.abel_price_4 = getdatalevi.abel_price_4.HasValue ? getdatalevi.abel_price_4.Value : 0m;
                            getdatalevi.abel_price_5 = getdatalevi.abel_price_5.HasValue ? getdatalevi.abel_price_5.Value : 0m;
                            getdatalevi.abel_price_6 = getdatalevi.abel_price_6.HasValue ? getdatalevi.abel_price_6.Value : 0m;
                            getdatalevi.abel_price_7 = getdatalevi.abel_price_7.HasValue ? getdatalevi.abel_price_7.Value : 0m;
                            getdatalevi.abel_price_8 = getdatalevi.abel_price_8.HasValue ? getdatalevi.abel_price_8.Value : 0m;
                            getdatalevi.abel_price_9 = getdatalevi.abel_price_9.HasValue ? getdatalevi.abel_price_9.Value : 0m;
                            getdatalevi.abel_price_10 = getdatalevi.abel_price_10.HasValue ? getdatalevi.abel_price_10.Value : 0m;
                            getdatalevi.abel_price_11 = getdatalevi.abel_price_11.HasValue ? getdatalevi.abel_price_11.Value : 0m;
                            getdatalevi.abel_price_12 = getdatalevi.abel_price_12.HasValue ? getdatalevi.abel_price_12.Value : 0m;

                            //old code
                            //getdatalevi.abel_qty_1 = bis.abio_qty_1.HasValue ? bis.abio_qty_1.Value : 0m;
                            //getdatalevi.abel_qty_2 = bis.abio_qty_2.HasValue ? bis.abio_qty_2.Value : 0m;
                            //getdatalevi.abel_qty_3 = bis.abio_qty_3.HasValue ? bis.abio_qty_3.Value : 0m;
                            //getdatalevi.abel_qty_4 = bis.abio_qty_4.HasValue ? bis.abio_qty_4.Value : 0m;
                            //getdatalevi.abel_qty_5 = bis.abio_qty_5.HasValue ? bis.abio_qty_5.Value : 0m;
                            //getdatalevi.abel_qty_6 = bis.abio_qty_6.HasValue ? bis.abio_qty_6.Value : 0m;
                            //getdatalevi.abel_qty_7 = bis.abio_qty_7.HasValue ? bis.abio_qty_7.Value : 0m;
                            //getdatalevi.abel_qty_8 = bis.abio_qty_8.HasValue ? bis.abio_qty_8.Value : 0m;
                            //getdatalevi.abel_qty_9 = bis.abio_qty_9.HasValue ? bis.abio_qty_9.Value : 0m;
                            //getdatalevi.abel_qty_10 = bis.abio_qty_10.HasValue ? bis.abio_qty_10.Value : 0m;
                            //getdatalevi.abel_qty_11 = bis.abio_qty_11.HasValue ? bis.abio_qty_11.Value : 0m;
                            //getdatalevi.abel_qty_12 = bis.abio_qty_12.HasValue ? bis.abio_qty_12.Value : 0m;
                            //getdatalevi.abel_total_qty_a = bis.abio_total_qty_a.HasValue ? bis.abio_total_qty_a.Value : 0m;
                            //getdatalevi.abel_total_qty_b = bis.abio_total_qty_b.HasValue ? bis.abio_total_qty_b.Value : 0m;
                            //getdatalevi.abel_grand_total_quantity = bis.abio_grand_total_quantity;

                            //new code
                            getdatalevi.abel_qty_1 = !string.IsNullOrEmpty(bis.abio_qty_1) && decimal.TryParse(bis.abio_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_1.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_2 = !string.IsNullOrEmpty(bis.abio_qty_2) && decimal.TryParse(bis.abio_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_2.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_3 = !string.IsNullOrEmpty(bis.abio_qty_3) && decimal.TryParse(bis.abio_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_3.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_4 = !string.IsNullOrEmpty(bis.abio_qty_4) && decimal.TryParse(bis.abio_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_4.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_5 = !string.IsNullOrEmpty(bis.abio_qty_5) && decimal.TryParse(bis.abio_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_5.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_6 = !string.IsNullOrEmpty(bis.abio_qty_6) && decimal.TryParse(bis.abio_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_6.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_7 = !string.IsNullOrEmpty(bis.abio_qty_7) && decimal.TryParse(bis.abio_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_7.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_8 = !string.IsNullOrEmpty(bis.abio_qty_8) && decimal.TryParse(bis.abio_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_8.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_9 = !string.IsNullOrEmpty(bis.abio_qty_9) && decimal.TryParse(bis.abio_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_9.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_10 = !string.IsNullOrEmpty(bis.abio_qty_10) && decimal.TryParse(bis.abio_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_10.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_11 = !string.IsNullOrEmpty(bis.abio_qty_11) && decimal.TryParse(bis.abio_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_11.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_qty_12 = !string.IsNullOrEmpty(bis.abio_qty_12) && decimal.TryParse(bis.abio_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_qty_12.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_total_qty_a = !string.IsNullOrEmpty(bis.abio_total_qty_a) && decimal.TryParse(bis.abio_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_qty_a.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_total_qty_b = !string.IsNullOrEmpty(bis.abio_total_qty_b) && decimal.TryParse(bis.abio_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_total_qty_b.Replace(",", "")) : (decimal?)0.00;
                            getdatalevi.abel_grand_total_quantity = !string.IsNullOrEmpty(bis.abio_grand_total_quantity) && decimal.TryParse(bis.abio_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(bis.abio_grand_total_quantity.Replace(",", "")) : (decimal?)0.00;

                            getdatalevi.abel_amount_1 = Math.Round(amtlevi1.Value, 2);
                            getdatalevi.abel_amount_2 = Math.Round(amtlevi2.Value, 2);
                            getdatalevi.abel_amount_3 = Math.Round(amtlevi3.Value, 2);
                            getdatalevi.abel_amount_4 = Math.Round(amtlevi4.Value, 2);
                            getdatalevi.abel_amount_5 = Math.Round(amtlevi5.Value, 2);
                            getdatalevi.abel_amount_6 = Math.Round(amtlevi6.Value, 2);
                            getdatalevi.abel_amount_7 = Math.Round(amtlevi7.Value, 2);
                            getdatalevi.abel_amount_8 = Math.Round(amtlevi8.Value, 2);
                            getdatalevi.abel_amount_9 = Math.Round(amtlevi9.Value, 2);
                            getdatalevi.abel_amount_10 = Math.Round(amtlevi10.Value, 2);
                            getdatalevi.abel_amount_11 = Math.Round(amtlevi11.Value, 2);
                            getdatalevi.abel_amount_12 = Math.Round(amtlevi12.Value, 2);
                            getdatalevi.abel_total_amount_a = 0m;
                            getdatalevi.abel_total_amount_b = Math.Round(totalamtleviB.Value, 2);
                            getdatalevi.abel_grand_total_amount = Math.Round(totalamtleviB.Value, 2);
                            getdatalevi.last_modified = DateTime.Now;
                            dbr.Entry(getdatalevi).State = EntityState.Modified;
                        }
                    }
                    //}
                }
                dbr.SaveChanges();

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/") { domain = domain + appname; }

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgUpdate,
                    status = "success",
                    checkingdata = "0",
                    method = "3",
                    div = "sawit",
                    rootUrl = domain,
                    action = "_Sawit",
                    controller = "PendapatanSawit",
                    paramName = "BudgetYear",
                    paramValue = bgtincomesawit.FirstOrDefault().abio_budgeting_year
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }
        public ActionResult DeleteSawit(string costcenter, string budgetyear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.delete)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            bgt_income_sawit[] bgt_income_sawit = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter && x.abio_budgeting_year.ToString() == budgetyear && x.fld_Deleted == false).ToArray();

            var getYear = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter).Select(s => s.abio_budgeting_year).FirstOrDefault();
            var getPrdk = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter).Select(s => s.abio_product_name).FirstOrDefault();
            var getCosC = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter).Select(s => s.abio_cost_center).FirstOrDefault();
            var getCDes = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter).Select(s => s.abio_cost_center_desc).FirstOrDefault();
            var getStat = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter).Select(s => s.abio_station_name).FirstOrDefault();
            var getGLCo = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter).Select(s => s.abio_gl_code).FirstOrDefault();
            var getJuml = dbr.bgt_income_sawit.Where(x => x.abio_cost_center == costcenter).Select(s => s.abio_grand_total_amount).FirstOrDefault();

            ViewBag.getYear = getYear;
            ViewBag.getPrdk = getPrdk;
            ViewBag.getCosC = getCosC;
            ViewBag.getCDes = getCDes;
            ViewBag.getStat = getStat;
            ViewBag.getGLCo = getGLCo;
            ViewBag.getJuml = getJuml;

            return PartialView(bgt_income_sawit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSawit(bgt_income_sawit[] deletebgtincomesawit)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.delete)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            try
            {
                foreach (var bis in deletebgtincomesawit)
                {
                    var getdata = dbr.bgt_income_sawit.Where(w => w.abio_id == bis.abio_id && w.abio_cost_center == bis.abio_cost_center && w.abio_budgeting_year == bis.abio_budgeting_year && w.fld_Deleted == false).FirstOrDefault();

                    if (getdata == null)
                    {
                        return Json(new { success = true, msg = GlobalResEstate.msgNoRecord, status = "alert", checkingdata = "0", method = "2", btn = "" });
                    }
                    else
                    {
                        //getdata.fld_Deleted = true;
                        //dbr.Entry(getdata).State = EntityState.Modified;
                        dbr.bgt_income_sawit.Remove(getdata);
                    }
                }
                foreach (var bis2 in deletebgtincomesawit)
                {
                    var getdatalevi = dbr.bgt_expenses_levi.Where(w => w.abel_cost_center_code == bis2.abio_cost_center && w.abel_budgeting_year == bis2.abio_budgeting_year && w.fld_Deleted == false).ToList();
                    foreach (var val in getdatalevi)
                    {
                        var getdatalevi2 = dbr.bgt_expenses_levi.Where(w => w.abel_id == val.abel_id && w.abel_cost_center_code == bis2.abio_cost_center && w.abel_budgeting_year == bis2.abio_budgeting_year && w.fld_Deleted == false).FirstOrDefault();
                        //getdatalevi2.fld_Deleted = true;
                        //dbr.Entry(getdatalevi2).State = EntityState.Modified;
                        dbr.bgt_expenses_levi.Remove(getdatalevi2);
                    }
                }
                dbr.SaveChanges();

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/") { domain = domain + appname; }

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "2",
                    div = "sawit",
                    rootUrl = domain,
                    action = "_Sawit",
                    controller = "PendapatanSawit",
                    paramName = "BudgetYear",
                    paramValue = deletebgtincomesawit.FirstOrDefault().abio_budgeting_year
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }


        public ActionResult PrintList(string BudgetYear, string CostCenter, string format = "PDF")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomeSawitList.rdlc");
            string filename = GetBudgetClass.GetScreenName("I1") + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeSawitListViewModel> data = dbe.Database.SqlQuery<IncomeSawitListViewModel>("exec sp_BudgetIncomeSawitList {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintView(string BudgetYear, string CostCenter, string format = "PDF")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomeSawitView.rdlc");
            string filename = GetBudgetClass.GetScreenName("I1") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeSawitDetailViewModel> data = dbe.Database.SqlQuery<IncomeSawitDetailViewModel>("exec sp_BudgetIncomeSawitView1 {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintViewExcel(string BudgetYear, string CostCenter, string format = "EXCEL")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I1");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomeSawitViewExcel.rdlc");
            string filename = GetBudgetClass.GetScreenName("I1") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeSawitDetailViewModel> data = dbe.Database.SqlQuery<IncomeSawitDetailViewModel>("exec sp_BudgetIncomeSawitView1 {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        private List<IncomeSawitListViewModel> GetRecordsIndex(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            if (BudgetYear >= 2024)
            {
                var query = dbe.bgt_income_sawit.Where(w => w.fld_Deleted == false)
                .GroupBy(g => new { g.abio_cost_center, g.abio_budgeting_year })
                .Select(g => new
                {
                    BudgetYear = g.FirstOrDefault().abio_budgeting_year,
                    CostCenterCode = g.FirstOrDefault().abio_cost_center,
                    CostCenterDesc = g.FirstOrDefault().abio_cost_center_desc,
                    JenisProduk = g.FirstOrDefault().abio_product_name,
                    Quantity = g.Sum(s => s.abio_grand_total_quantity),
                    Total = g.Sum(s => s.abio_grand_total_amount)
                });

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
                              select new IncomeSawitListViewModel
                              {
                                  BudgetYear = (int)q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  JenisProduk = q.JenisProduk,
                                  Quantity = q.Quantity,
                                  Total = q.Total
                              }).Distinct();

                return query3.ToList();
            }
            else
            {
                var query = dbe.bgt_income_sawit
                .GroupBy(g => new { g.abio_cost_center, g.abio_budgeting_year })
                .Select(g => new
                {
                    BudgetYear = g.FirstOrDefault().abio_budgeting_year,
                    CostCenterCode = g.FirstOrDefault().abio_cost_center,
                    CostCenterDesc = g.FirstOrDefault().abio_cost_center_desc,
                    JenisProduk = g.FirstOrDefault().abio_product_name,
                    Quantity = g.Sum(s => s.abio_grand_total_quantity),
                    Total = g.Sum(s => s.abio_grand_total_amount)
                });

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
                              select new IncomeSawitListViewModel
                              {
                                  BudgetYear = (int)q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  JenisProduk = q.JenisProduk,
                                  Quantity = q.Quantity,
                                  Total = q.Total
                              }).Distinct();

                return query3.ToList();
            }
        }
    }
}