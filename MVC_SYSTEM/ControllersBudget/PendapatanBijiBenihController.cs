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
using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using System.Net;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class PendapatanBijiBenihController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_MasterModels dbc = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_ModelsBudgetEst dbe = new MVC_SYSTEM_ModelsBudgetEst();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        Connection Connection = new Connection();
        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        GetBudgetClass GetBudgetClass = new GetBudgetClass();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        CostCenter CostCenter = new CostCenter();
        BudgetUserMatrix UserMatrix = new BudgetUserMatrix();//new oct

        private List<tbl_SAPCCPUP> GetCostCenters(int budgetYear, bool exclude = true)//new sept
        {
            if (exclude)
            {
                var income_bijibenih = GetRecords(budgetYear);
                var excludeCostCenters = new List<string>();

                for (int i = 0; i < income_bijibenih.Count(); i++)
                {
                    excludeCostCenters.Add(income_bijibenih[i].CostCenterCode);
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
        private List<ProductSelectList> GetProducts(int budgetYear)//new sept
        {
            //note
            //LocID must be based on user role tagged to location

            var WilayahID = wilayah.GetWilayahID();
            var query = db.bgt_Product.Where(w => w.PrdCode == "1701" && w.YearBgt == budgetYear && w.Active == true && w.Lowest == true && w.LocID == WilayahID).ToList();
            var query2 = query.AsQueryable();
            query2 = query2.OrderBy(q => q.PrdCode);

            var query3 = (from q in query2.AsEnumerable()
                          select new ProductSelectList
                          {
                              PrdId = q.PrdID.ToString(),
                              PrdCode = q.PrdCode,
                              PrdName = q.PrdName
                          }).Distinct();

            return query3.ToList();
        }
        public JsonResult GetDataFromCostCenter(string costcentercode)//new sept
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

                var WilayahData = dbc.tbl_Wilayah.Where(w => w.fld_ID == CostCenterData.FirstOrDefault().fld_WilayahID);

                if (StationNCData.Count() > 0)
                {
                    return Json(new
                    {
                        success = true,
                        costcenter = CostCenterData.FirstOrDefault().fld_CostCenter,
                        costcenterdesc = CostCenterData.FirstOrDefault().fld_CostCenterDesc,
                        stationcode = StationNCData.FirstOrDefault().Code_LL,
                        stationdesc = StationNCData.FirstOrDefault().Station_Desc,
                        costcentertrim = TrimedFirstCC,
                        wilayahid = CostCenterData.FirstOrDefault().fld_WilayahID,
                        wilayahname = WilayahData.FirstOrDefault().fld_WlyhName
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
        public string GetGLDesc(string glcode)
        {
            var GLData = dbc.tbl_SAPGLPUP.Where(x => x.fld_GLCode == glcode);

            var result = "";
            if (GLData.Count() > 0)
            {
                result = GLData.FirstOrDefault().fld_GLDesc;
            }
            return result;
        }
        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }
        public ActionResult Index(string BudgetYear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
            if (!RoleScreen.view)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> Year = new List<SelectListItem>();
            Year = new SelectList(db.bgt_Notification.Where(w => w.fld_Deleted == false).Select(s => new SelectListItem { Value = s.Bgt_Year.ToString(), Text = s.Bgt_Year.ToString() }).OrderByDescending(o => o.Value), "Value", "Text").ToList();
            ViewBag.BudgetYear = Year;

            if (string.IsNullOrEmpty(BudgetYear))
            {
                var SelectedYear = GetYears().FirstOrDefault().ToString();
                ViewBag.SelectedYear = SelectedYear;
            }
            else
            {
                ViewBag.SelectedYear = BudgetYear;
            }

            return View();
        }
        public ActionResult _Index(string sortOrder, string BudgetYear, string CostCenter, string BudgetYearFilter, string CostCenterFilter, int? page)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //Get total gl
            var ID = GetBudgetClass.GetScreenID("I2");
            var gldata = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            if (gldata.Count > 0)
            {
                ViewBag.TotalGL = gldata.Count();
            }
            else
            {
                ViewBag.TotalGL = 2;
            }

            if (!string.IsNullOrEmpty(BudgetYear) && !string.IsNullOrEmpty(CostCenter)) //all select
            {
                //page = 1;
            }
            else if (!string.IsNullOrEmpty(BudgetYear) && string.IsNullOrEmpty(CostCenter)) //year
            {
                //page = 1;
            }
            else if (string.IsNullOrEmpty(BudgetYear) && !string.IsNullOrEmpty(CostCenter)) //cc
            {
                //page = 1;
            }
            else
            {
                BudgetYear = BudgetYearFilter;//
                CostCenter = CostCenterFilter;//
            }
            ViewBag.CurrentSort = sortOrder;//
            ViewBag.YearFilter = BudgetYearFilter;//
            ViewBag.CostCenterFilter = CostCenterFilter;//
            ViewBag.YearSelect = BudgetYear;//
            ViewBag.CostCenterSelect = CostCenter;//

            //var records = new PagedList<bgt_income_bijibenih>();
            //List<bgt_income_bijibenih> bgt_income_bijibenih = new List<bgt_income_bijibenih>(); //Fitri add 9.11.2022
            List<IncomeBijiBenihListViewModel> IncomeBijiBenihListViewModel = new List<IncomeBijiBenihListViewModel>();

            List<bgt_income_bijibenih> result = new List<bgt_income_bijibenih>();
            if (Convert.ToInt32(BudgetYear) >= 2024)
            {
                //result = dbr.bgt_income_bijibenih.Where(w => w.abis_ladang_id == LadangID && w.abis_wilayah_id == WilayahID && w.abis_syarikat_id == SyarikatID && w.fld_Deleted == false).ToList();
                result = dbr.bgt_income_bijibenih.Where(w => w.abis_ladang_id == LadangID && w.abis_syarikat_id == SyarikatID && w.fld_Deleted == false).ToList();
            }
            else
            {
                result = dbr.bgt_income_bijibenih.ToList();
            }

            if (string.IsNullOrEmpty(BudgetYear))
            {
                if (string.IsNullOrEmpty(CostCenter))
                {
                    var query = result.GroupBy(g => new { g.abis_budgeting_year, g.abis_cost_center, g.abis_gl_code })
                        .Select(g => new
                        {
                            year = g.FirstOrDefault().abis_budgeting_year,
                            cc = g.FirstOrDefault().abis_cost_center,
                            ccd = g.FirstOrDefault().abis_cost_center_desc,
                            prd = g.FirstOrDefault().abis_product_name,
                            gl = g.FirstOrDefault().abis_gl_code,
                            qty1 = g.FirstOrDefault().abis_grand_quantity1,
                            qty2 = g.FirstOrDefault().abis_grand_quantity2,
                            amt1 = g.FirstOrDefault().abis_grand_amount1,
                            amt2 = g.FirstOrDefault().abis_grand_amount2,
                            allqty = g.Sum(m => m.abis_grand_total_quantity),
                            allamt = g.Sum(m => m.abis_grand_total_amount)
                        }).OrderByDescending(o => o.year).ThenBy(o => o.cc).ToList();
                    foreach (var val in query)
                    {
                        IncomeBijiBenihListViewModel.Add(new IncomeBijiBenihListViewModel()
                        {
                            AllAmount = val.allamt,
                            AllQuantity = val.allqty,
                            Amount1 = val.amt1,
                            Amount2 = val.amt2,
                            Quantity1 = val.qty1,
                            Quantity2 = val.qty2,
                            BudgetYear = (int)val.year,
                            JenisProduk = val.prd,
                            CostCenterCode = val.cc,
                            CostCenterDesc = val.ccd,
                            GlCode = val.gl
                        });
                    }
                }
                else
                {
                    var query = result.Where(w => w.abis_cost_center.Contains(CostCenter) || w.abis_cost_center_desc.Contains(CostCenter))
                                   .GroupBy(g => new { g.abis_budgeting_year, g.abis_cost_center, g.abis_gl_code })
                                   .Select(g => new
                                   {
                                       year = g.FirstOrDefault().abis_budgeting_year,
                                       cc = g.FirstOrDefault().abis_cost_center,
                                       ccd = g.FirstOrDefault().abis_cost_center_desc,
                                       prd = g.FirstOrDefault().abis_product_name,
                                       gl = g.FirstOrDefault().abis_gl_code,
                                       qty1 = g.FirstOrDefault().abis_grand_quantity1,
                                       qty2 = g.FirstOrDefault().abis_grand_quantity2,
                                       amt1 = g.FirstOrDefault().abis_grand_amount1,
                                       amt2 = g.FirstOrDefault().abis_grand_amount2,
                                       allqty = g.Sum(m => m.abis_grand_total_quantity),
                                       allamt = g.Sum(m => m.abis_grand_total_amount)
                                   }).OrderByDescending(o => o.year).ThenBy(o => o.cc).ToList();
                    foreach (var val in query)
                    {
                        IncomeBijiBenihListViewModel.Add(new IncomeBijiBenihListViewModel()
                        {
                            AllAmount = val.allamt,
                            AllQuantity = val.allqty,
                            Amount1 = val.amt1,
                            Amount2 = val.amt2,
                            Quantity1 = val.qty1,
                            Quantity2 = val.qty2,
                            BudgetYear = (int)val.year,
                            JenisProduk = val.prd,
                            CostCenterCode = val.cc,
                            CostCenterDesc = val.ccd,
                            GlCode = val.gl
                        });
                    }

                }
            }
            else
            {
                if (string.IsNullOrEmpty(CostCenter))
                {
                    var query = result.Where(w => w.abis_budgeting_year.ToString() == BudgetYear)
                                   .GroupBy(g => new { g.abis_budgeting_year, g.abis_cost_center, g.abis_gl_code })
                                   .Select(g => new
                                   {
                                       year = g.FirstOrDefault().abis_budgeting_year,
                                       cc = g.FirstOrDefault().abis_cost_center,
                                       ccd = g.FirstOrDefault().abis_cost_center_desc,
                                       prd = g.FirstOrDefault().abis_product_name,
                                       gl = g.FirstOrDefault().abis_gl_code,
                                       qty1 = g.FirstOrDefault().abis_grand_quantity1,
                                       qty2 = g.FirstOrDefault().abis_grand_quantity2,
                                       amt1 = g.FirstOrDefault().abis_grand_amount1,
                                       amt2 = g.FirstOrDefault().abis_grand_amount2,
                                       allqty = g.Sum(m => m.abis_grand_total_quantity),
                                       allamt = g.Sum(m => m.abis_grand_total_amount)
                                   }).OrderByDescending(o => o.year).ThenBy(o => o.cc).ToList();
                    foreach (var val in query)
                    {
                        IncomeBijiBenihListViewModel.Add(new IncomeBijiBenihListViewModel()
                        {
                            AllAmount = val.allamt,
                            AllQuantity = val.allqty,
                            Amount1 = val.amt1,
                            Amount2 = val.amt2,
                            Quantity1 = val.qty1,
                            Quantity2 = val.qty2,
                            BudgetYear = (int)val.year,
                            JenisProduk = val.prd,
                            CostCenterCode = val.cc,
                            CostCenterDesc = val.ccd,
                            GlCode = val.gl
                        });
                    }
                }
                else
                {

                    var query = result.Where(w => w.abis_budgeting_year.ToString() == BudgetYear && w.abis_cost_center.Contains(CostCenter) || w.abis_cost_center_desc.Contains(CostCenter))
                                   .GroupBy(g => new { g.abis_budgeting_year, g.abis_cost_center, g.abis_gl_code })
                                   .Select(g => new
                                   {
                                       year = g.FirstOrDefault().abis_budgeting_year,
                                       cc = g.FirstOrDefault().abis_cost_center,
                                       ccd = g.FirstOrDefault().abis_cost_center_desc,
                                       prd = g.FirstOrDefault().abis_product_name,
                                       gl = g.FirstOrDefault().abis_gl_code,
                                       qty1 = g.FirstOrDefault().abis_grand_quantity1,
                                       qty2 = g.FirstOrDefault().abis_grand_quantity2,
                                       amt1 = g.FirstOrDefault().abis_grand_amount1,
                                       amt2 = g.FirstOrDefault().abis_grand_amount2,
                                       allqty = g.Sum(m => m.abis_grand_total_quantity),
                                       allamt = g.Sum(m => m.abis_grand_total_amount)
                                   }).OrderByDescending(o => o.year).ThenBy(o => o.cc).ToList();
                    foreach (var val in query)
                    {
                        IncomeBijiBenihListViewModel.Add(new IncomeBijiBenihListViewModel()
                        {
                            AllAmount = val.allamt,
                            AllQuantity = val.allqty,
                            Amount1 = val.amt1,
                            Amount2 = val.amt2,
                            Quantity1 = val.qty1,
                            Quantity2 = val.qty2,
                            BudgetYear = (int)val.year,
                            JenisProduk = val.prd,
                            CostCenterCode = val.cc,
                            CostCenterDesc = val.ccd,
                            GlCode = val.gl
                        });
                    }
                }
            }
            int pageSize = int.Parse(GetConfig.GetData("paging")) * 2;
            //int pageSize = 2;
            int pageNumber = (page ?? 1);
            ViewBag.pageno = pageNumber;
            ViewBag.itemperpage = pageSize;
            return View(IncomeBijiBenihListViewModel.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult CreatePendapatanBijiBenih()
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
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

            int Year = GetBudgetClass.GetBudgetYear();
            ViewBag.SyarikatID = SyarikatID;

            //Get CC
            //var IncludeCC = dbr.bgt_income_bijibenih
            //    .Where(w => w.abis_budgeting_year == Year && w.abis_ladang_id == LadangID && w.abis_syarikat_id == SyarikatID && w.abis_wilayah_id == WilayahID && w.fld_Deleted == false)
            //    .Select(s => s.abis_cost_center)
            //    .ToList();

            //List<SelectListItem> CostCenter = new List<SelectListItem>();
            //CostCenter = new SelectList(dbc.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }).Distinct(), "Value", "Text").ToList();
            //CostCenter.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            //ViewBag.CostCenterList = CostCenter;

            

            //note
            //LocID must be based on user role tagged to location

            var LocationID = wilayah.GetWilayahID();

            List<SelectListItem> Product = new List<SelectListItem>();
            Product = new SelectList(db.bgt_Product
                .Where(x => x.PrdCode == "1701" && x.Lowest == true && x.Active == true /*&& x.LocID == LocationID*/)
                .Select(s => new SelectListItem { Value = s.PrdCode, Text = s.PrdName })
                .Distinct(), "Value", "Text").ToList();

            ViewBag.ProductList = Product;

            var ProductList = db.bgt_Product.Where(x => x.YearBgt == Year && x.PrdCode == "1701" && x.Lowest == true && x.Active == true /*&& x.LocID == LocationID*/).ToList();
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

            var costCenters = new SelectList(GetCostCenters(Year).Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter.TrimStart(new char[] { '0' }),
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();
            ViewBag.CostCenterList = costCenters;

            var getPriceBijiBenih = db.vw_FeldaGrpPriceBBSawit.Where(x => x.PriceYear == Year && x.Co_Active == true).OrderByDescending(o => o.PriceYear).ThenBy(t => t.fld_ID).ToList();
            if (getPriceBijiBenih.Count() > 0)
            {
                ViewBag.getPriceBijiBenih = getPriceBijiBenih;
                ViewBag.getPriceBijiBenihA = getPriceBijiBenih.Where(w => w.CC_Sort == 1).ToList();
                ViewBag.getPriceBijiBenihB = getPriceBijiBenih.Where(w => w.CC_Sort == 2).ToList();
            }
            else
            {
                var getLastYearPriceBijiBenih = db.vw_FeldaGrpPriceBBSawit.Where(x => x.PriceYear == (Year - 1) && x.Co_Active == true).OrderByDescending(o => o.PriceYear).ThenBy(t => t.fld_ID).ToList();
                ViewBag.getPriceBijiBenih = getLastYearPriceBijiBenih;
                ViewBag.getPriceBijiBenihA = getLastYearPriceBijiBenih.Where(w => w.CC_Sort == 1).ToList();
                ViewBag.getPriceBijiBenihB = getLastYearPriceBijiBenih.Where(w => w.CC_Sort == 2).ToList();
            }

            //var getPriceBijiBenih = db.bgt_FeldaGrp.Where(x => x.PriceYear == Year && x.Co_Active == true).OrderByDescending(o => o.PriceYear).ThenBy(t => t.fld_ID).ToList();
            //if (getPriceBijiBenih.Count() > 0)
            //{
            //    ViewBag.getPriceBijiBenih = getPriceBijiBenih;
            //}
            //else
            //{
            //    var getLastYearPriceBijiBenih = db.bgt_FeldaGrp.Where(x => x.PriceYear == (Year - 1) && x.Co_Active == true).OrderByDescending(o => o.PriceYear).ThenBy(t => t.fld_ID).ToList();
            //    ViewBag.getPriceBijiBenih = getLastYearPriceBijiBenih;
            //}

            var gl_list = dbc.tbl_SAPGLPUP.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).ToList();//new sept

            var GetGLList = GetBudgetClass.GetFeldaGrp((int)SyarikatID).ToList();
            if (GetGLList.Count > 0)
            {
                var GLcodeSK = GetGLList.Where(w => w.CC_Sort == 1).Select(s => s.GLBBSawit).FirstOrDefault();
                var GLcodeBL = GetGLList.Where(w => w.CC_Sort == 2).Select(s => s.GLBBSawit).FirstOrDefault();

                ViewBag.GLcodeSK = GLcodeSK;
                ViewBag.GLcodeBL = GLcodeBL;

                var newGLCodeSK = GLcodeSK;
                var newGLCodeBL = GLcodeBL;

                var gldescsk = gl_list.Where(w => w.fld_GLCode.EndsWith(newGLCodeSK)).Select(s => s.fld_GLDesc).FirstOrDefault();
                var gldescbl = gl_list.Where(w => w.fld_GLCode.EndsWith(newGLCodeBL)).Select(s => s.fld_GLDesc).FirstOrDefault();

                ViewBag.GLdescSK = gldescsk;
                ViewBag.GLdescBL = gldescbl;
            }

            ////Get GL
            //var ID = GetBudgetClass.GetScreenID("I2");
            //var getAllID = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            //List<Guid> GLID = new List<Guid>();
            //GLID = getAllID.Select(s => s.fld_GLID).ToList();
            //ViewBag.GLList = dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID)).OrderBy(o => o.fld_GLCode).ToList();
            //var gl_list = dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID)).OrderBy(o => o.fld_GLCode).ToList();
            //ViewBag.gl_code_non_fp = gl_list.FirstOrDefault().fld_GLCode;
            //ViewBag.gl_desc_non_fp = gl_list.FirstOrDefault().fld_GLDesc;
            //ViewBag.gl_code_fp = gl_list.LastOrDefault().fld_GLCode;
            //ViewBag.gl_desc_fp = gl_list.LastOrDefault().fld_GLDesc;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePendapatanBijiBenih(IncomeBijiBenihCreateEditViewModel[] bgtincomebijibenih, int BudgetYear, string CostCenterList, string CostCenterDesc, string stationcode, string stationname)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
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
                    var ladangObj = ladang.GetLadang();
                    //var wilayahObj = wilayah.GetWilayah();

                    foreach (var val in bgtincomebijibenih)
                    {
                        var checkdata = dbr.bgt_income_bijibenih.Where(x => x.abis_budgeting_year == BudgetYear && x.abis_cost_center == CostCenterList && x.fld_Deleted == false).FirstOrDefault();
                        if (checkdata == null)
                        {
                            bgt_income_bijibenih bib = new bgt_income_bijibenih()
                            {
                                abis_revision = 0,
                                abis_history = false,
                                abis_status = "DRAFT",
                                abis_budgeting_year = BudgetYear,
                                abis_syarikat_id = SyarikatID,
                                abis_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                                abis_ladang_id = LadangID,
                                abis_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                                abis_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                                //abis_wilayah_id = WilayahID,
                                //abis_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                                abis_wilayah_id = val.abis_wilayah_id,//new sept
                                abis_wilayah_name = !string.IsNullOrEmpty(val.abis_wilayah_name) ? val.abis_wilayah_name : null,//new sept
                                abis_fld_ID = val.abis_fld_ID, //new sept
                                abis_company_category = val.abis_company_category,
                                abis_company_code = val.abis_company_code,
                                abis_company_name = val.abis_company_name,
                                abis_product_code = val.abis_product_code,
                                abis_product_name = val.abis_product_name,
                                abis_station_code = stationcode.ToString(),
                                abis_station_name = stationname.ToString(),
                                abis_cost_center = CostCenterList,
                                abis_cost_center_desc = CostCenterDesc,
                                abis_gl_code = val.abis_gl_code,
                                abis_gl_desc = val.abis_gl_desc,
                                abis_uom = val.abis_uom,

                                abis_unit_price = !string.IsNullOrEmpty(val.abis_unit_price) && decimal.TryParse(val.abis_unit_price.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_unit_price.Replace(",", "")) : (decimal?)0.00,

                                abis_amount_1 = !string.IsNullOrEmpty(val.abis_amount_1) && decimal.TryParse(val.abis_amount_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_1.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_2 = !string.IsNullOrEmpty(val.abis_amount_2) && decimal.TryParse(val.abis_amount_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_2.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_3 = !string.IsNullOrEmpty(val.abis_amount_3) && decimal.TryParse(val.abis_amount_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_3.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_4 = !string.IsNullOrEmpty(val.abis_amount_4) && decimal.TryParse(val.abis_amount_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_4.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_5 = !string.IsNullOrEmpty(val.abis_amount_5) && decimal.TryParse(val.abis_amount_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_5.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_6 = !string.IsNullOrEmpty(val.abis_amount_6) && decimal.TryParse(val.abis_amount_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_6.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_7 = !string.IsNullOrEmpty(val.abis_amount_7) && decimal.TryParse(val.abis_amount_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_7.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_8 = !string.IsNullOrEmpty(val.abis_amount_8) && decimal.TryParse(val.abis_amount_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_8.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_9 = !string.IsNullOrEmpty(val.abis_amount_9) && decimal.TryParse(val.abis_amount_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_9.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_10 = !string.IsNullOrEmpty(val.abis_amount_10) && decimal.TryParse(val.abis_amount_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_10.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_11 = !string.IsNullOrEmpty(val.abis_amount_11) && decimal.TryParse(val.abis_amount_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_11.Replace(",", "")) : (decimal?)0.00,
                                abis_amount_12 = !string.IsNullOrEmpty(val.abis_amount_12) && decimal.TryParse(val.abis_amount_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_amount_12.Replace(",", "")) : (decimal?)0.00,
                                abis_total_amount_a = !string.IsNullOrEmpty(val.abis_total_amount_a) && decimal.TryParse(val.abis_total_amount_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_total_amount_a.Replace(",", "")) : (decimal?)0.00,
                                abis_total_amount_b = !string.IsNullOrEmpty(val.abis_total_amount_b) && decimal.TryParse(val.abis_total_amount_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_total_amount_b.Replace(",", "")) : (decimal?)0.00,
                                abis_grand_total_amount = !string.IsNullOrEmpty(val.abis_grand_total_amount) && decimal.TryParse(val.abis_grand_total_amount.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_grand_total_amount.Replace(",", "")) : (decimal?)0.00,
                                abis_grand_amount1 = !string.IsNullOrEmpty(val.abis_grand_amount1) && decimal.TryParse(val.abis_grand_amount1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_grand_amount1.Replace(",", "")) : (decimal?)0.00,
                                abis_grand_amount2 = !string.IsNullOrEmpty(val.abis_grand_amount2) && decimal.TryParse(val.abis_grand_amount2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_grand_amount2.Replace(",", "")) : (decimal?)0.00,

                                abis_qty_1 = !string.IsNullOrEmpty(val.abis_qty_1) && decimal.TryParse(val.abis_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_1.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_2 = !string.IsNullOrEmpty(val.abis_qty_2) && decimal.TryParse(val.abis_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_2.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_3 = !string.IsNullOrEmpty(val.abis_qty_3) && decimal.TryParse(val.abis_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_3.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_4 = !string.IsNullOrEmpty(val.abis_qty_4) && decimal.TryParse(val.abis_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_4.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_5 = !string.IsNullOrEmpty(val.abis_qty_5) && decimal.TryParse(val.abis_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_5.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_6 = !string.IsNullOrEmpty(val.abis_qty_6) && decimal.TryParse(val.abis_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_6.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_7 = !string.IsNullOrEmpty(val.abis_qty_7) && decimal.TryParse(val.abis_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_7.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_8 = !string.IsNullOrEmpty(val.abis_qty_8) && decimal.TryParse(val.abis_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_8.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_9 = !string.IsNullOrEmpty(val.abis_qty_9) && decimal.TryParse(val.abis_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_9.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_10 = !string.IsNullOrEmpty(val.abis_qty_10) && decimal.TryParse(val.abis_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_10.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_11 = !string.IsNullOrEmpty(val.abis_qty_11) && decimal.TryParse(val.abis_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_11.Replace(",", "")) : (decimal?)0.000,
                                abis_qty_12 = !string.IsNullOrEmpty(val.abis_qty_12) && decimal.TryParse(val.abis_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_qty_12.Replace(",", "")) : (decimal?)0.000,
                                abis_total_qty_a = !string.IsNullOrEmpty(val.abis_total_qty_a) && decimal.TryParse(val.abis_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_total_qty_a.Replace(",", "")) : (decimal?)0.000,
                                abis_total_qty_b = !string.IsNullOrEmpty(val.abis_total_qty_b) && decimal.TryParse(val.abis_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_total_qty_b.Replace(",", "")) : (decimal?)0.000,
                                abis_grand_total_quantity = !string.IsNullOrEmpty(val.abis_grand_total_quantity) && decimal.TryParse(val.abis_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_grand_total_quantity.Replace(",", "")) : (decimal?)0.000,
                                abis_grand_quantity1 = !string.IsNullOrEmpty(val.abis_grand_quantity1) && decimal.TryParse(val.abis_grand_quantity1.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_grand_quantity1.Replace(",", "")) : (decimal?)0.000,
                                abis_grand_quantity2 = !string.IsNullOrEmpty(val.abis_grand_quantity2) && decimal.TryParse(val.abis_grand_quantity2.Replace(",", ""), out resultAmount) ? decimal.Parse(val.abis_grand_quantity2.Replace(",", "")) : (decimal?)0.000,

                                abis_created_by = getidentity.ID(User.Identity.Name),
                                last_modified = DateTime.Now,
                                fld_Deleted = false,
                                fld_NegaraID = NegaraID
                            };
                            dbr.bgt_income_bijibenih.Add(bib);
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
                        div = "searchResult",
                        rootUrl = domain,
                        action = "_Index",
                        controller = "PendapatanBijiBenih",
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
        public ActionResult Update(string costcenter, string budgetyear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
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

            //bgt_income_bijibenih[] bgt_income_bijibenih = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter && x.abis_budgeting_year.ToString() == budgetyear && x.abis_wilayah_id == WilayahID && x.abis_syarikat_id == SyarikatID && x.fld_NegaraID == NegaraID && x.abis_ladang_id == LadangID && x.fld_Deleted == false).ToArray();
            bgt_income_bijibenih[] bgt_income_bijibenih = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter && x.abis_budgeting_year.ToString() == budgetyear && x.abis_syarikat_id == SyarikatID && x.fld_NegaraID == NegaraID && x.abis_ladang_id == LadangID && x.fld_Deleted == false).ToArray();

            var getfldgrp = dbr.bgt_income_bijibenih.Where(w => w.abis_cost_center == costcenter && w.abis_budgeting_year.ToString() == budgetyear).ToList();
            if (getfldgrp.Count() > 0)
            {
                ViewBag.GetData = getfldgrp;
                ViewBag.GroupCompanyA = getfldgrp.Where(w => w.abis_company_code == "1").ToList();
                ViewBag.GroupCompanyB = getfldgrp.Where(w => w.abis_company_code == "2").ToList();
                ViewBag.GroupCompanyACount = getfldgrp.Count();
                ViewBag.GroupCompanyBCount = getfldgrp.Count();
            }

            //note
            //LocID must be based on user role tagged to location
            //Add 21.8.2023
            var LocationID = wilayah.GetWilayahID() + 1;

            var ProductList = db.bgt_Product.Where(x => x.PrdCode == "1701" && x.Lowest == true && x.Active == true && x.LocID == LocationID).ToList();
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
            //End add

            //var getPriceBijiBenih = db.vw_FeldaGrpPriceBBSawit.Where(x => x.PriceYear == budgetyear && x.Co_Active == true).OrderBy(o => o.fld_ID).ToList();
            //if (getPriceBijiBenih.Count() > 0)
            //{
            //    ViewBag.getPriceBijiBenih = getPriceBijiBenih;
            //}
            //else
            //{
            //    var getLastYearPriceBijiBenih = db.vw_FeldaGrpPriceBBSawit.Where(x => x.PriceYear == (budgetyear - 1) && x.Co_Active == true).OrderBy(o => o.fld_ID).ToList();
            //    ViewBag.getPriceBijiBenih = getLastYearPriceBijiBenih;
            //}

            //new sept
            var getPriceBijiBenih = db.vw_FeldaGrpPriceBBSawit.Where(x => x.PriceYear.ToString() == budgetyear && x.Co_Active == true).OrderByDescending(o => o.PriceYear).ThenBy(t => t.fld_ID).ToList();
            if (getPriceBijiBenih.Count() > 0)
            {
                ViewBag.getPriceBijiBenih = getPriceBijiBenih;
                ViewBag.getPriceBijiBenihA = getPriceBijiBenih.Where(w => w.CC_Sort == 1).ToList();
                ViewBag.getPriceBijiBenihB = getPriceBijiBenih.Where(w => w.CC_Sort == 2).ToList();
            }
            else
            {
                var getLastYearPriceBijiBenih = db.vw_FeldaGrpPriceBBSawit.Where(x => x.PriceYear == (int.Parse(budgetyear) - 1) && x.Co_Active == true).OrderByDescending(o => o.PriceYear).ThenBy(t => t.fld_ID).ToList();
                ViewBag.getPriceBijiBenih = getLastYearPriceBijiBenih;
                ViewBag.getPriceBijiBenihA = getLastYearPriceBijiBenih.Where(w => w.CC_Sort == 1).ToList();
                ViewBag.getPriceBijiBenihB = getLastYearPriceBijiBenih.Where(w => w.CC_Sort == 2).ToList();
            }

            //Get GL
            var ID = GetBudgetClass.GetScreenID("I2");
            var getAllID = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            List<Guid> GLID = new List<Guid>();
            GLID = getAllID.Select(s => s.fld_GLID).ToList();
            ViewBag.GLList = dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID)).OrderBy(o => o.fld_GLCode).ToList();

            return PartialView("Update", bgt_income_bijibenih);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(IncomeBijiBenihCreateEditViewModel[] bgtibb, int BudgetYear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
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

            try
            {
                var resultAmount = 0m;

                foreach (var bib in bgtibb)
                {
                    var getdata = dbr.bgt_income_bijibenih.Where(w => w.abis_fld_ID == bib.abis_fld_ID && w.abis_cost_center == bib.abis_cost_center &&
                                  w.abis_budgeting_year == BudgetYear && w.abis_ladang_id == LadangID && w.abis_wilayah_id == bib.abis_wilayah_id &&
                                  w.abis_syarikat_id == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();

                    getdata.abis_unit_price = !string.IsNullOrEmpty(bib.abis_unit_price) && decimal.TryParse(bib.abis_unit_price.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_unit_price.Replace(",", "")) : (decimal?)0.00;

                    getdata.abis_qty_1 = !string.IsNullOrEmpty(bib.abis_qty_1) && decimal.TryParse(bib.abis_qty_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_1.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_2 = !string.IsNullOrEmpty(bib.abis_qty_2) && decimal.TryParse(bib.abis_qty_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_2.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_3 = !string.IsNullOrEmpty(bib.abis_qty_3) && decimal.TryParse(bib.abis_qty_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_3.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_4 = !string.IsNullOrEmpty(bib.abis_qty_4) && decimal.TryParse(bib.abis_qty_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_4.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_5 = !string.IsNullOrEmpty(bib.abis_qty_5) && decimal.TryParse(bib.abis_qty_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_5.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_6 = !string.IsNullOrEmpty(bib.abis_qty_6) && decimal.TryParse(bib.abis_qty_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_6.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_7 = !string.IsNullOrEmpty(bib.abis_qty_7) && decimal.TryParse(bib.abis_qty_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_7.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_8 = !string.IsNullOrEmpty(bib.abis_qty_8) && decimal.TryParse(bib.abis_qty_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_8.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_9 = !string.IsNullOrEmpty(bib.abis_qty_9) && decimal.TryParse(bib.abis_qty_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_9.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_10 = !string.IsNullOrEmpty(bib.abis_qty_10) && decimal.TryParse(bib.abis_qty_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_10.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_11 = !string.IsNullOrEmpty(bib.abis_qty_11) && decimal.TryParse(bib.abis_qty_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_11.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_qty_12 = !string.IsNullOrEmpty(bib.abis_qty_12) && decimal.TryParse(bib.abis_qty_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_qty_12.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_total_qty_a = !string.IsNullOrEmpty(bib.abis_total_qty_a) && decimal.TryParse(bib.abis_total_qty_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_total_qty_a.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_total_qty_b = !string.IsNullOrEmpty(bib.abis_total_qty_b) && decimal.TryParse(bib.abis_total_qty_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_total_qty_b.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_grand_total_quantity = !string.IsNullOrEmpty(bib.abis_grand_total_quantity) && decimal.TryParse(bib.abis_grand_total_quantity.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_grand_total_quantity.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_grand_quantity1 = !string.IsNullOrEmpty(bib.abis_grand_quantity1) && decimal.TryParse(bib.abis_grand_quantity1.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_grand_quantity1.Replace(",", "")) : (decimal?)0.000;
                    getdata.abis_grand_quantity2 = !string.IsNullOrEmpty(bib.abis_grand_quantity2) && decimal.TryParse(bib.abis_grand_quantity2.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_grand_quantity2.Replace(",", "")) : (decimal?)0.000;

                    getdata.abis_amount_1 = !string.IsNullOrEmpty(bib.abis_amount_1) && decimal.TryParse(bib.abis_amount_1.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_1.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_2 = !string.IsNullOrEmpty(bib.abis_amount_2) && decimal.TryParse(bib.abis_amount_2.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_2.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_3 = !string.IsNullOrEmpty(bib.abis_amount_3) && decimal.TryParse(bib.abis_amount_3.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_3.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_4 = !string.IsNullOrEmpty(bib.abis_amount_4) && decimal.TryParse(bib.abis_amount_4.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_4.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_5 = !string.IsNullOrEmpty(bib.abis_amount_5) && decimal.TryParse(bib.abis_amount_5.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_5.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_6 = !string.IsNullOrEmpty(bib.abis_amount_6) && decimal.TryParse(bib.abis_amount_6.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_6.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_7 = !string.IsNullOrEmpty(bib.abis_amount_7) && decimal.TryParse(bib.abis_amount_7.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_7.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_8 = !string.IsNullOrEmpty(bib.abis_amount_8) && decimal.TryParse(bib.abis_amount_8.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_8.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_9 = !string.IsNullOrEmpty(bib.abis_amount_9) && decimal.TryParse(bib.abis_amount_9.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_9.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_10 = !string.IsNullOrEmpty(bib.abis_amount_10) && decimal.TryParse(bib.abis_amount_10.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_10.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_11 = !string.IsNullOrEmpty(bib.abis_amount_11) && decimal.TryParse(bib.abis_amount_11.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_11.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_amount_12 = !string.IsNullOrEmpty(bib.abis_amount_12) && decimal.TryParse(bib.abis_amount_12.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_amount_12.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_total_amount_a = !string.IsNullOrEmpty(bib.abis_total_amount_a) && decimal.TryParse(bib.abis_total_amount_a.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_total_amount_a.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_total_amount_b = !string.IsNullOrEmpty(bib.abis_total_amount_b) && decimal.TryParse(bib.abis_total_amount_b.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_total_amount_b.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_grand_total_amount = !string.IsNullOrEmpty(bib.abis_grand_total_amount) && decimal.TryParse(bib.abis_grand_total_amount.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_grand_total_amount.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_grand_amount1 = !string.IsNullOrEmpty(bib.abis_grand_amount1) && decimal.TryParse(bib.abis_grand_amount1.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_grand_amount1.Replace(",", "")) : (decimal?)0.00;
                    getdata.abis_grand_amount2 = !string.IsNullOrEmpty(bib.abis_grand_amount2) && decimal.TryParse(bib.abis_grand_amount2.Replace(",", ""), out resultAmount) ? decimal.Parse(bib.abis_grand_amount2.Replace(",", "")) : (decimal?)0.00;

                    getdata.fld_NegaraID = NegaraID;
                    getdata.abis_syarikat_id = SyarikatID;
                    getdata.abis_wilayah_id = bib.abis_wilayah_id; //new sept
                    getdata.abis_wilayah_name = bib.abis_wilayah_name; //new sept
                    getdata.abis_ladang_id = LadangID;
                    getdata.abis_created_by = getidentity.ID(User.Identity.Name);
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
                    div = "searchResult",
                    rootUrl = domain,
                    action = "_Index",
                    controller = "PendapatanBijiBenih",
                    paramName = "BudgetYear",
                    paramValue = BudgetYear
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult Views(string costcenter, int budgetyear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
            if (!RoleScreen.view)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //M.s
            int totalpage = 0;
            int page = 0;
            int pagesize = 0;
            int totalldata = 0;

            if (budgetyear >= 2024)
            {
                bgt_income_bijibenih[] bgt_income_bijibenihs = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter && x.abis_budgeting_year == budgetyear && x.fld_Deleted == false).ToArray();

                ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                ViewBag.BudgetYear = bgt_income_bijibenihs.Select(s => s.abis_budgeting_year).FirstOrDefault();

                ViewBag.fld_GLCode = bgt_income_bijibenihs.Select(s => s.abis_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgt_income_bijibenihs.Select(s => s.abis_gl_desc).FirstOrDefault().ToUpper();

                ViewBag.ProductCode = bgt_income_bijibenihs.Select(s => s.abis_product_code).FirstOrDefault();
                ViewBag.ProductName = bgt_income_bijibenihs.Select(s => s.abis_product_name).FirstOrDefault();

                var bgtincomedata = bgt_income_bijibenihs.GroupBy(g => new { g.abis_cost_center, g.abis_budgeting_year }).ToList();

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

                return PartialView("Views", bgt_income_bijibenihs);
            }
            else
            {
                bgt_income_bijibenih[] bgtincomebijibenih = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter && x.abis_budgeting_year == budgetyear).ToArray();

                ViewBag.NamaSyarikat = "FGV Agri Services Sdn Bhd";
                ViewBag.NoSyarikat = "Company No. 353791-M";
                ViewBag.Message = @GlobalResEstate.msgNoRecord;

                ViewBag.BudgetYear = bgtincomebijibenih.Select(s => s.abis_budgeting_year).FirstOrDefault();

                ViewBag.fld_GLCode = bgtincomebijibenih.Select(s => s.abis_gl_code).FirstOrDefault();
                ViewBag.fld_GLDesc = bgtincomebijibenih.Select(s => s.abis_gl_desc).FirstOrDefault();

                ViewBag.ProductCode = bgtincomebijibenih.Select(s => s.abis_product_code).FirstOrDefault();
                ViewBag.ProductName = bgtincomebijibenih.Select(s => s.abis_product_name).FirstOrDefault();

                var bgtincomedata = bgtincomebijibenih.GroupBy(g => new { g.abis_cost_center, g.abis_budgeting_year }).ToList();

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

                return PartialView(bgtincomebijibenih);
            }
        }

        //public ActionResult View(string costcenter, int budgetyear)
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

        //    //M.s
        //    int totalpage = 0;
        //    int page = 0;
        //    int pagesize = 0;
        //    int totalldata = 0;


        //    if (budgetyear >= 2024)
        //    {
        //        bgt_income_bijibenih[] bgt_income_bijibenihs = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter && x.abis_budgeting_year == budgetyear && x.fld_Deleted == false).ToArray();

        //        //Get Syarikat
        //        ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //        ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //        ViewBag.Message = @GlobalResEstate.msgNoRecord;

        //        //Get Budget Year
        //        ViewBag.BudgetYear = bgt_income_bijibenihs.Select(s => s.abis_budgeting_year).FirstOrDefault();

        //        //Get GL Pendapatan
        //        ViewBag.fld_GLCode = bgt_income_bijibenihs.Select(s => s.abis_gl_code).FirstOrDefault();
        //        ViewBag.fld_GLDesc = bgt_income_bijibenihs.Select(s => s.abis_gl_desc).FirstOrDefault();

        //        //Get Produk
        //        ViewBag.ProductCode = bgt_income_bijibenihs.Select(s => s.abis_product_code).FirstOrDefault();
        //        ViewBag.ProductName = bgt_income_bijibenihs.Select(s => s.abis_product_name).FirstOrDefault();

        //        var bgtincomedata = bgt_income_bijibenihs.GroupBy(g => new { g.abis_cost_center, g.abis_budgeting_year }).ToList();

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

        //        return PartialView(bgt_income_bijibenihs);
        //    }
        //    else
        //    {
        //        bgt_income_bijibenih[] bgtincomebijibenih = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter && x.abis_budgeting_year == budgetyear).ToArray();

        //        //Get Syarikat
        //        ViewBag.NamaSyarikat = "FGV Agri Services Sdn Bhd";
        //        ViewBag.NoSyarikat = "Company No. 353791-M";
        //        ViewBag.Message = @GlobalResEstate.msgNoRecord;

        //        //Get Budget Year
        //        ViewBag.BudgetYear = bgtincomebijibenih.Select(s => s.abis_budgeting_year).FirstOrDefault();

        //        //Get GL Pendapatan
        //        ViewBag.fld_GLCode = bgtincomebijibenih.Select(s => s.abis_gl_code).FirstOrDefault();
        //        ViewBag.fld_GLDesc = bgtincomebijibenih.Select(s => s.abis_gl_desc).FirstOrDefault();

        //        //Get Produk
        //        ViewBag.ProductCode = bgtincomebijibenih.Select(s => s.abis_product_code).FirstOrDefault();
        //        ViewBag.ProductName = bgtincomebijibenih.Select(s => s.abis_product_name).FirstOrDefault();

        //        //M.s
        //        //int totalpage = 0;
        //        //int page = 0;
        //        //int pagesize = 0;
        //        //int totalldata = 0;
        //        var bgtincomedata = bgtincomebijibenih.GroupBy(g => new { g.abis_cost_center, g.abis_budgeting_year }).ToList();

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

        //        return PartialView(bgtincomebijibenih);
        //    }
        //}
        public ActionResult Delete(string costcenter, string budgetyear)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
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

            bgt_income_bijibenih[] bgt_income_bijibenih = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter && x.abis_budgeting_year.ToString() == budgetyear && x.fld_Deleted == false).ToArray();

            var getYear = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter).Select(s => s.abis_budgeting_year).FirstOrDefault();
            var getPrdk = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter).Select(s => s.abis_product_name).FirstOrDefault();
            var getCosC = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter).Select(s => s.abis_cost_center).FirstOrDefault();
            var getCDes = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter).Select(s => s.abis_cost_center_desc).FirstOrDefault();
            var getStat = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter).Select(s => s.abis_station_name).FirstOrDefault();
            var getGLCo = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter).Select(s => s.abis_gl_code).FirstOrDefault();
            var getJuml = dbr.bgt_income_bijibenih.Where(x => x.abis_cost_center == costcenter).Select(s => s.abis_grand_total_amount).FirstOrDefault();

            ViewBag.getYear = getYear;
            ViewBag.getPrdk = getPrdk;
            ViewBag.getCosC = getCosC;
            ViewBag.getCDes = getCDes;
            ViewBag.getStat = getStat;
            ViewBag.getGLCo = getGLCo;
            ViewBag.getJuml = getJuml;

            return PartialView(bgt_income_bijibenih);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(bgt_income_bijibenih[] deletebgtbijibenih)
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
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
                foreach (var bis in deletebgtbijibenih)
                {
                    var getdata = dbr.bgt_income_bijibenih.Where(w => w.abis_id == bis.abis_id && w.abis_cost_center == bis.abis_cost_center && w.abis_budgeting_year == bis.abis_budgeting_year && w.fld_Deleted == false).FirstOrDefault();

                    if (getdata == null)
                    {
                        return Json(new { success = true, msg = GlobalResEstate.msgNoRecord, status = "alert", checkingdata = "0", method = "2", btn = "" });
                    }
                    else
                    {
                        //getdata.fld_Deleted = true;
                        //dbr.Entry(getdata).State = EntityState.Modified;
                        dbr.bgt_income_bijibenih.Remove(getdata);
                    }
                }
                foreach (var bis2 in deletebgtbijibenih)
                {
                    var getdata = dbr.bgt_income_bijibenih.Where(w => w.abis_cost_center == bis2.abis_cost_center && w.abis_budgeting_year == bis2.abis_budgeting_year && w.fld_Deleted == false).ToList();
                    foreach (var val in getdata)
                    {
                        var getdata2 = dbr.bgt_income_bijibenih.Where(w => w.abis_id == val.abis_id && w.abis_cost_center == bis2.abis_cost_center && w.abis_budgeting_year == bis2.abis_budgeting_year && w.fld_Deleted == false).FirstOrDefault();
                        //getdata2.fld_Deleted = true;
                        //dbr.Entry(getdata2).State = EntityState.Modified;
                        dbr.bgt_income_bijibenih.Remove(getdata2);
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
                    div = "searchResult",
                    rootUrl = domain,
                    action = "_Index",
                    controller = "PendapatanBijiBenih",
                    paramName = "BudgetYear",
                    paramValue = deletebgtbijibenih.FirstOrDefault().abis_budgeting_year
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
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomeBijiBenihList.rdlc");
            string filename = GetBudgetClass.GetScreenName("I2") + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeBijiBenihListViewModel> data = dbe.Database.SqlQuery<IncomeBijiBenihListViewModel>("exec sp_BudgetIncomeBijiBenihList {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintView(string BudgetYear, string CostCenter, string format = "PDF")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomeBijiBenihView.rdlc");
            string filename = GetBudgetClass.GetScreenName("I2") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeBijiBenihDetailViewModel> data = dbe.Database.SqlQuery<IncomeBijiBenihDetailViewModel>("exec sp_BudgetIncomeBijiBenihView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintViewExcel(string BudgetYear, string CostCenter, string format = "EXCEL")
        {
            var userid = User.Identity.Name;
            var RoleScreen = UserMatrix.RoleScreen(userid, "I2");
            if (!RoleScreen.download)
            {
                return RedirectToAction("Denied", "bgtAlert");
            }
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "IncomeBijiBenihViewExcel.rdlc");
            string filename = GetBudgetClass.GetScreenName("I2") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<IncomeBijiBenihDetailViewModel> data = dbe.Database.SqlQuery<IncomeBijiBenihDetailViewModel>("exec sp_BudgetIncomeBijiBenihView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        private List<IncomeBijiBenihListViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            if (BudgetYear >= 2024)
            {
                var query = dbe.bgt_income_bijibenih.Where(w => w.fld_Deleted == false)
                .GroupBy(g => new { g.abis_cost_center, g.abis_budgeting_year })
                .Select(g => new
                {
                    BudgetYear = g.FirstOrDefault().abis_budgeting_year,
                    CostCenterCode = g.FirstOrDefault().abis_cost_center,
                    CostCenterDesc = g.FirstOrDefault().abis_cost_center_desc,
                    JenisProduk = g.FirstOrDefault().abis_product_name,
                    Quantity1 = g.Sum(s => s.abis_grand_quantity1),
                    Quantity2 = g.Sum(s => s.abis_grand_quantity2),
                    TotalQuantity = g.Sum(s => s.abis_grand_total_quantity),
                    Amount1 = g.Sum(s => s.abis_grand_amount1),
                    Amount2 = g.Sum(s => s.abis_grand_amount2),
                    Total = g.Sum(s => s.abis_grand_total_amount)
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
                              select new IncomeBijiBenihListViewModel
                              {
                                  BudgetYear = (int)q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  JenisProduk = q.JenisProduk,
                                  Quantity1 = q.Quantity1,
                                  Quantity2 = q.Quantity2,
                                  AllQuantity = q.TotalQuantity,
                                  Amount1 = q.Amount1,
                                  Amount2 = q.Amount2,
                                  AllAmount = q.Total
                              }).Distinct();

                return query3.ToList();
            }
            else
            {
                var query = dbe.bgt_income_bijibenih
                .GroupBy(g => new { g.abis_cost_center, g.abis_budgeting_year })
                .Select(g => new
                {
                    BudgetYear = g.FirstOrDefault().abis_budgeting_year,
                    CostCenterCode = g.FirstOrDefault().abis_cost_center,
                    CostCenterDesc = g.FirstOrDefault().abis_cost_center_desc,
                    JenisProduk = g.FirstOrDefault().abis_product_name,
                    Quantity1 = g.Sum(s => s.abis_grand_quantity1),
                    Quantity2 = g.Sum(s => s.abis_grand_quantity2),
                    TotalQuantity = g.Sum(s => s.abis_grand_total_quantity),
                    Amount1 = g.Sum(s => s.abis_grand_amount1),
                    Amount2 = g.Sum(s => s.abis_grand_amount2),
                    Total = g.Sum(s => s.abis_grand_total_amount)
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
                              select new IncomeBijiBenihListViewModel
                              {
                                  BudgetYear = (int)q.BudgetYear,
                                  CostCenterCode = q.CostCenterCode,
                                  CostCenterDesc = q.CostCenterDesc,
                                  JenisProduk = q.JenisProduk,
                                  Quantity1 = q.Quantity1,
                                  Quantity2 = q.Quantity2,
                                  AllQuantity = q.TotalQuantity,
                                  Amount1 = q.Amount1,
                                  Amount2 = q.Amount2,
                                  AllAmount = q.Total
                              }).Distinct();

                return query3.ToList();
            }
        }
    }
}