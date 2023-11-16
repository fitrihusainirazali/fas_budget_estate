using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsBudget;
using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.App_LocalResources;
using System.Net;
using System.Data.Entity;
using System.IO;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class PendaftaranKenderaanController : Controller
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

        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }
        public JsonResult GetCostCenterDesc(string costcentercode)
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
                return Json(new 
                { 
                    success = true, 
                    //costcenter = CostCenterData.FirstOrDefault().fld_CostCenter, 
                    costcenterdesc = CostCenterData.FirstOrDefault().fld_CostCenterDesc
                });
            }
            return Json(new { success = false });
        }
        private List<tbl_SAPCCPUP> GetCostCenters(int budgetYear, bool exclude = true)
        {
            if (exclude)
            {
                var exp_verhicle = GetRecords(budgetYear);
                var excludeCostCenters = new List<string>();

                for (int i = 0; i < exp_verhicle.Count(); i++)
                {
                    excludeCostCenters.Add(exp_verhicle[i].abvr_cost_center_code);
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
        public JsonResult GetInfoParentKenderaan(string kod, string costcenter)
        {
            var SelectedYear = GetYears().FirstOrDefault().ToString();
            //GetIdentity GetIdentity = new GetIdentity();
            //int? getuserid = GetIdentity.ID(User.Identity.Name);
            //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var JenisKdrn = db.bgt_JenisKdrn.Where(x => x.ccmt_Lowest == true && x.ccmt_code == kod && x.ccmt_active == true);

            if (JenisKdrn.Count() > 0)
            {
                //var VehicleData = dbe.bgt_vehicle_register.Where(w => w.abvr_deleted == false && w.abvr_budgeting_year.ToString() == SelectedYear &&
                //                  w.abvr_cost_center_code.Contains(costcenter) && w.abvr_material_code.Contains(kod) &&
                //                  w.abvr_syarikat_id == SyarikatID && w.abvr_wilayah_id == WilayahID && w.abvr_ladang_id == LadangID).OrderByDescending(o => o.abvr_material_code).ToList();

                var VehicleData = dbe.bgt_vehicle_register
                        .Where(w => w.abvr_deleted == false && w.abvr_budgeting_year.ToString() == SelectedYear && w.abvr_cost_center_code.Contains(costcenter) && w.abvr_material_code.Contains(kod))
                        .OrderByDescending(o => o.abvr_material_code).ToList();

                if (VehicleData.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(kod))
                    {
                        var currentmaterialcode = VehicleData.FirstOrDefault().abvr_material_code;
                        string lastchar = currentmaterialcode.Substring(currentmaterialcode.Length - 1);
                        if (lastchar.Equals("B") || lastchar.Contains("B"))
                        {
                            string valnolastchar = currentmaterialcode.Remove(currentmaterialcode.Length - 1, 1);
                            var newcurrentmaterialcode = Convert.ToInt32(valnolastchar) + 1;
                            return Json(new
                            {
                                success = true,
                                KodMaterial = JenisKdrn.FirstOrDefault().ccmt_code,
                                NamaMaterial = JenisKdrn.FirstOrDefault().ccmt_name,
                                NewKodMaterial = newcurrentmaterialcode.ToString()
                            });
                        }
                        else
                        {
                            var newmaterialcode = Convert.ToInt32(currentmaterialcode) + 1;
                            return Json(new
                            {
                                success = true,
                                KodMaterial = JenisKdrn.FirstOrDefault().ccmt_code,
                                NamaMaterial = JenisKdrn.FirstOrDefault().ccmt_name,
                                NewKodMaterial = newmaterialcode.ToString()
                            });
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(kod))
                    {
                        var newmaterialcode = kod + "01";
                        return Json(new
                        {
                            success = true,
                            KodMaterial = JenisKdrn.FirstOrDefault().ccmt_code,
                            NamaMaterial = JenisKdrn.FirstOrDefault().ccmt_name,
                            NewKodMaterial = newmaterialcode.ToString()
                        });
                    }
                    else
                    {
                        var newmaterialcode = kod;
                        return Json(new
                        {
                            success = true,
                            KodMaterial = JenisKdrn.FirstOrDefault().ccmt_code,
                            NamaMaterial = JenisKdrn.FirstOrDefault().ccmt_name,
                            NewKodMaterial = newmaterialcode.ToString()
                        });
                    }
                }
            }
            return Json(new { success = false });
        }
        // GET: PendaftaranKenderaan
        public ActionResult Index()
        {
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

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<bgt_vehicle_register>();

            if (BudgetYear == "" || BudgetYear == null)
            {
                if (CostCenter == "" || CostCenter == null)
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => x.abvr_deleted == false)
                        .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g =>g.FirstOrDefault())
                            .OrderByDescending(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => x.abvr_deleted == false)
                        .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g => g.FirstOrDefault()).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                    .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g => g.FirstOrDefault())
                        .OrderByDescending(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                         .ToList();    

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                        .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g => g.FirstOrDefault()).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                if (CostCenter == "" || CostCenter == null)
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && x.abvr_deleted == false)
                        .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g => g.FirstOrDefault())
                        .OrderByDescending(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && x.abvr_deleted == false)
                        .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g => g.FirstOrDefault()).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                        .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g => g.FirstOrDefault())
                        .OrderByDescending(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                        .GroupBy(g => new { g.abvr_budgeting_year, g.abvr_cost_center_code }).Select(g => g.FirstOrDefault()).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        // GET: PendaftaranKenderaan/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            //var Year = GetYears().FirstOrDefault().ToString();

            //Get CC
            //var IncludeCC = dbr.bgt_vehicle_register
            //    .Where(w => w.abvr_budgeting_year.ToString() == Year && w.abvr_syarikat_id == SyarikatID && w.abvr_wilayah_id == WilayahID && w.abvr_ladang_id == LadangID && w.abvr_deleted == false)
            //    .Select(s => s.abvr_cost_center_code)
            //    .ToList();

            //List<SelectListItem> CostCenter = new List<SelectListItem>();
            //CostCenter = new SelectList(dbc.tbl_SAPCCPUP.Where(x => !IncludeCC.Contains(x.fld_CostCenter) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCenter).Select(s => new SelectListItem { Value = s.fld_CostCenter, Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc }).Distinct(), "Value", "Text").ToList();
            //CostCenter.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            //ViewBag.CostCenterList = CostCenter;

            //new sept
            var SelectedYear = GetYears().FirstOrDefault().ToString();
            var year = int.Parse(SelectedYear);

            var costCenters = new SelectList(GetCostCenters(year).Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter.TrimStart(new char[] { '0' }),
                Text = s.fld_CostCenter.TrimStart(new char[] { '0' }) + " - " + s.fld_CostCenterDesc
            }), "Value", "Text").ToList();

            ViewBag.CostCenterList = costCenters;

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
            return RedirectToAction("Update", new { BudgetYear = BudgetYear, CostCenter = CostCenter });
        }

        public ActionResult Update(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return RedirectToAction("Create");
            }

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //Parent Kenderaan
            List<SelectListItem> ParentKenderaan = new List<SelectListItem>();
            ParentKenderaan = new SelectList(db.bgt_JenisKdrn.Where(x => x.ccmt_Lowest == true && x.ccmt_active == true /*&& x.ccmt_code.Contains("1302")*/).OrderBy(o => o.ccmt_code).Select(s => new SelectListItem { Value = s.ccmt_code, Text = s.ccmt_code + " - " + s.ccmt_name }).Distinct(), "Value", "Text").ToList();
            ParentKenderaan.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            ViewBag.ParentKenderaanList = ParentKenderaan;

            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.COstCenterDesc = cc.GetCostCenterDesc(CostCenter);

            return View();
        }

        public ActionResult _RekodKenderaan(string BudgetYear, string CostCenter, int page = 1)
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
            var records = new ModelsBudget.PagedList<bgt_vehicle_register>();

            if (BudgetYear == "" || BudgetYear == null)
            {
                if (CostCenter == "" || CostCenter == null)
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => x.abvr_deleted == false)
                        .OrderBy(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code).ThenBy(o => o.abvr_material_code)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => x.abvr_deleted == false).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                        .OrderBy(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code).ThenBy(o => o.abvr_material_code)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                    .Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            else
            {
                if (CostCenter == "" || CostCenter == null)
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && x.abvr_deleted == false)
                        .OrderBy(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code).ThenBy(o => o.abvr_material_code)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && x.abvr_deleted == false)
                    .Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                        .OrderBy(o => o.abvr_budgeting_year).ThenBy(o => o.abvr_cost_center_code).ThenBy(o => o.abvr_material_code)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    records.TotalRecords = dbr.bgt_vehicle_register.Where(x => x.abvr_budgeting_year.ToString() == BudgetYear && (x.abvr_cost_center_code.Contains(CostCenter) || x.abvr_cost_center_desc.Contains(CostCenter)) && x.abvr_deleted == false)
                    .Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
            }
            return View(records);
        }

        public ActionResult AddDetails(string BudgetYear, string CostCenter)
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> ParentKenderaan = new List<SelectListItem>();
            //ParentKenderaan = new SelectList(db.bgt_JenisKdrn.Where(x => x.ccmt_active == true && x.ccmt_code.Contains("1302")).OrderBy(o => o.ccmt_code).Select(s => new SelectListItem { Value = s.ccmt_code, Text = s.ccmt_code + " - " + s.ccmt_name }).Distinct(), "Value", "Text").ToList();
            ParentKenderaan = new SelectList(db.bgt_JenisKdrn.Where(x => x.ccmt_Lowest == true && x.ccmt_active == true).OrderBy(o => o.ccmt_code).Select(s => new SelectListItem { Value = s.ccmt_code, Text = s.ccmt_code + " - " + s.ccmt_name }).Distinct(), "Value", "Text").ToList();
            //ParentKenderaan.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            ViewBag.ParentKenderaanList = ParentKenderaan;

            ////Get GL
            //var IncludeGL = dbr.bgt_income_product
            //    .Where(w => w.abip_budgeting_year.ToString() == BudgetYear && w.abip_cost_center == CostCenter && w.abip_ladang_id == LadangID && w.abip_syarikat_id == SyarikatID && w.abip_wilayah_id == WilayahID && w.fld_Deleted == false)
            //    .Select(s => s.abip_gl_code)
            //    .ToList();

            //var ID = GetBudgetClass.GetScreenID("I3");
            //var getAllID = db.bgt_ScreenGLs.Where(w => w.fld_ScrID.ToString() == ID).ToList();
            //List<Guid> GLID = new List<Guid>();
            //GLID = getAllID.Select(s => s.fld_GLID).ToList();

            //List<SelectListItem> gllist = new List<SelectListItem>();
            //gllist = new SelectList(dbc.tbl_SAPGLPUP.Where(x => GLID.Contains(x.fld_ID) && !IncludeGL.Contains(x.fld_GLCode) && x.fld_Deleted == false).OrderBy(o => o.fld_GLCode).Select(s => new SelectListItem { Value = s.fld_GLCode, Text = s.fld_GLDesc }), "Value", "Text").ToList();
            //gllist.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            //ViewBag.GLList = gllist;

            ViewBag.BudgetYear = BudgetYear;
            ViewBag.CostCenter = CostCenter;
            ViewBag.CostCenterDesc = cc.GetCostCenterDesc(CostCenter);
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDetails(bgt_vehicle_register model, string Additional_Code)
        {
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
                    var syarikatObj = syarikat.GetSyarikat();
                    var ladangObj = ladang.GetLadang();
                    var wilayahObj = wilayah.GetWilayah();

                    //foreach (var val in model)
                    //{
                    var bvr = new bgt_vehicle_register
                        {
                            abvr_revision = 0,
                            abvr_revision_history = false,
                            abvr_revision_status = "DRAFT",
                            abvr_budgeting_year = model.abvr_budgeting_year,
                            abvr_cost_center_code = model.abvr_cost_center_code,
                            abvr_cost_center_desc = model.abvr_cost_center_desc,
                            abvr_jenis_code = model.abvr_jenis_code,
                            abvr_jenis_name = model.abvr_jenis_name,
                            abvr_material_code = model.abvr_material_code + Additional_Code,
                            abvr_material_name = model.abvr_material_name,
                            abvr_model = model.abvr_model,
                            abvr_register_no = model.abvr_register_no,
                            abvr_status = model.abvr_status,
                            abvr_tahun_dibuat = model.abvr_tahun_dibuat,
                            last_modified = DateTime.Now,
                            abvr_deleted = false,
                            abvr_syarikat_id = SyarikatID,
                            abvr_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null,
                            abvr_wilayah_id = WilayahID,
                            abvr_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null,
                            abvr_ladang_id = LadangID,
                            abvr_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null,
                            abvr_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null,
                            abvr_created_by = getidentity.ID(User.Identity.Name)
                        };
                        dbr.bgt_vehicle_register.Add(bvr);
                    //}
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
                        //checkingdata = "0",
                        //method = "3",
                        div = "v",
                        rooturl = domain,
                        controller = "PendaftaranKenderaan", 
                        action = "_RekodKenderaan",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abvr_budgeting_year,
                        paramName2 = "CostCenter",
                        paramValue2 = model.abvr_cost_center_code
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
        

        public ActionResult UpdateRecord(int? id)
        {
            if (/*string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(CostCenter) ||*/ id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            //bgt_vehicle_register bvr = dbr.bgt_vehicle_register.Where(x => x.abvr_id == id && x.abvr_cost_center_code == CostCenter && x.abvr_budgeting_year.ToString() == BudgetYear && x.abvr_syarikat_id == SyarikatID && x.abvr_wilayah_id == WilayahID && x.abvr_ladang_id == LadangID && x.abvr_deleted == false).FirstOrDefault();

            var bvr = dbe.bgt_vehicle_register.Find(id);
            if (bvr == null)
            {
                return HttpNotFound();
            }

            //Parent Kenderaan
            List<SelectListItem> ParentKenderaan = new List<SelectListItem>();
            ParentKenderaan = new SelectList(db.bgt_JenisKdrn.Where(x => x.ccmt_Lowest == true && x.ccmt_active == true).OrderBy(o => o.ccmt_code).Select(s => new SelectListItem { Value = s.ccmt_code, Text = s.ccmt_code + " - " + s.ccmt_name }).Distinct(), "Value", "Text").ToList();
            ParentKenderaan.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            ViewBag.ParentKenderaanList = ParentKenderaan;

            ViewBag.BudgetYear = bvr.abvr_budgeting_year.ToString();
            ViewBag.CostCenter = bvr.abvr_cost_center_code;
            ViewBag.COstCenterDesc = cc.GetCostCenterDesc(bvr.abvr_cost_center_code);
            return View(bvr);
            //return PartialView("UpdateRecord", bvr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRecord(int? id, bgt_vehicle_register model/*, string AdditionalCode*/)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            int Year = DateTime.Now.Year + 1;

            if (ModelState.IsValid)
            {
                try
                {
                    var syarikatObj = syarikat.GetSyarikat();
                    var ladangObj = ladang.GetLadang();
                    var wilayahObj = wilayah.GetWilayah();
                    //foreach (var bip in model)
                    //{
                    //var getdata = dbr.bgt_vehicle_register.Where(w => w.abvr_id == model.abvr_id && w.abvr_cost_center_code.Contains(model.abvr_cost_center_code) && w.abvr_budgeting_year == model.abvr_budgeting_year
                    //&& w.abvr_syarikat_id == SyarikatID && w.abvr_wilayah_id == WilayahID && w.abvr_ladang_id == LadangID && w.abvr_deleted == false).FirstOrDefault();

                    var getdata = dbr.bgt_vehicle_register.Find(id);
                    getdata.abvr_revision = model.abvr_revision;
                    getdata.abvr_revision_history = model.abvr_revision_history;
                    getdata.abvr_budgeting_year = model.abvr_budgeting_year;
                    getdata.abvr_cost_center_code = model.abvr_cost_center_code;
                    getdata.abvr_cost_center_desc = model.abvr_cost_center_desc;
                    getdata.abvr_jenis_code = model.abvr_jenis_code;
                    getdata.abvr_jenis_name = model.abvr_jenis_name;
                    getdata.abvr_material_code = model.abvr_material_code/* + AdditionalCode*/;
                    getdata.abvr_material_name = model.abvr_material_name;
                    getdata.abvr_model = model.abvr_model;
                    getdata.abvr_register_no = model.abvr_register_no;
                    getdata.abvr_status = model.abvr_status;
                    getdata.abvr_tahun_dibuat = model.abvr_tahun_dibuat;
                    getdata.abvr_syarikat_id = SyarikatID;
                    getdata.abvr_syarikat_name = !string.IsNullOrEmpty(syarikatObj.fld_NamaSyarikat) ? syarikatObj.fld_NamaSyarikat : null;
                    getdata.abvr_wilayah_id = WilayahID;
                    getdata.abvr_wilayah_name = !string.IsNullOrEmpty(wilayahObj.fld_WlyhName) ? wilayahObj.fld_WlyhName : null;
                    getdata.abvr_ladang_id = LadangID;
                    getdata.abvr_ladang_code = !string.IsNullOrEmpty(ladangObj.fld_LdgCode) ? ladangObj.fld_LdgCode : null;
                    getdata.abvr_ladang_name = !string.IsNullOrEmpty(ladangObj.fld_LdgName) ? ladangObj.fld_LdgName : null;
                    getdata.abvr_created_by = getidentity.ID(User.Identity.Name);
                    getdata.abvr_deleted = model.abvr_deleted;
                    getdata.last_modified = DateTime.Now;
                    dbr.Entry(getdata).State = EntityState.Modified;
                    //}
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
                        div = "v",
                        rooturl = domain,
                        controller = "PendaftaranKenderaan",
                        action = "_RekodKenderaan",
                        paramName1 = "BudgetYear",
                        paramValue1 = model.abvr_budgeting_year,
                        paramName2 = "CostCenter",
                        paramValue2 = model.abvr_cost_center_code
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
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

        public ActionResult DeleteRecord(string CostCenter, string BudgetYear, int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            bgt_vehicle_register bvr = dbr.bgt_vehicle_register.Where(x => x.abvr_id == id && x.abvr_cost_center_code == CostCenter && x.abvr_budgeting_year.ToString() == BudgetYear && x.abvr_deleted == false).FirstOrDefault();

            return PartialView(bvr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecord(bgt_vehicle_register model)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            try
            {
                //foreach (var bis in deleterecordsgl)
                //{
                    var data = dbr.bgt_vehicle_register.Where(w => w.abvr_id == model.abvr_id && w.abvr_cost_center_code == model.abvr_cost_center_code && w.abvr_budgeting_year == model.abvr_budgeting_year && w.abvr_deleted == false).FirstOrDefault();

                    if (data == null)
                    {
                        return Json(new { success = true, msg = GlobalResEstate.msgNoRecord, status = "alert", checkingdata = "0", method = "2", btn = "" });
                    }
                    else
                    {
                        //datas.abvr_deleted = true;
                        //datas.last_modified = DateTime.Now;
                        //dbr.Entry(data).State = EntityState.Modified;
                        dbr.bgt_vehicle_register.Remove(data);
                    }
                //}
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
                    div = "vehicleregister",
                    rootUrl = domain,
                    action = "_RekodKenderaan",
                    controller = "PendaftaranKenderaan",
                    paramName = "BudgetYear",
                    paramValue = model.abvr_budgeting_year,
                    paramName2 = "CostCenter",
                    paramValue2 = model.abvr_cost_center_code   
                }); 
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult View(string CostCenter, int BudgetYear)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            bgt_vehicle_register[] bgt_vehicle_register = dbr.bgt_vehicle_register.Where(x => x.abvr_cost_center_code == CostCenter && x.abvr_budgeting_year == BudgetYear && x.abvr_deleted == false).ToArray();

            //Get Syarikat
            ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Message = @GlobalResEstate.msgNoRecord;

            ////Get Budget Year
            //ViewBag.BudgetYear = bgt_income_product.Select(s => s.abip_budgeting_year).FirstOrDefault();

            ////Get GL Pendapatan
            //ViewBag.fld_GLCode = bgt_income_product.Select(s => s.abip_gl_code).FirstOrDefault();
            //ViewBag.fld_GLDesc = bgt_income_product.Select(s => s.abip_gl_desc).FirstOrDefault();

            ////Get Produk
            //ViewBag.ProductCode = bgt_income_product.Select(s => s.abip_product_code).FirstOrDefault();
            //ViewBag.ProductName = bgt_income_product.Select(s => s.abip_product_name).FirstOrDefault();

            //M.s
            int totalpage = 0;
            int page = 0;
            int pagesize = 0;
            int totalldata = 0;
            var bgtincomedata = bgt_vehicle_register.GroupBy(g => new { g.abvr_cost_center_code, g.abvr_budgeting_year }).ToList();

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

            return View(bgt_vehicle_register);
        }

        // GET: PendaftaranKenderaan/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: PendaftaranKenderaan/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public ActionResult Delete(string CostCenter, string BudgetYear)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            bgt_vehicle_register[] bvr = dbr.bgt_vehicle_register.Where(x => x.abvr_cost_center_code == CostCenter && x.abvr_budgeting_year.ToString() == BudgetYear && x.abvr_deleted == false).ToArray();

            return PartialView(bvr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(bgt_vehicle_register[] bvr)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);
            try
            {
                foreach (var val in bvr)
                {
                    var getdata = dbr.bgt_vehicle_register.Where(w => w.abvr_id == val.abvr_id && w.abvr_cost_center_code == val.abvr_cost_center_code && w.abvr_budgeting_year == val.abvr_budgeting_year && w.abvr_deleted == false).FirstOrDefault();

                    if (getdata == null)
                    {
                        return Json(new { success = true, msg = GlobalResEstate.msgNoRecord, status = "alert", checkingdata = "0", method = "2", btn = "" });
                    }
                    else
                    {
                        //getdata.abvr_deleted = true;
                        //dbr.Entry(getdata).State = EntityState.Modified;
                        dbr.bgt_vehicle_register.Remove(getdata);
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
                    div = "vehicleregister",
                    rootUrl = domain,
                    action = "_Index",
                    controller = "PendaftaranKenderaan"
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
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "ExpensesVehicleList.rdlc");
            string filename = GetBudgetClass.GetScreenName("E13") + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesVehicleListViewModel> data = dbe.Database.SqlQuery<ExpensesVehicleListViewModel>("exec sp_BudgetExpensesVehicleRegisterList {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintViewPdf(string BudgetYear, string CostCenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "ExpensesVehicleView.rdlc");
            string filename = GetBudgetClass.GetScreenName("E13") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesVehicleDetailViewModel> data = dbe.Database.SqlQuery<ExpensesVehicleDetailViewModel>("exec sp_BudgetExpensesVehicleRegisterView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintViewExcel(string BudgetYear, string CostCenter, string format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "ExpensesVehicleViewExcel.rdlc");
            string filename = GetBudgetClass.GetScreenName("E13") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesVehicleDetailViewModel> data = dbe.Database.SqlQuery<ExpensesVehicleDetailViewModel>("exec sp_BudgetExpensesVehicleRegisterView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        private List<ExpensesVehicleDetailViewModel> GetRecords(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            var query = from p in dbe.bgt_vehicle_register
                        where p.abvr_deleted == false && p.abvr_syarikat_id == syarikatId && p.abvr_ladang_id == ladangId && p.abvr_wilayah_id == wilayahId
                        group p by new { p.abvr_budgeting_year, p.abvr_cost_center_code, p.abvr_cost_center_desc } into g
                        select new
                        {
                            BudgetYear = g.Key.abvr_budgeting_year,
                            CostCenterCode = g.Key.abvr_cost_center_code,
                            CostCenterDesc = g.Key.abvr_cost_center_desc
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
                          select new ExpensesVehicleDetailViewModel
                          {
                              abvr_budgeting_year = (int)q.BudgetYear,
                              abvr_cost_center_code = q.CostCenterCode,
                              abvr_cost_center_desc = q.CostCenterDesc
                          }).Distinct();

            return query3.ToList();
        }
    }
}
