using Medical.Domain.Entities;
using Medical.Persistence;

namespace Medical.Repositories.Repository.Categoriesl;

public class CategoryRepository : IBaseRepository<Category>
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<bool> Create(Category entity)
    {
        await _db.Categories.AddAsync(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public IQueryable<Category> GetAll()
    {
        return _db.Categories;
    }

    public async Task<bool> Delete(Category entity)
    {
        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Category> Update(Category entity)
    {
        _db.Categories.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}