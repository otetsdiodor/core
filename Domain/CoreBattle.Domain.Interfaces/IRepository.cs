using CoreBattle.Domain.Core.GameDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreBattle.Domain.Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        IQueryable<T> GetAll();
        T Get(Guid id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
        void SaveChanges();
    }
}
