using Medical.Domain.Entities;
using Medical.Domain.Responses;

namespace Medical.Service.Interfaces.Interfaces.Requests;

public interface IRequestService
{
    Task<IBaseResponse<ICollection<Request>>> GetAll();
    Task<IBaseResponse<Request>> Delete(int id);
    Task<IBaseResponse<Request>> Create(Request category);
}
