using AutismHelper.ViewModels;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutismHelper.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly IHostEnvironment _env;
        private FirestoreDb _db;
        public ContactUsController(IHostEnvironment env)
        {
            _env = env;
            string path = AppDomain.CurrentDomain.BaseDirectory + @"autismhelperdatabase-firebase-adminsdk-nl68x-807c779e2f.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            _db = FirestoreDb.Create("autismhelperdatabase");
        }



        public async Task<ActionResult> Index()
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var albums = new List<Models.ContactUs>();
            var album = _db.Collection("ContactUs");
            var snapshot = await album.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();

                    string json = JsonConvert.SerializeObject(city);
                    Models.ContactUs newAlbum = JsonConvert.DeserializeObject<Models.ContactUs>(json);
                    newAlbum.Id = item.Id;

                    albums.Add(newAlbum);
                }
            }
            var x = new MainVM
            {
                ContactUsList = albums
            };
            return View(x);
        }


       
        public async Task<ActionResult> Delete(string id)
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.ContactUs>();
            var user = _db.Collection("ContactUs");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Models.ContactUs newuser = JsonConvert.DeserializeObject<Models.ContactUs>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }

            var x = new MainVM
            {
                ContactUsRecord = users.FirstOrDefault(s => s.IDD == id),
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
                DocumentReference empRef = _db.Collection("ContactUs").Document(collection.ContactUsRecord.Id);
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
