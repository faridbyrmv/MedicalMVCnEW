using Medical.Domain.Entities;
using Medical.Persistence;

namespace Medical.Repositories.Repository.ProductPhotos
{
    public class ProductPhotosRepository : IBaseRepository<ProductPhoto>
    {
        private readonly ApplicationDbContext _db;

        public ProductPhotosRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(ProductPhoto entity)
        {
            await _db.Photos.AddAsync(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public IQueryable<ProductPhoto> GetAll()
        {
            return _db.Photos;
        }

        public async Task<bool> Delete(ProductPhoto entity)
        {
            _db.Photos.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ProductPhoto> Update(ProductPhoto entity)
        {
            _db.Photos.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
