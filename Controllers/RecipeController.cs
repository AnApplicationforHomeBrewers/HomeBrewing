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
            using (var db = new DatabaseContext())
            {
                var RecipeInfo = db.Recipe.Where(u => u.UserId == _userManager.GetUserId(User)).ToList();
                ViewBag.Recipe = RecipeInfo;
                return View();
            }

        }
        [HttpGet]
        public IActionResult CreateRecipe()
        {
            return View();
        }

        
        public class RecipeModel
        {
            public string details { get; set; }
            public string title { get; set; }
            public dynamic ingredients { get; set; }

        }

        [HttpPost]
        public IActionResult SaveDatas([FromBody] RecipeModel model)
        {


                        using (var db = new DatabaseContext())
                          {

                              var tempObject = new CreateRecipeViewModel
                              {
                                  UserId = _userManager.GetUserId(User),
                                  Title = model.title,
                                  Details = model.details,
                                  CreatedDate = DateTime.Now,
                                  EditedDate = DateTime.Now

                              };
                              db.Recipe.Add(tempObject);
                              db.SaveChanges();



                              foreach (dynamic data in model.ingredients)
                              {
                                  db.Ingredient.Add(new IngredientViewModel { RecipeId = tempObject.Id, Name = data["Name"], Quantity = data["Quantity"], MeasurementUnit = data["MeasurementUnit"] });
                                  db.SaveChanges();
                              }

                          }
                          
            return View("Recipe/Recipe");

        }


        
        public IActionResult RecipeDetail()
        {
            var RecipeId = Convert.ToInt32(RouteData.Values["id"]);

            using (var db = new DatabaseContext())
            {
                ViewBag.Recipe = db.Recipe.Where(i => i.Id == RecipeId).FirstOrDefault();
                ViewBag.Ingredient = db.Ingredient.Where(i => i.RecipeId == RecipeId).ToList();
                

            }
                return View();
        }
        




    }
}