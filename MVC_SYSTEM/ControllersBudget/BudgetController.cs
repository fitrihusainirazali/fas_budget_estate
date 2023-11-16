using MVC_SYSTEM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.ControllersBudget
{
    [AccessDeniedAuthorize(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User,Exe Budget")]
    public class BudgetController : Controller
    {
        // GET: Budget
        public ActionResult Index()
        {
            return View();
        }
    }
}
