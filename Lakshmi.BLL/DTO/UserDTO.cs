using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakshmi.BLL.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }
        public string SecondName { get; set; }        
        public string FirstName { get; set; }
        public string Role { get; set; }
        public byte[] Userpic { get; set; }
        public byte[] UserpicMini { get; set; }
    }
}
