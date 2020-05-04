using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lakshmi.DAL.Entities;
using Lakshmi.DAL.EF;
using System.Data.Entity;
using Lakshmi.DAL.Interfaces;

namespace Lakshmi.DAL.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private ApplicationContext db;
        public LikeRepository(ApplicationContext context)
        {
            this.db = context;
        }


        public bool Check(int photoId, string userId)
        {
            var first = db.Likes.FirstOrDefault((c => c.PhotoId == photoId && c.ApplicationUserId == userId));
            if (first == null)
            {
                return false;
            }
            return true;
        }
        public void Create(Like like)
        {
            db.Likes.Add(like);
        }
        public void Delete(int photoId, string userId)
        {
            var first = db.Likes.Where((c => c.PhotoId == photoId)).ToList();
            var second = first.Where(c => c.ApplicationUserId == userId);
            Like newlike = new Like();
            foreach (var like in second)
            {
                newlike = like;
            }
            db.Likes.Remove(newlike);
        }
    }
}
