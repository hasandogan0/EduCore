using EduCore.DataAccess.Abstract;

namespace EduCore.DataAccess.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    Task<int> SaveAsync();
}
