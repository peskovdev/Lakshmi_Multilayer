using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Lakshmi.WEB.Models;
using Lakshmi.BLL.DTO;
using System.Security.Claims;
using Lakshmi.BLL.Interfaces;
using Lakshmi.BLL.Infrastructure;
//using Lakshmi.WEB.Filters;
//using Lakshmi.WEB.Models;

namespace Lakshmi.WEB.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }


        public ActionResult GetUsers()
        {
            return View();
        }
    }
}