using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lakshmi.DAL.Entities;

namespace Lakshmi.BLL.DTO
{
    public class PhotoDTO
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public byte[] Image { get; set; }
        public byte[] ImageMini { get; set; }        
        public int Likes { get; set; }
        public bool Liked { get; set; }

        public byte[] userpicMini { get; set; }        
        public string UserDtoId { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
    }
}
