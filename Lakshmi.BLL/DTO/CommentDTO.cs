using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lakshmi.DAL.Entities;

namespace Lakshmi.BLL.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Signature { get; set; }//Cам коментарий

        public int PhotoDtoId { get; set; } //Вторичный ключ для айди фотки

        public string UserDtoId { get; set; }
        public string NickName { get; set; }
    }
}
