using Medical.Domain.Enums;

namespace Medical.Domain.Responses;

public class BaseResponse<T> : IBaseResponse<T>
{
    public T Data { get; set; }
    public string Description { get; set; }
    public StatusCode StatusCode { get; set; }
}
