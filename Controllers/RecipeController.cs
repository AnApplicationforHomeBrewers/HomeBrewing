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
using HomeBrewing.Models.SubscriptionViewModels;

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
                var recipeObject = db.Recipe.Where(i => i.Id == RecipeId).FirstOrDefault();
                ViewBag.Recipe = recipeObject;
                ViewBag.Ingredient = db.Ingredient.Where(i => i.RecipeId == RecipeId).ToList();
                HomeBrewing.Models.AccountViewModels.PrivateAccountViewModel recipeUser;
                using (var db2 = new UserContext())
                {
                    recipeUser = db2.AspNetUsers.Where(u => u.Id == recipeObject.UserId).FirstOrDefault();

                }


                if (recipeUser.PrivateAccount == 1) { 
                    if (_userManager.GetUserId(User) != recipeObject.UserId) {
                        var subscriptionObject = db.Subscription.Any(f => f.FollowerUserID == _userManager.GetUserId(User) && f.FollowedUserID == recipeObject.UserId);
                        if (subscriptionObject)
                        {
                            return View();
                        }
                        else
                        {
                            return View("PermissionDenied");
                        }

                    }
                }
                var commentObject = db.RecipeComment.Where(c => c.RecipeId == RecipeId).ToList();
                var usernames = new List<String>();
                foreach (var item in commentObject){
                    usernames.Add(db.AspNetUsers.Where(u => u.Id == item.UserId).Select(s => s.Name).FirstOrDefault());
                }
                ViewBag.UserNames = usernames;
                ViewBag.Comments = commentObject;

            }

            

            return View();

           
               
        }

        public IActionResult PermissionDenied()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Comment(string _comment, int _recipeId){

            

            using (var db = new DatabaseContext()){
                var newComment = new CommentViewModel
                {
                    Comment = _comment,
                    RecipeId = _recipeId,
                    UserId = _userManager.GetUserId(User)
                };
                db.RecipeComment.Add(newComment);
                db.SaveChanges();
            
            }

            return RedirectToAction("RecipeDetail", new { id = _recipeId });
        }
        
        /*[HttpPost]
        public IActionResult RecipeDetail(string comment){
            var _RecipeId = Convert.ToInt32(RouteData.Values["id"]);

            using (var db = new DatabaseContext()){
                var tempObject = new CommentViewModel{
                    Comment = comment,
                    UserId = _userManager.GetUserId(User),
                    RecipeId = _RecipeId,
                };
                
                db.RecipeComment.Add(tempObject);
                db.SaveChanges();
            }

            

            using (var db = new DatabaseContext())
            {
                var recipeObject = db.Recipe.Where(i => i.Id == _RecipeId).FirstOrDefault();
                ViewBag.Recipe = recipeObject;
                ViewBag.Ingredient = db.Ingredient.Where(i => i.RecipeId == _RecipeId).ToList();
                HomeBrewing.Models.AccountViewModels.PrivateAccountViewModel recipeUser;
                using (var db2 = new UserContext())
                {
                    recipeUser = db2.AspNetUsers.Where(u => u.Id == recipeObject.UserId).FirstOrDefault();

                }


                if (recipeUser.PrivateAccount == 1) { 
                    if (_userManager.GetUserId(User) != recipeObject.UserId) {
                        var subscriptionObject = db.Subscription.Any(f => f.FollowerUserID == _userManager.GetUserId(User) && f.FollowedUserID == recipeObject.UserId);
                        if (subscriptionObject)
                        {
                            return View();
                        }
                        else
                        {
                            return View("PermissionDenied");
                        }

                    }
                }
            }


            

            return View();
        }*/



    }
}