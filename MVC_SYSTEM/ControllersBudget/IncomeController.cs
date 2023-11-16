using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.ControllersBudget
{
    public class IncomeController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();
        
        // GET: Income
        public ActionResult Index()
        {
            Screen screen = new Screen();
            ViewBag.Title = screen.GetScrHdr("ICC").ScrHdrDesc;
            ViewBag.Screens = screen.GetScreens().Where(s => !string.IsNullOrEmpty(s.ScrHdrCode) && s.ScrHdrCode.Contains("ICC")).ToList();

            //Temporary add
            //ViewBag.ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode.Contains("ICC")).OrderBy(o => o.ScrID).Select(s => new SelectListItem { Value = s.ScrCode, Text = s.ScrSort + ". " + s.ScrNameLongDesc }), "Value", "Text");
            //List<SelectListItem> ScrCode = new List<SelectListItem>();
            //ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode.Contains("ICC")).Select(s => new SelectListItem { Value = s.ScrCode, Text = s.ScrSort + ". " + s.ScrNameLongDesc }), "Value", "Text").ToList();
            //ViewBag.ScrCode = ScrCode;
            //ViewBag.ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode == "ICC").OrderBy(o => o.ScrID), "ScrCode", "ScrNameLongDesc");

            //fitri test
            List<SelectListItem> ScrCodeList = new List<SelectListItem>();
            ScrCodeList = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode.Contains("ICC")).OrderBy(o => o.ScrID), "ScrCode", "ScrNameLongDesc").ToList();
            ScrCodeList.Insert(0, new SelectListItem { Text = "Please Select", Value = "" });
            ViewBag.ScrCode = ScrCodeList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ScrCode)
        {
            var newScrCode = ScrCode.Replace(" ", "");

            if (newScrCode == null)
            {
                return RedirectToAction("Index", "Income");
            }
            if (newScrCode.Contains("I1"))
            {
                return RedirectToAction("Sawit", "PendapatanSawit");
            }
            if (newScrCode.Contains("I2"))
            {
                return RedirectToAction("Index", "PendapatanBijiBenih");
            }
            if (newScrCode.Contains("I3"))
            {
                return RedirectToAction("Index", "PendapatanProduk");
            }
            //return RedirectToAction(ScrCode.Replace(" ", ""), "Income");
            //return RedirectToAction("Index", "Income");
            return View();
        }
        public JsonResult GetScreen(string screencode)
        {
            var ScrData = db.bgt_Screens.Where(x => x.ScrCode == screencode);

            if (ScrData.Count() > 0)
            {
                return Json(new { success = true, ScCode = ScrData.FirstOrDefault().ScrCode });
            }
            return Json(new { success = false });
        }
    }
}