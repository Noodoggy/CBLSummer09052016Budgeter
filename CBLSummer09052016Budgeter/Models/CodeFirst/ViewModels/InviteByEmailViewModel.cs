using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels
{
    public class InviteByEmail
    {
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        public int HouseholdId { get; set; }
    }
}