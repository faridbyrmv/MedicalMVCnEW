using Medical.Service.Interfaces.Dtos.Categories;
using Medical.Service.Interfaces.Interfaces.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMVC.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> index()
    {
        var data = await _categoryService.GetAll();

        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return View(data.Data);
        }
        return BadRequest(data);

    }



    [Authorize]
    public async Task<IActionResult> newcategory()
    {
        return View();
    }



    [Authorize]
    [HttpPost]
    public async Task<IActionResult> newcategory(CreateCategoryDto category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }
        var data = await _categoryService.Create(category);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "home");
        }
        return BadRequest(data);
    }


    [Authorize]
    public async Task<IActionResult> remove(int id)
    {
        var data = await _categoryService.Delete(id);
        if (data.StatusCode == Medical.Domain.Enums.StatusCode.Ok)
        {
            return RedirectToAction("index", "home");
        }
        return BadRequest(data);
    }
}
