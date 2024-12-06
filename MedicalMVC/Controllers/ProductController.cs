using Medical.Domain.Enums;
using Medical.Service.Interfaces.Dtos.Products;
using Medical.Service.Interfaces.Interfaces.Categories;
using Medical.Service.Interfaces.Interfaces.Products;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMVC.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _service;
    private readonly ICategoryService _categoryService;
    public ProductController(IProductService service, ICategoryService categoryService, ILogger<ProductController> logger)
    {

        _service = service;
        _categoryService = categoryService;
    }


    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var productResponse = await _service.GetAllConfirmed(page);
        var categoryResponse = await _categoryService.GetAll();

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok && categoryResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var vm = new GetAllDto
            {
                Categories = categoryResponse.Data,
                Products = productResponse.Data,
                CurrentPage = productResponse.CurrentPage,
                TotalPages = productResponse.TotalPages,
                PageSize = productResponse.PageSize,
                TotalCount = productResponse.TotalCount
            };
            return View(vm);
        }
        return BadRequest(productResponse);
    }


    [HttpGet]
    public async Task<IActionResult> alphabet(int page = 1)
    {
        var productResponse = await _service.GetAllConfirmed(page);
        var categoryResponse = await _categoryService.GetAll();

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var data = productResponse.Data.OrderBy(x => x.Name).ToList();
            var vm = new GetAllDto
            {
                Categories = categoryResponse.Data,
                Products = data,
                CurrentPage = productResponse.CurrentPage,
                TotalPages = productResponse.TotalPages,
                PageSize = productResponse.PageSize,
                TotalCount = productResponse.TotalCount
            };
            return View(vm);
        }
        return BadRequest(productResponse);
    }


    [HttpGet]
    public async Task<IActionResult> bystate(State state)
    {
        var productResponse = await _service.GetProductsByState(state);
        var categoryResponse = await _categoryService.GetAll();

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var vm = new GetAllDto
            {
                Categories = categoryResponse.Data,
                Products = productResponse.Data,
                CurrentPage = productResponse.CurrentPage,
                TotalPages = productResponse.TotalPages,
                PageSize = productResponse.PageSize,
                TotalCount = productResponse.TotalCount
            };
            return View(vm);
        }
        return BadRequest(productResponse);
    }



    [HttpGet]
    public async Task<IActionResult> bycategory(int Id, int page = 1)
    {
        var productResponse = await _service.GetByCatAll(Id, page);
        var categoryResponse = await _categoryService.GetAll();

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var vm = new GetAllDto
            {
                Categories = categoryResponse.Data,
                Products = productResponse.Data,
                CurrentPage = productResponse.CurrentPage,
                TotalPages = productResponse.TotalPages,
                PageSize = productResponse.PageSize,
                TotalCount = productResponse.TotalCount
            };
            return View(vm);
        }
        return BadRequest(productResponse);
    }


    [HttpGet]
    public async Task<IActionResult> latest(int page = 1)
    {
        var productResponse = await _service.GetAllConfirmed(page);
        var categoryResponse = await _categoryService.GetAll();

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var data = productResponse.Data.OrderByDescending(x => x.AdminConfirmAt).ToList();
            var vm = new GetAllDto
            {
                Categories = categoryResponse.Data,
                Products = data,
                CurrentPage = productResponse.CurrentPage,
                TotalPages = productResponse.TotalPages,
                PageSize = productResponse.PageSize,
                TotalCount = productResponse.TotalCount
            };
            return View(vm);
        }
        return BadRequest(productResponse);
    }


    [HttpGet]
    public async Task<IActionResult> id(int id)
    {
        var data = await _service.GetById(id);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return View(data.Data);
        }
        return BadRequest(data.Data);
    }


    [HttpGet]
    public async Task<IActionResult> request()
    {
        var response = await _categoryService.GetAll();

        if (response.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var viewModel = new RequestDto
            {
                Categories = response.Data.ToList()
            };

            return View(viewModel);
        }
        return BadRequest(response.Data);

    }


    [HttpPost]
    public async Task<IActionResult> request([FromForm] RequestDto product)
    {
        var cat = await _categoryService.GetAll();
        product.Categories = cat.Data;
        if (!ModelState.IsValid)
            return View(product);

        var data = await _service.CreateForUsers(product);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "home");
        }
        return BadRequest(data);
    }


    [HttpPost]
    public async Task<IActionResult> remove(int id)
    {
        var cat = await _service.Delete(id);
        if (cat.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "home");
        }
        return BadRequest(cat);
    }
}
