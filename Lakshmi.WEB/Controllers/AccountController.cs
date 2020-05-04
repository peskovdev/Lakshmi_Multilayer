using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Lakshmi.WEB.Models;
using Lakshmi.BLL.DTO;
using Lakshmi.WEB.Filters;
using System.Security.Claims;
using Lakshmi.BLL.Interfaces;
using Lakshmi.BLL.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.IO;

namespace Lakshmi.Controllers
{
    [MyAuthorize(Roles = "admin, moder, user")]
    public class AccountController : Controller
    {        
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        /*позже побаловаться
         
        IUserService UserService;
        public AccountController(IUserService serv)
        {
            UserService = serv;
        }*/

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO { Email = model.Email, Password = model.Password };
                ClaimsIdentity claim = await UserService.Authenticate(userDto);

                if (claim == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    if (UserService.GetEmailConfirmed(userDto.Email)) { 
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(new AuthenticationProperties
                        {
                            IsPersistent = true
                        }, claim);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                        ModelState.AddModelError("", "Пользователь не подтвердил электронный адресс.");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model, HttpPostedFileBase uploadImage)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                byte[] imageData = null;
                if (uploadImage != null) 
                {                    
                    using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                    }
                }
                UserDTO userDto = new UserDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                    NickName = model.NickName, //Индивидуальный
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Userpic = imageData
                };
                OperationDetails operationDetails = await UserService.Create(userDto);
                if (operationDetails.Succedeed)
                {
                    // наш email с заголовком письма
                    MailAddress from = new MailAddress("lakshmi.sergey@gmail.com", "Lakshmi Registration");
                    // кому отправляем
                    MailAddress to = new MailAddress(userDto.Email);
                    // создаем объект сообщения
                    MailMessage m = new MailMessage(from, to);
                    // тема письма
                    m.Subject = "Email confirmation";
                    userDto.Id = UserService.GetUserId(userDto.Email);
                    // текст письма - включаем в него ссылку
                    m.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
                                    "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                        Url.Action("ConfirmEmail", "Account", new { Token = userDto.Id, Email = userDto.Email }, Request.Url.Scheme));
                    m.IsBodyHtml = true;
                    // адрес smtp-сервера, с которого мы и будем отправлять письмо
                    SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                    // логин и пароль
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("lakshmi.sergey@gmail.com", "veryhardpassword228");
                    smtp.Send(m);
                    return RedirectToAction("Confirm", "Account", new { Email = userDto.Email});                    
                }
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }

        [AllowAnonymous]
        public string Confirm(string Email)
        {
            return "На почтовый адрес " + Email + " Вам высланы дальнейшие" +
                    "инструкции по завершению регистрации";
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {
            ClaimsIdentity claim = await UserService.ConfirmingEmail(Token, Email);            
            if (claim != null) { 

                AuthenticationManager.SignOut();
                AuthenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true
                }, claim);
                return RedirectToAction("ChangeProfile", "Account");
            }
            return RedirectToAction("Confirm", "Account", new { Email = "" });
        }

        [HttpGet]
        public ActionResult ChangePassword() 
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePassword changePassword)
        {
            OperationDetails operationDetails = await UserService.UpdatePassword(User.Identity.GetUserId(), changePassword.OldPassword, changePassword.ConfirmPassword);
            if (operationDetails.Succedeed)
            {                
                return RedirectToAction("ChangeProfile", "Account", new { message = "Ваш пароль был успешно изменён" });
            }
            ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            return View(changePassword);
        }

        [HttpGet]
        public ActionResult ChangeProfile(string message)
        {
            ViewBag.Message = message;
            var user = UserService.GetUser(User.Identity.GetUserId());
            ChangeModel model = new ChangeModel
            {
                NickName = user.NickName,
                FirstName = user.FirstName,
                SecondName= user.SecondName,
                Userpic = user.Userpic
            };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ChangeProfile(ChangeModel model, string nick, string First, string Second, HttpPostedFileBase uploadImage) 
        {
            model.FirstName = model.FirstName.Trim();
            model.SecondName = model.SecondName.Trim();
            model.NickName = model.NickName.Trim();
            if (ModelState.IsValid)
            {
                OperationDetails operationDetailsNickName = new OperationDetails(true, "", "");
                OperationDetails operationDetailsFirstName= new OperationDetails(true, "", "");
                OperationDetails operationDetailsSecondName = new OperationDetails(true, "", "");
                OperationDetails operationDetailsUserPic = new OperationDetails(true, "", "");
                //Смена никнейма
                if (model.NickName != null && model.NickName != nick)
                {
                    operationDetailsNickName = await UserService.UpdateNickName(User.Identity.GetUserId(), model.NickName);                    
                }
                //Смена имени
                if (model.FirstName != null && model.FirstName != First)
                {
                    operationDetailsFirstName = await UserService.UpdateFirstName(User.Identity.GetUserId(), model.FirstName);
                }
                //Смена фамилии
                if (model.SecondName != null && model.SecondName != Second)
                {
                    operationDetailsSecondName = await UserService.UpdateSecondName(User.Identity.GetUserId(), model.SecondName);
                }
                //Смена авы
                if (uploadImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                    }
                    operationDetailsUserPic = await UserService.UpdateUserpic(User.Identity.GetUserId(), imageData, imageData);                    
                }

                if (operationDetailsNickName.Succedeed && operationDetailsFirstName.Succedeed && operationDetailsSecondName.Succedeed && operationDetailsUserPic.Succedeed)
                {
                    return RedirectToAction("ChangeProfile", "Account", new { message = "Изменения успешно сохранены" });
                }
                else
                {
                    ModelState.AddModelError(operationDetailsNickName.Property, operationDetailsNickName.Message);
                    ModelState.AddModelError(operationDetailsFirstName.Property, operationDetailsFirstName.Message);
                    ModelState.AddModelError(operationDetailsSecondName.Property, operationDetailsSecondName.Message);
                    ModelState.AddModelError(operationDetailsUserPic.Property, operationDetailsUserPic.Message);
                }                    
            }
            model.Userpic = UserService.GetUser(User.Identity.GetUserId()).Userpic;
            return View(model);
        }

        [HttpGet]
        public ActionResult DeleteAccount()
        {
            return View();
        }
        [HttpPost, ActionName("DeleteAccount")]
        public async Task<ActionResult> DeleteAccountConfirmed()
        {
            OperationDetails operationDetails = await UserService.Delete(User.Identity.GetUserId());
            AuthenticationManager.SignOut();
            return Content(operationDetails.Message);            
        }

        #region Вспомогательные Методы
        private async Task SetInitialDataAsync()
        {
            await UserService.SetInitialData(new UserDTO
            {
                Email = "peskov_sergei@list.ru",
                Password = "abc123456",
                NickName = "peskov",
                FirstName = "Сергей",
                SecondName = "Песков",
                Role = "admin",
            }, new List<string> { "user", "admin", "moder" });
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}