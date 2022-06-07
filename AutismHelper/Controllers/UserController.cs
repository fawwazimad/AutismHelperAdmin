using AutismHelper.Models;
using AutismHelper.ViewModels;
using Firebase.Auth;
using Firebase.Storage;
//using FireSharp.Config;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutismHelper.Controllers
{
    public class UserController : Controller
    {
        private readonly IHostEnvironment _env;
        private FirestoreDb _db;
        FirebaseAuthProvider authh;
        public UserController(IHostEnvironment env)
        {
            _env = env;
            string path = AppDomain.CurrentDomain.BaseDirectory + @"autismhelperdatabase-firebase-adminsdk-nl68x-807c779e2f.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            _db = FirestoreDb.Create("autismhelperdatabase");
            authh = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDqwuXHA_e6JjC-JdaT5XAVaURL9fHwmJ8"));
        }

        
        // GET: UserController
        public async Task<ActionResult> Index()
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.User>();
            var user = _db.Collection("User");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    
                    string json = JsonConvert.SerializeObject(city);
                    Models.User newuser = JsonConvert.DeserializeObject<Models.User>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }
            var x = new MainVM
            {
                UserList = users,
            };
            return View(x);
        }

        // GET: UserController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.User>();
            var user = _db.Collection("User");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Models.User newuser = JsonConvert.DeserializeObject<Models.User>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }

            var x = new MainVM
            {
                UserRecord = users.FirstOrDefault(s => s.IDD == id),
            };



            return View(x);
        }

        // GET: UserController/Create 
        public ActionResult Create()
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MainVM collection)
        {

            try
            {

                byte[] b;
                using (BinaryReader br = new BinaryReader(collection.file.OpenReadStream()))
                {
                    b = br.ReadBytes((int)collection.file.OpenReadStream().Length);

                }
                var stream = new MemoryStream(b);

                var s = Guid.NewGuid().ToString();
                var task = new FirebaseStorage("autismhelperdatabase-cec0f.appspot.com")
                 .Child(s).PutAsync(stream);


                task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

                var downloadUrl = await task;

                collection.UserRecord.ProfilePictureURL = downloadUrl;
                CollectionReference colRef = _db.Collection("User");

                



                var x = new
                {
                    Name = collection.UserRecord.Name,
                    
                    
                    Email = collection.UserRecord.Email,
                    ProfilePictureURL = collection.UserRecord.ProfilePictureURL,
                    Password = collection.UserRecord.Password
                };
                var xx = await colRef.AddAsync(x);
                var yy = xx.Id;
                var q = new
                {
                    Name = collection.UserRecord.Name,
                    IDD = yy,

                    Email = collection.UserRecord.Email,
                    ProfilePictureURL = collection.UserRecord.ProfilePictureURL,
                    Password = collection.UserRecord.Password

                };
                DocumentReference empRef = _db.Collection("User").Document(yy);
                await empRef.SetAsync(q, SetOptions.Overwrite);






                var newUser = await authh.CreateUserWithEmailAndPasswordAsync(collection.UserRecord.Email, collection.UserRecord.Password);



            }
            catch (Exception ex)
            {
                var e = ex;
                throw;
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.User>();
            var user = _db.Collection("User");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Models.User newuser = JsonConvert.DeserializeObject<Models.User>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }

            var x = new MainVM
            {
                UserRecord = users.FirstOrDefault(s => s.IDD == id),
            };



            return View(x);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, MainVM collection)
        {
            try
            {
                if (collection.file != null)
                {
                    byte[] b;
                    using (BinaryReader br = new BinaryReader(collection.file.OpenReadStream()))
                    {
                        b = br.ReadBytes((int)collection.file.OpenReadStream().Length);

                    }
                    var stream = new MemoryStream(b);

                    var s = Guid.NewGuid().ToString();
                    var task = new FirebaseStorage("autismhelperdatabase-cec0f.appspot.com")
                     .Child(s).PutAsync(stream);


                    task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

                    var downloadUrl = await task;

                    collection.UserRecord.ProfilePictureURL = downloadUrl;
                }
                
                CollectionReference colRef = _db.Collection("User");
                
                var x = new
                {
                    Name = collection.UserRecord.Name,
                    IDD = collection.UserRecord.IDD,
                    Email = collection.UserRecord.Email,
                    ProfilePictureURL = collection.UserRecord.ProfilePictureURL,
                    Password = collection.UserRecord.Password
                };
                DocumentReference empRef = _db.Collection("User").Document(collection.UserRecord.Id);
                await empRef.SetAsync(x, SetOptions.Overwrite);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.User>();
            var user = _db.Collection("User");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Models.User newuser = JsonConvert.DeserializeObject<Models.User>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }

            var x = new MainVM
            {
                UserRecord = users.FirstOrDefault(s => s.IDD == id),
            };

            return View(x);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, MainVM collection)
        {
            try
            {
                DocumentReference empRef = _db.Collection("User").Document(collection.UserRecord.Id);
                await empRef.DeleteAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
