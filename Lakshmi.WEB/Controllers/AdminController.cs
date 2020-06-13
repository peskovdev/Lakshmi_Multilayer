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
using System;
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

        public ActionResult Index(string searchNickName, string searchFirstame, string searchSecondName, string searchEmail, string searchId)
        {            
            ViewBag.searchNickName = searchNickName;
            ViewBag.searchName = searchFirstame;
            ViewBag.searchSecondName = searchSecondName;
            ViewBag.searchEmail = searchEmail;
            ViewBag.searchId = searchId;
            return View(UserService.FindUsersForAdmin(searchNickName, searchFirstame, searchSecondName, searchEmail, searchId));
        }
        public ActionResult GetInfo(string id) //управление ролями
        {
            UserForCheckByAdmin FullUser = new UserForCheckByAdmin()
            {
                User = UserService.GetUser(id),
                Roles = UserService.GetRoles(id),
                RolesAll = UserService.GetRolesAll()
            };
            if (id == null)
            {
                return HttpNotFound();
            }
            return View(FullUser);
        }
        public ActionResult CreateRole(string userid, string roleid) //Добавление роли
        {
            UserService.CreateRole(userid, roleid);
            return RedirectToAction("GetInfo", "Admin", new { id = userid });
        }

        public ActionResult DeleteRole(string userid, string roleid) //Удаление роли
        {
            UserService.DeleteRole(userid, roleid);
            return RedirectToAction("GetInfo", "Admin", new { id = userid });
        }

    }
}