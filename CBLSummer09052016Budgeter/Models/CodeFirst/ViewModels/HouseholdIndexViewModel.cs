using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels
{
    public class HouseholdIndexViewModel
    {
        public List<Household> List { get; set; }
        public List<ApplicationUser> UserList { get; set; }
    }
}