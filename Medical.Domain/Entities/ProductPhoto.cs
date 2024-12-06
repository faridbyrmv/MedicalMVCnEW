using Medical.Domain.Entities.BaseModel;

namespace Medical.Domain.Entities;

public class ProductPhoto : Base
{
    public string PhotoName { get; set; }
    public int ProductId { get; set; }
    public Product Products { get; set; }
}
