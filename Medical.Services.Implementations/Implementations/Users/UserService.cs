using Medical.Domain.Responses;
using Medical.Mail;
using Medical.Repositories.Repository;
using Medical.Service.Interfaces.Dtos.Account;
using Medical.Service.Interfaces.Interfaces.Users;
using MedicalMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace Medical.Services.Implementations.Implementations.Users;

public class UserService : IUserService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(IBaseRepository<User> userRepository, IHttpContextAccessor contextAccessor)
    {
        _userRepository = userRepository;
        _contextAccessor = contextAccessor;
    }

    public async Task<IBaseResponse<ICollection<User>>> GetAllAdmins()
    {
        try
        {
            var data = await _userRepository.GetAll().Where(x => !x.IsDeleted).ToListAsync();
            if (data == null)
            {
                return new BaseResponse<ICollection<User>>
                {
                    Description = "Users == null",
                    StatusCode = Domain.Enums.StatusCode.Ok
                };
            }
            Log.Information("GetAllAdmins UserService Success!!!");
            return new BaseResponse<ICollection<User>>
            {
                Data = data,
                Description = "Users have been found",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("GetAllAdmins UserService Error!!!", ex.Message);

            return new BaseResponse<ICollection<User>>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }



    public async Task<IBaseResponse<User>> LogIn(AccountDto account)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.UserEmail == account.UserEmail && !x.IsDeleted);
            if (user == null)
            {
                Log.Warning("User not exist!!");
                return new BaseResponse<User>
                {
                    Description = "Username or Password is wrong",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }

            if (account.UserPassword != user.UserPassword)
            {
                Log.Warning("Password is wrong!!");

                return new BaseResponse<User>
                {
                    Description = "Username or Password is wrong",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.UserEmail)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true
            };

            await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            Log.Information("LogIn UserService Success!!!");
            return new BaseResponse<User>
            {
                Description = "Login successful",
                StatusCode = Domain.Enums.StatusCode.Ok,
                Data = user
            };
        }
        catch (Exception ex)
        {
            Log.Error("LogIn UserService Error!!!", ex.Message);
            return new BaseResponse<User>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }


    public async Task<IBaseResponse<User>> Register(AccountDto account)
    {
        try
        {
            if (account == null)
            {
                Log.Warning("Account is null - UserService!!!");

                return new BaseResponse<User>
                {
                    Description = "Account is null",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }

            var user = new User
            {
                CreateAt = DateTime.Now,
                UserEmail = account.UserEmail,
                UserPassword = account.UserPassword
            };

            await _userRepository.Create(user);

            var mailService = new MailService();
            string emailSubject = "Welcome to Telesales Admin!";
            string emailMessage = "Welcome to Telesales Admin! We're excited to have you onboard.";
            string fromEmail = "hacibalaev.azik@mail.ru";
            string toEmail = account.UserEmail; 

            await mailService.Send(fromEmail, toEmail, emailMessage, emailSubject);

            Log.Information("Register UserService Success and email sent!!!");
            return new BaseResponse<User>
            {
                Data = user,
                Description = "User successfully created",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Register UserService Error!!!", ex.Message);

            return new BaseResponse<User>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }


    public async Task<IBaseResponse<User>> RemoveUser(int id)
    {
        try
        {
            var data = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                Log.Warning("User not found - UserService!!!");
                return new BaseResponse<User>
                {

                    Description = "Account is null",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }
            data.IsDeleted = true;

            await _userRepository.Update(data);

            Log.Information("RemmoveUser success UserService Error!!!");
            return new BaseResponse<User>
            {
                Data = data,
                Description = "User successfully created",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("RemoveUser UserService Error!!!", ex.Message);
            return new BaseResponse<User>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }
}
