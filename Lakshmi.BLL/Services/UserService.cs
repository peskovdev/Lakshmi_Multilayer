using Lakshmi.BLL.DTO;
using Lakshmi.BLL.Infrastructure;
using Lakshmi.DAL.Entities;
using Lakshmi.DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Lakshmi.BLL.Interfaces;
using Lakshmi.DAL.Interfaces;

namespace Lakshmi.BLL.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }
        public UserService(IUnitOfWork uow)
        {   
            Database = uow;
        }
        public async Task<OperationDetails> CreateSet(UserDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email, EmailConfirmed = true };
                var result = await Database.UserManager.CreateAsync(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                // добавляем роль
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);
                // создаем профиль клиента
                ClientProfile clientProfile = new ClientProfile
                {
                    Id = user.Id,
                    NickName = userDto.NickName, //Псевдоним
                    FirstName = userDto.FirstName, //Имя
                    SecondName= userDto.SecondName, //Фамилия
                };
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Регистрация успешно пройдена", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует", "Email");
            }
        }

        public async Task<OperationDetails> Create(UserDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            IEnumerable<ClientProfile> clientProfiles = Database.ClientManager.GetAll();
            int authenticity = 0;
            foreach (var c in clientProfiles)
            {
                if (c.NickName == userDto.NickName)
                {
                    authenticity++;
                }
            }
            if (authenticity != 0) //name authentication
            {
                return new OperationDetails(false, "Пользователь с таким ником уже существует", userDto.NickName);
            }
            else if (user != null)  //email authentication
            {
                return new OperationDetails(false, "Пользователь с таким почтовым адресом уже существует", "Email");
            }
            else
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email };

                var result = await Database.UserManager.CreateAsync(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                // создаем профиль клиента
                ClientProfile clientProfile = new ClientProfile
                {
                    Id = user.Id,
                    NickName = userDto.NickName, //Псевдоним
                    FirstName = userDto.FirstName, //Имя
                    SecondName = userDto.SecondName, //Фамилия                 
                    Userpic = userDto.Userpic, //Аватарка
                    UserpicMini = userDto.UserpicMini //Облегченная Аватарка
                };
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Регистрация успешно пройдена", "");
            }
        }        
       
        public async Task<OperationDetails> UpdatePassword(string id, string oldPassword, string newPassword)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                Database.UserManager.ChangePassword(id, oldPassword, newPassword);
                await Database.SaveAsync();
                return new OperationDetails(true, "Ваш пароль был успешно изменен", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователя с таким ID не существует", "");
            }
            
        }
        public async Task<OperationDetails> Delete(string id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                Database.UserManager.Delete(user);
                await Database.SaveAsync();
                return new OperationDetails(true, "Вы успешно удалили свой Аккаунт", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователя с таким ID не существует", "");
            }
        }
        public IEnumerable<UserDTO> GetUsers()
        {
            List<UserDTO> usersfull = new List<UserDTO>();
            var users = Database.UserManager.Users.ToList();
            foreach (var user in users)
            {
                usersfull.Add(GetUser(user.Id));
            }
            return usersfull.ToList();
        }
        public IEnumerable<UserDTO> GetUsersForAdmin()
        {
            List<UserDTO> usersfull = new List<UserDTO>();
            var users = Database.UserManager.Users.ToList();
            foreach (var user in users)
            {
                usersfull.Add(GetUserForAdmin(user.Id));
            }
            return usersfull.ToList();
        }
        public IEnumerable<UserDTO> FindUsers(string firstPart, string secondPart)
        {
            List<UserDTO> users = new List<UserDTO>();
            List<string> SecondId = new List<string>();
            int checker = 0;
            for (int i = 0; i <= 5; i++)
            {
                int count = 0;
                var usersNick = GetUsers();
                switch (i)
                {
                    case 0:
                        if (firstPart != null && firstPart != "" && firstPart != " ")
                        {
                            usersNick = usersNick.Where(s => s.NickName.Contains(firstPart));
                            count++;
                            checker++;
                        }                        
                        break;
                    case 1:
                        if (firstPart != null && firstPart != "" && firstPart != " ")
                        {
                            usersNick = usersNick.Where(s => s.FirstName.Contains(firstPart));
                            count++;
                            checker++;
                        }                            
                        break;
                    case 2:
                        if (firstPart != null && firstPart != "" && firstPart != " ")
                        {
                            usersNick = usersNick.Where(s => s.SecondName.Contains(firstPart));
                            count++;
                            checker++;
                        }
                            
                        break;
                    case 3:
                        if (secondPart != null && secondPart != "" && secondPart != " ")
                        {
                            usersNick = usersNick.Where(s => s.NickName.Contains(secondPart));
                            count++;
                            checker++;
                        }
                        break;
                    case 4:
                        if (secondPart != null && secondPart != "" && secondPart != " ")
                        {
                            usersNick = usersNick.Where(s => s.FirstName.Contains(secondPart));
                            count++;
                            checker++;
                        }
                        break;
                    case 5:
                        if (secondPart != null && secondPart != "" && secondPart != " ")
                        {
                            usersNick = usersNick.Where(s => s.SecondName.Contains(secondPart));
                            count++;
                            checker++;
                        }                            
                        break;
                }
                if(count != 0)
                {                                     
                    foreach (var user in usersNick)
                    {
                        int haveDublicate = 0;
                        foreach (string id in SecondId)
                        {
                            if (id == user.Id)
                            {
                                haveDublicate++;
                            }
                        }
                        if(haveDublicate == 0)
                        {
                            users.Add(new UserDTO
                            {
                                Id = user.Id,
                                NickName = user.NickName,
                                FirstName = user.FirstName,
                                SecondName = user.SecondName,
                                UserpicMini = user.UserpicMini
                            });
                            SecondId.Add(user.Id);
                        }
                        SecondId.Distinct();
                    }
                }
            }
            if(checker == 0)
            {
                return GetUsers();
            }
            return users;            
        }
        public IEnumerable<UserDTO> FindUsersForAdmin(string searchNickName, string searchFirstName, string searchSecondName, string searchEmail, string searchId)
        {
            var users = GetUsersForAdmin();
            if (!String.IsNullOrEmpty(searchNickName))
            {
                users = users.Where(s => s.NickName.Contains(searchNickName));
            }
            if (!String.IsNullOrEmpty(searchFirstName))
            {
                users = users.Where(s => s.FirstName.Contains(searchFirstName));
            }
            if (!String.IsNullOrEmpty(searchSecondName))
            {
                users = users.Where(s => s.SecondName.Contains(searchSecondName));
            }
            if (!String.IsNullOrEmpty(searchEmail))
            {
                users = users.Where(s => s.Email.Contains(searchEmail));
            }
            if (!String.IsNullOrEmpty(searchId))
            {
                users = users.Where(s => s.Id.Contains(searchId));
            }
            return users;
        }

        //Получить юзера
        public UserDTO GetUser(string id)
        {
            ApplicationUser user = Database.UserManager.FindById(id);
            UserDTO userDto = new UserDTO
            {
                Id = user.Id,
                NickName = user.ClientProfile.NickName,
                FirstName = user.ClientProfile.FirstName,
                SecondName = user.ClientProfile.SecondName,
                Userpic = user.ClientProfile.Userpic,
                UserpicMini = user.ClientProfile.UserpicMini
            };
            return userDto;
        }
        public List<string> GetRoles(string id)
        {
            List<string> roles = new List<string>();
            var Roles = Database.UserManager.GetRoles(id);
            foreach(string role in Roles)
            {
                roles.Add(role);
            }
            return roles;
        }
        public List<string> GetRolesAll()
        {
            List<string> roles = new List<string>();
            var Roles = Database.RoleManager.Roles;
            foreach (var role in Roles)
            {
                roles.Add(role.Name);
            }
            return roles;
        }
        public void CreateRole(string userid, string roleid) //Добавление роли
        {            
            Database.UserManager.AddToRole(userid, roleid);
        }

        public void DeleteRole(string userid, string roleid) //Удаление роли
        {
            Database.UserManager.RemoveFromRole(userid, roleid);
        }
        public UserDTO GetUserForAdmin(string id)
        {
            ApplicationUser user = Database.UserManager.FindById(id);
            UserDTO userDto = new UserDTO
            {
                Id = user.Id,
                NickName = user.ClientProfile.NickName,
                FirstName = user.ClientProfile.FirstName,
                SecondName = user.ClientProfile.SecondName,
                Email = user.Email,
                Userpic = user.ClientProfile.Userpic,
                UserpicMini = user.ClientProfile.UserpicMini
            };
            return userDto;
        }

        //изменить никнейм
        public async Task<OperationDetails> UpdateNickName(string id, string nickName)
        {
            if(nickName != null) {
                int count = nickName.Count(c => c == ' ');
                if (count == 0) 
                {                    
                    IEnumerable<ClientProfile> clientProfiles = Database.ClientManager.GetAll();
                    int authenticity = 0;
                    foreach (var c in clientProfiles)
                    {
                        if (c.NickName == nickName)
                        {
                            authenticity++;
                        }
                    }
                    ClientProfile clientProfile = Database.ClientManager.Get(id);
                    if (authenticity == 0)
                    {
                        clientProfile.NickName = nickName;   //Change NickName
                        Database.ClientManager.Update(clientProfile);//save
                        await Database.SaveAsync();
                        return new OperationDetails(true, "Вы успешно поменяли свой никнейм", "");
                    }
                    else
                    {
                        return new OperationDetails(false, "Пользователь с таким никнеймом уже существует", "Name");
                    }
                }
                else
                {
                    return new OperationDetails(false, "Никнейм не должен содержать пробелов", "");
                }
            }
            else
            {
                return new OperationDetails(false, "Никнейм не был передан", "");
            }
        }
        //изменить фамилию
        public async Task<OperationDetails> UpdateSecondName(string id, string SecondName)
        {            
            int count = SecondName.Count(c => c == ' ');
            if (count == 0)
            {
                ClientProfile clientProfile = Database.ClientManager.Get(id);
                if (clientProfile != null)
                {
                    if (SecondName != null)
                        clientProfile.SecondName = SecondName;
                    Database.ClientManager.Update(clientProfile);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Вы успешно поменяли ваше имя", "");
                }
                return new OperationDetails(false, "Пользователь с таким ID не существует", "");
            }
            else
            {
                return new OperationDetails(false, "Фамилия должна состоять из одного слова", "");
            }
        }

        //изменить имя
        public async Task<OperationDetails> UpdateFirstName(string id, string FirstName)
        {            
            int count = FirstName.Count(c => c == ' ');
            if (count == 0)
            {
                ClientProfile clientProfile = Database.ClientManager.Get(id);
                if (clientProfile != null)
                {
                    if (FirstName != null)
                        clientProfile.FirstName = FirstName;
                    Database.ClientManager.Update(clientProfile);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Вы успешно поменяли ваше имя", "");
                }
                return new OperationDetails(false, "Пользователь с таким ID не существует", "");
            }
            else
                return new OperationDetails(false, "Имя должно состоять из 1 слова", "");
        }

        //сменить аву
        public async Task<OperationDetails> UpdateUserpic(string id, byte[] Userpic, byte[] UserpicMini)
        {            
            ClientProfile clientProfile = Database.ClientManager.Get(id);
            if (clientProfile != null)
            {
                if (Userpic != null)
                    clientProfile.Userpic = Userpic;
                if (UserpicMini != null) 
                    clientProfile.UserpicMini = UserpicMini;

                Database.ClientManager.Update(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Вы успешно поменяли ваш аватар", "");
            }
            return new OperationDetails(false, "Пользователь с таким ID не существует", "");
        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)//используется при авторизации
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.Password);
            // авторизуем его и возвращаем объект ClaimsIdentity
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }
        public async Task<ClaimsIdentity> ConfirmingEmail(string Token, string Email)//используется при регистрации
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = Database.UserManager.FindById(Token);
            // авторизуем его и возвращаем объект ClaimsIdentity
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.EmailConfirmed = true;
                    // добавляем роль
                    await Database.UserManager.AddToRoleAsync(user.Id, "user");
                    await Database.UserManager.UpdateAsync(user);
                    claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    return claim;
                }
            }
            return claim;
        }
        public string GetUserId(string email)
        {
            ApplicationUser user = Database.UserManager.FindByEmail(email);
            return user.Id;
        }

        public bool GetEmailConfirmed(string Email)
        {
            ApplicationUser user = Database.UserManager.FindByEmail(Email);
            return user.EmailConfirmed;
        }

        // начальная инициализация бд
        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }
            await CreateSet(adminDto);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
