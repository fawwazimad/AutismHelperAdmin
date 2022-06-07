using AutismHelper.Models;
using AutismHelper.ViewModels;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AutismHelper.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHostEnvironment _env;
        private FirestoreDb _db;
        FirebaseAuthProvider auth;
        public AccountController(IHostEnvironment env)
        {
            _env = env;
            string path = AppDomain.CurrentDomain.BaseDirectory + @"autismhelperdatabase-firebase-adminsdk-nl68x-807c779e2f.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            _db = FirestoreDb.Create("autismhelperdatabase");
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyDqwuXHA_e6JjC-JdaT5XAVaURL9fHwmJ8"));
        }

        private async void CreateRolesandUsers()
        {
            var user = await auth.CreateUserWithEmailAndPasswordAsync("Admin@Admin.com", "Qq123456-","Admin",true);
            
        }
        
        public async Task<ActionResult> Login()
        {
            //CreateRolesandUsers();
            return View();
        }

        public class role
        {
            public string UserEmail { get; set; }
            public string roleName { get; set; }
        } 

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel entity)
        {
            if (ModelState.IsValid)
            {
                var auth1 = await auth.SignInWithEmailAndPasswordAsync(entity.Email, entity.Password);
                var user = await auth.GetUserAsync(auth1.FirebaseToken);
                HttpContext.Session.SetString("Token", auth1.FirebaseToken);
                HttpContext.Session.SetString("UserEmail", user.Email);
                ///
                //var rools = new List<Rule>();
                //var rool = _db.Collection("Rule");
                //var snapshot = await rool.GetSnapshotAsync();

                //foreach (var item in snapshot.Documents)
                //{
                //    if (item.Exists)
                //    {
                //        Dictionary<string, object> city = item.ToDictionary();
                //        string json = JsonConvert.SerializeObject(city);
                //        Rule newAboutUs = JsonConvert.DeserializeObject<Rule>(json);


                //        rools.Add(newAboutUs);
                //    }
                //}
                //var userrole = rools.FirstOrDefault(x => x.UserEmail == user.Email);

                ///
                if (user.Email.ToLower() == "Admin@Admin.com".ToLower())
                {
                    //HttpContext.Session.SetString("Role", "Admin");
                    HttpContext.Session.SetString("isAuth", "true");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email or Password");
                    return View(entity);
                }
                
                return RedirectToAction("Index", "User");
                

            }
            ModelState.AddModelError("", "Invalid Email or Password");
            return View(entity);
        }

        


        public async Task<ActionResult> LogOut()
        {
            HttpContext.Session.SetString("isAuth", "false");
            HttpContext.Session.SetString("Token", "");
            HttpContext.Session.SetString("UserEmail", "");
            return RedirectToAction("Login");
        }
    }
}
