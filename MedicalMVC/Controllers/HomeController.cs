using Medical.Domain.Entities;
using Medical.Service.Interfaces.Dtos.Products;
using Medical.Service.Interfaces.Interfaces.Products;
using Medical.Service.Interfaces.Interfaces.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMVC.Controllers;

public class HomeController : Controller
{

    private readonly IRequestService _requestService;
    private readonly IProductService _productService;
    public HomeController(IRequestService requestService, IProductService productService, ILogger<HomeController> logger)
    {
        _requestService = requestService;
        _productService = productService;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var productResponse = await _productService.GetAllConfirmed();
        var second = await _productService.GetProductsByState(Medical.Domain.Enums.State.İkinciƏl);

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var latestConfirmed = productResponse.Data?
                .OrderByDescending(x => x.AdminConfirmAt)
                .Take(4)
                .ToList();

            var secondHard = second.Data?.Take(4).ToList();

            var vm = new IndexHomeDto
            {
                LatestProduct = latestConfirmed,
                SecondHandProduct = secondHard,
            };
            return View(vm);
        }
        return BadRequest(productResponse);
    }


    public async Task<IActionResult> contact()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> contact(Request request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var data = await _requestService.Create(request);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("Index");
        }
        return BadRequest(data.Data);
    }

}
