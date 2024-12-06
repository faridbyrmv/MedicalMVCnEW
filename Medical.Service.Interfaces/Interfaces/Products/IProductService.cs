using Medical.Domain.Entities;
using Medical.Domain.Enums;
using Medical.Domain.Responses;
using Medical.Service.Interfaces.Dtos.Products;

namespace Medical.Service.Interfaces.Interfaces.Products;

public interface IProductService
{
    Task<PagedResponse<ICollection<Product>>> GetNotConfirmed(int page = 1, int pageSize = 15);
    Task<PagedResponse<ICollection<Product>>> GetAllConfirmed(int page = 1, int pageSize = 15);
    Task<PagedResponse<ICollection<Product>>> GetByCatAll(int id, int page = 1, int pageSize = 15);
    Task<PagedResponse<ICollection<Product>>> GetProductsByState(State state, int page = 1, int pageSize = 15);



    Task<IBaseResponse<Product>> GetById(int id);
    Task<IBaseResponse<Product>> UpdateConfirm(ConfirmDto product);
    Task<IBaseResponse<Product>> Delete(int id);
    Task<IBaseResponse<Product>> CreateForUsers(RequestDto product);

}
