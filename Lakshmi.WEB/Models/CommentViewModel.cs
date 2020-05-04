using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lakshmi.WEB.Models
{
    public class CommentViewModel
    {
        public string UserDtoId { get; set; }
        public int PhotoId { get; set; }
        public string Signature { get; set; }
    }
}