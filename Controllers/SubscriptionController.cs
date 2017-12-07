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
            return View();
        }

        public IActionResult Search(string NameQuery){
            using (var db = new DatabaseContext()){
                var users = db.AspNetUsers.Where(u => u.Name == NameQuery).ToList();
                ViewBag.Users = users;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Subscribe (string subscribedUserID){
            using (var db = new DatabaseContext()){
                var newSubscription = new SubscriptionInfoViewModel
                              {
                                  FollowerUserID = _userManager.GetUserId(User),
                                  FollowedUserID = subscribedUserID
                              };
                              db.Subscription.Add(newSubscription);
                              db.SaveChanges();
                return View("Index");
            }
        }
    }
}