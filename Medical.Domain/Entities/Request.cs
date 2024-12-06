using Medical.Domain.Entities.BaseModel;

namespace Medical.Domain.Entities;

public class Request : Base
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Message { get; set; }
}
