using Data_Access_Layer.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repository
{
    internal class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context; 
        }

        public void Update(OrderHeader orderHeader)
        {
           _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderDB = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderDB != null)
            {
                orderDB.OrderStatus = orderStatus;
                if(paymentStatus != null)
                {
                    orderDB.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string? paymentIntentId )
        {
            var orderDB = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
           
            orderDB.SessionID = sessionId;
            orderDB.PaymentIntentId = paymentIntentId;
        }
    }
}
