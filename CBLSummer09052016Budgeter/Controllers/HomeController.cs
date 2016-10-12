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
            model.NewTransaction = new Transaction();
            var h = db.Households.Find(hid);
            if (h.Budget.Count != 0) { model.BudgetId = h.Budget.FirstOrDefault().Id; }
            ViewBag.HouseholdId = db.Users.Find(User.Identity.GetUserId()).Household.Id;
            ViewBag.Household = db.Users.Find(User.Identity.GetUserId()).Household.Name;
            
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
            //var monthToDate = Enumerable.Range(1, DateTime.Today.Month)
            //    .Select(m => new DateTime(DateTime.Today.Month, m, 1))
            //    .ToList();
            var month = new DateTime(DateTime.Today.Month);
            var bu = household.Budget.SelectMany(b => b.BudgetItem).ToList();
            //var sums = from month in monthToDate
            var sums =
                       /*select */new
                                  {


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
                                      month = month.ToString("MMMM")

                                  };
            object[] data1 = new object[2];
            object[] data2 = new object[2];
            object[] data3 = new object[2];
            data1[0] = "Income";
            data1[1] = sums.income;            
            object[] data = new object[3];
            data[0] = data1;
            data2[0] = "Expense";
            data2[1] = sums.expense;
            data[1] = data2;
            data3[0] = "Budget";
            data3[1] = sums.budget;
            data[2] = data3;

            //{new {   "Income", sums.income },
            //    new { "Expense", sums.expense },
            //    new { "Budget", sums.budget }};

            if (sums.income == 0 && sums.expense == 0)
            {
                ViewBag.Message = "There are no transactions for this month.  Here are last month's transactions:";
                var lastMonth = DateTime.Now.Month - 1;
                var lastMoSums =
                    new
                    {


                        income = (from account in household.Account
                                  from transaction in account.Transaction
                                  where transaction.TransactionType.Name == "Income" &&
                                                     transaction.Date.Month == lastMonth
                                  select transaction.Amount).DefaultIfEmpty().Sum(),

                        expense = (from account in household.Account
                                   from transaction in account.Transaction
                                   where transaction.TransactionType.Name == "Expense" &&
                                                      transaction.Date.Month == lastMonth
                                   select transaction.Amount).DefaultIfEmpty().Sum(),

                        budget = bu.Select(a => a.Amount).DefaultIfEmpty().Sum(),
                        month = lastMonth.ToString("MMMM")
                    };

                data1[0] = "Income";
                data1[1] = sums.income;
               
                data[0] = data1;
                data2[0] = "Expense";
                data2[1] = sums.expense;
                data[1] = data2;
                data3[0] = "Budget";
                data3[1] = sums.budget;
                data[2] = data3;
                string datastr = JsonConvert.SerializeObject(data, Formatting.None);
                ViewBag.data = new HtmlString(datastr);
                return Content(JsonConvert.SerializeObject(data, Formatting.None));
            }
            else
            {
                string datastr = JsonConvert.SerializeObject(data, Formatting.None);
                ViewBag.data = new HtmlString(datastr);
                return Content(JsonConvert.SerializeObject(data, Formatting.None));
            };
        }

        public JsonResult GetPieChartData()
        {

            var uh = db.Users.Find(User.Identity.GetUserId()).HouseholdId;
            var household = db.Households.Find(uh);
            //var monthToDate = Enumerable.Range(1, DateTime.Today.Month)
            //    .Select(m => new DateTime(DateTime.Today.Month, m, 1))
            //    .ToList();
            var month = DateTime.Today.Month;
            var bu = household.Budget.SelectMany(b => b.BudgetItem).ToList();
            //var sums = from month in monthToDate
            var sums =
                       /*select */new
                                  {


                                      income = (from account in household.Account
                                                from transaction in account.Transaction
                                                where transaction.TransactionType.Name == "Income" &&
                                                       transaction.Date.Month == month
                                                select transaction.Amount).DefaultIfEmpty().Sum(),

                                      expense = (from account in household.Account
                                                 from transaction in account.Transaction
                                                 where transaction.TransactionType.Name == "Expense" &&
                                                        transaction.Date.Month == month
                                                 select transaction.Amount).DefaultIfEmpty().Sum(),

                                      budget = bu.Select(a => a.Amount).DefaultIfEmpty().Sum(),
                                      month = month.ToString("MMMM")

                                  };


            var data = new[]{new {  Name = "Income", Value = sums.income },
                new { Name =  "Expense", Value = sums.expense },
                new { Name = "Budget", Value = sums.budget }};

            if (sums.income == 0 && sums.expense == 0)
            {
                ViewBag.Message = "There are no transactions for this month.  Here are last month's transactions:";
                var lastMonth = DateTime.Now.Month - 1;
                var lastMoSums =
                    new
                    {


                        income = (from account in household.Account
                                  from transaction in account.Transaction
                                  where transaction.TransactionType.Name == "Income" &&
                                                     transaction.Date.Month == lastMonth
                                  select transaction.Amount).DefaultIfEmpty().Sum(),

                        expense = (from account in household.Account
                                   from transaction in account.Transaction
                                   where transaction.TransactionType.Name == "Expense" &&
                                                      transaction.Date.Month == lastMonth
                                   select transaction.Amount).DefaultIfEmpty().Sum(),

                        budget = bu.Select(a => a.Amount).DefaultIfEmpty().Sum(),
                        month = lastMonth.ToString("MMMM")
                    };

                data = new[]{new {  Name = "Income", Value = sums.income },
                new { Name =  "Expense", Value = sums.expense },
                new { Name = "Budget", Value = sums.budget }};
                return Json(data);
            }
            else
            {



                return Json(data);
            }
        }

    }
}

