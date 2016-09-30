using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst
{
    public class Invitation
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public bool Accepted { get; set; }
        public bool Void { get; set; }
    }
}