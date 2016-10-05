using CBLSummer09052016Budgeter.Models;
using CBLSummer09052016Budgeter.Models.CodeFirst;
using CBLSummer09052016Budgeter.Models.CodeFirst.Extensions;
using CBLSummer09052016Budgeter.Models.CodeFirst.Helpers;
using CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels;
using Microsoft.AspNet.Identity;
using MvcBreadCrumbs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBLSummer09052016Budgeter.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewIndex()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var hid = user.HouseholdId;
            
            var model = new DashboardViewModel();
            model.AccountList = new List<Account>();
            model.RecentTransactions = new List<Transaction>();
            if (user.HouseholdId != null)
            {
                model.AccountList = db.Accounts.NotVoid().Where(ho => ho.HouseholdId == hid).ToList();
            }
            else
            { return RedirectToAction("Create", "Households"); }

            foreach (var item in model.AccountList)
            { 
                foreach (var t in item.Transaction.NotVoid())
                {
                    BalanceHelper.IsReconciled(t.Id);
                    var tadd = db.Transactions.Find(t.Id);
                    model.RecentTransactions.Add(t);
                }              
            }
            model.RecentTransactions = model.RecentTransactions.OrderByDescending(d => d.Date).Take(5).ToList();
            model.NewTransaction = new Models.CodeFirst.Transaction();
            var h = db.Households.Find(hid);
            model.BudgetId = h.Budget.FirstOrDefault().Id;
            ViewBag.HouseholdId = db.Users.Find(User.Identity.GetUserId()).Household.Id;
            ViewBag.Household = db.Users.Find(User.Identity.GetUserId()).Household.Name;
            ViewBag.Date = DateTimeOffset.Now;
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            ViewBag.TransactionMethodId = new SelectList(db.TransactionMethods, "Id", "Name");

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetMonthly()
        {
            var uh = db.Users.Find(User.Identity.GetUserId()).HouseholdId;
            var household = db.Households.Find(uh);
            var monthToDate = Enumerable.Range(1, DateTime.Today.Month)
                .Select(m => new DateTime(DateTime.Today.Month, m, 1))
                .ToList();
            var bu = household.Budget.SelectMany(b => b.BudgetItem).ToList();
            var sums = from month in monthToDate
                       select new
                       {
                           month = month.ToString("MMM"),

                           income = (from account in household.Account
                                     from transaction in account.Transaction
                                     where transaction.TransactionType.Name == "Income" &&
                                            transaction.Date.Month == month.Month
                                     select transaction.Amount).DefaultIfEmpty().Sum(),

                           expense = (from account in household.Account
                                      from transaction in account.Transaction
                                      where transaction.TransactionType.Name == "Expense" &&
                                             transaction.Date.Month == month.Month
                                      select transaction.Amount).DefaultIfEmpty().Sum(),

                           budget = bu.Select(a => a.Amount).DefaultIfEmpty().Sum(),
                       };
            return Content(JsonConvert.SerializeObject(sums), "application/json");
        }

    }
}

