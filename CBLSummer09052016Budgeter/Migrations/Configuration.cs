namespace CBLSummer09052016Budgeter.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CBLSummer09052016Budgeter.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CBLSummer09052016Budgeter.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.TransactionTypes.AddOrUpdate(
                  p => p.Name,
                  new Models.CodeFirst.TransactionType { Name = "Income" },
                  new Models.CodeFirst.TransactionType { Name = "Expense" }
                );

            context.Categories.AddOrUpdate(
                  p => p.Name,
                  new Models.CodeFirst.Category { Name = "Rent/Mortgage" },
                  new Models.CodeFirst.Category { Name = "Utilities" },
                  new Models.CodeFirst.Category { Name = "Phone/Cellular" },
                  new Models.CodeFirst.Category { Name = "Automotive/Transportation" },
                  new Models.CodeFirst.Category { Name = "Entertainment" },
                  new Models.CodeFirst.Category { Name = "Investments" },
                  new Models.CodeFirst.Category { Name = "Groceries/Food" },
                  new Models.CodeFirst.Category { Name = "Clothing" },
                  new Models.CodeFirst.Category { Name = "Collected Rent" },
                  new Models.CodeFirst.Category { Name = "Earned Interest" },
                  new Models.CodeFirst.Category { Name = "Services Rendered Payment" },
                  new Models.CodeFirst.Category { Name = "Paycheck"},
                  new Models.CodeFirst.Category { Name = "Salary"},
                  new Models.CodeFirst.Category { Name = "Payment for Goods"}
                );

            context.TransactionMethods.AddOrUpdate(
                  p => p.Name,
                  new Models.CodeFirst.TransactionMethod { Name = "Cash" },
                  new Models.CodeFirst.TransactionMethod { Name = "Credit" },
                  new Models.CodeFirst.TransactionMethod { Name = "Check" }

                );
        }
    }
}
