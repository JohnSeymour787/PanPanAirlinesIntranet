using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using PanPanIntranet.Models;
using PanPanIntranet.ViewModels;


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
                    employees.Add(CreateObject(reader));

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



        /// <summary>
        /// GET method to edit each employee's details based on their ID. When action is run, will query DB to get all details of specific employee, before 
        /// turning it into an Employee model object and passing this to the Edit.cshtml view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            OleDbConnection conn = new OleDbConnection();
            OleDbCommand command = new OleDbCommand("select employeeID, lastName, firstName, address, phone, role from Employees where employeeID = ID");
            command.Parameters.Add("ID", OleDbType.Integer).Value = id;
            
            try
            {
                conn.ConnectionString = HomeController.connectionString;
                conn.Open();
                command.Connection = conn;
                OleDbDataReader reader = command.ExecuteReader();
                reader.Read();

                Employee toEdit = CreateObject(reader);

                return View(toEdit);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "EmployeeDetails");
            }
            finally
            {
                conn.Close();
            }

        }

        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Adds an employee and all their details to the DB. Uses a view model combining the Employee model fields and 2 additional fields for username and password.
        /// </summary>
        /// <param name="employeeToAdd"></param>
        /// <returns></returns>
        public ActionResult AddEmployee(AddEmployeeViewModel employeeToAdd)
        {
            OleDbConnection conn = new OleDbConnection();
            OleDbCommand command = new OleDbCommand("insert into Employees ([lastName], [firstName], [address], [phone], [role], [username], [password]) values (?, ?, ?, ?, ?, ?, ?);");

            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToAdd.Employee.LastName;
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToAdd.Employee.FirstName;
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToAdd.Employee.Address;
            command.Parameters.Add("?", OleDbType.Integer).Value = employeeToAdd.Employee.Phone;
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToAdd.Employee.Role.ToString();
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToAdd.Username;
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToAdd.Password;


            try
            {
                conn.ConnectionString = HomeController.connectionString;
                conn.Open();
                command.Connection = conn;

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
            finally
            {
                conn.Close();
            }

            return RedirectToAction("Index", "EmployeeDetails");
        }

        /// <summary>
        /// POST method called upon submission of update form from Edit.cshtml. Takes updated Employee model details from that form
        /// and updates DB with new details.
        /// </summary>
        /// <param name="employeeToUpdate"></param>
        /// <returns></returns>
        public ActionResult UpdateUserDetails(Employee employeeToUpdate, bool edit)
        {
            OleDbConnection conn = new OleDbConnection();
            OleDbCommand command = new OleDbCommand("update Employees set " +
                "lastName=?, " +
                "firstName=?, " +
                "address=?, " +
                "phone=?, " +
                "role=? " +
                "where employeeID = ?");
                
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToUpdate.LastName;
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToUpdate.FirstName;
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToUpdate.Address;
            command.Parameters.Add("?", OleDbType.Integer).Value = employeeToUpdate.Phone;
            command.Parameters.Add("?", OleDbType.VarWChar).Value = employeeToUpdate.Role.ToString();
            command.Parameters.Add("?", OleDbType.Integer).Value = employeeToUpdate.EmployeeID;

            try
            {
                conn.ConnectionString = HomeController.connectionString;
                conn.Open();
                command.Connection = conn;
                
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                
            }
            finally
            {
                conn.Close();
            }

            return RedirectToAction("Index", "EmployeeDetails");
        }


        /// <summary>
        /// Creates an Employee model object from an OleDbDataReader resulting from a select query.
        /// Creates an Employee model for the current record the reader is pointing to.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Employee CreateObject(OleDbDataReader reader)
        {
            Employee result = new Employee();

            result.EmployeeID = (int)reader.GetValue(0);
            result.LastName = reader.GetString(1);
            result.FirstName = reader.GetString(2);
            result.Address = reader.GetString(3);
            result.Phone = (int)reader.GetValue(4);
            //If a parse was successful then the corresponding role is applied, otherwise, the default role value
            //is assigned. (Note that Enum.TryParse will default to the first enum value if it can't succeed, but this 
            //is safer.
            bool roleExists = Enum.TryParse(reader.GetString(5), out Employee.CompanyRole role);
            result.Role = roleExists ? role : Employee.CompanyRole.None;

            return result;
        }
    }
}