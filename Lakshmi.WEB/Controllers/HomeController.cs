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
        public ActionResult FindPhotos(string findCapture)
        {            
            ViewBag.Capture = findCapture;
            return View(dataService.FindPhotos(findCapture, User.Identity.GetUserId()));
        }
        public ActionResult FindUsers(string fullName)
        {
            int count = 0;
            string[] partName = new string[2] { fullName, null };
            if (fullName != null && fullName != "")
                count = fullName.Count(c => c == ' ');            
            if (count != 0)
            {
                partName = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
                
            ViewBag.Name = fullName;
            return View(UserService.FindUsers(partName[0], partName[1]));
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