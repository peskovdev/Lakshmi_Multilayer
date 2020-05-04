using Ninject.Modules;
using Lakshmi.BLL.Services;
using Lakshmi.BLL.Interfaces;

namespace Lakshmi.WEB.Util
{
    public class DataModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDataService>().To<DataService>();
        }
    }
}