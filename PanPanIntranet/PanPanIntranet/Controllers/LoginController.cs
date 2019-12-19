using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PanPanIntranet.ViewModels;

/* TODO
 * -Checking username and pw with DB data
 * -Logout redirection
 * -Password hiding in login view
 * -
 * 
 */

namespace PanPanIntranet.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AuthoriseLogin(LoginViewModel loginDetails)
        {

            Session["username"] = loginDetails.Username;
            //Session["role"] = **Get from DB via query** 

            return RedirectToAction("Index", "Home");
        }


        public ActionResult Logout()
        {
            //Clearing all Session variables
            Session.Abandon();
            //Might need to change this to see if it can redirect to the previous page it was on
            return RedirectToAction("Index", "Home");
        }
    }
}