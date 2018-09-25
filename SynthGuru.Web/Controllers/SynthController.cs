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
            var bll = new BusinessLayer.BusinessLayer();

            var allSynths = bll.GetAllSynthModels();
            var viewModel = new SynthViewModel()
            {
                SynthList = allSynths
            };

            return View(viewModel);
        }
    }
}