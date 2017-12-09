using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HomeBrewing.Models;
using HomeBrewing.Models.SubscriptionViewModels;

using HomeBrewing.Services;

namespace HomeBrewing.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            using (var db = new DatabaseContext())
            {
                var tempData = (from subsTable in db.Subscription
                                join userTable in db.AspNetUsers
                                on subsTable.FollowerUserID equals userTable.Id
                                where subsTable.FollowedUserID == _userManager.GetUserId(User)
                                && subsTable.Status == 0
                                select new { subsTable, userTable }).ToList();
                List<JoinedTableViewModel> list = new List<JoinedTableViewModel>();

                foreach (var data in tempData)
                {
                    JoinedTableViewModel temp = new JoinedTableViewModel();
                    temp.FollowedUserId = data.subsTable.FollowedUserID;
                    temp.FollowerUserId = data.subsTable.FollowerUserID;
                    temp.Name = data.userTable.Name;
                    temp.Surname = data.userTable.Surname;
                    temp.Status = data.subsTable.Status;
                    temp.UserName = data.userTable.Surname;
                    list.Add(temp);                 


                }
                ViewBag.Followers = list;

                return View();
            }
            


        }

        public IActionResult Search(string NameQuery){
            using (var db = new DatabaseContext()){
                var users = db.AspNetUsers.Where(u => u.Name == NameQuery).ToList();
                ViewBag.Users = users;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Subscribe(string subscribedUserID)
        {
            using (var db = new DatabaseContext())
            {
                var isAlreadyAdded = db.Subscription.Any(u => u.FollowerUserID == _userManager.GetUserId(User) && u.FollowedUserID == subscribedUserID);
                if (!isAlreadyAdded) { 
                var newSubscription = new SubscriptionInfoViewModel
                {
                    FollowerUserID = _userManager.GetUserId(User),
                    FollowedUserID = subscribedUserID,
                    Status = 1
                };
                db.Subscription.Add(newSubscription);
                db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult SubscribeToPrivate(string subscribedUserID)
        {
            using (var db = new DatabaseContext())
            {
                var isAlreadyAdded = db.Subscription.Any(u => u.FollowerUserID == _userManager.GetUserId(User) && u.FollowedUserID == subscribedUserID);
                if (!isAlreadyAdded)
                {
                    var newSubscription = new SubscriptionInfoViewModel
                    {
                        FollowerUserID = _userManager.GetUserId(User),
                        FollowedUserID = subscribedUserID,
                        Status = 0
                    };
                    db.Subscription.Add(newSubscription);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult ConfirmInvitation(string subscriberUserID)
        {
            using (var db = new DatabaseContext())
            {
                
                var findInvitation = db.Subscription.Where(u => u.FollowerUserID == subscriberUserID && u.FollowedUserID == _userManager.GetUserId(User)).FirstOrDefault();
                findInvitation.Status = 1;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult RejectInvitation(string subscriberUserID)
        {
            using (var db = new DatabaseContext())
            {
                var findInvitation = db.Subscription.Where(u => u.FollowerUserID == subscriberUserID && u.FollowedUserID == _userManager.GetUserId(User)).FirstOrDefault();
                db.Subscription.Remove(findInvitation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}