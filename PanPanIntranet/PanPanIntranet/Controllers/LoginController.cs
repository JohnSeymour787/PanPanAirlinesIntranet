using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PanPanIntranet.ViewModels;
using System.Data.OleDb;

/* TODO
 * -Logout redirection
 * -Password hiding in login view
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
            OleDbConnection conn = new OleDbConnection(HomeController.connectionString);
            //Using MS Access's StringCompare function to ensure query matching is case sensitive (is insensitive by default). See: https://stackoverflow.com/questions/10046627/how-to-write-case-sensitive-query-for-ms-access
            OleDbCommand command = new OleDbCommand("SELECT * FROM Employees WHERE(StrComp(username, UN, 0) = 0) AND (StrComp(password, PW, 0) = 0)");
            command.Parameters.Add("UN", OleDbType.VarWChar).Value = loginDetails.Username ?? "";
            command.Parameters.Add("PW", OleDbType.VarWChar).Value = loginDetails.Password ?? "";

            try
            {
                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();

                //Attempting to read the first row of the query result, if there is one
                if (reader.Read())
                {
                    Session["username"] = reader.GetString(6);
                    //Role is at column index 5 in the DB
                    Session["role"] = reader.GetString(5);
                    return RedirectToAction("Index", "Home");
                }
                //Otherwise, no rows to read
                else
                    return RedirectToAction("Index", "Login");
            }
            finally
            {
                conn.Close();
            }
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