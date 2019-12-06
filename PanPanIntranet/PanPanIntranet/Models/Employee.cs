using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PanPanIntranet.Models
{
    public class Employee
    {
        public enum CompanyRole
        {
            None,
            Admin,
            Pilot,
            FlightAttendant,
            Manager,
            Executive
        }


        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public int Phone { get; set; }
        public CompanyRole Role { get; set; }
    }
}