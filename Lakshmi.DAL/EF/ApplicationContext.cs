using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Lakshmi.DAL.Entities;

namespace Lakshmi.DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(string conectionString) 
            : base(conectionString) 
        {
        }

        public DbSet<ClientProfile> ClientProfiles { get; set; }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
    }
}
