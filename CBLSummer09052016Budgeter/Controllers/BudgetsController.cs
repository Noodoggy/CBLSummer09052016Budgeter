using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CBLSummer09052016Budgeter.Models;
using CBLSummer09052016Budgeter.Models.CodeFirst;
using CBLSummer09052016Budgeter.Models.CodeFirst.Extensions;
using CBLSummer09052016Budgeter.Models.CodeFirst.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using CBLSummer09052016Budgeter.Models.CodeFirst.Helpers;

namespace CBLSummer09052016Budgeter.Controllers
{
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index()
        {
            var budgets = new BudgetViewModel();
            budgets.BudgetList = db.Budgets.Include(b => b.Household).Include(bi => bi.BudgetItem).ToList();
            budgets.NewBudgetItem = new BudgetItem();
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.HouseholdId = db.Users.Find(User.Identity.GetUserId()).Household.Id;
            budgets.NewBudget = new Budget();
            budgets.NewBudget.HouseholdId = ViewBag.HouseholdId;

            return View(budgets);
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = db.Users.Find(User.Identity.GetUserId()).Household.Id; 

            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HouseholdId")] Budget budget)
        {
            budget.Household = db.Households.Find(budget.HouseholdId);
            if (ModelState.IsValid)
            {
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HouseholdId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            db.Budgets.Remove(budget);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult GetChart(int id)
        {
            //original-- tryng to go through accounts instead.
            var uh = db.Users.Find(User.Identity.GetUserId()).HouseholdId;
            var house = db.Households.Find(uh);
            var tod = DateTimeOffset.Now;
            decimal totalExpense = 0;
            decimal totalBudget = 0;
            var totalAcc = (from a in house.Account
                            select a.Balance).DefaultIfEmpty().Sum();

            var bar = (from c in house.Account.SelectMany(t => t.Transaction).ToList()

                       where c.TransactionType.Name == "Expense"

                       let aSum = (from t in house.Account.SelectMany(t => t.Transaction).ToList()
                                   where t.Date.Year == tod.Year && t.Date.Month == tod.Month && t.Category == c.Category
                                   select t.Amount).DefaultIfEmpty().Sum()

                       let bSum = (from b in house.Budget.SelectMany(bi => bi.BudgetItem).ToList()
                                   where b.Category == c.Category
                                   select b.Amount).DefaultIfEmpty().Sum()

                       let _ = totalExpense += aSum
                       let __ = totalBudget += bSum

                       select new
                       {
                           Category = c.Category.Name,
                           Actual = aSum,
                           Budgeted = bSum
                       }).ToArray();

            //attempt to go through Accounts
            //    var uh = db.Users.Find(User.Identity.GetUserId()).HouseholdId;
            //    var houseId = db.Households.Find(uh).Id;
            //    var account = db.Accounts.Where(h => h.HouseholdId == houseId);
            //    var tod = DateTimeOffset.Now;
            //    decimal totalExpense = 0;
            //    decimal totalBudget = 0;


            //    var bar = (from c in account.SelectMany(t => t.Transaction).ToList()

            //               where c.TransactionType.Name == "Expense"

            //               let aSum = (from t in account.SelectMany(t => t.Transaction).ToList()
            //                           where t.Date.Year == tod.Year && t.Date.Month == tod.Month && t.Category == c.Category
            //                           select t.Amount).DefaultIfEmpty().Sum()

            //               let bSum = (from b in account.Where(g => g.Transaction.Where(c.Category.BudgetItem == )
            //                           where b.Transaction == c.Category
            //                           select b.Amount).DefaultIfEmpty().Sum()

            //               let _ = totalExpense += aSum
            //               let __ = totalBudget += bSum

            //               select new
            //               {
            //                   Category = c.Category.Name,
            //                   Actual = aSum,
            //                   Budgeted = bSum
            //               }).ToArray();


            //trying to use Antonio's code
            //var u = User.Identity.GetUserId();
            //var h = db.Users.Find(u).HouseholdId;
            //var house = db.Households.Find(h);
            //var tod = DateTimeOffset.Now;
            //decimal totalExpense = 0;
            //decimal totalBudget = 0;
            //var totalAcc = (from a in house.Account
            //                select a.Balance).DefaultIfEmpty().Sum();


            //var bar = (from y in house.Category
            //           from x in y.TransactionType
            //           where x.Name == "Expense" 

            //           let aSum = (from c in house.Account.Select(c => c.Transaction)
            //                       from t in c.Where(t => t.Category.Name == y.Name)
            //                       where t.Date.Year == tod.Year && t.Date.Month == tod.Month && t.Category.Name == y.Name
            //                       select t.Amount).DefaultIfEmpty().Sum()

            //           let bSum = (from b in house.Budget.Where(i => i.Id == id)
            //                       from w in b.BudgetItem.Where(z => z.CategoryId == y.Id)
            //                       select w.Amount).DefaultIfEmpty().Sum()

            //           let _ = totalExpense += aSum
            //           let __ = totalBudget += bSum

            //           select new
            //           {
            //               Category = y.Name,
            //               Actual = aSum,
            //               Budgeted = bSum
            //           }).ToArray();

            //var donut = (from c in house.Category

            //             where c.Name == "Expense"
            //             let aSum = (from t in c.Transaction
            //                         where t.Date.Year == tod.Year && t.Date.Month == tod.Month
            //                         select t.Amount).DefaultIfEmpty().Sum()

            //             select new
            //             {
            //                 label = c.Name,
            //                 value = aSum
            //             }).ToArray();


            var donut = (from c in house.Account.SelectMany(t => t.Transaction).ToList()
                         where c.TransactionType.Name == "Expense"
                         let aSum = (from t in house.Account.SelectMany(t => t.Transaction).ToList()
                                     where t.Date.Year == tod.Year && t.Date.Month == tod.Month
                                     select t.Amount).DefaultIfEmpty().Sum()

                         select new
                         {
                             label = c.Category.Name,
                             value = aSum
                         }).ToArray();


            var result = new
            {
                totalAcc = totalAcc,
                totalBudget = totalBudget,
                totalExpense = totalExpense,
                bar = bar,
                donut = donut
            };

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        public ActionResult GetMonthly()
        {
            var uh = db.Users.Find(User.Identity.GetUserId()).HouseholdId;
            var household = db.Households.Find(uh);
            var monthsToDate = Enumerable.Range(1, DateTime.Today.Month)
                .Select(m => new DateTime(DateTime.Today.Year, m, 1))
                .ToList();
            var bu = household.Budget.SelectMany(b => b.BudgetItem).ToList();
            var sums = from month in monthsToDate
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



protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
