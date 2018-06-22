using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3
{
    public interface IRepstoriy<T> where T:class
    {
        void Inserter(T t);

        void Update(T t);

        IQueryable<T> Quyer();

        T GetById(int id);

        T FristOrDefult(object key);

        void Delete(T t);

        void Save();
    }
}
