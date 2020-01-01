using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using PanPanIntranet.Models;


/*
 * ToDo:
 * -DB Connection String in another file <-- Maybe
 */


namespace PanPanIntranet.Controllers
{
    public class HomeController : Controller
    {
        public const string connectionString = @"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = |DataDirectory|\PanPanDB.mdb; Persist Security Info = True";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}