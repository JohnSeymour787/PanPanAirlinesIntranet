using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using PanPanIntranet.Models;


namespace PanPanIntranet.Controllers
{
    public class EmployeeDetailsController : Controller
    {
        // GET: EmployeeDetails
        /// <summary>
        /// Only allows access for logged-in users
        /// Checks user role
        /// Queries DB and displays details of all users if current user has either manager or executive role
        /// Otherwise, only displays details of current user
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //If no logged in user then redirect to home page
            if ((Session["username"] == null) || (Session["role"] == null))
                return RedirectToAction("Index", "Home");

            //Parsing the user role text column in the DB to a CompanyRole enum type
            bool roleExists = Enum.TryParse((string)Session["role"], out Employee.CompanyRole role);

            //If there was a parsing error then also redirect to home
            if (!roleExists) return RedirectToAction("Index", "Home");

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand command = new OleDbCommand("select employeeID, lastName, firstName, address, phone, role from Employees");

            try
            {
                conn.ConnectionString = HomeController.connectionString;

                //Any role other than Executives or Managers will only display the details of that user, by limiting the DB query
                if ((role != Employee.CompanyRole.Executive) && (role != Employee.CompanyRole.Manager))
                {
                    //Thus, adding the where clause to the query string
                    command.CommandText += " where username = UN";
                    command.Parameters.Add("UN", OleDbType.VarChar).Value = Session["username"];
                }

                //Otherwise, for managers and executives the initial query string to return all tuples remains

                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();


                //Converting query result into list of Employee model classes
                List<Employee> employees = new List<Employee>();

                while (reader.Read())
                {
                    //Attempting to convert string-stored Role column in DB to enum type used by model class
                    roleExists = Enum.TryParse(reader.GetString(5), out role);

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

                return View(employees);
            }
            //If any exceptions occur during DB access, redirect to home
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
            finally
            { 
                conn.Close();
            }
        }
    }
}