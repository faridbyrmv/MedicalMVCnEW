using Medical.Domain.Entities;
using Medical.Persistence;

namespace Medical.Repositories.Repository.Products
{
    public class ProductRepository : IBaseRepository<Product>
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(Product entity)
        {
            await _db.Products.AddAsync(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public IQueryable<Product> GetAll()
        {
            return _db.Products;
        }

        public async Task<bool> Delete(Product entity)
        {
            _db.Products.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Product> Update(Product entity)
        {
            _db.Products.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}