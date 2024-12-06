using Medical.Domain.Responses;
using Medical.Service.Interfaces.Dtos.Account;
using MedicalMVC.Models;

namespace Medical.Service.Interfaces.Interfaces.Users;

public interface IUserService
{
    Task<IBaseResponse<User>> Register(AccountDto account);
    Task<IBaseResponse<User>> LogIn(AccountDto account);
    Task<IBaseResponse<ICollection<User>>> GetAllAdmins();
    Task<IBaseResponse<User>> RemoveUser(int id);
}
