using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lakshmi.DAL.Entities;
using Lakshmi.DAL.EF;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Lakshmi.DAL.Interfaces
{
    public class PhotoRepository : IRepository<Photo>
    {
        private ApplicationContext db;

        public PhotoRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Photo> GetAll()
        {
            return db.Photos.ToList();
        }

        public Photo Get(int id)
        {
            return db.Photos.Include(p => p.Likes).FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Photo> Find(Func<Photo, Boolean> predicate)
        {
            return db.Photos.Where(predicate).ToList();
        }

        public void Create(Photo photo)
        {
            db.Photos.Add(photo);
        }

        public void Update(Photo photo)
        {
            db.Entry(photo).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Photo photo = db.Photos.Find(id);
            if (photo != null)
                db.Photos.Remove(photo);
        }
    }
}
