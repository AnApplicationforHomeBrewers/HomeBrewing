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
                List<UserInfoViewModel> alreadySent = new List<UserInfoViewModel>();
                List<UserInfoViewModel> alreadyFriends = new List<UserInfoViewModel>();
                List<UserInfoViewModel> sendInvitation = new List<UserInfoViewModel>();
                foreach (var user in users)
                {
                    if (user.Id != _userManager.GetUserId(User)) { 
                 if (db.Subscription.Any(u=> u.FollowerUserID== _userManager.GetUserId(User) && u.FollowedUserID == user.Id && u.Status == 0 ) )  {
                            alreadySent.Add(user);
                    }

                 else if (db.Subscription.Any(u => u.FollowerUserID == _userManager.GetUserId(User) && u.FollowedUserID == user.Id && u.Status == 1))
                    {
                            alreadyFriends.Add(user);
                    }

                 else
                        {
                            sendInvitation.Add(user);

                        }

                    }
                }


                ViewBag.AlreadySent = alreadySent;
                ViewBag.AlreadyFriends = alreadyFriends;
                ViewBag.SendInvitation = sendInvitation;
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

        [HttpPost]
        public IActionResult UnFollow(string subscribedUserID)
        {
            using (var db = new DatabaseContext())
            {
                var cancelrequest = db.Subscription.Where(u => u.FollowerUserID == _userManager.GetUserId(User) && u.FollowedUserID == subscribedUserID).FirstOrDefault();

                db.Subscription.Remove(cancelrequest);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        

        [HttpPost]
        public IActionResult CancelRequest(string subscribedUserID)
        {
            using (var db = new DatabaseContext())
            {
                var cancelRequest = db.Subscription.Where(u => u.FollowerUserID == _userManager.GetUserId(User) && u.FollowedUserID == subscribedUserID).FirstOrDefault();
               
                    db.Subscription.Remove(cancelRequest);
                    db.SaveChanges();
                }
            return RedirectToAction("Index");
        }

       
        public IActionResult SeeFollow()
        {
            using (var db = new DatabaseContext())
            {
                var getFollowersId = db.Subscription.Where(u => u.FollowedUserID == _userManager.GetUserId(User) && u.Status == 1).ToList();
                var getFollowedId = db.Subscription.Where(u => u.FollowerUserID == _userManager.GetUserId(User) && u.Status == 1).ToList();
                List<UserInfoViewModel> followers = new List<UserInfoViewModel>();
                List<UserInfoViewModel> followed = new List<UserInfoViewModel>();

                foreach (var user in getFollowersId)
                {
                    UserInfoViewModel temp = db.AspNetUsers.Where(u => u.Id == user.FollowerUserID).FirstOrDefault();
                    followers.Add(temp);
                }

                foreach(var user in getFollowedId)
                {
                    UserInfoViewModel temp = db.AspNetUsers.Where(u => u.Id == user.FollowedUserID).FirstOrDefault();
                    followed.Add(temp);
                }

                ViewBag.Followers = followers;
                ViewBag.Followed = followed;
                return View();
            }
        }

        public IActionResult UserDetail()
        {
            var userId = RouteData.Values["id"].ToString();
            using (var db = new UserContext())
            {
                var userInfo = db.AspNetUsers.Where(u => u.Id == userId).FirstOrDefault();
                ViewBag.UserInfo = userInfo;
            }
            using (var db = new DatabaseContext())
            {
                var recipeInfo = db.Recipe.Where(u => u.UserId == userId).ToList();
                ViewBag.recipeInfo = recipeInfo;
            }
            return View();
        }


    }
    }

