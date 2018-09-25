using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SynthGuru.DataAccessLayer.Repositories
{
    public interface IGenericDataRepository<T> where T : class
    {
        IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties);
        IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        int Add(params T[] items);
        int Update(params T[] items);
        int Remove(params T[] items);
    }
}