using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository.IRepository
{
    public interface IUnitOfWork // parent wrapper till de andra repositorys
    {
        IProductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository  OrderDetail { get; }
        void Save(); // save implementeras i unit of work
    }
}
