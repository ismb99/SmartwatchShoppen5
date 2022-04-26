using Data_Access_Layer.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Product = new ProductRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
        }

        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
