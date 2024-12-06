﻿namespace Medical.Repositories.Repository;

public interface IBaseRepository<T>
{
    Task<bool> Create(T entity);

    IQueryable<T> GetAll();

    Task<bool> Delete(T entity);

    Task<T> Update(T entity);
}
