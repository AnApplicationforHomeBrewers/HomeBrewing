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

                //Suggested Recipes Query
                var likedRecipes = db.RecipeLike.Where(u => u.UserId == _userManager.GetUserId(User)).Select(r => r.RecipeId).ToList();
                var usersLikedRecipes = new List<string>();
                foreach(var recipe in likedRecipes){
                    var users = db.RecipeLike.Where(r => r.RecipeId == recipe).Select(u => u.UserId).ToList();
                    foreach(var user in users){
                        if((user == _userManager.GetUserId(User)) || (user == usersLikedRecipes.Find(u => u == user)))
                        {
                            continue;
                        }
                        else 
                        {
                            usersLikedRecipes.Add(user);
                        }
                    }       
                }
                var suggestedRecipes = new List<int>();
                var suggestedRecipesCounts = new List<int>();
                var suggestedRecipesDict = new Dictionary<int,int>();
                foreach (var user in usersLikedRecipes){
                    var recipes = db.RecipeLike.Where(u => u.UserId == user).Select(r => r.RecipeId).ToList();
                    foreach(var recipe in recipes){
                        if((recipe == likedRecipes.Find(r => r == recipe)))
                        {
                            continue;
                        }
                        else if(recipe == suggestedRecipes.Find(r => r == recipe))
                        {
                            suggestedRecipesCounts[suggestedRecipes.FindIndex(r => r == recipe)] += 1;
                        }
                        else 
                        {
                            suggestedRecipes.Add(recipe);
                            suggestedRecipesCounts.Add(1);
                        }
                    }
                }
                for(var i = 0; i < suggestedRecipes.Count; i++)
                {
                    suggestedRecipesDict.Add(suggestedRecipes[i],suggestedRecipesCounts[i]);
                }

                var suggestedRecipesList = suggestedRecipesDict.ToList();
                suggestedRecipesList.Sort((pair1,pair2) => pair2.Value.CompareTo(pair1.Value));
                
                var SuggestedRecipesInfo = new List<CreateRecipeViewModel>();
                var count = 5;
                if (suggestedRecipesList.Count < 5){
                    count = SuggestedRecipesInfo.Count;
                }
                for(var i = 0; i < count; i++){
                    SuggestedRecipesInfo.Add(db.Recipe.Where(r => r.Id == suggestedRecipesList[i].Key).FirstOrDefault());
                }
                ViewBag.SuggestedRecipes = SuggestedRecipesInfo;
                
            }
            return View();

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

                var commentObject = db.RecipeComment.Where(c => c.RecipeId == RecipeId).ToList();
                var usernames = new List<String>();
                foreach (var item in commentObject)
                {
                    usernames.Add(db.AspNetUsers.Where(u => u.Id == item.UserId).Select(s => s.Name).FirstOrDefault());
                }
                ViewBag.UserNames = usernames;
                ViewBag.Comments = commentObject;

                var likeCount = db.RecipeLike.Where(r => r.RecipeId == RecipeId).ToList();
                ViewBag.Likes = likeCount.Count;

                var DislikeCount = db.RecipeDislike.Where(r => r.RecipeId == RecipeId).ToList();
                ViewBag.Dislikes = DislikeCount.Count;

                if (recipeUser.PrivateAccount == 1) { 
                    if (_userManager.GetUserId(User) != recipeObject.UserId) {
                        var subscriptionObject = db.Subscription.Any(f => f.FollowerUserID == _userManager.GetUserId(User) && f.FollowedUserID == recipeObject.UserId && f.Status==1);
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

        [HttpPost]
        public IActionResult Like(int recipeID)
        {
            using (var db = new DatabaseContext())
            {
                var check = db.RecipeLike.Where(r => r.RecipeId == recipeID && r.UserId == _userManager.GetUserId(User)).ToList();
                if (check.Count == 0)
                {
                    var recipeId = new RecipeLikeViewModel
                    {
                        UserId = _userManager.GetUserId(User),
                        RecipeId = recipeID
                    };
                    db.RecipeLike.Add(recipeId);
                }
                else
                {
                    db.RecipeLike.Remove(check[0]);
                }
                db.SaveChanges();
            }
            return RedirectToAction("RecipeDetail", new { id = recipeID });
        }

        [HttpPost]
        public IActionResult Dislike(int recipeID)
        {
            using (var db = new DatabaseContext())
            {
                var check = db.RecipeDislike.Where(r => r.RecipeId == recipeID && r.UserId == _userManager.GetUserId(User)).ToList();
                if (check.Count == 0)
                {
                    var recipeId = new RecipeDislikeViewModel
                    {
                        UserId = _userManager.GetUserId(User),
                        RecipeId = recipeID
                    };
                    db.RecipeDislike.Add(recipeId);
                }
                else
                {
                    db.RecipeDislike.Remove(check[0]);
                }
                db.SaveChanges();
            }
            return RedirectToAction("RecipeDetail", new { id = recipeID });
        }


        [HttpPost]
        public IActionResult DeleteRecipe(int RecipeId)
        {
            using (var db = new DatabaseContext())
            {

                var recipe = db.Recipe.Where(u => u.Id == RecipeId).FirstOrDefault();
                if (_userManager.GetUserId(User) == recipe.UserId) {
                db.Recipe.Remove(recipe);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Recipe");
            
        }




    }
}