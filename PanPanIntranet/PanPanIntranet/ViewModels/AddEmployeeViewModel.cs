using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PanPanIntranet.Models;

namespace PanPanIntranet.ViewModels
{
    public class AddEmployeeViewModel
    {
        public Employee Employee { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}