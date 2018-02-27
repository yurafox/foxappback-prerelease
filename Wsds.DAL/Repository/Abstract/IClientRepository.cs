using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IClientRepository
    {
        IEnumerable<Client_DTO> Clients { get; }
        Client_DTO Client(long id);
        IEnumerable<Client_DTO> GetClientByUserID(long userId);
        IEnumerable<Client_DTO> GetClientByEmail(string email);
        PersonInfo_DTO GetPersonById(long idPerson);
    }
}
