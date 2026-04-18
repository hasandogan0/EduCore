using EduCore.DataAccess.Abstract;
using EduCore.DataAccess.Concrete;
using EduCore.DataAccess.Data;

namespace EduCore.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly EduCoreDbContext _context;

    public UnitOfWork(EduCoreDbContext context)
    {
        _context = context;
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        return new EfGenericRepository<T>(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
