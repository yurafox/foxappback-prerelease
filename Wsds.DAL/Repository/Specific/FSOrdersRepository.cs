using System;
using System.Collections.Generic;
using System.Linq;
using Wsds.DAL.Entities;
using Wsds.DAL.Repository.Abstract;
using FoxStoreDBContext = Wsds.DAL.ORM.FoxStoreDBContext;

namespace Wsds.DAL.Repository.Specific
{
    public class FSOrdersRepository : IOrdersRepository
    {
        private FoxStoreDBContext _context;

        public FSOrdersRepository(FoxStoreDBContext context)
        {
            _context = context;
        }
        public IEnumerable<Client_Order> ClientOrders => _context.CLIENT_ORDERS;
        public IEnumerable<Order_Spec_Product> ClientOrdersSpecProd => _context.ORDER_SPEC_PRODUCTS;

        public void SaveClientOrder(Client_Order order)
        {
            if (order.ID == 0)
            {
                _context.CLIENT_ORDERS.Add(order);
            }
            else
            {
                Client_Order dbEntry = _context.CLIENT_ORDERS.FirstOrDefault(c => c.ID == order.ID);
                if (dbEntry != null) {
                    dbEntry.ID_CLIENT = order.ID_CLIENT;
                    dbEntry.ID_PAYMENT_METHOD = order.ID_PAYMENT_METHOD;
                    dbEntry.ID_PAYMENT_STATUS = order.ID_PAYMENT_STATUS;
                    dbEntry.ORDER_DATE = order.ORDER_DATE;
                    dbEntry.STATUS = order.STATUS;
                    dbEntry.TOTAL = order.TOTAL;
                    dbEntry.CUR = order.CUR;
                }

            };
            _context.SaveChanges();
        }

        public Client_Order ClientOrder(int id)
        {
            return _context.CLIENT_ORDERS.FirstOrDefault(o => o.ID == id);
        }

        public void SomeMethod(int param)
        {
            throw new NotImplementedException();
        }

        public void DeleteClientOrder(int Id)
        {
            Client_Order dbEntry = _context.CLIENT_ORDERS.FirstOrDefault(o => o.ID == Id);
            if (dbEntry != null)
            {
                _context.CLIENT_ORDERS.Remove(dbEntry);
                _context.SaveChanges();
            }
        }
    }
}
