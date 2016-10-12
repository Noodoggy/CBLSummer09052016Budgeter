using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels
{
    public class InviteViewModel
    {
        public string HouseholdName { get; set; }
        //public virtual Household Household{ get; set; }
        //public string HouseholdName { get; set; }
        public string UserToAddName { get; set; }
        //public string[] selected { get; set; }
        public int HouseId { get; set; }
        public string UserId { get; set; }
        
    }

}
