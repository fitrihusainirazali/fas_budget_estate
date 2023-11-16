using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Attributes;
using System.Linq.Dynamic;
using MVC_SYSTEM.App_LocalResources;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsBudget.ViewModels;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class LeviController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_MasterModels dbc = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_ModelsBudgetEst dbe = new ConnectionBudget().GetConnection();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        GetWilayah GetWilayah = new GetWilayah();
        Connection Connection = new Connection();
        GetLadang GetLadang = new GetLadang();
        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        GetBudgetClass GetBudgetClass = new GetBudgetClass();

        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }
        // GET: Levi
        public ActionResult Index()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> Year = new List<SelectListItem>();
            Year = new SelectList(db.bgt_Notification.Where(w => w.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.Bgt_Year.ToString(), Text = s.Bgt_Year.ToString() })
                .OrderByDescending(o => o.Text), "Value", "Text").ToList();
            ViewBag.BudgetYear = Year;

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

            var records = new ViewingModels.PagedList<ExpensesLeviListViewModel>
            {
                Content = GetRecordsIndex(budgetYear, CostCenter, page, pageSize),
                TotalRecords = GetRecordsIndex(budgetYear, CostCenter).Count(),
                CurrentPage = page,
                PageSize = pageSize
            };
            return View(records);
        }
        public ActionResult ViewLevi(string costcenter, string budgetyear)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            if (Convert.ToInt32(budgetyear) >= 2024)
            {
                bgt_expenses_levi[] bgt_expenses_levi = dbr.bgt_expenses_levi.Where(x => x.abel_cost_center_code == costcenter && x.abel_budgeting_year.ToString() == budgetyear && x.fld_Deleted == false).ToArray();
                var LeviBTSList = db.bgt_LeviBTS.Where(x => x.Levi_Year.ToString() == budgetyear && x.LeviCaj == true && x.Location == WilayahID && x.fld_Deleted == false).ToList();
                var getMaxMonth = LeviBTSList.Max(m => m.Levi_Month);
                var leviValue = LeviBTSList.Where(w => w.Levi_Month == getMaxMonth).Select(s => s.LeviValue).FirstOrDefault();

                if (leviValue < 0 || leviValue == null)
                {
                    leviValue = 0;
                }

                ViewBag.LeviValue = leviValue;

                //M.s
                int totalpage = 0;
                int page = 0;
                int pagesize = 0;
                int totalldata = 0;
                var bgtexpdata = bgt_expenses_levi.GroupBy(g => new { g.abel_cost_center_code, g.abel_budgeting_year }).ToList();

                totalldata = bgtexpdata.Count();
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
                ViewBag.Message = @GlobalResEstate.msgNoRecord;
                ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                return PartialView(bgt_expenses_levi);
            }
            else
            {
                bgt_expenses_levi[] bgt_expenses_levi = dbr.bgt_expenses_levi.Where(x => x.abel_cost_center_code == costcenter && x.abel_budgeting_year.ToString() == budgetyear).ToArray();

                var LeviBTSList = db.bgt_LeviBTS.Where(x => x.Levi_Year.ToString() == budgetyear && x.LeviCaj == true && x.Location == WilayahID && x.fld_Deleted == false).ToList();
                var getMaxMonth = LeviBTSList.Max(m => m.Levi_Month);
                var leviValue = LeviBTSList.Where(w => w.Levi_Month == getMaxMonth).Select(s => s.LeviValue).FirstOrDefault();

                if (leviValue < 0 || leviValue == null)
                {
                    leviValue = 0;
                }

                ViewBag.LeviValue = leviValue;

                //M.s
                int totalpage = 0;
                int page = 0;
                int pagesize = 0;
                int totalldata = 0;
                var bgtexpdata = bgt_expenses_levi.GroupBy(g => new { g.abel_cost_center_code, g.abel_budgeting_year }).ToList();

                totalldata = bgtexpdata.Count();
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
                ViewBag.Message = @GlobalResEstate.msgNoRecord;
                ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                return PartialView(bgt_expenses_levi);
            }
        }

        public FileStreamResult exportPDF(string budgetyear, string costcenter)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbv = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            Document pdfDoc = new Document(PageSize.A4, 20, 20, 10, 6);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();

            ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            //Get Year
            var advanceyear = DateTime.Now.Year + 1;
            var getBudgetYear = db.bgt_Notification.Where(x => x.Bgt_Year == advanceyear).ToList();
            if (getBudgetYear.Count() > 0)
            {
                ViewBag.BudgetYear = getBudgetYear.FirstOrDefault().Bgt_Year;
            }
            else
            {
                ViewBag.BudgetYear = advanceyear;
            }

            //Get GL
            var GL = dbc.tbl_SAPGLPUP.Where(x => x.fld_GLCode == "0053120140").ToList();
            ViewBag.fld_GLCode = GL.FirstOrDefault().fld_GLCode;
            ViewBag.fld_GLDesc = GL.FirstOrDefault().fld_GLDesc;

            var bgt_expenses_levi = dbv.bgt_expenses_levi.Where(w => w.fld_Deleted == false).GroupBy(g => g.abel_cost_center_code).Select(g => g.FirstOrDefault()).OrderBy(o => o.abel_budgeting_year).ThenBy(t => t.abel_cost_center_code).ToList();

            if (costcenter != "" && budgetyear == "")
            {
                bgt_expenses_levi = bgt_expenses_levi.Where(x => x.abel_cost_center_code.Contains(costcenter) || x.abel_cost_center_desc.Contains(costcenter)).ToList();
            }
            else if (costcenter == "" && budgetyear != "")
            {
                bgt_expenses_levi = bgt_expenses_levi.Where(x => x.abel_budgeting_year.ToString() == budgetyear).ToList();
            }
            else if (costcenter != "" && budgetyear != "")
            {
                bgt_expenses_levi = bgt_expenses_levi.Where(x => (x.abel_cost_center_code.Contains(costcenter) || x.abel_cost_center_desc.Contains(costcenter)) && x.abel_budgeting_year.ToString() == budgetyear).ToList();
            }
            else
            {
                bgt_expenses_levi = bgt_expenses_levi.ToList();
            }

            if (bgt_expenses_levi.Count() > 0)
            {
                PdfPTable table0 = new PdfPTable(3);
                table0.WidthPercentage = 100;
                PdfPCell cell0 = new PdfPCell();
                cell0.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table0.AddCell(cell0);

                chunk = new Chunk(ViewBag.NamaSyarikat, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table0.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table0.AddCell(cell0);

                //-------------------------------------------

                PdfPTable table1 = new PdfPTable(3);
                table1.WidthPercentage = 100;
                Image image = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Asset/Images/logo_FTPSB.jpg"));
                PdfPCell cellimage = new PdfPCell(image);
                cellimage.HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                cellimage.VerticalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                cellimage.Border = Rectangle.BOTTOM_BORDER;
                cellimage.BorderColor = BaseColor.RED;
                image.ScaleAbsolute(40, 30);
                table1.AddCell(cellimage);

                chunk = new Chunk("(" + ViewBag.NoSyarikat + ")", FontFactory.GetFont("Arial", 9, /*Font.BOLD,*/ BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = Rectangle.BOTTOM_BORDER;
                cell0.BorderColor = BaseColor.RED;
                table1.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = Rectangle.BOTTOM_BORDER;
                cell0.BorderColor = BaseColor.RED;
                table1.AddCell(cell0);

                //----------------------------------------------

                PdfPTable table2 = new PdfPTable(3);
                table2.WidthPercentage = 100;

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table2.AddCell(cell0);

                chunk = new Chunk("Bajet " + ViewBag.BudgetYear, FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table2.AddCell(cell0);

                PdfPCell celldate = new PdfPCell();
                chunk = new Chunk("Tarikh : " + DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                celldate = new PdfPCell(new Phrase(chunk));
                celldate.HorizontalAlignment = Element.ALIGN_RIGHT;
                celldate.VerticalAlignment = Element.ALIGN_MIDDLE;
                celldate.Border = 0;
                table2.AddCell(celldate);

                //------------------------------------

                PdfPTable table3 = new PdfPTable(3);
                table3.WidthPercentage = 100;
                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table3.AddCell(cell0);

                chunk = new Chunk(GetBudgetClass.GetScreenName("E11").ToUpperInvariant(), FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table3.AddCell(cell0);

                PdfPCell cellrevision = new PdfPCell();
                chunk = new Chunk("Revision : " + bgt_expenses_levi.FirstOrDefault().abel_status, FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cellrevision = new PdfPCell(new Phrase(chunk));
                cellrevision.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellrevision.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellrevision.Border = 0;
                table3.AddCell(cellrevision);

                //------------------------------------

                PdfPTable table4 = new PdfPTable(3);
                table4.WidthPercentage = 100;

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table4.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table4.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table4.AddCell(cell0);

                pdfDoc.Add(table0);
                pdfDoc.Add(table1);
                pdfDoc.Add(table2);
                pdfDoc.Add(table3);
                pdfDoc.Add(table4);

                //--------------------------------------------------------------------

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                float[] widths = new float[] { 0.5f, 1.5f, 2, 1, 1, 1 };
                table.SetWidths(widths);
                PdfPCell cell = new PdfPCell();

                table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SpacingBefore = 5f;
                widths = new float[] { 0.5f, 1.5f, 2, 1, 1, 1 };
                table.SetWidths(widths);

                chunk = new Chunk(@GlobalResEstate.hdrNo, FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));//1
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(@GlobalResEstate.lblCostC, FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));//2
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("Keterangan Cost Center", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));//3
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(@GlobalResEstate.hdrQuantity, FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));//4
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(@GlobalResEstate.lblTotal + "(RM)", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));//5
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("Jenis Produk", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));//6
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                int i = 1;
                foreach (var item in bgt_expenses_levi)
                {
                    chunk = new Chunk(i.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk((item.abel_cost_center_code), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk((item.abel_cost_center_desc), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk((item.abel_grand_total_quantity.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk((item.abel_grand_total_amount.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(item.abel_product_name, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    i++;
                }
                pdfDoc.Add(table);
            }

            else
            {
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk(GlobalResEstate.msgNoRecord, FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            Response.AppendHeader("Content-Disposition", "inline; filename=" + GetBudgetClass.GetScreenName("E11") + " " + budgetyear + " - LISTING.pdf");
            return new FileStreamResult(output, "application/pdf");
        }

        public void ExportExcel(string costcenter, string budgetyear)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var bgt_expenses_levi_list = dbr.bgt_expenses_levi.Where(w => w.fld_Deleted == false).GroupBy(g => g.abel_cost_center_code).Select(g => g.FirstOrDefault()).OrderBy(o => o.abel_budgeting_year).ThenBy(o => o.abel_cost_center_code).ToList();

            if (costcenter != "" && budgetyear == "")
            {
                bgt_expenses_levi_list = bgt_expenses_levi_list.Where(x => x.abel_cost_center_code.Contains(costcenter) || x.abel_cost_center_desc.Contains(costcenter)).ToList();
            }
            else if (costcenter == "" && budgetyear != "")
            {
                bgt_expenses_levi_list = bgt_expenses_levi_list.Where(x => x.abel_budgeting_year.ToString() == budgetyear).ToList();
            }
            else if (costcenter != "" && budgetyear != "")
            {
                bgt_expenses_levi_list = bgt_expenses_levi_list.Where(x => (x.abel_cost_center_code.Contains(costcenter) || x.abel_cost_center_desc.Contains(costcenter)) && x.abel_budgeting_year.ToString() == budgetyear).ToList();
            }
            else
            {
                bgt_expenses_levi_list = bgt_expenses_levi_list.ToList();
            }

            List<bgtincomleviviewmodel> bgtincomleviviewmodel = new List<bgtincomleviviewmodel>();
            foreach (var val in bgtincomleviviewmodel)
            {
                bgtincomleviviewmodel.Add(new bgtincomleviviewmodel()
                {
                    abel_id = val.abel_id,
                    abel_cost_center_code = val.abel_cost_center_code,
                    abel_cost_center_desc = val.abel_cost_center_desc,
                    abel_product_name = val.abel_product_name,
                    abel_grand_total_quantity = val.abel_grand_total_quantity,
                    abel_grand_total_amount = val.abel_grand_total_amount,
                    abel_budgeting_year = val.abel_budgeting_year
                });
            }

            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            int i = 1; ;
            gv.AutoGenerateColumns = false;
            gv.Columns.Add(new System.Web.UI.WebControls.BoundField { HeaderText = "No.", DataField = "i" });
            gv.Columns.Add(new System.Web.UI.WebControls.BoundField { HeaderText = @GlobalResEstate.lblCostC, DataField = "abel_cost_center_code" });
            gv.Columns.Add(new System.Web.UI.WebControls.BoundField { HeaderText = "Cost Center Description", DataField = "abel_cost_center_desc" });
            gv.Columns.Add(new System.Web.UI.WebControls.BoundField { HeaderText = @GlobalResEstate.hdrQuantity, DataField = "abel_grand_total_quantity" });
            gv.Columns.Add(new System.Web.UI.WebControls.BoundField { HeaderText = @GlobalResEstate.lblTotal + "(RM)", DataField = "abel_grand_total_amount" });
            gv.Columns.Add(new System.Web.UI.WebControls.BoundField { HeaderText = "Jenis Produk", DataField = "abel_product_name" });

            DataTable dt = new DataTable();
            dt.Columns.Add("i");
            dt.Columns.Add("abel_cost_center_code");
            dt.Columns.Add("abel_cost_center_desc");
            dt.Columns.Add("abel_grand_total_quantity");
            dt.Columns.Add("abel_grand_total_amount");
            dt.Columns.Add("abel_product_name");

            foreach (var item in bgt_expenses_levi_list)
            {
                dt.Rows.Add(i, item.abel_cost_center_code, item.abel_cost_center_desc, item.abel_grand_total_quantity, item.abel_grand_total_amount, item.abel_product_name);
                i++;
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + GetBudgetClass.GetScreenName("E11") + " " + budgetyear + " - LISTING.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            gv.RenderControl(htw);

            string headerTable =
            @"<Table>            
             <tr><th colspan=6 align=center style=font-size:medium> " + ViewBag.NamaSyarikat + " </th></tr>" +
            "<tr><td colspan=6 align=center style=font-size:small> " + "(" + ViewBag.NoSyarikat + ")" + " </td></tr> " +
            "<tr><td colspan=6 align=center style=font-size:small> Bajet " + budgetyear + "</td></tr>" +
            "<tr><td colspan=6 align=center style=font-size:small> " + GetBudgetClass.GetScreenName("E11") + " </td></tr> " +
            "</Table>";

            Response.Write(headerTable);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
        public PdfPCell CellNoBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            return cell;
        }
        public PdfPCell CellWithGrayBottomBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = Rectangle.BOTTOM_BORDER;
            cell.BorderColor = BaseColor.GRAY;
            return cell;
        }
        public PdfPCell CellWithRedFullBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
            cell.BorderColor = BaseColor.RED;
            return cell;
        }
        public PdfPCell CellWithRedTopBorder(string Param, int HorizontalAlignment)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = Rectangle.TOP_BORDER;
            cell.BorderColor = BaseColor.RED;
            return cell;
        }
        public PdfPCell CellWithRedTopBottomBorder(string Param, int HorizontalAlignment)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            cell.BorderColor = BaseColor.RED;
            return cell;
        }
        public FileStreamResult ViewLeviPDF(string budgetyear, string costcenter)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsBudgetEst dbr = MVC_SYSTEM_ModelsBudgetEst.ConnectToSqlServer(host, catalog, user, pass);

            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 50F, 5);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();

            bgt_expenses_levi[] bgt_expenses_levi = dbr.bgt_expenses_levi.Where(x => x.abel_cost_center_code == costcenter && x.abel_budgeting_year.ToString() == budgetyear && x.fld_Deleted == false).ToArray();

            ViewBag.NamaSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbc.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            PdfPCell cell;
            PdfPTable table;
            PdfPTable tableharga;
            float[] widths;

            if (bgt_expenses_levi.Count() == 0)
            {
                pdfDoc.Open();
                table = new PdfPTable(1);
                table.WidthPercentage = 100;
                widths = new float[] { 1 };
                table.SetWidths(widths);
                cell = new PdfPCell();
                cell = CellNoBorder("No Data", 1, 0);
                table.AddCell(cell);
                pdfDoc.Add(table);
                pdfDoc.NewPage();
            }
            else
            {
                pdfDoc.Open();
                PdfPTable table0 = new PdfPTable(3);
                table0.WidthPercentage = 100;
                PdfPCell cell0 = new PdfPCell();
                cell0.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table0.AddCell(cell0);

                chunk = new Chunk(ViewBag.NamaSyarikat, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table0.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table0.AddCell(cell0);

                //-------------------------------------------

                PdfPTable table1 = new PdfPTable(3);
                table1.WidthPercentage = 100;
                Image image = Image.GetInstance(System.Web.HttpContext.Current.Server.MapPath("~/Asset/Images/logo_FTPSB.jpg"));
                PdfPCell cellimage = new PdfPCell(image);
                cellimage.HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                cellimage.VerticalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                cellimage.Border = Rectangle.BOTTOM_BORDER;
                cellimage.BorderColor = BaseColor.RED;
                image.ScaleAbsolute(50, 40);
                table1.AddCell(cellimage);

                chunk = new Chunk("(" + ViewBag.NoSyarikat + ")", FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = Rectangle.BOTTOM_BORDER;
                cell0.BorderColor = BaseColor.RED;
                table1.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = Rectangle.BOTTOM_BORDER;
                cell0.BorderColor = BaseColor.RED;
                table1.AddCell(cell0);

                //----------------------------------------------

                PdfPTable table2 = new PdfPTable(3);
                table2.WidthPercentage = 100;

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table2.AddCell(cell0);

                chunk = new Chunk("Bajet " + bgt_expenses_levi.FirstOrDefault().abel_budgeting_year, FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table2.AddCell(cell0);

                PdfPCell celldate = new PdfPCell();
                chunk = new Chunk("Tarikh : " + DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                celldate = new PdfPCell(new Phrase(chunk));
                celldate.HorizontalAlignment = Element.ALIGN_RIGHT;
                celldate.VerticalAlignment = Element.ALIGN_MIDDLE;
                celldate.Border = 0;
                table2.AddCell(celldate);

                //------------------------------------

                PdfPTable table3 = new PdfPTable(3);
                table3.WidthPercentage = 100;
                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table3.AddCell(cell0);

                chunk = new Chunk(GetBudgetClass.GetScreenName("E11"), FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table3.AddCell(cell0);

                PdfPCell cellrevision = new PdfPCell();
                chunk = new Chunk("Revision : " + bgt_expenses_levi.FirstOrDefault().abel_status, FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cellrevision = new PdfPCell(new Phrase(chunk));
                cellrevision.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellrevision.VerticalAlignment = Element.ALIGN_MIDDLE;
                cellrevision.Border = 0;
                table3.AddCell(cellrevision);

                //------------------------------------

                PdfPTable table4 = new PdfPTable(3);
                table4.WidthPercentage = 100;

                chunk = new Chunk("Unit : " + bgt_expenses_levi.FirstOrDefault().abel_cost_center_code + " - " + bgt_expenses_levi.FirstOrDefault().abel_cost_center_desc.ToUpper(), FontFactory.GetFont("Arial", 9, BaseColor.BLACK));
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_LEFT;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table4.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table4.AddCell(cell0);

                chunk = new Chunk(" ");
                cell0 = new PdfPCell(new Phrase(chunk));
                cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell0.Border = 0;
                table4.AddCell(cell0);

                pdfDoc.Add(table0);
                pdfDoc.Add(table1);
                pdfDoc.Add(table2);
                pdfDoc.Add(table3);
                pdfDoc.Add(table4);

                //--------------------------------------------------------------------------

                table = new PdfPTable(14);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                widths = new float[] { 2.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2 };
                table.SetWidths(widths);
                cell = new PdfPCell();

                cell = CellWithGrayBottomBorder("1. " + "GL Pendapatan  :   " + bgt_expenses_levi.FirstOrDefault().abel_gl_expenses_code + " - " + bgt_expenses_levi.FirstOrDefault().abel_gl_expenses_name.ToUpper(), 0, 0);
                cell.Colspan = 14;
                table.AddCell(cell);

                cell = CellWithGrayBottomBorder("2. " + "Produk  :   " + bgt_expenses_levi.FirstOrDefault().abel_product_name.ToUpper(), 0, 0);
                cell.Colspan = 14;
                table.AddCell(cell);

                cell = CellNoBorder(" ", 0, 1);
                cell.Colspan = 14;
                table.AddCell(cell);
                cell = CellNoBorder(" ", 0, 1);
                cell.Colspan = 14;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("KUANTITI / " + bgt_expenses_levi.FirstOrDefault().abel_uom, 0, 1);
                cell.Colspan = 14;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jan", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Feb", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Mac", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Apr", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Mei", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jun", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jul", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Ogos", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Sept", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Okt", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Nov", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Dis", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jumlah", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                int i = 1;
                foreach (var item in bgt_expenses_levi)
                {
                    var TotQty = "";
                    if (item.abel_company_code == "1")
                    {
                        TotQty = item.abel_total_qty_a.ToString();
                    }
                    else
                    {
                        TotQty = item.abel_total_qty_b.ToString();
                    }

                    cell = CellWithGrayBottomBorder(@i + "." + item.abel_company_name, 0, 0);
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_1.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_2.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_3.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_4.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_5.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_6.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_7.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_8.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_9.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_10.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_11.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_qty_12.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(TotQty, 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);
                    i++;
                }

                cell = CellWithRedTopBorder(" ", 2);
                cell.Colspan = 1;
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_1).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_2).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_3).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_4).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_5).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_6).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_7).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_8).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_9).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_10).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_11).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_qty_12).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.FirstOrDefault().abel_grand_total_quantity.ToString(), 2);
                table.AddCell(cell);

                cell = CellNoBorder(" ", 0, 1);
                cell.Colspan = 14;
                table.AddCell(cell);
                cell = CellNoBorder(" ", 0, 1);
                cell.Colspan = 14;
                table.AddCell(cell);

                //--------------------NILAI-------------------------

                cell = CellWithRedFullBorder("NILAI (RM)", 0, 1);
                cell.Colspan = 14;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jan", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Feb", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Mac", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Apr", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Mei", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jun", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jul", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Ogos", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Sept", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Okt", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Nov", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Dis", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                cell = CellWithRedFullBorder("Jumlah", 1, 1);
                cell.Rowspan = 2;
                table.AddCell(cell);

                int j = 1;
                foreach (var item in bgt_expenses_levi)
                {
                    var TotAmt = "";
                    if (item.abel_company_category == "SYARIKAT DALAMAN")
                    {
                        TotAmt = item.abel_total_amount_a.ToString();
                    }
                    else
                    {
                        TotAmt = item.abel_total_amount_b.ToString();
                    }

                    cell = CellWithGrayBottomBorder(@j + "." + item.abel_company_name, 0, 0);
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_1.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_2.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_3.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_4.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_5.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_6.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_7.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_8.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_9.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_10.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_11.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_amount_12.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    cell = CellWithGrayBottomBorder(TotAmt, 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);
                    j++;
                }

                cell = CellWithRedTopBorder(" ", 2);
                cell.Colspan = 1;
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_1).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_2).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_3).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_4).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_5).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_6).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_7).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_8).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_9).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_10).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_11).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.Sum(m => m.abel_amount_12).ToString(), 2);
                table.AddCell(cell);

                cell = CellWithRedTopBottomBorder(bgt_expenses_levi.FirstOrDefault().abel_grand_total_amount.ToString(), 2);
                table.AddCell(cell);

                cell = CellNoBorder(" ", 0, 1);
                cell.Colspan = 14;
                table.AddCell(cell);
                cell = CellNoBorder(" ", 0, 1);
                cell.Colspan = 14;
                table.AddCell(cell);

                //--------------------HARGA-------------------------

                //cell = CellWithRedFullBorder("HARGA (RM / MT)", 0, 1);
                //cell.Colspan = 14;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //table.AddCell(cell);

                tableharga = new PdfPTable(13);
                tableharga.WidthPercentage = 100;
                tableharga.SpacingBefore = 10f;
                widths = new float[] { 2.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                tableharga.SetWidths(widths);

                cell = CellWithRedFullBorder("HARGA (RM / MT)", 0, 1);
                cell.Colspan = 13;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Jan", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Feb", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Mac", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Apr", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Mei", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Jun", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Jul", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Ogos", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Sept", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Okt", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Nov", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                cell = CellWithRedFullBorder("Dis", 1, 1);
                cell.Rowspan = 2;
                tableharga.AddCell(cell);

                //cell = CellWithRedFullBorder("", 1, 1);
                //cell.Rowspan = 2;
                //tableharga.AddCell(cell);

                int k = 1;
                foreach (var item in bgt_expenses_levi)
                {
                    cell = CellWithGrayBottomBorder(@k + "." + item.abel_company_name, 0, 0);
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_1.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_2.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_3.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_4.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_5.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_6.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_7.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_8.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_9.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_10.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_11.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    cell = CellWithGrayBottomBorder(item.abel_price_12.ToString(), 0, 0);
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableharga.AddCell(cell);

                    //cell = CellWithGrayBottomBorder("", 0, 0);
                    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //tableharga.AddCell(cell);
                    k++;
                }

                pdfDoc.Add(table);
                pdfDoc.Add(tableharga);
            }

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            Response.AppendHeader("Content-Disposition", "inline; filename=" + GetBudgetClass.GetScreenName("E11") + " " + budgetyear + " - VIEW.pdf");
            return new FileStreamResult(output, "application/pdf");
        }

        public ActionResult PrintList(string BudgetYear, string CostCenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "ExpensesLeviList.rdlc");
            string filename = GetBudgetClass.GetScreenName("E11") + " - LISTING";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesLeviListViewModel> data = dbe.Database.SqlQuery<ExpensesLeviListViewModel>("exec sp_BudgetExpensesLeviList {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult PrintView(string BudgetYear, string CostCenter, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "ExpensesLeviView.rdlc");
            string filename = GetBudgetClass.GetScreenName("E11") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesLeviDetailViewModel> data = dbe.Database.SqlQuery<ExpensesLeviDetailViewModel>("exec sp_BudgetExpensesLeviView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        public ActionResult PrintViewExcel(string BudgetYear, string CostCenter, string format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ReportsBudget"), "ExpensesLeviViewExcel.rdlc");
            string filename = GetBudgetClass.GetScreenName("E11") + " - VIEW";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<ExpensesLeviDetailViewModel> data = dbe.Database.SqlQuery<ExpensesLeviDetailViewModel>("exec sp_BudgetExpensesLeviView {0}, {1}", BudgetYear, CostCenter).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }
        private List<ExpensesLeviListViewModel> GetRecordsIndex(int? BudgetYear = null, string CostCenter = null, int? Page = null, int? PageSize = null)
        {
            if (Convert.ToInt32(BudgetYear) >= 2024)
            {
                var query = dbe.bgt_expenses_levi.Where(w => w.fld_Deleted == false)
                .GroupBy(g => new { g.abel_cost_center_code, g.abel_budgeting_year })
                .Select(g => new
                {
                BudgetYear = g.FirstOrDefault().abel_budgeting_year,
                CostCenterCode = g.FirstOrDefault().abel_cost_center_code,
                CostCenterDesc = g.FirstOrDefault().abel_cost_center_desc,
                JenisProduk = g.FirstOrDefault().abel_product_name,
                Quantity = g.Sum(s => s.abel_grand_total_quantity),
                Total = g.Sum(s => s.abel_grand_total_amount)
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
                              select new ExpensesLeviListViewModel
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
                var query = dbe.bgt_expenses_levi
                .GroupBy(g => new { g.abel_cost_center_code, g.abel_budgeting_year })
                .Select(g => new
                {
                    BudgetYear = g.FirstOrDefault().abel_budgeting_year,
                    CostCenterCode = g.FirstOrDefault().abel_cost_center_code,
                    CostCenterDesc = g.FirstOrDefault().abel_cost_center_desc,
                    JenisProduk = g.FirstOrDefault().abel_product_name,
                    Quantity = g.Sum(s => s.abel_grand_total_quantity),
                    Total = g.Sum(s => s.abel_grand_total_amount)
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
                              select new ExpensesLeviListViewModel
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
