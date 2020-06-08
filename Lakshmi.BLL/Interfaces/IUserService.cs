using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lakshmi.BLL.DTO;
using Lakshmi.BLL.Infrastructure;

namespace Lakshmi.BLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<OperationDetails> Create(UserDTO userDto); 
        Task<OperationDetails> CreateSet(UserDTO userDto);
        Task<OperationDetails> UpdateNickName(string id, string nickName);
        Task<OperationDetails> UpdatePassword(string id, string oldPassword, string newPassword);
        Task<OperationDetails> UpdateFirstName(string id, string FullName);
        Task<OperationDetails> UpdateSecondName(string id, string FullName);
        Task<OperationDetails> UpdateUserpic(string id, byte[] Userpic, byte[] UserpicMini);
        Task<OperationDetails> Delete(string id);
        
        string GetUserId(string email);
        bool GetEmailConfirmed(string Email);
        UserDTO GetUser(string id);
        IEnumerable<UserDTO> GetUsers();
        IEnumerable<UserDTO> FindUsers(string firstPart, string secondPart);
        IEnumerable<UserDTO> FindUsersForAdmin(string searchNickName, string searchFirstName, string searchSecondName, string searchEmail, string searchId);



        Task<ClaimsIdentity> ConfirmingEmail(string Token, string Email);
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        Task SetInitialData(UserDTO adminDto, List<string> roles);
    }
}
