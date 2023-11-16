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
using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using System.IO;
using PagedList;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class PendapatanProdukController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_MasterModels dbc = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_ModelsBudgetEst dbe = new ConnectionBudget().GetConnection();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        Connection Connection = new Connection();
        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        GetBudgetClass GetBudgetClass = new GetBudgetClass();
        CostCenter CostCenter = new CostCenter();
        private CostCenter cc = new CostCenter();
        private errorlog errlog = new errorlog();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private Screen scr = new Screen();
        private GL gl = new GL();
        BudgetUserMatrix UserMatrix = new BudgetUserMatrix();//new oct

        public JsonResult GetPriceSale(int? productid)
        {
            var PrdData = db.bgt_Product.Where(x => x.PrdID == productid && x.Lowest == true && x.Active == true).FirstOrDefault();
            //var PrdData = db.bgt_Product.Find(productid);

            if (PrdData != null)
            {
                var UOMID = PrdData.UOMID;
                var UomData = db.bgt_UOM.Where(w => w.UOMID == UOMID).ToList();
                return Json(new
                {
                    success = true,
                    PrdID = PrdData.PrdID,
                    PrdCode = PrdData.PrdCode,
                    PrdName = PrdData.PrdName,
                    UomID = UomData.FirstOrDefault().UOM,
                    PriceSale = PrdData.PriceSale
                });
            }
            return Json(new { success = false });
        }

        //new aug
        private List<tbl_SAPCCPUP> GetCostCenters(int budgetYear, bool exclude = true)
        {
            if (exclude)
            {
                var income_product = GetRecordsIndex(budgetYear);
                var excludeCostCenters = new List<string>();

                for (int i = 0; i < income_product.Count(); i++)
                {
                    excludeCostCenters.Add(income_product[i].CostCenterCode);
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

        //new aug
        private List<ProductSelectList> GetProducts(int? budgetYear, string costcenter)
        {
            //LocID must be based on user role tagged to location
            var trimcc = costcenter.Substring(4, 2);
            int loccode = 0;

            if(trimcc == "80")
            {
                loccode = 3;//swk
            }
            else if(trimcc == "01" || trimcc == "05" || trimcc == "06" || trimcc == "07" || trimcc == "08" || trimcc == "09" || trimcc == "10" || trimcc == "11" || trimcc == "12" || trimcc == "13" || trimcc == "26" || trimcc == "30" || trimcc == "31" || trimcc == "32" || trimcc == "33" || trimcc == "40" || trimcc == "41" || trimcc == "46" || trimcc == "50")
            {
                loccode = 1;//semenanjung
            }
            else if(trimcc == "70" || trimcc == "71" || trimcc == "73" || trimcc == "74" || trimcc == "75")
            {
                loccode = 2;//sbh
            }
            else
            {
                costcenter = null;//all
            }

            //var WilayahID = wilayah.GetWilayahID();
            //var query = db.bgt_Product.Where(w => w.PrdCode != "1701" && w.PrdCode != "1801" && w.YearBgt == budgetYear && w.Active == true && w.Lowest == true && w.LocID == lokasi).ToList();
            var query = from q in db.bgt_Product 
                        where q.PrdCode != "1701" && q.PrdCode != "1801" && q.YearBgt == budgetYear && q.Active == true && q.Lowest == true
                        group q by new { q.CoCode, q.YearBgt, q.PrdCode, q.LocID } into r
                        select r;

            if (!string.IsNullOrEmpty(costcenter))
                query = query.Where(q => q.FirstOrDefault().LocID == loccode || q.FirstOrDefault().LocID == 0 || q.FirstOrDefault().LocID == 5);

            var query2 = query.AsQueryable();
            query2 = query2.OrderBy(q => q.FirstOrDefault().PrdCode);

            var query3 = (from q in query2.AsEnumerable()
                          select new ProductSelectList
                          {
                              PrdId = q.FirstOrDefault().PrdID.ToString(),
                              PrdCode = q.FirstOrDefault().PrdCode,
                              PrdName = q.FirstOrDefault().PrdName
                          }).Distinct();

            return query3.ToList();
        }

        // new aug
        //public List<tbl_SAPGLPUP> GetGlSk(string glcode)
        //{
        //    using (var db = new MVC_SYSTEM_MasterModels())
        //    {
        //        if (!string.IsNullOrEmpty(glcode))
        //            return db.tbl_SAPGLPUP.Where(g => g.fld_GLCode.Equals(glcode)).OrderBy(g => g.fld_GLCode).ToList();
        //        else
        //            return db.tbl_SAPGLPUP.OrderBy(g => g.fld_GLCode).ToList();
        //    }
        //}
        private List<tbl_SAPGLPUP> GetGLs(int? budgetYear = null, string costCenter = null, string excludeRecordId = null)
        {
            var screen = scr.GetScreen("I3");
            var income_product = GetRecordsDetails(budgetYear, costCenter, null, null, excludeRecordId);
            var excludeGLs = new List<string>();

            for (int i = 0; i < income_product.Count(); i++)
            {
                excludeGLs.Add(income_product[i].abip_gl_code);
            }

            return gl.GetGLs(screen.ScrID)
                .Where(g => !excludeGLs.Contains(g.fld_GLCode.TrimStart(new char[] { '0' })))
                .OrderBy(o => o.fld_GLCode)
                .ToList();
        }

        public JsonResult GetCostCenterStation(string costcentercode)
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

            var CostCenterData = dbc.tbl_SAPCCPUP.Where(x => x.fld_CostCenter == newcostcentercode);

            if (CostCenterData.Count() > 0)
            {
                var cccode = CostCenterData.FirstOrDefault().fld_CostCenter;
                var trimcccode = cccode.Substring(5, 2);
                var StationNCData = db.bgt_Station_NC.Where(w => w.Code_LL.Equals(trimcccode));

                if (StationNCData.Count() > 0)
                {
                    return Json(new
                    {
                        success = true,
                        costcenter = CostCenterData.FirstOrDefault().fld_CostCenter,
                        costcenterdesc = CostCenterData.FirstOrDefault().fld_CostCenterDesc,
                        stationcode = StationNCData.FirstOrDefault().Code_LL,
                        stationdesc = StationNCData.FirstOrDefault().Station_Desc
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
                        stationdesc = ""
                    });
                }
            }
            return Json(new { success = false });
        }
        public JsonResult GetGLDesc(string glcode)
        {
            var GLData = dbc.tbl_SAPGLPUP.Where(x => x.fld_GLCode.Contains(glcode) && x.fld_Deleted == false).FirstOrDefault();

            if (GLData != null)
            {
                return Json(new { 
                    success = true, 
                    fld_GLCode = GLData.fld_GLCode.TrimStart(new char[] { '0' }), 
                    fld_GLDesc = GLData.fld_GLDesc 
                });
            }
            return Json(new { success = false });
        }
        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }
        public ActionResult Index()
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
            if (!RoleScreen.view)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            List<SelectListItem> BudgetYear = new List<SelectListItem>();
            BudgetYear = new SelectList(db.bgt_Notification.Where(w => w.fld_Deleted == false).Select(s => new SelectListItem { Value = s.Bgt_Year.ToString(), Text = s.Bgt_Year.ToString() }).OrderByDescending(o => o.Value), "Value", "Text").ToList();
            ViewBag.BudgetYear = BudgetYear;

            var SelectedYear = GetYears().FirstOrDefault().ToString();
            ViewBag.SelectedYear = SelectedYear;

            return View();
        }
        public ActionResult _Index(string BudgetYear, string CostCenter, int page = 1)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;
            ViewBag.CostCenter = CostCenter;

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            int role = GetIdentity.RoleID(getuserid).Value;

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<IncomeProductListViewModel>
            {
                Content = GetRecordsIndex(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecordsIndex(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(records);
        }

        // GET: PendapatanProduk/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bgt_income_product bgt_income_product = await dbe.bgt_income_product.FindAsync(id);
            if (bgt_income_product == null)
            {
                return HttpNotFound();
            }
            return View(bgt_income_product);
        }

        // GET: PendapatanProduk/Create
        public ActionResult Create()
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
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

            //int Year = GetBudgetClass.GetBudgetYear();
            var SelectedYear = GetYears().FirstOrDefault().ToString();
            var Year = int.Parse(SelectedYear);

            //Get CC
            //var IncludeCC = dbr.bgt_income_product
            //    .Where(w => w.abip_budgeting_year == Year && w.abip_syarikat_id == SyarikatID && w.abip_wilayah_id == WilayahID && w.abip_ladang_id == LadangID && w.fld_Deleted == false)
            //    .Select(s => s.abip_cost_center)
            //    .ToList();

            //List<SelectListItem> CostCenter = new List<SelectListItem>();
            //CostCenter = new SelectList(dbc.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }).Distinct(), "Value", "Text").ToList();
            //CostCenter.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            //ViewBag.CostCenterList = CostCenter;

            var costCenters = new SelectList(GetCostCenters(Year).Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter.TrimStart(new char[] { '0' }),
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();

            ViewBag.CostCenterList = costCenters;

            //List<SelectListItem> Product = new List<SelectListItem>();
            //Product = new SelectList(db.bgt_Product.Where(x => x.Lowest == true && x.YearBgt == Year && x.Active == true).OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text").ToList();
            //ViewBag.ProductList = Product;

            var ProductList = db.bgt_Product.Where(x => x.Lowest == true && x.YearBgt == Year && x.Active == true).ToList();
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

            //Get Price
            var getPriceBijiBenih = db.bgt_FeldaGrp.Where(x => x.PriceYear == Year && x.Co_Active == true).OrderBy(o => o.fld_ID).ToList();
            if (getPriceBijiBenih.Count() > 0)
            {
                ViewBag.getPriceBijiBenih = getPriceBijiBenih;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string BudgetYear, string CostCenter, string stationcode, string stationname)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter) || string.IsNullOrEmpty(stationcode) || string.IsNullOrEmpty(stationname))
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Update", new { BudgetYear = BudgetYear, CostCenter = CostCenter, stationcode = stationcode, stationname = stationname });
        }

        public ActionResult Update(string BudgetYear, string CostCenter, string stationcode, string stationname)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter) || string.IsNullOrEmpty(stationcode) || string.IsNullOrEmpty(stationname))
            {
                return RedirectToAction("Create");
            }

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //Produk
            List<SelectListItem> Product = new List<SelectListItem>();
            Product = new SelectList(db.bgt_Product.Where(x => x.Active == true).OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text").ToList();
            Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            ViewBag.ProductList = Product;

            //Get GL
            var IncludeGL = dbr.bgt_income_product
                .Where(w => w.abip_budgeting_year.ToString() == BudgetYear && w.abip_cost_center == CostCenter && w.abip_ladang_id == LadangID && w.abip_syarikat_id == SyarikatID && /*w.abip_wilayah_id == WilayahID &&*/ w.fld_Deleted == false)
                .Select(s => s.abip_gl_code)
                .ToList();

            //Get GL
            var ID = GetBudgetClass.GetScreenID("I3");
            var getAllID = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            List<Guid> GLID = new List<Guid>();
            GLID = getAllID.Select(s => s.fld_GLID).ToList();

            List<SelectListItem> gllist = new List<SelectListItem>();
            gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && !IncludeGL.Contains(x.fld_GLCode) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
            gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            ViewBag.GLList = gllist;

            string newcostcentercode = "";

            if (CostCenter.StartsWith("P"))
            {
                newcostcentercode = CostCenter;
            }
            else
            {
                newcostcentercode = "0" + CostCenter;
            }

            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.COstCenterDesc = cc.GetCostCenterDesc(newcostcentercode);
            ViewBag.stationcode = stationcode;
            ViewBag.stationname = stationname;

            //Get uom
            var GetUom = db.bgt_Product.Where(x => x.YearBgt.ToString() == BudgetYear && x.Active == true).ToList();
            if (GetUom.Count() > 0)
            {
                var UomID = GetUom.FirstOrDefault().UOMID;
                var UOMList = db.bgt_UOM.Where(x => x.UOMID == UomID).ToList();
                if (UOMList.Count() > 0)
                {
                    ViewBag.UOM = UOMList.FirstOrDefault().UOM;
                }
            }

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Update(bgt_income_product[] model)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            foreach (var val in model)
        //            {
        //                var bip = new bgt_income_product
        //                {
        //                    abip_revision = 0,
        //                    abip_history = false,
        //                    abip_status = "DRAFT",
        //                    abip_budgeting_year = val.abip_budgeting_year,
        //                    abip_company_category = val.abip_company_category,
        //                    abip_company_list_sort = val.abip_company_list_sort,
        //                    abip_company_code = val.abip_company_code,
        //                    abip_company_name = val.abip_company_name,
        //                    abip_product_code = val.abip_product_code,
        //                    abip_product_name = val.abip_product_name,
        //                    //abip_station_code = val.abip_station_code,
        //                    //abip_station_name = val.abip_station_name,
        //                    abip_cost_center = val.abip_cost_center,
        //                    abip_cost_center_desc = val.abip_cost_center_desc,
        //                    abip_gl_code = val.abip_gl_code,
        //                    abip_gl_desc = val.abip_gl_desc,
        //                    abip_uom = val.abip_uom,
        //                    abip_amount_1 = val.abip_amount_1,
        //                    abip_amount_2 = val.abip_amount_2,
        //                    abip_amount_3 = val.abip_amount_3,
        //                    abip_amount_4 = val.abip_amount_4,
        //                    abip_amount_5 = val.abip_amount_5,
        //                    abip_amount_6 = val.abip_amount_6,
        //                    abip_amount_7 = val.abip_amount_7,
        //                    abip_amount_8 = val.abip_amount_8,
        //                    abip_amount_9 = val.abip_amount_9,
        //                    abip_amount_10 = val.abip_amount_10,
        //                    abip_amount_11 = val.abip_amount_11,
        //                    abip_amount_12 = val.abip_amount_12,
        //                    abip_qty_1 = val.abip_qty_1,
        //                    abip_qty_2 = val.abip_qty_2,
        //                    abip_qty_3 = val.abip_qty_3,
        //                    abip_qty_4 = val.abip_qty_4,
        //                    abip_qty_5 = val.abip_qty_5,
        //                    abip_qty_6 = val.abip_qty_6,
        //                    abip_qty_7 = val.abip_qty_7,
        //                    abip_qty_8 = val.abip_qty_8,
        //                    abip_qty_9 = val.abip_qty_9,
        //                    abip_qty_10 = val.abip_qty_10,
        //                    abip_qty_11 = val.abip_qty_11,
        //                    abip_qty_12 = val.abip_qty_12,
        //                    abip_unit_price = val.abip_unit_price,
        //                    abip_total_qty_a = val.abip_total_qty_a,
        //                    abip_total_qty_b = val.abip_total_qty_b,
        //                    abip_total_amount_a = val.abip_total_amount_a,
        //                    abip_total_amount_b = val.abip_total_amount_b,
        //                    abip_grand_total_quantity = val.abip_grand_total_quantity,
        //                    abip_grand_total_amount = val.abip_grand_total_amount,
        //                    last_modified = DateTime.Now,
        //                    fld_Deleted = false,
        //                    abip_syarikat_id = SyarikatID,
        //                    abip_syarikat_name = val.abip_syarikat_name,
        //                    abip_wilayah_id = WilayahID,
        //                    abip_wilayah_name = val.abip_wilayah_name,
        //                    abip_ladang_id = LadangID,
        //                    abip_ladang_name = val.abip_ladang_name,
        //                    abip_ladang_code = val.abip_ladang_code,
        //                    abip_created_by = val.abip_created_by
        //                };
        //                dbr.bgt_income_product.Add(bip);
        //            }
        //            dbr.SaveChangesAsync();

        //            string appname = Request.ApplicationPath;
        //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //            var lang = Request.RequestContext.RouteData.Values["lang"];

        //            if (appname != "/")
        //            {
        //                domain = domain + appname;
        //            }

        //            return Json(new
        //            {
        //                success = true,
        //                msg = GlobalResEstate.msgUpdate,
        //                status = "success",
        //                checkingdata = "0",
        //                method = "2",
        //                div = "produk",
        //                rooturl = domain,
        //                controller = "PendapatanProduk",
        //                action = "_Index",
        //                paramName1 = "BudgetYear",
        //                paramValue1 = model.FirstOrDefault().abip_budgeting_year.ToString(),
        //                paramName2 = "CostCenter",
        //                paramValue2 = model.FirstOrDefault().abip_cost_center
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new
        //            {
        //                success = false,
        //                msg = GlobalResEstate.msgError,
        //                status = "danger",
        //            });
        //        }
        //    }
        //    return View(model);
        //}

        public ActionResult _RecordsGL(string BudgetYear, string CostCenter, int page = 1)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;
            ViewBag.CostCenter = CostCenter;

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            int role = GetIdentity.RoleID(getuserid).Value;

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<IncomeProductDetailViewModel>
            {
                Content = GetRecordsDetails(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecordsDetails(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(records);
        }

        //new aug
        public ActionResult AddProduct(string BudgetYear, string CostCenter, string stationcode, string stationname)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
            if (!RoleScreen.add)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter) || string.IsNullOrEmpty(stationcode) || string.IsNullOrEmpty(stationname))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //List<SelectListItem> Product = new List<SelectListItem>();

            //var IncludeGLProduct = dbr.bgt_income_product
            //    .Where(w => w.abip_budgeting_year.ToString() == BudgetYear && w.abip_cost_center == CostCenter && w.abip_ladang_id == LadangID && w.abip_syarikat_id == SyarikatID && w.abip_wilayah_id == WilayahID && w.fld_Deleted == false)
            //    .ToList();

            var gl_list = new SelectList(GetGLs(int.Parse(BudgetYear), CostCenter).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem
            {
                Value = s.fld_GLCode.TrimStart(new char[] { '0' }),
                Text = s.fld_GLCode.TrimStart(new char[] { '0' }) + " - " + s.fld_GLDesc
            }), "Value", "Text").ToList();

            var product_list = new SelectList(GetProducts(int.Parse(BudgetYear), CostCenter).OrderBy(o => o.PrdCode).Select(s => new SelectListItem
            {
                Value = s.PrdId,
                Text = s.PrdCode + " - " + s.PrdName
            }), "Value", "Text").ToList();

            string newcostcentercode = "";

            if (CostCenter.StartsWith("P"))
            {
                newcostcentercode = CostCenter;
            }
            else
            {
                newcostcentercode = "0" + CostCenter;
            }

            //Get wilayah id/name by costcenter
            var CostCenterData = dbc.tbl_SAPCCPUP.Where(w => w.fld_CostCenter == newcostcentercode && w.fld_Deleted == false);
            var wilayahid = CostCenterData.FirstOrDefault().fld_WilayahID;

            var WilayahData = dbc.tbl_Wilayah.Where(w => w.fld_ID == wilayahid && w.fld_Deleted == false);
            var wilayahname = WilayahData.FirstOrDefault().fld_WlyhName;

            ViewBag.wilayahid = wilayahid;
            ViewBag.wilayahname = wilayahname;
            ViewBag.GLList = gl_list;
            ViewBag.ProductList = product_list;
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(newcostcentercode);
            ViewBag.stationcode = stationcode;
            ViewBag.stationname = stationname;
            ViewBag.SyarikatID = SyarikatID;

            var GetUom = db.bgt_Product.Where(x => x.YearBgt.ToString() == BudgetYear && x.Active == true).ToList();
            if (GetUom.Count() > 0)
            {
                var UomID = GetUom.FirstOrDefault().UOMID;
                var UOMList = db.bgt_UOM.Where(x => x.UOMID == UomID).ToList();
                if (UOMList.Count() > 0)
                {
                    ViewBag.UOM = UOMList.FirstOrDefault().UOM;
                }
            }

            return View();
        }

        //new aug
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(IncomeProductCreateEditViewModel[] model, string yr, string cc, string ccdesc, string stationcode, string stationname)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
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

            if (ModelState.IsValid)
            {
                try
                {
                    var resultAmount = 0m;
                    var syarikatObj = syarikat.GetSyarikat();
                    //var wilayahObj = wilayah.GetWilayah();
                    var ladangObj = ladang.GetLadang();

                    foreach (var val in model)
                    {
                        var bip = new bgt_income_product
                        {
                            abip_revision = 0,
                            abip_history = false,
                            abip_status = "DRAFT",
                            abip_budgeting_year = val.abip_budgeting_year,
                            abip_fld_ID = val.abip_fld_ID,
                            abip_company_category = val.abip_company_category,
                            abip_company_list_sort = val.abip_company_list_sort,
                            abip_company_code = val.abip_company_code,
                            abip_company_name = val.abip_company_name,
                            abip_product_id = val.abip_product_id,
                            abip_product_code = val.abip_product_code,
                            abip_product_name = val.abip_product_name,
                            abip_station_code = stationcode.ToString(),
                            abip_station_name = stationname.ToString(),
                            abip_cost_center = cc.ToString(),
                            abip_cost_center_desc = ccdesc.ToString(),
                            abip_gl_code = val.abip_gl_code,
                            abip_gl_desc = val.abip_gl_desc,
                            abip_uom = val.abip_uom,
                            abip_unit_price = !string.IsNullOrEmpty(val.abip_unit_price) && decimal.TryParse(val.abip_unit_price.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_unit_price.Replace(",", "")) : (decimal?)0.00,

                            abip_amount_1 = !string.IsNullOrEmpty(val.abip_amount_1) && decimal.TryParse(val.abip_amount_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_1.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_2 = !string.IsNullOrEmpty(val.abip_amount_2) && decimal.TryParse(val.abip_amount_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_2.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_3 = !string.IsNullOrEmpty(val.abip_amount_3) && decimal.TryParse(val.abip_amount_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_3.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_4 = !string.IsNullOrEmpty(val.abip_amount_4) && decimal.TryParse(val.abip_amount_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_4.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_5 = !string.IsNullOrEmpty(val.abip_amount_5) && decimal.TryParse(val.abip_amount_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_5.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_6 = !string.IsNullOrEmpty(val.abip_amount_6) && decimal.TryParse(val.abip_amount_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_6.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_7 = !string.IsNullOrEmpty(val.abip_amount_7) && decimal.TryParse(val.abip_amount_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_7.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_8 = !string.IsNullOrEmpty(val.abip_amount_8) && decimal.TryParse(val.abip_amount_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_8.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_9 = !string.IsNullOrEmpty(val.abip_amount_9) && decimal.TryParse(val.abip_amount_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_9.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_10 = !string.IsNullOrEmpty(val.abip_amount_10) && decimal.TryParse(val.abip_amount_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_10.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_11 = !string.IsNullOrEmpty(val.abip_amount_11) && decimal.TryParse(val.abip_amount_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_11.Replace(",", "")) : (decimal?)0.00,
                            abip_amount_12 = !string.IsNullOrEmpty(val.abip_amount_12) && decimal.TryParse(val.abip_amount_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_amount_12.Replace(",", "")) : (decimal?)0.00,
                            abip_total_amount_a = !string.IsNullOrEmpty(val.abip_total_amount_a) && decimal.TryParse(val.abip_total_amount_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_total_amount_a.Replace(",", "")) : (decimal?)0.00,
                            abip_total_amount_b = !string.IsNullOrEmpty(val.abip_total_amount_b) && decimal.TryParse(val.abip_total_amount_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_total_amount_b.Replace(",", "")) : (decimal?)0.00,
                            abip_grand_total_amount = !string.IsNullOrEmpty(val.abip_grand_total_amount) && decimal.TryParse(val.abip_grand_total_amount.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_grand_total_amount.Replace(",", "")) : (decimal?)0.00,

                            abip_qty_1 = !string.IsNullOrEmpty(val.abip_qty_1) && decimal.TryParse(val.abip_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_1.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_2 = !string.IsNullOrEmpty(val.abip_qty_2) && decimal.TryParse(val.abip_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_2.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_3 = !string.IsNullOrEmpty(val.abip_qty_3) && decimal.TryParse(val.abip_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_3.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_4 = !string.IsNullOrEmpty(val.abip_qty_4) && decimal.TryParse(val.abip_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_4.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_5 = !string.IsNullOrEmpty(val.abip_qty_5) && decimal.TryParse(val.abip_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_5.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_6 = !string.IsNullOrEmpty(val.abip_qty_6) && decimal.TryParse(val.abip_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_6.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_7 = !string.IsNullOrEmpty(val.abip_qty_7) && decimal.TryParse(val.abip_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_7.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_8 = !string.IsNullOrEmpty(val.abip_qty_8) && decimal.TryParse(val.abip_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_8.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_9 = !string.IsNullOrEmpty(val.abip_qty_9) && decimal.TryParse(val.abip_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_9.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_10 = !string.IsNullOrEmpty(val.abip_qty_10) && decimal.TryParse(val.abip_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_10.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_11 = !string.IsNullOrEmpty(val.abip_qty_11) && decimal.TryParse(val.abip_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_11.Replace(",", "")) : (decimal?)0.000,
                            abip_qty_12 = !string.IsNullOrEmpty(val.abip_qty_12) && decimal.TryParse(val.abip_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_qty_12.Replace(",", "")) : (decimal?)0.000,
                            abip_total_qty_a = !string.IsNullOrEmpty(val.abip_total_qty_a) && decimal.TryParse(val.abip_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_total_qty_a.Replace(",", "")) : (decimal?)0.000,
                            abip_total_qty_b = !string.IsNullOrEmpty(val.abip_total_qty_b) && decimal.TryParse(val.abip_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_total_qty_b.Replace(",", "")) : (decimal?)0.000,
                            abip_grand_total_quantity = !string.IsNullOrEmpty(val.abip_grand_total_quantity) && decimal.TryParse(val.abip_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abip_grand_total_quantity.Replace(",", "")) : (decimal?)0.000,

                            last_modified = DateTime.Now,
                            fld_Deleted = false,
                            abip_syarikat_id = SyarikatID,
                            abip_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                            //abip_wilayah_id = WilayahID,
                            //abip_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                            abip_wilayah_id = val.abip_wilayah_id,                                                          //new sept
                            abip_wilayah_name = !string.IsNullOrEmpty(val.abip_wilayah_name) ? val.abip_wilayah_name : null,//new sept
                            abip_ladang_id = LadangID,
                            abip_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                            abip_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                            abip_created_by = getidentity.ID(User.Identity.Name)
                        };
                        dbr.bgt_income_product.Add(bip);
                    }
                    dbr.SaveChanges();

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
                        method = "3",
                        div = "RecordsPartial",
                        rootUrl = domain,
                        action = "_RecordsGL",
                        controller = "PendapatanProduk",
                        paramName = "CostCenter",
                        paramValue = cc.ToString(),
                        paramName2 = "BudgetYear",
                        paramValue2 = yr.ToString(),
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
                db.Dispose();
                dbr.Dispose();
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        //public ActionResult AddDetails(string BudgetYear, string CostCenter, string stationcode, string stationname)
        //{
        //    if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter) || string.IsNullOrEmpty(stationcode) || string.IsNullOrEmpty(stationname))
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

        //    //Produk (nnt kompem semula dgn en shamon)
        //    //if login semenanjung, locID = 1,2
        //    //if sabah sarawak, LocID = 3,4,5,6

        //    List<SelectListItem> Product = new List<SelectListItem>();

        //    var IncludeGLProduct = dbr.bgt_income_product
        //        .Where(w => w.abip_budgeting_year.ToString() == BudgetYear && w.abip_cost_center == CostCenter && w.abip_ladang_id == LadangID && w.abip_syarikat_id == SyarikatID && w.abip_wilayah_id == WilayahID && w.fld_Deleted == false)
        //        .ToList();

        //    //check gl
        //    //var ID = GetBudgetClass.GetScreenID("I3");
        //    //var getAllID = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
        //    //List<Guid> GLID = new List<Guid>();
        //    //GLID = getAllID.Select(s => s.fld_GLID).ToList();
        //    //List<SelectListItem> gllist = new List<SelectListItem>();

        //    //new code aug
        //    var gl_list = new SelectList(GetGLs(int.Parse(BudgetYear), CostCenter).OrderBy(o => o.Code).Select(s => new SelectListItem
        //    {
        //        Value = s.Code.TrimStart(new char[] { '0' }),
        //        Text = s.Code.TrimStart(new char[] { '0' }) + " - " + s.Description
        //    }), "Value", "Text").ToList();

        //    var product_list = new SelectList(GetProducts(int.Parse(BudgetYear)).OrderBy(o => o.PrdCode).Select(s => new SelectListItem
        //    {
        //        Value = s.PrdCode,
        //        Text = s.PrdCode + " - " + s.PrdName
        //    }), "Value", "Text").ToList();

        //    ViewBag.GLList = gl_list;
        //    ViewBag.ProductList = product_list;
        //    ViewBag.BudgetYear = BudgetYear;
        //    ViewBag.CostCenter = CostCenter;
        //    ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
        //    ViewBag.stationcode = stationcode;
        //    ViewBag.stationname = stationname;
        //    ViewBag.SyarikatID = SyarikatID;


        //    //old code
        //    //if (IncludeGLProduct.Count > 0)
        //    //{
        //    //    var includeglcode = "";
        //    //    var includeprdcode = "";
        //    //    includeglcode = IncludeGLProduct.FirstOrDefault().abip_gl_code;
        //    //    includeprdcode = IncludeGLProduct.FirstOrDefault().abip_product_code;

        //    //    if (WilayahID == 1)
        //    //    {
        //    //        if (string.IsNullOrEmpty(includeprdcode) && string.IsNullOrEmpty(includeglcode))//all null
        //    //        {
        //    //            //select product
        //    //            Product = new SelectList(db.bgt_Product
        //    //                .Where(x => x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                .OrderBy(o => o.PrdCode)
        //    //                .Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                .ToList();
        //    //            Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //            ViewBag.ProductList = Product;
        //    //            //select gl
        //    //            gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //            gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //            ViewBag.GLList = gllist;
        //    //        }
        //    //        else if (!string.IsNullOrEmpty(includeprdcode) && string.IsNullOrEmpty(includeglcode))//prd =1, gl =0
        //    //        {
        //    //            //cros cek
        //    //            var prdvalue = IncludeGLProduct.Where(w => w.abip_product_code.Contains(includeprdcode)).ToList();
        //    //            if (prdvalue.Count > 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode)
        //    //                    .Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //        }
        //    //        else if (string.IsNullOrEmpty(includeprdcode) && !string.IsNullOrEmpty(includeglcode))//prd =0, gl =1
        //    //        {
        //    //            //cros cek
        //    //            var glvalue = IncludeGLProduct.Where(w => w.abip_gl_code.Contains(includeglcode)).ToList();
        //    //            if (glvalue.Count > 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //        }
        //    //        else if (!string.IsNullOrEmpty(includeprdcode) && !string.IsNullOrEmpty(includeglcode))//prd =1, gl =1
        //    //        {
        //    //            var prdvalue = IncludeGLProduct.Where(w => w.abip_product_code.Contains(includeprdcode)).ToList();
        //    //            var glvalue = IncludeGLProduct.Where(w => w.abip_gl_code.Contains(includeglcode)).ToList();
        //    //            if (prdvalue.Count > 0 && glvalue.Count == 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //            else if (prdvalue.Count == 0 && glvalue.Count > 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //            else if (prdvalue.Count > 0 && glvalue.Count > 0)
        //    //            {
        //    //                //new cros cek
        //    //                var NewIncludeGLProduct = IncludeGLProduct.Where(w => w.abip_product_code.Contains(prdvalue.FirstOrDefault().abip_product_code) && w.abip_gl_code.Contains(glvalue.FirstOrDefault().abip_gl_code)).ToList();
        //    //                var newgl = NewIncludeGLProduct.FirstOrDefault().abip_gl_code;
        //    //                var newprdk = NewIncludeGLProduct.FirstOrDefault().abip_product_code;

        //    //                if (NewIncludeGLProduct.Count > 0)
        //    //                {
        //    //                    //select product
        //    //                    Product = new SelectList(db.bgt_Product
        //    //                        .Where(x => !newprdk.Contains(x.PrdCode) && x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                        .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                        .ToList();
        //    //                    Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                    ViewBag.ProductList = Product;
        //    //                    //select gl
        //    //                    gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => /*!newgl.Contains(x.fld_GLCode) &&*/ GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                    gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                    ViewBag.GLList = gllist;
        //    //                }
        //    //                else
        //    //                {
        //    //                    //select product
        //    //                    Product = new SelectList(db.bgt_Product
        //    //                        .Where(x => x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                        .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                        .ToList();
        //    //                    Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                    ViewBag.ProductList = Product;
        //    //                    //select gl
        //    //                    gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                    gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                    ViewBag.GLList = gllist;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //    else //wilayah sbh/srwk
        //    //    {
        //    //        if (string.IsNullOrEmpty(includeprdcode) && string.IsNullOrEmpty(includeglcode))//all null
        //    //        {
        //    //            //select product
        //    //            Product = new SelectList(db.bgt_Product
        //    //                .Where(x => x.Lowest == true && x.Active == true && (x.LocID != 1 && x.LocID != 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                .ToList();
        //    //            Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //            ViewBag.ProductList = Product;
        //    //            //select gl
        //    //            gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //            gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //            ViewBag.GLList = gllist;
        //    //        }
        //    //        else if (!string.IsNullOrEmpty(includeprdcode) && string.IsNullOrEmpty(includeglcode))//prd =1, gl =0
        //    //        {
        //    //            //cros cek
        //    //            var prdvalue = IncludeGLProduct.Where(w => w.abip_product_code.Contains(includeprdcode)).ToList();
        //    //            if (prdvalue.Count > 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID != 1 && x.LocID != 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //        }
        //    //        else if (string.IsNullOrEmpty(includeprdcode) && !string.IsNullOrEmpty(includeglcode))//prd =0, gl =1
        //    //        {
        //    //            //cros cek
        //    //            var glvalue = IncludeGLProduct.Where(w => w.abip_gl_code.Contains(includeglcode)).ToList();
        //    //            if (glvalue.Count > 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID != 1 && x.LocID != 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //        }
        //    //        else if (!string.IsNullOrEmpty(includeprdcode) && !string.IsNullOrEmpty(includeglcode))//prd =1, gl =1
        //    //        {
        //    //            var prdvalue = IncludeGLProduct.Where(w => w.abip_product_code.Contains(includeprdcode)).ToList();
        //    //            var glvalue = IncludeGLProduct.Where(w => w.abip_gl_code.Contains(includeglcode)).ToList();
        //    //            if (prdvalue.Count > 0 && glvalue.Count == 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID != 1 && x.LocID != 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //            else if (prdvalue.Count == 0 && glvalue.Count > 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => x.Lowest == true && x.Active == true && (x.LocID != 1 && x.LocID != 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //            else if (prdvalue.Count > 0 && glvalue.Count > 0)
        //    //            {
        //    //                //select product
        //    //                Product = new SelectList(db.bgt_Product
        //    //                    .Where(x => !includeprdcode.Contains(x.PrdCode) && x.Lowest == true && x.Active == true && (x.LocID != 1 || x.LocID != 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //                    .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //                    .ToList();
        //    //                Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.ProductList = Product;
        //    //                //select gl
        //    //                gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => !includeglcode.Contains(x.fld_GLCode) && GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //                gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //                ViewBag.GLList = gllist;
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    if (WilayahID == 1)
        //    //    {
        //    //        List<SelectListItem> newproduct = new List<SelectListItem>();
        //    //        newproduct = new SelectList(db.bgt_Product
        //    //            .Where(x => x.Lowest == true && x.Active == true && (x.LocID == 1 || x.LocID == 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //            .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //            .ToList();
        //    //        newproduct.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //        ViewBag.ProductList = newproduct;

        //    //        gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //        gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //        ViewBag.GLList = gllist;
        //    //    }
        //    //    else
        //    //    {
        //    //        Product = new SelectList(db.bgt_Product
        //    //            .Where(x => x.Lowest == true && x.Active == true && (x.LocID != 1 || x.LocID != 2) && x.PrdCode != "1701" && x.PrdCode != "1801")
        //    //            .OrderBy(o => o.PrdCode).Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName }), "Value", "Text")
        //    //            .ToList();
        //    //        Product.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //        ViewBag.ProductList = Product;

        //    //        gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode + " - " + s.fld_GLDesc }), "Value", "Text").ToList();
        //    //        gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
        //    //        ViewBag.GLList = gllist;
        //    //    }
        //    //}

        //    //ViewBag.BudgetYear = BudgetYear;
        //    //ViewBag.CostCenter = CostCenter;
        //    //ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
        //    //ViewBag.stationcode = stationcode;
        //    //ViewBag.stationname = stationname;
        //    //ViewBag.SyarikatID = SyarikatID;

        //    //Get uom
        //    var GetUom = db.bgt_Product.Where(x => x.YearBgt.ToString() == BudgetYear && x.Active == true).ToList();
        //    if (GetUom.Count() > 0)
        //    {
        //        var UomID = GetUom.FirstOrDefault().UOMID;
        //        var UOMList = db.bgt_UOM.Where(x => x.UOMID == UomID).ToList();
        //        if (UOMList.Count() > 0)
        //        {
        //            ViewBag.UOM = UOMList.FirstOrDefault().UOM;
        //        }
        //    }

        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddDetails(bgt_income_product[] model, string yr, string cc, string stationcode, string stationname)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var syarikatObj = syarikat.GetSyarikat();
        //            var wilayahObj = wilayah.GetWilayah();
        //            var ladangObj = ladang.GetLadang();

        //            foreach (var val in model)
        //            {
        //                var bip = new bgt_income_product
        //                {
        //                    abip_revision = 0,
        //                    abip_history = false,
        //                    abip_status = "DRAFT",
        //                    abip_budgeting_year = val.abip_budgeting_year,
        //                    abip_company_category = val.abip_company_category,
        //                    abip_company_list_sort = val.abip_company_list_sort,
        //                    abip_company_code = val.abip_company_code,
        //                    abip_company_name = val.abip_company_name,
        //                    abip_product_code = val.abip_product_code,
        //                    abip_product_name = val.abip_product_name,
        //                    abip_station_code = stationcode.ToString(),
        //                    abip_station_name = stationname.ToString(),
        //                    abip_cost_center = val.abip_cost_center,
        //                    abip_cost_center_desc = val.abip_cost_center_desc,
        //                    abip_gl_code = val.abip_gl_code,
        //                    abip_gl_desc = val.abip_gl_desc,
        //                    abip_uom = val.abip_uom,
        //                    abip_amount_1 = val.abip_amount_1.HasValue ? val.abip_amount_1.Value : 0m,
        //                    abip_amount_2 = val.abip_amount_2.HasValue ? val.abip_amount_2.Value : 0m,
        //                    abip_amount_3 = val.abip_amount_3.HasValue ? val.abip_amount_3.Value : 0m,
        //                    abip_amount_4 = val.abip_amount_4.HasValue ? val.abip_amount_4.Value : 0m,
        //                    abip_amount_5 = val.abip_amount_5.HasValue ? val.abip_amount_5.Value : 0m,
        //                    abip_amount_6 = val.abip_amount_6.HasValue ? val.abip_amount_6.Value : 0m,
        //                    abip_amount_7 = val.abip_amount_7.HasValue ? val.abip_amount_7.Value : 0m,
        //                    abip_amount_8 = val.abip_amount_8.HasValue ? val.abip_amount_8.Value : 0m,
        //                    abip_amount_9 = val.abip_amount_9.HasValue ? val.abip_amount_9.Value : 0m,
        //                    abip_amount_10 = val.abip_amount_10.HasValue ? val.abip_amount_10.Value : 0m,
        //                    abip_amount_11 = val.abip_amount_11.HasValue ? val.abip_amount_11.Value : 0m,
        //                    abip_amount_12 = val.abip_amount_12.HasValue ? val.abip_amount_12.Value : 0m,
        //                    abip_qty_1 = val.abip_qty_1.HasValue ? val.abip_qty_1.Value : 0m,
        //                    abip_qty_2 = val.abip_qty_2.HasValue ? val.abip_qty_2.Value : 0m,
        //                    abip_qty_3 = val.abip_qty_3.HasValue ? val.abip_qty_3.Value : 0m,
        //                    abip_qty_4 = val.abip_qty_4.HasValue ? val.abip_qty_4.Value : 0m,
        //                    abip_qty_5 = val.abip_qty_5.HasValue ? val.abip_qty_5.Value : 0m,
        //                    abip_qty_6 = val.abip_qty_6.HasValue ? val.abip_qty_6.Value : 0m,
        //                    abip_qty_7 = val.abip_qty_7.HasValue ? val.abip_qty_7.Value : 0m,
        //                    abip_qty_8 = val.abip_qty_8.HasValue ? val.abip_qty_8.Value : 0m,
        //                    abip_qty_9 = val.abip_qty_9.HasValue ? val.abip_qty_9.Value : 0m,
        //                    abip_qty_10 = val.abip_qty_10.HasValue ? val.abip_qty_10.Value : 0m,
        //                    abip_qty_11 = val.abip_qty_11.HasValue ? val.abip_qty_11.Value : 0m,
        //                    abip_qty_12 = val.abip_qty_12.HasValue ? val.abip_qty_12.Value : 0m,
        //                    abip_unit_price = val.abip_unit_price.HasValue ? val.abip_unit_price.Value : 0m,
        //                    abip_total_qty_a = val.abip_total_qty_a.HasValue ? val.abip_total_qty_a.Value : 0m,
        //                    abip_total_qty_b = val.abip_total_qty_b.HasValue ? val.abip_total_qty_b.Value : 0m,
        //                    abip_total_amount_a = val.abip_total_amount_a.HasValue ? val.abip_total_amount_a.Value : 0m,
        //                    abip_total_amount_b = val.abip_total_amount_b.HasValue ? val.abip_total_amount_b.Value : 0m,
        //                    abip_grand_total_quantity = val.abip_grand_total_quantity.HasValue ? val.abip_grand_total_quantity.Value : 0m,
        //                    abip_grand_total_amount = val.abip_grand_total_amount.HasValue ? val.abip_grand_total_amount.Value : 0m,
        //                    last_modified = DateTime.Now,
        //                    fld_Deleted = false,
        //                    abip_syarikat_id = SyarikatID,
        //                    abip_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
        //                    abip_wilayah_id = WilayahID,
        //                    abip_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
        //                    abip_ladang_id = LadangID,
        //                    abip_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
        //                    abip_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
        //                    abip_created_by = getidentity.ID(User.Identity.Name)
        //                };
        //                dbr.bgt_income_product.Add(bip);
        //            }
        //            dbr.SaveChanges();

        //            string appname = Request.ApplicationPath;
        //            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
        //            var lang = Request.RequestContext.RouteData.Values["lang"];

        //            if (appname != "/")
        //            {
        //                domain = domain + appname;
        //            }

        //            return Json(new
        //            {
        //                success = true,
        //                msg = GlobalResEstate.msgAdd,
        //                status = "success",
        //                checkingdata = "0",
        //                method = "3",
        //                div = "RecordsPartial",
        //                rootUrl = domain,
        //                action = "_RecordsGL",
        //                controller = "PendapatanProduk",
        //                paramName = "CostCenter",
        //                paramValue = model.FirstOrDefault().abip_cost_center,
        //                paramName2 = "BudgetYear",
        //                paramValue2 = model.FirstOrDefault().abip_budgeting_year,
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new
        //            {
        //                success = false,
        //                msg = GlobalResEstate.msgError,
        //                status = "danger",
        //            });
        //        }
        //    }
        //    else
        //    {
        //        db.Dispose();
        //        dbr.Dispose();
        //        return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult View(string costcenter, int budgetyear/*, string prdcode*/)
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

        //    if (budgetyear >= 2024)
        //    {
        //        bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == costcenter && x.abip_budgeting_year == budgetyear && /*x.abip_product_code == prdcode &&*/ x.fld_Deleted == false).ToArray();

        //        //Get Syarikat
        //        ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //        ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //        ViewBag.Message = @GlobalResEstate.msgNoRecord;

        //        //Get Budget Year
        //        ViewBag.BudgetYear = bgt_income_product.Select(s => s.abip_budgeting_year).FirstOrDefault();

        //        //Get GL Pendapatan
        //        ViewBag.fld_GLCode = bgt_income_product.Select(s => s.abip_gl_code).FirstOrDefault();
        //        ViewBag.fld_GLDesc = bgt_income_product.Select(s => s.abip_gl_desc).FirstOrDefault();

        //        //Get Produk
        //        ViewBag.ProductCode = bgt_income_product.Select(s => s.abip_product_code).FirstOrDefault();
        //        ViewBag.ProductName = bgt_income_product.Select(s => s.abip_product_name).FirstOrDefault();

        //        //M.s
        //        int totalpage = 0;
        //        int page = 0;
        //        int pagesize = 0;
        //        int totalldata = 0;
        //        var bgtincomedata = bgt_income_product.GroupBy(g => new { g.abip_cost_center, g.abip_budgeting_year }).ToList();

        //        totalldata = bgtincomedata.Count();
        //        pagesize = 12;
        //        totalpage = totalldata / pagesize;
        //        page = totalldata / pagesize;

        //        if (totalpage == 0)
        //        {
        //            int newtotalpage = totalpage + 1;
        //            ViewBag.newtotalpage = newtotalpage;
        //        }
        //        else
        //        {
        //            int newtotalpage = totalpage + 1;
        //            ViewBag.newtotalpage = newtotalpage;
        //        }
        //        if (page == 0)
        //        {
        //            int curretpage = 1;
        //            ViewBag.curretpage = curretpage;
        //        }
        //        else
        //        {
        //            int curretpage = 1;
        //            ViewBag.curretpage = curretpage;
        //        }

        //        return PartialView(bgt_income_product);
        //    }
        //    else
        //    {
        //        bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == costcenter && x.abip_budgeting_year == budgetyear /*&& x.abip_product_code == prdcode*/).ToArray();

        //        ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //        ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //        ViewBag.Message = @GlobalResEstate.msgNoRecord;
        //        ViewBag.BudgetYear = bgt_income_product.Select(s => s.abip_budgeting_year).FirstOrDefault();
        //        ViewBag.fld_GLCode = bgt_income_product.Select(s => s.abip_gl_code).FirstOrDefault();
        //        ViewBag.fld_GLDesc = bgt_income_product.Select(s => s.abip_gl_desc).FirstOrDefault();
        //        ViewBag.ProductCode = bgt_income_product.Select(s => s.abip_product_code).FirstOrDefault();
        //        ViewBag.ProductName = bgt_income_product.Select(s => s.abip_product_name).FirstOrDefault();

        //        int totalpage = 0;
        //        int page = 0;
        //        int pagesize = 0;
        //        int totalldata = 0;
        //        var bgtincomedata = bgt_income_product.GroupBy(g => new { g.abip_cost_center, g.abip_budgeting_year }).ToList();

        //        totalldata = bgtincomedata.Count();
        //        pagesize = 12;
        //        totalpage = totalldata / pagesize;
        //        page = totalldata / pagesize;

        //        if (totalpage == 0)
        //        {
        //            int newtotalpage = totalpage + 1;
        //            ViewBag.newtotalpage = newtotalpage;
        //        }
        //        else
        //        {
        //            int newtotalpage = totalpage + 1;
        //            ViewBag.newtotalpage = newtotalpage;
        //        }
        //        if (page == 0)
        //        {
        //            int curretpage = 1;
        //            ViewBag.curretpage = curretpage;
        //        }
        //        else
        //        {
        //            int curretpage = 1;
        //            ViewBag.curretpage = curretpage;
        //        }

        //        return PartialView(bgt_income_product);
        //    }
        //}

        public ActionResult Views(string costcenter, int budgetyear/*, string prdcode*/)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            if (budgetyear >= 2024)
            {
                bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == costcenter && x.abip_budgeting_year == budgetyear && /*x.abip_product_code == prdcode &&*/ x.fld_Deleted == false).ToArray();

                //Get Syarikat
                ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                //Get Budget Year
                ViewBag.BudgetYear = bgt_income_product.Select(s => s.abip_budgeting_year).FirstOrDefault();

                //Get GL Pendapatan
                ViewBag.fld_GLCode = bgt_income_product.Select(s => s.abip_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_income_product.Select(s => s.abip_gl_desc).FirstOrDefault();

                //Get Produk
                ViewBag.ProductCode = bgt_income_product.Select(s => s.abip_product_code).FirstOrDefault();
                ViewBag.ProductName = bgt_income_product.Select(s => s.abip_product_name).FirstOrDefault();

                //M.s
                int totalpage = 0;
                int page = 0;
                int pagesize = 0;
                int totalldata = 0;
                var bgtincomedata = bgt_income_product.GroupBy(g => new { g.abip_cost_center, g.abip_budgeting_year }).ToList();

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

                return PartialView(bgt_income_product);
            }
            else
            {
                bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == costcenter && x.abip_budgeting_year == budgetyear /*&& x.abip_product_code == prdcode*/).ToArray();

                ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;
                ViewBag.BudgetYear = bgt_income_product.Select(s => s.abip_budgeting_year).FirstOrDefault();
                ViewBag.fld_GLCode = bgt_income_product.Select(s => s.abip_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_income_product.Select(s => s.abip_gl_desc).FirstOrDefault();
                ViewBag.ProductCode = bgt_income_product.Select(s => s.abip_product_code).FirstOrDefault();
                ViewBag.ProductName = bgt_income_product.Select(s => s.abip_product_name).FirstOrDefault();

                int totalpage = 0;
                int page = 0;
                int pagesize = 0;
                int totalldata = 0;
                var bgtincomedata = bgt_income_product.GroupBy(g => new { g.abip_cost_center, g.abip_budgeting_year }).ToList();

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

                return PartialView(bgt_income_product);
            }
        }

        public ActionResult UpdateRecordsGL(string BudgetYear, string CostCenter, /*string gl,*/ string prdcode)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
            if (!RoleScreen.edit)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter) /*|| string.IsNullOrEmpty(gl)*/)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == CostCenter && x.abip_budgeting_year.ToString() == BudgetYear /*&& x.abip_gl_code == gl.ToString()*/ && x.abip_product_code == prdcode && x.abip_syarikat_id == SyarikatID && x.abip_wilayah_id == WilayahID && x.abip_ladang_id == LadangID && x.fld_Deleted == false).ToArray();
            bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == CostCenter && x.abip_budgeting_year.ToString() == BudgetYear && x.abip_product_code == prdcode && x.abip_syarikat_id == SyarikatID && x.abip_ladang_id == LadangID && x.fld_Deleted == false).ToArray();

            var ID = GetBudgetClass.GetScreenID("I3");
            var getAllID = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            List<Guid> GLID = new List<Guid>();
            GLID = getAllID.Select(s => s.fld_GLID).ToList();

            //new code
            var glcodeA = "00" + bgt_income_product.Where(w => w.abip_company_code == "1").FirstOrDefault().abip_gl_code;
            var glcodeB = "00" + bgt_income_product.Where(w => w.abip_company_code == "2").LastOrDefault().abip_gl_code;
            ViewBag.GLListA = new SelectList(GetGLs(bgt_income_product.FirstOrDefault().abip_budgeting_year, bgt_income_product.FirstOrDefault().abip_cost_center).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode.TrimStart(new char[] { '0' }) + " - " + s.fld_GLDesc }), "Value", "Text", glcodeA);
            ViewBag.GLListB = new SelectList(GetGLs(bgt_income_product.FirstOrDefault().abip_budgeting_year, bgt_income_product.FirstOrDefault().abip_cost_center).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLCode.TrimStart(new char[] { '0' }) + " - " + s.fld_GLDesc }), "Value", "Text", glcodeB);

            ViewBag.ProductList = new SelectList(GetProducts((int)bgt_income_product.FirstOrDefault().abip_budgeting_year, CostCenter).Select( s=> new SelectListItem 
            {
                Value = s.PrdId,
                Text = s.PrdCode + " - " + s.PrdName
            }), "Value", "Text", bgt_income_product.FirstOrDefault().abip_product_id);

            var GetUom = db.bgt_Product.Where(x => x.YearBgt.ToString() == BudgetYear && x.Active == true).ToList();
            if (GetUom.Count() > 0)
            {
                var UomID = GetUom.FirstOrDefault().UOMID;
                var UOMList = db.bgt_UOM.Where(x => x.UOMID == UomID).ToList();
                if (UOMList.Count() > 0)
                {
                    ViewBag.UOM = UOMList.FirstOrDefault().UOM;
                }
            }
            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.COstCenterDesc = cc.GetCostCenterDesc(CostCenter);
            return View(bgt_income_product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRecordsGL(IncomeProductCreateEditViewModel[] model)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
            if (!RoleScreen.edit)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            int Year = DateTime.Now.Year + 1;
            var resultAmount = 0m;

            try
            {
                var syarikatObj = syarikat.GetSyarikat();
                var wilayahObj = wilayah.GetWilayah();
                var ladangObj = ladang.GetLadang();

                foreach (var bip in model)
                {
                    var getdata = dbr.bgt_income_product.Where(w => w.abip_id == bip.abip_id &&
                                                                    w.abip_cost_center == bip.abip_cost_center &&
                                                                    w.abip_budgeting_year == bip.abip_budgeting_year &&
                                                                    w.abip_fld_ID == bip.abip_fld_ID &&
                                                                    w.abip_syarikat_id == SyarikatID && w.abip_wilayah_id == bip.abip_wilayah_id && w.abip_ladang_id == LadangID &&
                                                                    w.fld_Deleted == false).FirstOrDefault();
                    getdata.abip_fld_ID = bip.abip_fld_ID;
                    getdata.abip_gl_code = bip.abip_gl_code;
                    getdata.abip_gl_desc = bip.abip_gl_desc;
                    getdata.abip_product_id = bip.abip_product_id;//
                    getdata.abip_product_code = bip.abip_product_code;
                    getdata.abip_product_name = bip.abip_product_name;

                    getdata.abip_amount_1 = !string.IsNullOrEmpty(bip.abip_amount_1) && decimal.TryParse(bip.abip_amount_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_1.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_2 = !string.IsNullOrEmpty(bip.abip_amount_2) && decimal.TryParse(bip.abip_amount_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_2.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_3 = !string.IsNullOrEmpty(bip.abip_amount_3) && decimal.TryParse(bip.abip_amount_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_3.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_4 = !string.IsNullOrEmpty(bip.abip_amount_4) && decimal.TryParse(bip.abip_amount_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_4.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_5 = !string.IsNullOrEmpty(bip.abip_amount_5) && decimal.TryParse(bip.abip_amount_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_5.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_6 = !string.IsNullOrEmpty(bip.abip_amount_6) && decimal.TryParse(bip.abip_amount_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_6.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_7 = !string.IsNullOrEmpty(bip.abip_amount_7) && decimal.TryParse(bip.abip_amount_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_7.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_8 = !string.IsNullOrEmpty(bip.abip_amount_8) && decimal.TryParse(bip.abip_amount_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_8.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_9 = !string.IsNullOrEmpty(bip.abip_amount_9) && decimal.TryParse(bip.abip_amount_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_9.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_10 = !string.IsNullOrEmpty(bip.abip_amount_10) && decimal.TryParse(bip.abip_amount_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_10.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_11 = !string.IsNullOrEmpty(bip.abip_amount_11) && decimal.TryParse(bip.abip_amount_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_11.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_amount_12 = !string.IsNullOrEmpty(bip.abip_amount_12) && decimal.TryParse(bip.abip_amount_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_amount_12.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_total_amount_a = !string.IsNullOrEmpty(bip.abip_total_amount_a) && decimal.TryParse(bip.abip_total_amount_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_total_amount_a.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_total_amount_b = !string.IsNullOrEmpty(bip.abip_total_amount_b) && decimal.TryParse(bip.abip_total_amount_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_total_amount_b.Replace(",", "")) : (decimal?)0.00;
                    getdata.abip_grand_total_amount = !string.IsNullOrEmpty(bip.abip_grand_total_amount) && decimal.TryParse(bip.abip_grand_total_amount.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_grand_total_amount.Replace(",", "")) : (decimal?)0.00;

                    getdata.abip_qty_1 = !string.IsNullOrEmpty(bip.abip_qty_1) && decimal.TryParse(bip.abip_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_1.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_2 = !string.IsNullOrEmpty(bip.abip_qty_2) && decimal.TryParse(bip.abip_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_2.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_3 = !string.IsNullOrEmpty(bip.abip_qty_3) && decimal.TryParse(bip.abip_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_3.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_4 = !string.IsNullOrEmpty(bip.abip_qty_4) && decimal.TryParse(bip.abip_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_4.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_5 = !string.IsNullOrEmpty(bip.abip_qty_5) && decimal.TryParse(bip.abip_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_5.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_6 = !string.IsNullOrEmpty(bip.abip_qty_6) && decimal.TryParse(bip.abip_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_6.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_7 = !string.IsNullOrEmpty(bip.abip_qty_7) && decimal.TryParse(bip.abip_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_7.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_8 = !string.IsNullOrEmpty(bip.abip_qty_8) && decimal.TryParse(bip.abip_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_8.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_9 = !string.IsNullOrEmpty(bip.abip_qty_9) && decimal.TryParse(bip.abip_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_9.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_10 = !string.IsNullOrEmpty(bip.abip_qty_10) && decimal.TryParse(bip.abip_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_10.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_11 = !string.IsNullOrEmpty(bip.abip_qty_11) && decimal.TryParse(bip.abip_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_11.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_qty_12 = !string.IsNullOrEmpty(bip.abip_qty_12) && decimal.TryParse(bip.abip_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_qty_12.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_total_qty_a = !string.IsNullOrEmpty(bip.abip_total_qty_a) && decimal.TryParse(bip.abip_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_total_qty_a.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_total_qty_b = !string.IsNullOrEmpty(bip.abip_total_qty_b) && decimal.TryParse(bip.abip_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_total_qty_b.Replace(",", "")) : (decimal?)0.000;
                    getdata.abip_grand_total_quantity = !string.IsNullOrEmpty(bip.abip_grand_total_quantity) && decimal.TryParse(bip.abip_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_grand_total_quantity.Replace(",", "")) : (decimal?)0.000;

                    getdata.abip_unit_price = !string.IsNullOrEmpty(bip.abip_unit_price) && decimal.TryParse(bip.abip_unit_price.Replace(",", ""), out resultAmount) ? decimal.Parse(bip.abip_unit_price.Replace(",", "")) : (decimal?)0.00;

                    getdata.abip_syarikat_id = SyarikatID;
                    getdata.abip_wilayah_id = bip.abip_wilayah_id;//edit sept
                    getdata.abip_ladang_id = LadangID;
                    getdata.abip_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                    getdata.abip_wilayah_name = !string.IsNullOrEmpty(bip.abip_wilayah_name) ? bip.abip_wilayah_name : null;//edit sept
                    getdata.abip_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                    getdata.abip_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                    getdata.abip_created_by = getidentity.ID(User.Identity.Name);
                    getdata.last_modified = DateTime.Now;
                    dbr.Entry(getdata).State = EntityState.Modified;
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
                    div = "RecordsPartial",
                    rootUrl = domain,
                    action = "_RecordsGL",
                    controller = "PendapatanProduk",
                    paramName = "CostCenter",
                    paramValue = model.FirstOrDefault().abip_cost_center,
                    paramName2 = "BudgetYear",
                    paramValue2 = model.FirstOrDefault().abip_budgeting_year,
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult Delete(string costcenter, string budgetyear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
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

            bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == costcenter && x.abip_budgeting_year.ToString() == budgetyear && x.fld_Deleted == false).ToArray();

            return PartialView(bgt_income_product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(bgt_income_product[] deletebgtincomeproduct)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
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
                foreach (var bip in deletebgtincomeproduct)
                {
                    var getdata = dbr.bgt_income_product.Where(w => w.abip_id == bip.abip_id && w.abip_cost_center == bip.abip_cost_center && w.abip_budgeting_year == bip.abip_budgeting_year && w.fld_Deleted == false).FirstOrDefault();

                    if (getdata == null)
                    {
                        return Json(new { success = true, msg = GlobalResEstate.msgNoRecord, status = "alert", checkingdata = "0", method = "2", btn = "" });
                    }
                    else
                    {
                        //getdata.fld_Deleted = true;
                        //dbr.Entry(getdata).State = EntityState.Modified;
                        dbr.bgt_income_product.Remove(getdata);
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
                    div = "produk",
                    rootUrl = domain,
                    action = "_Index",
                    controller = "PendapatanProduk",
                    paramName = "BudgetYear",
                    paramValue = deletebgtincomeproduct.FirstOrDefault().abip_budgeting_year
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult DeleteRecordsGL(string costcenter, string budgetyear, /*string gl,*/ string prdcode)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
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

            bgt_income_product[] bgt_income_product = dbr.bgt_income_product.Where(x => x.abip_cost_center == costcenter && x.abip_budgeting_year.ToString() == budgetyear && /*x.abip_gl_code == gl &&*/ x.abip_product_code == prdcode && x.fld_Deleted == false).ToArray();

            return PartialView(bgt_income_product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecordsGL(bgt_income_product[] deleterecordsgl)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
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
                foreach (var bip in deleterecordsgl)
                {
                    var datas = dbr.bgt_income_product.Where(w => w.abip_id == bip.abip_id && w.abip_cost_center == bip.abip_cost_center && w.abip_budgeting_year == bip.abip_budgeting_year && /*w.abip_gl_code == bip.abip_gl_code &&*/ w.abip_product_id == bip.abip_product_id && w.fld_Deleted == false).FirstOrDefault();

                    if (datas == null)
                    {
                        return Json(new { success = true, msg = GlobalResEstate.msgNoRecord, status = "alert", checkingdata = "0", method = "2", btn = "" });
                    }
                    else
                    {
                        //datas.fld_Deleted = true;
                        //datas.last_modified = DateTime.Now;
                        //dbr.Entry(datas).State = EntityState.Modified;
                        dbr.bgt_income_product.Remove(datas);
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
                    method = "3",
                    div = "RecordsPartial",
                    rootUrl = domain,
                    action = "_RecordsGL",
                    controller = "PendapatanProduk",
                    paramName = "BudgetYear",
                    paramValue = deleterecordsgl.FirstOrDefault().abip_budgeting_year.ToString(),
                    paramName2 = "CostCenter",
                    paramValue2 = deleterecordsgl.FirstOrDefault().abip_cost_center
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult PrintList(string BudgetYear, string CostCenter, string Stesen, string format = "PDF")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomePrdctList.rdlc");
            string filename = GetBudgetClass.GetScreenName("I3") + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeProductListViewModel> data = dbe.Database.SqlQuery<IncomeProductListViewModel>("exec sp_BudgetIncomeProductList {0}, {1}", BudgetYear, CostCenter, Stesen).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintViewPdf(string BudgetYear, string CostCenter, string format = "PDF")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomePrdctPdf.rdlc");
            string filename = GetBudgetClass.GetScreenName("I3") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeProductDetailViewModel> data = dbe.Database.SqlQuery<IncomeProductDetailViewModel>("exec sp_BudgetIncomeProductView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename, true);//new oct
        }
        public ActionResult PrintViewExcel(string BudgetYear, string CostCenter, string format = "EXCEL")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I3");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomePrdctExcel.rdlc");
            string filename = GetBudgetClass.GetScreenName("I3") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeProductDetailViewModel> data = dbe.Database.SqlQuery<IncomeProductDetailViewModel>("exec sp_BudgetIncomeProductView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename, true);//new oct
        }
        private List<IncomeProductListViewModel> GetRecordsIndex(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (BudgetYear >= 2024)
            {
                var query = dbe.bgt_income_product.Where(w => w.abip_syarikat_id == SyarikatID && /*w.abip_wilayah_id == WilayahID &&*/ w.abip_ladang_id == LadangID && w.fld_Deleted == false)
                .GroupBy(g => new { g.abip_cost_center, g.abip_budgeting_year })
                .Select(g => new
                {
                    BudgetYear = g.FirstOrDefault().abip_budgeting_year,
                    CostCenterCode = g.FirstOrDefault().abip_cost_center,
                    CostCenterDesc = g.FirstOrDefault().abip_cost_center_desc,
                    JenisProduk = g.FirstOrDefault().abip_product_name,
                    StesenName = g.FirstOrDefault().abip_station_name,
                    stationcode = g.FirstOrDefault().abip_station_code,
                    Total = g.Sum(s => s.abip_grand_total_amount)
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
                              select new IncomeProductListViewModel
                              {
                                  BudgetYear = (int)q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  JenisProduk = q.JenisProduk,
                                  StesenName = q.StesenName,
                                  stationcode = q.stationcode,
                                  Total = q.Total
                              }).Distinct();

                return query3.ToList();
            }
            else
            {
                var query = dbe.bgt_income_product
                .GroupBy(g => new { g.abip_cost_center, g.abip_budgeting_year })
                .Select(g => new
                {
                    BudgetYear = g.FirstOrDefault().abip_budgeting_year,
                    CostCenterCode = g.FirstOrDefault().abip_cost_center,
                    CostCenterDesc = g.FirstOrDefault().abip_cost_center_desc,
                    JenisProduk = g.FirstOrDefault().abip_product_name,
                    StesenName = g.FirstOrDefault().abip_station_name,
                    stationcode = g.FirstOrDefault().abip_station_code,
                    Total = g.Sum(s => s.abip_grand_total_amount)
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
                              select new IncomeProductListViewModel
                              {
                                  BudgetYear = (int)q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  JenisProduk = q.JenisProduk,
                                  StesenName = q.StesenName,
                                  stationcode = q.stationcode,
                                  Total = q.Total
                              }).Distinct();

                return query3.ToList();
            }
        }
        private List<IncomeProductDetailViewModel> GetRecordsDetails(int? budgeting_year = null, string cost_center = null, int? page = null, int? pageSize = null, string excludeId = null, string glCode = null)
        {
            var query = from c in dbe.bgt_income_product
                        where c.fld_Deleted == false
                        group c by new { c.abip_budgeting_year, c.abip_cost_center, c.abip_cost_center_desc, c.abip_product_code, c.abip_product_name }
                        into g
                        select new
                        {
                            yr = g.Key.abip_budgeting_year,
                            cc = g.Key.abip_cost_center,
                            ccd = g.Key.abip_cost_center_desc,
                            prdkc = g.Key.abip_product_code,
                            prdk = g.Key.abip_product_name,
                            glc = g.FirstOrDefault().abip_gl_code,
                            gld = g.FirstOrDefault().abip_gl_desc,
                            gqty = g.Sum(c => c.abip_grand_total_quantity),
                            gtotal = g.Sum(c => c.abip_grand_total_amount)
                        };

            if (budgeting_year.HasValue)
                query = query.Where(q => q.yr == budgeting_year);
            if (!string.IsNullOrEmpty(cost_center))
                query = query.Where(q => q.cc.Contains(cost_center) || q.ccd.Contains(cost_center));
            //if (!string.IsNullOrEmpty(excludeId))
                //query = query.Where(q => q.glc != excludeId);
            if (!string.IsNullOrEmpty(glCode))
                query = query.Where(q => q.glc == glCode);

            var query2 = query.AsQueryable();

            if (page.HasValue && pageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.yr)
                    .ThenBy(q => q.cc)
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.yr)
                    .ThenBy(q => q.cc);
            }
            var query3 = (from q in query2.AsEnumerable()
                          select new IncomeProductDetailViewModel
                          {
                              abip_budgeting_year = q.yr,
                              abip_cost_center = q.cc,
                              abip_cost_center_desc = q.ccd,
                              //abip_gl_code = q.glc,
                              //abip_gl_desc = q.gld,
                              abip_product_code = q.prdkc,
                              abip_product_name = q.prdk,
                              abip_grand_total_quantity = q.gqty,
                              abip_grand_total_amount = q.gtotal
                          }).Distinct();
            return query3.ToList();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                dbe.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
