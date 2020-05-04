using Lakshmi.DAL.EF;
using Lakshmi.DAL.Entities;
using Lakshmi.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Lakshmi.DAL.Repositories
{
    public class ClientManager : IClientManager
    {
        public ApplicationContext Database { get; set; }
        public ClientManager(ApplicationContext db)
        {
            Database = db;
        }
        public IEnumerable<ClientProfile> GetAll()
        {
            return Database.ClientProfiles.ToList();
        }

        public ClientProfile Get(string id)
        {
            return Database.ClientProfiles.Find(id);               
        }
        public void Update(ClientProfile item)
        {           
            Database.Entry(item).State = EntityState.Modified;
            Database.SaveChanges();
        }
        public void Create(ClientProfile item)
        {
            Database.ClientProfiles.Add(item);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
