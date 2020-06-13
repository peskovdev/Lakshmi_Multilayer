using Lakshmi.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lakshmi.WEB.Models
{
    
    public class UserForCheckByAdmin
    {
        public UserDTO User { get; set; }
        public List<string> Roles {get;set;}
        public List<string> RolesAll { get; set; }        
    }
}