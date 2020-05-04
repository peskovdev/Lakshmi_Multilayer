using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lakshmi.DAL.Entities
{
    public class ClientProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        
        public string FirstName{ get; set; }//имя
        public string SecondName { get; set; }//фамилия
        public string NickName { get; set; }//Псевдоним
        public byte[] Userpic { get; set; }
        public byte[] UserpicMini { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}