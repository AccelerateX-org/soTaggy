using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoTaggy.Web.Areas.Tagging.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        //
        // GET: /Tagging/Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HowItWorks()
        {
            return View();
        }
    }
}
