using Medical.Domain.Entities;
using Medical.Domain.Responses;
using Medical.Service.Interfaces.Dtos.Categories;

namespace Medical.Service.Interfaces.Interfaces.Categories;

public interface ICategoryService
{
    Task<IBaseResponse<ICollection<Category>>> GetAll();
    Task<IBaseResponse<Category>> GetById(int id);
    Task<IBaseResponse<Category>> Update(int id, CreateCategoryDto category);
    Task<IBaseResponse<Category>> Delete(int id);
    Task<IBaseResponse<Category>> Create(CreateCategoryDto category);
}
