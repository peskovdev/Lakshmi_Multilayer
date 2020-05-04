using Lakshmi.DAL.Entities;
using Microsoft.AspNet.Identity;

namespace Lakshmi.DAL.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
                : base(store)
        {
        }
    }
}