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
using HomeBrewing.Models.RecipeViewModels;

using HomeBrewing.Services;


namespace HomeBrewing.Controllers
{

    [Authorize]
    public class RecipeController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public RecipeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Recipe()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateRecipe()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRecipe(CreateRecipeViewModel model)
        {

            

            using (var db = new DatabaseContext())
            {

                db.Recipe.Add(new CreateRecipeViewModel { UserId = _userManager.GetUserId(User) , Title = model.Title , Details = model.Details , Requirements= model.Requirements });
                db.SaveChanges();

            }



            return RedirectToAction("Recipe", "Recipe");
        }
    }
}