using AutismHelper.Models.IService;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutismHelper.Models
{
    [FirestoreData]
    public class User 
    {
        
        public string IDD { get; set; }
        public string Id { get; set; }
        public string ProfilePictureURL { get; set; }
        public string Password { get; set; }
        
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
