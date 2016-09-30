using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBLSummer09052016Budgeter.Models.CodeFirst.Helpers
{
    public class HouseholdUsersHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsInHousehold(string user, int? householdId)
        {
            var flag = new bool();
            if (householdId != null)
            {
                var isInHousehold = db.Households.FirstOrDefault(p => p.Id == householdId);//find project
                flag = isInHousehold.User.Any(u => u.Id == user);//query if user on project
            }
            else
            {
                flag = false;
            }
            return flag;//return true or false
        }

        public IList<string> ListHouseholdUsers(int householdId)
        {
            var household = db.Households.Find(householdId);
            IList<string> members = new List<string>();
            foreach (var item in household.User)
            {
                members.Add(item.Id);
            }
            return (members);
        }

        public void AddToHousehold(string userId, int? householdId)
        {
            if (!IsInHousehold(userId, householdId))            //if user is not on project
            {
                ApplicationUser user = db.Users.Find(userId);           //find user
                Household household = db.Households.Find(householdId);            //select project
                household.User.Add(user);                     //add to project, else do nothing
                db.SaveChanges();
            }
        }

        public void RemoveFromHousehold(string userId, int? householdId)
        {
            if (IsInHousehold(userId, householdId))         //if user is on project
            {
                ApplicationUser user = db.Users.Find(userId);               //find user
                Household household = db.Households.First(p => p.Id == householdId);            //select project
                household.User.Remove(user);          //remove from project, else do nothing
                db.SaveChanges();
            }
        }
        //InviteCheck()
        //{

        //}
    }
}