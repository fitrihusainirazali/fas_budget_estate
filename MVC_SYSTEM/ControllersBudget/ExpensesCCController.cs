using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.ControllersBudget
{
    public class ExpensesCCController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();

        // GET: ExpensesCC
        public ActionResult Index()
        {
            Screen screen = new Screen();
            ViewBag.Title = screen.GetScrHdr("ECC").ScrHdrDesc;
            ViewBag.Screens = screen.GetScreens().Where(s => !string.IsNullOrEmpty(s.ScrHdrCode) && s.ScrHdrCode.Contains("ECC")).ToList();

            //Temporary add
            //ViewBag.ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode.Contains("ECC")).OrderBy(o => o.ScrID).Select(s => new SelectListItem { Value = s.ScrCode, Text = s.ScrSort + ". " + s.ScrNameLongDesc }), "Value", "Text");
            //List<SelectListItem> ScrCode = new List<SelectListItem>();
            //ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode.Contains("ECC")).Select(s => new SelectListItem { Value = s.ScrCode, Text = s.ScrSort + ". " + s.ScrNameLongDesc }), "Value", "Text").ToList();
            //ViewBag.ScrCode = ScrCode;
            //ViewBag.ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode == "ECC").OrderBy(o => o.ScrID), "ScrCode", "ScrNameLongDesc");
           
            //fitri test
            List<SelectListItem> ScrCodeList = new List<SelectListItem>();
            ScrCodeList = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode.Contains("ECC")).OrderBy(o => o.ScrID), "ScrCode", "ScrNameLongDesc").ToList();
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
                return RedirectToAction("Index", "ExpensesCC");
            }
            if (newScrCode.Contains("E9"))
            {
                return RedirectToAction("Index", "ExpensesCapital");
            }
            if (newScrCode.Contains("E10"))
            {
                return RedirectToAction("Index", "ExpensesPAU");
            }
            if (newScrCode.Contains("E11"))
            {
                return RedirectToAction("Index", "Levi");
            }
            //fitri change
            if (newScrCode == "E12")
            {
                return RedirectToAction("Index", "ExpensesGaji");
            }
            if (newScrCode.Contains("E13"))
            {
                return RedirectToAction("Index", "PendaftaranKenderaan");
            }
            if (newScrCode.Contains("E14"))
            {
                return RedirectToAction("Index", "PerbelanjaanKenderaan");
            }
            if (newScrCode.Contains("E15"))
            {
                return RedirectToAction("Index", "ExpensesOther");
            }
            //end fitri

            //return RedirectToAction(ScrCode.Replace(" ", ""), "ExpensesCC");
            //return RedirectToAction("Index", "ExpensesCC");
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
