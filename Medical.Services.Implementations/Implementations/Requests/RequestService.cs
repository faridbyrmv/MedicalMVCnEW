using Medical.Domain.Entities;
using Medical.Domain.Responses;
using Medical.Repositories.Repository;
using Medical.Service.Interfaces.Interfaces.Requests;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Medical.Services.Implementations.Implementations.Requests;

public class RequestService : IRequestService
{
    private readonly IBaseRepository<Request> _repository;
    public async Task<IBaseResponse<Request>> Create(Request request)
    {
        try
        {
            if (request == null)
            {
                return new BaseResponse<Request>
                {
                    Description = "Request is null",
                    StatusCode = Domain.Enums.StatusCode.Error,
                };
            }
            var data = new Request
            {
                CreateAt = DateTime.Now,
                Email = request.Email,
                Message = request.Message,
                Name = request.Name,
                Phone = request.Phone,
            };

            await _repository.Create(data);

            Log.Information("Create Reques success RequestService!!!");
            return new BaseResponse<Request>
            {
                Data = data,
                Description = $"Request: {data.Name} has been successfully created",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Create RequestService Error!!!", ex.Message);
            return new BaseResponse<Request>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<Request>> Delete(int id)
    {
        try
        {
            var data = await _repository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                return new BaseResponse<Request>
                {
                    Description = "Request is null",
                    StatusCode = Domain.Enums.StatusCode.Error,
                };
            }
            data.IsDeleted = true;

            await _repository.Update(data);

            Log.Information("Remove Reques success RequestService!!!");
            return new BaseResponse<Request>
            {
                Data = data,
                Description = $"Request: {data.Name} has been successfully  Removed",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("Delete RequestService Error!!!", ex.Message);
            return new BaseResponse<Request>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }

    public async Task<IBaseResponse<ICollection<Request>>> GetAll()
    {
        try
        {
            var data = await _repository.GetAll().Where(x => !x.IsDeleted).ToListAsync();
            if (data == null)
            {
                return new BaseResponse<ICollection<Request>>
                {
                    Description = "Request is null",
                    StatusCode = Domain.Enums.StatusCode.Ok,
                };
            }
            Log.Information("GetAll Reques success RequestService!!!");
            return new BaseResponse<ICollection<Request>>
            {
                Data = data,
                Description = $"Requests (Count: {data.Count}) has been successfully Found",
                StatusCode = Domain.Enums.StatusCode.Ok
            };
        }
        catch (Exception ex)
        {
            Log.Error("GetAll RequestService Error!!!", ex.Message);

            return new BaseResponse<ICollection<Request>>
            {
                Description = ex.Message,
                StatusCode = Domain.Enums.StatusCode.Error
            };
        }
    }
}
