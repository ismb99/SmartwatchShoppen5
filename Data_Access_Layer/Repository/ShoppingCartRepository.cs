using Data_Access_Layer.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private  ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public int AddAmount(ShoppingCart shoppingCart, int amount)
        {
            shoppingCart.Amount += amount;
            return shoppingCart.Amount;
        }

        public int ReduceAmount(ShoppingCart shoppingCart, int amount)
        {
            shoppingCart.Amount -= amount;
            return shoppingCart.Amount;
        }
    }
}
