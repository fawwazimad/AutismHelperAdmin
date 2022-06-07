using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutismHelper.Models
{
    public class Picture
    {
        public string Id { get; set; }
        public string IDD { get; set; }
        public string URL { get; set; }
        public string Label { get; set; }
        public string AlbumID { get; set; }

        
        public string UserID { get; set; }
    }
}
