using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakshmi.DAL.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public byte[] Image { get; set; }
        public byte[] ImageMini { get; set; }

        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Photo()
        {
            Comments = new List<Comment>();
            Likes = new List<Like>();
        }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
