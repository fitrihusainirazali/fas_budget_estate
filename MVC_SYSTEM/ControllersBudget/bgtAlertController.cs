using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using System.Web.Mvc;

namespace MVC_SYSTEM.ControllersBudget
{
    public class bgtAlertController : Controller
    {
        public ActionResult Denied()
        {
            ViewBag.Message = "You cannot access this page.";
            return View();
        }

        [AccessDeniedAuthorize(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User, Super User, Normal User")]
        public ActionResult AlreadySignIn()
        {
            GetIdentity name = new GetIdentity();
            ViewBag.Message = "System has been login by " + name.MyName(User.Identity.Name) + ". Please logout this user to login another user.";
            return View();
        }

        //public ActionResult error404()
        //{
        //    ViewBag.Message = "Sorry page is not exist. TQ";
        //    return View();
        //}

        //public ActionResult error400()
        //{
        //    ViewBag.Message = "Sorry you cannot go this page. TQ";
        //    return View();
        //}
    }
}