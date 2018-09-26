using Newtonsoft.Json;
using SynthGuru.BusinessLayer;
using SynthGuru.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SynthGuru.Web.Controllers
{
    public class SynthController : Controller
    {
        // GET: Content
        public ActionResult Index()
        {
            return View();
            //var synthModelBL = new SynthModelBL();

            //var resp = synthModelBL.GetAllUnpacked();

            //return Json(resp.ReturnObj, "json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList()
        {
            var synthModelBL = new SynthModelBL();

            var resp = synthModelBL.GetAllUnpacked();

            return Json(new { data = resp.ReturnObj }, JsonRequestBehavior.AllowGet);
        }
    }
}