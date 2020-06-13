using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakshmi.DAL.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public int PhotoId { get; set; } //Вторичный ключ для айди фотки
        public Photo Photo { get; set; }


        public string ApplicationUserId { get; set; } //Вторичный ключ для айди пользователя
        public ApplicationUser ApplicationUser { get; set; }
    }
}
