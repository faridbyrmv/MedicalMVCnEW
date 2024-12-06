using Medical.Service.Interfaces.Interfaces.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MedicalMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class RequestController : Controller
{
    private readonly IRequestService _requestService;
    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> index()
    {
        var data = await _requestService.GetAll();

        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return View(data.Data);
        }
        return BadRequest(data);
    }



    [Authorize]
    public async Task<IActionResult> remove(int id)
    {
        var data = await _requestService.Delete(id);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "home");
        }
        return BadRequest(data.Data);
    }
}
