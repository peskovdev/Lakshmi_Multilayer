using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lakshmi.BLL.Interfaces;
using Lakshmi.DAL.Repositories;

namespace Lakshmi.BLL.Services
{
    public class ServiceCreator : IServiceCreator
    {
        public static object UserManager { get; internal set; }

        public IUserService CreateUserService(string connection)
        {
            return new UserService(new UnitOfWork(connection));
        }
    }
}
