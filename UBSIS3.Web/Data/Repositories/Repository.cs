using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UBSIS3.Web.Data.Context;
using UBSIS3.Web.Data.Interfaces;
using UBSIS3.Web.Models;

namespace UBSIS3.Web.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : BaseModel
    {
        protected  readonly ApplicationContext _context;
        protected readonly DbSet<T> _dbSet;
        private readonly IErrorLogRepository _errorRepo;


        public Repository(ApplicationContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAllEntities()
        {
            return _context.Set<T>();
        }
        public IEnumerable<T> GetAllEntitiesNoTrack()
        {
            return _context.Set<T>().AsNoTracking();
        }
        public virtual IEnumerable<T> GetAllEntitiesWithFullJoin()
        {
            throw new NotImplementedException();
        }

        public T GetEntityById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public virtual T GetEntityByIdWithFullJoin(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindEntities(Func<T, bool> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }
        public IEnumerable<T> FindEntitiesNoTrack(Func<T, bool> predicate)
        {
            return _context.Set<T>().AsNoTracking().Where(predicate);
        }
        public virtual IEnumerable<T> FindEntitiesWithFullJoin(Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public bool CreateEntity(T entity)
        {
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.Now;
            _context.Add(entity);
            return Save();
        }
        public bool CreateEntities(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }


        public bool UpdateEntity(T entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            //_context.Update(entity);
            return Save();
        }
        public bool UpdateEntities(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }


        public virtual bool DeleteEntity(T entity)
        {
            _context.Remove(entity);
            return Save();
        }
        public bool DeleteEntities(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Remove(entity);
            }
            return Save();
        }

        protected bool Save()
        {
            try
            {
                return _context.SaveChanges() > 0 ? true : false;
            }
            catch (Exception e)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorDetail = e.ToString(),
                    ErrorLocation="Repository Save method"
                });
                return false;
            }
        }
        public int CountEntity(Func<T, bool> predicate)
        {
            return _context.Set<T>().Where(predicate).Count();
        }
        public bool AnyEntity(Func<T, bool> predicate)
        {
            return _context.Set<T>().Any(predicate);
        }
        public bool AnyEntity()
        {
            return _context.Set<T>().Any();
        }

    }
}
