using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IOrdersRepository
    {
        IEnumerable<Client_Order> ClientOrders { get; }
        Client_Order ClientOrder(int id);
        void SomeMethod(int param);
        void SaveClientOrder(Client_Order order);
        void DeleteClientOrder(int Id);
    }
}
