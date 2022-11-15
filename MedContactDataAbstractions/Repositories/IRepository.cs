using System.Linq.Expressions;
using MedContactDb.Entities;
using MedContactCore;

namespace MedContactDataAbstractions.Repositories
{
    public interface IRepository<T> where T : IBaseEntity
    {
        //READ
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetByIdTrackAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Get();

        IQueryable<T> FindBy(Expression<Func<T, bool>> searchExpression,
            params Expression<Func<T, object>>[] includes);

        //CREATE
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        //UPDATE
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        Task PatchAsync(Guid id, List<PatchModel> patchData);


        //DELETE
        void Remove(T entity);
    }
}