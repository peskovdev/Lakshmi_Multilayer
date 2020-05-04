using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lakshmi.BLL.DTO;

namespace Lakshmi.WEB.Models
{
    public class PageViewModel
    {
        public UserDTO User { get; set; }
        public IEnumerable<PhotoDTO> Photos { get; set; }
    }
}