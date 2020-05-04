using System;
using System.Collections.Generic;
using Lakshmi.DAL.Entities;


namespace Lakshmi.DAL.Interfaces
{
    public interface IClientManager : IDisposable
    {
        void Create(ClientProfile item);
        IEnumerable<ClientProfile> GetAll();
        ClientProfile Get(string id);
        void Update(ClientProfile item);
    }
}
