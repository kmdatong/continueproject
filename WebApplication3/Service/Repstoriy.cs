using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Domains;

namespace WebApplication3
{
    public class Repstoriy<T> : IRepstoriy<T> where T:class
    {
        private readonly SMContext _context;

        public Repstoriy(SMContext context)
        {
            _context = context;
        }

        public void Delete(T t)
        {
            _context.Set<T>().Remove(t);
            _context.SaveChanges();
        }

        public T FristOrDefult(object key)
        {
            return _context.Set<T>().Find(key);
        }
        

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        
        public IQueryable<T> Quyer()
        {
            return _context.Set<T>();
        }


        public void Inserter(T t)
        {
            _context.Set<T>().Add(t);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(T t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }

    }
}
