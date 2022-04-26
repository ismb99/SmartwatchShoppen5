using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository.IRepository
{
    public interface IRepository <T> where T : class // Ska hantera flera olika klasser
    {
        T FindByCondition(Expression<Func<T, bool>> filter);
        // Metod parameter motsvarar samma expression som finns i firstOrDefault medtoden som tex
        // (x=>x.id == id)
        // med firstordefault kan man söka på mer än ett id. Tex om man behöver name kan man använda firstOrDefault
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        IEnumerable<T> GetAllUsers();
        void Create(T entity);
        void Delete(T entity);
        void DeleteMany(IEnumerable<T> entity); // Ta bort flera
        T GetById(object id);
        IEnumerable<T> GetAllProducs(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetAllProducsInCart(Expression<Func<T, bool>> filter);

    }
}
