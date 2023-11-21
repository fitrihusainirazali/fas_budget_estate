using MVC_SYSTEM.ClassBudget;
using MVC_SYSTEM.ModelsBudget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.ControllersBudget
{
    public class ExpensesHQController : Controller
    {
        private MVC_SYSTEM_ModelsBudget db = new MVC_SYSTEM_ModelsBudget();

        // GET: ExpensesCC
        public ActionResult Index()
        {
            Screen screen = new Screen();
            ViewBag.Title = screen.GetScrHdr("EHQ").ScrHdrDesc;
            ViewBag.Screens = screen.GetScreens().Where(s => !string.IsNullOrEmpty(s.ScrHdrCode) && s.ScrHdrCode.Contains("EHQ")).ToList();

            ViewBag.ScrCode = new SelectList(db.bgt_Screens.Where(x => x.ScrHdrCode.Contains("EHQ")).OrderBy(o => o.ScrID), "ScrCode", "ScrNameLongDesc");

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(string ScrCode)
        //{

        //    if (ScrCode == "E1") // depreciation
        //    {
        //        return RedirectToAction("ExpDepListing", "ExpensesDepreciation");
        //    }
        //    if (ScrCode == "E2") // gaji kakitangan
        //    {
        //        return RedirectToAction("ExpSalListing", "ExpensesSalary");
        //    }
        //    if (ScrCode == "E3")// insurans
        //    {
        //        return RedirectToAction("ExpInsListing", "ExpensesInsurans");
        //    }
        //    if (ScrCode == "E4")//IT
        //    {
        //        return RedirectToAction("ExpITListing", "ExpensesIT");
        //    }
        //    if (ScrCode == "E5")// ctrlHQ
        //    {
        //        return RedirectToAction("ExpChqListing", "ExpensesCtrlHQ");
        //    }
        //    if (ScrCode == "E6")// CorpServ
        //    {
        //        return RedirectToAction("ExpCorpSListing", "ExpensesCorpServ");
        //    }
        //    if (ScrCode == "E7")//medical
        //    {
        //        return RedirectToAction("ExpMedListing", "ExpensesMedical");
        //    }
        //    if (ScrCode == "E8")// Windfall Tax
        //    {
        //        return RedirectToAction("ExpWindListing", "ExpensesWindfallTax");
        //    }
        //    //return RedirectToAction(ScrCode.Replace(" ", ""), "ExpensesCC");
        //    return RedirectToAction("Index", "Levi");
        //    //return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ScrCode)
        {
            var newScrCode = ScrCode.Replace(" ", "");

            if (newScrCode == "E1") // depreciation
            {
                return RedirectToAction("ExpDepListing", "ExpensesDepreciation");
            }
            if (newScrCode == "E2") // gaji kakitangan
            {
                return RedirectToAction("ExpSalListing", "ExpensesSalary");
            }
            if (newScrCode == "E3")// insurans
            {
                return RedirectToAction("ExpInsListing", "ExpensesInsurans");
            }
            if (newScrCode == "E4")//IT
            {
                //return RedirectToAction("ExpITListing", "ExpensesIT");
                return RedirectToAction("ButiranBelanjaIT", "bgtHQExpensesIT");
            }
            if (newScrCode == "E5")// ctrlHQ
            {
                //return RedirectToAction("ExpCHQListing", "ExpensesCtrlHQ");
                return RedirectToAction("Index", "bgtHQExpensesCtrlHQ");
            }
            if (newScrCode == "E6")// CorpServ
            {
                //return RedirectToAction("ExpCorpSListing", "ExpensesCorpServ");
                return RedirectToAction("Index", "bgtHQExpensesCorp");
            }
            if (newScrCode == "E7")//medical
            {
                return RedirectToAction("ExpMedListing", "ExpensesMedical");
            }
            if (newScrCode == "E8")// Windfall Tax
            {
                return RedirectToAction("ExpWindListing", "ExpensesWindfallTax");
            }
            return RedirectToAction("Index", "ExpensesHQ");
        }
    }
}
