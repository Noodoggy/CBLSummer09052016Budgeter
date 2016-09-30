using CBLSummer09052016Budgeter.Models.CodeFirst.Helpers;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.Extensions
{
    public static class Extensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        
        //household identity claim authorization

        public static string GetHouseholdId(this IIdentity user)     //create housedhold claim
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            if (HouseholdClaim != null)
                return HouseholdClaim.Value;
            else
                return null;
        }

        public static bool IsInHousehold(this IIdentity user)           //check to see if in household
        {
            var cUser = (ClaimsIdentity)user;
            var hid = cUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            return (hid != null && !string.IsNullOrWhiteSpace(hid.Value));
        }

        public class AuthorizeHouseholdRequired : AuthorizeAttribute        //inherit attribute class
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)      //override method to check for IsInHousehold
            {
                var isAuthorized = base.AuthorizeCore(httpContext);
                if (!isAuthorized)
                {
                    return false;
                }
                return httpContext.User.Identity.IsInHousehold();
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)       //override to handle unauthorized access or authorized
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Dashboard" }));
                }
            }
        }

        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)      //log out user and refresh authentication
            {
                context.GetOwinContext().Authentication.SignOut();
                await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }


        //invitation idnetity claim authorization


        public static string GetInvitationId(this IIdentity user)     //create housedhold claim
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var InvitationClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "InvitationId");
            if (InvitationClaim != null)
                return InvitationClaim.Value;
            else
                return null;
        }

        public static bool IsInvited(this IIdentity user)           //check to see if in household
        {
            var cUser = (ClaimsIdentity)user;
            var hid = cUser.Claims.FirstOrDefault(c => c.Type == "InvitationId");
            return (hid != null && !string.IsNullOrWhiteSpace(hid.Value));
        }

        public class AuthorizeInvitationRequired : AuthorizeAttribute        //inherit attribute class
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)      //override method to check for IsInHousehold
            {
                var isAuthorized = base.AuthorizeCore(httpContext);
                if (!isAuthorized)
                {
                    return false;
                }
                return httpContext.User.Identity.IsInvited();
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)       //override to handle unauthorized access or authorized
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Dashboard" }));
                }
            }
        }

            public static IEnumerable<Transaction> NotVoid(this IEnumerable<Transaction> transactions)
            {
                return transactions.Where(v => !v.Void);
            }

        public static IEnumerable<Account> NotVoid(this IEnumerable<Account> accounts)
        {
            return accounts.Where(v => !v.Void);
        }

        public static IEnumerable<Household> NotVoid(this IEnumerable<Household> households)
        {
            return households.Where(v => !v.Void);
        }

        public static IEnumerable<Budget> NotVoid(this IEnumerable<Budget> budgets)
        {
            return budgets.Where(v => !v.Void);
        }

        public static IEnumerable<BudgetItem> NotVoid(this IEnumerable<BudgetItem> budgetItems)
        {
            return budgetItems.Where(v => !v.Void);
        }

        public static IEnumerable<Invitation> NotVoid(this IEnumerable<Invitation> invitation)
        {
            return invitation.Where(v => !v.Void);
        }

        public static IEnumerable<Transaction> Past30Days(this IEnumerable<Transaction> pastTransactions)
        {
            return pastTransactions.Where(u => DateTime.UtcNow - u.Date <= TimeSpan.FromDays(30));
        }

        public static IEnumerable<Transaction> CalendarMonthToDate (this IEnumerable<Transaction> MTDTransactions)
        {
            return MTDTransactions.Where(u => u.Date.Month == DateTime.UtcNow.Month);
        }

        public static IEnumerable<Transaction> CalendarYearToDate (this IEnumerable<Transaction> YTDTransactions)
        {
            return YTDTransactions.Where(u => u.Date.Year == DateTime.UtcNow.Year);
        }

        public static decimal IndexBalance(this Account account, DateTime date)
        {


            var accountId = account.Id;
            
            //var transBalance = a.NotVoid().FromYear(date.Year).FromMonth(date.Month).FromDay(date.Day).Select(t => t.Amount).Sum();
            var balance = BalanceHelper.GetBalance(accountId, date);
            return balance;
        }

        public static decimal IndexReconciledBalance(this Account account, DateTime date)
        {


            var accountId = account.Id;

            //var transBalance = a.NotVoid().FromYear(date.Year).FromMonth(date.Month).FromDay(date.Day).Select(t => t.Amount).Sum();
            var balance = BalanceHelper.GetReconciledBalance(accountId, date);
            return balance;
        }

        public static IEnumerable<Transaction> BeforeDate (this IEnumerable<Transaction> beforeDateTransactions, DateTime date)
        {
            return beforeDateTransactions.NotVoid().Where(y => y.Date.CompareTo(date) < 0);
        }

        public static IEnumerable<Transaction>AreReconciled(this IEnumerable<Transaction> transactions)
        {
            return transactions.Where(v => v.Reconciled);
        }


        //public static IEnumerable<Transaction> FromMonth(this IEnumerable<Transaction> monthTransactions, int month)
        //{
        //    return monthTransactions.Where(u => DateTime.UtcNow.Month - u.Date.Month >= DateTime.UtcNow.Month - month);
        //}

        //public static IEnumerable<Transaction> FromDay(this IEnumerable<Transaction> dayTransactions, int day)
        //{
        //    return dayTransactions.Where(u => DateTime.UtcNow.Day - u.Date.Day > DateTime.UtcNow.Day - day);
        //}
    }
}