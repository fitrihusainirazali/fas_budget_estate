using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.ModelsBudget.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FGV.Prodata.Web.Exporter;
using FGV.Prodata.App;
using FGV.Prodata.Web.Mvc.UI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsBudgetView;
using System.Data.Entity;
using Ganss.Excel;
using Microsoft.AspNet.Identity;
using System.Windows.Forms;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class bgtIntegrationController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_ModelsBudgetEst dbe = new ConnectionBudget().GetConnection();

        private GetConfig config = new GetConfig();
        //private Screen scr = new Screen();
        private CostCenter cc = new CostCenter();
        private GL gl = new GL();
        private PrdAct_NC prdAct = new PrdAct_NC();
        private Station_NC station = new Station_NC();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();
        private errorlog errlog = new errorlog();
        private readonly string screenCode = "E44"; //ScreenCode for Integration is not available yet

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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IntegrationDownload(string BudgetYear = null)
        {
            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString(),
                Selected = !string.IsNullOrEmpty(BudgetYear) ? s.ToString().Contains(BudgetYear) : false
            }), "Value", "Text").ToList();
            var selectedYear = BudgetYear ?? GetYears().FirstOrDefault().ToString();

            ViewBag.Title = "Integration - Download";//scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Years = years;
            ViewBag.SelectedYear = selectedYear;

            return View();
        }

        public ActionResult IntegrationUpload(string BudgetYear = null)
        {
            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString(),
                Selected = !string.IsNullOrEmpty(BudgetYear) ? s.ToString().Contains(BudgetYear) : false
            }), "Value", "Text").ToList();
            var selectedYear = BudgetYear ?? GetYears().FirstOrDefault().ToString();

            ViewBag.Title = "Integration - Upload Actual Figure";//scr.GetScreen(screenCode).ScrNameLongDesc;
            ViewBag.Years = years;
            ViewBag.SelectedYear = selectedYear;

            return View();
        }

        public ActionResult Records(string BudgetYear, int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<IntegrationListViewModel>
            {
                Content = GetRecords(budgetYear, page, pageSize),
                TotalRecords = GetRecords(budgetYear).Count,
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_Records", records);
        }

        public ActionResult UploadFigureRecords(string BudgetYear, int page = 1)
        {
            int pageSize = int.Parse(config.GetData("paging"));
            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var records = new ViewingModels.PagedList<IntegrationListViewModel>
            {
                Content = GetFigureRecords(budgetYear, page, pageSize),
                TotalRecords = GetFigureRecords(budgetYear).Count,
                CurrentPage = page,
                PageSize = pageSize
            };

            return PartialView("_UploadFigureRecords", records);
        }

        public ActionResult DetailsFigure(string BudgetYear)
        {
            if (string.IsNullOrEmpty(BudgetYear))
            {
                return RedirectToAction("IntegrationUpload", new { BudgetYear = BudgetYear });
            }

            var years = new SelectList(GetYears().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString(),
                Selected = !string.IsNullOrEmpty(BudgetYear) ? s.ToString().Contains(BudgetYear) : false
            }), "Value", "Text").ToList();
            var selectedYear = BudgetYear ?? GetYears().FirstOrDefault().ToString();

            int? budgetYear = !string.IsNullOrEmpty(BudgetYear) ? int.Parse(BudgetYear) : (int?)null;

            var figureData = db.bgt_Actual_Figure.Where(x => x.actu_Year == budgetYear).ToList();
            List<IntegrationUploadFigureListModel> tData = new List<IntegrationUploadFigureListModel>() { };
            figureData.ForEach(x => {
                tData.Add(new IntegrationUploadFigureListModel {
                    actu_Year = x.actu_Year,
                    actu_GLCode = x.actu_GLCode,
                    actu_CostCenter = x.actu_CostCenter,
                    actu_ActualAmt = x.actu_ActualAmt ?? 0,
                });
            });

            ViewBag.Title = "Integration - Upload Actual Figure (Detail)";
            ViewBag.BudgetYear = selectedYear;
            ViewBag.FilterYears = years;

            return View(tData);
        }

        private List<IntegrationListViewModel> GetRecords(int? BudgetYear = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var wilayahId = wilayah.GetWilayahID() != 0 ? wilayah.GetWilayahID() : null;
            var ladangId = ladang.GetLadangID() != 0 ? ladang.GetLadangID() : null;

            var query = from i in dbe.bgt_Log_Audit_Trail select i;

            if (BudgetYear.HasValue)
            {
                query = query.Where(q => q.alat_budgeting_year == BudgetYear);
            }

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.alat_budgeting_year)
                    .ThenByDescending(q => q.alat_last_access_time)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.alat_budgeting_year)
                    .ThenByDescending(q => q.alat_last_access_time);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new IntegrationListViewModel
                          {
                              BudgetYear = q.alat_budgeting_year,
                              DownloadBy = q.alat_preparer_name ?? string.Empty,
                              DownloadDate = (q.alat_last_access_time ?? GetDateTime()).ToLocalDateTime().ToString("dd/MM/yyyy hh:mm:ss tt"), //q.UploadedBy != null && q.Version != null ? GetFirstUploadDate(q.BudgetYear.Value, q.UploadedBy, q.Version.Value).ToString("dd/MM/yyyy hh:mm:ss tt") : "",
                          }).Distinct();

            return query3.ToList();
        }

        private List<IntegrationListViewModel> GetFigureRecords(int? BudgetYear = null, int? Page = null, int? PageSize = null)
        {
            var syarikatId = syarikat.GetSyarikatID();
            var wilayahId = wilayah.GetWilayahID() != 0 ? wilayah.GetWilayahID() : null;
            var ladangId = ladang.GetLadangID() != 0 ? ladang.GetLadangID() : null;

            var query = from i in db.bgt_log_Actual_Figure select i;

            if (BudgetYear.HasValue)
            {
                query = query.Where(q => q.actu_Year == BudgetYear);
            }

            var query2 = query.AsQueryable();

            if (Page.HasValue && PageSize.HasValue)
            {
                query2 = query2
                    .OrderByDescending(q => q.actu_Year)
                    .ThenByDescending(q => q.actu_modified)
                    .Skip((Page.Value - 1) * PageSize.Value)
                    .Take(PageSize.Value);
            }
            else
            {
                query2 = query2
                    .OrderByDescending(q => q.actu_Year)
                    .ThenByDescending(q => q.actu_modified);
            }

            var query3 = (from q in query2.AsEnumerable()
                          select new IntegrationListViewModel
                          {
                              BudgetYear = q.actu_Year,
                              DownloadBy = q.actu_modifiedByID ?? string.Empty,
                              DownloadDate = (q.actu_modified ?? GetDateTime()).ToLocalDateTime().ToString("dd/MM/yyyy hh:mm:ss tt"), //q.UploadedBy != null && q.Version != null ? GetFirstUploadDate(q.BudgetYear.Value, q.UploadedBy, q.Version.Value).ToString("dd/MM/yyyy hh:mm:ss tt") : "",
                          }).Distinct();

            return query3.ToList();
        }

        public ActionResult DownloadExcel(int? BudgetYear = null)
        {

            List<IntegrationListDetailViewModel> integrateData = this.getAllRecord(BudgetYear).OrderBy(x => x.cost_center_code).ThenBy(x2 => x2.gl_code).ToList();
            var currYear = GetDateTime().Year;
            int getuserid = getidentity.ID(User.Identity.Name);

            if (integrateData.Count() == 0)
            {
                TempData["SweetAlert"] = SweetAlert.SetAlert("No Record Found.", SweetAlert.SweetAlertType.Info);
            }
            else {

                //byte[] stream;
                //string fileName = "Integration Data " + BudgetYear.ToString();

                //string tabName = "Sheet1";
                //var newFile = Server.MapPath("~/" + fileName + ".xlsx");
                //new ExcelMapper().Save(newFile, integrateData, tabName, true);
                //stream = System.IO.File.ReadAllBytes(newFile);

                //return (File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx"));

                string tabName = "Sheet1";
                string fileName = "Integration Data " + BudgetYear.ToString();

                MemoryStream memoryStream = new MemoryStream();
                new ExcelMapper().Save(memoryStream, integrateData, tabName, true);

                HttpResponse response = HttpContext.ApplicationInstance.Response;
                response.Clear();

                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                response.BinaryWrite(memoryStream.ToArray());

                response.End();
            }

            return RedirectToAction("IntegrationDownload", new { BudgetYear = BudgetYear });
        }

        //UploadExcel
        [HttpPost]
        public async Task<ActionResult> UploadExcel(HttpPostedFileBase file, int? BudgetYear = null)
        {
            var currYear = GetDateTime().Year;
            int getuserid = getidentity.ID(User.Identity.Name);
            string userName = User.Identity.Name;
            var currDateTime = this.GetDateTime();

            if (file != null && file.ContentLength > 0)
            {
                using (var stream = file.InputStream)
                {
                    if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        //workbook = new XSSFWorkbook(stream); // For .xlsx files
                    }
                    else if (Path.GetExtension(file.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                    {
                        //workbook = new HSSFWorkbook(stream); // For .xls files
                    }
                    else
                    {
                        TempData["SweetAlert"] = SweetAlert.SetAlert("Unsupported file format.", SweetAlert.SweetAlertType.Error);
                        return RedirectToAction("ButiranBelanjaIT", new { BudgetYear = BudgetYear });
                    }

                    try {
                        var excelData = new ExcelMapper(stream) { MinRowNumber = 1 }.Fetch<IntegrationUploadFigureModel>();
                        int? count = db.bgt_Actual_Figure.Max(x => x.actu_ID);

                        excelData.ForEach(x =>
                        {
                            count = count + 1;
                            var newFigureData = new bgt_Actual_Figure
                            {
                                actu_ID = count ?? 0,
                                actu_Year = x.actu_Year,
                                actu_GLCode = x.actu_GLCode,
                                actu_CostCenter = x.actu_CostCenter,
                                actu_ActualAmt = x.actu_ActualAmt,
                                actu_modified = currDateTime,
                            };
                            db.bgt_Actual_Figure.Add(newFigureData);
                        });

                        var newAuditLog = new bgt_log_Actual_Figure
                        {
                            actu_Year = BudgetYear ?? currYear,
                            actu_modified = currDateTime,
                            actu_modifiedByID = userName,
                            actu_SyarikatID = 0,
                        };
                        db.bgt_log_Actual_Figure.Add(newAuditLog);

                        await db.SaveChangesAsync();

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

            return RedirectToAction("IntegrationUpload", new { BudgetYear = BudgetYear });
        }

        public ActionResult UpdateAuditLog(int? BudgetYear = null)
        {

            List<IntegrationListDetailViewModel> integrateData = this.getAllRecord(BudgetYear).OrderBy(x => x.cost_center_code).ThenBy(x2 => x2.gl_code).ToList();
            var currYear = GetDateTime().Year;
            int getuserid = getidentity.ID(User.Identity.Name);

            if (integrateData.Count() == 0)
            {
                TempData["SweetAlert"] = SweetAlert.SetAlert("No Record Found.", SweetAlert.SweetAlertType.Info);
            }
            else
            {
                try
                {
                    //save ke bgt_log_audit_trail
                    var newDetail = new bgt_log_audit_trail
                    {
                        alat_budgeting_year = BudgetYear ?? currYear, //BudgetYear ?? DateTime.Now.ToLocalDateTime().Year
                        alat_preparer_id = getuserid.ToString(),
                        alat_preparer_name = User.Identity.Name,
                        alat_last_access_time =  GetDateTime(),
                        last_modified = GetDateTime(),
                    };
                    dbe.bgt_Log_Audit_Trail.Add(newDetail);
                    dbe.SaveChanges();
                }
                catch (Exception ex)
                {
                    errlog.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    TempData["SweetAlert"] = SweetAlert.SetAlert("Error", SweetAlert.SweetAlertType.Error);
                    return RedirectToAction("IntegrationDownload", new { BudgetYear = BudgetYear });
                }
            }
            return RedirectToAction("IntegrationDownload", new { BudgetYear = BudgetYear });
        }

        public List<IntegrationListDetailViewModel> getAllRecord(int? BudgetYear = null) {

            List<IntegrationListDetailViewModel> integrateData = new List<IntegrationListDetailViewModel>();
            List<IntegrationListDetailViewModel> massageData = new List<IntegrationListDetailViewModel>();

            try
            {
                //var screenId = scr.GetScreen(screenCode).ScrID;
                var costCenters = cc.GetCostCenters().DistinctBy(x => x.fld_CostCenter).ToList();
                //var gls = gl.GetGLs(screenId);

                var currDate = GetDateTime().ToString("dd/MM/yyyy hh:mm:ss tt");
                var userId = User.Identity.Name;
                var currYear = GetDateTime().Year;

                //var dataCapital = dbe.bgt_expenses_capitals.Where(x => x.abce_budgeting_year == BudgetYear && !x.abce_deleted).ToList();
                //dataCapital.ForEach(x => {
                //    integrateData.Add(new IntegrationListDetailViewModel
                //    {
                //        revision = x.abce_revision,
                //        budgeting_year = x.abce_budgeting_year,
                //        syarikat_id = x.abce_syarikat_id ?? 0,
                //        cost_center_code = x.abce_cost_center_code,
                //        gl_code = x.abce_gl_code,
                //        amount_1 = x.abce_amount_1 ?? 0,
                //        amount_2 = x.abce_amount_2 ?? 0,
                //        amount_3 = x.abce_amount_3 ?? 0,
                //        amount_4 = x.abce_amount_4 ?? 0,
                //        amount_5 = x.abce_amount_5 ?? 0,
                //        amount_6 = x.abce_amount_6 ?? 0,
                //        amount_7 = x.abce_amount_7 ?? 0,
                //        amount_8 = x.abce_amount_8 ?? 0,
                //        amount_9 = x.abce_amount_9 ?? 0,
                //        amount_10 = x.abce_amount_10 ?? 0,
                //        amount_11 = x.abce_amount_11 ?? 0,
                //        amount_12 = x.abce_amount_12 ?? 0,
                //        total = x.abce_total ?? 0,
                //        download_by = userId,
                //        download_date = currDate,
                //    });
                //});
                
                var dataSawit = dbe.bgt_income_sawit.Where(x => x.abio_budgeting_year == BudgetYear && x.fld_Deleted != true).ToList();
                dataSawit.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abio_revision ?? 0,
                        budgeting_year = x.abio_budgeting_year ?? currYear,
                        syarikat_id = x.fld_SyarikatID ?? 0,
                        cost_center_code = x.abio_cost_center,
                        gl_code = x.abio_gl_code,
                        amount_1 = x.abio_amount_1 ?? 0,
                        amount_2 = x.abio_amount_2 ?? 0,
                        amount_3 = x.abio_amount_3 ?? 0,
                        amount_4 = x.abio_amount_4 ?? 0,
                        amount_5 = x.abio_amount_5 ?? 0,
                        amount_6 = x.abio_amount_6 ?? 0,
                        amount_7 = x.abio_amount_7 ?? 0,
                        amount_8 = x.abio_amount_8 ?? 0,
                        amount_9 = x.abio_amount_9 ?? 0,
                        amount_10 = x.abio_amount_10 ?? 0,
                        amount_11 = x.abio_amount_11 ?? 0,
                        amount_12 = x.abio_amount_12 ?? 0,
                        total = x.abio_grand_total_amount ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataBenih = dbe.bgt_income_bijibenih.Where(x => x.abis_budgeting_year == BudgetYear && x.fld_Deleted != true).ToList();
                dataBenih.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abis_revision ?? 0,
                        budgeting_year = x.abis_budgeting_year ?? currYear,
                        syarikat_id = x.abis_syarikat_id ?? 0,
                        cost_center_code = x.abis_cost_center,
                        gl_code = x.abis_gl_code,
                        amount_1 = x.abis_amount_1 ?? 0,
                        amount_2 = x.abis_amount_2 ?? 0,
                        amount_3 = x.abis_amount_3 ?? 0,
                        amount_4 = x.abis_amount_4 ?? 0,
                        amount_5 = x.abis_amount_5 ?? 0,
                        amount_6 = x.abis_amount_6 ?? 0,
                        amount_7 = x.abis_amount_7 ?? 0,
                        amount_8 = x.abis_amount_8 ?? 0,
                        amount_9 = x.abis_amount_9 ?? 0,
                        amount_10 = x.abis_amount_10 ?? 0,
                        amount_11 = x.abis_amount_11 ?? 0,
                        amount_12 = x.abis_amount_12 ?? 0,
                        total = x.abis_grand_total_amount ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataProduct = dbe.bgt_income_product.Where(x => x.abip_budgeting_year == BudgetYear && x.fld_Deleted != true).ToList();
                dataProduct.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abip_revision ?? 0,
                        budgeting_year = x.abip_budgeting_year ?? currYear,
                        syarikat_id = x.abip_syarikat_id ?? 0,
                        cost_center_code = x.abip_cost_center,
                        gl_code = x.abip_gl_code,
                        amount_1 = x.abip_amount_1 ?? 0,
                        amount_2 = x.abip_amount_2 ?? 0,
                        amount_3 = x.abip_amount_3 ?? 0,
                        amount_4 = x.abip_amount_4 ?? 0,
                        amount_5 = x.abip_amount_5 ?? 0,
                        amount_6 = x.abip_amount_6 ?? 0,
                        amount_7 = x.abip_amount_7 ?? 0,
                        amount_8 = x.abip_amount_8 ?? 0,
                        amount_9 = x.abip_amount_9 ?? 0,
                        amount_10 = x.abip_amount_10 ?? 0,
                        amount_11 = x.abip_amount_11 ?? 0,
                        amount_12 = x.abip_amount_12 ?? 0,
                        total = x.abip_grand_total_amount ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataPau = dbe.bgt_expenses_paus.Where(x => x.abep_budgeting_year == BudgetYear && x.abep_deleted != true).ToList();
                dataPau.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abep_revision,
                        budgeting_year = x.abep_budgeting_year,
                        syarikat_id = x.abep_syarikat_id ?? 0,
                        cost_center_code = x.abep_cost_center_code,
                        gl_code = x.abep_gl_expenses_code,
                        amount_1 = x.abep_amount_1 ?? 0,
                        amount_2 = x.abep_amount_2 ?? 0,
                        amount_3 = x.abep_amount_3 ?? 0,
                        amount_4 = x.abep_amount_4 ?? 0,
                        amount_5 = x.abep_amount_5 ?? 0,
                        amount_6 = x.abep_amount_6 ?? 0,
                        amount_7 = x.abep_amount_7 ?? 0,
                        amount_8 = x.abep_amount_8 ?? 0,
                        amount_9 = x.abep_amount_9 ?? 0,
                        amount_10 = x.abep_amount_10 ?? 0,
                        amount_11 = x.abep_amount_11 ?? 0,
                        amount_12 = x.abep_amount_12 ?? 0,
                        total = x.abep_total ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataLevi = dbe.bgt_expenses_levi.Where(x => x.abel_budgeting_year == BudgetYear && x.fld_Deleted != true).ToList();
                dataLevi.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abel_revision ?? 0,
                        budgeting_year = x.abel_budgeting_year ?? currYear,
                        syarikat_id = x.abel_syarikat_id ?? 0,
                        cost_center_code = x.abel_cost_center_code,
                        gl_code = x.abel_gl_expenses_code,
                        amount_1 = x.abel_amount_1 ?? 0,
                        amount_2 = x.abel_amount_2 ?? 0,
                        amount_3 = x.abel_amount_3 ?? 0,
                        amount_4 = x.abel_amount_4 ?? 0,
                        amount_5 = x.abel_amount_5 ?? 0,
                        amount_6 = x.abel_amount_6 ?? 0,
                        amount_7 = x.abel_amount_7 ?? 0,
                        amount_8 = x.abel_amount_8 ?? 0,
                        amount_9 = x.abel_amount_9 ?? 0,
                        amount_10 = x.abel_amount_10 ?? 0,
                        amount_11 = x.abel_amount_11 ?? 0,
                        amount_12 = x.abel_amount_12 ?? 0,
                        total = x.abel_grand_total_amount ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataGaji = dbe.bgt_expenses_gajis.Where(x => x.abeg_budgeting_year == BudgetYear && !x.abeg_Deleted).ToList();
                dataGaji.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abeg_revision ?? 0,
                        budgeting_year = x.abeg_budgeting_year,
                        syarikat_id = x.abeg_syarikat_id ?? 0,
                        cost_center_code = x.abeg_cost_center_code,
                        gl_code = x.abeg_gl_expenses_code,
                        amount_1 = x.abeg_month_1 ?? 0,
                        amount_2 = x.abeg_month_2 ?? 0,
                        amount_3 = x.abeg_month_3 ?? 0,
                        amount_4 = x.abeg_month_4 ?? 0,
                        amount_5 = x.abeg_month_5 ?? 0,
                        amount_6 = x.abeg_month_6 ?? 0,
                        amount_7 = x.abeg_month_7 ?? 0,
                        amount_8 = x.abeg_month_8 ?? 0,
                        amount_9 = x.abeg_month_9 ?? 0,
                        amount_10 = x.abeg_month_10 ?? 0,
                        amount_11 = x.abeg_month_11 ?? 0,
                        amount_12 = x.abeg_month_12 ?? 0,
                        total = x.abeg_total ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataVehicle = dbe.bgt_expenses_vehicle.Where(x => x.abev_budgeting_year == BudgetYear && x.abev_deleted != true).ToList();
                dataVehicle.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abev_revision ?? 0,
                        budgeting_year = x.abev_budgeting_year,
                        syarikat_id = x.abev_syarikat_id ?? 0,
                        cost_center_code = x.abev_cost_center_code,
                        gl_code = x.abev_gl_expenses_code,
                        amount_1 = x.abev_amount_1 ?? 0,
                        amount_2 = x.abev_amount_2 ?? 0,
                        amount_3 = x.abev_amount_3 ?? 0,
                        amount_4 = x.abev_amount_4 ?? 0,
                        amount_5 = x.abev_amount_5 ?? 0,
                        amount_6 = x.abev_amount_6 ?? 0,
                        amount_7 = x.abev_amount_7 ?? 0,
                        amount_8 = x.abev_amount_8 ?? 0,
                        amount_9 = x.abev_amount_9 ?? 0,
                        amount_10 = x.abev_amount_10 ?? 0,
                        amount_11 = x.abev_amount_11 ?? 0,
                        amount_12 = x.abev_amount_12 ?? 0,
                        total = x.abev_total ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataOthers = dbe.bgt_expenses_others.Where(x => x.abeo_budgeting_year == BudgetYear && !x.abeo_deleted).ToList();
                dataOthers.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abeo_revision,
                        budgeting_year = x.abeo_budgeting_year,
                        syarikat_id = x.abeo_syarikat_id ?? 0,
                        cost_center_code = x.abeo_cost_center_code,
                        gl_code = x.abeo_gl_expenses_code,
                        amount_1 = x.abeo_month_1 ?? 0,
                        amount_2 = x.abeo_month_2 ?? 0,
                        amount_3 = x.abeo_month_3 ?? 0,
                        amount_4 = x.abeo_month_4 ?? 0,
                        amount_5 = x.abeo_month_5 ?? 0,
                        amount_6 = x.abeo_month_6 ?? 0,
                        amount_7 = x.abeo_month_7 ?? 0,
                        amount_8 = x.abeo_month_8 ?? 0,
                        amount_9 = x.abeo_month_9 ?? 0,
                        amount_10 = x.abeo_month_10 ?? 0,
                        amount_11 = x.abeo_month_11 ?? 0,
                        amount_12 = x.abeo_month_12 ?? 0,
                        total = x.abeo_total ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataCtrlHQ = dbe.bgt_expenses_CtrlHQ.Where(x => x.abhq_budgeting_year == BudgetYear && !x.abhq_Deleted).ToList();
                dataCtrlHQ.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abhq_revision,
                        budgeting_year = x.abhq_budgeting_year ?? currYear,
                        syarikat_id = x.abhq_SyarikatID ?? 0,
                        cost_center_code = x.abhq_cost_center,
                        gl_code = x.abhq_gl_code,
                        amount_1 = x.abhq_month_1 ?? 0,
                        amount_2 = x.abhq_month_2 ?? 0,
                        amount_3 = x.abhq_month_3 ?? 0,
                        amount_4 = x.abhq_month_4 ?? 0,
                        amount_5 = x.abhq_month_5 ?? 0,
                        amount_6 = x.abhq_month_6 ?? 0,
                        amount_7 = x.abhq_month_7 ?? 0,
                        amount_8 = x.abhq_month_8 ?? 0,
                        amount_9 = x.abhq_month_9 ?? 0,
                        amount_10 = x.abhq_month_10 ?? 0,
                        amount_11 = x.abhq_month_11 ?? 0,
                        amount_12 = x.abhq_month_12 ?? 0,
                        total = x.abhq_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataDepreciation = dbe.bgt_expenses_Depreciation.Where(x => x.abed_budgeting_year == BudgetYear && !x.abed_Deleted).ToList();
                dataDepreciation.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abed_revision,
                        budgeting_year = x.abed_budgeting_year ?? currYear,
                        syarikat_id = x.abed_SyarikatID ?? 0,
                        cost_center_code = x.abed_cost_center,
                        gl_code = x.abed_gl_code,
                        amount_1 = x.abed_month_1 ?? 0,
                        amount_2 = x.abed_month_2 ?? 0,
                        amount_3 = x.abed_month_3 ?? 0,
                        amount_4 = x.abed_month_4 ?? 0,
                        amount_5 = x.abed_month_5 ?? 0,
                        amount_6 = x.abed_month_6 ?? 0,
                        amount_7 = x.abed_month_7 ?? 0,
                        amount_8 = x.abed_month_8 ?? 0,
                        amount_9 = x.abed_month_9 ?? 0,
                        amount_10 = x.abed_month_10 ?? 0,
                        amount_11 = x.abed_month_11 ?? 0,
                        amount_12 = x.abed_month_12 ?? 0,
                        total = x.abed_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataInsurans = dbe.bgt_expenses_Insurans.Where(x => x.abei_budgeting_year == BudgetYear && !x.abei_Deleted).ToList();
                dataInsurans.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abei_revision,
                        budgeting_year = x.abei_budgeting_year ?? currYear,
                        syarikat_id = x.abei_SyarikatID ?? 0,
                        cost_center_code = x.abei_cost_center,
                        gl_code = x.abei_gl_code,
                        amount_1 = x.abei_month_1 ?? 0,
                        amount_2 = x.abei_month_2 ?? 0,
                        amount_3 = x.abei_month_3 ?? 0,
                        amount_4 = x.abei_month_4 ?? 0,
                        amount_5 = x.abei_month_5 ?? 0,
                        amount_6 = x.abei_month_6 ?? 0,
                        amount_7 = x.abei_month_7 ?? 0,
                        amount_8 = x.abei_month_8 ?? 0,
                        amount_9 = x.abei_month_9 ?? 0,
                        amount_10 = x.abei_month_10 ?? 0,
                        amount_11 = x.abei_month_11 ?? 0,
                        amount_12 = x.abei_month_12 ?? 0,
                        total = x.abei_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataIt = dbe.bgt_expenses_IT.Where(x => x.abet_budgeting_year == BudgetYear && !x.abet_Deleted).ToList();
                dataIt.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abet_revision,
                        budgeting_year = x.abet_budgeting_year ?? currYear,
                        syarikat_id = x.abet_SyarikatID ?? 0,
                        cost_center_code = x.abet_cost_center,
                        gl_code = x.abet_gl_code,
                        amount_1 = x.abet_month_1 ?? 0,
                        amount_2 = x.abet_month_2 ?? 0,
                        amount_3 = x.abet_month_3 ?? 0,
                        amount_4 = x.abet_month_4 ?? 0,
                        amount_5 = x.abet_month_5 ?? 0,
                        amount_6 = x.abet_month_6 ?? 0,
                        amount_7 = x.abet_month_7 ?? 0,
                        amount_8 = x.abet_month_8 ?? 0,
                        amount_9 = x.abet_month_9 ?? 0,
                        amount_10 = x.abet_month_10 ?? 0,
                        amount_11 = x.abet_month_11 ?? 0,
                        amount_12 = x.abet_month_12 ?? 0,
                        total = x.abet_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataCorp = dbe.bgt_expenses_KhidCor.Where(x => x.abcs_budgeting_year == BudgetYear && !x.abcs_Deleted).ToList();
                dataCorp.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abcs_revision,
                        budgeting_year = x.abcs_budgeting_year ?? currYear,
                        syarikat_id = x.abcs_SyarikatID ?? 0,
                        cost_center_code = x.abcs_cost_center,
                        gl_code = x.abcs_gl_code,
                        amount_1 = x.abcs_month_1 ?? 0,
                        amount_2 = x.abcs_month_2 ?? 0,
                        amount_3 = x.abcs_month_3 ?? 0,
                        amount_4 = x.abcs_month_4 ?? 0,
                        amount_5 = x.abcs_month_5 ?? 0,
                        amount_6 = x.abcs_month_6 ?? 0,
                        amount_7 = x.abcs_month_7 ?? 0,
                        amount_8 = x.abcs_month_8 ?? 0,
                        amount_9 = x.abcs_month_9 ?? 0,
                        amount_10 = x.abcs_month_10 ?? 0,
                        amount_11 = x.abcs_month_11 ?? 0,
                        amount_12 = x.abcs_month_12 ?? 0,
                        total = x.abcs_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataMedical = dbe.bgt_expenses_Medical.Where(x => x.abem_budgeting_year == BudgetYear && !x.abem_Deleted).ToList();
                dataMedical.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abem_revision,
                        budgeting_year = x.abem_budgeting_year ?? currYear,
                        syarikat_id = x.abem_SyarikatID ?? 0,
                        cost_center_code = x.abem_cost_center,
                        gl_code = x.abem_gl_code,
                        amount_1 = x.abem_month_1 ?? 0,
                        amount_2 = x.abem_month_2 ?? 0,
                        amount_3 = x.abem_month_3 ?? 0,
                        amount_4 = x.abem_month_4 ?? 0,
                        amount_5 = x.abem_month_5 ?? 0,
                        amount_6 = x.abem_month_6 ?? 0,
                        amount_7 = x.abem_month_7 ?? 0,
                        amount_8 = x.abem_month_8 ?? 0,
                        amount_9 = x.abem_month_9 ?? 0,
                        amount_10 = x.abem_month_10 ?? 0,
                        amount_11 = x.abem_month_11 ?? 0,
                        amount_12 = x.abem_month_12 ?? 0,
                        total = x.abem_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataSalary = dbe.bgt_expenses_Salary.Where(x => x.abes_budgeting_year == BudgetYear && !x.abes_Deleted).ToList();
                dataSalary.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abes_revision,
                        budgeting_year = x.abes_budgeting_year ?? currYear,
                        syarikat_id = x.abes_SyarikatID ?? 0,
                        cost_center_code = x.abes_cost_center,
                        gl_code = x.abes_gl_code,
                        amount_1 = x.abes_month_1 ?? 0,
                        amount_2 = x.abes_month_2 ?? 0,
                        amount_3 = x.abes_month_3 ?? 0,
                        amount_4 = x.abes_month_4 ?? 0,
                        amount_5 = x.abes_month_5 ?? 0,
                        amount_6 = x.abes_month_6 ?? 0,
                        amount_7 = x.abes_month_7 ?? 0,
                        amount_8 = x.abes_month_8 ?? 0,
                        amount_9 = x.abes_month_9 ?? 0,
                        amount_10 = x.abes_month_10 ?? 0,
                        amount_11 = x.abes_month_11 ?? 0,
                        amount_12 = x.abes_month_12 ?? 0,
                        total = x.abes_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                var dataWindfall = dbe.bgt_expenses_Windfall.Where(x => x.abew_budgeting_year == BudgetYear && !x.abew_Deleted).ToList();
                dataWindfall.ForEach(x => {
                    integrateData.Add(new IntegrationListDetailViewModel
                    {
                        revision = x.abew_revision,
                        budgeting_year = x.abew_budgeting_year ?? currYear,
                        syarikat_id = x.abew_SyarikatID ?? 0,
                        cost_center_code = x.abew_cost_center,
                        gl_code = x.abew_gl_code,
                        amount_1 = x.abew_month_1 ?? 0,
                        amount_2 = x.abew_month_2 ?? 0,
                        amount_3 = x.abew_month_3 ?? 0,
                        amount_4 = x.abew_month_4 ?? 0,
                        amount_5 = x.abew_month_5 ?? 0,
                        amount_6 = x.abew_month_6 ?? 0,
                        amount_7 = x.abew_month_7 ?? 0,
                        amount_8 = x.abew_month_8 ?? 0,
                        amount_9 = x.abew_month_9 ?? 0,
                        amount_10 = x.abew_month_10 ?? 0,
                        amount_11 = x.abew_month_11 ?? 0,
                        amount_12 = x.abew_month_12 ?? 0,
                        total = x.abew_jumlah_keseluruhan ?? 0,
                        download_by = userId,
                        download_date = currDate,
                    });
                });

                massageData = integrateData.GroupBy(g => new { g.cost_center_code, g.gl_code, g.revision, g.budgeting_year, g.syarikat_id }).Select(s => new IntegrationListDetailViewModel
                {
                    revision = s.FirstOrDefault().revision,
                    budgeting_year = s.FirstOrDefault().budgeting_year,
                    syarikat_id = s.FirstOrDefault().syarikat_id,
                    cost_center_code = s.FirstOrDefault().cost_center_code,
                    gl_code = s.FirstOrDefault().gl_code,
                    amount_1 = s.Sum(c => c.amount_1),
                    amount_2 = s.Sum(c => c.amount_2),
                    amount_3 = s.Sum(c => c.amount_3),
                    amount_4 = s.Sum(c => c.amount_4),
                    amount_5 = s.Sum(c => c.amount_5),
                    amount_6 = s.Sum(c => c.amount_6),
                    amount_7 = s.Sum(c => c.amount_7),
                    amount_8 = s.Sum(c => c.amount_8),
                    amount_9 = s.Sum(c => c.amount_9),
                    amount_10 = s.Sum(c => c.amount_10),
                    amount_11 = s.Sum(c => c.amount_11),
                    amount_12 = s.Sum(c => c.amount_12),
                    total = s.Sum(c => c.total),
                    download_by = userId,
                    download_date = currDate,
                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return massageData;
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

        public class IntegrationUploadFigureModel
        {
            //Year
            [Column(1)]
            public int actu_Year { get; set; }
            //GL Account
            [Column(2)]
            public string actu_GLCode { get; set; }
            //Profit Center
            [Column(3)]
            public string actu_CostCenter { get; set; }
            //Actual Amount
            [Column(4)]
            public decimal actu_ActualAmt { get; set; }
        }

    }
}