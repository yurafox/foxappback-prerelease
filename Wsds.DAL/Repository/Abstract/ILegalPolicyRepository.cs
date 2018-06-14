using System.Collections.Generic;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ILegalPolicyRepository
    {
        string GetLegalPolicy(long idLang);
    }
}
