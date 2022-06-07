using AutismHelper.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutismHelper.ViewModels
{
    public class MainVM
    {
        public User UserRecord { get; set; }

        public List<User> UserList { get; set; }

        public IFormFile file { get; set; }

        public AboutUs AboutUsRecord { get; set; }
        public List<AboutUs> AboutUsList { get; set; }

        public Album AlbumRecord { get; set; }
        public List<Album> AlbumList { get; set; }

        public Picture PictureRecord { get; set; }
        public List<Picture> PictureList { get; set; }

        public List<ContactUs> ContactUsList { get; set; }
        public ContactUs ContactUsRecord { get; set; }
    }
}
