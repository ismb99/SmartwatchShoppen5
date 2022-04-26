using Data_Access_Layer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _context.shoppingCarts.Include(p => p.Product);
            this.dbSet = _context.Set<T>();
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteMany(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }


        public T GetById(object id)
        {
            return dbSet.Find(id);
        }


        public IEnumerable<T> GetAllProducs(Expression<Func<T, bool>> filter)
        {
            var result = _context.shoppingCarts.Include(p => p.Product);
            return (IEnumerable<T>)result;
        }
        //public IEnumerable<T> GetAllProducs2(Expression<Func<T, bool>> filter)
        //{
        //    var result = _context.shoppingCarts.Include(p => p.Product).ThenInclude(x=>x.);
        //    return (IEnumerable<T>)result;
        //}
        public IEnumerable<T> GetAllProducsInCart(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList();
        }

        //public IEnumerable<T> GetAll()
        //{
        //    var result = dbSet;
        //    return result.ToList();

        //    //var result = _context.Products.Include(p => p.Category);
        //    //return (IEnumerable<T>)result;

        //    //IQueryable<T> query = dbSet;
        //    //if(filter != null)
        //    //{
        //    //query = query.Where(filter);
        //    //}
        //    //return query.ToList();
        //}

        public T FindByCondition(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAllUsers()
        {
            var users = dbSet;
            return users.ToList();
        }





        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

     
    }
}
