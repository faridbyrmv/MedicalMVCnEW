using Medical.Service.Interfaces.Dtos.Products;
using Medical.Service.Interfaces.Interfaces.Categories;
using Medical.Service.Interfaces.Interfaces.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IProductService _service;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService service, ICategoryService categoryService)
    {
        _service = service;
        _categoryService = categoryService;
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> notconfirmed()
    {
        var productResponse = await _service.GetNotConfirmed();

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return View(productResponse.Data);
        }
        return BadRequest(productResponse);
    }



    [Authorize]
    [HttpGet]
    public async Task<IActionResult> confirm(int id)
    {
        var productResponse = await _service.GetById(id);

        if (productResponse.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            var product = productResponse.Data;
            var categoriesResponse = await _categoryService.GetAll();
            var categories = categoriesResponse.Data;
            var catbyid = await _categoryService.GetById(product.CategoryId);
            var result = catbyid.Data;
            var model = new ConfirmDto
            {
                ID = product.Id,
                UserEmail = product.UserEmail,
                Description = product.Description,
                UserName = product.UserName,
                UserPhone = product.UserPhone,
                Categories = categories,
                CategoryId = product.CategoryId,
                Category = result,
                Owners = product.Owners,
                State = product.State
            };
            return View(model);
        }
        return BadRequest(productResponse);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> confirm(ConfirmDto product)
    {
        var cat = await _categoryService.GetAll();
        var catbyid = await _categoryService.GetById(product.CategoryId);
        product.Categories = cat.Data;
        product.Category = catbyid.Data;
        if (!ModelState.IsValid)
        {
            return View(product);
        }
        var data = await _service.UpdateConfirm(product);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("NotConfirmed", "Product");
        }
        return BadRequest(data);
    }



    [Authorize]
    public async Task<IActionResult> remove(int id)
    {
        var data = await _service.Delete(id);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "Home");
        }
        return BadRequest(data);

    }
}
