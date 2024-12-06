using Medical.Domain.Entities;

namespace Medical.Service.Interfaces.Dtos.Products;

public class IndexHomeDto
{
    public ICollection<Product> LatestProduct { get; set; }
    public ICollection<Product> SecondHandProduct { get; set; }
}
