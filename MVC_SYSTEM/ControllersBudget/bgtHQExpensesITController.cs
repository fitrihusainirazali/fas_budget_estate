
using Itenso.TimePeriod;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using System.Net;
using FGV.Prodata.Web.Exporter;
using FGV.Prodata.App;
using FGV.Prodata.Web.Mvc.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Threading.Tasks;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsBudgetView;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class bgtHQExpensesITController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_ModelsBudgetEst dbe = new ConnectionBudget().GetConnection();

        private GetConfig config = new GetConfig();
        private Screen scr = new Screen();
        private CostCenter cc = new CostCenter();
        private GL gl = new GL();
        private PrdAct_NC prdAct = new PrdAct_NC();
        private Station_NC station = new Station_NC();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private errorlog errlog = new errorlog();
        private readonly string screenCode = "E4";

        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetIdentity GetIdentity = new GetIdentity();
        Connection Connection = new Connection();
        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        public DateTime GetDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Singapore Standard Time");
            return _localTime;
        }

        // GET: bgtHQExpenses
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ButiranBelanjaIT(string BudgetYear = null)
        {
            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString(),
                Selected = !string.IsNullOrEmpty(BudgetYear) ? s.ToString().Contains(BudgetYear) : false
            }), "Value", "Text").ToList();
            var selectedYear = BudgetYear ?? GetYears().FirstOrDefault().ToString();

            ViewBag.Title = scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Years = years;
            ViewBag.SelectedYear = selectedYear;

            return View();
        }

        public ActionResult Records(string BudgetYear, int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<ExpensesHQListViewModel>
            {
                Content = GetRecords(budgetYear, page, pageSize),
                TotalRecords = GetRecords(budgetYear).Count,
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        // GET: ExpensesInsurance/Details
        public ActionResult Details(string BudgetYear, string CostCenter = null, string ProductActivity = null, string Station = null, string Views = null, string Option = null, string Version = "1")
        {
            if (string.IsNullOrEmpty(BudgetYear) || string.IsNullOrEmpty(Version))
            {
                return RedirectToAction("ButiranBelanjaIT", new { BudgetYear = BudgetYear });
            }

            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString(),
                Selected = !string.IsNullOrEmpty(BudgetYear) ? s.ToString().Contains(BudgetYear) : false
            }), "Value", "Text").ToList();
            var selectedYear = BudgetYear ?? GetYears().FirstOrDefault().ToString();

            var filterCostCenters = new SelectList(cc.GetCostCenters().Select(s => new SelectListItem
            {
                Value = s.fld_CostCenter,
                Text = s.fld_CostCenter + " - " + s.fld_CostCenterDesc,
                Selected = !string.IsNullOrEmpty(CostCenter) ? s.fld_CostCenter.Contains(CostCenter) : false
            }), "Value", "Text").ToList();

            var filterProductActivities = new SelectList(prdAct.GetPrdActs().Select(s => new SelectListItem
            {
                Value = s.Code_PP,
                Text = s.Code_PP + " - " + s.PrdAct_Desc,
                Selected = !string.IsNullOrEmpty(ProductActivity) ? s.Code_PP.Equals(ProductActivity) : false
            }), "Value", "Text").ToList();

            var filterStations = new SelectList(station.GetStations().Select(s => new SelectListItem
            {
                Value = s.Code_LL,
                Text = s.Code_LL + " - " + s.Station_Desc,
                Selected = !string.IsNullOrEmpty(Station) ? s.Code_LL.Equals(Station) : false
            }), "Value", "Text").ToList();

            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;
            int? version = !string.IsNullOrEmpty(Version) ? int.Parse(Version) : (int?)null;

            var screen = scr.GetScreen(screenCode);

            var costCenters = cc.GetCostCenters().DistinctBy(x => x.fld_CostCenter).ToList();
            if (!string.IsNullOrEmpty(CostCenter) || !string.IsNullOrEmpty(ProductActivity) || !string.IsNullOrEmpty(Station))
            {
                costCenters = cc.GetCostCenters(CostCenter, ProductActivity, Station);
            }

            var gls = gl.GetGLs(screen.ScrID);
            var expensesInsurances = GetRecordsDetail(budgetYear, CostCenter, version);

            ViewBag.Title = screen.ScrNameLongDesc;
            ViewBag.FilterYears = years;
            ViewBag.FilterCostCenters = filterCostCenters;
            ViewBag.FilterProductActivities = filterProductActivities;
            ViewBag.FilterStations = filterStations;
            ViewBag.FilterViews = !string.IsNullOrEmpty(Views) ? Views : string.Empty;
            ViewBag.FilterOption = !string.IsNullOrEmpty(Option) ? Option : string.Empty;

            ViewBag.BudgetYear = selectedYear;
            ViewBag.Version = Version;
            ViewBag.CostCenter = CostCenter;
            ViewBag.ProductActivity = ProductActivity;
            ViewBag.Station = Station;
            ViewBag.Views = Views;
            ViewBag.Option = Option;

            ViewBag.CostCenters = costCenters;
            ViewBag.GLs = gls;

            return View(expensesInsurances);
        }

        private List<ExpensesHQListViewModel> GetRecords(int? BudgetYear = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var wilayahId = wilayah.GetWilayahID() != 0 ? wilayah.GetWilayahID() : null;
            var ladangId = ladang.GetLadangID() != 0 ? ladang.GetLadangID() : null;

            var query = from i in dbe.bgt_expenses_IT
                        where !i.abet_Deleted
                        group i by new
                        {
                            i.abet_budgeting_year,
                            i.abet_createdby,
                            i.abet_version,
                            i.abet_SyarikatID,
                            i.abet_WilayahID,
                            i.abet_LadangID
                        } into g
                        select new
                        {
                            BudgetYear = g.Key.abet_budgeting_year,
                            UploadedBy = g.Key.abet_createdby,
                            Version = g.Key.abet_version,
                            SyarikatId = g.Key.abet_SyarikatID,
                            WilayahId = g.Key.abet_WilayahID,
                            LadangId = g.Key.abet_LadangID
                        };

            if (BudgetYear.HasValue)
            {
                query = query.Where(q => q.BudgetYear == BudgetYear);
                if (BudgetYear.Value > 2023)
                {
                    query = query.Where(q => q.SyarikatId == syarikatId && q.WilayahId == wilayahId && q.LadangId == ladangId);
                }
            }

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.BudgetYear)
                    .ThenByDescending(q => q.Version)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.BudgetYear)
                    .ThenByDescending(q => q.Version);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesHQListViewModel
                          {
                              BudgetYear = q.BudgetYear.Value,
                              UploadedBy = q.UploadedBy ?? string.Empty,
                              UploadedDate = q.UploadedBy != null && q.Version != null ? GetFirstUploadDate(q.BudgetYear.Value, q.UploadedBy, q.Version.Value).ToString("dd/MM/yyyy hh:mm:ss tt") : "", 
                              //GetFirstUploadDate(q.BudgetYear.Value, q.UploadedBy, q.Version.Value).ToString("dd/MM/yyyy hh:mm:ss tt"), //string.Format("{0:dd/MM/yyyy}", q.UploadedDate),
                              Version = q.Version ?? 0
                          }).Distinct();

            return query3.ToList();
        }

        private List<ExpensesHQListDetailViewModel> GetRecordsDetail(int? BudgetYear = null, string CostCenter = null, int? Version = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var wilayahId = wilayah.GetWilayahID();
            var ladangId = ladang.GetLadangID();

            var query = from i in dbe.bgt_expenses_IT
                        where !i.abet_Deleted
                        select i;

            if (BudgetYear.HasValue)
            {
                query = query.Where(q => q.abet_budgeting_year == BudgetYear);
                if (BudgetYear.Value > 2023)
                {
                    query = query.Where(q => q.abet_SyarikatID == syarikatId && q.abet_WilayahID == wilayahId && q.abet_LadangID == ladangId);
                }
            }
            if (!string.IsNullOrEmpty(CostCenter))
            {
                query = query.Where(q => q.abet_cost_center.Contains(CostCenter) || q.abet_cost_center_name.Contains(CostCenter));
            }
            if (Version.HasValue)
            {
                query = query.Where(q => q.abet_version == Version);
            }

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.abet_budgeting_year)
                    .ThenByDescending(q => q.abet_version)
                    .ThenBy(q => q.abet_cost_center)
                    .ThenBy(q => q.abet_gl_code)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.abet_budgeting_year)
                    .ThenByDescending(q => q.abet_version)
                    .ThenBy(q => q.abet_cost_center)
                    .ThenBy(q => q.abet_gl_code);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new ExpensesHQListDetailViewModel
                          {
                              Id = q.abet_id,
                              BudgetYear = q.abet_budgeting_year ?? DateTime.Now.ToLocalDateTime().Year,
                              CostCenterCode = q.abet_cost_center,
                              CostCenterDesc = q.abet_cost_center_name,
                              GLCode = q.abet_gl_code,
                              GLDesc = q.abet_gl_desc,
                              Month1 = q.abet_month_1 ?? 0,
                              Month2 = q.abet_month_2 ?? 0,
                              Month3 = q.abet_month_3 ?? 0,
                              Month4 = q.abet_month_4 ?? 0,
                              Month5 = q.abet_month_5 ?? 0,
                              Month6 = q.abet_month_6 ?? 0,
                              Month7 = q.abet_month_7 ?? 0,
                              Month8 = q.abet_month_8 ?? 0,
                              Month9 = q.abet_month_9 ?? 0,
                              Month10 = q.abet_month_10 ?? 0,
                              Month11 = q.abet_month_11 ?? 0,
                              Month12 = q.abet_month_12 ?? 0,
                              Total = q.abet_jumlah_keseluruhan ?? 0
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult DownloadTemplate(string year = null)
        {
            ExcelExporter excelExporter = new ExcelExporter();
            year = !string.IsNullOrEmpty(year) ? year : (DateTime.Now.Year + 1).ToString();

            try
            {
                var screenId = scr.GetScreen(screenCode).ScrID;
                var costCenters = cc.GetCostCenters().DistinctBy(x => x.fld_CostCenter).ToList();
                var gls = gl.GetGLs(screenId);
                var total_gl = gls.Count;

                var header1 = new List<string>();
                header1.Add(string.Empty);
                header1.Add(string.Empty); //"TEXT"
                foreach (var gl in gls)
                {
                    header1.Add(gl.fld_GLDesc);
                }

                var header2 = new List<string>();
                header2.Add("Bil");
                header2.Add("Cost Center");
                foreach (var gl in gls)
                {
                    header2.Add(gl.fld_GLCode);
                }

                var datas = new List<List<string>>();
                for (int i = 0; i < costCenters.Count; i++)
                {
                    var data = new List<string>();
                    data.Add((i + 1).ToString());
                    data.Add(costCenters[i].fld_CostCenter);
                    foreach (var gl in gls)
                    {
                        data.Add(string.Empty);
                    }

                    datas.Add(data);
                }

                //var hqExpXcel = db.bgt_HQExpXcels.Where(x => x.hqxc_ScrCode.Contains("EHQE3")).FirstOrDefault() ?? new bgt_HQExpXcel();

                var screenName = "IT";//hqExpXcel.hqxc_ScrName.Trim() ?? scr.GetScreen(screenCode).ScrName.Trim();
                var tabName = "IT"; // hqExpXcel.hqxc_TabName.Trim();
                var fileName = syarikat.GetSyarikat().fld_NamaPndkSyarikat + " Bajet HQ " + year + " (" + screenName + ") VER1 " + DateTime.Now.ToString("ddMMyy");

                excelExporter.ExportFromList(datas, header1, header2, tabName, fileName, ExcelExporter.ExcelFormat.Xls, false, null, syarikat.GetSyarikat().fld_NamaSyarikat.ToUpper(), "BAJET " + year, scr.GetScreen(screenCode).ScrNameLongDesc.ToUpper(), 6);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("ButiranBelanjaIT");
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file, string year = null)
        {
            var BudgetYear = !string.IsNullOrEmpty(year) ? int.Parse(year) : DateTime.Now.Year + 1;

            if (file != null && file.ContentLength > 0)
            {
                IWorkbook workbook;
                using (var stream = file.InputStream)
                {
                    if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        workbook = new XSSFWorkbook(stream); // For .xlsx files
                    }
                    else if (Path.GetExtension(file.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                    {
                        workbook = new HSSFWorkbook(stream); // For .xls files
                    }
                    else
                    {
                        TempData["SweetAlert"] = SweetAlert.SetAlert("Unsupported file format.", SweetAlert.SweetAlertType.Error);
                        return RedirectToAction("ButiranBelanjaIT", new { BudgetYear = BudgetYear });
                    }

                    try
                    {
                        ISheet sheet = workbook.GetSheetAt(0); // Assuming you want to read the first sheet

                        IRow headerRow = sheet.GetRow(5);
                        if (headerRow != null)
                        {
                            string[] columnNames = new string[headerRow.LastCellNum];
                            for (int cellIndex = 0; cellIndex < headerRow.LastCellNum; cellIndex++)
                            {
                                ICell cell = headerRow.GetCell(cellIndex);
                                columnNames[cellIndex] = cell.ToString();
                            }

                            var list_old = dbe.bgt_expenses_IT
                                .Where(i => i.abet_budgeting_year == BudgetYear)
                                .GroupBy(i => i.abet_budgeting_year)
                                .Select(g => g.Key)
                                .ToList();
                            var version = list_old.Count + 1;

                            int skipRows = 6; // Number of rows to skip
                            for (int rowNumber = skipRows; rowNumber <= sheet.LastRowNum; rowNumber++)
                            {
                                IRow row = sheet.GetRow(rowNumber);

                                if (row != null)
                                {
                                    ICell cellCC = row.GetCell(1);
                                    var ccCode = cellCC.ToString();

                                    for (int cellIndex = 2; cellIndex < row.LastCellNum; cellIndex++)
                                    {
                                        ICell cell = row.GetCell(cellIndex);
                                        if (cell != null)
                                        {
                                            var glCode = columnNames[cellIndex];
                                            var resultAmt = 0m;

                                            var syarikatObj = syarikat.GetSyarikat();
                                            var wilayahObj = wilayah.GetWilayah();
                                            var ladangObj = ladang.GetLadang();

                                            var totalAmount = !string.IsNullOrEmpty(cell.ToString()) && decimal.TryParse(cell.ToString().Replace(",", ""), out resultAmt)
                                                    ? decimal.Parse(cell.ToString().Replace(",", ""))
                                                    : 0;
                                            var monthlyAmount = Math.Round(totalAmount / 12, 2);
                                            var monthLastAmount = totalAmount - monthlyAmount * 11;

                                            if (!string.IsNullOrEmpty(ccCode) && !string.IsNullOrEmpty(glCode))
                                            {
                                                var expenses_hq = new bgt_expenses_IT
                                                {
                                                    abet_budgeting_year = BudgetYear,
                                                    abet_cost_center = ccCode,
                                                    abet_cost_center_name = cc.GetCostCenterDesc(ccCode),
                                                    abet_gl_code = glCode,
                                                    abet_gl_desc = gl.GetGLDesc(glCode),
                                                    abet_proration = "monthly",
                                                    abet_jumlah_setahun = totalAmount,
                                                    abet_month_1 = monthlyAmount,
                                                    abet_month_2 = monthlyAmount,
                                                    abet_month_3 = monthlyAmount,
                                                    abet_month_4 = monthlyAmount,
                                                    abet_month_5 = monthlyAmount,
                                                    abet_month_6 = monthlyAmount,
                                                    abet_month_7 = monthlyAmount,
                                                    abet_month_8 = monthlyAmount,
                                                    abet_month_9 = monthlyAmount,
                                                    abet_month_10 = monthlyAmount,
                                                    abet_month_11 = monthlyAmount,
                                                    abet_month_12 = monthLastAmount,
                                                    abet_jumlah_RM = totalAmount,
                                                    abet_jumlah_keseluruhan = totalAmount,
                                                    last_modified = DateTime.Now,
                                                    abet_NegaraID = 1,
                                                    abet_SyarikatID = syarikatObj.fld_SyarikatID != 0 ? syarikatObj.fld_SyarikatID : (int?)null,
                                                    abet_WilayahID = wilayahObj.fld_ID != 0 ? wilayahObj.fld_ID : (int?)null,
                                                    abet_LadangID = ladangObj.fld_ID != 0 ? ladangObj.fld_ID : (int?)null,
                                                    abet_revision = 0,
                                                    abet_status = "DRAFT",
                                                    abet_history = false,
                                                    abet_createdby = User.Identity.Name,
                                                    abet_upload_date = DateTime.Now.ToLocalDateTime(),
                                                    abet_version = version
                                                };
                                                dbe.bgt_expenses_IT.Add(expenses_hq);
                                            }
                                        }
                                    }
                                }
                            }
                            await dbe.SaveChangesAsync();
                        }
                        TempData["SweetAlert"] = SweetAlert.SetAlert("File has been uploaded successfully", SweetAlert.SweetAlertType.Success);
                    }
                    catch (Exception ex)
                    {
                        errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                        TempData["SweetAlert"] = SweetAlert.SetAlert("Error", SweetAlert.SweetAlertType.Error);
                    }
                }
            }
            else
            {
                TempData["SweetAlert"] = SweetAlert.SetAlert("No file.", SweetAlert.SweetAlertType.Error);
            }

            return RedirectToAction("ButiranBelanjaIT", new { BudgetYear = BudgetYear });
        }

        public ActionResult Download(string BudgetYear, string Version)
        {
            ExcelExporter excelExporter = new ExcelExporter();

            try
            {
                var screenId = scr.GetScreen(screenCode).ScrID;
                var costCenters = cc.GetCostCenters().DistinctBy(x => x.fld_CostCenter).ToList();
                var gls = gl.GetGLs(screenId);
                var total_gl = gls.Count;

                var header1 = new List<string>();
                header1.Add(string.Empty);
                header1.Add(string.Empty); //"TEXT"
                foreach (var gl in gls)
                {
                    header1.Add(gl.fld_GLDesc);
                }

                var header2 = new List<string>();
                header2.Add("Bil");
                header2.Add("Cost Center");
                foreach (var gl in gls)
                {
                    header2.Add(gl.fld_GLCode);
                }

                var datas = new List<List<string>>();
                for (int i = 0; i < costCenters.Count; i++)
                {
                    var data = new List<string>();
                    data.Add((i + 1).ToString());
                    data.Add(costCenters[i].fld_CostCenter);
                    foreach (var gl in gls)
                    {
                        var value = GetDataValue(int.Parse(BudgetYear), int.Parse(Version), costCenters[i].fld_CostCenter, gl.fld_GLCode);
                        data.Add(value.ToString("0.00"));
                    }

                    datas.Add(data);
                }

                //var hqExpXcel = db.bgt_HQExpXcels.Where(x => x.hqxc_ScrCode.Contains("EHQE3")).FirstOrDefault() ?? new bgt_HQExpXcel();

                var screenName = "IT"; // hqExpXcel.hqxc_ScrName.Trim() ?? scr.GetScreen(screenCode).ScrName.Trim();
                var tabName = "IT";// hqExpXcel.hqxc_TabName.Trim();
                var fileName = syarikat.GetSyarikat().fld_NamaPndkSyarikat + " Bajet HQ " + BudgetYear + " (" + screenName + ") VER" + Version + " " + DateTime.Now.ToString("ddMMyy");

                excelExporter.ExportFromList(datas, header1, header2, tabName, fileName, ExcelExporter.ExcelFormat.Xls, false, null, syarikat.GetSyarikat().fld_NamaSyarikat.ToUpper(), "BAJET " + BudgetYear, scr.GetScreen(screenCode).ScrNameLongDesc.ToUpper(), 6);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("ButiranBelanjaIT", new { BudgetYear = BudgetYear });
        }

        public ActionResult ExportList(int BudgetYear, string CostCenter, int Version, string ProductActivity, string Station, string Views, string Option, string format = "PDF")
        {

            LocalReport lr = new LocalReport();
            string path = "";
            string title = scr.GetScreen(screenCode).ScrNameLongDesc;
            string filename = "";

            List<ExpensesHQListDetailViewModel> data = this.GetRecordsDetail(BudgetYear, CostCenter, Version, null, null);
            List<bgt_expenses_ITView> data2 = new List<bgt_expenses_ITView> { };

            decimal value_per_month = 0;
            decimal value_last_month = 0;

            if (Views == "monthly") {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "HqExpIT_detail.rdlc");
                filename = title + " - Monthly";

                data.ForEach(x => {
                    var expenses_hq = new bgt_expenses_ITView
                    {
                        abet_budgeting_year = x.BudgetYear,
                        abet_cost_center = x.CostCenterCode,
                        abet_cost_center_name = x.CostCenterDesc,
                        abet_gl_code = x.GLCode,
                        abet_gl_desc = x.GLDesc,
                        abet_month_1 = x.Month1,
                        abet_month_2 = x.Month2,
                        abet_month_3 = x.Month3,
                        abet_month_4 = x.Month4,
                        abet_month_5 = x.Month5,
                        abet_month_6 = x.Month6,
                        abet_month_7 = x.Month7,
                        abet_month_8 = x.Month8,
                        abet_month_9 = x.Month9,
                        abet_month_10 = x.Month10,
                        abet_month_11 = x.Month11,
                        abet_month_12 = x.Month12,
                        abet_jumlah_keseluruhan = x.Total,
                    };
                    data2.Add(expenses_hq);
                });
            }
            else if (Views == "quarterly") {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "HqExpIT_detail.rdlc");
                filename = title + " - Quarterly";

                data.ForEach(x => {
                    value_per_month = Math.Round(x.Total / 4, 2);
                    value_last_month = x.Total - value_per_month * 3;
                    var expenses_hq = new bgt_expenses_ITView
                    {
                        abet_budgeting_year = x.BudgetYear,
                        abet_cost_center = x.CostCenterCode,
                        abet_cost_center_name = x.CostCenterDesc,
                        abet_gl_code = x.GLCode,
                        abet_gl_desc = x.GLDesc,
                        abet_month_1 = 0,
                        abet_month_2 = 0,
                        abet_month_3 = value_per_month,
                        abet_month_4 = 0,
                        abet_month_5 = 0,
                        abet_month_6 = value_per_month,
                        abet_month_7 = 0,
                        abet_month_8 = 0,
                        abet_month_9 = value_per_month,
                        abet_month_10 = 0,
                        abet_month_11 = 0,
                        abet_month_12 = value_last_month,
                        abet_jumlah_keseluruhan = x.Total,
                    };
                    data2.Add(expenses_hq);
                });

            }
            else if (Views == "semiannually")
            {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "HqExpIT_detail.rdlc");
                filename = title + " - Semi Anually";

                data.ForEach(x => {
                    value_per_month = Math.Round(x.Total / 2, 2);
                    value_last_month = x.Total - value_per_month * 1;
                    var expenses_hq = new bgt_expenses_ITView
                    {
                        abet_budgeting_year = x.BudgetYear,
                        abet_cost_center = x.CostCenterCode,
                        abet_cost_center_name = x.CostCenterDesc,
                        abet_gl_code = x.GLCode,
                        abet_gl_desc = x.GLDesc,
                        abet_month_1 = 0,
                        abet_month_2 = 0,
                        abet_month_3 = 0,
                        abet_month_4 = 0,
                        abet_month_5 = 0,
                        abet_month_6 = value_per_month,
                        abet_month_7 = 0,
                        abet_month_8 = 0,
                        abet_month_9 = 0,
                        abet_month_10 = 0,
                        abet_month_11 = 0,
                        abet_month_12 = value_last_month,
                        abet_jumlah_keseluruhan = x.Total,
                    };
                    data2.Add(expenses_hq);
                });
            }
            else if (Views == "annually")
            {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "HqExpIT_summary.rdlc");
                filename = title + " - Anually";

                data.ForEach(x => {
                    var expenses_hq = new bgt_expenses_ITView
                    {
                        abet_budgeting_year = x.BudgetYear,
                        abet_cost_center = x.CostCenterCode,
                        abet_cost_center_name = x.CostCenterDesc,
                        abet_gl_code = x.GLCode,
                        abet_gl_desc = x.GLDesc,
                        abet_month_1 = x.Month1,
                        abet_month_2 = x.Month2,
                        abet_month_3 = x.Month3,
                        abet_month_4 = x.Month4,
                        abet_month_5 = x.Month5,
                        abet_month_6 = x.Month6,
                        abet_month_7 = x.Month7,
                        abet_month_8 = x.Month8,
                        abet_month_9 = x.Month9,
                        abet_month_10 = x.Month10,
                        abet_month_11 = x.Month11,
                        abet_month_12 = x.Month12,
                        abet_jumlah_keseluruhan = x.Total,
                    };
                    data2.Add(expenses_hq);
                });
            }
            else if (Option == "details")
            {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "HqExpIT_detail.rdlc");
                filename = title + " - Detail";

                data.ForEach(x => {
                    var expenses_hq = new bgt_expenses_ITView
                    {
                        abet_budgeting_year = x.BudgetYear,
                        abet_cost_center = x.CostCenterCode,
                        abet_cost_center_name = x.CostCenterDesc,
                        abet_gl_code = x.GLCode,
                        abet_gl_desc = x.GLDesc,
                        abet_month_1 = x.Month1,
                        abet_month_2 = x.Month2,
                        abet_month_3 = x.Month3,
                        abet_month_4 = x.Month4,
                        abet_month_5 = x.Month5,
                        abet_month_6 = x.Month6,
                        abet_month_7 = x.Month7,
                        abet_month_8 = x.Month8,
                        abet_month_9 = x.Month9,
                        abet_month_10 = x.Month10,
                        abet_month_11 = x.Month11,
                        abet_month_12 = x.Month12,
                        abet_jumlah_keseluruhan = x.Total,
                    };
                    data2.Add(expenses_hq);
                });
            }
            else {
                path = Path.Combine(Server.MapPath("~/ReportsBudget"), "HqExpIT_summary.rdlc");
                filename = title + " - Summary";

                data.ForEach(x => {
                    var expenses_hq = new bgt_expenses_ITView
                    {
                        abet_budgeting_year = x.BudgetYear,
                        abet_cost_center = x.CostCenterCode,
                        abet_cost_center_name = x.CostCenterDesc,
                        abet_gl_code = x.GLCode,
                        abet_gl_desc = x.GLDesc,
                        abet_month_1 = x.Month1,
                        abet_month_2 = x.Month2,
                        abet_month_3 = x.Month3,
                        abet_month_4 = x.Month4,
                        abet_month_5 = x.Month5,
                        abet_month_6 = x.Month6,
                        abet_month_7 = x.Month7,
                        abet_month_8 = x.Month8,
                        abet_month_9 = x.Month9,
                        abet_month_10 = x.Month10,
                        abet_month_11 = x.Month11,
                        abet_month_12 = x.Month12,
                        abet_jumlah_keseluruhan = x.Total,
                    };
                    data2.Add(expenses_hq);
                });
            }


            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            ReportDataSource rd = new ReportDataSource("DataSet1", data2);
            lr.DataSources.Add(rd);

            List<ReportParameter> paramList = new List<ReportParameter>();
            paramList.Add(new ReportParameter("titleParam", title));
            paramList.Add(new ReportParameter("costcenterParam", CostCenter ?? "All"));
            lr.SetParameters(paramList);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintExcelList(string year, string costcenter, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "HqExpIT_detail.rdlc");
            string title = scr.GetScreen(screenCode).ScrNameLongDesc;
            string filename = title + " - Detail";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesHQListDetailViewModel> data = this.GetRecordsDetail(2024, null, 2, null, null);

            List<bgt_expenses_ITView> data2 = new List<bgt_expenses_ITView> { };

            data.ForEach(x => {
                var expenses_hq = new bgt_expenses_ITView
                {
                    abet_budgeting_year = x.BudgetYear,
                    abet_cost_center = x.CostCenterCode,
                    abet_cost_center_name = x.CostCenterDesc,
                    abet_gl_code = x.GLCode,
                    abet_gl_desc = x.GLDesc,
                    abet_month_1 = x.Month1,
                    abet_month_2 = x.Month2,
                    abet_month_3 = x.Month3,
                    abet_month_4 = x.Month4,
                    abet_month_5 = x.Month5,
                    abet_month_6 = x.Month6,
                    abet_month_7 = x.Month7,
                    abet_month_8 = x.Month8,
                    abet_month_9 = x.Month9,
                    abet_month_10 = x.Month10,
                    abet_month_11 = x.Month11,
                    abet_month_12 = x.Month12,
                    abet_jumlah_keseluruhan = x.Total,
                };
                data2.Add(expenses_hq);
            });

            ReportDataSource rd = new ReportDataSource("DataSet1", data2);
            lr.DataSources.Add(rd);

            List<ReportParameter> paramList = new List<ReportParameter>();
            paramList.Add(new ReportParameter("titleParam", title));
            lr.SetParameters(paramList);

            return Reporting.DownloadReport(lr, Format, Reporting.PageOrientation.Landscape, filename);
        }

        private DateTime GetFirstUploadDate(int year, string uploadedBy, int version)
        {
            var data = dbe.bgt_expenses_IT
                .Where(x => x.abet_budgeting_year == year)
                .Where(x => x.abet_createdby.Contains(uploadedBy))
                .Where(x => x.abet_version == version)
                .FirstOrDefault();
            if (data != null)
            {
                return data.abet_upload_date ?? DateTime.Now;
            }
            return DateTime.Now;
        }

        private decimal GetDataValue(int year, int version, string costCenterCode, string glCode)
        {
            var data = dbe.bgt_expenses_IT
                .Where(x => x.abet_budgeting_year == year)
                .Where(x => x.abet_version == version)
                .Where(x => x.abet_cost_center.Contains(costCenterCode))
                .Where(x => x.abet_gl_code.Contains(glCode))
                .FirstOrDefault();
            if (data != null)
            {
                return data.abet_jumlah_keseluruhan ?? 0;
            }
            return 0;
        }

        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}