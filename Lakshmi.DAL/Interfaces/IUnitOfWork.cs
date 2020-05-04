using Lakshmi.DAL.Identity;
using System;
using System.Threading.Tasks;
using Lakshmi.DAL.Entities;

namespace Lakshmi.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; }
        IClientManager ClientManager { get; }
        ApplicationRoleManager RoleManager { get; }
        
        IRepository<Photo> Photos { get; }
        IRepository<Comment> Comments { get; }
        ILikeRepository Likes { get; }
        void Save();
        Task SaveAsync();
    }
}
