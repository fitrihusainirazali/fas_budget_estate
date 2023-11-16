using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.ControllersBudget
{
    public class DetailRecordsController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();

        // GET: ExpensesCC
        public ActionResult Index()
        {
            Screen screen = new Screen();
            ViewBag.Title = screen.GetScrHdr("6BCO").ScrHdrDesc;
            ViewBag.Screens = screen.GetScreens().Where(s => !string.IsNullOrEmpty(s.ScrHdrCode) && s.ScrHdrCode.Contains("6BCO")).ToList();

            ViewBag.ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode == "6BCO").OrderBy(o => o.ScrID), "ScrCode", "ScrNameLongDesc");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ScrCode)
        {
            if (ScrCode == "B1")
            {
                return RedirectToAction("CCdetails", "CCdetailsRecs");
            }
            
            return RedirectToAction("Index", "Levi");
        }
    }
}
