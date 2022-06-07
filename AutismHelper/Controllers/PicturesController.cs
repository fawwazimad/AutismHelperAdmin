using AutismHelper.ViewModels;
using Firebase.Storage;
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
    public class PicturesController : Controller
    {
        private readonly IHostEnvironment _env;
        private FirestoreDb _db;
        public PicturesController(IHostEnvironment env)
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
            var albums = new List<Models.Picture>();
            var album = _db.Collection("Picture");
            var snapshot = await album.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();

                    string json = JsonConvert.SerializeObject(city);
                    Models.Picture newAlbum = JsonConvert.DeserializeObject<Models.Picture>(json);
                    newAlbum.Id = item.Id;

                    albums.Add(newAlbum);
                }
            }
            var x = new MainVM
            {
                PictureList = albums
            };
            return View(x);
        }


        public async Task<ActionResult> Details(string id)
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.Picture>();
            var user = _db.Collection("Picture");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Models.Picture newuser = JsonConvert.DeserializeObject<Models.Picture>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }

            var x = new MainVM
            {
                PictureRecord = users.FirstOrDefault(s => s.IDD == id),
            };



            return View(x);
        }

        public async Task<ActionResult> Create()
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var albums = new List<Models.Album>();
            var album = _db.Collection("Album");
            var snapshot = await album.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();

                    string json = JsonConvert.SerializeObject(city);
                    Models.Album newAlbum = JsonConvert.DeserializeObject<Models.Album>(json);
                    newAlbum.Id = item.Id;

                    albums.Add(newAlbum);
                }
            }
            var x = new MainVM
            {
                AlbumList = albums
            };



            return View(x);
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

                collection.PictureRecord.URL = downloadUrl;
                CollectionReference colRef = _db.Collection("Picture");

                



                var x = new
                {

                    
                    URL = collection.PictureRecord.URL,
                    Label = collection.PictureRecord.Label,

                    AlbumID = collection.PictureRecord.AlbumID,
                    UserID = "Admin"


                };
                var xx = await colRef.AddAsync(x);
                var yy = xx.Id;
                var q = new
                {
                    IDD = yy,
                    URL = collection.PictureRecord.URL,
                    Label = collection.PictureRecord.Label,

                    AlbumID = collection.PictureRecord.AlbumID,
                    UserID = "Admin"

                };
                DocumentReference empRef = _db.Collection("Picture").Document(yy);
                await empRef.SetAsync(q, SetOptions.Overwrite);

            }
            catch (Exception ex)
            {
                var e = ex;
                throw;
            }
            return RedirectToAction(nameof(Index));

        }


        public async Task<ActionResult> Edit(string id)
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.Picture>();
            var user = _db.Collection("Picture");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Models.Picture newuser = JsonConvert.DeserializeObject<Models.Picture>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }
            var albums = new List<Models.Album>();
            var album = _db.Collection("Album");
            var snapshot1 = await album.GetSnapshotAsync();

            foreach (var item in snapshot1.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();

                    string json = JsonConvert.SerializeObject(city);
                    Models.Album newAlbum = JsonConvert.DeserializeObject<Models.Album>(json);
                    newAlbum.Id = item.Id;

                    albums.Add(newAlbum);
                }
            }
            var x = new MainVM
            {
                PictureRecord = users.FirstOrDefault(s => s.IDD == id),
                AlbumList = albums
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

                    collection.PictureRecord.URL = downloadUrl;
                }

                CollectionReference colRef = _db.Collection("Picture");

                var x = new
                {

                    IDD = collection.PictureRecord.IDD,
                    URL = collection.PictureRecord.URL,
                    Label = collection.PictureRecord.Label,
                    AlbumID = collection.PictureRecord.AlbumID,
                    UserID = "Admin"


                };
                DocumentReference empRef = _db.Collection("Picture").Document(collection.PictureRecord.Id);
                await empRef.SetAsync(x, SetOptions.Overwrite);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            var auth = HttpContext.Session.GetString("isAuth");
            if (auth == "false")
            {
                return RedirectToAction("Login", "Account");
            }
            var users = new List<Models.Picture>();
            var user = _db.Collection("Picture");
            var snapshot = await user.GetSnapshotAsync();

            foreach (var item in snapshot.Documents)
            {
                if (item.Exists)
                {
                    Dictionary<string, object> city = item.ToDictionary();
                    string json = JsonConvert.SerializeObject(city);
                    Models.Picture newuser = JsonConvert.DeserializeObject<Models.Picture>(json);
                    newuser.Id = item.Id;

                    users.Add(newuser);
                }
            }

            var x = new MainVM
            {
                PictureRecord = users.FirstOrDefault(s => s.IDD == id),
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
                DocumentReference empRef = _db.Collection("Picture").Document(collection.PictureRecord.Id);
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
