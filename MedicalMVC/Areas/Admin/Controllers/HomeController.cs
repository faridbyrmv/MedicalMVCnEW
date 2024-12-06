using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View();
    }
}
