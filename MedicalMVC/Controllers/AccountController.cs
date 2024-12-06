using Medical.Service.Interfaces.Dtos.Account;
using Medical.Service.Interfaces.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMVC.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _service;

    public AccountController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(AccountDto account)
    {
        var data = await _service.LogIn(account);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("Home", "Admin");
        }

        return View(account);
    }
}
