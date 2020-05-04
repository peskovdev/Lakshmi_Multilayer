using Lakshmi.WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lakshmi.WEB.Filters;
using Lakshmi.BLL.Interfaces;
using Lakshmi.BLL.DTO;
using AutoMapper;
using Lakshmi.BLL.Infrastructure;
using System.IO;
using Microsoft.AspNet.Identity;

namespace Lakshmi.WEB.Controllers
{
    [MyAuthorize(Roles = "admin, moder, user")]
    public class CrudController : Controller
    {
        IDataService dataService;
        public CrudController(IDataService serv)
        {
            dataService = serv;
        }
        [HttpGet]
        public ActionResult AddPhoto()//айди для вторичного ключа(айди должен быть юзера)
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddPhoto(PhotoViewModel photo, HttpPostedFileBase uploadImage)
        {
            if (uploadImage != null && uploadImage.ContentType.Contains("image"))
            {
                byte[] imageData = null;
                //считываем следующий переданный файл в массив данных
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                // установка массива байтов
                PhotoDTO photoDto = new PhotoDTO
                {
                    UserDtoId = photo.UserDtoId,
                    Caption = photo.Caption,
                    Image = imageData
                };
                dataService.MakePhoto(photoDto);
                return RedirectToAction("Page", "Home", new { id = photoDto.UserDtoId });
            }
            return View();
        }

        [HttpPost]//добавить лайк
        public ActionResult ChangeLike(int id, string idName)
        {
            var valueLike = dataService.ChangeLikes(id, User.Identity.GetUserId());
            int countLikes = dataService.CountLikes(id);
            var likeViewModel = new LikeViewModel { Value = valueLike, Count = countLikes, IdPhoto = id, IdName = idName };
            return PartialView(likeViewModel);
        }

        [HttpGet]
        public ActionResult UpdatePhoto(int id) //изменить фото
        {
            PhotoDTO photo = dataService.GetPhoto(id);
            return PartialView(photo);
        }
        [HttpPost]
        public ActionResult UpdatePhoto(PhotoDTO photo)
        {
            dataService.ChangePhoto(photo);
            return RedirectToAction("Page", "Home", new { id = photo.UserDtoId });
        }

        [HttpGet] //deleting
        public ActionResult DeletePhoto(int id, string userId) //Удалить фото
        {
            ViewBag.userId = userId;
            return PartialView();
        }
        [HttpPost, ActionName("DeletePhoto")]
        public ActionResult DeletePhotoConfirmed(int id, string userId)
        {
            dataService.DeletePhoto(id);

            return RedirectToAction("Page", "Home", new { id = userId });
        }

        [HttpPost]
        public ActionResult FormForUpdateComment(int id) //изменить коммент
        {
            CommentDTO comment = dataService.GetComment(id);
            return PartialView(comment);
        }

        [HttpPost]
        public ActionResult UpdateComment(CommentDTO comment)
        {
            dataService.ChangeComment(comment);
            var comments = dataService.GetComments(comment.PhotoDtoId);
            return PartialView("~/views/Home/GetComments.cshtml", comments);
        }

        [HttpPost]
        public ActionResult DeleteComment(int id, int photoId)
        {
            dataService.DeleteComment(id);
            var comments = dataService.GetComments(photoId);
            return PartialView("~/views/Home/GetComments.cshtml", comments);
        }



        [HttpPost]
        public ActionResult AddComment(CommentViewModel comment)
        {
            try
            {
                var commentDto = new CommentDTO
                {
                    Signature = comment.Signature,
                    PhotoDtoId = comment.PhotoId,
                    UserDtoId = comment.UserDtoId
                };
                dataService.MakeComment(commentDto);
                var comments = dataService.GetComments(comment.PhotoId);
                return PartialView("~/views/Home/GetComments.cshtml", comments);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
            }
            return View(comment);
        }
    }
}