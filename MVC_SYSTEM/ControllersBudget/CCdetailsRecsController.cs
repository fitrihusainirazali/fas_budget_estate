using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MVC_SYSTEM.ModelsBudget;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.ClassBudget;
using Microsoft.Reporting.WebForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Linq.Dynamic;
using MVC_SYSTEM.Controllers;
using MVC_SYSTEM.ModelsBudgetView;
using MVC_SYSTEM.MasterModels;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class CCdetailsRecsController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        private MVC_SYSTEM_ModelsBudgetEst dbEst = new ConnectionBudget().GetConnection();
        private MVC_SYSTEM_ModelsBudgetView dbBview = new MVC_SYSTEM_ModelsBudgetView();
        private MVC_SYSTEM_MasterModels dbM = new MVC_SYSTEM_MasterModels();

        private Negara negara = new Negara();
        private Syarikat syarikat = new Syarikat();
        private Wilayah wilayah = new Wilayah();
        private Ladang ladang = new Ladang();

        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        private GetConfig config = new GetConfig();
        private errorlog errlog = new errorlog();
        GetIdentity GetIdentity = new GetIdentity();
        Connection Connection = new Connection();
        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private Screen scr = new Screen();
        private readonly string screenCode = "B1";

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

        private List<int> GetYears()
        {
            return db.bgt_Notification.OrderByDescending(n => n.Bgt_Year).Select(n => n.Bgt_Year).ToList();
        }

        /**************************************************************************************************************************/
        public ActionResult CCdetails()
        {
            int currentyear = GetDateTime().Year;

            List<SelectListItem> YearList = new List<SelectListItem>();
            YearList = new SelectList(db.bgt_Notification.Select(s => new SelectListItem { Value = s.Bgt_Year.ToString(), Text = s.Bgt_Year.ToString() }).OrderByDescending(o => o.Value), "Value", "Text", currentyear.ToString()).ToList();

            ViewBag.YearList = YearList;

            return View();
        }


        
        public ActionResult _CCdetails(string YearList, int page = 1)
        {
            var negaraId = negara.GetNegaraID();
            var syarikatId = syarikat.GetSyarikatID();
            var ladangId = ladang.GetLadangID();
            var wilayahId = wilayah.GetWilayahID();

            ViewBag.NegaraID = negaraId;
            ViewBag.SyarikatID = syarikatId;

            ViewBag.NamaSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatId && x.fld_NegaraID == negaraId).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = dbM.tbl_Syarikat.Where(x => x.fld_SyarikatID == syarikatId && x.fld_NegaraID == negaraId).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearList = YearList;
            

            List<sp_CCdetailsRecs> rptGLAll = dbEst.Database.SqlQuery<sp_CCdetailsRecs>("exec sp_CCdetailsRecs {0}", YearList).ToList();

            if (rptGLAll.Count == 0)
            {
                ViewBag.Message = GlobalResEstate.msgNoRecord;
            }

            return View(rptGLAll);
        }




        public ActionResult CCrecPDF(string YearList, string format = "PDF")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "RptCCdetailsRecs.rdlc");
            //string filename = scr.GetScreen(screenCode).ScrNameLongDesc;
            string filename = "DETAIL RECORDS";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<sp_CCdetailsRecs> data = dbEst.Database.SqlQuery<sp_CCdetailsRecs>("exec sp_CCdetailsRecs {0}", YearList).ToList();
            ReportDataSource rd = new ReportDataSource("DataSet1", data);
            lr.DataSources.Add(rd);

            return Reporting.DownloadReport(lr, format, Reporting.PageOrientation.Landscape, filename);
        }

        public ActionResult CCrecExcel(string YearList, string Format = "EXCEL")
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsBudget"), "RptCCdetailsRecsExcel.rdlc");
            //string filename = scr.GetScreen(screenCode).ScrNameLongDesc;
            string filename = "DETAIL RECORDS";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                //show error
            }

            List<sp_CCdetailsRecs> data = dbEst.Database.SqlQuery<sp_CCdetailsRecs>("exec sp_CCdetailsRecs {0}", YearList).ToList();
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
    }
}
