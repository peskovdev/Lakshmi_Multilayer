using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lakshmi.DAL.Entities;

namespace Lakshmi.DAL.Interfaces
{
    public interface ILikeRepository
    {
        void Create(Like item);
        void Delete(int photoId, string userId);
        bool Check(int photoId, string userId);
    }
}
