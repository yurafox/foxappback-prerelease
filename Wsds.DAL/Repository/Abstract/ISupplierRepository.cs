using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ISupplierRepository
    {
        IEnumerable<Supplier_DTO> Suppliers { get; }
        Supplier_DTO Supplier(long id);
    }
}
