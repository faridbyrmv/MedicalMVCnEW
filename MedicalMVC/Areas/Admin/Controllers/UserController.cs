using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Medical.Service.Interfaces.Interfaces.Users;
using Medical.Service.Interfaces.Dtos.Account;

namespace MedicalMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> admins()
    {
        var data = await _userService.GetAllAdmins();
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return View(data.Data);
        }
        return BadRequest(data.Data);

    }


    [Authorize]
    public async Task<IActionResult> remove(int id)
    {
        var data = await _userService.RemoveUser(id);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "home");
        }
        return BadRequest(data.Data);
    }


    public async Task<IActionResult> registrate()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> registrate(AccountDto account)
    {
        if (!ModelState.IsValid)
            return View(account);

        var data = await _userService.Register(account);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "home");
        }

        return BadRequest(data);
    }


    [HttpPost]
    public async Task<IActionResult> logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
