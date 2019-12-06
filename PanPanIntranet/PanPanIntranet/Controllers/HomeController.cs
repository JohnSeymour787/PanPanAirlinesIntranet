using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using PanPanIntranet.Models;


/*
 * ToDo:
 * -Relative File Path for DB
 * -DB Connection String in another file
 * -try/catch exception handling for DB connection
 * 
 */


namespace PanPanIntranet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            OleDbConnection conn = new OleDbConnection(@"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = C:\Users\NAME\Documents\PanPanDB.mdb; Persist Security Info = True");
            OleDbCommand command = new OleDbCommand("select * from Employees");
            conn.Open();
            command.Connection = conn;
            OleDbDataReader reader = command.ExecuteReader();


            //Converting query result into list of Employee model classes
            List<Employee> employees = new List<Employee>();

            while (reader.Read())
            {
                //Attempting to convert string-stored Role column in DB to enum type used by model class
                bool roleExists = Enum.TryParse(reader.GetString(5), out Employee.CompanyRole role);

                employees.Add(new Employee()
                {
                    EmployeeID = (int)reader.GetValue(0),
                    LastName = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    Address = reader.GetString(3),
                    Phone = (int)reader.GetValue(4),
                    //If the parse was successful then the corresponding role is applied, otherwise, the default role value
                    //is assigned. (Note that Enum.TryParse will default to the first enum value if it can't succeed, but this 
                    //is safer.
                    Role = roleExists ? role : Employee.CompanyRole.None
                }
                );
            }

            conn.Close();
            return View(employees);
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