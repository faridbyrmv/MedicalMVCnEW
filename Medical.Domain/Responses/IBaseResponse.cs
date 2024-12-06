using Medical.Domain.Enums;

namespace Medical.Domain.Responses;

public interface IBaseResponse<T>
{
    T Data { get; set; }
    string Description { get; set; }
    StatusCode StatusCode { get; set; }
}
