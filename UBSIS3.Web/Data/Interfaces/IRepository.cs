using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UBSIS3.Web.Models;

namespace UBSIS3.Web.Data.Interfaces
{
    public interface IRepository<T> where T : BaseModel
    {
        IEnumerable<T> GetAllEntities();
        IEnumerable<T> GetAllEntitiesNoTrack();
        IEnumerable<T> GetAllEntitiesWithFullJoin();

        T GetEntityById(int id);
        T GetEntityByIdWithFullJoin(int id);

        IEnumerable<T> FindEntities(Func<T, bool> predicate);
        IEnumerable<T> FindEntitiesNoTrack(Func<T, bool> predicate);
        IEnumerable<T> FindEntitiesWithFullJoin(Func<T, bool> predicate);

        bool CreateEntity(T entity);
        bool CreateEntities(IEnumerable<T> entities);

        bool UpdateEntity(T entity);
        bool UpdateEntities(IEnumerable<T> entities);

        bool DeleteEntity(T entity);
        bool DeleteEntities(IEnumerable<T> entities);

        int CountEntity(Func<T, bool> predicate);
        bool AnyEntity(Func<T, bool> predicate);
        bool AnyEntity();
    }
}
