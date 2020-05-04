using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lakshmi.BLL.Interfaces;
using Lakshmi.BLL.DTO;
using Lakshmi.WEB.Models;
using Lakshmi.WEB.Filters;
using AutoMapper;
using Lakshmi.BLL.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace Lakshmi.WEB.Controllers
{
    [MyAuthorize(Roles = "admin, moder, user")]
    public class HomeController : Controller
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        IDataService dataService;
        public HomeController(IDataService serv)
        {
            dataService = serv;
        }

        public ActionResult Index()
        {            
            return View(dataService.GetPhotosAll(User.Identity.GetUserId()));
        }
        
        public ActionResult PhotoInfo(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var photo = dataService.GetPhoto((int)id);
            if (photo != null)
            {
                ViewBag.Comments = dataService.GetComments((int)id);
                return PartialView(photo);
            }
            return HttpNotFound();
        }
                
        public ActionResult GetComments(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var comments = dataService.GetComments((int)id);
            if (comments != null)
            {
                return PartialView(comments);
            }
            return HttpNotFound();
        }

        public ActionResult Page(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var user = UserService.GetUser(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var photos = dataService.GetPhotos(id, User.Identity.GetUserId());           
            PageViewModel page = new PageViewModel()
            {
                Photos = photos,
                User = user
            };
            return View(page);
        }        
    }
}