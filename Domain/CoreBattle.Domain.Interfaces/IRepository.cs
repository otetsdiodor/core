using CoreBattle.Domain.Core.GameDomain;
using System;
using System.Collections.Generic;

namespace CoreBattle.Domain.Interfaces
{
    public interface IRepository<T> where T : Entity
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
        void SaveChanges();
    }
}
