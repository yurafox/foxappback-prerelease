using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class ClientOrderController : Controller
    {
        private readonly IOrdersRepository _repo;

        public ClientOrderController(IOrdersRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        IEnumerable<Client_Order> Get() => _repo.ClientOrders;

        [HttpGet("{id}")]
        Client_Order Get(int id) => _repo.ClientOrder(id);

        [HttpGet("SomeMethod")]
        void SomeMethodName(int id) => _repo.SomeMethod(id);

        [HttpPost]
        void SaveClientOrder(Client_Order order) => _repo.SaveClientOrder(order);

        [HttpDelete]
        void DeleteClientOrder(int id) => _repo.DeleteClientOrder(id);

    }
}