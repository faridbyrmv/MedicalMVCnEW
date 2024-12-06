using Medical.Domain.Entities;
using Medical.Domain.Enums;
using Medical.Domain.Responses;
using Medical.Repositories.Repository;
using Medical.Service.Interfaces.Dtos.Products;
using Medical.Service.Interfaces.Interfaces.Products;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Medical.Services.Implementations.Implementations.Products;

public class ProductService : IProductService
{
    private readonly IBaseRepository<Product> _repository;
    private readonly IBaseRepository<ProductPhoto> _photoRepository;
    private readonly IBaseRepository<Category> _categoryRepository;


    public ProductService(IBaseRepository<Product> repository, IBaseRepository<ProductPhoto> photoRepository, IBaseRepository<Category> categoryRepository)
    {
        _repository = repository;
        _photoRepository = photoRepository;
        _categoryRepository = categoryRepository;
    }


    public async Task<IBaseResponse<Product>> CreateForUsers(RequestDto product)
    {
        try
        {
            if (product == null)
            {
                Log.Warning("CreateForUsers VM ProductService == null!!!");

                return new BaseResponse<Product>
                {
                    Description = "Product is null",
                    StatusCode = Domain.Enums.StatusCode.Error,
                };
            }

            var data = new Product
            {
                UserName = product.Name,
                UserEmail = product.Email,
                UserPhone = product.Phone,
                CategoryId = product.CategoryId,
                Description = product.Description,
                Owners = product.Owners,
                State = product.State
            };

            await _repository.Create(data);

            foreach (var item in product.Photos)
            {
                if (item == null || item.Length == 0) continue;

                var uploadDirectory = Path.Combine("wwwroot", "img");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                var saveFilePath = Path.Combine(uploadDirectory, fileName);

                await using (var stream = new FileStream(saveFilePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                var roomPhotoEntity = new ProductPhoto
                {
                    ProductId = data.Id,
                    CreateAt = DateTime.Now,
                    PhotoName = fileName
                };

                await _photoRepository.Create(roomPhotoEntity);
            }

            Log.Information("CreateForUsers Product success!!! -  ProductService");

            return new BaseResponse<Product>
            {
                Data = data,
                Description = $"Product: {data.UserName} has been successfully created",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {

            Log.Error("Create ProductService Errorr!!!", ex.Message);
            return new BaseResponse<Product>
            {
                Description = "Error during creating product",
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }


    public async Task<IBaseResponse<Product>> Delete(int id)
    {
        try
        {
            var data = await _repository.GetAll().SingleOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                Log.Warning("Delete ProductService not found!!!");

                return new BaseResponse<Product>
                {
                    Description = $"Product with ID{id} = NULL",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }
            var phn = await _photoRepository.GetAll().Where(x => x.ProductId == id).ToListAsync();
            foreach (var item in phn)
            {
                item.IsDeleted = true;
                await _photoRepository.Update(item);
            }
            data.IsDeleted = true;
            await _repository.Update(data);

            Log.Information("Delete Product success!!! -  ProductService");

            return new BaseResponse<Product>
            {
                Data = data,
                Description = $"Product: {data.Name} has been successfully removed",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Delate ProductService Errorr!!!", ex.Message);
            return new BaseResponse<Product>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }

    }


    public async Task<IBaseResponse<Product>> GetById(int id)
    {
        try
        {
            var data = await _repository.GetAll().SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (data == null)
            {
                Log.Warning("GetById ProductService not found!!!");

                return new BaseResponse<Product>
                {
                    Description = $"Product with ID {id} not found",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }
            data.Photos = await _photoRepository.GetAll().Where(x => x.ProductId == id).ToListAsync();
            data.Category = await _categoryRepository.GetAll().SingleOrDefaultAsync(x => x.Id == data.CategoryId);

            Log.Information("GetById Product success!!! -  ProductService");

            return new BaseResponse<Product>
            {
                Data = data,
                Description = $"Product: {data.Name} has been successfully found",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("GetById ProductService Errorr!!!", ex.Message);
            return new BaseResponse<Product>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }


    public async Task<PagedResponse<ICollection<Product>>> GetAllConfirmed(int page = 1, int pageSize = 15)
    {
        try
        {
            var totalProducts = await _repository.GetAll().CountAsync(x => !x.IsDeleted && x.IsConfirm);
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var data = await _repository.GetAll()
                          .Where(x => !x.IsDeleted && x.IsConfirm)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();

            if (data.Count == 0 || !data.Any())
            {
                return new PagedResponse<ICollection<Product>>
                {
                    Description = "No products found",
                    StatusCode = StatusCode.Ok,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalCount = totalProducts
                };
            }

            foreach (var item in data)
            {
                item.Photos = _photoRepository.GetAll().Where(x => x.ProductId == item.Id).Take(1).ToList();
                item.Category = _categoryRepository.GetAll().SingleOrDefault(x => x.Id == item.CategoryId);
            }

            Log.Information("GetAllConfirmed Product success!!! -  ProductService");

            return new PagedResponse<ICollection<Product>>
            {
                Data = data,
                Description = "Confirmed Products have been successfully found",
                StatusCode = StatusCode.Ok,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalProducts
            };
        }
        catch (Exception ex)
        {
            Log.Error("GetAllConfirmed ProductService Errorr!!!", ex.Message);
            return new PagedResponse<ICollection<Product>>
            {
                Description = ex.Message,
                StatusCode = StatusCode.Error
            };
        }
    }


    public async Task<PagedResponse<ICollection<Product>>> GetByCatAll(int id, int page = 1, int pageSize = 15)
    {
        try
        {
            var totalProducts = await _repository.GetAll().CountAsync(x => !x.IsDeleted && x.IsConfirm && x.CategoryId == id);
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            var data = await _repository.GetAll()
                         .Where(x => !x.IsDeleted && x.IsConfirm && x.CategoryId == id)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToListAsync();

            if (data.Count == 0 || !data.Any())
            {
                return new PagedResponse<ICollection<Product>>
                {
                    Description = "No products found",
                    StatusCode = StatusCode.Ok,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalCount = totalProducts
                };
            }

            foreach (var item in data)
            {
                item.Photos = await _photoRepository.GetAll().Where(x => x.ProductId == item.Id).Take(1).ToListAsync();
                item.Category = await _categoryRepository.GetAll().SingleOrDefaultAsync(x => x.Id == item.CategoryId);
            }

            Log.Information("GetByCatAll Product success!!! -  ProductService");

            return new PagedResponse<ICollection<Product>>
            {
                Data = data,
                Description = "Products by Category have been successfully found",
                StatusCode = StatusCode.Ok,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalProducts
            };
        }
        catch (Exception ex)
        {
            Log.Error("GetByCatAll ProductService Errorr!!!", ex.Message);
            return new PagedResponse<ICollection<Product>>
            {
                Description = ex.Message,
                StatusCode = StatusCode.Error
            };
        }
    }

    public async Task<PagedResponse<ICollection<Product>>> GetNotConfirmed(int page = 1, int pageSize = 15)
    {
        try
        {
            var totalProducts = await _repository.GetAll().CountAsync(x => !x.IsDeleted && !x.IsConfirm);
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var data = await _repository.GetAll()
                          .Where(x => !x.IsDeleted && !x.IsConfirm)
                          .Skip((page - 1) * pageSize)?
                          .Take(pageSize)
                          .ToListAsync();

            if (data.Count == 0 || !data.Any())
            {
                return new PagedResponse<ICollection<Product>>
                {
                    Description = "No products found",
                    StatusCode = StatusCode.Ok,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalCount = totalProducts
                };
            }

            foreach (var item in data)
            {
                item.Photos = await _photoRepository.GetAll().Where(x => x.ProductId == item.Id).Take(1).ToListAsync();
                item.Category = await _categoryRepository.GetAll().SingleOrDefaultAsync(x => x.Id == item.CategoryId);
            }

            Log.Information("GetNotConfirmed Product success!!! -  ProductService");

            return new PagedResponse<ICollection<Product>>
            {
                Data = data,
                Description = "Not Confirmed(by Admin) Products have been successfully found",
                StatusCode = StatusCode.Ok,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalProducts
            };
        }
        catch (Exception ex)
        {
            Log.Error("Get Not Confirmed ProductService Errorr!!!", ex.Message);
            return new PagedResponse<ICollection<Product>>
            {
                Description = ex.Message,
                StatusCode = StatusCode.Error
            };
        }
    }


    public async Task<IBaseResponse<Product>> UpdateConfirm(ConfirmDto product)
    {
        try
        {
            var data = await _repository.GetAll().SingleOrDefaultAsync(x => x.Id == product.ID);

            if (data == null)
            {
                Log.Warning("GetById ProductService not found!!!");

                return new BaseResponse<Product>
                {
                    Description = $"Product with ID {product.ID} not found",
                    StatusCode = Domain.Enums.StatusCode.Error
                };
            }
            data.AgeGroup = product.AgeGroup;
            data.UpdateAt = DateTime.Now;
            data.Brand = product.Brand;
            data.Control = product.Control;
            data.Name = product.Name;
            data.Country = product.Country;
            data.CategoryId = product.CategoryId;
            data.AdminConfirmAt = DateTime.Now;
            data.IsConfirm = true;
            data.UserPhone = product.UserPhone;
            data.UserName = product.UserName;
            data.UserEmail = product.UserEmail;
            data.Type = product.Type;
            data.Control = product.Control;
            data.Owners = product.Owners;

            await _repository.Update(data);

            Log.Information("UpdateConfirm Product success!!! -  ProductService");

            return new BaseResponse<Product>
            {
                Data = data,
                Description = $"Product: {data.Name} has been successfully Updated and Confirmed",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Update ProductService Errorr!!!", ex.Message);
            return new BaseResponse<Product>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }


    public async Task<PagedResponse<ICollection<Product>>> GetProductsByState(State state, int page = 1, int pageSize = 15)
    {
        try
        {
            var totalProducts = await _repository.GetAll().CountAsync(p => p.State == state && !p.IsDeleted && p.IsConfirm); ;
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var data = await _repository.GetAll()
                          .Where(p => p.State == state && !p.IsDeleted && p.IsConfirm)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();

            if (data.Count == 0 || !data.Any())
            {
                return new PagedResponse<ICollection<Product>>
                {
                    Description = "No products found",
                    StatusCode = StatusCode.Ok,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalCount = totalProducts
                };
            }

            foreach (var item in data)
            {
                item.Photos = await _photoRepository.GetAll().Where(x => x.ProductId == item.Id).Take(1).ToListAsync();
                item.Category = await _categoryRepository.GetAll().SingleOrDefaultAsync(x => x.Id == item.CategoryId);
            }

            Log.Information("GetProductsByState Product success!!! -  ProductService");

            return new PagedResponse<ICollection<Product>>
            {
                Data = data,
                Description = "Products by State have been successfully found",
                StatusCode = StatusCode.Ok,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalProducts
            };
        }
        catch (Exception ex)
        {
            Log.Error("GetProductsByState ProductService Error!!!", ex.Message);
            return new PagedResponse<ICollection<Product>>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }
}
