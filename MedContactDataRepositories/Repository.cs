﻿using System.Linq.Expressions;
using MedContactDb;
using MedContactDb.Entities;
using MedContactCore;
using MedContactDataAbstractions.Repositories;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace MedContactDataRepositories 
{
    public class Repository<T> : IRepository<T>
    where T : class, IBaseEntity
    {
        protected readonly MedContactContext Database;
        protected readonly DbSet<T> DbSet;

        public Repository(MedContactContext database)
        {
            Database = database;
            DbSet = database.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.Id.Equals(id));
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual IQueryable<T> Get()
        {
            return DbSet;
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> searchExpression,
            params Expression<Func<T, object>>[] includes)
        {
            var result = DbSet.Where(searchExpression);
            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) =>
                    current.Include(include));
            }
            return result;
        }

        public virtual async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public virtual async Task PatchAsync(Guid id, List<PatchModel> patchData)
        {
            var model = await DbSet.FirstOrDefaultAsync(entity => entity.Id.Equals(id));
            if (model != null )
            {
                Dictionary<string, object?> nameValuePropertiesPairs = new Dictionary<string, object?>();
                foreach (var patch in patchData)
                {
                    if (patch.PropertyName != null)
                        nameValuePropertiesPairs.Add(patch.PropertyName, patch.PropertyValue);
                }

                //var nameValuePropertiesPairs = patchData
                //.ToDictionary(
                //    patchModel => patchModel.PropertyName,
                //    patchModel => patchModel.PropertyValue);

                var dbEntityEntry = Database.Entry(model);
                dbEntityEntry.CurrentValues.SetValues(nameValuePropertiesPairs);
                dbEntityEntry.State = EntityState.Modified;
            }    
        }

        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}

