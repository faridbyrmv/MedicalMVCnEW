using Medical.Domain.Entities;
using Medical.Persistence;

namespace Medical.Repositories.Repository.Requests;

public class RequestRepository : IBaseRepository<Request>
{
    private readonly ApplicationDbContext _db;

    public RequestRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<bool> Create(Request entity)
    {
        await _db.Requests.AddAsync(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public IQueryable<Request> GetAll()
    {
        return _db.Requests;
    }

    public async Task<bool> Delete(Request entity)
    {
        _db.Requests.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Request> Update(Request entity)
    {
        _db.Requests.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}
