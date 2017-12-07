using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeBrewing.Models;
using HomeBrewing.Models.SubscriptionViewModels;
using HomeBrewing.Models.RecipeViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomeBrewing.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            using (var db = new DatabaseContext())
            {
                var subscribedUsersInfo = db.Subscription.Where(s => s.FollowerUserID == _userManager.GetUserId(User)).ToList();
                var RecipesInfo = new List<List<CreateRecipeViewModel>>();
                foreach (var item in subscribedUsersInfo)
                {
                    var recipes = db.Recipe.Where(r => r.UserId == item.FollowedUserID).ToList();
                    RecipesInfo.Add(recipes);
                }
                ViewBag.Recipes = RecipesInfo;
                return View();
            }
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Home Brewing.";

            return View();
        }

        public IActionResult Contact()
        {
            
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
