using Medical.Domain.Entities;
using Medical.Domain.Responses;
using Medical.Repositories.Repository;
using Medical.Service.Interfaces.Dtos.Categories;
using Medical.Service.Interfaces.Interfaces.Categories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Medical.Services.Implementations.Implementations.Categories;

public class CategoryService : ICategoryService
{
    private readonly IBaseRepository<Category> _repository;
    public CategoryService(IBaseRepository<Category> repository)
    {
        _repository = repository;
    }



    public async Task<IBaseResponse<Category>> Create(CreateCategoryDto category)
    {
        try
        {
            if (category == null)
            {
                Log.Warning("Create VM CategoryService == null!!!");

                return new BaseResponse<Category>
                {
                    Description = "Category is null",
                    StatusCode = Domain.Enums.StatusCode.Error,
                };
            }

            var data = new Category
            {
                CreateAt = DateTime.Now,
                Name = category.Name,
            };

            await _repository.Create(data);

            Log.Information("Create Category success!!! -  CategoryService");

            return new BaseResponse<Category>
            {
                Data = data,
                Description = $"Category: {data.Name} has been successfully created",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Create CategoryService Error!!!", ex.Message);
            return new BaseResponse<Category>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }


    public async Task<IBaseResponse<Category>> Delete(int id)
    {
        try
        {
            var data = await _repository.GetAll().SingleOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                Log.Warning("Delete CategoryService not found!!!");

                return new BaseResponse<Category>
                {
                    Description = $"Category with ID{id} = NULL",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }
            data.IsDeleted = true;

            await _repository.Update(data);

            Log.Information("Delete Category success!!! -  CategoryService");

            return new BaseResponse<Category>
            {
                Data = data,
                Description = $"Category: {data.Name} has been successfully removed",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Delate CategoryService Error!!!", ex.Message);
            return new BaseResponse<Category>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }

    }


    public async Task<IBaseResponse<Category>> GetById(int id)
    {
        try
        {
            var data = await _repository.GetAll().SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (data == null)
            {
                Log.Warning("GetById CategoryService not found!!!");

                return new BaseResponse<Category>
                {
                    Description = $"Category with ID{id} = NULL",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }

            Log.Information("GetById Category success!!! -  CategoryService");

            return new BaseResponse<Category>
            {
                Data = data,
                Description = $"Category: {data.Name} has been successfully found",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("GetById CategoryService Error!!!", ex.Message);
            return new BaseResponse<Category>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }



    public async Task<IBaseResponse<ICollection<Category>>> GetAll()
    {
        try
        {
            var data = await _repository.GetAll().Where(x => !x.IsDeleted).ToListAsync();
            if (data == null)
            {
                return new BaseResponse<ICollection<Category>>
                {
                    Description = $"Category = NULL",
                    StatusCode = Domain.Enums.StatusCode.Ok
                };
            }

            Log.Information("GetAll Category success!!! -  CategoryService");

            return new BaseResponse<ICollection<Category>>
            {
                Data = data,
                Description = $"Category have been successfully found",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Get All CategoryService Error!!!", ex.Message);
            return new BaseResponse<ICollection<Category>>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }



    public async Task<IBaseResponse<Category>> Update(int id, CreateCategoryDto category)
    {
        try
        {
            var data = await _repository.GetAll().SingleOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                Log.Warning("Update CategoryService not found!!!");
                return new BaseResponse<Category>
                {
                    Description = $"Category with ID{id} = NULL",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }
            data.UpdateAt = DateTime.Now;
            data.Name = category.Name;

            await _repository.Update(data);

            Log.Information("GetAll Category success!!! -  CategoryService");

            return new BaseResponse<Category>
            {
                Data = data,
                Description = $"Category: {data.Name} has been successfully Update",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Update CategoryService Error!!!", ex.Message);
            return new BaseResponse<Category>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }
}