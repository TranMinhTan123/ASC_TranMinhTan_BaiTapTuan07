using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranMinhTan_BTTuan07.Models;
namespace TranMinhTan_BTTuan07.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        ThietLapCauHinhDataContext data = new ThietLapCauHinhDataContext();
        public ActionResult Index()
        {
            return View();
        }
    }
}
